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

    int currentStep = 0;

    public void Rebuild()
    {
        WireButtons();
        ResetVisuals();
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
        // Lock all
        for (int s = 0; s < steps.Count; s++)
            foreach (var node in steps[s].nodes)
                node.SetState(StageNodeUI.NodeState.Locked, false);

        // Tutorial step available
        currentStep = 0;
        SetStepState(0, StageNodeUI.NodeState.Available);

        UpdateLinks();
    }

    void SetStepState(int stepIndex, StageNodeUI.NodeState state)
    {
        if (stepIndex < 0 || stepIndex >= steps.Count) return;
        foreach (var n in steps[stepIndex].nodes)
            n.SetState(state, false);
    }

    void OnNodeClicked(int stepIndex, StageNodeUI chosen)
    {
        if (stepIndex != currentStep) return;
        if (chosen.State != StageNodeUI.NodeState.Available) return;

        // Chosen becomes cleared+selected
        chosen.SetState(StageNodeUI.NodeState.Cleared, true);

        // Others in same step get greyed out
        foreach (var n in steps[stepIndex].nodes)
            if (n != chosen) n.SetState(StageNodeUI.NodeState.GreyedOut, false);

        // Unlock next step
        int next = currentStep + 1;
        if (next < steps.Count)
        {
            SetStepState(next, StageNodeUI.NodeState.Available);
            currentStep = next;
        }

        UpdateLinks();
    }

    void UpdateLinks()
    {
        // Simple visual rule:
        // - link is bright if its "to" node is not locked/greyed out
        // - otherwise dim it
        foreach (var link in links)
        {
            if (!link) continue;
            var toNode = link.to ? link.to.GetComponent<StageNodeUI>() : null;

            bool on = toNode &&
                      toNode.State != StageNodeUI.NodeState.Locked &&
                      toNode.State != StageNodeUI.NodeState.GreyedOut;

            link.SetActive(on);
        }
    }
}