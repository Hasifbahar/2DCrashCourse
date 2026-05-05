using Cysharp.Threading.Tasks;
using PonyuDev.SherpaOnnx.Tts;
using PonyuDev.SherpaOnnx.Tts.Engine;
using UnityEngine;

public class SherpaTTSManager : MonoBehaviour
{
    [SerializeField] private TtsOrchestrator _orchestrator;

    private bool _ready = false;

    private void Awake()
    {
        if (_orchestrator.IsInitialized)
        {
            _ready = true;
        }
        else
        {
            _orchestrator.Initialized += OnInitialized;
        }
    }

    private void OnInitialized()
    {
        _ready = true;
        _orchestrator.Initialized -= OnInitialized;
        Debug.Log("<color=#55ff55>TTS Ready</color>");
    }

    public async UniTask<TtsResult> SpeakAsync(string text)
    {
        if (!_ready || string.IsNullOrEmpty(text)) return null;

        var result = await _orchestrator.GenerateAndPlayAsync(text);
        return result;
    }

    public void Stop()
    {
        // No direct stop API, so fallback:
        AudioSource audio = _orchestrator.GetComponent<AudioSource>();
        if (audio != null && audio.isPlaying)
            audio.Stop();
    }
}