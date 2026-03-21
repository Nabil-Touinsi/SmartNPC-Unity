using UnityEngine;

public class DialogueCameraController : MonoBehaviour
{
    public Transform player;
    public Transform npc;

    [Header("Normal Follow")]
    public Vector3 normalOffset = new Vector3(0f, 3f, -6f);
    public float normalFollowSpeed = 6f;

    [Header("Dialogue View")]
    public Vector3 dialogueOffsetFromPlayer = new Vector3(0.9f, 1.8f, -1.8f);
    public float dialogueMoveSpeed = 7f;
    public float lookHeight = 1.45f;

    [Header("State")]
    public bool inDialogue = false;

    private void LateUpdate()
    {
        if (player == null)
            return;

        if (!inDialogue)
        {
            Vector3 normalTargetPos = player.position + normalOffset;
            transform.position = Vector3.Lerp(transform.position, normalTargetPos, Time.deltaTime * normalFollowSpeed);
            transform.LookAt(player.position + Vector3.up * 1.2f);
            return;
        }

        if (npc == null)
            return;

        Vector3 dirToNpc = (npc.position - player.position).normalized;
        dirToNpc.y = 0f;

        if (dirToNpc.sqrMagnitude < 0.001f)
            dirToNpc = Vector3.forward;

        Vector3 right = Vector3.Cross(Vector3.up, dirToNpc).normalized;

        Vector3 dialogueTargetPos =
            player.position
            - dirToNpc * Mathf.Abs(dialogueOffsetFromPlayer.z)
            + right * dialogueOffsetFromPlayer.x
            + Vector3.up * dialogueOffsetFromPlayer.y;

        Vector3 lookTarget = npc.position + Vector3.up * lookHeight;

        transform.position = Vector3.Lerp(transform.position, dialogueTargetPos, Time.deltaTime * dialogueMoveSpeed);

        Quaternion targetRot = Quaternion.LookRotation(lookTarget - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * dialogueMoveSpeed);
    }

    public void SetDialogueState(bool active)
    {
        inDialogue = active;
    }
}