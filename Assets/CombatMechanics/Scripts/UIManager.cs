using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Slider playerHealthSlider;
    public Slider enemyHealthSlider;
    public TMP_Text playerCooldownText;

    public PlayerCombatData playerCombatData;
    public EnemyCombatData enemyCombatData;
    private void OnEnable()
    {
        playerCombatData.modifyHealth += UpdatePlayerHealthUI;
        enemyCombatData.modifyHealth += UpdateEnemyHealthUI;
        playerCombatData.modifyCooldown += UpdatePlayerCooldownUI;
    }

    private void UpdatePlayerCooldownUI()
    {
        if (playerCombatData.currentCooldown == 0)
        {
            playerCooldownText.text = "Ability Ready!";
        }
        else
        {
            playerCooldownText.text = $"Cooldown: {playerCombatData.currentCooldown}";
        }
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
        playerHealthSlider.maxValue = playerCombatData.maxHealth;
        playerHealthSlider.value = playerCombatData.health;
        enemyHealthSlider.maxValue = enemyCombatData.health;
        enemyHealthSlider.value = enemyCombatData.health;
        UpdatePlayerCooldownUI();
    }
}
