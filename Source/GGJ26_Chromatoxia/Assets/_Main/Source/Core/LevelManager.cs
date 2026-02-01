using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum BootstrapStartMode { Run, Tutorial }

public class LevelManager : MonoBehaviour
{
    public static LevelManager I { get; private set; }

    // Set this from MainMenu BEFORE loading RunBootstrap
    public static BootstrapStartMode startMode = BootstrapStartMode.Run;

    [Header("Menu Scene (Single)")]
    public string menuScene = "MainMenu";

    [Header("Rooms (Additive)")]
    public string runStartScene = "RunStart";
    public string tutorialScene = "Tutorial";

    [Header("UI (in RunBootstrap)")]
    public CanvasGroup fadeCanvas;        // optional (black image + CanvasGroup)
    public float fadeTime = 0.25f;

    public GameObject loadingCanvas;      // LoadingCanvas root
    public Slider loadingBar;             // optional
    public TMPro.TMP_Text loadingText;    // optional

    public GameObject hudCanvas;          // HUDCanvas root

    [Header("Loading")]
    public float minLoadingTime = 1f; // seconds (set to 1)

    [Header("Debug")]
    public bool logTransitions = true;

    string currentRoomScene;
    bool loading;

    public bool usingKeyword;

    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;

        // Init UI states
        SetLoadingUI(false, 0f);
        SetHudVisible(false);

