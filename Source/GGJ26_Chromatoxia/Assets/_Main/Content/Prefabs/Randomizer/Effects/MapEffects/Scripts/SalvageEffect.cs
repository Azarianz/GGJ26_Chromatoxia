using UnityEngine;

[CreateAssetMenu(menuName = "RandomEffects/Map/Salvage")]
public class SalvageEffect : RandomEffect
{
    [Range(0f, 1f)]
    public float dropBonus = 0.15f;

    private void OnEnable()
    {
        effectName = "Salvage";
        type = EffectType.Map;
        stackable = false;
        maxStacks = 1;
    }

    public override void Apply(GameModifiers mods, int stacks)
    {
        mods.dropBonus += dropBonus;
    }
}