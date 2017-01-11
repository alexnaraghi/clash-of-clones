using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Display's the player's mana.
/// </summary>
public class ManaUIVR : MonoBehaviour 
{
    [SerializeField] private TransformSlider _slider;
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
    }

    void Update()
    {
        _slider.Value = SL.Get<GameModel>().MyPlayer.Mana / Consts.MaxMana;
        int mana = Mathf.FloorToInt(SL.Get<GameModel>().MyPlayer.Mana);

        if(_lastUpdatedMana != mana)
        {
            _countText.text = mana.ToString();
            _lastUpdatedMana = mana;
        }
    }
}