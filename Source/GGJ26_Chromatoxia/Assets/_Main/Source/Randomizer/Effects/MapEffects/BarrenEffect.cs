using UnityEngine;

[CreateAssetMenu(menuName = "RandomEffects/Map/Barren")]
public class BarrenEffect : RandomEffect
{
    private void OnEnable()
    {
        effectName = "Barren";
        type = EffectType.Map;
        stackable = false;
        maxStacks = 1;
    }

    public override void Apply(GameModifiers mods, int stacks)
    {
        
    }
}