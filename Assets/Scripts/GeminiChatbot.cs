using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable] public class GeminiPart { public string text; }
[System.Serializable] public class GeminiContent { public string role; public List<GeminiPart> parts; }
[System.Serializable] public class GeminiSystemInstruction { public List<GeminiPart> parts; }
[System.Serializable] public class GeminiRequest { public List<GeminiContent> contents; public GeminiSystemInstruction system_instruction; }
[System.Serializable] public class GeminiResponse { public Candidate[] candidates; [System.Serializable] public class Candidate { public GeminiContent content; } }

[System.Serializable]
public class ElevenLabsRequest
{
    public string text;
    public string model_id = "eleven_flash_v2_5";
    public VoiceSettings voice_settings;

    [System.Serializable]
    public class VoiceSettings
    {
        public float stability = 0.5f;
        public float similarity_boost = 0.75f;
    }
}

[RequireComponent(typeof(AudioSource))]
public class GeminiChatbot : MonoBehaviour
{
    [Header("Config Settings")]
    [Tooltip("Drag and drop your secrets.json file here.")]
    [SerializeField] private TextAsset apiConfigFile;

    [System.Serializable]
    private class ApiSecrets
    {
        public string geminiApiKey;
        public string elevenLabsApiKey;
    }

    [Header("Feature Toggles")]
    [Tooltip("Check this to enable ElevenLabs Voice generation. Uncheck to run text-only.")]
    [SerializeField] private bool useElevenLabsTTS = true;

    [Header("API Config (Gemini)")]
    private string geminiApiKey;
    [SerializeField] private GeminiModel selectedModel = GeminiModel.Gemini_3_1_Flash_Lite;

    public enum GeminiModel { Gemini_3_1_Flash_Lite, Gemini_3_Flash, Gemini_2_5_Flash, Gemini_2_5_Flash_Lite }

    [Header("API Config (ElevenLabs)")]
    private string elevenLabsApiKey;
    [Tooltip("The ID of the voice you want to use")]
    [SerializeField] private string voiceId = "21m00Tcm4TNLbtqAWWHP";

    [Header("UI")]
    public TMP_InputField inputField;
    public TMP_Text chatDisplay;
    public ScrollRect scrollRect;
    public Button sendButton;

    [Header("Settings")]
    [TextArea(3, 5)] public string systemPrompt = "You are a helpful assistant in a Unity game. Be concise.";
    [TextArea(3, 5)] public string startSpeech = "Hello! I'm Gemini, your in-game assistant. How can I help you today?";
    public float typeSpeed = 0.02f;
    public int maxHistory = 3;

    private List<GeminiContent> chatHistory = new List<GeminiContent>();
    private Coroutine chatRoutine;
    private AudioSource audioSource;

    [Header("DOTWEEN")]
    private Tween thinkingTween;
    private const int maxDots = 5;
    private string thinkingBase = "\n<color=#000000><i>Gemini is thinking";
    private int thinkingStartIndex = -1;
    private float thinkingTimer = 0f;

    void Awake()
    {
        LoadApiKeys();
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        sendButton.onClick.AddListener(OnSendClick);
        chatDisplay.text = "<color=#013220><i>System: Connection ready.</i></color>\n";
        StartCoroutine(ForceScroll());
        StartCoroutine(DisplayAndSpeakGreeting());
    }

    private string GetModelIdentifier() => selectedModel switch
    {
        GeminiModel.Gemini_3_1_Flash_Lite => "gemini-3.1-flash-lite-preview",
        GeminiModel.Gemini_3_Flash => "gemini-3-flash",
        GeminiModel.Gemini_2_5_Flash => "gemini-2.5-flash",
        GeminiModel.Gemini_2_5_Flash_Lite => "gemini-2.5-flash-lite",
        _ => "gemini-3.1-flash-lite-preview"
    };

    private string GetGeminiEndpoint() => $"https://generativelanguage.googleapis.com/v1beta/models/{GetModelIdentifier()}:generateContent?key={geminiApiKey}";
    private string GetElevenLabsEndpoint() => $"https://api.elevenlabs.io/v1/text-to-speech/{voiceId}";

