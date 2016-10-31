using UnityEngine;

/// <summary>
/// Snaps placement ghost to grid.  The grid puts 0,0 at the center of a cell.
/// </summary>
public class SnapToGrid : MonoBehaviour
{
    [SerializeField] private PlacementGhost _ghost;
    
    private LayerMask _groundMask;

    void Awake()
    {
        _groundMask = LayerMask.GetMask("Ground");
    }

    public void SetGhost(PlacementGhost ghost)
    {
        _ghost = ghost;
    }

    void Update()
    {
        if(_ghost != null && _ghost.Model != null && _ghost.Card != null)
        {
            var inputPosition = Input.mousePosition;

            Ray ray = Camera.main.ScreenPointToRay(inputPosition);
            RaycastHit hit;

            bool isValidCell = false;

            // If the position is on the ground
            if (Physics.Raycast(ray, out hit, int.MaxValue, _groundMask))
            {
                // If the territory is NOT controlled by the enemy ie friendly or neutral, except projectiles.
                bool isPlaceableTerritory = _ghost.Card.IsProjectile 
                    || !GameModel.Instance.EnemyPlayer.IsInTerritory(hit.point);
                    
                if(isPlaceableTerritory)
                {
                    var position = hit.point;

                    var gridPoint = TerritoryData.GetGridPosition(hit.point);
                    bool isInXBounds = gridPoint.X >= 0 && gridPoint.X < Consts.GridWidth;
                    bool isInYBounds = gridPoint.Y >= 0 && gridPoint.Y < Consts.GridHeight;

                    if(isInXBounds && isInYBounds)
                    {
                        isValidCell = true;

                        var snappedPosition = TerritoryData.GetCenter(gridPoint.X, gridPoint.Y);
                        snappedPosition = new Vector3(snappedPosition.x, position.y, snappedPosition.z);

                        _ghost.Model.SetActive(true);
                        _ghost.transform.position = snappedPosition;
                    }
                }

            }

            if(!isValidCell)
            {
                _ghost.Model.SetActive(false);
            }
            
        }
    }
}
