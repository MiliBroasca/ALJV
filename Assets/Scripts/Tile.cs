using UnityEngine;

public class Tile : MonoBehaviour
{
    public SpriteRenderer sr;

    public void SetType(CellType type)
    {
        if (sr == null)
            sr = GetComponent<SpriteRenderer>();

        switch (type)
        {
            case CellType.Empty:
                sr.color = Color.white;
                break;
            case CellType.Reward:
                sr.color = Color.green;
                break;
            case CellType.Trap:
                sr.color = Color.red;
                break;
            case CellType.Enemy:
                sr.color = Color.black;
                break;
            case CellType.Exit:
                sr.color = Color.yellow;
                break;
            case CellType.Start:
                sr.color = Color.blue;
                break;
        }
    }
}