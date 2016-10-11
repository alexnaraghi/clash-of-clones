using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simply prints a big message to the center of the screen for some seconds.
/// </summary>
public class MessageUI : MonoBehaviour 
{
    [SerializeField] private Text _text;

    /// <summary>
    /// The number of seconds to display the message.
    /// </summary>
    [SerializeField] private float _displaySeconds;

    public void PrintMessage(string message)
    {
        // EARLY OUT! //
        if (_text == null)
        {
            Debug.LogWarning("Can't print a message without text.");
            return;
        }

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

    void OnDestroy()
    {
        this.CancelInvoke();
    }
}