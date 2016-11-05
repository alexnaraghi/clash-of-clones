using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A visual clock that destroys itself once it reaches the total specified seconds.
/// </summary>
public class SpawnClock : MonoBehaviour 
{
    [SerializeField] private Slider _slider;
    private float _secondsPassed;
    private float _totalSeconds;

    public void Init(Vector3 position, float seconds)
    {
        transform.position = position;
        _totalSeconds = seconds;
    }

    void Update()
    {
        _secondsPassed += Time.deltaTime;
        _slider.value = _secondsPassed / _totalSeconds;

        if(_secondsPassed >= _totalSeconds)
        {
            Destroy(gameObject);
        }
    }
}