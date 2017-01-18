using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Valve.VR.InteractionSystem;
using UnityEngine.Events;
using VRTK.Highlighters;

[Serializable]
public class FigurinePlacedEvent : UnityEvent<Vector3> { }

/// <summary>
/// Interactable elements of a card figurine.  Allows it to be picked up and placed on the grid.
/// Note that figurines are just the visual pre-spawn components (ghosts) of an entity.  Once placed on the 
/// board it needs to be destroyed and replaced by a real working entity.
/// </summary>
[RequireComponent(typeof(Interactable))]
public class CardFigurine : MonoBehaviour 
{
    public Color HighlightColor;
    public ushort HapticSpawnDrop = 1500;
    public ushort HapticGridSquareChange = 500;

    // Statically attached.
    private Interactable _interactable;
    private VRTK_BaseHighlighter _highlighter;

    // Added on init.
    private CardData _data;

    // Transform information for before the game object was grabbed.
    // We used this to return it to it's original slot after being ungrabbed.
    private Vector3 _originalLocalPosition;
    private Quaternion _originalLocalRotation;

    // A grid snapper is dynamically attached to the figurine while grabbed so we can track which square we 
    // will be placing into).
    private GridSnapVR _snapper;

    // Added during times when the object should not be interacted with, like once it's placed onto the board.
    private IgnoreHovering _ignoreHovering;

    // Drop onto board context.
    private float _elapsedSpawnSeconds;
    private const float _spawnMetersPerSecond = 150f;

    private Hand.AttachmentFlags attachmentFlags = Hand.defaultAttachmentFlags 
        & ( ~Hand.AttachmentFlags.SnapOnAttach ) 
        & ( ~Hand.AttachmentFlags.DetachOthers );

    // The figurine has been placed on the board and has floated to its spawn position.  Ready to spawn.
    public FigurinePlacedEvent FigurinePlacedEvent;

    // The data of this card.
    public CardData Data { get { return _data; } }

