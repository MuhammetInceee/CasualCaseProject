using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace YinYangCase.Game.Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;
        
        [SerializeField] private TextMeshProUGUI moneyText;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI staminaText;
        
        //Actions
        public static Action PlayerStatUIRefresh;

        private void OnEnable()
        {
            PlayerStatUIRefresh += UIRefresh;
        }

        private void OnDisable()
        {
            PlayerStatUIRefresh -= UIRefresh;
        }

        private void UIRefresh()
        {
            SetPlayerStatUIText(moneyText, "Money", playerData.Money);
            SetPlayerStatUIText(healthText, "Health", playerData.Health);
            SetPlayerStatUIText(staminaText, "Stamina", playerData.Stamina);
        }

        private void SetPlayerStatUIText(TextMeshProUGUI textMeshProUGUI, string statName, float statValue)
        {
            textMeshProUGUI.text = $"{statName}: {statValue}";
        }

    }
}
