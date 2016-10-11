using System.Collections.Generic;

/// <summary>
/// For testing, creates some cards that we can use to form decks.
/// </summary>
public static class TestFactory 
{
    public static CardDefinition CreateBasicTank()    
    {
        var card = new CardDefinition(
            prefabName : "Tank",
            ghostPrefabName : "TankGhost",
            cardImageName : "Tank",
            manaCost : 3,
            placementWidth : 1,
            placementHeight : 1,
            startHP : 100,
            attackSpeed : 1,
            movementSpeed : 5,
            directAttackDamage : 0,
            areaAttackDamage : 30,
            aggroRange : 15,
            attackRange : 15,
            isBuilding : false,
            attacksUnits : true
        );
        return card;
    }

    public static CardDefinition CreateHeavyTank()    
    {
        var card = new CardDefinition(
            prefabName : "Tank2",
            cardImageName : "Tank2",
            ghostPrefabName : "TankGhost2",
            manaCost : 5,
            startHP : 200,
            placementWidth : 1,
            placementHeight : 1,
            attackSpeed : 3,
            movementSpeed : 2,
            directAttackDamage : 0,
            areaAttackDamage : 50,
            aggroRange : 20,
            attackRange : 20,
            isBuilding : false,
            attacksUnits : true
        );
        return card;
    }

    public static CardDefinition CreateCheatTank()    
    {
        var card = new CardDefinition(
            prefabName : "Tank2",
            cardImageName : "Tank2",
            ghostPrefabName : "TankGhost2",
            manaCost : 1,
            startHP : 200,
            placementWidth : 1,
            placementHeight : 1,
            attackSpeed : 1,
            movementSpeed : 15,
            directAttackDamage : 0,
            areaAttackDamage : 900,
            aggroRange : 20,
            attackRange : 20,
            isBuilding : false,
            attacksUnits : true
        );
        return card;
    }

    /// <summary>
    /// Special case for HQ building.
    /// </summary>
    public static CardDefinition CreateHQ()    
    {
        var card = new CardDefinition(
            prefabName : "",
            cardImageName : "",
            ghostPrefabName : "",
            manaCost : 0,
            startHP : 1000,
            placementWidth : 0,
            placementHeight : 0,
            attackSpeed : 3,
            movementSpeed : 0,
            areaAttackDamage : 30,
            directAttackDamage : 0,
            aggroRange : 20,
            attackRange : 20,
            isBuilding : true,
            attacksUnits : true
        );
        return card;
    }

    /// <summary>
    /// Special case for permanent building
    /// </summary>
    public static CardDefinition CreateOutpost()    
    {
        var card = new CardDefinition(
            prefabName : "",
            cardImageName : "",
            ghostPrefabName : "",
            manaCost : 0,
            startHP : 750,
            placementWidth : 0,
            placementHeight : 0,
            attackSpeed : 3,
            movementSpeed : 0,
            areaAttackDamage : 30,
            directAttackDamage : 0,
            aggroRange : 20,
            attackRange : 20,
            isBuilding : true,
            attacksUnits : true
        );
        return card;
    }

    public static List<CardDefinition> CreateDeck()
    {
        var definitions = new List<CardDefinition>();

        // Make a deck.
        for (int i = 0; i < 6; i++)
        {
            definitions.Add(CreateBasicTank());
        }
        for (int i = 7; i < 10; i++)
        {
            definitions.Add(CreateHeavyTank());
        }
        return definitions;
    }
}