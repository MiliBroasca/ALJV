using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    CellType E = CellType.Empty;
    CellType T = CellType.Trap;
    CellType S = CellType.Start;
    CellType D = CellType.Door;
    CellType EN = CellType.Enemy;
    CellType R = CellType.Reward;
    CellType B = CellType.Boss;

    void Start()
    {
        GridManager.instance.InitGrid();

        string scene = SceneManager.GetActiveScene().name;

        CellType[,] room = GetRoomLayout(scene);

        ApplyRoom(room);

        FindObjectOfType<GridVisualizer>().GenerateVisuals();
    }

    CellType[,] GetRoomLayout(string scene)
    {
        switch (scene)
        {
            case "RoomA": return RoomA();
            case "RoomB": return RoomB();
            case "RoomC": return RoomC();
            case "RoomD": return RoomD();
            case "RoomE": return RoomE();
            case "RoomF": return RoomF();
            case "RoomG": return RoomG();
            case "RoomH": return RoomH();
            case "RoomI": return RoomI();
            case "RoomJ": return RoomJ();
            case "RoomK": return RoomK();
            case "RoomL": return RoomL();
        }

        return null;
    }

    void ApplyRoom(CellType[,] room)
    {
        for (int x = 0; x < 7; x++)
        {
            for (int y = 0; y < 7; y++)
            {
                GridManager.instance.SetCell(x, y, room[x, y]);
            }
        }
    }

    // 👇 AICI începi să lipești camerele

    CellType[,] RoomA()
    {
        return new CellType[7, 7]
        {
        {S,E,E,D,E,E,E},
        {E,EN,E,E,E,R,E},
        {E,E,T,E,E,E,E},
        {E,E,E,EN,E,E,D},
        {E,E,E,E,E,E,E},
        {E,R,E,E,T,E,E},
        {E,E,E,D,E,E,E}
        };
    }
    CellType[,] RoomB()
    {
        return new CellType[7, 7]
        {
        {S,E,E,E,E,E,E},
        {E,E,EN,E,R,E,E},
        {E,T,E,E,E,T,E},
        {E,E,E,E,EN,E,D},
        {E,E,E,E,E,E,E},
        {E,R,E,EN,E,E,E},
        {E,E,E,D,E,E,E}
        };
    }
    CellType[,] RoomC()
    {
        return new CellType[7, 7]
        {
        {S,E,E,E,E,E,E},
        {E,R,E,E,E,EN,E},
        {E,E,T,E,T,E,E},
        {E,E,E,E,E,E,D},
        {E,EN,E,E,E,E,E},
        {E,E,E,R,E,E,E},
        {E,E,E,E,E,E,E}
        };
    }
    CellType[,] RoomD()
    {
        return new CellType[7, 7]
        {
        {S,E,E,D,E,E,E},
        {E,EN,E,E,E,EN,E},
        {E,E,T,E,E,E,E},
        {E,E,E,R,E,E,D},
        {E,E,E,E,E,E,E},
        {E,T,E,E,E,R,E},
        {E,E,E,E,E,E,E}
        };
    }
    CellType[,] RoomE()
    {
        return new CellType[7, 7]
        {
        {S,E,E,E,E,E,E},
        {E,E,E,E,EN,E,E},
        {E,T,E,R,E,T,E},
        {E,E,E,E,E,E,D},
        {E,E,EN,E,E,E,E},
        {E,E,E,E,R,E,E},
        {E,E,E,E,E,E,E}
        };
    }
    CellType[,] RoomF()
    {
        return new CellType[7, 7]
        {
        {S,E,E,E,E,E,E},
        {E,EN,E,E,E,E,E},
        {E,E,T,E,EN,E,E},
        {E,E,E,E,E,E,D},
        {E,E,E,R,E,E,E},
        {E,E,E,E,T,E,E},
        {E,E,E,E,E,E,E}
        };
    }
    CellType[,] RoomG()
    {
        return new CellType[7, 7]
        {
        {S,E,E,E,E,E,E},
        {E,E,R,E,E,EN,E},
        {E,T,E,E,T,E,E},
        {E,E,E,EN,E,E,D},
        {E,E,E,E,E,E,E},
        {E,EN,E,E,E,R,E},
        {E,E,E,D,E,E,E}
        };
    }
    CellType[,] RoomH()
    {
        return new CellType[7, 7]
        {
        {S,E,E,E,E,E,E},
        {E,EN,E,E,E,E,E},
        {E,E,T,E,R,E,E},
        {E,E,E,E,EN,E,D},
        {E,E,E,E,E,E,E},
        {E,E,R,E,E,E,E},
        {E,E,E,E,E,E,E}
        };
    }
    CellType[,] RoomI()
    {
        return new CellType[7, 7]
        {
        {S,E,E,E,E,E,E},
        {E,E,E,E,EN,E,E},
        {E,T,E,E,E,T,E},
        {E,E,E,R,E,E,D},
        {E,E,EN,E,E,E,E},
        {E,E,E,E,R,E,E},
        {E,E,E,E,E,E,E}
        };
    }
    CellType[,] RoomJ()
    {
        return new CellType[7, 7]
        {
        {S,E,E,E,E,E,E},
        {E,EN,E,R,E,E,E},
        {E,E,T,E,E,E,E},
        {S,E,E,E,E,EN,D},
        {E,E,E,E,E,E,E},
        {E,R,E,E,T,E,E},
        {E,E,E,E,E,E,E}
        };
    }
    CellType[,] RoomK()
    {
        return new CellType[7, 7]
        {
        {S,E,E,E,E,E,E},
        {E,E,EN,E,E,E,E},
        {E,T,E,R,E,T,E},
        {E,E,E,E,E,E,D},
        {E,E,E,EN,E,E,E},
        {E,E,E,E,R,E,E},
        {E,E,E,E,E,E,E}
        };
    }
    CellType[,] RoomL()
    {
        return new CellType[7, 7]
        {
        {S,E,E,E,E,E,E},
        {E,E,E,E,E,E,E},
        {E,E,T,E,T,E,E},
        {E,E,E,B,E,E,E},
        {E,E,T,E,T,E,E},
        {E,E,E,E,E,E,E},
        {E,E,E,E,E,E,E}
        };
    }
}