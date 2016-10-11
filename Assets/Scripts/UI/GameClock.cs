using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates the clock.
/// </summary>
public class GameClock : MonoBehaviour 
{
    [SerializeField] private Text _timer;

    void Update()
    {
        // EARLY OUT! //
        if(_timer == null)
        {
            Debug.LogWarning("Clock requires text.");
            return;
        }

        int secondsLeft = Mathf.RoundToInt(GameState.Instance.SecondsLeft);

        int minutes = secondsLeft / 60;
        secondsLeft -= minutes * 60;
        int seconds = secondsLeft;

        _timer.text = string.Format("{0}:{1:D2}", minutes, seconds);
    }
}