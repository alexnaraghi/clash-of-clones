using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using VRStandardAssets.Utils;

/// <summary>
/// The display of a card in our hand.
/// </summary>
public class CardUI : MonoBehaviour 
{
    [SerializeField] private Image _background;
    [SerializeField] private Image _image;
    [SerializeField] private Text _manaText;
    [SerializeField] private Image _manaIcon;
    [SerializeField] private Toggle _toggle;
    [SerializeField] private Color _disabledColor;
    [SerializeField] private VRInteractiveItem _interactiveItem;

    private Color _imageOriginalColor;
    private Color _manaTextOriginalColor;
    private Color _manaIconOriginalColor;
    
    private PlacementGhost _ghost;

    public CardData Definition;

    void Awake()
    {
        // We have to get the placement ghost to be able to toggle ourselves off when the card is placed.
        _ghost = GameObject.Find("PlacementGhost").GetComponent<PlacementGhost>();

        Assert.IsNotNull(_ghost);
        Assert.IsNotNull(_image);
        Assert.IsNotNull(_manaText);
        Assert.IsNotNull(_manaIcon);

        _imageOriginalColor = _image.color;
        _manaTextOriginalColor = _manaText.color;
        _manaIconOriginalColor = _manaIcon.color;

        if(_ghost != null)
        {
            _ghost.PlacedEvent.AddListener(onUnitPlaced);
        }

        if(_interactiveItem != null)
        {
            _interactiveItem.OnClick.AddListener(onClick);
        }
    }

    private void onClick()
    {
        if(_toggle.interactable)
        {
            setToggle(isEnabled: true);
        }
    }

    void Start()
    {
        GameModel.Instance.GameOverEvent.AddListener(onGameEnded);
    }

    private void onGameEnded()
    {
        setToggle(false);
        SetInteractable(false);
    }


    /// <summary>
    /// Change the card's artwork and attributes to match the given definition.
    /// </summary>
    public void Init(CardData definition)
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
            _ghost.SetCard(Definition, GameModel.Instance.RightPlayer);
        }
        else
        {
            _ghost.UnsetCard(Definition);
        }
    }

    public void SetInteractable(bool isInteractable)
    {
        if(_toggle != null)
        {
            _toggle.interactable = isInteractable;

            // We have lots of elements in a card, all of them have to be toggled.
            if(isInteractable)
            {
                _image.color =      _imageOriginalColor;
                _manaText.color =   _manaTextOriginalColor;
                _manaIcon.color =   _manaIconOriginalColor;
            }
            else
            {
                _image.color =      _disabledColor;
                _manaText.color =   _disabledColor;
                _manaIcon.color =   _disabledColor;
            }
        }
    }

    private void onUnitPlaced()
    {
        setToggle(false);
    }

    private void setToggle(bool isEnabled)
    {
        if(_toggle != null)
        {
            _toggle.isOn = isEnabled;
        }
    }

}