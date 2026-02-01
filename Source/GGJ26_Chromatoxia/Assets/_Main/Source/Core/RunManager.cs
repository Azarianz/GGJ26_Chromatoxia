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
        if (I != null)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);
    }

    // =====================
    // Run Lifecycle
    // =====================
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

        currentStep = Mathf.Max(currentStep, pendingNextStepAfterClear);

        currentNodeId = "";
        currentNodeSceneName = "";
    }

    public bool IsCleared(string nodeId) => clearedNodeIds.Contains(nodeId);

    // =====================
    // BOOTSTRAP DESTRUCTION
    // =====================

    /// <summary>
    /// Destroys the entire RunBootstrap root.
    /// Call this when returning to Main Menu or abandoning a run.
    /// </summary>
    public void DestroyBootstrap()
    {
        // Safety: unpause in case we died during pause
        Time.timeScale = 1f;

        // Find the top-level Bootstrap root
        Transform root = transform;
        while (root.parent != null)
            root = root.parent;

        Debug.Log($"[RunManager] Destroying Bootstrap root: {root.name}");

        Destroy(root.gameObject);

        I = null;
    }
}