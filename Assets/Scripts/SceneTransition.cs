using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MaskTransitions;
public class SceneTransition : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        // Use GetKey for the modifier (Alt) and GetKeyDown for the trigger (W)
        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.W))
        {
            GeminiScene();
        }
    }

    public void GeminiScene()
    {
        TransitionManager.Instance.LoadLevel("GeminiScene");
    }

    public void TTSScene()
        {
            TransitionManager.Instance.LoadLevel("TTSScene");
    }
}
