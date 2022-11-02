using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityExtensions;

public class UIManager : MonoSingleton<UIManager>
{
    public string levelEndText = "YOU WIN!";
    [Header("Panels")]
    public Transform[] panels;
    public TextMeshProUGUI[] levelTexts;
    
    
    private void Start()
    {
        GameManager.OnGameStateChange += OnGameStateChange;
        totalMoney = PlayerPrefs.GetInt("Money");
    }  
    
    private void OnGameStateChange(GameManager.GameState gameState)
    {
        ChangeGameStateUI(gameState);
    }

    private void ChangeGameStateUI(GameManager.GameState gameState)
    {
        DisableAllUIs();
        switch (gameState)
        {
            case GameManager.GameState.Tap:
                panels[0].Show();
                UpdateLevelText(0);
                break;
            case GameManager.GameState.InGame:
                panels[1].Show();
                UpdateLevelText(1);
                break;
            case GameManager.GameState.Score:
                panels[2].Show();
                break;
            case GameManager.GameState.End:
                panels[3].Show();
                UpdateLevelText(2);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameState), gameState, null);
        }
    }

    private void DisableAllUIs()
    {
        foreach (var panel in panels)
        {
            panel.Hide();
        }
    }
    
    void UpdateLevelText(int i)
    {
        int currentLevel = PlayerPrefs.GetInt("Level") + 1;
        levelTexts[i].text = i==levelTexts.Length-1? levelTexts[i].text = levelEndText : levelTexts[i].text = "LVL " + currentLevel;
    }
    
    public void ShowScore(int boxAmount)
    {
        scoreImage.Show();
        RectTransform scoreRect = scoreImage.GetComponent<RectTransform>();
        
        scoreRect.DOScale(scoreGrowScale, scoreGrowTime).OnComplete(() => scoreRect.DOScale(1, scoreGrowTime));
        totalMoney += boxAmount * moneyMultiplier;
        scoreText.text = ""+ totalMoney;
    }
    
    public void SaveMoney()
    {
        PlayerPrefs.SetInt("Money", totalMoney);
    }
    
    private void OnDisable()
    {
        GameManager.OnGameStateChange -= OnGameStateChange;
    }

    

 }
    

