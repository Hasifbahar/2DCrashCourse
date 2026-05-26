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
    
    public void UpdateScore()
    {
        scoreText.text = "Score: " + score;
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
        tutorialText.DOColor(new Color(0, 0, 0, 0), 0.5f).OnComplete(() => {
            tutorial1.SetActive(false);
        });
    }
}