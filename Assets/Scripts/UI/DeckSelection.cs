using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DeckSelection : MonoBehaviour 
{
    public GameObject CollectionContent; 
    public GameObject DeckContent; 

    [SerializeField] private DeckSelectionElement SelectionElementPrefab;
    
    private List<DeckSelectionElement> _cards = new List<DeckSelectionElement>();

    private List<DeckSelectionElement> _deckCards = new List<DeckSelectionElement>();
    private List<DeckSelectionElement> _collectionCards = new List<DeckSelectionElement>();

	// Use this for initialization
	void Start () 
    {
        // EARLY OUT! //
        if(SelectionElementPrefab == null) return;

        loadCards();
	}

    private void loadCards()
    {
        var availableCards = SL.Get<Config>().DeckSelections.Cards;
        foreach(var cardName in availableCards)
        {
            var card = SL.Get<Config>().GetCardByName(cardName);
            var element = Instantiate(SelectionElementPrefab);
            if(card != null && element != null)
            {
                element.Init(card);
                
                // Put the card in the collection by default.
                element.transform.SetParent(CollectionContent.transform, worldPositionStays: false);
                element.SelectedEvent.AddListener(() => Toggle(element));

                _cards.Add(element);
                _collectionCards.Add(element);
            }
        }
    }

    public void PopulateDeck(DeckData deck)
    {
        foreach(var card in deck.CardList)
        {
            // Do something to select the cards.
        }
    }

    public void Toggle(DeckSelectionElement element)
    {
        if(_deckCards.Count < Consts.DeckSize && _collectionCards.Remove(element))
        {
            _deckCards.Add(element);
            element.transform.SetParent(DeckContent.transform, worldPositionStays: false);
        }
        else if(_deckCards.Remove(element))
        {
            _collectionCards.Add(element);
            element.transform.SetParent(CollectionContent.transform, worldPositionStays: false);
        }
    }

    public bool IsDeckComplete()
    {
        return _deckCards.Count == 8;
    }

    public List<CardData> GetDeckList()
    {
        var cards = new List<CardData>();
        foreach(var card in _deckCards)
        {
            cards.Add(card.Definition);
        }

        return cards;
    }
}
