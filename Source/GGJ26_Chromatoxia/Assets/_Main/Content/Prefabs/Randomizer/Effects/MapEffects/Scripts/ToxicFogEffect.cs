using UnityEngine;

[CreateAssetMenu(menuName = "RandomEffects/Map/Toxic Fog")]
public class ToxicFogEffect : RandomEffect
{
    public float oxygenMult = 1.35f;

    private void OnEnable()
    {
        effectName = "Toxic Fog";
        type = EffectType.Map;
        stackable = false;
        maxStacks = 1;
    }

    public override void Apply(GameModifiers mods, int stacks)
    {
        mods.oxygenDrainMultiplier *= oxygenMult;
    }
}