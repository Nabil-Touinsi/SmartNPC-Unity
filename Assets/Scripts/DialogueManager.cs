using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TMP_InputField playerInput;
    public TextMeshProUGUI npcText;
    public GameObject dialoguePanel;

    public void SendMessageToNPC()
    {
        string message = playerInput.text;

        if (!string.IsNullOrWhiteSpace(message))
        {
            npcText.text = "Tu as dit : " + message;
            playerInput.text = "";
        }
    }

    public void OpenDialogue()
    {
        dialoguePanel.SetActive(true);
    }

    public void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
    }
}