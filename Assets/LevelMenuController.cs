using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MaskTransitions;

public class LevelMenuController : MonoBehaviour
{
   public void OnLevel1Click()
   {
		TransitionManager.Instance.LoadLevel("Level 1", 0.5f);
        //SceneManager.LoadScene("Level 1");
    }

   public void OnLevel2Click()
   {
        TransitionManager.Instance.LoadLevel("Level 2", 0.5f);
        //SceneManager.LoadScene("Level 2");
    }

   public void OnLevel3Click()
   {
		TransitionManager.Instance.LoadLevel("Level 3", 0.5f);
        //SceneManager.LoadScene("Level 3");
    }

}
