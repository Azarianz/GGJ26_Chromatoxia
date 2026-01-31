using UnityEngine;

[CreateAssetMenu(menuName = "RandomEffects/Mutation/Elites")]
public class ElitesEffect : RandomEffect
{
    [Range(0f, 1f)]
    public float eliteChancePerStack = 0.10f;

    private void OnEnable()
    {
        effectName = "Elites";
        type = EffectType.Mutation;
        stackable = true;
        maxStacks = 3;
    }

    public override void Apply(GameModifiers mods, int stacks)
    {
        mods.eliteChance += eliteChancePerStack * stacks;
    }
}