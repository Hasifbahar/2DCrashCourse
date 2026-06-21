using UnityEngine;

public class SceneMusicPlayer : MonoBehaviour
{
    [Header("Scene Music")]
    [Tooltip("Drag the MP3 you want to play for this scene here!")]
    public AudioClip thisSceneMusic;

    void Start()
    {
        // The second the scene loads, tell the manager to play this specific track
        if (thisSceneMusic != null)
        {
            SoundEffectManager.PlayBGM(thisSceneMusic);
        }
    }
}