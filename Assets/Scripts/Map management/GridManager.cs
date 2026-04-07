using UnityEngine;
using UnityEngine.SceneManagement;

public class GridManager : MonoBehaviour
{
    public int width = 7;
    public int height = 7;

    public CellType[,] grid;

    public static GridManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitGrid()
    {
        grid = new CellType[width, height];
    }

    public void SetCell(int x, int y, CellType type)
    {
        grid[x, y] = type;
    }

    public CellType GetCell(int x, int y)
    {
        return grid[x, y];
    }

    public Vector3 GridToWorld(Vector2Int pos)
    {
        return new Vector3(pos.x, pos.y, 0);
    }

    public bool IsInsideGrid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width &&
               pos.y >= 0 && pos.y < height;
    }
    public string GetNextScene(Vector2Int pos)
    {
        string scene = SceneManager.GetActiveScene().name;

        // ROOM A
        if (scene == "RoomA")
        {
            if (pos == new Vector2Int(0, 3)) return "RoomB";
            if (pos == new Vector2Int(3, 6)) return "RoomC";
            if (pos == new Vector2Int(6, 3)) return "RoomD";
        }

        // ROOM B
        else if (scene == "RoomB")
        {
            //if (pos == new Vector2Int(3, 0)) return "RoomA";
            if (pos == new Vector2Int(3, 6)) return "RoomE";
            if (pos == new Vector2Int(6, 3)) return "RoomF";
        }

        // ROOM C
        else if (scene == "RoomC")
        {
            //if (pos == new Vector2Int(3, 0)) return "RoomA";
            if (pos == new Vector2Int(3, 6)) return "RoomG";
        }

        // ROOM D
        else if (scene == "RoomD")
        {
            //if (pos == new Vector2Int(3, 0)) return "RoomA";
            if (pos == new Vector2Int(0, 3)) return "RoomG";
            if (pos == new Vector2Int(3, 6)) return "RoomH";
        }

        // ROOM E
        else if (scene == "RoomE")
        {
            //if (pos == new Vector2Int(3, 0)) return "RoomB";
            if (pos == new Vector2Int(3, 6)) return "RoomI";
        }

        // ROOM F
        else if (scene == "RoomF")
        {
            //if (pos == new Vector2Int(3, 0)) return "RoomB";
            if (pos == new Vector2Int(3, 6)) return "RoomJ";
        }

        // ROOM G
        else if (scene == "RoomG")
        {
            //if (pos == new Vector2Int(0, 3)) return "RoomC";
            if (pos == new Vector2Int(3, 6)) return "RoomJ";
            if (pos == new Vector2Int(6, 3)) return "RoomK";
        }

        // ROOM H
        else if (scene == "RoomH")
        {
            //if (pos == new Vector2Int(3, 6)) return "RoomD";
            if (pos == new Vector2Int(3, 6)) return "RoomK";
        }

        // ROOM I
        else if (scene == "RoomI")
        {
            //if (pos == new Vector2Int(0, 3)) return "RoomE";
            if (pos == new Vector2Int(3, 6)) return "RoomL";
        }

        // ROOM J
        else if (scene == "RoomJ")
        {
            //if (pos == new Vector2Int(0, 3)) return "RoomF";
            if (pos == new Vector2Int(3, 6)) return "RoomL";
        }

        // ROOM K
        else if (scene == "RoomK")
        {
            //if (pos == new Vector2Int(3, 6)) return "RoomG";
            if (pos == new Vector2Int(3, 6)) return "RoomL";
        }

        // ROOM L (boss)
        else if (scene == "RoomL")
        {
            //if (pos == new Vector2Int(0, 3)) return "RoomI";
            //if (pos == new Vector2Int(3, 6)) return "RoomJ";
            //if (pos == new Vector2Int(3, 0)) return "RoomK";
        }

        return null;
    }
}