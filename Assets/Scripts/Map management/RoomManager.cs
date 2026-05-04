using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    // Make these fields static so they can be accessed from static methods
    static CellType E = CellType.Empty;
    static CellType T = CellType.Trap;
    static CellType S = CellType.Start;
    static CellType D = CellType.Door;
    static CellType EN = CellType.Enemy;
    static CellType R = CellType.Reward;
    static CellType B = CellType.Boss;

    public Action generatedGrid;

    void Start()
    {
        if (GridManager.instance.grid == null)
        {
            GridManager.instance.InitGrid();

            string scene = SceneManager.GetActiveScene().name;

            CellType[,] room = GetRoomLayout(scene);

            ApplyRoom(room);
        }
        FindObjectOfType<GridVisualizer>().GenerateVisuals();
        generatedGrid?.Invoke();
    }

    CellType[,] GetRoomLayout(string scene)
    {
        switch (scene)
        {
            case "RoomA": return RoomA();
            case "RoomAGenetic": return RoomA();
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

    // camerele

    public static CellType[,] RoomA()
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
    public static CellType[,] RoomB()
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
    public static CellType[,] RoomC()
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
    public static CellType[,] RoomD()
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
    public static CellType[,] RoomE()
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
    public static CellType[,] RoomF()
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
    public static CellType[,] RoomG()
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
    public static CellType[,] RoomH()
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
    public static CellType[,] RoomI()
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
    public static CellType[,] RoomJ()
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
    public static CellType[,] RoomK()
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
    public static CellType[,] RoomL()
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