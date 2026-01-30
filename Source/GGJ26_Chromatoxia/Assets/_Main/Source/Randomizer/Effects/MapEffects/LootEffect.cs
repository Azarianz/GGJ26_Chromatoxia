using UnityEngine;

[CreateAssetMenu(menuName = "RandomEffects/Map/Loot")]
public class LootMapEffect : RandomEffect
{
    public int guaranteedLootCount = 2; // your loot spawner reads this if you want

    private void OnEnable()
    {
        effectName = "Loot";
        type = EffectType.Map;
        stackable = false;
        maxStacks = 1;
    }

    public override void Apply(GameModifiers mods, int stacks)
    {
        // Jam-simple: treat as "more loot draws" OR handle in your spawner.
        mods.lootDrawMult *= 1.25f;
    }
}