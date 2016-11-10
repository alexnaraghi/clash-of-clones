using System;

public enum AttributeModifier
{
    None,
    AttackSpeed,
    MovementSpeed
}

[Serializable]
public class Effect 
{
    public AttributeModifier Attribute;
    public float Multiplier;
}