    // Initialize the figurine + mesh using the definition of the card.
    public void Init(CardData data)
    {
        _data = data;

        var prefab = SL.Get<ResourceManager>().Load<GameObject>(Consts.UnitGhostsPath + data.GhostPrefabName);
        if(prefab != null)
        {
            var go = Utils.Instantiate(prefab, transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
        }

        if(_highlighter != null)
        {
            _highlighter.ResetHighlighter();
        }
    }

    // Changes whether this figurine can be interacted with by the player's hands.
    public void SetInteractable( bool isInteractable)
    {
        if(isInteractable)
        {
            if(_ignoreHovering != null)
            {
                Destroy(_ignoreHovering);
                _ignoreHovering = null;
            }
        }
        else
        {

            if(_ignoreHovering == null)
            {
                _ignoreHovering = gameObject.AddComponent<IgnoreHovering>();
            }
        }
    }

    private void Awake()
    {
        _interactable = gameObject.GetComponent<Interactable>();

        // Optional.
        _highlighter = gameObject.GetComponent<VRTK_BaseHighlighter>();
        if(_highlighter != null)
        {
            _highlighter.Initialise(null, new Dictionary<string, object>(){ {"resetMainTexture", true} });
        }
    }

    // Is the figurine in a state where it can be placed?
    private bool canPlaceFigurineOnBoard()
    {
        bool canPlace = false;
        if(_snapper != null && _snapper.IsOverBoard)
        {
            bool canPlayCard = SL.Get<GameModel>().MyPlayer.CanPlayCard(Data);
            if (canPlayCard)
            {
                if (isPlaceableTerritory(_snapper.WorldPosition))
                {
                    canPlace = true;
                }
            }
        }
        return canPlace;
    }

    // Detatches the object and places it on the board.
    private void detachToBoard(Hand hand)
    {
        // EARLY OUT! //
        if(_snapper == null)
        {
            Debug.LogWarning(string.Format("Cannot use figurine, missing data. Snapper:{0}", _snapper));
            return;
        }

        var gridPoint = _snapper.GridPoint;
        var snappedPosition = TerritoryData.GetCenter(gridPoint.X, gridPoint.Y);

        // Send the figurine down to its spawn position.
        var pos = transform.position;
        SetInteractable(false);

        StartCoroutine(dropFigurineToSpawn(pos, snappedPosition, hand));
    }

    // Sends the figurine down to the destination position over time.  Once down, actually spawns the entity
    // and destroys the figurine.
    // Assumes the figurine is no longer grabbed.
    private IEnumerator dropFigurineToSpawn(Vector3 startPos, Vector3 destination, Hand hand)
    {
        _elapsedSpawnSeconds = 0f;
        float totalMeters = Vector3.Distance(startPos, destination);
        float _totalSeconds = totalMeters / _spawnMetersPerSecond;

        while(_elapsedSpawnSeconds < _totalSeconds)
        {
            hand.controller.TriggerHapticPulse( HapticSpawnDrop );

            var currentPos = SteamVR_Utils.Lerp(startPos, destination, _elapsedSpawnSeconds / _totalSeconds);
            transform.position = currentPos;

            _elapsedSpawnSeconds += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        FigurinePlacedEvent.Invoke(destination);
    }

    // Is the territory is NOT controlled by the enemy ie friendly or neutral (but projectiles can go anwywhere).
    private bool isPlaceableTerritory(Vector3 position)
    {
        return Data.IsProjectile
            || !SL.Get<GameModel>().EnemyPlayer.IsInTerritory(position);
    }

    // Triggered when the grid square changes that the figurine is over.
    // Updates the visual/haptic components of placement snapping.
    private void onFigurineGridSquareChanged()
    {
        if(_snapper != null)
        {
            bool isHighlightActive = _snapper.IsOverBoard && isPlaceableTerritory(_snapper.WorldPosition);
            SL.Get<GridSquareHighlight>().gameObject.SetActive(isHighlightActive);

            if(isHighlightActive)
            {
                var center = TerritoryData.GetCenter(_snapper.GridPoint.X, _snapper.GridPoint.Y);
                var snappedPos = new Vector3(center.x, 1f, center.z);
                SL.Get<GridSquareHighlight>().transform.position = snappedPos;

                // Trigger haptic pulse when the grid square changes.
                Hand hand = GetComponentInParent<Hand>();
				if ( hand && hand.controller != null )
				{
                    hand.controller.TriggerHapticPulse( HapticGridSquareChange );
				}
            }
        }
    }

#region Valve Hand Events
    //-------------------------------------------------
    // Called every Update() while a Hand is hovering over this object
    //-------------------------------------------------
    private void HandHoverUpdate( Hand hand )
    {
        if ( hand.GetStandardInteractionButtonDown() || ( ( hand.controller != null ) && hand.controller.GetPressDown( Valve.VR.EVRButtonId.k_EButton_Grip ) ) )
        {
            if ( hand.currentAttachedObject != gameObject )
            {
                // Save our position/rotation so that we can restore it when we detach
                _originalLocalPosition = transform.localPosition;
                _originalLocalRotation = transform.localRotation;

                // Call this to continue receiving HandHoverUpdate messages,
                // and prevent the hand from hovering over anything else
                hand.HoverLock( _interactable );

                // Attach this object to the hand
                hand.AttachObject( gameObject, attachmentFlags );
            }
            else
            {
                // Detach this object from the hand
                hand.DetachObject( gameObject );

                // Call this to undo HoverLock
                hand.HoverUnlock( _interactable );

                if(canPlaceFigurineOnBoard())
                {
                    detachToBoard(hand);
                }
                else
                {
                    // Restore position/rotation
                    transform.localPosition = _originalLocalPosition;
                    transform.localRotation = _originalLocalRotation;
                }
            }
        }
    }

    //-------------------------------------------------
    // Called when this GameObject becomes attached to the hand
    //-------------------------------------------------
    private void OnAttachedToHand( Hand hand )
    {
        if(hand != null)
        {
            // Attach a snapper so we know if we are over the board.
            if(_snapper != null)
            {
                _snapper.enabled = true;
            }
            else
            {
                _snapper = gameObject.AddComponent<GridSnapVR>();
            }
            _snapper.GridSquareChangedEvent.AddListener(onFigurineGridSquareChanged);

            if(!Data.IsProjectile)
            {
                SL.Get<TerritoryUI>().Show();
            }
        }

        unhighlight();
    }

    //-------------------------------------------------
    // Called when this GameObject is detached from the hand
    //-------------------------------------------------
    private void OnDetachedFromHand( Hand hand )
    {
        if(hand != null)
        {
            if(_snapper != null)
            {
                _snapper.GridSquareChangedEvent.RemoveListener(onFigurineGridSquareChanged);
                _snapper.enabled = false;
            }

            SL.Get<TerritoryUI>().Hide();
            SL.Get<GridSquareHighlight>().gameObject.SetActive(false);
        }
    }
#endregion



    private void OnHandHoverBegin()
    {
        highlight();
    }


    //-------------------------------------------------
    private void OnHandHoverEnd()
    {
        unhighlight();
    }

    private void highlight()
    {
        if(_highlighter != null)
        {
            _highlighter.Highlight(HighlightColor);
        }
    }

    private void unhighlight()
    {
        if (_highlighter != null)
        {
            _highlighter.Unhighlight(null);
        }
    }
}
