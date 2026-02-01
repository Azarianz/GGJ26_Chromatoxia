using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameEndType { Win, Lose }

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }

    [Header("Game Jam Toggles")]
    [SerializeField] bool pauseTimeOnEnd = true; // if true, Time.timeScale = 0 on end
    [SerializeField] bool autoResetOnSceneLoad = true; // fixes "ended" carrying into next stage

    bool ended;

    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;

        // If this object exists in multiple scenes, the first one stays and others get destroyed.
        // So we *must* reset between scenes.
        if (autoResetOnSceneLoad)
            SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        if (autoResetOnSceneLoad)
            SceneManager.sceneLoaded -= OnSceneLoaded;

        if (I == this) I = null;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Ensures F1/debug win/lose works in every new room,
        // and prevents staying "ended" after returning to graph / loading next stage.
        ResetForNewRoom();
    }

    // =====================
    // Call from gameplay
    // =====================
    public void Win(string reason = null) => End(GameEndType.Win, reason);
    public void Lose(string reason = null) => End(GameEndType.Lose, reason);

    void End(GameEndType type, string reason)
    {
        if (ended) return;
        ended = true;

        if (pauseTimeOnEnd)
            Time.timeScale = 0f;

        if (GameUIManager.Instance != null)
            GameUIManager.Instance.ShowGameEnd(type, reason);

        Debug.Log($"[GameManager] {type}" + (string.IsNullOrEmpty(reason) ? "" : $" ({reason})"));
    }

    // =====================
    // Debug helpers
    // =====================
    public void DebugWinStage() => Win("Node Cleared");
    public void DebugLoseStage() => Lose("Debug Lose");

    // =====================
    // Reset per room
    // =====================
    public void ResetForNewRoom()
    {
        ended = false;

        // Always unpause as safety (prevents "stuck timescale 0" bugs).
        Time.timeScale = 1f;

        if (GameUIManager.Instance != null)
            GameUIManager.Instance.HideGameEnd();
    }
}