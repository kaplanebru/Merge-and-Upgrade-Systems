using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;


public class UpgradeSystem : MonoBehaviour
{
    private void Awake()
    {
        GetParameters();
    }
    
    
    [Serializable] 
    public class UpgradeStats
    {
        public string statName;
        public float startValue;
        public float upgradeValue;
        
        [Header("Price Settings")]
        [ReadOnly]public int price;
        public int goldMultiplier;
        public float goldBaseNumber;
        private float increment;

        [ReadOnly]public int levelIndex;
        [ReadOnly]public float currentValue;
        [ReadOnly]public int uiValue;


        public void UpdateStats()
        {
            levelIndex = PlayerPrefs.GetInt(statName);
            increment = upgradeValue * levelIndex;
            currentValue = startValue + increment;
            uiValue = Mathf.FloorToInt(currentValue);
            CalculateGold();
        }

        public void IncreaseStats()
        {
            levelIndex += 1;
            PlayerPrefs.SetInt(statName, levelIndex);
            increment = upgradeValue * levelIndex;
            currentValue = startValue + increment;
            uiValue = Mathf.FloorToInt(currentValue); //roundtoInt
            CalculateGold();

        }

        public void CalculateGold()
        {
            price = Mathf.FloorToInt(goldMultiplier * Mathf.Pow(goldBaseNumber, levelIndex));
        }
    }
    

    public UpgradeStats[] stats;
    
    public void GetParameters()
    {
        for (int i = 0; i < stats.Length; i++)
        {
            stats[i].UpdateStats();
        }
    }

    public void Stamina()
    {
        SpendMoney(stats[0].price);
        stats[0].IncreaseStats();
  

        GameManager.Instance.getButtons.progressTexts[0].text = "" + stats[0].uiValue; 
        GameManager.Instance.getButtons.price[0].text = "" + stats[0].price;
  
        CheckButtons();
        
    }

    public void Strength()
    {
        
        SpendMoney(stats[1].price);
        stats[1].IncreaseStats();
        CheckButtons();
        
    }

    public void Impact()
    {
        SpendMoney(stats[2].price);
        stats[2].IncreaseStats();
        CheckButtons();
    }

    public void Income()
    {
        SpendMoney(stats[3].price); 
        stats[3].IncreaseStats();
        CheckButtons();
        
    }

    public void SpendMoney(int price)
    {
        UIManager.Instance.StartCoroutine("MoneyFX");  
        GameManager.Instance.moneyUI.DecreaseMoney(price); 
    }
    

    public void CheckButtons()
    {
        for (int i = 0; i < stats.Length; i++)
        {
            if (stats[i].price <= UIManager.Instance.totalMoney)
                GameManager.Instance.getButtons.EnableButton(i);
            
            else
                GameManager.Instance.getButtons.DisableOneButton(i);
            
        }
    }

    
    
 


}
