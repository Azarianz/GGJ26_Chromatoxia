using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RandomizerManager : MonoBehaviour
{
    [Header("Context")]
    public GameModifiers ctx;

    [Header("UI Canvas")]
    [SerializeField] private CanvasGroup randomizerCanvas;

    [Header("UI Timing")]
    [SerializeField] private float fadeInTime = 0.25f;
    [SerializeField] private float visibleTime = 2.5f;
    [SerializeField] private float fadeOutTime = 0.25f;


    [Header("Pools")]
    public List<RandomEffect> mutations = new();
    public List<RandomEffect> mapEffects = new();

    [Header("Start Draw Settings")]
    [Range(1, 6)] public int minCards = 1;
    [Range(1, 6)] public int maxCards = 6;

    [Header("Optional UI")]
    public GameObject cardUIPrefab;
    public Transform uiListParent;

    [Header("Debug")]
    public bool debugLogEffects = true;

    // Active effects (for debugging / UI / saving)
    public List<RandomEffect> currentEffects = new();

    // stacking storage
    private Dictionary<RandomEffect, int> stacks = new();

    [Header("When to run")]
    public bool runOnSceneLoaded = true;
    public List<string> ignoreScenes = new() { "MainMenu", "RunBootstrap" };

    private System.Random rng;
    [SerializeField] private int debugSeed = 0; // optional, 0 = auto


    #region Card Functions
    public void DrawRandomCards()
    {
        int count = rng.Next(minCards, maxCards + 1); // max exclusive
        DrawAndApplyCards(count);
        ShowRandomizerUI();
    }

    public void DrawAndApplyCards(int count)
    {
        if (count <= 0) return;

        List<RandomEffect> all = new();
        if (mutations != null) all.AddRange(mutations);
        if (mapEffects != null) all.AddRange(mapEffects);
        if (all.Count == 0) return;

        List<RandomEffect> available = new(all);

        int target = Mathf.Min(count, available.Count);
        int applied = 0;

        while (applied < target && available.Count > 0)
        {
            if (TryApplyOneUnique(available))
                applied++;
        }
    }

    bool TryApplyOneUnique(List<RandomEffect> available)
    {
        int idx = rng.Next(0, available.Count);
        var effect = available[idx];
        available.RemoveAt(idx);

        if (effect == null) return false;

        int current = stacks.TryGetValue(effect, out int s) ? s : 0;
        if (current > 0 && !effect.stackable) return false;

        int next = Mathf.Clamp(current + 1, 1, effect.maxStacks);
        stacks[effect] = next;

        if (!currentEffects.Contains(effect))
            currentEffects.Add(effect);

        effect.Apply(ctx, next);

        if (debugLogEffects)
            Debug.Log($"[Randomizer] Applied {effect.effectName} ({effect.type}) | Stack {next}/{effect.maxStacks}");

        if (cardUIPrefab != null)
        {
            Transform parent = uiListParent != null ? uiListParent : transform;
            GameObject go = Instantiate(cardUIPrefab, parent);

            var ui = go.GetComponent<RandomizerCardUI>();
            if (ui != null) ui.Setup(effect, next);
        }

        return true;
    }


    void ApplyOne(List<RandomEffect> pool)
    {
        if (pool == null || pool.Count == 0) return;

        var effect = pool[Random.Range(0, pool.Count)];
        if (effect == null) return;

        int current = stacks.TryGetValue(effect, out int s) ? s : 0;

        // already active and can't stack -> try a few times to find something else
        if (current > 0 && !effect.stackable)
        {
            const int tries = 10;
            for (int t = 0; t < tries; t++)
            {
                effect = pool[Random.Range(0, pool.Count)];
                if (effect == null) continue;

                current = stacks.TryGetValue(effect, out s) ? s : 0;
                if (current == 0 || effect.stackable) break;
            }

            // if still non-stackable active, just give up this draw
            if (current > 0 && !effect.stackable) return;
        }

        int next = Mathf.Clamp(current + 1, 1, effect.maxStacks);
        stacks[effect] = next;

        if (!currentEffects.Contains(effect))
            currentEffects.Add(effect);

        effect.Apply(ctx, next);

        if (debugLogEffects)
        {
            Debug.Log(
                $"[Randomizer] Applied {effect.effectName} " +
                $"({effect.type}) | Stack {next}/{effect.maxStacks}"
            );
        }

        // Optional UI spawn
        if (cardUIPrefab != null)
        {
            Transform parent = uiListParent != null ? uiListParent : transform;

            GameObject go = Instantiate(cardUIPrefab, parent);

            // Bind UI
            var ui = go.GetComponent<RandomizerCardUI>();
            if (ui != null)
                ui.Setup(effect, next);
            else if (debugLogEffects)
                Debug.LogWarning("[Randomizer] cardUIPrefab has no RandomizerCardUI component.", go);
        }
    }
    #endregion
    #region UI
    Coroutine uiRoutine;
    void ClearSpawnedCardsUI()
    {
        if (!uiListParent) return;

        for (int i = uiListParent.childCount - 1; i >= 0; i--)
            Destroy(uiListParent.GetChild(i).gameObject);
    }
    void ShowRandomizerUI()
    {
        if (!randomizerCanvas) return;

        if (uiRoutine != null)
            StopCoroutine(uiRoutine);

        uiRoutine = StartCoroutine(RandomizerUIRoutine());
    }

    IEnumerator RandomizerUIRoutine()
    {
        // Ensure visible + interactable
        randomizerCanvas.gameObject.SetActive(true);
        randomizerCanvas.blocksRaycasts = true;
        randomizerCanvas.interactable = true;

        // ---- Fade IN ----
        yield return FadeCanvas(0f, 1f, fadeInTime);

        // ---- Hold ----
        yield return new WaitForSecondsRealtime(visibleTime);

        // ---- Fade OUT ----
        yield return FadeCanvas(1f, 0f, fadeOutTime);

        // Disable interaction after hiding
        randomizerCanvas.blocksRaycasts = false;
        randomizerCanvas.interactable = false;
        randomizerCanvas.gameObject.SetActive(false);

        uiRoutine = null;
    }
    IEnumerator FadeCanvas(float from, float to, float duration)
    {
        if (!randomizerCanvas) yield break;

        randomizerCanvas.alpha = from;

        if (duration <= 0f)
        {
            randomizerCanvas.alpha = to;
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(from, to, t / duration);
            randomizerCanvas.alpha = a;
            yield return null;
        }

        randomizerCanvas.alpha = to;
    }

    #endregion

    void Awake()
    {
        // existing canvas init...
        if (randomizerCanvas)
        {
            randomizerCanvas.alpha = 0f;
            randomizerCanvas.interactable = false;
            randomizerCanvas.blocksRaycasts = false;
            randomizerCanvas.gameObject.SetActive(false);
        }

        // Seed RNG once
        int seed = (debugSeed != 0) ? debugSeed : System.Environment.TickCount;
        rng = new System.Random(seed);

        if (debugLogEffects)
            Debug.Log($"[Randomizer] RNG seed = {seed}");
    }
    void OnEnable()
    {
        if (runOnSceneLoaded)
            SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        if (runOnSceneLoaded)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (ignoreScenes != null && ignoreScenes.Contains(scene.name))
            return;

        OnLevelStart();
    }

    public void OnLevelStart()
    {
        Debug.Log("[Randomizer] Level Start - Drawing Cards");
        // Optional: clear previous UI cards each level
        ClearSpawnedCardsUI();

        // Optional: clear stacks each level if you want fresh rolls
        stacks.Clear();
        currentEffects.Clear();

        DrawRandomCards();
    }
}