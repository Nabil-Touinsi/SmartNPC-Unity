using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class Ollama : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField playerInput;
    public TextMeshProUGUI npcText;

    [Header("Ollama Settings")]
    public string apiUrl = "http://localhost:11434/api/generate";
    public string modelName = "llama3.2:3b";

    private bool isRequestRunning = false;

    public void SendMessageToAI()
    {
        if (isRequestRunning) return;

        string question = playerInput.text.Trim();

        if (string.IsNullOrEmpty(question))
        {
            npcText.text = "Écris une question.";
            return;
        }

        StartCoroutine(CallOllama(question));
    }

    private IEnumerator CallOllama(string question)
    {
        isRequestRunning = true;
        npcText.text = "Le NPC réfléchit...";

        string prompt =
            "Tu es un PNJ de jeu vidéo immersif. " +
            "Réponds naturellement et brièvement.\n\n" +
            "Joueur: " + question + "\n" +
            "NPC:";

        OllamaRequest body = new OllamaRequest
        {
            model = modelName,
            prompt = prompt,
            stream = false
        };

        string jsonData = JsonUtility.ToJson(body);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            string responseText = request.downloadHandler.text;
            Debug.Log("OLLAMA RAW RESPONSE: " + responseText);

            if (request.result != UnityWebRequest.Result.Success)
            {
                npcText.text = "Erreur IA : " + request.responseCode;
                Debug.LogError("OLLAMA ERROR: " + responseText);
                isRequestRunning = false;
                yield break;
            }

            OllamaResponse response = null;

            try
            {
                response = JsonUtility.FromJson<OllamaResponse>(responseText);
            }
            catch (Exception e)
            {
                npcText.text = "Erreur lecture réponse.";
                Debug.LogError("Parsing error: " + e.Message);
                isRequestRunning = false;
                yield break;
            }

            if (response == null || string.IsNullOrEmpty(response.response))
            {
                npcText.text = "Aucune réponse.";
            }
            else
            {
                npcText.text = response.response.Trim();
                playerInput.text = "";
            }
        }

        isRequestRunning = false;
    }

    [Serializable]
    public class OllamaRequest
    {
        public string model;
        public string prompt;
        public bool stream;
    }

    [Serializable]
    public class OllamaResponse
    {
        public string model;
        public string response;
        public bool done;
    }
}