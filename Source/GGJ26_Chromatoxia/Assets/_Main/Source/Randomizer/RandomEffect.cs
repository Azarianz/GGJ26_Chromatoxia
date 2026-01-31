using UnityEngine;

public enum EffectType
{
    Mutation,
    Map
}
    
public abstract class RandomEffect : ScriptableObject
{
    [Header("Effect Info")]
    public string effectName;
    public string effectDescription;
    public Sprite effectIcon;

    [Header("Effect Type")]
    public EffectType type;
    public bool isPositive;

    [Header("Optional")]
    public bool stackable = false;
    public int maxStacks = 3;

    public abstract void Apply(GameModifiers ctx, int stacks);
}
