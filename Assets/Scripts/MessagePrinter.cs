using UnityEngine;
using UnityEngine.UI;

public class MessagePrinter : MonoBehaviour 
{
    [SerializeField] private Text _text;

    /// <summary>
    /// The number of seconds to display the message.
    /// </summary>
    [SerializeField] private float _displaySeconds;

    void Start()
    {
        PrintMessage("Clash of Clones");
    }

    public void PrintMessage(string message)    
    {
        // EARLY OUT! //
        if(_text == null) return;

        _text.enabled = true;
        _text.text = message;

        this.Invoke(hideMessage, _displaySeconds);
    }

    void hideMessage()
    {
        // EARLY OUT! //
        if(_text == null) return;

        _text.enabled = false;
    }
}