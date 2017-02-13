using System;
using UnityEngine;
using UnityEngine.Events;

public class GridSnapVR : MonoBehaviour 
{
    private bool _isOverBoard;

    // These will update to the last known valid grid position.  If the transform leaves the board,
    // these values will stay (but _isOverBoard will be false).
    private GridPoint _knownGridPoint;
    private Vector3 _knownWorldPosition;

    public UnityEvent GridSquareChangedEvent = new UnityEvent();

    public bool IsOverBoard
    {
        get
        {
            return _isOverBoard;
        }
    }

    public GridPoint GridPoint
    {
        get
        {
            return _knownGridPoint;
        }
    }

    public Vector3 WorldPosition
    {
        get
        {
            return _knownWorldPosition;
        }
    }

    private void Update()
    {
        var trackedPos = transform.position;
        var gridPoint = TerritoryData.GetGridPosition(trackedPos);

        bool isAboveBoard = transform.position.y > Consts.PlacementBufferY;

        // Note this is referring to x and y in 2D space, corresponds to X and Z in 3D space.
        bool isInXBounds = gridPoint.X >= 0 && gridPoint.X < Consts.GridWidth;
        bool isInYBounds = gridPoint.Y >= 0 && gridPoint.Y < Consts.GridHeight;

        var prevIsOverBoard = _isOverBoard;
        var prevGridPoint = _knownGridPoint;

        _isOverBoard = isAboveBoard && isInXBounds && isInYBounds;

        if(_isOverBoard)
        {
            _knownGridPoint = gridPoint;
            _knownWorldPosition = trackedPos;
        }

        if(_isOverBoard != prevIsOverBoard 
            || _knownGridPoint.X != prevGridPoint.X 
            || _knownGridPoint.Y != prevGridPoint.Y)
        {
            GridSquareChangedEvent.Invoke();
        }
    }
}
