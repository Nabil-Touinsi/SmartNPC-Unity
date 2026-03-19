using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.IO;

public class GroqService : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField playerInput;
    public Transform content;
    public GameObject messagePrefab;

    [Header("Groq Config")]
    [SerializeField] private string url = "https://api.groq.com/openai/v1/chat/completions";
    [SerializeField] private string model = "llama-3.1-8b-instant";

    private string apiKey;

 private void Awake()
{
    LoadEnv();

    apiKey = System.Environment.GetEnvironmentVariable("GROQ_API_KEY");

    if (string.IsNullOrWhiteSpace(apiKey))
    {
        Debug.LogError("GROQ_API_KEY introuvable !");
    }
}
void LoadEnv()
{
    string path = Path.Combine(Application.dataPath, "..", ".env");

    if (!File.Exists(path))
    {
        Debug.LogWarning(".env non trouvé");
        return;
    }

    foreach (var line in File.ReadAllLines(path))
    {
        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
            continue;

        var parts = line.Split('=');
        if (parts.Length == 2)
        {
            System.Environment.SetEnvironmentVariable(parts[0], parts[1]);
        }
    }
}

    public void SendMessageToAI()
    {
        string message = playerInput.text;

        if (string.IsNullOrWhiteSpace(message))
            return;

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            AddMessage("Erreur : clé API Groq introuvable.");
            return;
        }

        AddMessage("Vous : " + message);
        playerInput.text = "";

        StartCoroutine(CallGroqAPI(message));
    }

    IEnumerator CallGroqAPI(string userMessage)
    {
        AddMessage("NPC : ...");

        string safeMessage = userMessage
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n");

        string jsonBody =
            "{ \"model\": \"" + model + "\", \"messages\": [{\"role\":\"user\",\"content\":\"" + safeMessage + "\"}] }";

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            AddMessage("Erreur : " + request.error);
        }
        else
        {
            string response = request.downloadHandler.text;
            string parsed = ExtractContent(response);
            AddMessage("NPC : " + parsed);
        }
    }

    void AddMessage(string message)
    {
        GameObject newMessage = Instantiate(messagePrefab, content);
        newMessage.SetActive(true);

        TextMeshProUGUI textComponent = newMessage.GetComponent<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = message;
        }

        Canvas.ForceUpdateCanvases();
    }

    string ExtractContent(string json)
    {
        string marker = "\"content\":\"";
        int start = json.IndexOf(marker);

        if (start == -1)
            return "Erreur parsing";

        start += marker.Length;
        int end = json.IndexOf("\"", start);

        if (end == -1)
            return "Erreur parsing";

        return json.Substring(start, end - start)
                   .Replace("\\n", "\n")
                   .Replace("\\\"", "\"");
    }
}