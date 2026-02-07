using UnityEngine;

[CreateAssetMenu(menuName = "RandomEffects/Map/Hazards")]
public class HazardsEffect : RandomEffect
{
    private void OnEnable()
    {
        effectName = "Hazards";
        type = EffectType.Map;
        stackable = false;
        maxStacks = 1;
    }

    public override void Apply(GameModifiers mods, int stacks)
    {
        
    }
}