using UnityEngine;

public class NPCSimpleLife : MonoBehaviour
{
    [Header("Auto refs")]
    public Transform player;
    public NPCInteraction npcInteraction;

    [Header("Runtime refs")]
    public Transform visualRoot;
    public Transform body;
    public Transform head;
    public Transform armLeft;
    public Transform armRight;
    public Transform eyeLeft;
    public Transform eyeRight;

    [Header("Idle")]
    public float idleBobAmplitude = 0.025f;
    public float idleBobSpeed = 1.8f;
    public float armSwingAngle = 4f;
    public float armSwingSpeed = 1.4f;

    [Header("Look At Player")]
    public float headTurnSpeed = 6f;
    public float maxHeadYaw = 22f;
    public float maxHeadPitch = 10f;
    public float lookDistance = 4.5f;

    [Header("Talking Reaction")]
    public float talkBobMultiplier = 1.8f;
    public float talkArmMultiplier = 2.2f;
    public float talkHeadNodAngle = 5f;
    public float talkHeadNodSpeed = 5.5f;

    [Header("Blink")]
    public Vector2 blinkIntervalRange = new Vector2(2.5f, 5.5f);
    public float blinkDuration = 0.08f;

    private Vector3 visualRootBaseLocalPos;
    private Vector3 bodyBaseLocalPos;
    private Vector3 headBaseLocalPos;
    private Vector3 armLeftBaseLocalPos;
    private Vector3 armRightBaseLocalPos;
    private Vector3 eyeLeftBaseLocalScale;
    private Vector3 eyeRightBaseLocalScale;

    private Quaternion bodyBaseLocalRot;
    private Quaternion headBaseLocalRot;
    private Quaternion armLeftBaseLocalRot;
    private Quaternion armRightBaseLocalRot;

    private float blinkTimer;
    private float blinkEndTime;
    private bool blinking;

    private void Awake()
    {
        AutoBind();
        CacheBasePose();
        ResetBlinkTimer();
    }

    private void LateUpdate()
    {
        if (visualRoot == null || head == null)
        {
            AutoBind();

            if (visualRoot == null || head == null)
                return;
        }

        bool isTalking = IsDialogueOpen();

        ApplyIdleMotion(isTalking);
        ApplyHeadLook(isTalking);
        ApplyBlink();
    }

    [ContextMenu("Auto Bind")]
    public void AutoBind()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.Find("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        if (npcInteraction == null)
            npcInteraction = GetComponent<NPCInteraction>();

        if (visualRoot == null)
            visualRoot = transform.Find("NPC_Visual");

        if (visualRoot == null)
            return;

        if (body == null)
            body = visualRoot.Find("Body");

        if (head == null)
            head = visualRoot.Find("Head");

        if (armLeft == null)
            armLeft = visualRoot.Find("Arm_Left");

        if (armRight == null)
            armRight = visualRoot.Find("Arm_Right");

        if (eyeLeft == null && head != null)
            eyeLeft = head.Find("Eye_Left");

        if (eyeRight == null && head != null)
            eyeRight = head.Find("Eye_Right");
    }

    private void CacheBasePose()
    {
        if (visualRoot != null)
            visualRootBaseLocalPos = visualRoot.localPosition;

        if (body != null)
        {
            bodyBaseLocalPos = body.localPosition;
            bodyBaseLocalRot = body.localRotation;
        }

        if (head != null)
        {
            headBaseLocalPos = head.localPosition;
            headBaseLocalRot = head.localRotation;
        }

        if (armLeft != null)
        {
            armLeftBaseLocalPos = armLeft.localPosition;
            armLeftBaseLocalRot = armLeft.localRotation;
        }

        if (armRight != null)
        {
            armRightBaseLocalPos = armRight.localPosition;
            armRightBaseLocalRot = armRight.localRotation;
        }

        if (eyeLeft != null)
            eyeLeftBaseLocalScale = eyeLeft.localScale;

        if (eyeRight != null)
            eyeRightBaseLocalScale = eyeRight.localScale;
    }

