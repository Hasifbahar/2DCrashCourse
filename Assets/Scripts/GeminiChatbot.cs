using Cysharp.Threading.Tasks;
using DG.Tweening;
using PonyuDev.SherpaOnnx.Tts.Engine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable] public class GeminiPart { public string text; }
[System.Serializable]
public class GeminiContent
{
    public string role;
    public List<GeminiPart> parts;
}
[System.Serializable]
public class GeminiSystemInstruction
{
    public List<GeminiPart> parts;
}
[System.Serializable]
public class GeminiRequest
{
    public List<GeminiContent> contents;
    public GeminiSystemInstruction system_instruction;
}
[System.Serializable]
public class GeminiResponse
{
    public Candidate[] candidates;

    [System.Serializable]
    public class Candidate
    {
        public GeminiContent content;
    }
}
public class GeminiChatbot : MonoBehaviour
{
    [Header("API Config")]
    [SerializeField] private string apiKey = "YOUR_API_KEY_HERE";
    private const string endpoint =
            "https://generativelanguage.googleapis.com/v1beta/models/gemini-3.1-flash-lite-preview:generateContent";

    [Header("UI")]
    public TMP_InputField inputField;
    public TMP_Text chatDisplay;
    public ScrollRect scrollRect;
    public Button sendButton;

    [Header("Settings")]
    [TextArea(3, 5)]
    public string systemPrompt = "You are a helpful assistant in a Unity game. Be concise.";

    public float typeSpeed = 0.02f;
    public int maxHistory = 3;

    private List<GeminiContent> chatHistory = new List<GeminiContent>();
    private Coroutine chatRoutine;
    private SherpaTTSManager _ttsManager;

    [Header("DOTWEEN")]
    private Tween thinkingTween;
    private const int maxDots = 5;
    private string thinkingBase = "\n<color=#000000><i>Gemini is thinking";
    private int thinkingStartIndex = -1;
    private float thinkingTimer = 0f;
    void Start()
    {
        _ttsManager = GetComponent<SherpaTTSManager>(); // Cache it here
        _ttsManager.SpeakAsync("Hi my name is Gemini! ").Forget();
        sendButton.onClick.AddListener(OnSendClick);
        chatDisplay.text = "<color=#013220><i>System: Connection ready.</i></color>\n";
        StartCoroutine(ForceScroll());
    }

    void Update()
    {
        // Enter = Send | Shift+Enter = New line
        if (Input.GetKeyDown(KeyCode.Return) && inputField.isFocused && !Input.GetKey(KeyCode.LeftShift))
        {
            OnSendClick();
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

        if (chatRoutine != null)
            StopCoroutine(chatRoutine);

        chatRoutine = StartCoroutine(ChatFlow(message));
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
            system_instruction = new GeminiSystemInstruction
            {
                parts = new List<GeminiPart> { new GeminiPart { text = systemPrompt } }
            }
        };

        string json = JsonUtility.ToJson(requestData);
        string url = $"{endpoint}?key={apiKey}";

        using (UnityWebRequest req = new UnityWebRequest(url, "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            // Remove thinking text
            StopThinkingAnimation();

            if (req.result != UnityWebRequest.Result.Success)
            {
                AppendMessage("System", $"Error: {req.error}", Color.red);
                Debug.LogError(req.downloadHandler.text);
                sendButton.interactable = true;
                yield break;
            }

            // Parse response
            string reply = ParseResponse(req.downloadHandler.text);

            if (string.IsNullOrEmpty(reply))
            {
                AppendMessage("System", "Empty AI response.", Color.yellow);
                sendButton.interactable = true;
                yield break;
            }

            AddToHistory("model", reply);

            yield return Typewriter(reply);
        }

        sendButton.interactable = true;
    }
    private void StartThinkingAnimation()
    {
        thinkingTimer = 0f;

        string initialText = thinkingBase + ".</i></color>";

        thinkingStartIndex = chatDisplay.text.Length;
        chatDisplay.text += initialText;

        ScrollToBottom();

        thinkingTween = DOTween.To(() => thinkingTimer, x => thinkingTimer = x, 1f, 1f)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart)
            .OnUpdate(() =>
            {
                if (thinkingStartIndex < 0) return;

                
                int dotsCount = Mathf.FloorToInt(thinkingTimer * (maxDots + 1));
                string dots = new string('.', dotsCount);

                string newText = $"{thinkingBase}{dots}</i></color>";

                chatDisplay.text =
                    chatDisplay.text.Substring(0, thinkingStartIndex) + newText;

                ScrollToBottom();
            });

    }
    private void StopThinkingAnimation()
    {
        if (thinkingTween != null && thinkingTween.IsActive())
            thinkingTween.Kill();

        chatDisplay.DOKill();

        // Remove ONLY the last thinking text
        if (thinkingStartIndex >= 0 && thinkingStartIndex < chatDisplay.text.Length)
        {
            chatDisplay.text = chatDisplay.text.Substring(0, thinkingStartIndex);
        }

        thinkingStartIndex = -1;
    }
    private IEnumerator Typewriter(string text)
    {
        string header = "\n<color=#00008B><b>Gemini:</b></color> ";
        StopThinkingAnimation();
        chatDisplay.text += header;

        string cleanText = text.Replace("*", "").Replace("#", "").Replace("_", "").Trim();

        float dynamicTypeSpeed = typeSpeed; // fallback

        if (_ttsManager != null)
        {
            TtsResult result = null;

            // wait for TTS result (generation done + playback started)
            yield return UniTask.ToCoroutine(async () =>
            {
                result = await _ttsManager.SpeakAsync(cleanText);
            });

            if (result != null && result.DurationSeconds > 0f)
            {
                dynamicTypeSpeed = result.DurationSeconds / Mathf.Max(text.Length, 1);
            }
        }

        // Type with synced speed
        for (int i = 0; i < text.Length; i++)
        {
            chatDisplay.text += text[i];
            ScrollToBottom();
            yield return new WaitForSeconds(dynamicTypeSpeed);
        }
    }

    private void AppendMessage(string sender, string msg, Color color)
    {
        string hex = ColorUtility.ToHtmlStringRGB(color);
        chatDisplay.text += $"\n<color=#{hex}><b>{sender}:</b></color> {msg}";
        ScrollToBottom();
    }

    private void AddToHistory(string role, string text)
    {
        chatHistory.Add(new GeminiContent
        {
            role = role,
            parts = new List<GeminiPart> { new GeminiPart { text = text } }
        });

        // Trim history safely
        while (chatHistory.Count > maxHistory)
        {
            chatHistory.RemoveAt(0);
        }
    }

    private string ParseResponse(string json)
    {
        try
        {
            GeminiResponse res = JsonUtility.FromJson<GeminiResponse>(json);

            if (res?.candidates != null &&
                res.candidates.Length > 0 &&
                res.candidates[0].content?.parts != null &&
                res.candidates[0].content.parts.Count > 0)
            {
                return res.candidates[0].content.parts[0].text;
            }
        }
        catch
        {
            Debug.LogWarning("Failed to parse response.");
        }

        return null;
    }

    private void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.DONormalizedPos(new Vector2(0, 0), 0.25f);
    }

    private IEnumerator ForceScroll()
    {
        yield return null;
        scrollRect.verticalNormalizedPosition = 0f;
    }
}