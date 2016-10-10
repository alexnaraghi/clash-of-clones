using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Inventory
/// </summary>
public class Player : MonoBehaviour
{
    // Differentiate units and buildings per player.
    public Material PlayerMaterial;
    public Color PlayerColor;

    public List<CardDefinition> AllCards;
    public CardState CardState;
    public List<Entity> Units;
    public float Mana;
    public string Name;

    public Entity TopOutpost;
    public Entity HQ;
    public Entity BottomOutpost;

    public TerritoryChunk[] TerritoryChunks;

    /// <summary>
    /// Building array for convenience, links a building with its territory.
    /// </summary>
    [HideInInspector] public Building[] Buildings;

    void Awake()
    {
        Units = new List<Entity>();

        if(TerritoryChunks.Length == 3)
        {
            Buildings = new Building[3]
            {
                new Building(TopOutpost, TerritoryChunks[2]),
                new Building(HQ, TerritoryChunks[1]),
                new Building(BottomOutpost, TerritoryChunks[0])
            };
        }
        else
        {
            Debug.LogWarning("Must have 3 territories per player that correspond to their buildings.");
        }
    }

    public void Init(string name, List<CardDefinition> deck)
    {
        // EARLY OUT! //
        if(Name == null || CardState == null) return;

        Name = name;
        Mana = Consts.StartMana;

        // Don't we duplicate some cards to form a full deck?
        AllCards = deck;
        CardState.Init(deck);

        initBuildings();
    }

    private void initBuildings()
    {
        // EARLY OUT! //
        if(TopOutpost == null || HQ == null || BottomOutpost == null) return;

        TopOutpost.Init(    this, TestFactory.CreateOutpost());
        BottomOutpost.Init( this, TestFactory.CreateOutpost());
        HQ.Init(            this, TestFactory.CreateHQ());
    }

    void Update()
    {
        float unclampedMana = Mana + Consts.ManaRechargePerSecond * Time.deltaTime;
        Mana = Mathf.Clamp(unclampedMana, 0f, Consts.MaxMana);
    }

    public bool CanPlayCard(CardDefinition card)
    {
        return (CardState.Hand.IndexOf(card) != -1 && Mana >= card.ManaCost);
    }

    public void PlayCard(CardDefinition card, Vector3 position)
    {
        // EARLY OUT! //
        if(!CanPlayCard(card)) return;

        CardState.PlayCard(card);
        Mana -= card.ManaCost;

        var unit = Entity.SpawnFromDefinition(this, card, position);
        RotateForPlayer(unit.gameObject);
        
        Units.Add(unit);
    }
    
    /// <summary>
    /// Helper to rotate a model in the opposite player's direction.  Asumming facing down to start. 
    /// </summary>
    public void RotateForPlayer(GameObject go)
    {
        if(go != null)
        {
            if(this == GameState.Instance.LeftPlayer)
            {
                go.transform.Rotate(new Vector3(0f, 90f, 0f));
            }
            else
            {
                go.transform.Rotate(new Vector3(0f, 90f, 0f));
            }
        }
    }

    /// <summary>
    /// Is the given position within this player's territory?
    /// </summary>
    public bool IsInTerritory(Vector3 position)
    {
        bool isInTerritory = false;

        foreach(var building in Buildings)
        {
            if(building.IsInControlledTerritory(position))
            {
                isInTerritory = true;
            }
        }

        return isInTerritory;
    }
}
