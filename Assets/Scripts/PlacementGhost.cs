using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// UnitPlacement
/// </summary>
public class PlacementGhost : MonoBehaviour 
{
    public int Width;
    public int Height;
    public GameObject Model;
    public UnityEvent PlacedEvent;

    [SerializeField] private AudioSource _errorSound;
    [SerializeField] private TerritoryGui _territoryGui;
    private CardDefinition _card;

    void Update()
    {
        // If the model is active, it is in a placeable location.
        if(Input.GetMouseButtonDown(0) && Model != null && Model.activeSelf)
        {
            if(GameState.Instance.MyPlayer.CanPlayCard(_card))
            {
                GameState.Instance.MyPlayer.PlayCard(_card, transform.position);
                PlacedEvent.Invoke();
                clear();
            }
            else
            {
                // Otherwise, let the user know the placement is invalid.
                if(_errorSound != null)
                {
                    _errorSound.Play();
                }
            }
        }
    }

    /// <summary>
    /// Set the ghost to a card.  If the card is null, clears the ghost.
    /// </summary>
    public void SetCard(CardDefinition card, Player player)
    {
        if(card != null)
        {
            clear();
            _card = card;

            // Set the ghost model.
            var prefab = Resources.Load<GameObject>(Consts.UnitGhostsPath + card.GhostPrefabName);

            if(prefab != null)
            {
                Model = (GameObject)Instantiate(prefab, Vector3.zero, Quaternion.identity);
                if(Model != null)
                {
                    Model.transform.SetParent(transform, false);
                    player.RotateForPlayer(Model);
                    _territoryGui.IsShowingEnemyTerritory = true;
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
    }

    /// <summary>
    /// If the given card is active, unset it.  If it's a different card, do nothing.
    /// </summary>
    /// <param name="card"></param>
    public void UnsetCard(CardDefinition card)
    {
        if(card != null && _card == card)
        {
            clear();
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
