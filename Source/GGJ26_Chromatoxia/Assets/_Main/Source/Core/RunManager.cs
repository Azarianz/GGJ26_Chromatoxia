using System.Collections.Generic;
using UnityEngine;

public class RunManager : MonoBehaviour
{
    public static RunManager I { get; private set; }

    [Header("Run State")]
    public bool hasRun = false;
    public int seed = 12345;

    // This is what the StageGraph should unlock/select from RIGHT NOW
    public int currentStep = 0;

    public List<string> clearedNodeIds = new();

    [Header("Current Node")]
    public string currentNodeId;
    public string currentNodeSceneName;

    // The step that becomes selectable AFTER we clear the current node
    int pendingNextStepAfterClear = 0;

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    public void NewRun(int newSeed)
    {
        hasRun = true;
        seed = newSeed;

        currentStep = 0;
        clearedNodeIds.Clear();

        currentNodeId = "";
        currentNodeSceneName = "";
        pendingNextStepAfterClear = 0;
    }

    public void EnterNode(string nodeId, string sceneName, int nextStepAfterClear)
    {
        currentNodeId = nodeId;
        currentNodeSceneName = sceneName;
        pendingNextStepAfterClear = Mathf.Max(0, nextStepAfterClear);
    }

    public void ClearCurrentNode()
    {
        if (string.IsNullOrEmpty(currentNodeId)) return;

        if (!clearedNodeIds.Contains(currentNodeId))
            clearedNodeIds.Add(currentNodeId);

        // NOW we unlock the next selectable step
        currentStep = Mathf.Max(currentStep, pendingNextStepAfterClear);

        // optional: clear current node pointer
        currentNodeId = "";
        currentNodeSceneName = "";
    }

    public bool IsCleared(string nodeId) => clearedNodeIds.Contains(nodeId);
}