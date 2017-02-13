using UnityEngine;

/// <summary>
/// Utility to visualize placement grids for debugging.
/// </summary>
public class DrawDebugGrid : MonoBehaviour 
{
    private const float _gridWidth = 100f;
    private const float _gridHeight = 100f;

    // The height of the grid on the vertical axis.
    private const float _gridVerticalHeight = 0.2f;
    private const float _edgeThickness = 0.2f;

    public bool IsDrawingEnemyGrid;
    public bool IsDrawingFriendlyGrid;
    public bool IsDrawingMapGrid;

    void Update()
    {
        foreach(var child in transform)
        {
            Destroy(((Transform)child).gameObject);
        }

        if(IsDrawingEnemyGrid)
        {
            CreateTerritoryGridForPlayer(SL.Get<GameModel>().EnemyPlayer);
        }
        
        if(IsDrawingFriendlyGrid)
        {
            CreateTerritoryGridForPlayer(SL.Get<GameModel>().MyPlayer);
        }

        if(IsDrawingMapGrid)
        {
            // Our snapping actually puts 0,0 in the middle of a grid square, so we need to make sure to
            // offset by half a cell on the start position!
            int startX = (int)((-_gridWidth / 2f) / Consts.GridCellWidth);
            int startZ = (int)((-_gridHeight / 2f) / Consts.GridCellHeight);

            int numWidths = (int)(_gridWidth / Consts.GridCellWidth);
            int numHeights = (int)(_gridHeight / Consts.GridCellHeight);

            CreateGrid(startX, startZ, numWidths, numHeights);
        }
    }

    public void CreateTerritoryGridForPlayer(PlayerModel player)
    {
        foreach(var building in player.Buildings)
        {
            if(building.Entity.HP > 0)
            {
                foreach(var t in building.Territory.Territories)
                {
                    CreateGrid(t.StartX, t.StartY, t.Width, t.Height);
                }
            }
        }
    }

    public void CreateGrid(int gridX, int gridY, int gridWidth, int gridHeight)
    {
        CreateGrid(gridX, gridY, gridWidth, gridHeight, _gridVerticalHeight, _edgeThickness);
    }

    public void CreateGrid(int gridX, int gridY, int gridWidth, int gridHeight, 
        float gridVerticalHeight, float edgeThickness)
    {
        // Take the grid coordinates and convert them to game units.

        // Our snapping actually puts 0,0 in the middle of a grid square, so we need to make sure to
        // offset by half a cell on the start position!
        float startX = gridX * Consts.GridCellWidth - (Consts.GridCellWidth / 2f);
        float startZ = gridY * Consts.GridCellHeight - (Consts.GridCellHeight / 2f);

        float width = gridWidth * Consts.GridCellWidth;
        float height = gridHeight * Consts.GridCellHeight;

        for (int i = 0; i <= gridWidth; i++)
        {
            float x = startX + i * Consts.GridCellWidth;
            float z = startZ + height / 2f;

            createEdge(new Vector3(x, gridVerticalHeight, z), edgeThickness, height);
        }

        for (int i = 0; i <= gridHeight; i++)
        {
            float x = startX + width / 2f;
            float z = startZ + i * Consts.GridCellHeight;
            createEdge(new Vector3(x, gridVerticalHeight, z), width, edgeThickness);
        }
    }

    private GameObject createEdge(Vector3 position, float width, float height)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.SetParent(transform, true);
        go.transform.localScale = new Vector3(width, _edgeThickness, height);
        go.transform.position = position;

        return go;
    }
}