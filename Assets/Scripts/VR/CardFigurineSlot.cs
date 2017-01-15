using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Slot for a card that can be interacted with.
/// </summary>
public class CardFigurineSlot : MonoBehaviour 
{
    public int HandIndex;
    public UnityEvent CardChangedEvent;
    public GameObject LockedGameObject;

    // The prefab to generate that contains all interaction scripts, and initializes the visuals using the
    // card data that we pass in.
    [SerializeField] private CardFigurine _figurinePrefab;

    private CardFigurine _figurine;

    // The current figurine in the slot.
    public CardFigurine Figurine
    {
        get
        {
            return _figurine;
        }
    }

    private void Awake()
    {
        if(SL.Get<GameModel>().MyPlayer.CardState != null)
        {
            SL.Get<GameModel>().MyPlayer.CardState.CardChangedEvent.AddListener(onCardChanged);
        }
    }

    private void OnEnable()
    {
        SL.Get<GameModel>().MyPlayer.ManaChangedEvent.AddListener(onManaChanged);
    }

    private void OnDisable()
    {
        if (SL.Exists && SL.Get<GameModel>() != null && SL.Get<GameModel>().MyPlayer.ManaChangedEvent != null)
        {
            SL.Get<GameModel>().MyPlayer.ManaChangedEvent.RemoveListener(onManaChanged);
        }
    }

    private void OnDestroy()
    {
        // Might be a bad idea to try and access service locator on destroy.
        if (SL.Exists && SL.Get<GameModel>() != null && SL.Get<GameModel>().MyPlayer.CardState != null)
        {
            SL.Get<GameModel>().MyPlayer.CardState.CardChangedEvent.RemoveListener(onCardChanged);
        }
    }

    private CardFigurine createFigurineFromDefinition(CardData data)
    {
        CardFigurine figurine = null;
        if(_figurinePrefab != null && data != null)
        {
            // Parent to this slot.
            figurine = Utils.Instantiate(_figurinePrefab, transform);
            if(figurine != null)
            {
                figurine.transform.localPosition = Vector3.zero;
                figurine.transform.localRotation = Quaternion.identity;
                figurine.Init(data);
            }
        }
        return figurine;
    }

    private void destroyFigurine()
    {
        if (_figurine != null)
        {
            // Destroy the figurine.
            _figurine.FigurinePlacedEvent.RemoveListener(onFigurinePlaced);
            Destroy(_figurine.gameObject);
            _figurine = null;
        }
    }

    // Triggered when the card at this hand index was changed.
    private void onCardChanged(int index)
    {
        if(index == HandIndex)
        {
            var handState = SL.Get<GameModel>().MyPlayer.CardState.Hand;

            // EARLY OUT! //
            if(handState == null)
            {
                Debug.LogWarning("Hand is null");
                return;
            }

            // Destroy the old figurine.
            destroyFigurine();

            var data = handState[index];
            _figurine = createFigurineFromDefinition(data);
            if(_figurine != null)
            {
                _figurine.FigurinePlacedEvent.AddListener(onFigurinePlaced);
            }

            // Update if we have enough mana to use this figurine.
            onManaChanged();

            CardChangedEvent.Invoke();
        }
    }

    // Triggered when the figurine was placed.
    private void onFigurinePlaced(Vector3 destination)
    {
        if (_figurine != null)
        {
            var data = _figurine.Data;

            // Destroy the figurine.
            destroyFigurine();

            // This will cause a card change event to be invoked which will re-populate the slot
            // automatically.
            SL.Get<GameModel>().MyPlayer.PlayCard(data, destination);
        }
    }

    private void onManaChanged()
    {
        int mana = Mathf.FloorToInt(SL.Get<GameModel>().MyPlayer.Mana);

        if(_figurine != null)
        {
            bool canInteract = _figurine.Data.ManaCost > mana;
            LockedGameObject.SetActive(canInteract);
            _figurine.SetInteractable(canInteract);
        }
    }
}