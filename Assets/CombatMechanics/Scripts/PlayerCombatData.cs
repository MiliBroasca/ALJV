using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatData : MonoBehaviour
{
    private CombatManager combatManager;
    [Header("Player Combat Stats")]
    public int maxHealth = 100;
    public int health = 100;
    public int attackPower = 20;
    public int abilityPower = 40;
    public int abilityCooldown = 3;
    public int currentCooldown = 0;
    public int healPower = 30;

    public Action modifyHealth;
    public Action modifyCooldown;

    private void Start()
    {
        combatManager = CombatManager.Instance;
        if (combatManager != null)
            combatManager.newTurn += HandleNewTurn;
    }

    private void OnDisable()
    {
        if (combatManager != null)
            combatManager.newTurn -= HandleNewTurn;
    }

    public bool UseAbility()
    {
        if (currentCooldown == 0)
        {
            currentCooldown = abilityCooldown;
            modifyCooldown?.Invoke();
            return true;
        }
        else
        {
            Debug.Log("Ability is on cooldown for " + currentCooldown + " more turns.");
            return false;
        }
    }

    private void HandleNewTurn()
    {
        currentCooldown = Mathf.Max(currentCooldown - 1, 0);
        modifyCooldown?.Invoke();
    }

    public void TakeDamage(int damage)
    {
        health = Mathf.Max(health - damage, 0);
        modifyHealth?.Invoke();
    }

    public void Heal(int healAmount)
    {
        health = Mathf.Min(health + healAmount, maxHealth);
        modifyHealth?.Invoke();
    }

    public void IsAlive()
    {
        if (health > 0)
            Debug.Log("Player is alive.");
        else
            Debug.Log("Player is dead.");
    }
}
