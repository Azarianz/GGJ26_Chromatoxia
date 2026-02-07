using UnityEngine;

[CreateAssetMenu(menuName = "RandomEffects/Mutation/Horde")]
public class HordeEffect : RandomEffect
{
    public int increasePop = 10;
    public float spawnDecrTime = 0.30f;

    private void OnEnable()
    {
        effectName = "Horde";
        type = EffectType.Mutation;
        stackable = true;
        maxStacks = 3;
    }

    public override void Apply(GameModifiers mods, int stacks)
    {
        mods.spawnMult *= (1f + spawnDecrTime * stacks);
        mods.maxPopulation += increasePop * stacks;
    }
}