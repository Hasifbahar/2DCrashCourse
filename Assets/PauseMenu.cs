using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 
using DG.Tweening; // For your bouncy menu animation

public class PauseMenu : MonoBehaviour
{
    public GameObject container;
    
    [Header("UI Elements")]
    public TextMeshProUGUI scoreTextDisplay; // Drag your Score Text here

    void Update()
    {
        // Toggle the pause menu on and off with Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (container.activeSelf)
            {
                ResumeButton();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        container.SetActive(true);
        Time.timeScale = 0; // Freeze the game
        PauseController.SetPause(true); // Tell your other script the game is paused

        // Animate the menu scaling up from 0 to 1
        container.transform.localScale = Vector3.zero;
        container.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).SetUpdate(true); 

        // Fetch the score that GameUI saved, default to 0 if not found
        int currentScore = PlayerPrefs.GetInt("PlayerScore", 0);
        
        if (scoreTextDisplay != null) 
        {
            scoreTextDisplay.text = "SCORE: " + currentScore;
        }
    }

    public void ResumeButton()
    {
        container.SetActive(false);
        Time.timeScale = 1;
        PauseController.SetPause(false);
    }

    public void MainMenuButton()
    {
        Time.timeScale = 1; // CRITICAL: Always reset time before changing scenes!
        PauseController.SetPause(false);
        UnityEngine.SceneManagement.SceneManager.LoadScene("NewStartScene");
    }
}