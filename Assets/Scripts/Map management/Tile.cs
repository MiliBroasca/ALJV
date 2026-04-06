using UnityEngine;

public class Tile : MonoBehaviour
{
    public SpriteRenderer sr;

    public Sprite emptySprite;
    public Sprite rewardSprite;
    public Sprite trapSprite;
    public Sprite enemySprite;
    public Sprite exitSprite;
    public Sprite startSprite;

    public void SetType(CellType type)
    {
        if (sr == null)
            sr = GetComponent<SpriteRenderer>();

        switch (type)
        {
            case CellType.Empty:
                sr.sprite = emptySprite;
                break;
            case CellType.Reward:
                sr.sprite = rewardSprite;
                break;
            case CellType.Trap:
                sr.sprite = trapSprite;
                break;
            case CellType.Enemy:
                sr.sprite = enemySprite;
                break;
            case CellType.Exit:
                sr.sprite = exitSprite;
                break;
            case CellType.Start:
                sr.sprite = startSprite;
                break;
        }
    }
}