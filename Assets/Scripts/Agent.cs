using UnityEngine;
using UnityEngine.SceneManagement;

public class Agent : MonoBehaviour
{
    private static Agent instance;

    public enum AIProfile
    {
        ShortestPath,
        CollectRewards,
        Aggressive,
        Safe
    }

    [Header("AI Profile")]
    public AIProfile aiProfile = AIProfile.ShortestPath;

    [Header("Movement")]
    public Vector2Int position = Vector2Int.zero;
    public bool trainAutomatically = true;
    public float moveDelay = 0.2f;

    [Header("Episode")]
    public bool showEndScreen = false;
    public int maxStepsPerEpisode = 300;

    private float timer = 0f;
    private int steps = 0;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        Application.runInBackground = true;
    }

    void Start()
    {
        position = new Vector2Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.y)
        );

        UpdatePosition();
    }

    void Update()
    {
        if (!trainAutomatically)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                MakeMove();

            return;
        }

        timer += Time.deltaTime;

        if (timer >= moveDelay)
        {
            timer = 0f;
            MakeMove();
        }
    }

    void MakeMove()
    {
        if (QLearningAgent.instance == null)
        {
            Debug.LogError("QLearningAgent missing from scene!");
            return;
        }

        string currentRoom = SceneManager.GetActiveScene().name;
        string state = GetState(currentRoom, position);

        int action = QLearningAgent.instance.ChooseAction(state);
        Vector2Int move = ActionToMove(action);

        Vector2Int newPos = position + move;

        if (!GridManager.instance.IsInsideGrid(newPos))
        {
            float invalidReward = -2f;
            QLearningAgent.instance.Learn(state, action, invalidReward, state);
            return;
        }

        position = newPos;
        steps++;

        CellType cell = GridManager.instance.GetCell(position.x, position.y);

        float reward = GetTileReward(cell);
        bool done = false;

        Debug.Log("AI stepped on: " + cell + " | Reward: " + reward);

        if (cell == CellType.Reward)
        {
            if (ScoreManager.instance != null)
                ScoreManager.instance.AddScore(10);

            GridManager.instance.grid[position.x, position.y] = CellType.Empty;
            RefreshGrid();
        }

        else if (cell == CellType.Trap)
        {
            if (ScoreManager.instance != null)
                ScoreManager.instance.AddScore(-5);
        }

        else if (cell == CellType.Enemy)
        {
            bool wonCombat = ResolveCombatAutomatically();

            if (wonCombat)
            {
                if (ScoreManager.instance != null)
                    ScoreManager.instance.AddScore(20);

                GridManager.instance.grid[position.x, position.y] = CellType.Empty;
                RefreshGrid();
            }
            else
            {
                reward -= 20f;

                if (ScoreManager.instance != null)
                    ScoreManager.instance.AddScore(-20);

                done = true;
            }
        }

        else if (cell == CellType.Boss)
        {
            if (ScoreManager.instance != null)
                ScoreManager.instance.AddScore(100);

            int finalScore = ScoreManager.instance != null ? ScoreManager.instance.score : 0;

            Debug.Log("AI defeated the boss with score " + finalScore);

            done = true;

            if (QLearningAgent.instance != null)
                QLearningAgent.instance.SaveQTable();

            if (showEndScreen && EndGameUI.instance != null)
            {
                steps = 0;
                EndGameUI.instance.ShowVictoryScreen(finalScore);
                enabled = false;
                return;
            }
        }

        string nextScene = GridManager.instance.GetNextScene(position);

        if (cell == CellType.Door && nextScene != null)
        {
            Debug.Log("AI Door! Going to: " + nextScene);

            Vector2Int nextPosition = Vector2Int.zero;
            string nextState = GetState(nextScene, nextPosition);

            QLearningAgent.instance.Learn(state, action, reward, nextState);

            position = nextPosition;

            if (ScoreManager.instance != null)
                ScoreManager.instance.playerPosition = nextPosition;

            GridManager.instance.grid = null;
            SceneManager.LoadScene(nextScene);

            return;
        }

        if (steps >= maxStepsPerEpisode)
        {
            reward -= 30f;
            done = true;
        }

        string finalRoom = SceneManager.GetActiveScene().name;
        string learnedNextState = GetState(finalRoom, position);

        QLearningAgent.instance.Learn(state, action, reward, learnedNextState);

        UpdatePosition();

        if (done)
            ResetEpisode();
    }

    float GetTileReward(CellType cell)
    {
        switch (aiProfile)
        {
            case AIProfile.ShortestPath:
                switch (cell)
                {
                    case CellType.Reward: return 1f;
                    case CellType.Trap: return -5f;
                    case CellType.Enemy: return -2f;
                    case CellType.Door: return 20f;
                    case CellType.Boss: return 100f;
                    default: return -1f;
                }

            case AIProfile.CollectRewards:
                switch (cell)
                {
                    case CellType.Reward: return 25f;
                    case CellType.Trap: return -5f;
                    case CellType.Enemy: return 5f;
                    case CellType.Door: return 3f;
                    case CellType.Boss: return 50f;
                    default: return -0.2f;
                }

            case AIProfile.Aggressive:
                switch (cell)
                {
                    case CellType.Reward: return 10f;
                    case CellType.Trap: return -3f;
                    case CellType.Enemy: return 25f;
                    case CellType.Door: return 10f;
                    case CellType.Boss: return 100f;
                    default: return -0.3f;
                }

            case AIProfile.Safe:
                switch (cell)
                {
                    case CellType.Reward: return 8f;
                    case CellType.Trap: return -20f;
                    case CellType.Enemy: return -10f;
                    case CellType.Door: return 15f;
                    case CellType.Boss: return 100f;
                    default: return -0.3f;
                }
        }

        return -0.1f;
    }

    Vector2Int ActionToMove(int action)
    {
        switch (action)
        {
            case 0: return Vector2Int.up;
            case 1: return Vector2Int.down;
            case 2: return Vector2Int.left;
            case 3: return Vector2Int.right;
            default: return Vector2Int.zero;
        }
    }

    string GetState(string room, Vector2Int pos)
    {
        return room + "_" + pos.x + "_" + pos.y;
    }

    bool ResolveCombatAutomatically()
    {
        return Random.value > 0.35f;
    }

    void RefreshGrid()
    {
        GridVisualizer visualizer = FindObjectOfType<GridVisualizer>();

        if (visualizer != null)
            visualizer.GenerateVisuals();
    }

    void ResetEpisode()
    {
        if (QLearningAgent.instance != null)
            QLearningAgent.instance.SaveQTable();

        Debug.Log("Episode ended. Resetting...");

        steps = 0;
        position = Vector2Int.zero;

        if (ScoreManager.instance != null)
            ScoreManager.instance.ResetScore();

        if (GridManager.instance != null)
            GridManager.instance.grid = null;

        SceneManager.LoadScene("RoomA");
    }

    public void ResetAgent()
    {
        steps = 0;
        position = Vector2Int.zero;
        enabled = true;
        UpdatePosition();
    }

    void UpdatePosition()
    {
        transform.position = GridManager.instance.GridToWorld(position);
    }
}