using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// Gui for a slider based health display.
/// </summary> 
public class HealthUI : MonoBehaviour
{
    [SerializeField] private GameObject _sliderPrefab;                           
    [SerializeField] private float _sliderScale = 5f;                           
    [SerializeField] private Entity _entity;
    
    private Slider _slider;
    private Image _fillImage;                           

    private void Awake()
    {
        Assert.IsNotNull(_entity);
        Assert.IsNotNull(_sliderPrefab);

        if(_sliderPrefab != null)
        {
            var go = Instantiate(_sliderPrefab);
            if(go != null)
            {
                go.transform.SetParent(transform, worldPositionStays: false);
                go.transform.localScale = Vector3.one * _sliderScale;
                _slider = go.GetComponentInChildren<Slider>();
                _fillImage = go.GetComponentInChildren<Image>();

                Assert.IsNotNull(_slider);
                Assert.IsNotNull(_fillImage);
            }
        }

        if(_entity != null)
        {
            _entity.InitializedEvent.AddListener(init);
        }
    }

    public void init()
    {
        _entity.DamageTakenEvent.AddListener(onDamageTaken);

        if(_slider != null)
        {
            _slider.maxValue = _entity.MaxHP;
        }
        
        if(_fillImage != null)
        {
            _fillImage.color = _entity.Owner.PlayerColor;
        }

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
        if (_entity != null && _slider != null)
        {
            // Set the slider's value appropriately.
            _slider.value = _entity.HP;
        }
    }
}