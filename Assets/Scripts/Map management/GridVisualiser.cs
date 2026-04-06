using UnityEngine;
using UnityEngine.SceneManagement;

public class GridVisualizer : MonoBehaviour
{
    public GameObject floorPrefab;
    public GameObject trapPrefab;
    public GameObject enemyPrefab;
    public GameObject rewardPrefab;
    public GameObject doorPrefab;
    public GameObject startPrefab;
    public GameObject bossPrefab;

    public void GenerateVisuals()
    {
        CellType[,] grid = GridManager.instance.grid;
        string currentScene = SceneManager.GetActiveScene().name;

        for (int x = 0; x < 7; x++)
        {
            for (int y = 0; y < 7; y++)
            {
                CellType type = grid[x, y];

                GameObject prefab = GetPrefab(type);

                if (prefab != null)
                {
                    Vector3 pos = GridManager.instance.GridToWorld(new Vector2Int(x, y));
                    GameObject obj = Instantiate(prefab, pos, Quaternion.identity, transform);

                    if (type == CellType.Door)
                    {
                        SetupDoor(obj, currentScene, x, y);
                    }
                }
            }
        }
    }
    void SetupDoor(GameObject doorObj, string scene, int x, int y)
    {
        Door door = doorObj.GetComponent<Door>();

        if (door == null)
            door = doorObj.AddComponent<Door>();

        // determină direcția ușii
        string entry = "";

        if (x == 0) entry = "right";
        else if (x == 6) entry = "left";
        else if (y == 0) entry = "up";
        else if (y == 6) entry = "down";

        door.entryPoint = entry;

        // 🔥 aici legăm camerele (pe baza schemei tale)

        if (scene == "RoomA")
        {
            if (x == 6) door.nextScene = "RoomB";
            else if (y == 6) door.nextScene = "RoomC";
            else if (y == 0) door.nextScene = "RoomD";
        }

        else if (scene == "RoomB")
        {
            if (x == 0) door.nextScene = "RoomA";
            else if (x == 6) door.nextScene = "RoomF";
            else if (y == 0) door.nextScene = "RoomE";
        }

        else if (scene == "RoomC")
        {
            if (y == 0) door.nextScene = "RoomA";
            else if (x == 6) door.nextScene = "RoomG";
        }

        else if (scene == "RoomD")
        {
            if (y == 6) door.nextScene = "RoomA";
            else if (x == 6) door.nextScene = "RoomG";
            else if (y == 0) door.nextScene = "RoomH";
        }

        else if (scene == "RoomE")
        {
            if (y == 6) door.nextScene = "RoomB";
            else if (x == 6) door.nextScene = "RoomJ";
        }

        else if (scene == "RoomF")
        {
            if (x == 0) door.nextScene = "RoomB";
            else if (x == 6) door.nextScene = "RoomI";
        }

        else if (scene == "RoomG")
        {
            if (x == 0) door.nextScene = "RoomC"; // sau D (simplificare)
            else if (x == 6) door.nextScene = "RoomJ";
            else if (y == 0) door.nextScene = "RoomK";
        }

        else if (scene == "RoomH")
        {
            if (y == 6) door.nextScene = "RoomD";
            else if (x == 6) door.nextScene = "RoomK";
        }

        else if (scene == "RoomI")
        {
            if (x == 0) door.nextScene = "RoomF";
            else if (x == 6) door.nextScene = "RoomL";
        }

        else if (scene == "RoomJ")
        {
            if (x == 0) door.nextScene = "RoomE";
            else if (x == 6) door.nextScene = "RoomL";
        }

        else if (scene == "RoomK")
        {
            if (y == 6) door.nextScene = "RoomG";
            else if (x == 6) door.nextScene = "RoomL";
        }

        else if (scene == "RoomL")
        {
            if (x == 0) door.nextScene = "RoomI";
            else if (y == 6) door.nextScene = "RoomJ";
            else if (y == 0) door.nextScene = "RoomK";
        }
        if (door.nextScene == null || door.nextScene == "")
        {
            Debug.LogWarning("Door not configured correctly at: " + x + "," + y + " in " + scene);
        }
    }

    GameObject GetPrefab(CellType type)
    {
        switch (type)
        {
            case CellType.Empty: return floorPrefab;
            case CellType.Trap: return trapPrefab;
            case CellType.Enemy: return enemyPrefab;
            case CellType.Reward: return rewardPrefab;
            case CellType.Door: return doorPrefab;
            case CellType.Start: return startPrefab;
            case CellType.Boss: return bossPrefab;
            default: return floorPrefab;
        }
    }
}