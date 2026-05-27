using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class QLearningAgent : MonoBehaviour
{
    public static QLearningAgent instance;

    [Header("Q-Learning Settings")]
    public float learningRate = 0.1f;
    public float discountFactor = 0.95f;
    public float epsilon = 1.0f;
    public float minEpsilon = 0.05f;
    public float epsilonDecay = 0.995f;

    private Dictionary<string, float[]> qTable = new Dictionary<string, float[]>();

    private string SavePath
    {
        get
        {
            string profile = "Default";

            Agent agent = FindObjectOfType<Agent>();
            if (agent != null)
                profile = agent.aiProfile.ToString();

            return Application.persistentDataPath + "/qtable_" + profile + ".json";
        }
    }

    private void Awake()
    {
        Application.runInBackground = true;
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        LoadQTable();
    }

    public int ChooseAction(string state)
    {
        EnsureStateExists(state);

        if (UnityEngine.Random.value < epsilon)
            return UnityEngine.Random.Range(0, 4);

        float[] values = qTable[state];

        int bestAction = 0;
        float bestValue = values[0];

        for (int i = 1; i < values.Length; i++)
        {
            if (values[i] > bestValue)
            {
                bestValue = values[i];
                bestAction = i;
            }
        }

        return bestAction;
    }

    public void Learn(string state, int action, float reward, string nextState)
    {
        EnsureStateExists(state);
        EnsureStateExists(nextState);

        float currentQ = qTable[state][action];
        float maxNextQ = GetMaxQ(nextState);

        qTable[state][action] =
            currentQ + learningRate * (reward + discountFactor * maxNextQ - currentQ);

        epsilon = Mathf.Max(minEpsilon, epsilon * epsilonDecay);
    }

    private float GetMaxQ(string state)
    {
        EnsureStateExists(state);

        float[] values = qTable[state];
        float max = values[0];

        for (int i = 1; i < values.Length; i++)
        {
            if (values[i] > max)
                max = values[i];
        }

        return max;
    }

    private void EnsureStateExists(string state)
    {
        if (!qTable.ContainsKey(state))
        {
            qTable[state] = new float[4];
            // 0 = up, 1 = down, 2 = left, 3 = right
        }
    }

    public void SaveQTable()
    {
        QTableData data = new QTableData();

        foreach (var pair in qTable)
        {
            QStateData stateData = new QStateData();
            stateData.state = pair.Key;
            stateData.qValues = pair.Value;

            data.states.Add(stateData);
        }

        data.epsilon = epsilon;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);

        Debug.Log("Q-table saved at: " + SavePath);
    }

    public void LoadQTable()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("No saved Q-table found. Starting fresh.");
            return;
        }

        string json = File.ReadAllText(SavePath);
        QTableData data = JsonUtility.FromJson<QTableData>(json);

        qTable.Clear();

        foreach (QStateData stateData in data.states)
        {
            qTable[stateData.state] = stateData.qValues;
        }

        epsilon = data.epsilon;

        Debug.Log("Q-table loaded. States: " + qTable.Count);
    }

    public void DeleteSavedQTable()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            qTable.Clear();
            epsilon = 1.0f;

            Debug.Log("Saved Q-table deleted.");
        }
    }

    private void OnApplicationQuit()
    {
        SaveQTable();
    }

    public int GetKnownStatesCount()
    {
        return qTable.Count;
    }
}

[Serializable]
public class QTableData
{
    public List<QStateData> states = new List<QStateData>();
    public float epsilon;
}

[Serializable]
public class QStateData
{
    public string state;
    public float[] qValues;
}