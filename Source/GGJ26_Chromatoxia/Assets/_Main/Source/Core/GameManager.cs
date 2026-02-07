using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameEndType { Win, Lose }

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }

    [Header("Game Jam Toggles")]
    [SerializeField] bool pauseTimeOnEnd = true;
    [SerializeField] bool autoResetOnSceneLoad = true;

    bool ended;

    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;

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

        Debug.Log($"[GameManager] {type}" +
                  (string.IsNullOrEmpty(reason) ? "" : $" ({reason})"));
    }

    // =====================
    // Game Over Menu Button
    // =====================
    public void OnGameOverMenuClicked()
    {
        Time.timeScale = 1f;
        ended = false;

        if (RunManager.I != null)
            RunManager.I.DestroyBootstrap(); // optional

        SceneManager.LoadScene("MainMenu");
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
        Time.timeScale = 1f;

        if (GameUIManager.Instance != null)
            GameUIManager.Instance.HideGameEnd();
    }
}