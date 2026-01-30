using UnityEngine;

[CreateAssetMenu(menuName = "RandomEffects/Mutation/Resilience")]
public class ResilienceEffect : RandomEffect
{
    public float hpIncreasePerStack = 0.25f;

    private void OnEnable()
    {
        effectName = "Resilience";
        type = EffectType.Mutation;
        stackable = true;
        maxStacks = 3;
    }

    public override void Apply(GameModifiers ctx, int stacks)
    {
        ctx.enemyHpMult = 1f + hpIncreasePerStack * stacks;
    }
}