using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    static CellType E = CellType.Empty;
    static CellType T = CellType.Trap;
    static CellType S = CellType.Start;
    static CellType D = CellType.Door;
    static CellType EN = CellType.Enemy;
    static CellType R = CellType.Reward;
    static CellType B = CellType.Boss;

    public static MapVariant SelectedMapVariant
    {
        get
        {
            if (MapConfig.Instance != null)
                return MapConfig.Instance.selectedMapVariant;

            return MapVariant.Original;
        }
    }

    public Action generatedGrid;

    void Start()
    {
        if (GridManager.instance.grid == null)
        {
            GridManager.instance.InitGrid();

            string scene = SceneManager.GetActiveScene().name;
            CellType[,] room = GetRoomLayout(scene);

            if (room == null)
            {
                Debug.LogError("No room layout found for scene: " + scene);
                return;
            }

            ApplyRoom(room);
        }

        FindObjectOfType<GridVisualizer>().GenerateVisuals();
        generatedGrid?.Invoke();
    }

    CellType[,] GetRoomLayout(string scene)
    {
        switch (scene)
        {
            case "RoomA":
            case "RoomAGenetic":
                return RoomA();

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

    public static CellType[,] RoomA()
    {
        return SelectedMapVariant == MapVariant.Original ? RoomA_Original() : RoomA_Variant2();
    }

    public static CellType[,] RoomB()
    {
        return SelectedMapVariant == MapVariant.Original ? RoomB_Original() : RoomB_Variant2();
    }

    public static CellType[,] RoomC()
    {
        return SelectedMapVariant == MapVariant.Original ? RoomC_Original() : RoomC_Variant2();
    }

    public static CellType[,] RoomD()
    {
        return SelectedMapVariant == MapVariant.Original ? RoomD_Original() : RoomD_Variant2();
    }

    public static CellType[,] RoomE()
    {
        return SelectedMapVariant == MapVariant.Original ? RoomE_Original() : RoomE_Variant2();
    }

    public static CellType[,] RoomF()
    {
        return SelectedMapVariant == MapVariant.Original ? RoomF_Original() : RoomF_Variant2();
    }

    public static CellType[,] RoomG()
    {
        return SelectedMapVariant == MapVariant.Original ? RoomG_Original() : RoomG_Variant2();
    }

    public static CellType[,] RoomH()
    {
        return SelectedMapVariant == MapVariant.Original ? RoomH_Original() : RoomH_Variant2();
    }

    public static CellType[,] RoomI()
    {
        return SelectedMapVariant == MapVariant.Original ? RoomI_Original() : RoomI_Variant2();
    }

    public static CellType[,] RoomJ()
    {
        return SelectedMapVariant == MapVariant.Original ? RoomJ_Original() : RoomJ_Variant2();
    }

    public static CellType[,] RoomK()
    {
        return SelectedMapVariant == MapVariant.Original ? RoomK_Original() : RoomK_Variant2();
    }

    public static CellType[,] RoomL()
    {
        return SelectedMapVariant == MapVariant.Original ? RoomL_Original() : RoomL_Variant2();
    }

    // =========================
    // ORIGINAL MAPS
    // =========================

    static CellType[,] RoomA_Original()
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

    static CellType[,] RoomB_Original()
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

    static CellType[,] RoomC_Original()
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

    static CellType[,] RoomD_Original()
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

    static CellType[,] RoomE_Original()
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

    static CellType[,] RoomF_Original()
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

    static CellType[,] RoomG_Original()
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

    static CellType[,] RoomH_Original()
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

    static CellType[,] RoomI_Original()
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

    static CellType[,] RoomJ_Original()
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

    static CellType[,] RoomK_Original()
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

    static CellType[,] RoomL_Original()
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

    // =========================
    // VARIANT 2 MAPS
    // =========================

    static CellType[,] RoomA_Variant2()
    {
        return new CellType[7, 7]
        {
            {S,E,E,D,E,E,E},
            {E,E,R,E,EN,E,E},
            {E,T,E,E,E,T,E},
            {E,E,E,R,E,E,D},
            {E,EN,E,E,E,E,E},
            {E,E,E,T,E,R,E},
            {E,E,E,D,E,E,E}
        };
    }

    static CellType[,] RoomB_Variant2()
    {
        return new CellType[7, 7]
        {
            {S,E,E,E,E,E,E},
            {E,R,E,E,EN,E,E},
            {E,E,T,E,E,E,E},
            {E,E,EN,E,E,R,D},
            {E,T,E,E,E,T,E},
            {E,E,E,R,EN,E,E},
            {E,E,E,D,E,E,E}
        };
    }

    static CellType[,] RoomC_Variant2()
    {
        return new CellType[7, 7]
        {
            {S,E,E,E,E,E,E},
            {E,E,EN,E,R,E,E},
            {E,T,E,E,E,T,E},
            {E,E,E,R,E,E,D},
            {E,E,T,E,EN,E,E},
            {E,R,E,E,E,E,E},
            {E,E,E,E,E,E,E}
        };
    }

    static CellType[,] RoomD_Variant2()
    {
        return new CellType[7, 7]
        {
            {S,E,E,D,E,E,E},
            {E,R,E,E,E,EN,E},
            {E,E,T,E,T,E,E},
            {E,EN,E,E,R,E,D},
            {E,E,E,E,E,E,E},
            {E,T,E,EN,E,R,E},
            {E,E,E,E,E,E,E}
        };
    }

    static CellType[,] RoomE_Variant2()
    {
        return new CellType[7, 7]
        {
            {S,E,E,E,E,E,E},
            {E,EN,E,R,E,E,E},
            {E,E,T,E,E,T,E},
            {E,E,E,EN,E,E,D},
            {E,R,E,E,E,E,E},
            {E,E,T,E,R,E,E},
            {E,E,E,E,E,E,E}
        };
    }

    static CellType[,] RoomF_Variant2()
    {
        return new CellType[7, 7]
        {
            {S,E,E,E,E,E,E},
            {E,E,R,E,E,EN,E},
            {E,T,E,E,E,E,E},
            {E,E,E,EN,E,R,D},
            {E,E,T,E,E,E,E},
            {E,E,E,E,T,E,E},
            {E,E,E,E,E,E,E}
        };
    }

    static CellType[,] RoomG_Variant2()
    {
        return new CellType[7, 7]
        {
            {S,E,E,E,E,E,E},
            {E,EN,E,E,R,E,E},
            {E,E,T,E,E,T,E},
            {E,R,E,E,EN,E,D},
            {E,E,E,T,E,E,E},
            {E,E,E,E,R,EN,E},
            {E,E,E,D,E,E,E}
        };
    }

    static CellType[,] RoomH_Variant2()
    {
        return new CellType[7, 7]
        {
            {S,E,E,E,E,E,E},
            {E,R,E,E,EN,E,E},
            {E,E,T,E,E,T,E},
            {E,E,E,R,E,EN,D},
            {E,E,E,E,E,E,E},
            {E,T,E,E,R,E,E},
            {E,E,E,E,E,E,E}
        };
    }

    static CellType[,] RoomI_Variant2()
    {
        return new CellType[7, 7]
        {
            {S,E,E,E,E,E,E},
            {E,EN,E,E,R,E,E},
            {E,E,T,E,E,T,E},
            {E,E,R,E,E,E,D},
            {E,T,E,EN,E,E,E},
            {E,E,E,E,E,R,E},
            {E,E,E,E,E,E,E}
        };
    }

    static CellType[,] RoomJ_Variant2()
    {
        return new CellType[7, 7]
        {
            {S,E,E,E,E,E,E},
            {E,R,E,E,EN,E,E},
            {E,T,E,E,E,T,E},
            {S,E,E,R,E,EN,D},
            {E,E,E,E,E,E,E},
            {E,E,EN,E,R,E,E},
            {E,E,E,E,E,E,E}
        };
    }

    static CellType[,] RoomK_Variant2()
    {
        return new CellType[7, 7]
        {
            {S,E,E,E,E,E,E},
            {E,EN,E,E,R,E,E},
            {E,E,T,E,E,T,E},
            {E,R,E,E,E,E,D},
            {E,E,E,T,EN,E,E},
            {E,E,R,E,E,E,E},
            {E,E,E,E,E,E,E}
        };
    }

    static CellType[,] RoomL_Variant2()
    {
        return new CellType[7, 7]
        {
            {S,E,E,E,E,E,E},
            {E,E,R,E,E,E,E},
            {E,T,E,E,E,T,E},
            {E,E,EN,B,EN,E,E},
            {E,T,E,E,E,T,E},
            {E,E,E,R,E,E,E},
            {E,E,E,E,E,E,E}
        };
    }
}