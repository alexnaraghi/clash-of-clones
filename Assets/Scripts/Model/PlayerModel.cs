using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

/// <summary>
/// Represents the state of a player.
/// </summary>
public class PlayerModel : MonoBehaviour
{
    // Differentiate units and buildings per player.
    public Material PlayerMaterial;
    public Color PlayerColor;

    /// <summary>
    /// The cards passed in as the start deck.
    /// </summary>
    public List<CardData> AllCards;

    /// <summary>
    /// The current state of the player's deck, hand, discard.
    /// </summary>
    public PlayerCardModel CardState;

    /// <summary>
    /// All units belonging to this player currently in play.
    /// </summary>
    public List<Entity> Units;
    
    /// <summary>
    /// Current mana.
    /// </summary>
    public float Mana;

    /// <summary>
    /// Player name.
    /// </summary>
    public string Name;

    // Buildings
    public Entity TopOutpost;
    public Entity HQ;
    public Entity BottomOutpost;

    /// <summary>
    /// Definition of the territories belonging to the above buildings.
    /// </summary>
    public TerritoryChunkData[] TerritoryChunks;

    /// <summary>
    /// Building array for convenience, links a building with its territory.
    /// </summary>
    [HideInInspector] public BuildingData[] Buildings;

    void Awake()
    {
        Units = new List<Entity>();

        Assert.IsNotNull(TopOutpost);
        Assert.IsNotNull(HQ);
        Assert.IsNotNull(BottomOutpost);

        // If this looks nasty it's because it is.
        // TODO: Refactor how territory is linked to buildings.
        if(TerritoryChunks.Length == 3)
        {
            Buildings = new BuildingData[3]
            {
                new BuildingData(TopOutpost, TerritoryChunks[2]),
                new BuildingData(HQ, TerritoryChunks[1]),
                new BuildingData(BottomOutpost, TerritoryChunks[0])
            };
        }
        else
        {
            Debug.LogWarning("Must have 3 territories per player that correspond to their buildings.");
        }
    }

    public void Init(string name, List<CardData> deck)
    {
        // EARLY OUT! //
        if(Name == null || CardState == null)
        {
            Debug.LogWarning("Name and deck of cards are required to initialize player.");
            return;
        }

        Name = name;
        Mana = Consts.StartMana;

        // Don't we duplicate some cards to form a full deck?
        AllCards = deck;
        CardState.Init(deck);

        initBuildings();
    }

    void Update()
    {
        if(GameModel.Instance.IsPlaying)
        {
            float unclampedMana = Mana + Consts.ManaRechargePerSecond * Time.deltaTime;
            Mana = Mathf.Clamp(unclampedMana, 0f, Consts.MaxMana);
        }
    }

    public bool CanPlayCard(CardData card)
    {
        return card != null
            && (CardState.Hand.IndexOf(card) != -1 
            && Mana >= card.ManaCost);
    }

    public void PlayCard(CardData card, Vector3 position)
    {
        // EARLY OUT! //
        if(card == null)
        {
            Debug.LogWarning("Cannot play a null card!");
            return;
        }

        // EARLY OUT! //
        if(!CanPlayCard(card))
        {
            Debug.LogWarning("Tried to play a card that couldn't be played: " + card.PrefabName);
            return;
        }

        CardState.PlayCard(card);
        Mana -= card.ManaCost;

        var unit = Entity.SpawnFromDefinition(this, card, position, isFromPlayersHand: true);
        if(unit != null)
        {
            // Rotate the unit to face the enemy bases.
            RotateForPlayer(unit.gameObject);
            
            //TODO: Remove units when out of HP.
            Units.Add(unit);
        }
    }

    /// <summary>
    /// Helper to rotate a model in the opposite player's direction.  Asumming facing down to start. 
    /// </summary>
    public void RotateForPlayer(GameObject go)
    {
        if(go != null)
        {
            if(this == GameModel.Instance.LeftPlayer)
            {
                go.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
            }
            else
            {
                go.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
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

    private void initBuildings()
    {
        // EARLY OUT! //
        if(TopOutpost == null || HQ == null || BottomOutpost == null) return;

        TopOutpost.Init(    this, Config.Instance.GetCardByName("Outpost"), false);
        BottomOutpost.Init( this, Config.Instance.GetCardByName("Outpost"), false);
        HQ.Init(            this, Config.Instance.GetCardByName("HQ"), false);
    }
}
