using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YinYangCase.Game.Managers
{
    public class PlayerDataManager : MonoBehaviour
    {
        private readonly string moneyKey = "Money";
        private readonly string healthKey = "Health";
        private readonly string staminaKey = "Stamina";
        
        
        [SerializeField] private PlayerDataSO playerData;
        
        //Actions
        public static Action<float> SpendMoneyAction;
        public static Action<float> EarnMoneyAction;
        public static Action<float> HealingAction;

        private void SubscribeEvent()
        {
            SpendMoneyAction += SpendMoney;
            EarnMoneyAction += EarnMoney;
            HealingAction += Healing;
        }

        private void UnSubscribeEvent()
        {
            SpendMoneyAction -= SpendMoney;
            EarnMoneyAction -= EarnMoney;
            HealingAction -= Healing;
        }
        
        
        private void OnEnable()
        {
            GetLastDataValues();
            SubscribeEvent();
        }

        private void OnDisable()
        {
            UnSubscribeEvent();
        }

        private void GetLastDataValues()
        {
            playerData.Money = GetPlayerPrefsFloat(moneyKey, 1000);
            playerData.Health = GetPlayerPrefsFloat(healthKey, 1000);
            playerData.Stamina = GetPlayerPrefsFloat(staminaKey, 1000);
    
            UIManager.PlayerStatUIRefresh.Invoke();
        }
        
        private void SpendMoney(float money)
        {
            playerData.Money -= money;
            PlayerPrefs.SetFloat(moneyKey, playerData.Money);
        }
        
        private void EarnMoney(float money)
        {
            playerData.Money += money;
            PlayerPrefs.SetFloat(moneyKey, playerData.Money);
        }

        private void Healing(float heal)
        {
            playerData.Health += heal;
            PlayerPrefs.SetFloat(healthKey, playerData.Health);
        }
        
        
        private float GetPlayerPrefsFloat(string key, float defaultValue)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }
    }
}
