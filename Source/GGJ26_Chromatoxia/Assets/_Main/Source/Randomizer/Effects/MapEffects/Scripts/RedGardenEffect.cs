using UnityEngine;

[CreateAssetMenu(menuName = "RandomEffects/Map/Red Garden")]
public class RedGardenEffect : RandomEffect
{
    private void OnEnable()
    {
        effectName = "Red Garden";
        type = EffectType.Map;
        stackable = false;
        maxStacks = 1;
    }

    public override void Apply(GameModifiers mods, int stacks)
    {
        
    }
}