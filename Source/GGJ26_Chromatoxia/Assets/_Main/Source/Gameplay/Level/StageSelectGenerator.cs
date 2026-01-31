using System.Collections.Generic;
using UnityEngine;

public class StageSelectGenerator : MonoBehaviour
{
    [Header("Parents")]
    public RectTransform nodesParent;
    public RectTransform linksParent;

    [Header("Prefabs")]
    public StageNodeUI nodePrefab;
    public StageLinkUI linkPrefab;

    [Header("Layout")]
    [Min(2)] public int totalSteps = 5;          // includes tutorial + boss columns
    [Min(1)] public int minNodesPerMidStep = 2;  // between start and end
    [Min(1)] public int maxNodesPerMidStep = 3;

    public float xStart = -520f;
    public float xEnd = 520f;

    public float yMin = -140f;
    public float yMax = 140f;
    public float minYSpacing = 80f; // prevent overlap

    [Header("Seed (optional)")]
    public bool useSeed = false;
    public int seed = 12345;

    [Header("References")]
    public StageSelectController controller;

    public void Generate()
    {
        if (!nodesParent || !linksParent || !nodePrefab || !linkPrefab || !controller)
        {
            Debug.LogError("StageSelectGenerator: assign parents/prefabs/controller.");
            return;
        }

        if (useSeed) Random.InitState(seed);

        // Clear old
        ClearChildren(nodesParent);
        ClearChildren(linksParent);

        // Build steps list
        var steps = new List<StageSelectController.Step>();

        float stepDx = (xEnd - xStart) / (totalSteps - 1);

        for (int s = 0; s < totalSteps; s++)
        {
            int count;
            if (s == 0 || s == totalSteps - 1) count = 1; // tutorial + boss
            else count = Random.Range(minNodesPerMidStep, maxNodesPerMidStep + 1);

            float x = xStart + stepDx * s;
            var ys = GenerateYs(count, yMin, yMax, minYSpacing);

            var step = new StageSelectController.Step();
            step.nodes = new List<StageNodeUI>(count);

            for (int i = 0; i < count; i++)
            {
                var node = Instantiate(nodePrefab, nodesParent);
                node.name = $"Node_S{s}_N{i}";
                node.nodeId = node.name;

                var rt = (RectTransform)node.transform;
                rt.anchoredPosition = new Vector2(x, ys[i]);

                // Optional: mark tutorial/boss visually
                if (s == 0) node.debugLabel = "Tutorial";
                else if (s == totalSteps - 1) node.debugLabel = "Boss";

                step.nodes.Add(node);
            }

            steps.Add(step);
        }

        // Create links between adjacent steps (connect ALL to ALL; looks like branches)
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

        // Push into controller and refresh visuals
        controller.steps = steps;
        controller.links = links;
        controller.Rebuild();
    }

    static void ClearChildren(RectTransform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            if (Application.isPlaying) Destroy(parent.GetChild(i).gameObject);
            else DestroyImmediate(parent.GetChild(i).gameObject);
        }
    }

    static List<float> GenerateYs(int count, float min, float max, float spacing)
    {
        // Simple greedy placement: sample random y, accept if far enough from others.
        // Jam-simple and works well for 2–3 nodes/step.
        var ys = new List<float>(count);

        int safety = 200;
        while (ys.Count < count && safety-- > 0)
        {
            float y = Random.Range(min, max);
            bool ok = true;
            for (int i = 0; i < ys.Count; i++)
            {
                if (Mathf.Abs(ys[i] - y) < spacing) { ok = false; break; }
            }
            if (ok) ys.Add(y);
        }

        // If we failed due to tight range, fallback to evenly spaced
        if (ys.Count < count)
        {
            ys.Clear();
            float span = max - min;
            float step = (count == 1) ? 0f : span / (count - 1);
            for (int i = 0; i < count; i++) ys.Add(min + step * i);
        }

        ys.Sort(); // looks nicer
        return ys;
    }
}