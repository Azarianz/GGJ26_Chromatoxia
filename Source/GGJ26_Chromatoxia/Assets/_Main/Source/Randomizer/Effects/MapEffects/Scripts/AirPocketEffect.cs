using UnityEngine;

[CreateAssetMenu(menuName = "RandomEffects/Map/Air Pocket")]
public class AirPocketEffect : RandomEffect
{
    public float oxygenMult = 0.85f;

    private void OnEnable()
    {
        effectName = "Air Pocket";
        type = EffectType.Map;
        stackable = false;
        maxStacks = 1;
    }

    public override void Apply(GameModifiers mods, int stacks)
    {
        mods.oxygenDrainMultiplier *= oxygenMult;
    }
}