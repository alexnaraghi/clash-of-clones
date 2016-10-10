using UnityEngine;
using UnityEngine.UI;

public class Mana : MonoBehaviour 
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Text _countText;

    void Start()
    {
        // EARLY OUT! //
        if(_slider == null) return;

        _slider.minValue = 0f;
        _slider.maxValue = Consts.MaxMana;
    }

    void Update()
    {
        _slider.value = GameState.Instance.MyPlayer.Mana;
        _countText.text = Mathf.FloorToInt(_slider.value).ToString();
    }
}