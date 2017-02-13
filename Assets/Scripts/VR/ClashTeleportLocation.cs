using System;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

[Serializable]
public class TeleportSetEvent : UnityEvent<ClashTeleportLocation> { }

[RequireComponent(typeof(Interactable))]
public class ClashTeleportLocation : MonoBehaviour 
{
    public GameObject Destination;
    public TeleportSetEvent TeleportSetEvent;

    private void HandHoverUpdate( Hand hand )
    {
        if ( hand.GetStandardInteractionButtonDown() || ( ( hand.controller != null ) 
            && hand.controller.GetPressDown( Valve.VR.EVRButtonId.k_EButton_Grip ) ) )
        {
            TeleportSetEvent.Invoke(this);
        }
    }
}
