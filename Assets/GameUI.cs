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
    public void ShowTutorial()
    {
        StartCoroutine(ShowTutorialForSeconds(3f));
    }

    public void HideTutorial() {
        tutorialImage.DOColor(new Color(1, 1, 1, 0), 0.5f);
        tutorialText.DOColor(new Color(0, 0, 0, 0), 0.5f).OnComplete(() => {
            tutorial1.SetActive(false);
        });
    }
}
