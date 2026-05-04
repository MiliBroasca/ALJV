using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour
{
    private static CombatManager _instance;
    public static CombatManager Instance { get { return  _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    [SerializeField] private PlayerCombatData playerCombatData;
    [SerializeField] private EnemyCombatData enemyCombatData;

    public bool playerTurn = true;

    public Action newTurn;

    public void PlayerAttack()
    {
        if (playerTurn)
        {
            enemyCombatData.TakeDamage(playerCombatData.attackPower);
            Debug.Log("Player attacks! Enemy health: " + enemyCombatData.health);
            playerTurn = false;
        }
    }

    public void PlayerAbility()
    {
        if (playerCombatData.UseAbility() && playerTurn)
        {
            enemyCombatData.TakeDamage(playerCombatData.abilityPower);
            Debug.Log("Player uses ability! Enemy health: " + enemyCombatData.health);
            playerTurn = false;
        }
    }

    public void PlayerHeal()
    {
        if (playerTurn)
        {
            playerCombatData.Heal(playerCombatData.healPower);
            Debug.Log("Player heals! Player health: " + playerCombatData.health);
            playerTurn = false;
        }
    }

    public void EnemyAttack()
    {
        playerCombatData.TakeDamage(enemyCombatData.attackPower);
        Debug.Log("Enemy attacks! Player health: " + playerCombatData.health);
    }

    public IEnumerator StartCombat()
    {
        int turn = 1;
        while (playerCombatData.health > 0 && enemyCombatData.health > 0)
        {
            if (!playerTurn)
            {
                yield return new WaitForSeconds(2f); // Simulate time between turns
                EnemyAttack();
                if (playerCombatData.health <= 0)
                {
                    Debug.Log("Player defeated!");
                    break;
                }
                turn++;
                newTurn?.Invoke();
                playerTurn = true;
            }
            else
            {
                yield return null; // Wait for player input (e.g., button press) to perform an action
            }
        }

        if (playerCombatData.health <= 0)
        {
            Debug.Log("Player has been defeated. Game Over.");
            
        }
        else if (enemyCombatData.health <= 0)
        {
            Debug.Log("Enemy has been defeated. You win!");
            ScoreManager.instance.health = playerCombatData.health; // Update health in ScoreManager
            ScoreManager.instance.AddScore(20); // Reward player with score for winning combat
            yield return new WaitForSeconds(3f); // Wait before returning to main game
            SceneManager.LoadScene(ScoreManager.instance.currentScene); // Return to previous scene
        }
    }

    private void Start()
    {
        StartCoroutine(StartCombat());
    }
}
