using UnityEngine;

[CreateAssetMenu(menuName = "RandomEffects/Mutation/Frenzy")]
public class FrenzyEffect : RandomEffect
{
    public float speedPerStack = 0.20f;

    private void OnEnable()
    {
        effectName = "Frenzy";
        type = EffectType.Mutation;
        stackable = true;
        maxStacks = 3;
    }

    public override void Apply(GameModifiers mods, int stacks)
    {
        mods.enemySpeedMult *= (1f + speedPerStack * stacks);
    }
}