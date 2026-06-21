using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundEffectManager : MonoBehaviour
{
    private static SoundEffectManager Instance;

    private static AudioSource audioSource; // For SFX
    private static AudioSource musicSource; // NEW: For Background Music
    
    private static SoundEffectLibrary soundEffectLibrary;
    [SerializeField] private Slider sfxSlider;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            
            // Grab BOTH AudioSources we stacked on the GameObject!
            AudioSource[] sources = GetComponents<AudioSource>();
            audioSource = sources[0]; // The first one is for UI/SFX
            
            if (sources.Length > 1) 
            {
                musicSource = sources[1]; // The second one is for Music
            }
            
            soundEffectLibrary = GetComponent<SoundEffectLibrary>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- NEW BACKGROUND MUSIC METHOD ---
    public static void PlayBGM(AudioClip newMusic)
    {
        if (musicSource == null || newMusic == null) return;

        // THE MAGIC TRICK: If the song is already playing, do absolutely nothing!
        // This keeps the music seamless between the Start Menu and Level Selection.
        if (musicSource.clip == newMusic) return;

        // If it's a new song, swap the tape and hit play
        musicSource.clip = newMusic;
        musicSource.Play();
    }
    // -----------------------------------

    public static void Play(string soundName)
    {
        AudioClip audioClip = soundEffectLibrary.GetRandomClip(soundName);
        if(audioClip != null)
        {
            audioSource.PlayOneShot(audioClip);
        }
    }

    public void PlayUIButtonSound(string soundName)
    {
        Play(soundName);
    }

    void Start()
    {
        if (sfxSlider != null) 
        {
            sfxSlider.onValueChanged.AddListener(delegate { OnValueChanged(); });
        }
    }

    public static void SetVolume(float volume)
    {
        audioSource.volume = volume;
        // Optionally add: if (musicSource != null) musicSource.volume = volume; 
    }

    public void OnValueChanged()
    {
        if (sfxSlider != null) SetVolume(sfxSlider.value);
    }
}