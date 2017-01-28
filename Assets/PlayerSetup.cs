using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour 
{
    [Header("Local")]
    [SerializeField] private Behaviour[] _localBehavioursToDisable;
    [SerializeField] private GameObject[] _localGameObjectsToDisable;
    [SerializeField] private string _localLayer;

    [Header("Remote")]
    [SerializeField] private Behaviour[] _remoteBehavioursToDisable;
    [SerializeField] private GameObject[] _remoteGameObjectsToDisable;
    [SerializeField] private GameObject _remoteHeadPrefab;
    [SerializeField] private GameObject _remoteControllerPrefab;
    [SerializeField] private string _remoteLayer;

    [Header("References")]
    [SerializeField] private GameObject _head;
    [SerializeField] private GameObject _hand1;
    [SerializeField] private GameObject _hand2;

    private const string gameObjectPrefix = "Player";

    // Use this for initialization
    public override void OnStartClient() 
    {
        base.OnStartClient();
	}

    private void Start()
    {
        // EARLY OUT! //
        if(this.DisabledFromMissingObject(_remoteHeadPrefab, _remoteControllerPrefab))
        {
            return;
        }

        if(isLocalPlayer)
        {
            setLocal();
        }
        else
        {
            // I guess we should allow these to be disabled too?  Would be nice to freely switch between
            // local and remote, but honestly maybe that won't happen.
            var remoteHead =        Instantiate(_remoteHeadPrefab,       _head.transform);
            var remoteController1 = Instantiate(_remoteControllerPrefab, _hand1.transform);
            var remoteController2 = Instantiate(_remoteControllerPrefab, _hand2.transform);

            setRemote();
        }
    }
    
    private void setLocal()
    {
        gameObject.name = string.Format("{0} {1} ({2})", "Local", gameObjectPrefix, netId);
        gameObject.layer = LayerMask.NameToLayer(_localLayer);

        setBehaviours (_localBehavioursToDisable,   isEnabled: false);
        setGameObjects(_localGameObjectsToDisable,  isEnabled: false);
        setBehaviours (_remoteBehavioursToDisable,  isEnabled: true);
        setGameObjects(_remoteGameObjectsToDisable, isEnabled: true);
    }

    private void setRemote()
    {
        gameObject.name = string.Format("{0} {1} ({2})", "Remote", gameObjectPrefix, netId);
        gameObject.layer = LayerMask.NameToLayer(_remoteLayer);
        
        setBehaviours (_localBehavioursToDisable,   isEnabled: true);
        setGameObjects(_localGameObjectsToDisable,  isEnabled: true);
        setBehaviours (_remoteBehavioursToDisable,  isEnabled: false);
        setGameObjects(_remoteGameObjectsToDisable, isEnabled: false);
    }

    private void setBehaviours(Behaviour[] behaviours, bool isEnabled)
    {
        // EARLY OUT! //
        if(behaviours == null) return;

        foreach(var be in behaviours)
        {
            be.enabled = isEnabled;
        }
    }

    private void setGameObjects(GameObject[] gameObjects, bool isEnabled)
    {
        // EARLY OUT! //
        if(gameObject == null) return;

        foreach(var go in gameObjects)
        {
            go.SetActive(isEnabled);
        }
    }
}
