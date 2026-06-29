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
    }

   public void OnLevel2Click()
   {
        TransitionManager.Instance.LoadLevel("Level 2", 0.5f);
    }

   public void OnLevel3Click()
   {
		TransitionManager.Instance.LoadLevel("Level 3", 0.5f);
    }

   // --- NEW: Added Level 4 transition ---
   public void OnLevel4Click()
   {
        TransitionManager.Instance.LoadLevel("Level 4", 0.5f);
   }
}