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
            attacksUnits : true,
            isProjectile : false,
            spawnSeconds : 0
        );
        return card;
    }

    public static CardDefinition CreateHeavyTank()    
    {
        var card = new CardDefinition(
            prefabName : "Tank2",
            cardImageName : "Tank2",
            ghostPrefabName : "TankGhost2",
            manaCost : 6,
            startHP : 250,
            placementWidth : 1,
            placementHeight : 1,
            attackSpeed : 3,
            movementSpeed : 2,
            directAttackDamage : 0,
            areaAttackDamage : 50,
            aggroRange : 20,
            attackRange : 20,
            isBuilding : false,
            attacksUnits : true,
            isProjectile : false,
            spawnSeconds : 0
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
            attacksUnits : true,
            isProjectile : false,
            spawnSeconds : 0
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
            areaAttackDamage : 0,
            directAttackDamage : 50,
            aggroRange : 25,
            attackRange : 25,
            isBuilding : true,
            attacksUnits : true,
            isProjectile : false,
            spawnSeconds : 0
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
            attackSpeed : 1,
            movementSpeed : 0,
            areaAttackDamage : 0,
            directAttackDamage : 30,
            aggroRange : 25,
            attackRange : 25,
            isBuilding : true,
            attacksUnits : true,
            isProjectile : false,
            spawnSeconds : 0
        );
        return card;
    }

    /// <summary>
    /// Special case for permanent building
    /// </summary>
    public static CardDefinition CreateStaticDefense()    
    {
        var card = new CardDefinition(
            prefabName : "Turret",
            cardImageName : "Turret",
            ghostPrefabName : "TurretGhost",
            manaCost : 4,
            startHP : 200,
            placementWidth : 1,
            placementHeight : 1,
            attackSpeed : 1,
            movementSpeed : 0,
            areaAttackDamage : 0,
            directAttackDamage : 15,
            aggroRange : 20,
            attackRange : 20,
            isBuilding : true,
            attacksUnits : true,
            isProjectile : false,
            spawnSeconds : 0
            
        );
        return card;
    }

    public static CardDefinition CreateSkeletonArmy()    
    {
        var card = new CardDefinition(
            prefabName : "SkellyArmy",
            cardImageName : "SkellyArmy",
            ghostPrefabName : "SkellyGhost",
            manaCost : 4,
            startHP : 0,
            placementWidth : 1,
            placementHeight : 1,
            attackSpeed : 0,
            movementSpeed : 0,
            areaAttackDamage : 0,
            directAttackDamage : 0,
            aggroRange : 0,
            attackRange : 0,
            isBuilding : false,
            attacksUnits : false,
            isProjectile : false,
            spawnSeconds : 0
        );
        return card;
    }

    public static CardDefinition CreateSkeletonHouse()    
    {
        var card = new CardDefinition(
            prefabName : "SkellyHouse",
            cardImageName : "SkellyHouse",
            ghostPrefabName : "SkellyHouseGhost",
            manaCost : 6,
            startHP : 100,
            placementWidth : 1,
            placementHeight : 1,
            attackSpeed : 0,
            movementSpeed : 0,
            areaAttackDamage : 0,
            directAttackDamage : 0,
            aggroRange : 0,
            attackRange : 0,
            isBuilding : false,
            attacksUnits : false,
            isProjectile : false,
            spawnSeconds : 10
        );
        return card;
    }

    public static CardDefinition CreateSkeleton()    
    {
        var card = new CardDefinition(
            prefabName : "Skelly",
            cardImageName : "Skelly",
            ghostPrefabName : "",
            manaCost : 1,
            startHP : 100,
            placementWidth : 1,
            placementHeight : 1,
            attackSpeed : 1,
            movementSpeed : 5,
            areaAttackDamage : 0,
            directAttackDamage : 50,
            aggroRange : 20,
            attackRange : 3,
            isBuilding : false,
            attacksUnits : true,
            isProjectile : false,
            spawnSeconds : 0 
        );
        return card;
    }

    public static CardDefinition CreateFireball()    
    {
        var card = new CardDefinition(
            prefabName : "FireballSpell",
            cardImageName : "Fireball",
            ghostPrefabName : "FireballSpellGhost",
            manaCost : 3,
            startHP : 0,
            placementWidth : 1,
            placementHeight : 1,
            attackSpeed : 0,
            movementSpeed : 0,
            areaAttackDamage : 100,
            directAttackDamage : 0,
            aggroRange : 0,
            attackRange : 0,
            isBuilding : false,
            attacksUnits : false,
            isProjectile : true,
            spawnSeconds : 0
        );
        return card;
    }

    public static List<CardDefinition> CreateDeck()
    {
        var definitions = new List<CardDefinition>();

        // Make a deck.
        for (int i = 0; i < 2; i++)
        {
            definitions.Add(CreateBasicTank());
        }
        for (int i = 2; i < 4; i++)
        {
            definitions.Add(CreateHeavyTank());
        }
        for (int i = 4; i < 6; i++)
        {
            definitions.Add(CreateSkeletonArmy());
        }
        for (int i = 6; i < 8; i++)
        {
            definitions.Add(CreateSkeletonHouse());
        }
        for (int i = 8; i < 9; i++)
        {
            definitions.Add(CreateStaticDefense());
        }
        for (int i = 8; i < 10; i++)
        {
            definitions.Add(CreateFireball());
        }
        return definitions;
    }
}