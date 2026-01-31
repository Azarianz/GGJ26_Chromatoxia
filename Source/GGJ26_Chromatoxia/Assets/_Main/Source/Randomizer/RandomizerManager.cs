using System.Collections.Generic;
using UnityEngine;

public class RandomizerManager : MonoBehaviour
{
    [Header("Context")]
    public GameModifiers ctx;

    [Header("Pools")]
    public List<RandomEffect> mutations = new();
    public List<RandomEffect> mapEffects = new();

    [Header("Start Draw Settings")]
    [Range(1, 6)] public int minCards = 1;
    [Range(1, 6)] public int maxCards = 6;

    [Header("Optional UI")]
    public GameObject cardUIPrefab;
    public Transform cardUIParent;

    [Header("Debug")]
    public bool debugLogEffects = true;

    // Active effects (for debugging / UI / saving)
    public List<RandomEffect> currentEffects = new();

    // stacking storage
    private Dictionary<RandomEffect, int> stacks = new();

    void Start()
    {
        DrawRandomCardsOnStart();
    }

    public void DrawRandomCardsOnStart()
    {
        int count = Random.Range(minCards, maxCards + 1); // inclusive
        DrawAndApplyCards(count);
    }

    public void DrawAndApplyCards(int count)
    {
        if (count <= 0) return;

        // Combine both pools so a "card" can be either mutation or map effect
        List<RandomEffect> all = new();
        if (mutations != null) all.AddRange(mutations);
        if (mapEffects != null) all.AddRange(mapEffects);

        if (all.Count == 0) return;

        for (int i = 0; i < count; i++)
        {
            ApplyOne(all);
        }
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
            Transform parent = cardUIParent != null ? cardUIParent : transform;
            Instantiate(cardUIPrefab, parent);
            // If your UI prefab has a script like CardUI, you can pass effect info here.
            // e.g. GetComponent<CardUI>().Setup(effect.effectName, effect.type, next);
        }
    }
}