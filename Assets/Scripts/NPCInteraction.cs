using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public Transform player;
    public float interactionDistance = 2.5f;

    public GameObject interactionText;
    public GameObject dialoguePanel;

    private bool dialogueOpen = false;

    void Start()
    {
        interactionText.SetActive(false);
        dialoguePanel.SetActive(false);
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (!dialogueOpen)
        {
            if (distance <= interactionDistance)
            {
                interactionText.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    dialoguePanel.SetActive(true);
                    interactionText.SetActive(false);
                    dialogueOpen = true;

                    Debug.Log("Dialogue ouvert");
                }
            }
            else
            {
                interactionText.SetActive(false);
            }
        }
    }
}