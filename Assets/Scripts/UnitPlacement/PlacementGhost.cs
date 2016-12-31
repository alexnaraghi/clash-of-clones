using UnityEngine;
using UnityEngine.Events;
using VRStandardAssets.Utils;

/// <summary>
/// Display to allow the human player to place a unit.
/// </summary>
public class PlacementGhost : MonoBehaviour 
{
    /// <summary>
    /// The sound when the player tries to place but doesn't have enough mana.
    /// </summary>
    [SerializeField] private AudioSource _errorSound;

    /// <summary>
    /// The territory display so we can show enemy territory when we are placing.
    /// </summary>
    [SerializeField] private TerritoryUI _territoryGui;

    [SerializeField] private VRInput _vrInput;
    [SerializeField] private Reticle _reticle;

    
    /// <summary>
    /// The card that is being placed.
    /// </summary>
    private CardData _card;

    /// <summary>
    /// The ghost's model.
    /// </summary>
    public GameObject Model;

    public bool IsValidPlacementPosition;

    /// <summary>
    /// Triggered on successful placement of a card.
    /// </summary>
    public UnityEvent PlacedEvent;

    public CardData Card
    {
        get { return _card; }
    }

    void OnEnable()
    {
        _vrInput.OnClick.AddListener(onClick);
    }

    void OnDisable()
    {
        _vrInput.OnClick.RemoveListener(onClick);
    }

    private void onClick()
    {
        // If we have something to place.
        if(Model != null)
        {
            // If the model is active, it is in a placeable location.
            if(IsValidPlacementPosition && GameModel.Instance.MyPlayer.CanPlayCard(_card))
            {
                GameModel.Instance.MyPlayer.PlayCard(_card, transform.position);
                PlacedEvent.Invoke();
                clear();
                IsValidPlacementPosition = false;
            }
            else
            {
                // Otherwise, let the user know the placement is invalid.
                if(_errorSound != null)
                {
                    // This is too annoying for now.
                    //_errorSound.Play();
                }
            }
        }
    }

    /// <summary>
    /// Set the ghost to a card.  If the card is null, clears the ghost.
    /// </summary>
    public void SetCard(CardData card, PlayerModel player)
    {
        if(card != null)
        {
            clear();
            _card = card;

            // Set the ghost model.
            var prefab = Resources.Load<GameObject>(Consts.UnitGhostsPath + card.GhostPrefabName);

            if(prefab != null)
            {
                Model = Utils.Instantiate(prefab, Vector3.zero, Quaternion.identity);
                if(Model != null)
                {
                    Model.transform.SetParent(transform, false);
                    player.RotateForPlayer(Model);

                    if(!_card.IsProjectile)
                    {
                        _territoryGui.IsShowingEnemyTerritory = true;
                    }
                }
            }
            else
            {
                Debug.LogWarning("Could not find ghost: " + card.GhostPrefabName);
            }
        }
        else
        {
            Debug.LogWarning("Cannot set card to null.  Did you mean to unset card?");
        }

        if(_reticle != null)
        {
            _reticle.IsRaycasting = false;
        }
    }

    /// <summary>
    /// If the given card is active, unset it.  If it's a different card, do nothing.
    /// </summary>
    /// <param name="card"></param>
    public void UnsetCard(CardData card)
    {
        if(card != null && _card == card)
        {
            clear();
        }

        if(_reticle != null)
        {
            _reticle.IsRaycasting = true;
        }
    }

    /// <summary>
    /// Clear the ghost.
    /// </summary>
    private void clear()
    {
        // Get rid of the old ghost.
        if(Model != null)
        {
            Destroy(Model);
        }
        _card = null;

        _territoryGui.IsShowingEnemyTerritory = false;
    }
}
