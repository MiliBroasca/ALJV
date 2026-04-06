using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        enemyCombatData.TakeDamage(playerCombatData.attackPower);
        Debug.Log("Player attacks! Enemy health: " + enemyCombatData.health);
        playerTurn = false;
    }

    public void PlayerAbility()
    {
        enemyCombatData.TakeDamage(playerCombatData.abilityPower);
        Debug.Log("Player uses ability! Enemy health: " + enemyCombatData.health);
        playerTurn = false;
    }

    public void PlayerHeal()
    {
        playerCombatData.Heal(playerCombatData.healPower);
        Debug.Log("Player heals! Player health: " + playerCombatData.health);
        playerTurn = false;
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
            //Debug.Log("Turn " + turn);
            //PlayerAttack();
            //if (enemyCombatData.health <= 0)
            //{
            //    Debug.Log("Enemy defeated!");
            //    break;
            //}
            
            //yield return new WaitForSeconds(2f); // Simulate time between turns
        }
    }

    private void Start()
    {
        StartCoroutine(StartCombat());
    }
}
