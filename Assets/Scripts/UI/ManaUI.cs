using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Display's the player's mana.
/// </summary>
public class ManaUI : MonoBehaviour 
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Text _countText;

    private int _lastUpdatedMana;

    void Start()
    {
        // EARLY OUT! //
        if(_slider == null || _countText == null)
        {
            Debug.LogWarning("Mana requires slider and count text.");
            return;
        }

        _slider.minValue = 0f;
        _slider.maxValue = Consts.MaxMana;
    }

    void Update()
    {
        _slider.value = SL.Get<GameModel>().MyPlayer.Mana;
        int mana = Mathf.FloorToInt(SL.Get<GameModel>().MyPlayer.Mana);

        if(_lastUpdatedMana != mana)
        {
            _countText.text = mana.ToString();
            _lastUpdatedMana = mana;
        }
    }
}