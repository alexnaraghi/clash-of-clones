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
        rumbleOnTouch = new Vector2(500f, 1f);
        holdButtonToGrab = false;
        precisionSnap = true;
    }

    public void DisableInteractions()
    {
        isGrabbable = false;
        isUsable = false;
    }

    protected override void OnEnable()
    {
        GameModel.Instance.MyPlayer.ManaChangedEvent.AddListener(onManaChanged);
    }

    protected override void OnDisable()
    {
        GameModel.Instance.MyPlayer.ManaChangedEvent.RemoveListener(onManaChanged);
    }

    private void onManaChanged()
    {
        int mana = Mathf.FloorToInt(GameModel.Instance.MyPlayer.Mana);

        if(_data != null)
        {
            setInteractable(_data.ManaCost <= mana);
        }
    }

    private void setInteractable(bool isInteractable)
    {
        // Only allow grabs on playable cards.
        isGrabbable = GameModel.Instance.MyPlayer.CanPlayCard(_data);
    }
}
