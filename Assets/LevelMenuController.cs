using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMenuController : MonoBehaviour
{
   public void OnLevel1Click()
   {
	   SceneManager.LoadScene("SampleScene");
   }

   public void OnLevel2Click()
   {
	    SceneManager.LoadScene("SampleScene2");
   }

   public void OnLevel3Click()
   {
	   SceneManager.LoadScene("SampleScene3");
   }

}
