using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndGameUI : MonoBehaviour
{
    public static EndGameUI instance;

    [Header("UI")]
    public GameObject panel;
    public TMP_Text messageText;

    private void Awake()
    {
        instance = this;

        if (panel != null)
            panel.SetActive(false);
    }

    public void ShowVictoryScreen(int finalScore)
    {
        Time.timeScale = 0f;

        if (messageText != null)
            messageText.text = "You defeated the boss!\nFinal score: " + finalScore;

        if (panel != null)
            panel.SetActive(true);
    }

    public void RestartWithAI()
    {
        Time.timeScale = 1f;

        if (ScoreManager.instance != null)
            ScoreManager.instance.ResetScore();

        if (GridManager.instance != null)
            GridManager.instance.grid = null;

        Agent agent = FindObjectOfType<Agent>();
        if (agent != null)
            agent.ResetAgent();

        SceneManager.LoadScene("RoomA");
    }

    public void RestartWithPlayer()
    {
        Time.timeScale = 1f;

        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.score = 0;
            ScoreManager.instance.health = 100;
            ScoreManager.instance.playerPosition = Vector2Int.zero;
        }

        if (GridManager.instance != null)
            GridManager.instance.grid = null;

        SceneManager.LoadScene("RoomA");
    }
}