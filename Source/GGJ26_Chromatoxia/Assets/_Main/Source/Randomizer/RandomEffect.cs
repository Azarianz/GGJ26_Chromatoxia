using UnityEngine;

public enum EffectType
{
    Mutation,
    Map
}
    
public abstract class RandomEffect : ScriptableObject
{
    public string effectName;
    public EffectType type;

    [Header("Optional")]
    public bool stackable = false;
    public int maxStacks = 3;

    public abstract void Apply(GameModifiers ctx, int stacks);
}
