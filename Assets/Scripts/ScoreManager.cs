using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public int score = 0;
    public TextMeshProUGUI scoreText;

    public int health = 100;
    public int maxHealth = 100;
    public Vector2Int playerPosition = new Vector2Int(0, 0);
    public string currentScene;
    public bool isBossFight = false;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        UpdateUI();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        scoreText = FindObjectOfType<TextMeshProUGUI>();

        if (scoreText == null)
            Debug.LogError("ScoreText not found!");

        UpdateUI();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    public void ResetScore()
    {
        score = 0;
        health = 100;
        playerPosition = Vector2Int.zero;
        UpdateUI();
    }
}