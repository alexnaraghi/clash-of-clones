using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates the clock.
/// </summary>
public class GameClockUI : MonoBehaviour 
{
    [SerializeField] private Text _timer;
    private int _lastUpdatedTime;

    void Update()
    {
        // EARLY OUT! //
        if(this.DisabledFromMissingObject(_timer)) return;

        int secondsLeft = Mathf.RoundToInt(SL.Get<GameModel>().SecondsLeft);

        if(_lastUpdatedTime != secondsLeft)
        {
            int minutes = secondsLeft / 60;
            secondsLeft -= minutes * 60;
            int seconds = secondsLeft;
        
            _timer.text = string.Format("{0}:{1:D2}", minutes, seconds);
            _lastUpdatedTime = secondsLeft;
        }
    }
}