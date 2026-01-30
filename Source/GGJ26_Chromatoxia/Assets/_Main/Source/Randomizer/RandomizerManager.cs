using System.Collections.Generic;
using UnityEngine;

public class RandomizerManager : MonoBehaviour
{
    public GameModifiers ctx;

    public List<RandomEffect> mutations = new();
    public List<RandomEffect> mapEffects = new();

    // stacking storage
    private Dictionary<RandomEffect, int> stacks = new();

    public void RollZoneEffects()
    {
        ApplyOne(mutations);
        ApplyOne(mapEffects);
    }

    void ApplyOne(List<RandomEffect> pool)
    {
        if (pool == null || pool.Count == 0) return;

        var effect = pool[Random.Range(0, pool.Count)];
        if (effect == null) return;

        int current = stacks.TryGetValue(effect, out int s) ? s : 0;

        if (current > 0 && !effect.stackable) return; // already active, ignore

        int next = Mathf.Clamp(current + 1, 1, effect.maxStacks);
        stacks[effect] = next;

        effect.Apply(ctx, next);
    }
}