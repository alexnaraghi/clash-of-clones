using System;

/// <summary>
/// Definition of a card.  Intended to be the definition of a card in the deck of the player.  For how cards 
/// get modified by effects during play, see <see cref="Entity"/> (currently a TODO).
/// </summary>
/// <remarks>
/// We also use cards to define the default player structures.  They also will fire weapons, and this structure
/// is reasonable for that.  Consider breaking this up into two or more definition structures if it becomes
/// unwieldy, for now it feels robust enough to keep using.
/// </remarks>
[Serializable]
public class CardDefinition 
{
    // Prefab values.
    public string PrefabName;
    public string GhostPrefabName;
    public string CardImageName;

    // TODO: Quantify better what these values mean.
    public int ManaCost;

    public int PlacementWidth;
    public int PlacementHeight;

    public int StartHP;

    // Right now this is seconds between attack, but shouldn't this be the inverse?
    public int AttackSpeed;
    public int MovementSpeed;
    public int DirectAttackDamage;
    public int AreaAttackDamage;

    public int AggroRange;
    public int AttackRange;

    public bool IsBuilding;
    public bool AttacksUnits;

    public bool IsProjectile;
    
    /// <summary>
    /// Default constructor.
    /// </summary>
    public CardDefinition() { }

    /// <summary>
    /// Constructor for test factory, ensures everything gets set on test objects.
    /// </summary>
    public CardDefinition(string prefabName, string ghostPrefabName, string cardImageName, int manaCost,
        int placementWidth, int placementHeight, int startHP, int attackSpeed, int movementSpeed, 
        int directAttackDamage, int areaAttackDamage, int aggroRange, int attackRange, bool isBuilding, 
        bool attacksUnits, bool isProjectile)
    {
        PrefabName = prefabName;
        GhostPrefabName = ghostPrefabName;
        CardImageName = cardImageName;
        ManaCost = manaCost;
        PlacementWidth = placementWidth;
        PlacementHeight = placementHeight;
        StartHP = startHP;
        AttackSpeed = attackSpeed;
        MovementSpeed = movementSpeed;
        DirectAttackDamage = directAttackDamage;
        AreaAttackDamage = areaAttackDamage;
        AggroRange = aggroRange;
        AttackRange = attackRange;
        IsBuilding = isBuilding;
        AttacksUnits = attacksUnits;
        IsProjectile = isProjectile;
    }
}
