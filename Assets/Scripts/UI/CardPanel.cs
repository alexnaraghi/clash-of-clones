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
        GameModel.Instance.MyPlayer.CardState.HandChangedEvent.AddListener(onHandChanged);
        GameModel.Instance.MyPlayer.ManaChangedEvent.AddListener(onManaChanged);
    }

    private void onHandChanged()
    {
        var handState = GameModel.Instance.MyPlayer.CardState.Hand;

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

        var peekCard = GameModel.Instance.MyPlayer.CardState.Peek();
        if(peekCard != null)
        {
            PeekCard.Init(peekCard);
        }
    }

    private void onManaChanged()
    {
        var handState = GameModel.Instance.MyPlayer.CardState.Hand;
        int mana = Mathf.FloorToInt(GameModel.Instance.MyPlayer.Mana);

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