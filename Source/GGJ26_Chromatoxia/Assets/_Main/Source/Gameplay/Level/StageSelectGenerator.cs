// StageSelectGenerator.cs
using System.Collections.Generic;
using UnityEngine;

public class StageSelectGenerator : MonoBehaviour
{
    [Header("Parents")]
    public RectTransform nodesParent;
    public RectTransform linksParent;

    [Header("Prefabs")]
    public StageNodeUI normalNodePrefab;
    public StageNodeUI bossNodePrefab;
    public StageLinkUI linkPrefab;

    [Header("Layout")]
    [Min(2)] public int totalSteps = 5;
    [Min(1)] public int minNodesPerMidStep = 2;
    [Min(1)] public int maxNodesPerMidStep = 3;

    public float xStart = -520f;
    public float xEnd = 520f;

    public float yMin = -140f;
    public float yMax = 140f;
    public float minYSpacing = 80f;

    [Header("Seed (optional)")]
    public bool useSeed = false;
    public int seed = 12345;

    [Header("References")]
    public StageSelectController controller;

    [Header("Scene Mapping")]
    public string tutorialSceneName = "TutorialLevel";
    public string bossSceneName = "Boss";
    public string[] midScenePool; // e.g. RunStart, RoomA, RoomB, RoomC

    private void Start()
    {
        Generate();
    }

    public void Generate()
    {
        if (!nodesParent || !linksParent || !normalNodePrefab || !bossNodePrefab || !linkPrefab || !controller)
        {
            Debug.LogError("StageSelectGenerator: assign parents/prefabs/controller.");
            return;
        }

        // Force deterministic seed if run exists
        if (RunManager.I != null && RunManager.I.hasRun)
        {
            useSeed = true;
            seed = RunManager.I.seed;
        }

        if (useSeed) Random.InitState(seed);

        ClearChildren(nodesParent);
        ClearChildren(linksParent);

        var steps = new List<StageSelectController.Step>();
        float stepDx = (xEnd - xStart) / (totalSteps - 1);

        for (int s = 0; s < totalSteps; s++)
        {
            int count = (s == 0 || s == totalSteps - 1)
                ? 1
                : Random.Range(minNodesPerMidStep, maxNodesPerMidStep + 1);

            float x = xStart + stepDx * s;
            var ys = GenerateYs(count, yMin, yMax, minYSpacing);

            var step = new StageSelectController.Step
            {
                nodes = new List<StageNodeUI>(count)
            };

            bool isBossStep = (s == totalSteps - 1);
            var prefab = isBossStep ? bossNodePrefab : normalNodePrefab;

            for (int i = 0; i < count; i++)
            {
                var node = Instantiate(prefab, nodesParent);
                node.name = $"Node_S{s}_N{i}";
                node.nodeId = node.name;

                var rt = (RectTransform)node.transform;
                rt.anchoredPosition = new Vector2(x, ys[i]);

                if (s == 0) node.debugLabel = "TutorialLevel";
                else if (isBossStep) node.debugLabel = "Boss";

                if (s == 0) node.sceneName = tutorialSceneName;
                else if (isBossStep) node.sceneName = bossSceneName;
                else
                {
                    if (midScenePool != null && midScenePool.Length > 0)
                        node.sceneName = midScenePool[Random.Range(0, midScenePool.Length)];
                    else
                        node.sceneName = "RunStart";
                }

                step.nodes.Add(node);
            }

            steps.Add(step);
        }

        var links = new List<StageLinkUI>();
        for (int s = 0; s < steps.Count - 1; s++)
        {
            foreach (var a in steps[s].nodes)
                foreach (var b in steps[s + 1].nodes)
                {
                    var link = Instantiate(linkPrefab, linksParent);
                    link.name = $"Link_S{s}_to_S{s + 1}_{a.name}_{b.name}";
                    link.from = (RectTransform)a.transform;
                    link.to = (RectTransform)b.transform;
                    links.Add(link);
                }
        }

        controller.steps = steps;
        controller.links = links;
        controller.Rebuild();
    }

    static void ClearChildren(RectTransform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            if (Application.isPlaying) Object.Destroy(parent.GetChild(i).gameObject);
            else Object.DestroyImmediate(parent.GetChild(i).gameObject);
        }
    }

    static List<float> GenerateYs(int count, float min, float max, float spacing)
    {
        var ys = new List<float>(count);

        int safety = 200;
        while (ys.Count < count && safety-- > 0)
        {
            float y = Random.Range(min, max);
            bool ok = true;

            for (int i = 0; i < ys.Count; i++)
                if (Mathf.Abs(ys[i] - y) < spacing) { ok = false; break; }

            if (ok) ys.Add(y);
        }

        if (ys.Count < count)
        {
            ys.Clear();
            float span = max - min;
            float step = (count == 1) ? 0f : span / (count - 1);
            for (int i = 0; i < count; i++) ys.Add(min + step * i);
        }

        ys.Sort();
        return ys;
    }
}