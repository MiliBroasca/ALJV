using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatData : MonoBehaviour
{
    public int health = 100;
    public int attackPower = 20;
    public int abbilityPower = 40;
    public int abbilityCooldown = 3;
    public int healPower = 30;

    public Action modifyHealth;

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health < 0)
            health = 0;
        modifyHealth?.Invoke();
    }

    public void Heal(int healAmount)
    {
        health += healAmount;
        if (health > 100)
            health = 100;
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
