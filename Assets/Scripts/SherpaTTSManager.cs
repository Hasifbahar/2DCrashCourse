using UnityEngine;
using Cysharp.Threading.Tasks;
using PonyuDev.SherpaOnnx.Tts;
using PonyuDev.SherpaOnnx.Tts.Data; // For TtsGenerationConfig
using PonyuDev.SherpaOnnx.Tts.Engine; // For TtsResult and Callbacks
using System;

public class SherpaTTSManager : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    [Header("Speech Config")]
    [Range(0.5f, 2.0f)] public float speed = 1.0f;
    public int speakerId = 0;
    public float silenceScale = 0.2f;
    public int numSteps = 5;

    private ITtsService _tts;
    private bool _isReady = false;

    public bool IsReady => _isReady;

    private async void Awake()
    {
        if (_audioSource == null) _audioSource = GetComponent<AudioSource>();

        try
        {
            // 1. Manually create the service (No MonoBehaviour needed)
            _tts = new TtsService();

            // 2. Async initialization is required (especially for Android)
            await _tts.InitializeAsync();

            _isReady = true;
            Debug.Log("<color=#55ff55>Manual TTS Service Initialized.</color>");
        }
        catch (Exception e)
        {
            Debug.LogError($"TTS Initialization failed: {e.Message}");
        }
    }

    public async UniTask<TtsResult> SpeakAsync(string text)
    {
        if (!_isReady || string.IsNullOrEmpty(text)) return null;

        Stop();

        var config = new TtsGenerationConfig
        {
            Speed = speed,
            SpeakerId = speakerId,
            SilenceScale = silenceScale,
            NumSteps = numSteps
        };

        try
        {
            TtsCallbackProgress progressCallback = (samples, count, progress) => 1;

            // Change from GenerateWithConfigAsync to GenerateWithCallbackProgressAsync
            // This is the method the engine was "falling back" to anyway.
            var result = await _tts.GenerateWithCallbackProgressAsync(
                text,
                speed,
                speakerId,
                progressCallback
            );

            if (result != null && result.IsValid)
            {
                AudioClip clip = result.ToAudioClip("Gemini_Voice");
                _audioSource.clip = clip;
                _audioSource.Play();

                await UniTask.WaitUntil(() => _audioSource.isPlaying);
                return result;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"TTS Generation Error: {e.Message}");
        }

        return null;
    }

    public void Stop()
    {
        if (_audioSource != null && _audioSource.isPlaying)
            _audioSource.Stop();
    }

    private void OnDestroy()
    {
        // 6. Manual disposal is required for POCO services
        _tts?.Dispose();
    }
}