using System;
using UnityEngine;

/// <summary>
/// Keeps the hand display up to date.
/// </summary>
public class CardPanel : MonoBehaviour 
{
    public CardUI[] Hand;
    public CardUI PeekCard;

    void Start()
    {
        SL.Get<GameModel>().MyPlayer.CardState.HandChangedEvent.AddListener(onHandChanged);
        SL.Get<GameModel>().MyPlayer.ManaChangedEvent.AddListener(onManaChanged);
    }

    private void onHandChanged()
    {
        var handState = SL.Get<GameModel>().MyPlayer.CardState.Hand;

        // EARLY OUT! //
        if(handState.Length != Hand.Length)
        {
            Debug.LogWarning("Hand length and card UI are not the same length!: " + handState.Length + "-" + Hand.Length);
            return;
        }

        for(int i = 0; i < Hand.Length; i++)
        {
            var visibleCard = Hand[i].Definition;
            var handCard = handState[i];

            if(visibleCard != handCard)
            {
                //Change card.
                Hand[i].Init(handCard);
            }
        }

        var peekCard = SL.Get<GameModel>().MyPlayer.CardState.Peek();
        if(peekCard != null)
        {
            PeekCard.Init(peekCard);
        }
    }

    private void onManaChanged()
    {
        var handState = SL.Get<GameModel>().MyPlayer.CardState.Hand;
        int mana = Mathf.FloorToInt(SL.Get<GameModel>().MyPlayer.Mana);

        for(int i = 0; i < Hand.Length; i++)
        {
            var handCard = handState[i];
            if(handCard != null)
            {
                Hand[i].SetInteractable(handCard.ManaCost <= mana);
            }
        }
    }
}