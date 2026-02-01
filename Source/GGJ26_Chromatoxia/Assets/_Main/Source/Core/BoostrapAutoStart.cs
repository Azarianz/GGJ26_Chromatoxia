using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapAutoStart : MonoBehaviour
{
#if UNITY_EDITOR
    public bool enableKeywordJump = true;
#endif

    [Header("Dev Shortcut")]
    [Tooltip("Type a partial scene name to jump there.\nExample: 'boss' -> loads a scene that contains 'boss'.")]
    public string keyword;

    void Start()
    {
        RunManager.I.NewRun(Random.Range(0, 999999));

        string raw = keyword;
        string key = (raw ?? "").Trim();

        Debug.Log($"[BootstrapAutoStart] keyword raw='{raw}' trimmed='{key}'");

#if UNITY_EDITOR
        if (!enableKeywordJump)
        {
            Debug.Log("[BootstrapAutoStart] enableKeywordJump = false -> using default flow");
            StartDefault();
            return;
        }
#endif

        if (!string.IsNullOrEmpty(key))
        {
            // 1) Try exact
            if (Application.CanStreamedLevelBeLoaded(key))
            {
                Debug.Log($"[BootstrapAutoStart] Keyword exact match -> '{key}'");
                LevelManager.I.LoadRoom(key);
                return;
            }

            // 2) Try "contains" match against Build Settings scene names
            string found = FindSceneNameContains(key);
            if (!string.IsNullOrEmpty(found))
            {
                Debug.Log($"[BootstrapAutoStart] Keyword contains match '{key}' -> '{found}'");
                LevelManager.I.LoadRoom(found);
                return;
            }

            Debug.LogWarning($"[BootstrapAutoStart] No scene found for keyword='{key}'. Falling back to default.");
        }

        StartDefault();
    }

    void StartDefault()
    {
        if (LevelManager.startMode == BootstrapStartMode.Tutorial)
            LevelManager.I.StartTutorial();
        else
            LevelManager.I.StartRun();
    }

    string FindSceneNameContains(string key)
    {
        key = key.ToLowerInvariant();

        int count = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < count; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);

            if (name != null && name.ToLowerInvariant().Contains(key))
                return name;
        }

        return null;
    }
}