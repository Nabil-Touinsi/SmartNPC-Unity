using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    [HideInInspector] public bool canMove = true;

    void Update()
    {
        if (!canMove)
            return;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveX, 0f, moveZ);
        transform.Translate(movement * speed * Time.deltaTime, Space.World);
    }
}