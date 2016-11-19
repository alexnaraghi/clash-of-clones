using UnityEngine;
using VRStandardAssets.Utils;
using UnityEngine.Assertions;

/// <summary>
/// Snaps placement ghost to grid.  The grid puts 0,0 at the center of a cell.
/// </summary>
public class SnapToGrid : MonoBehaviour
{
    [SerializeField] private PlacementGhost _ghost;
    [SerializeField] private VREyeRaycaster _vrRaycaster;
    
    private LayerMask _groundMask;

    void Awake()
    {
        Assert.IsNotNull(_vrRaycaster);

        _groundMask = LayerMask.GetMask("Ground");

        if(_vrRaycaster != null)
        {
            _vrRaycaster.OnRaycastHit.AddListener(onRaycastHit);
        }
    }

    void OnDestroy()
    {
        if(_vrRaycaster != null)
        {
            _vrRaycaster.OnRaycastHit.RemoveListener(onRaycastHit);
        }
    }

    public void SetGhost(PlacementGhost ghost)
    {
        _ghost = ghost;
    }

    public void onRaycastHit(RaycastHit hit)
    {
        if(_ghost != null && _ghost.Model != null && _ghost.Card != null)
        {
            bool isValidCell = false;

            // If the territory is NOT controlled by the enemy ie friendly or neutral, except projectiles.
            bool isPlaceableTerritory = _ghost.Card.IsProjectile
                || !GameModel.Instance.EnemyPlayer.IsInTerritory(hit.point);

            if (isPlaceableTerritory)
            {
                var position = hit.point;

                var gridPoint = TerritoryData.GetGridPosition(hit.point);
                bool isInXBounds = gridPoint.X >= 0 && gridPoint.X < Consts.GridWidth;
                bool isInYBounds = gridPoint.Y >= 0 && gridPoint.Y < Consts.GridHeight;

                if (isInXBounds && isInYBounds)
                {
                    isValidCell = true;

                    var snappedPosition = TerritoryData.GetCenter(gridPoint.X, gridPoint.Y);
                    snappedPosition = new Vector3(snappedPosition.x, position.y, snappedPosition.z);

                    _ghost.Model.SetActive(true);
                    _ghost.transform.position = snappedPosition;
                    _ghost.IsValidPlacementPosition = true;
                }
            }

            if(!isValidCell)
            {
                _ghost.Model.SetActive(false);
                _ghost.IsValidPlacementPosition = false;
            }
            
        }
    }
}
