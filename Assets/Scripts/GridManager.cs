using UnityEngine;

public enum CellType
{
    Empty,
    Reward,
    Trap,
    Enemy,
    Exit,
    Start
}

public class GridManager : MonoBehaviour
{
    public int width = 5;
    public int height = 5;

    public GameObject tilePrefab;

    public CellType[,] grid;

    void Start()
    {
        GenerateGrid();
        DrawGrid();
    }

    void GenerateGrid()
    {
        grid = new CellType[width, height];

        // Inițial toate sunt Empty
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = CellType.Empty;
            }
        }

        // Setări manuale (simple)
        grid[0, 0] = CellType.Start;
        grid[4, 4] = CellType.Exit;

        grid[1, 0] = CellType.Trap;
        grid[2, 2] = CellType.Enemy;
        grid[3, 1] = CellType.Reward;
        grid[1, 3] = CellType.Reward;
    }

    void DrawGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject tile = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity);

                Tile tileScript = tile.GetComponent<Tile>();
                tileScript.SetType(grid[x, y]);
            }
        }
    }
}