// StageSelectController.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectController : MonoBehaviour
{
    [System.Serializable]
    public class Step
    {
        public List<StageNodeUI> nodes = new();
    }

    [Header("Filled by Generator")]
    public List<Step> steps = new();
    public List<StageLinkUI> links = new();

    [Header("UI")]
    public Button proceedButton;

    [Header("Graph Camera Zoom")]
    public RectTransform graphRoot;
    public RectTransform graphViewport;
    public float cameraZoomScale = 1.35f;
    public float cameraZoomDuration = 0.25f;
    public float cameraHold = 0.05f;

    StageNodeUI selectedNextNode;
    int currentStep = 1; // IMPORTANT: start AFTER the starting node

    public void Rebuild()
    {
        WireButtons();

        if (proceedButton)
        {
            proceedButton.onClick.RemoveAllListeners();
            proceedButton.onClick.AddListener(ConfirmProceed);
            proceedButton.gameObject.SetActive(false);
        }

        ApplyRunProgress();
    }

    void WireButtons()
    {
        for (int s = 0; s < steps.Count; s++)
        {
            foreach (var node in steps[s].nodes)
            {
                var btn = node.GetComponent<Button>();
                if (!btn) continue;

                btn.onClick.RemoveAllListeners();
                int stepIndex = s;
                btn.onClick.AddListener(() => OnNodeClicked(stepIndex, node));
            }
        }
    }

    public void ResetVisuals()
    {
        // Lock everything
        for (int s = 0; s < steps.Count; s++)
            foreach (var node in steps[s].nodes)
                node.SetState(StageNodeUI.NodeState.Locked, false);

        selectedNextNode = null;

        // STEP 0 = starting point, always cleared
        if (steps.Count > 0)
        {
            foreach (var node in steps[0].nodes)
                node.SetState(StageNodeUI.NodeState.Cleared, false);
        }

        // First playable step
        currentStep = Mathf.Min(1, steps.Count - 1);
        SetStepState(currentStep, StageNodeUI.NodeState.Available);

        if (proceedButton) proceedButton.gameObject.SetActive(false);

        UpdateLinks();
    }

    void SetStepState(int stepIndex, StageNodeUI.NodeState state)
    {
        if (stepIndex < 0 || stepIndex >= steps.Count) return;

        foreach (var n in steps[stepIndex].nodes)
        {
            if (!n) continue;
            if (n.State == StageNodeUI.NodeState.Cleared) continue;

            n.SetState(state, false);
        }
    }

    void OnNodeClicked(int stepIndex, StageNodeUI chosen)
    {
        if (stepIndex != currentStep) return;
        if (chosen.State != StageNodeUI.NodeState.Available) return;

        selectedNextNode = chosen;

        foreach (var n in steps[stepIndex].nodes)
            n.SetState(n.State, n == chosen);

        if (proceedButton) proceedButton.gameObject.SetActive(true);
    }

    void ConfirmProceed()
    {
        if (selectedNextNode == null) return;

        var chosen = selectedNextNode;
        selectedNextNode = null;

        if (proceedButton) proceedButton.gameObject.SetActive(false);

        if (RunManager.I != null)
        {
            int nextStepAfterClear = Mathf.Clamp(currentStep + 1, 0, steps.Count - 1);
            RunManager.I.EnterNode(chosen.nodeId, chosen.sceneName, nextStepAfterClear);
        }

        EnterNodeCameraZoom(chosen);
    }

    void EnterNodeCameraZoom(StageNodeUI node)
    {
        StartCoroutine(EnterNodeCameraZoomRoutine(node));
    }

    IEnumerator EnterNodeCameraZoomRoutine(StageNodeUI node)
    {
        if (!node || string.IsNullOrEmpty(node.sceneName))
            yield break;

        if (!graphRoot || !graphViewport)
        {
            LevelManager.I.LoadRoom(node.sceneName, true);
            yield break;
        }

        var nodeRT = node.GetComponent<RectTransform>();
        if (!nodeRT)
        {
            LevelManager.I.LoadRoom(node.sceneName, true);
            yield break;
        }

        Vector3 startScale = graphRoot.localScale;
        Vector2 startPos = graphRoot.anchoredPosition;
        Vector3 endScale = startScale * cameraZoomScale;

        Canvas canvas = graphRoot.GetComponentInParent<Canvas>();
        Camera uiCam = (canvas && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            ? canvas.worldCamera
            : null;

        Vector2 nodeScreen = RectTransformUtility.WorldToScreenPoint(uiCam, nodeRT.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            graphViewport, nodeScreen, uiCam, out Vector2 nodeInViewport);

        Vector2 viewportCenter = graphViewport.rect.center;
        Vector2 endPos = startPos + (viewportCenter - nodeInViewport) * cameraZoomScale;

        float t = 0f;
        while (t < cameraZoomDuration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Clamp01(t / cameraZoomDuration);
            graphRoot.localScale = Vector3.Lerp(startScale, endScale, a);
            graphRoot.anchoredPosition = Vector2.Lerp(startPos, endPos, a);
            yield return null;
        }

        graphRoot.localScale = startScale;
        graphRoot.anchoredPosition = startPos;

        yield return new WaitForSecondsRealtime(cameraHold);

        LevelManager.I.LoadRoom(node.sceneName, true);
    }

    void UpdateLinks()
    {
        foreach (var link in links)
        {
            if (!link) continue;

            var toNode = link.to ? link.to.GetComponent<StageNodeUI>() : null;
            bool on = toNode && toNode.State != StageNodeUI.NodeState.Locked;
            link.SetActive(on);
        }
    }

    public void ApplyRunProgress()
    {
        if (RunManager.I == null || !RunManager.I.hasRun)
        {
            ResetVisuals();
            return;
        }

        // Lock all
        for (int s = 0; s < steps.Count; s++)
            foreach (var n in steps[s].nodes)
                n.SetState(StageNodeUI.NodeState.Locked, false);

        // Step 0 always cleared (starting point)
        if (steps.Count > 0)
            foreach (var n in steps[0].nodes)
                n.SetState(StageNodeUI.NodeState.Cleared, false);

        // Cleared nodes from run
        for (int s = 1; s < steps.Count; s++)
            foreach (var n in steps[s].nodes)
                if (RunManager.I.IsCleared(n.nodeId))
                    n.SetState(StageNodeUI.NodeState.Cleared, false);

        currentStep = Mathf.Clamp(RunManager.I.currentStep, 1, steps.Count - 1);
        SetStepState(currentStep, StageNodeUI.NodeState.Available);

        if (proceedButton) proceedButton.gameObject.SetActive(false);
        UpdateLinks();
    }
}