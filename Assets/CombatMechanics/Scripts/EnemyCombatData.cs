using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatData : MonoBehaviour
{
    public int health = 80;
    public int attackPower = 15;

    public int maxHealth = 100;
    public int minHealth = 60;
    public int bossHealth = 150;
    public int bossAttackPower = 15;

    public Action modifyHealth;

    private void OnEnable()
    {
        if (ScoreManager.instance.isBossFight)
        {
            health = bossHealth; // Boss has more health
            attackPower = 25; // Boss has stronger attacks
        } else
        {
            health = (int)(UnityEngine.Random.Range(minHealth, maxHealth));
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health < 0)
            health = 0;
        modifyHealth?.Invoke();
    }

    public void IsAlive()
    {
        if (health > 0)
            Debug.Log("Enemy is alive.");
        else
            Debug.Log("Enemy is dead.");
    }
}
