using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// UnitDefinition
/// </summary>
[Serializable]
public class CardDefinition 
{
    public string PrefabName;
    public string GhostPrefabName;
    public string CardImageName;

    public int ManaCost;

    public int PlacementWidth;
    public int PlacementHeight;

    public int StartHP;

    // Right now this is seconds between attack.
    public int AttackSpeed;
    public int MovementSpeed;
    public int DirectAttackDamage;
    public int AreaAttackDamage;

    public int AggroRange;
    public int AttackRange;

    public bool IsBuilding;
    public bool AttacksUnits;

    public CardDefinition()
    {

    }

    /// <summary>
    /// Constructor for test factory, ensures everything gets set on test objects.
    /// </summary>
    public CardDefinition(string prefabName, string ghostPrefabName, string cardImageName, int manaCost,
        int placementWidth, int placementHeight, int startHP, int attackSpeed, int movementSpeed, 
        int directAttackDamage, int areaAttackDamage, int aggroRange, int attackRange, bool isBuilding, 
        bool attacksUnits)
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
    }
}
