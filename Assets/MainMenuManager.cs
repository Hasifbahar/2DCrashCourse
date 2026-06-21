using UnityEngine;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Score Pop-Up")]
    public GameObject scorePanel; // The UI window that pops up
    public TextMeshProUGUI scoreText; // The text that shows the number

    void Start()
    {
        // Make sure the score panel is hidden when the game first loads
        if(scorePanel != null)
        {
            scorePanel.SetActive(false);
        }
    }

    public void OpenScorePanel()
    {
        scorePanel.SetActive(true); // Show the menu

        // Fetch the saved score, defaulting to 0 if they haven't played yet
        int savedScore = PlayerPrefs.GetInt("PlayerScore", 0);
        
        if (scoreText != null) 
        {
            scoreText.text = "SCORE: " + savedScore;
        }
    }

    public void CloseScorePanel()
    {
        scorePanel.SetActive(false); // Hide the menu
    }

    // Add this new method to handle starting the game
    public void StartGame()
    {
        // 1. Reset the score
        PlayerPrefs.SetInt("PlayerScore", 0);
        PlayerPrefs.Save(); 
    
        // 2. Unpause the game
        Time.timeScale = 1; 

        // 3. Trigger your smooth TransitionManager fade animation!
        // (Make sure the exact name of your scene goes inside the quotes)
        MaskTransitions.TransitionManager.Instance.LoadLevel("Level 1*"); 
    }
}