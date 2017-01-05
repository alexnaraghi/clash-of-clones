using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using VRTK;

// TODO:  This script does quite a few unrelated things.  I should break this up into several scripts for each
// visual or interaction component of a slot.  The slot should just contain logic for when a figurine is 
// grabbed, used, etc.

/// <summary>
/// Slot for a card that can be interacted with.
/// </summary>
public class CardFigurineSlot : MonoBehaviour 
{
    public int HandIndex;
    public GameObject LockedGameObject;

    private CardFigurine _figurine;

    // A grid snapper is dynamically attached to the figurine while grabbed so we can track which square we 
    // will be placing into).
    private GridSnapVR _snapper;
    private GameObject _placementHighlighter;
    private GameObject _territoryUI;

    private float _elapsedSpawnSeconds;
    private const float _spawnMetersPerSecond = 150f;

    public UnityEvent CardChangedEvent;

    public CardFigurine Figurine
    {
        get
        {
            return _figurine;
        }
    }

    private void Awake()
    {
        if(GameModel.Instance.MyPlayer.CardState != null)
        {
            GameModel.Instance.MyPlayer.CardState.CardChangedEvent.AddListener(onCardChanged);
        }

        _placementHighlighter = GameModel.Instance.GridHighlight.gameObject;
        _territoryUI = GameModel.Instance.TerritoryCanvas.gameObject;
    }

    private void OnEnable()
    {
        GameModel.Instance.MyPlayer.ManaChangedEvent.AddListener(onManaChanged);
    }

    private void OnDisable()
    {
        GameModel.Instance.MyPlayer.ManaChangedEvent.RemoveListener(onManaChanged);
    }

    private void OnDestroy()
    {
        if (GameModel.Instance != null && GameModel.Instance.MyPlayer.CardState != null)
        {
            GameModel.Instance.MyPlayer.CardState.CardChangedEvent.RemoveListener(onCardChanged);
        }
    }

    private void onCardChanged(int index)
    {
        if(index == HandIndex)
        {
            var handState = GameModel.Instance.MyPlayer.CardState.Hand;

            // EARLY OUT! //
            if(handState == null)
            {
                Debug.LogWarning("Hand is null");
                return;
            }

            // Destroy the old figurine.
            destroyFigurine();

            var data = handState[index];
            var go = createFigurineFromDefinition(data);
            if(go != null)
            {
                _figurine = go.AddComponent<CardFigurine>();
                _figurine.Init(data);
                registerFigurine();
            }

            CardChangedEvent.Invoke();
        }
    }

