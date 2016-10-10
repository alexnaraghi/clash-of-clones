using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerritoryGui : MonoBehaviour 
{
    [SerializeField] private Image _imagePrefab;

    private List<RectTransform> _imagePool;

    public bool IsShowingEnemyTerritory;
    public int NumberOfTerritoriesToPool = 6;

    void OnEnable()
    {
        _imagePool = new List<RectTransform>();

        for (int i = 0; i < NumberOfTerritoriesToPool; i++)
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
        if(IsShowingEnemyTerritory)
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