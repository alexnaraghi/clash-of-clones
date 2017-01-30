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
    /*
    [SerializeField] private GameObject _remoteHeadPrefab;
    [SerializeField] private GameObject _remoteControllerPrefab;
    */
    [SerializeField] private string _remoteLayer;

    [Header("References")]
    [SerializeField] private GameObject _vrObjectBase;
    [SerializeField] private GameObject _head;
    [SerializeField] private GameObject _hand1;
    [SerializeField] private GameObject _hand2;
    [SerializeField] private GameObject _fallbackObjectBase;
    [SerializeField] private GameObject _fallbackHead;
    [SerializeField] private GameObject _fallbackHand1;
    [SerializeField] private GameObject _fallbackHand2;

    [Header("Children")]
    [SerializeField] private GameObject _modelHead;
    [SerializeField] private GameObject _modelHand1;
    [SerializeField] private GameObject _modelHand2;

    [Header("Network")]
    [SerializeField] private NetworkTransformChild _netHead;
    [SerializeField] private NetworkTransformChild _netHand1;
    [SerializeField] private NetworkTransformChild _netHand2;
    
    

    private bool isUsingFallback;


    private const string gameObjectPrefix = "Player";

    // Use this for initialization
    public override void OnStartClient() 
    {
        base.OnStartClient();
	}

    private void Start()
    {
        // EARLY OUT! //
        if(this.DisabledFromMissingObject( 
            _head, _hand1, _hand2, _fallbackHead, _fallbackHand1))
        {
            return;
        }

        if(isLocalPlayer)
        {
            setLocal();
            setModels(isEnabled: false);
        }
        else
        {
            setRemote();
            setModels(isEnabled: true);
        }
        moveToVR();
    }

    private void Update()
    {
        // TODO: Make this event driven.
        if(isUsingFallback && _vrObjectBase.activeInHierarchy)
        {
            moveToVR();
            isUsingFallback = false;
        }
        else if(!isUsingFallback && _fallbackObjectBase.activeInHierarchy)
        {
            moveToFallback();
            isUsingFallback = true;
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

    private void moveToFallback()
    {
        _netHead.target = _fallbackHead.transform;
        _netHand1.target = _fallbackHand1.transform;
        _netHand2.target = _fallbackHand2.transform;
        _modelHead.transform.SetParent (_fallbackHead.transform,  worldPositionStays: false);
        _modelHand1.transform.SetParent(_fallbackHand1.transform, worldPositionStays: false);
        _modelHand2.transform.SetParent(_fallbackHand2.transform, worldPositionStays: false);

    }

    private void moveToVR()
    {
        _netHead.target = _head.transform;
        _netHand1.target = _hand1.transform;
        _netHand2.target = _hand2.transform;
        _modelHead.transform.SetParent (_head.transform,  worldPositionStays: false);
        _modelHand1.transform.SetParent(_hand1.transform, worldPositionStays: false);
        _modelHand2.transform.SetParent(_hand2.transform, worldPositionStays: false);
    }

    private void setModels(bool isEnabled)
    {
        _modelHead.SetActive (isEnabled);
        _modelHand1.SetActive(isEnabled);
        _modelHand2.SetActive(isEnabled);
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