    private GameObject createFigurineFromDefinition(CardData data)
    {
        var prefab = ResourceManager.Instance.Load<GameObject>(Consts.UnitGhostsPath + data.GhostPrefabName);

        GameObject go = null;
        if(prefab != null)
        {
            // Parent to this slot.
            go = Utils.Instantiate(prefab, transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
        }

        return go;
    }

    private void destroyFigurine()
    {
        if(_figurine != null)
        {
            // If destroyed while held.
            _figurine.InteractableObjectUsed -= OnFigurineUsed;

            unregisterFigurine();
            Destroy(_figurine.gameObject);
            _figurine = null;
        }
    }

    // Assume figurine exists.
    private void registerFigurine()
    {
        _figurine.InteractableObjectGrabbed += onGrabbed;
        _figurine.InteractableObjectUngrabbed += onUngrabbed;
    }

    // Assume figurine exists.
    private void unregisterFigurine()
    {
        _figurine.InteractableObjectGrabbed -= onGrabbed;
        _figurine.InteractableObjectUngrabbed -= onUngrabbed;
    }

    private void onGrabbed(object sender, InteractableObjectEventArgs e)
    {
        if(_figurine != null)
        {
            // Don't allow a card to be picked up if we don't have enough mana.
            // TODO: Add visual indication to the player.
            bool canPlayCard = GameModel.Instance.MyPlayer.CanPlayCard(_figurine.Data);
            if(canPlayCard)
            {
                _figurine.transform.SetParent(null);
                _figurine.InteractableObjectUsed += OnFigurineUsed;

                // Attach a snapper so we know if we are over the board.
                _snapper = _figurine.GetComponent<GridSnapVR>();
                if(_snapper != null)
                {
                    _snapper.enabled = true;
                }
                else
                {
                    _snapper = _figurine.gameObject.AddComponent<GridSnapVR>();
                }
                _snapper.GridSquareChangedEvent.AddListener(OnFigurineGridSquareChanged);

                if(_territoryUI != null)
                {
                    _territoryUI.SetActive(true);
                }
            }
        }
    }

    private void onUngrabbed(object sender, InteractableObjectEventArgs e)
    {
        ungrab(shouldReturnToSlot : true);
    }

    /// <summary>
    /// Ungrabs the figurine.
    /// </summary>
    /// <param name="shouldReturnToSlot">If true, returns the figurine to the slot and repositions.
    /// If false, the figurine will be unparented.</param>
    private void ungrab(bool shouldReturnToSlot)
    {
        if(_figurine != null)
        {
            _figurine.InteractableObjectUsed -= OnFigurineUsed;
            if(shouldReturnToSlot)
            {
                _figurine.transform.SetParent(transform);
                _figurine.transform.localPosition = Vector3.zero;
                _figurine.transform.localRotation = Quaternion.identity;
            }
            else
            {
                _figurine.transform.SetParent(null);
            }
        }
        
        if(_snapper != null)
        {
            _snapper.enabled = false;
            _snapper.GridSquareChangedEvent.RemoveListener(OnFigurineGridSquareChanged);
            _snapper = null;
        }

        if(_placementHighlighter != null)
        {
            _placementHighlighter.SetActive(false);
        }

        if(_territoryUI != null)
        {
            _territoryUI.SetActive(false);
        }
    }

    private void OnFigurineGridSquareChanged()
    {
        if(_placementHighlighter != null && _snapper != null)
        {
            bool isHighlightActive = _snapper.IsOverBoard && isPlaceableTerritory(_snapper.WorldPosition);
            _placementHighlighter.SetActive(isHighlightActive);

            if(isHighlightActive)
            {
                var snappedPos = new Vector3(_snapper.WorldPosition.x, 1f, _snapper.WorldPosition.z);
                _placementHighlighter.transform.position = snappedPos;
            }
        }
    }

    private void OnFigurineUsed(object sender, InteractableObjectEventArgs e)
    {
        // EARLY OUT! //
        if(_figurine == null || _snapper == null)
        {
            Debug.LogWarning(string.Format("Cannot use figurine, missing data. Figurine:{0} Snapper:{1}", _figurine, _snapper));
            return;
        }

        var card = _figurine.Data;
        bool canPlayCard = GameModel.Instance.MyPlayer.CanPlayCard(card);
        if(canPlayCard && _snapper.IsOverBoard)
        {
            var gridPoint = _snapper.GridPoint;

            if(isPlaceableTerritory(_snapper.WorldPosition))
            {
                var snappedPosition = TerritoryData.GetCenter(gridPoint.X, gridPoint.Y);

                // Send the figurine down to its spawn position.
                var pos = _figurine.transform.position;
                _figurine.ForceStopInteracting();
                _figurine.DisableInteractions();
                ungrab(shouldReturnToSlot : false);

                StartCoroutine(DropFigurineToSpawn(pos, snappedPosition));
            }
        }
    }

    private bool isPlaceableTerritory(Vector3 position)
    {
        // If the territory is NOT controlled by the enemy ie friendly or neutral, except projectiles.
            return _figurine.Data.IsProjectile
                || !GameModel.Instance.EnemyPlayer.IsInTerritory(position);
    }

    // Sends the figurine down to the destination position over time.  Once down, actually spawns the entity
    // and destroys the figurine.
    // Assumes the figurine is no longer grabbed.
    IEnumerator DropFigurineToSpawn(Vector3 startPos, Vector3 destination)
    {
        _elapsedSpawnSeconds = 0f;
        float totalMeters = Vector3.Distance(startPos, destination);
        float _totalSeconds = totalMeters / _spawnMetersPerSecond;

        while(_elapsedSpawnSeconds < _totalSeconds)
        {
            var currentPos = SteamVR_Utils.Lerp(startPos, destination, _elapsedSpawnSeconds / _totalSeconds);
            _figurine.transform.position = currentPos;

            _elapsedSpawnSeconds += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        // Cleanup the slot before getting a new card from the hand.
        var figurine = _figurine;
        destroyFigurine();

        // This will cause a card change event to be invoked which will re-populate the slot
        // automatically.
        GameModel.Instance.MyPlayer.PlayCard(figurine.Data, destination);
    }

    private void onManaChanged()
    {
        int mana = Mathf.FloorToInt(GameModel.Instance.MyPlayer.Mana);

        if(_figurine != null)
        {
            LockedGameObject.SetActive(_figurine.Data.ManaCost > mana);
        }
    }
}