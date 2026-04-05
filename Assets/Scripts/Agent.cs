using UnityEngine;

public class Agent : MonoBehaviour
{
    public Vector2Int position;
    public int score = 0;

    public GridManager gridManager;

    public int maxSteps = 20;

    void Start()
    {
        position = new Vector2Int(0, 0); // start
        transform.position = new Vector3(position.x, position.y, -1);

        StartCoroutine(RunSimulation());
    }

    System.Collections.IEnumerator RunSimulation()
    {
        for (int i = 0; i < maxSteps; i++)
        {
            Vector2Int move = GetRandomMove();
            Move(move);

            yield return new WaitForSeconds(0.3f);
        }

        Debug.Log("Final Score: " + score);
    }

    Vector2Int GetRandomMove()
    {
        int rand = Random.Range(0, 4);

        switch (rand)
        {
            case 0: return Vector2Int.up;
            case 1: return Vector2Int.down;
            case 2: return Vector2Int.left;
            case 3: return Vector2Int.right;
        }

        return Vector2Int.zero;
    }

    void Move(Vector2Int direction)
    {
        Vector2Int newPos = position + direction;

        // verificare limite
        if (newPos.x < 0 || newPos.x >= gridManager.width ||
            newPos.y < 0 || newPos.y >= gridManager.height)
            return;

        position = newPos;
        transform.position = new Vector3(position.x, position.y, -1);

        EvaluateCell();
    }

    void EvaluateCell()
    {
        CellType cell = gridManager.grid[position.x, position.y];

        switch (cell)
        {
            case CellType.Reward:
                score += 10;
                break;
            case CellType.Trap:
                score -= 5;
                break;
            case CellType.Enemy:
                score -= 10;
                break;
            case CellType.Exit:
                score += 20;
                break;
        }

        Debug.Log("Score: " + score);
    }
}