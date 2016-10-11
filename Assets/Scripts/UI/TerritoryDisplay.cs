using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Shows the territory of the enemy player.
/// </summary>
public class TerritoryDisplay : MonoBehaviour 
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

    /// <summary>
    /// Is the territory display showing?
    /// </summary>
    public bool IsShowingEnemyTerritory
    {
        get { return _isShowingEnemyTerritory;}
        set { _isShowingEnemyTerritory = value; }
    }
    private bool _isShowingEnemyTerritory;

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
                rectTransform.gameObject.SetActive(false);
                _imagePool.Add(rectTransform);
            }
        }
    }

    void Update()
    {
        int numImagesUsed = 0;
        if(_isShowingEnemyTerritory)
        {
            var player = GameState.Instance.EnemyPlayer;
            foreach(var building in player.Buildings)
            {
                if(building.Entity != null && building.Entity.HP > 0)
                {
                    foreach(var territory in building.Territory.Territories)
                    {
                        if(numImagesUsed < _imagePool.Count)
                        {
                            var rectTransform = _imagePool[numImagesUsed];
                            var rect = toWorldRect(territory);
                            rectTransform.gameObject.SetActive(true);
                            rectTransform.offsetMin = new Vector2(rect.xMin, rect.yMin);
                            rectTransform.offsetMax = new Vector2(rect.xMax, rect.yMax);
                            
                            numImagesUsed++;
                        }
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
    
    /// <summary>
    /// Converts a territory definition into world space.
    /// </summary>
    /// <param name="territory">The territory to convert.</param>
    /// <returns>A rectangle in world space on the Y plane.</returns>
    private Rect toWorldRect(Territory territory)
    {
        Rect rect = new Rect(
            territory.StartX * Consts.GridCellWidth - Consts.GridCellWidth / 2f,
            territory.StartY * Consts.GridCellHeight - Consts.GridCellHeight / 2f,
            territory.Width * Consts.GridCellWidth,
            territory.Height * Consts.GridCellHeight
        );
        return rect;

    }   
}