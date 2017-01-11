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
    [SerializeField] private Reticle _reticle;
    
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

    void Update()
    {
        // If we aren't hitting a valid placement location, put the item in the air at the reticle.
        if(!_ghost.IsValidPlacementPosition)
        {
            if(_reticle != null && _reticle.ReticleTransform != null)
            {
                _reticle.Hide();
                _ghost.transform.position = _reticle.ReticleTransform.position;
            }
        }
    }

    public void onRaycastHit(RaycastHit hit)
    {
        if(_ghost != null && _ghost.Model != null && _ghost.Card != null)
        {
            bool isValidCell = false;

            bool isValidHitLocation = hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground");

            var hitPos = hit.point;
            
            if (isValidHitLocation)
            {
                // If the territory is NOT controlled by the enemy ie friendly or neutral, except projectiles.
                bool isPlaceableTerritory = _ghost.Card.IsProjectile
                    || !SL.Get<GameModel>().EnemyPlayer.IsInTerritory(hit.point);

                if(isPlaceableTerritory)
                {
                    var gridPoint = TerritoryData.GetGridPosition(hit.point);
                    bool isInXBounds = gridPoint.X >= 0 && gridPoint.X < Consts.GridWidth;
                    bool isInYBounds = gridPoint.Y >= 0 && gridPoint.Y < Consts.GridHeight;

                    if (isInXBounds && isInYBounds)
                    {
                        isValidCell = true;

                        var snappedPosition = TerritoryData.GetCenter(gridPoint.X, gridPoint.Y);
                        snappedPosition = new Vector3(snappedPosition.x, snappedPosition.y, snappedPosition.z);

                        _ghost.transform.position = snappedPosition;
                        _ghost.IsValidPlacementPosition = true;
                        
                        if(_reticle != null)
                        {
                            _reticle.Show();
                        }
                    }
                }
            }

            if(!isValidCell)
            {
                _ghost.IsValidPlacementPosition = false;
            }
        }
    }

    private static bool isHitValidLocation(GameObject target)
    {
        return target.layer == LayerMask.NameToLayer("Ground");
    }

}
