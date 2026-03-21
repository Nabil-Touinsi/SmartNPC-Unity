using UnityEngine;

[RequireComponent(typeof(DialogueCameraController))]
public class CameraFollow : MonoBehaviour
{
    private void Awake()
    {
        DialogueCameraController controller = GetComponent<DialogueCameraController>();
        if (controller == null)
        {
            gameObject.AddComponent<DialogueCameraController>();
        }
    }
}