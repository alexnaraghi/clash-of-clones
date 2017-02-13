using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// The display of a card in our hand.
/// </summary>
public class DeckSelectionElement : MonoBehaviour 
{
    [SerializeField] private Image _background;
    [SerializeField] private Image _image;
    [SerializeField] private Text _manaText;
    [SerializeField] private Image _manaIcon;

    // private Color _imageOriginalColor;
    // private Color _manaTextOriginalColor;
    // private Color _manaIconOriginalColor;

    public CardData Definition;

    public UnityEvent SelectedEvent;

    void Awake()
    {
        Assert.IsNotNull(_image);
        Assert.IsNotNull(_manaText);
        Assert.IsNotNull(_manaIcon);

        // _imageOriginalColor = _image.color;
        // _manaTextOriginalColor = _manaText.color;
        // _manaIconOriginalColor = _manaIcon.color;

/*
        if(_interactiveItem != null)
        {
            _interactiveItem.OnClick.AddListener(onClick);
        }
*/
    }

    private void onClick()
    {
        SelectedEvent.Invoke();
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
        _image.sprite = SL.Get<ResourceManager>().Load<Sprite>(Consts.ImagePath + definition.CardImageName);
    }
}