using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;

public class Config : MonoBehaviour 
{
    public static Config Instance;

    public CardDataList Cards;
    public DeckDataList Decks;

    private const string CARD_PATH = "Data/CardConfig";
    private const string DECK_PATH = "Data/DeckConfig";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("More than one card config exists!  Destroying...");
            Destroy(this);
        }
    }

    void Start()
    {
        Cards = IO.LoadFromJson<CardDataList>(CARD_PATH);
        Decks = IO.LoadFromJson<DeckDataList>(DECK_PATH);
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
        return card;
    }
}