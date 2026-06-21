using UnityEngine;
using UnityEngine.UI; // CRITICAL: Needed to modify UI Images

public class SpriteScoreDisplay : MonoBehaviour
{
    [Header("Your 0-9 Number Sprites")]
    [Tooltip("Drag your Number 0 to Number 9 sprites here IN EXACT ORDER (0 first, 9 last)")]
    public Sprite[] numberSprites;

    [Header("UI Image Slots (Left to Right)")]
    [Tooltip("Drag your UI Images here (e.g., Hundreds digit, Tens digit, Ones digit)")]
    public Image[] digitImages;

    private void OnEnable()
    {
        // Whenever this menu is turned on (like when pausing), fetch the latest score!
        int currentScore = PlayerPrefs.GetInt("PlayerScore", 0);
        UpdateScoreDisplay(currentScore);
    }

    public void UpdateScoreDisplay(int score)
    {
        // 1. Calculate the maximum possible score based on how many digits you set up
        // (e.g., if you have 3 image slots, the max score is 999)
        int maxScore = (int)Mathf.Pow(10, digitImages.Length) - 1; 
        if (score > maxScore) score = maxScore;

        // 2. Convert the score to a string, padding with leading zeros to match your image count
        // If you have 3 image slots, a score of 5 becomes "005"
        string scoreString = score.ToString("D" + digitImages.Length);

        // 3. Loop through each character in the string and update the UI Images
        for (int i = 0; i < digitImages.Length; i++)
        {
            // A neat trick to convert a character (like '5') back into a real integer (5)
            int digitValue = scoreString[i] - '0';
            
            // Assign the exact pixel art sprite from your array
            digitImages[i].sprite = numberSprites[digitValue];
        }
    }
}