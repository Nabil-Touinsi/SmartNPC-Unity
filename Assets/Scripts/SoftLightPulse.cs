using UnityEngine;

public class SoftLightPulse : MonoBehaviour
{
    public float amplitude = 0.15f;
    public float speed = 1.1f;
    public bool randomPhase = true;

    private Light targetLight;
    private float baseIntensity;
    private float phase;

    private void Awake()
    {
        targetLight = GetComponent<Light>();

        if (targetLight == null)
            targetLight = GetComponentInChildren<Light>();

        if (targetLight == null)
            return;

        baseIntensity = targetLight.intensity;

        if (randomPhase)
            phase = Random.Range(0f, 10f);
    }

    private void Update()
    {
        if (targetLight == null)
            return;

        float pulse = Mathf.Sin(Time.time * speed + phase) * amplitude;
        targetLight.intensity = baseIntensity + pulse;
    }
}