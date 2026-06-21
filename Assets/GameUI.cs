using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public int score = 0;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private GameObject tutorial1;
    [SerializeField] private TMP_Text tutorialText;
    [SerializeField] private Image tutorialImage;

    // --- NEW: Load the score when the UI starts ---
    private void Start()
    {
        // Load the saved score, default to 0 if playing for the first time
        score = PlayerPrefs.GetInt("PlayerScore", 0);
        UpdateScore(); // Update the text right away
    }

    // --- MODIFIED: Update the text AND save the game ---
    public void UpdateScore()
    {
        scoreText.text = "Score: " + score;

        // Save the score permanently so it doesn't reset when changing levels
        PlayerPrefs.SetInt("PlayerScore", score);
        PlayerPrefs.Save();
    }

    // --- NEW: Method to penalize the player safely ---
    public void SubtractScore(int amount)
    {
        score -= amount;
        
        // Prevent the score from going into negative numbers
        if (score < 0) 
        {
            score = 0; 
        }
        
        UpdateScore(); // This updates the text and saves the lower score
    }

    // --- NEW: Method to add score ---
    public void AddScore(int amount)
    {
        score += amount;
        UpdateScore(); // This updates the text and saves the higher score
    }

    

    public IEnumerator ShowTutorialForSeconds(float seconds)
    {
        tutorial1.SetActive(true);
        tutorialImage.DOColor(new Color(1, 1, 1, 1), 0.5f);
        tutorialText.DOColor(new Color(0,0,0,1), 0.5f);
        yield return new WaitForSeconds(seconds);
    }
    
    // MODIFIED: Now it takes a specific message and updates the UI text!
    public void ShowTutorial(string clueMessage)
    {
        tutorialText.text = clueMessage;
        StartCoroutine(ShowTutorialForSeconds(5f)); // You might want to increase this from 3f to 5f so players have time to read the Java clues!
    }

    public void HideTutorial() {
        tutorialImage.DOColor(new Color(1, 1, 1, 0), 0.5f);
        tutorialText.DOColor(new Color(0, 0, 0, 0), 0.5f).OnComplete(() => tutorial1.SetActive(false));
    }
}