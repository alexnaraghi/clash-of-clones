using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// All a teleport has to do is adjust the camera rig to the destination marker's transform.
/// </summary>
public class ClashTeleport: MonoBehaviour
{
    public UnityEvent TeleportingEvent;
    public UnityEvent TeleportedEvent;

    public Transform TeleportingLocation;
    private ClashTeleportLocation[] _locations;

    private void Start()
    {
        _locations = GameObject.FindObjectsOfType<ClashTeleportLocation>();

        foreach(var location in _locations)
        {
            location.TeleportSetEvent.AddListener(onTeleporting);
        }
    }

    private void OnDestroy()
    {
        if(_locations != null)
        {
            foreach(var location in _locations)
            {
                location.TeleportSetEvent.RemoveListener(onTeleporting);
            }
        }
    }

    private void onTeleporting(ClashTeleportLocation location)
    {
        if(location != null && location.Destination != null)
        {
            var destination = location.Destination.transform;
            TeleportingLocation = destination;

            TeleportingEvent.Invoke();

            transform.position = destination.position;
            transform.rotation = destination.rotation;
            transform.localScale = destination.localScale;

            TeleportedEvent.Invoke();
            TeleportingLocation = null;
        }
    }
}