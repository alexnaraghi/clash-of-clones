using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// Gui for a slider based health display.
/// </summary> 
[RequireComponent(typeof(Entity))]
public class HealthUI : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Image _fillImage;                              

    private Entity _entity;

    private void Awake()
    {

        _entity = GetComponent<Entity>();
        Assert.IsNotNull(_entity);

        _entity.InitializedEvent.AddListener(init);
    }

    public void init()
    {
        _entity.DamageTakenEvent.AddListener(onDamageTaken);
        _slider.maxValue = _entity.MaxHP;
        _fillImage.color = _entity.Owner.PlayerColor;

        SetHealthUI();
    }


    private void onDamageTaken()
    {
        if (_entity != null)
        {
            // Change the UI elements appropriately.
            SetHealthUI();
        }
    }


    private void SetHealthUI()
    {
        if (_entity != null)
        {
            // Set the slider's value appropriately.
            _slider.value = _entity.HP;
        }
    }
}