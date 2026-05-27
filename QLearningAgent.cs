csharp Assets/Scripts/AI/QLearningAgent.cs
using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QLearningAgent : MonoBehaviour
{
    public static QLearningAgent instance;

    public int width = 7;
    public int height = 7;

    [Header("Q-Learning params")]
    public float alpha = 0.1f;
    public float gamma = 0.99f;
    public float epsilon = 0.2f;
    public int episodes = 1000;
    public int maxStepsPerEpisode = 200;

    float[,] qTable; // [state, action]
    readonly Vector2Int[] actions = new Vector2Int[]
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    string savePath;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        // initialize
        qTable = new float[width * height, actions.Length];
        savePath = Path.Combine(Application.persistentDataPath, $"qtable_{SceneManager.GetActiveScene().name}.json");
        LoadQTable();
    }

    int PosToState(Vector2Int p) => p.x + p.y * width;

    int ArgMax(float[] values)
    {
        int best = 0;
        float bestVal = values[0];
        for (int i = 1; i < values.Length; i++)
            if (values[i] > bestVal) { best = i; bestVal = values[i]; }
        return best;
    }

    // Called by Agent at runtime to pick an action index
    public int GetAction(Vector2Int pos)
    {
        if (qTable == null) return UnityEngine.Random.Range(0, actions.Length);

        int s = PosToState(pos);
        if (UnityEngine.Random.value < epsilon)
            return UnityEngine.Random.Range(0, actions.Length);

        // greedy
        float[] row = new float[actions.Length];
        for (int a = 0; a < actions.Length; a++) row[a] = qTable[s, a];
        return ArgMax(row);
    }

    // Public entry to start training
    public void StartTraining()
    {
        StartCoroutine(Train());
    }

    IEnumerator Train()
    {
        if (GridManager.instance == null)
        {
            Debug.LogError("GridManager.instance is null. Cannot train.");
            yield break;
        }

        CellType[,] room = GridManager.instance.grid;
        if (room == null)
        {
            Debug.LogError("Grid not initialized. Ensure RoomManager has applied the room.");
            yield break;
        }

        System.Random rnd = new System.Random();

        for (int ep = 0; ep < episodes; ep++)
        {
            // start from random empty cell (or start cell if available)
            Vector2Int statePos = FindRandomStart(room, rnd);
            int state = PosToState(statePos);

            for (int step = 0; step < maxStepsPerEpisode; step++)
            {
                // epsilon-greedy
                int action = (UnityEngine.Random.value < epsilon) ? UnityEngine.Random.Range(0, actions.Length) : ArgMaxRow(state);

                Vector2Int nextPos = statePos + actions[action];

                // if outside grid, penalize and stay in same state
                float reward;
                int nextState;
                if (!GridManager.instance.IsInsideGrid(nextPos))
                {
                    reward = -1f;
                    nextPos = statePos;
                    nextState = state;
                }
                else
                {
                    nextState = PosToState(nextPos);
                    CellType cell = room[nextPos.x, nextPos.y];

                    reward = RewardFromCell(cell);

                    // treat door as neutral and do not switch scenes during training
                }

                // Q update
                float maxNext = MaxQ(nextState);
                qTable[state, action] = qTable[state, action] + alpha * (reward + gamma * maxNext - qTable[state, action]);

                statePos = nextPos;
                state = nextState;

                // optionally end episode on terminal cell (boss or heavy negative)
                if (IsTerminalCell(room[statePos.x, statePos.y]))
                    break;
            }

            // occasionally yield so Unity stays responsive
            if ((ep + 1) % 50 == 0) yield return null;
        }

        SaveQTable();
        Debug.Log($"Training finished. Q-table saved to: {savePath}");
    }

    Vector2Int FindRandomStart(CellType[,] room, System.Random rnd)
    {
        // try to find a Start cell first
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (room[x, y] == CellType.Start) return new Vector2Int(x, y);

        // otherwise pick a random inside cell
        while (true)
        {
            int x = rnd.Next(0, width);
            int y = rnd.Next(0, height);
            if (room[x, y] != CellType.Trap && room[x, y] != CellType.Boss && room[x, y] != CellType.Enemy)
                return new Vector2Int(x, y);
        }
    }

    float RewardFromCell(CellType cell)
    {
        switch (cell)
        {
            case CellType.Reward: return 10f;
            case CellType.Trap: return -5f;
            case CellType.Enemy: return -10f;
            case CellType.Boss: return -100f;
            default: return 0f;
        }
    }

    bool IsTerminalCell(CellType cell)
    {
        return cell == CellType.Boss;
    }

    int ArgMaxRow(int state)
    {
        int best = 0;
        float bestVal = qTable[state, 0];
        for (int a = 1; a < actions.Length; a++)
            if (qTable[state, a] > bestVal) { best = a; bestVal = qTable[state, a]; }
        return best;
    }

    float MaxQ(int state)
    {
        float max = qTable[state, 0];
        for (int a = 1; a < actions.Length; a++) if (qTable[state, a] > max) max = qTable[state, a];
        return max;
    }

    void SaveQTable()
    {
        try
        {
            int states = qTable.GetLength(0);
            int actionsCount = qTable.GetLength(1);
            float[] flat = new float[states * actionsCount];
            int idx = 0;
            for (int s = 0; s < states; s++)
                for (int a = 0; a < actionsCount; a++)
                    flat[idx++] = qTable[s, a];

            string json = JsonUtility.ToJson(new FloatArrayWrapper { data = flat });
            File.WriteAllText(savePath, json);
        }
        catch (Exception e) { Debug.LogError("Failed to save Q-table: " + e); }
    }

    void LoadQTable()
    {
        try
        {
            if (!File.Exists(savePath)) return;
            string json = File.ReadAllText(savePath);
            FloatArrayWrapper w = JsonUtility.FromJson<FloatArrayWrapper>(json);
            int states = width * height;
            int actionsCount = actions.Length;
            if (w.data.Length != states * actionsCount) return; // size mismatch -> skip

            qTable = new float[states, actionsCount];
            int idx = 0;
            for (int s = 0; s < states; s++)
                for (int a = 0; a < actionsCount; a++)
                    qTable[s, a] = w.data[idx++];
        }
        catch (Exception e) { Debug.LogWarning("Failed to load Q-table: " + e); }
    }

    [Serializable]
    class FloatArrayWrapper { public float[] data; }
}