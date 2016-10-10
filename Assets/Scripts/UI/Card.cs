using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour 
{
    [SerializeField] private Image _image;
    [SerializeField] private Text _manaText;
    [SerializeField] private Toggle _toggle;
    
    private PlacementGhost _ghost;

    public CardDefinition Definition;

    void Awake()
    {
        _ghost = GameObject.Find("PlacementGhost").GetComponent<PlacementGhost>();

        if(_ghost != null)
        {
            _ghost.PlacedEvent.AddListener(onUnitPlaced);
        }
    }

    public void SetCard(CardDefinition definition)
    {
        // EARLY OUT! //
        if(definition == null || _image == null || _manaText == null) return;

        Definition = definition;

        _manaText.text = definition.ManaCost.ToString();
        _image.sprite = Resources.Load<Sprite>(Consts.ImagePath + definition.CardImageName);
    }

    public void OnToggled(bool isOn)
    {
        // EARLY OUT! //
        if(_ghost == null) return;

        if(isOn)
        {
            _ghost.SetCard(Definition, GameState.Instance.RightPlayer);
        }
        else
        {
            _ghost.UnsetCard(Definition);
        }
    }

    private void onUnitPlaced()
    {
        if(_toggle != null)
        {
            _toggle.isOn = false;
        }
    }

}