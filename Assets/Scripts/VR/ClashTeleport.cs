using UnityEngine;
using System.Collections;
using VRTK;

/// <summary>
/// VRTK's teleport logic is way too wacky/convoluted for what we want to do.
/// All a teleport has to do is adjust the camera rig to the destination marker's transform.  That's it.
/// </summary>
public class ClashTeleport: MonoBehaviour
{
    public event TeleportEventHandler Teleporting;
    public event TeleportEventHandler Teleported;

    public void InitDestinationSetListener(GameObject markerMaker, bool register)
    {
        if (markerMaker)
        {
            foreach (var worldMarker in markerMaker.GetComponents<VRTK_DestinationMarker>())
            {
                if (register)
                {
                    worldMarker.DestinationMarkerSet += new DestinationMarkerEventHandler(DoTeleport);
                }
                else
                {
                    worldMarker.DestinationMarkerSet -= new DestinationMarkerEventHandler(DoTeleport);
                }
            }
        }
    }

    protected virtual void Awake()
    {
        // Is this necessary?  Found it in vrtk basic teleport.
        Utilities.SetPlayerObject(gameObject, VRTK_PlayerObject.ObjectTypes.CameraRig);
    }

    protected virtual void OnEnable()
    {
        // Again, this is VRTK behavior.  I'm guessing it is so destination markers can be registered
        // on the same frame as this.
        StartCoroutine(InitListenersAtEndOfFrame());
    }

    protected virtual void OnDisable()
    {
        InitDestinationMarkerListeners(false);
    }

    protected void OnTeleporting(object sender, DestinationMarkerEventArgs e)
    {
        if (Teleporting != null)
        {
            Teleporting(this, e);
        }
    }

    protected void OnTeleported(object sender, DestinationMarkerEventArgs e)
    {
        if (Teleported != null)
        {
            Teleported(this, e);
        }
    }

    private void InitDestinationMarkerListeners(bool state)
    {
        var leftHand = VRTK_DeviceFinder.GetControllerLeftHand();
        var rightHand = VRTK_DeviceFinder.GetControllerRightHand();
        InitDestinationSetListener(leftHand, state);
        InitDestinationSetListener(rightHand, state);
        foreach (var destinationMarker in VRTK_ObjectCache.registeredDestinationMarkers)
        {
            if (destinationMarker.gameObject != leftHand && destinationMarker.gameObject != rightHand)
            {
                InitDestinationSetListener(destinationMarker.gameObject, state);
            }
        }
    }

    private IEnumerator InitListenersAtEndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        InitDestinationMarkerListeners(true);
    }

    private void DoTeleport(object sender, DestinationMarkerEventArgs e)
    {
        if(e.target != null)
        {
            OnTeleporting(sender, e);
            transform.position = e.target.position;
            transform.rotation = e.target.rotation;
            transform.localScale = e.target.localScale;
            OnTeleported(sender, e);
        }
    }
}