    private void LoadApiKeys()
    {
        if (apiConfigFile != null)
        {
            string jsonContent = apiConfigFile.text;
            ApiSecrets secrets = JsonUtility.FromJson<ApiSecrets>(jsonContent);

            if (secrets != null)
            {
                geminiApiKey = secrets.geminiApiKey;
                elevenLabsApiKey = secrets.elevenLabsApiKey;
                Debug.Log("API Keys loaded successfully from TextAsset.");
            }
        }
        else
        {
            Debug.LogError("API Config File is missing! Please assign your secrets.json file.");
        }
    }

    public void OnSendClick()
    {
        if (string.IsNullOrWhiteSpace(inputField.text))
        {
            inputField.transform.DOShakePosition(0.3f, 8f);
            return;
        }

        string message = inputField.text.Trim();
        inputField.text = "";

        if (chatRoutine != null) StopCoroutine(chatRoutine);
        chatRoutine = StartCoroutine(ChatFlow(message));
    }

    private IEnumerator DisplayAndSpeakGreeting()
    {
        sendButton.interactable = false;
        AudioClip greetingClip = null;

        // Check if TTS feature toggle is true
        if (useElevenLabsTTS)
        {
            yield return FetchElevenLabsAudio(startSpeech, clip => greetingClip = clip);
        }

        yield return Typewriter(startSpeech, greetingClip);
        sendButton.interactable = true;
    }

