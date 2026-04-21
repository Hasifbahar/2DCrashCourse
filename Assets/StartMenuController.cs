using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MaskTransitions;

public class StartMenuController : MonoBehaviour
{
   public void OnStartClick()
   {
		TransitionManager.Instance.LoadLevel("LevelScene", 0.5f);
   }

   public void OnExitClick()
   {
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
		Application.Quit();
   }
}
