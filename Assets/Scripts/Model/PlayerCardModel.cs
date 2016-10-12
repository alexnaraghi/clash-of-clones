using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Represents the state of the game with respect to the players' cards.
/// </summary>
public class PlayerCardModel : MonoBehaviour 
{
    public List<CardDefinition> Deck;
    public List<CardDefinition> Discard;
    public CardDefinition[] Hand;

    void Awake()
    {
        Deck = new List<CardDefinition>();
        Discard = new List<CardDefinition>();
        Hand = new CardDefinition[Consts.HandSize];
    }

    public void Init(List<CardDefinition> deck)
    {
        // EARLY OUT! //
        if(deck == null || deck.Count == 0)
        {
            Debug.LogWarning("Can't do anything without a deck.");
            return;
        }
        
        // Clone the start deck so we can manipulate it.
        Discard = deck.ToList();
        Shuffle();
        initHand();
    }

    public void PlayCard(CardDefinition card)
    {
        int index = Hand.IndexOf(card);
        if(index != -1)
        {
            replaceInHand(index, card);
        }
        else
        {
            Debug.LogWarning("Couldn't find card in hand: " + card.PrefabName);
        }
    }

    public CardDefinition Peek()
    {
        if(Deck.Count == 0)
        {
            Shuffle();
        }

        return Deck.Last();
    }

    /// <summary>
    /// Get a random card from the hand.  Useful for basic AI card selection, not much else.
    /// </summary>
    public CardDefinition GetRandomCardFromHand()
    {
        int rand = Random.Range(0, Hand.Length);
        return Hand[rand];
    }

    /// <summary>
    /// Shuffle the discard pile and turn it into the draw pile.
    /// </summary>
    public void Shuffle()
    {
        //Debug.Log("Shuffling discard into deck, Count: " + Discard.Count);
        
        // Randomized shuffle.
        Deck = Discard.OrderBy(c => Random.value).ToList();
        Discard.Clear();
    }

    private void initHand()
    {
        for(int i = 0; i < Hand.Length; i++)
        {
            if(Hand[i] != null)
            {
                Debug.LogWarning("You should only be filling an empty hand, but it had cards in it!");
                Discard.Add(Hand[i]);
            }

            Hand[i] = drawCard();
        }
    }

    private void replaceInHand(int handIndex, CardDefinition newCard)
    {
        if(Hand[handIndex] != null)
        {
            Discard.Add(newCard);
        }
        Hand[handIndex] = drawCard();
    }

    private CardDefinition drawCard()
    {
        if(Deck.Count == 0)
        {
            Shuffle();
        }

        var top = Deck.Last();
        Deck.RemoveAt(Deck.Count - 1);
        return top;
    }
}