    private IEnumerator ChatFlow(string message)
    {
        sendButton.interactable = false;
        AppendMessage("Player", message, Color.cyan);
        StartThinkingAnimation();
        ScrollToBottom();
        AddToHistory("user", message);

        GeminiRequest requestData = new GeminiRequest
        {
            contents = chatHistory,
            system_instruction = new GeminiSystemInstruction { parts = new List<GeminiPart> { new GeminiPart { text = systemPrompt } } }
        };

        string json = JsonUtility.ToJson(requestData);
        using (UnityWebRequest req = new UnityWebRequest(GetGeminiEndpoint(), "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();
            StopThinkingAnimation();

            if (req.result != UnityWebRequest.Result.Success)
            {
                AppendMessage("System", $"Error: {req.error}", Color.red);
                sendButton.interactable = true;
                yield break;
            }

            string reply = ParseResponse(req.downloadHandler.text);
            if (string.IsNullOrEmpty(reply))
            {
                AppendMessage("System", "Empty AI response.", Color.yellow);
                sendButton.interactable = true;
                yield break;
            }

            AddToHistory("model", reply);

            AudioClip downloadedVoiceClip = null;

            // Check if TTS feature toggle is true before pulling data from network
            if (useElevenLabsTTS)
            {
                yield return FetchElevenLabsAudio(reply, clip => downloadedVoiceClip = clip);
            }
            else
            {
                // Ensure audio completely cuts off if feature is deactivated dynamically mid-sentence
                if (audioSource.isPlaying) audioSource.Stop();
            }

            yield return Typewriter(reply, downloadedVoiceClip);
        }
        sendButton.interactable = true;
    }

    private IEnumerator FetchElevenLabsAudio(string textToSpeak, System.Action<AudioClip> onAudioReady)
    {
        string cleanText = textToSpeak.Replace("*", "").Replace("#", "").Replace("_", "").Trim();

        ElevenLabsRequest ttsRequest = new ElevenLabsRequest
        {
            text = cleanText,
            voice_settings = new ElevenLabsRequest.VoiceSettings { stability = 0.5f, similarity_boost = 0.75f }
        };

        string jsonPayload = JsonUtility.ToJson(ttsRequest);

        using (UnityWebRequest ttsReq = new UnityWebRequest(GetElevenLabsEndpoint() + "?output_format=mp3_44100_128", "POST"))
        {
            ttsReq.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonPayload));
            ttsReq.downloadHandler = new DownloadHandlerAudioClip(GetElevenLabsEndpoint(), AudioType.MPEG);

            ttsReq.SetRequestHeader("Content-Type", "application/json");
            ttsReq.SetRequestHeader("xi-api-key", elevenLabsApiKey);
            ttsReq.SetRequestHeader("accept", "audio/mpeg");

            yield return ttsReq.SendWebRequest();

            if (ttsReq.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(ttsReq);
                if (clip != null) onAudioReady?.Invoke(clip);
            }
            else
            {
                Debug.LogError($"ElevenLabs Error: {ttsReq.error}");
                onAudioReady?.Invoke(null);
            }
        }
    }

    private IEnumerator Typewriter(string text, AudioClip voiceClip)
    {
        string header = "\n<color=#00008B><b>Gemini:</b></color> ";
        StopThinkingAnimation();
        chatDisplay.text += header;

        float calculatedTypeSpeed = typeSpeed;

        // Dynamic evaluation path handles missing clip gracefully
        if (useElevenLabsTTS && voiceClip != null)
        {
            calculatedTypeSpeed = voiceClip.length / Mathf.Max(text.Length, 1);
            audioSource.clip = voiceClip;
            audioSource.Play();
        }

        for (int i = 0; i < text.Length; i++)
        {
            chatDisplay.text += text[i];
            ScrollToBottom();
            yield return new WaitForSeconds(calculatedTypeSpeed);
        }
    }

    // --- Core UI & Animation Helpers ---
    private void StartThinkingAnimation()
    {
        thinkingTimer = 0f;
        string initialText = thinkingBase + ".</i></color>";
        thinkingStartIndex = chatDisplay.text.Length;
        chatDisplay.text += initialText;
        ScrollToBottom();

        thinkingTween = DOTween.To(() => thinkingTimer, x => thinkingTimer = x, 1f, 1f)
            .SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart)
            .OnUpdate(() =>
            {
                if (thinkingStartIndex < 0) return;
                int dotsCount = Mathf.FloorToInt(thinkingTimer * (maxDots + 1));
                string dots = new string('.', dotsCount);
                chatDisplay.text = chatDisplay.text.Substring(0, thinkingStartIndex) + $"{thinkingBase}{dots}</i></color>";
                ScrollToBottom();
            });
    }

    private void StopThinkingAnimation()
    {
        if (thinkingTween != null && thinkingTween.IsActive()) thinkingTween.Kill();
        chatDisplay.DOKill();
        if (thinkingStartIndex >= 0 && thinkingStartIndex < chatDisplay.text.Length)
            chatDisplay.text = chatDisplay.text.Substring(0, thinkingStartIndex);
        thinkingStartIndex = -1;
    }

    private void AppendMessage(string sender, string msg, Color color)
    {
        string hex = ColorUtility.ToHtmlStringRGB(color);
        chatDisplay.text += $"\n<color=#{hex}><b>{sender}:</b></color> {msg}";
        ScrollToBottom();
    }

    private void AddToHistory(string role, string text)
    {
        chatHistory.Add(new GeminiContent { role = role, parts = new List<GeminiPart> { new GeminiPart { text = text } } });
        while (chatHistory.Count > maxHistory) chatHistory.RemoveAt(0);
    }

    private string ParseResponse(string json)
    {
        try
        {
            GeminiResponse res = JsonUtility.FromJson<GeminiResponse>(json);
            if (res?.candidates?.Length > 0 && res.candidates[0].content?.parts?.Count > 0)
                return res.candidates[0].content.parts[0].text;
        }
        catch { Debug.LogWarning("Failed to parse response."); }
        return null;
    }

    private void ScrollToBottom() { Canvas.ForceUpdateCanvases(); scrollRect.DONormalizedPos(new Vector2(0, 0), 0.25f); }
    private IEnumerator ForceScroll() { yield return null; scrollRect.verticalNormalizedPosition = 0f; }

    // --- NEW: BACK BUTTON METHOD ---
    public void ReturnToPreviousLevel()
    {
        // 1. Read the memory to see where we came from
        string levelToLoad = PlayerPrefs.GetString("LastLevel", "Level 1");

        // 2. Use your custom TransitionManager for a smooth exit!
        MaskTransitions.TransitionManager.Instance.LoadLevel(levelToLoad);
    }
}