        if (fadeCanvas != null)
        {
            fadeCanvas.alpha = 0f;
            fadeCanvas.blocksRaycasts = false;
            fadeCanvas.interactable = false;
        }
    }

    void Start()
    {
        if (usingKeyword)
            return;

        // When RunBootstrap scene loads, auto-start based on startMode
        if (startMode == BootstrapStartMode.Tutorial) StartTutorial();
        else StartRun();
    }

    void OnDestroy()
    {
        if (I == this) I = null;
    }

    // =========================
    // Entry Points
    // =========================
    public void StartRun()
    {
        int seed = Random.Range(int.MinValue, int.MaxValue);
        StartRun(seed);
    }

    public void StartRun(int seed)
    {
        if (loading) return;

        if (logTransitions) Debug.Log($"[LevelManager] StartRun seed={seed}");
        Random.InitState(seed);

        // Reset run-wide systems
        GameUIManager.Instance?.HideGameEnd();

        if (InventoryManager.I != null)
            InventoryManager.I.ResetInventory();

        if (GameModifiers.Instance != null)
            GameModifiers.Instance.ResetAll();


        LoadRoom(runStartScene);
    }

    public void StartTutorial()
    {
        if (loading) return;

        if (logTransitions) Debug.Log("[LevelManager] StartTutorial");

        GameUIManager.Instance?.HideGameEnd();

        if (GameModifiers.Instance != null)
            GameModifiers.Instance.ResetAll();

        if (InventoryManager.I != null)
            InventoryManager.I.ResetInventory();

        LoadRoom(tutorialScene);
    }

    public void EndToMenu()
    {
        if (loading) return;
        StartCoroutine(EndRoutine());
    }

    // =========================
    // Room Loading (Additive)
    // =========================
    public void LoadRoom(string roomScene, bool showHud = true)
    {
        if (loading) return;

        if (string.IsNullOrEmpty(roomScene))
        {
            Debug.LogError("[LevelManager] LoadRoom called with empty scene name.");
            return;
        }

        StartCoroutine(LoadRoomRoutine(roomScene, showHud));
    }

    IEnumerator LoadRoomRoutine(string nextRoom, bool showHud)
    {
        loading = true;

        float loadStartTime = Time.unscaledTime; // ⬅️ start timer

        // IMPORTANT: clear any previous game-end UI
        if (GameUIManager.Instance != null)
            GameUIManager.Instance.HideGameEnd();

        // Hide HUD while loading
        SetHudVisible(false);
        SetLoadingUI(true, 0f, $"Loading...");

        if (fadeCanvas != null) yield return Fade(1f);

        // Unload current room
        yield return UnloadRoomIfLoaded(currentRoomScene);

        // Load next room additively
        AsyncOperation load = SceneManager.LoadSceneAsync(nextRoom, LoadSceneMode.Additive);
        if (load == null)
        {
            Debug.LogError($"[LevelManager] Failed to load '{nextRoom}'.");
            SetLoadingUI(false, 0f);
            loading = false;
            yield break;
        }

        while (!load.isDone)
        {
            float p = Mathf.Clamp01(load.progress / 0.9f);
            SetLoadingUI(true, p, $"Loading...");
            yield return null;
        }

        currentRoomScene = nextRoom;

        Scene room = SceneManager.GetSceneByName(nextRoom);
        if (room.IsValid())
            SceneManager.SetActiveScene(room);

        // ⏱️ Enforce minimum loading time
        float elapsed = Time.unscaledTime - loadStartTime;
        if (elapsed < minLoadingTime)
            yield return new WaitForSecondsRealtime(minLoadingTime - elapsed);

        if (fadeCanvas != null) yield return Fade(0f);

        // UI: hide loading, show HUD
        SetLoadingUI(false, 1f);
        SetHudVisible(showHud);

        loading = false;

        if (logTransitions)
            Debug.Log($"[LevelManager] Room '{nextRoom}' loaded + active.");
    }

    IEnumerator UnloadRoomIfLoaded(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
            yield break;

        Scene s = SceneManager.GetSceneByName(sceneName);
        if (!s.IsValid() || !s.isLoaded)
            yield break;

        AsyncOperation unload = SceneManager.UnloadSceneAsync(sceneName);
        if (unload == null)
            yield break;

        while (!unload.isDone)
            yield return null;
    }

    IEnumerator EndRoutine()
    {
        loading = true;

        if (logTransitions)
            Debug.Log("[LevelManager] End -> Menu");

        // UI during end
        SetHudVisible(false);
        SetLoadingUI(true, 0f, "Returning to menu...");

        if (fadeCanvas != null) yield return Fade(1f);

        yield return UnloadRoomIfLoaded(currentRoomScene);
        currentRoomScene = null;

        // Back to menu as Single clears bootstrap entirely
        SceneManager.LoadScene(menuScene, LoadSceneMode.Single);
    }

    // =========================
    // UI Helpers
    // =========================
    void SetHudVisible(bool on)
    {
        if (hudCanvas == null) return;

        hudCanvas.SetActive(on);

        // If HUD has a CanvasGroup, force it visible when ON
        var cg = hudCanvas.GetComponentInChildren<CanvasGroup>(true);
        if (cg != null)
        {
            if (on)
            {
                cg.alpha = 1f;
                cg.interactable = true;
                cg.blocksRaycasts = true;
            }
            else
            {
                // optional: if you want it fully hidden when off
                cg.alpha = 0f;
                cg.interactable = false;
                cg.blocksRaycasts = false;
            }
        }
    }

    void SetLoadingUI(bool on, float progress, string msg = null)
    {
        if (loadingCanvas != null)
            loadingCanvas.SetActive(on);

        if (loadingBar != null)
            loadingBar.value = progress;

        if (loadingText != null && msg != null)
            loadingText.text = msg;
    }

    IEnumerator Fade(float target)
    {
        if (fadeCanvas == null) yield break;

        fadeCanvas.blocksRaycasts = true;

        float start = fadeCanvas.alpha;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / Mathf.Max(0.01f, fadeTime);
            fadeCanvas.alpha = Mathf.Lerp(start, target, t);
            yield return null;
        }

        fadeCanvas.alpha = target;
        fadeCanvas.blocksRaycasts = target > 0.01f;
    }

    public void EndToStageSelect()
    {
        LoadRoom("StageGraph", false); // your stage graph scene
    }

    // Optional getters
    public string GetCurrentRoomSceneName() => currentRoomScene;
    public bool IsLoading() => loading;
}