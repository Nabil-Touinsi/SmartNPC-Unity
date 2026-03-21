using UnityEngine;

public class SimpleSway : MonoBehaviour
{
    public Vector3 rotationAmplitude = new Vector3(0f, 0f, 3f);
    public Vector3 rotationSpeed = new Vector3(0f, 0f, 1.2f);
    public bool randomPhase = true;

    private Quaternion baseLocalRotation;
    private Vector3 phaseOffset;

    private void Awake()
    {
        baseLocalRotation = transform.localRotation;

        if (randomPhase)
        {
            phaseOffset = new Vector3(
                Random.Range(0f, 10f),
                Random.Range(0f, 10f),
                Random.Range(0f, 10f)
            );
        }
    }

    private void Update()
    {
        float rx = Mathf.Sin(Time.time * rotationSpeed.x + phaseOffset.x) * rotationAmplitude.x;
        float ry = Mathf.Sin(Time.time * rotationSpeed.y + phaseOffset.y) * rotationAmplitude.y;
        float rz = Mathf.Sin(Time.time * rotationSpeed.z + phaseOffset.z) * rotationAmplitude.z;

        Quaternion offset = Quaternion.Euler(rx, ry, rz);
        transform.localRotation = baseLocalRotation * offset;
    }
}