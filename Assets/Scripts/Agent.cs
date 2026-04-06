using UnityEngine;

public class Agent : MonoBehaviour
{
    public Vector2Int position;

    void Start()
    {
        UpdatePosition();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MakeMove();
        }
    }

    void MakeMove()
    {
        Vector2Int move = GetRandomMove();

        Vector2Int newPos = position + move;

        if (!GridManager.instance.IsInsideGrid(newPos))
            return;

        position = newPos;

        CellType cell = GridManager.instance.GetCell(position.x, position.y);

        Debug.Log("AI stepped on: " + cell);

        UpdatePosition();
    }

    Vector2Int GetRandomMove()
    {
        Vector2Int[] moves = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        return moves[Random.Range(0, moves.Length)];
    }

    void UpdatePosition()
    {
        transform.position = GridManager.instance.GridToWorld(position);
    }
}