using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Shows the territory of the enemy player.
/// </summary>
public class TerritoryUI : MonoBehaviour 
{
    /// <summary>
    /// The prefab of the image to display a territory with.
    /// </summary>
    [SerializeField] private Image _imagePrefab;

    /// <summary>
    /// Pool of images to use for the territory.
    /// TODO: Add general pooling functionality to game.
    /// </summary>
    private List<RectTransform> _imagePool;
    private int _numberOfTerritoriesToPool = 6;

    void OnEnable()
    {
        // EARLY OUT! //
        if(_imagePrefab == null)
        {
            Debug.LogWarning("TerritoryDisplay requires an image prefab.");
            return;
        }

        _imagePool = new List<RectTransform>();

        for (int i = 0; i < _numberOfTerritoriesToPool; i++)
        {
            var image = Instantiate(_imagePrefab);
            var rectTransform = image.GetComponent<RectTransform>();
            if(rectTransform != null)
            {
                rectTransform.SetParent(transform, false);
                rectTransform.localPosition = Vector3.zero;
                rectTransform.localRotation = Quaternion.identity;
                _imagePool.Add(rectTransform);
            }
        }
    }

    void Update()
    {
        int numImagesUsed = 0;
        var player = GameModel.Instance.EnemyPlayer;
        foreach(var building in player.Buildings)
        {
            if(building.Entity != null && building.Entity.HP > 0)
            {
                foreach(var territory in building.Territory.Territories)
                {
                    if(numImagesUsed < _imagePool.Count)
                    {
                        var rectTransform = _imagePool[numImagesUsed];
                        var rect = TerritoryData.ToWorldRect(territory);
                        rectTransform.gameObject.SetActive(true);
                        rectTransform.offsetMin = new Vector2(rect.xMin, rect.yMin);
                        rectTransform.offsetMax = new Vector2(rect.xMax, rect.yMax);
                        
                        numImagesUsed++;
                    }
                }
            }
        }

        for(int i = numImagesUsed; i < _imagePool.Count; i++)
        {
            if(_imagePool[i].gameObject.activeSelf)
            {
                _imagePool[i].gameObject.SetActive(false);
            }
        }
    }   
}