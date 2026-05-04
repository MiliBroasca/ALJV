using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public Vector2Int position;
    public bool isMovingEnabled = false;

    public Action arrivedAtBoss;

    void Start()
    {
        position = ScoreManager.instance.playerPosition;
        UpdatePosition();
    }

    void Update()
    {
        if (isMovingEnabled)
        {
            if (Input.GetKeyDown(KeyCode.W)) Move(Vector2Int.up);
            if (Input.GetKeyDown(KeyCode.S)) Move(Vector2Int.down);
            if (Input.GetKeyDown(KeyCode.A)) Move(Vector2Int.left);
            if (Input.GetKeyDown(KeyCode.D)) Move(Vector2Int.right);
        }
    }

    void Move(Vector2Int dir)
    {
        Vector2Int newPos = position + dir;

        if (!GridManager.instance.IsInsideGrid(newPos))
            return;

        position = newPos;

        CellType cell = GridManager.instance.GetCell(position.x, position.y);


        Debug.Log("Stepped on: " + cell);

        if (cell == CellType.Trap)
        {
            Debug.Log("Trap!");
            ScoreManager.instance.AddScore(-5);
        }

        if (cell == CellType.Reward)
        {
            Debug.Log("Reward!");
            ScoreManager.instance.AddScore(10);
            GridManager.instance.grid[position.x, position.y] = CellType.Empty;
            FindObjectOfType<GridVisualizer>().GenerateVisuals();
        }
        // integrate enemy combat
        if (cell == CellType.Enemy)
        {
            Debug.Log("Enemy!");
            ScoreManager.instance.playerPosition = position;
            ScoreManager.instance.currentScene = SceneManager.GetActiveScene().name;
            GridManager.instance.grid[position.x, position.y] = CellType.Empty; // clear enemy
            // SceneManager.LoadScene("CombatScene");
            // For now, just simulate combat by giving a random outcome
            int damage = Random.Range(5, 15);
            ScoreManager.instance.TakeDamage(damage);
            ScoreManager.instance.AddScore(20); // reward for defeating enemy
            FindObjectOfType<GridVisualizer>().GenerateVisuals(); // update visuals to show enemy removed
        }
        if (cell == CellType.Door)
            Debug.Log("Door!");
        if (cell == CellType.Boss)
        {
            Debug.Log("Boss!");
            ScoreManager.instance.playerPosition = position;
            ScoreManager.instance.currentScene = SceneManager.GetActiveScene().name;
            GridManager.instance.grid[position.x, position.y] = CellType.Empty; // clear boss
            ScoreManager.instance.isBossFight = true;
            // SceneManager.LoadScene("CombatScene");
            int damage = Random.Range(10, 30);
            ScoreManager.instance.TakeDamage(damage);
            ScoreManager.instance.AddScore(30);
        }

        UpdatePosition();
        CheckCurrentCell();
    }

    public void MoveFromGenome(Vector2Int dir)
    {
        Move(dir);
    }

    void UpdatePosition()
    {
        transform.position = GridManager.instance.GridToWorld(position);
    }
    void CheckCurrentCell()
    {
        Vector2Int pos = position;

        CellType cell = GridManager.instance.grid[pos.x, pos.y];

        if (cell == CellType.Door)
        {
            ScoreManager.instance.playerPosition = new Vector2Int(0, 0); // reset position for next scene
            position = new Vector2Int(0, 0); // reset position for next scene
            // UpdatePosition();

            string nextScene = GridManager.instance.GetNextScene(pos);

            if (nextScene != null)
            {
                GridManager.instance.grid = null; // clear grid
                SceneManager.LoadScene(nextScene);
                UpdatePosition();
            }
            else
            {
                Debug.Log("Door without connection!");
            }
        }

        if (cell == CellType.Boss)
        {
            arrivedAtBoss?.Invoke();
        }
    }
}