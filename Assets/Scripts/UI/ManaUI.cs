using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Display's the player's mana.
/// </summary>
public class ManaUI : MonoBehaviour 
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Text _countText;

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
        _slider.value = GameModel.Instance.MyPlayer.Mana;
        _countText.text = Mathf.FloorToInt(_slider.value).ToString();
    }
}