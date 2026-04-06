using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatData : MonoBehaviour
{
    public int health = 80;
    public int attackPower = 15;

    public Action modifyHealth;

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
