using UnityEngine;
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
public class CardData
{
    public string Name;
    
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

    public bool IsAirUnit;
    public bool IsBuilding;
    public bool AttacksGroundUnits;
    public bool AttacksAirUnits;

    public bool IsProjectile;

    /// <summary>
    /// If a spawner is attached, will use this to determine how often to spawn a new wave.
    /// </summary>
    public int SpawnSeconds;
    
    /// <summary>
    /// Default constructor.
    /// </summary>
    public CardData() { }
}
