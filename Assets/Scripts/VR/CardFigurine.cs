using UnityEngine;
using System.Collections;
using System;
using VRTK;

public class CardFigurine : VRTK_InteractableObject 
{
    private CardData _data;

    public CardData Data { get { return _data; } }

    public void Init(CardData data)
    {
        _data = data; 
        
        isGrabbable = true;
        isUsable = true;
        useOnlyIfGrabbed = true;
        holdButtonToGrab = false;
    }

    public void DisableInteractions()
    {
        isGrabbable = false;
        isUsable = false;
    }

    protected override void OnEnable()
    {
        SL.Get<GameModel>().MyPlayer.ManaChangedEvent.AddListener(onManaChanged);
    }

    protected override void OnDisable()
    {
        if (SL.Exists && SL.Get<GameModel>() != null && SL.Get<GameModel>().MyPlayer.ManaChangedEvent != null)
        {
            SL.Get<GameModel>().MyPlayer.ManaChangedEvent.RemoveListener(onManaChanged);
        }
    }

    private void onManaChanged()
    {
        int mana = Mathf.FloorToInt(SL.Get<GameModel>().MyPlayer.Mana);

        if(_data != null)
        {
            setInteractable(_data.ManaCost <= mana);
        }
    }

    private void setInteractable(bool isInteractable)
    {
        // Only allow grabs on playable cards.
        isGrabbable = SL.Get<GameModel>().MyPlayer.CanPlayCard(_data);
    }
}
