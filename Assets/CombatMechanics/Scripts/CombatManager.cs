using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [SerializeField] private PlayerCombatData playerCombatData;
    [SerializeField] private EnemyCombatData enemyCombatData;

    public void PlayerAttack()
    {
        enemyCombatData.TakeDamage(playerCombatData.attackPower);
        Debug.Log("Player attacks! Enemy health: " + enemyCombatData.health);
    }

    public void PlayerAbbility()
    {
        enemyCombatData.TakeDamage(playerCombatData.abbilityPower);
        Debug.Log("Player uses ability! Enemy health: " + enemyCombatData.health);
    }

    public void PlayerHeal()
    {
        playerCombatData.Heal(playerCombatData.healPower);
        Debug.Log("Player heals! Player health: " + playerCombatData.health);
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
            Debug.Log("Turn " + turn);
            PlayerAttack();
            if (enemyCombatData.health <= 0)
            {
                Debug.Log("Enemy defeated!");
                break;
            }
            yield return new WaitForSeconds(2f); // Simulate time between turns
            EnemyAttack();
            if (playerCombatData.health <= 0)
            {
                Debug.Log("Player defeated!");
                break;
            }
            turn++;
            yield return new WaitForSeconds(2f); // Simulate time between turns
        }
    }

    private void Start()
    {
        StartCoroutine(StartCombat());
    }
}
