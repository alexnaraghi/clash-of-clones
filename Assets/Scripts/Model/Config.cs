using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;

public class Config : MonoBehaviour 
{
    public CardDataList Cards;
    public DeckDataList Decks;
    public DeckSelectionData DeckSelections;

    private const string CARD_PATH = "CardConfig";
    private const string DECK_PATH = "DeckConfig";
    private const string DECK_SELECTION_PATH = "DeckSelectionConfig";
    
    void Awake()
    {
        Cards = IO.LoadFromJson<CardDataList>(CARD_PATH);
        Decks = IO.LoadFromJson<DeckDataList>(DECK_PATH);
        DeckSelections = IO.LoadFromJson<DeckSelectionData>(DECK_SELECTION_PATH);
    }

    public List<CardData> GetDeckByName(string name)
    {
        List<CardData> cards = new List<CardData>();
        if(Decks != null && Decks.Decks != null)
        {
            var deck = Decks.Decks.FirstOrDefault(c => c.Name == name);

            if(deck != null)
            {
                foreach(var cardName in deck.CardList)
                {
                    var card = GetCardByName(cardName);

                    if(card != null)
                    {
                        cards.Add(card);
                    }
                    else
                    {
                        Debug.LogWarning("Card not found: " + cardName);
                    }
                }
            }
        }
        return cards;
    }

    public CardData GetCardByName(string name)
    {
        // EARLY OUT! //
        if(Cards == null || Cards.Cards == null) return null;

        var card = Cards.Cards.FirstOrDefault(c => c.Name == name);
        if(card == null)
        {
            Debug.Log("Card not found: " + name);
        }

        // If the card exists, return a modifiable clone of it.
        if(card != null)
        {
            card = card.Clone();

            // Should the config be doing this?  It's doesn't seem like exactly the right place, but works
            // for now.
            // Pre-load all card prefabs to avoid performance hitches during gameplay.
            if(!string.IsNullOrEmpty(card.PrefabName))
            {
                SL.Get<ResourceManager>().Load<GameObject>(Consts.UnitsPath + card.PrefabName);
            }

            if(!string.IsNullOrEmpty(card.GhostPrefabName))
            {
                SL.Get<ResourceManager>().Load<GameObject>(Consts.UnitGhostsPath + card.GhostPrefabName);
            }

            if(!string.IsNullOrEmpty(card.CardImageName))
            {
                SL.Get<ResourceManager>().Load<Sprite>(Consts.ImagePath + card.CardImageName);
            }
        }
        
        return card;
    }
}