    private void ApplyIdleMotion(bool isTalking)
    {
        float t = Time.time;

        float bobAmp = idleBobAmplitude * (isTalking ? talkBobMultiplier : 1f);
        float bobSpeed = idleBobSpeed * (isTalking ? 1.35f : 1f);
        float bob = Mathf.Sin(t * bobSpeed) * bobAmp;

        if (visualRoot != null)
        {
            Vector3 targetPos = visualRootBaseLocalPos + new Vector3(0f, bob, 0f);
            visualRoot.localPosition = Vector3.Lerp(visualRoot.localPosition, targetPos, Time.deltaTime * 8f);
        }

        float armMul = isTalking ? talkArmMultiplier : 1f;
        float armWave = Mathf.Sin(t * armSwingSpeed * armMul) * armSwingAngle;

        if (armLeft != null)
        {
            Quaternion target = armLeftBaseLocalRot * Quaternion.Euler(0f, 0f, -armWave);
            armLeft.localRotation = Quaternion.Slerp(armLeft.localRotation, target, Time.deltaTime * 6f);
        }

        if (armRight != null)
        {
            Quaternion target = armRightBaseLocalRot * Quaternion.Euler(0f, 0f, armWave);
            armRight.localRotation = Quaternion.Slerp(armRight.localRotation, target, Time.deltaTime * 6f);
        }

        if (body != null)
        {
            float bodyTilt = Mathf.Sin(t * 1.1f) * 1.5f;
            Quaternion target = bodyBaseLocalRot * Quaternion.Euler(0f, 0f, bodyTilt);
            body.localRotation = Quaternion.Slerp(body.localRotation, target, Time.deltaTime * 4f);
        }
    }

    private void ApplyHeadLook(bool isTalking)
    {
        if (head == null)
            return;

        float yaw = 0f;
        float pitch = 0f;

        if (player != null)
        {
            Vector3 toPlayer = player.position - head.position;
            float distance = toPlayer.magnitude;

            if (distance <= lookDistance && distance > 0.001f)
            {
                Vector3 localDir = transform.InverseTransformDirection(toPlayer.normalized);

                yaw = Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg;
                pitch = -Mathf.Atan2(localDir.y, localDir.z) * Mathf.Rad2Deg;

                yaw = Mathf.Clamp(yaw, -maxHeadYaw, maxHeadYaw);
                pitch = Mathf.Clamp(pitch, -maxHeadPitch, maxHeadPitch);
            }
        }

        if (isTalking)
        {
            pitch += Mathf.Sin(Time.time * talkHeadNodSpeed) * talkHeadNodAngle;
        }

        Quaternion targetRot = headBaseLocalRot * Quaternion.Euler(pitch, yaw, 0f);
        head.localRotation = Quaternion.Slerp(head.localRotation, targetRot, Time.deltaTime * headTurnSpeed);
    }

    private void ApplyBlink()
    {
        if (eyeLeft == null || eyeRight == null)
            return;

        if (!blinking)
        {
            blinkTimer -= Time.deltaTime;
            if (blinkTimer <= 0f)
            {
                blinking = true;
                blinkEndTime = Time.time + blinkDuration;
            }
        }

        if (blinking)
        {
            Vector3 closedScaleL = new Vector3(eyeLeftBaseLocalScale.x, 0.005f, eyeLeftBaseLocalScale.z);
            Vector3 closedScaleR = new Vector3(eyeRightBaseLocalScale.x, 0.005f, eyeRightBaseLocalScale.z);

            eyeLeft.localScale = Vector3.Lerp(eyeLeft.localScale, closedScaleL, Time.deltaTime * 30f);
            eyeRight.localScale = Vector3.Lerp(eyeRight.localScale, closedScaleR, Time.deltaTime * 30f);

            if (Time.time >= blinkEndTime)
            {
                blinking = false;
                ResetBlinkTimer();
            }
        }
        else
        {
            eyeLeft.localScale = Vector3.Lerp(eyeLeft.localScale, eyeLeftBaseLocalScale, Time.deltaTime * 20f);
            eyeRight.localScale = Vector3.Lerp(eyeRight.localScale, eyeRightBaseLocalScale, Time.deltaTime * 20f);
        }
    }

    private void ResetBlinkTimer()
    {
        blinkTimer = Random.Range(blinkIntervalRange.x, blinkIntervalRange.y);
    }

    private bool IsDialogueOpen()
    {
        if (npcInteraction == null || npcInteraction.dialoguePanel == null)
            return false;

        return npcInteraction.dialoguePanel.activeSelf;
    }
}