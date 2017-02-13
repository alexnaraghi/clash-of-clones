using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class FigurineManaTextUI : MonoBehaviour 
{
    [SerializeField] private CardFigurineSlot _slot;
    
    private Text _text;

    private void Awake()
    {
        _text = GetComponent<Text>();
        
        // EARLY OUT! //
        if(_text == null)
        {
            Debug.LogWarning("FigurineManaTextUI must be placed on a text element.");
            return;
        }

        // EARLY OUT! //
        if(_slot == null)
        {
            Debug.LogWarning("FigurineManaTextUI must be linked to a CardFigurineSlot element.");
            return;
        }

        _text.text = "";

        if(_slot != null)    
        {
            _slot.CardChangedEvent.AddListener(onCardChanged);
        }
    }

	private void OnDestroy() 
    {
	    if(_slot != null)    
        {
            _slot.CardChangedEvent.RemoveListener(onCardChanged);
        }
	}

    private void onCardChanged()
    {
        var figurine = _slot.Figurine;
        if(figurine != null)
        {
            _text.text = figurine.Data.ManaCost.ToString();
        }
    }
}
