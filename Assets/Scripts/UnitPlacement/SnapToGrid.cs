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
        if(_ghost != null && _ghost.Model != null)
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

                    bool isInXBounds = Mathf.Abs(position.x / Consts.GridCellWidth) < (Consts.GridWidth / 2);
                    bool isInYBounds = Mathf.Abs(position.z / Consts.GridCellHeight) < (Consts.GridHeight / 2);

                    if(isInXBounds && isInYBounds)
                    {
                        isValidCell = true;

                        var snappedX = Mathf.RoundToInt(position.x / Consts.GridCellWidth) * Consts.GridCellWidth;
                        var snappedZ = Mathf.RoundToInt(position.z / Consts.GridCellHeight) * Consts.GridCellHeight;
                        var snappedPosition = new Vector3(snappedX, position.y, snappedZ);

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
