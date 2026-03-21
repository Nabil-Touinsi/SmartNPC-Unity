using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public Transform player;
    public float interactionDistance = 2.5f;
    public GameObject interactionText;
    public GameObject dialoguePanel;

    private bool dialogueOpen = false;
    private PlayerMovement playerMovement;
    private DialogueCameraController dialogueCamera;

    void Start()
    {
        if (interactionText != null)
            interactionText.SetActive(false);

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (player != null)
            playerMovement = player.GetComponent<PlayerMovement>();

        dialogueCamera = FindFirstObjectByType<DialogueCameraController>();

        if (dialogueCamera == null)
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                dialogueCamera = cam.GetComponent<DialogueCameraController>();
            }
        }

        if (dialogueCamera != null)
        {
            dialogueCamera.player = player;
            dialogueCamera.npc = transform;
            dialogueCamera.SetDialogueState(false);
        }
    }

    void Update()
    {
        if (player == null)
            return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (!dialogueOpen)
        {
            if (distance <= interactionDistance)
            {
                if (interactionText != null)
                    interactionText.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    OpenDialogue();
                }
            }
            else
            {
                if (interactionText != null)
                    interactionText.SetActive(false);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseDialogue();
            }
        }
    }

    public void OpenDialogue()
    {
        dialogueOpen = true;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (interactionText != null)
            interactionText.SetActive(false);

        if (playerMovement != null)
            playerMovement.canMove = false;

        if (dialogueCamera != null)
        {
            dialogueCamera.player = player;
            dialogueCamera.npc = transform;
            dialogueCamera.SetDialogueState(true);
        }

        Debug.Log("Dialogue ouvert");
    }

    public void CloseDialogue()
    {
        dialogueOpen = false;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (playerMovement != null)
            playerMovement.canMove = true;

        if (dialogueCamera != null)
            dialogueCamera.SetDialogueState(false);

        Debug.Log("Dialogue fermé");
    }

    public bool IsDialogueOpen()
    {
        return dialogueOpen;
    }
}