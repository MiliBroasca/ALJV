using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Slider playerHealthSlider;
    public Slider enemyHealthSlider;

    public PlayerCombatData playerCombatData;
    public EnemyCombatData enemyCombatData;
    private void OnEnable()
    {
        playerCombatData.modifyHealth += UpdatePlayerHealthUI;
        enemyCombatData.modifyHealth += UpdateEnemyHealthUI;
    }

    private void UpdateEnemyHealthUI()
    {
        enemyHealthSlider.value = enemyCombatData.health;
    }

    private void UpdatePlayerHealthUI()
    {
        playerHealthSlider.value = playerCombatData.health;
    }

    private void OnDisable()
    {
        playerCombatData.modifyHealth -= UpdatePlayerHealthUI;
        enemyCombatData.modifyHealth -= UpdateEnemyHealthUI;
    }

    private void Start()
    {
        playerHealthSlider.maxValue = playerCombatData.health;
        playerHealthSlider.value = playerCombatData.health;
        enemyHealthSlider.maxValue = enemyCombatData.health;
        enemyHealthSlider.value = enemyCombatData.health;
    }
}
