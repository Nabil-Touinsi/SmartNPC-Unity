using UnityEngine;
using UnityEditor;

public static class ShopAmbientBuilder
{
    [MenuItem("Tools/SmartNPC/Apply Ambient Life")]
    public static void ApplyAmbientLife()
    {
        ApplySignSway();
        ApplyLampSway();
        ApplyLightPulse();

        Debug.Log("Ambient life appliquée avec succès.");
    }

    private static void ApplySignSway()
    {
        GameObject sign = GameObject.Find("Shop_Sign");
        if (sign == null)
        {
            Debug.LogWarning("Shop_Sign introuvable.");
            return;
        }

        SimpleSway sway = sign.GetComponent<SimpleSway>();
        if (sway == null)
            sway = sign.AddComponent<SimpleSway>();

        sway.rotationAmplitude = new Vector3(0f, 0f, 1.2f);
        sway.rotationSpeed = new Vector3(0f, 0f, 0.9f);

        EditorUtility.SetDirty(sign);
    }

    private static void ApplyLampSway()
    {
        GameObject lampsRoot = GameObject.Find("Hanging_Lamps");
        if (lampsRoot == null)
        {
            Debug.LogWarning("Hanging_Lamps introuvable.");
            return;
        }

        SimpleSway sway = lampsRoot.GetComponent<SimpleSway>();
        if (sway == null)
            sway = lampsRoot.AddComponent<SimpleSway>();

        sway.rotationAmplitude = new Vector3(1.5f, 0f, 1.5f);
        sway.rotationSpeed = new Vector3(0.8f, 0f, 1.1f);

        EditorUtility.SetDirty(lampsRoot);
    }

    private static void ApplyLightPulse()
    {
        ApplyPulseToObject("ShopLight_Warm_Left", 0.12f, 1.0f);
        ApplyPulseToObject("ShopLight_Warm_Right", 0.12f, 1.15f);
    }

    private static void ApplyPulseToObject(string objectName, float amplitude, float speed)
    {
        GameObject obj = GameObject.Find(objectName);
        if (obj == null)
        {
            Debug.LogWarning(objectName + " introuvable.");
            return;
        }

        SoftLightPulse pulse = obj.GetComponent<SoftLightPulse>();
        if (pulse == null)
            pulse = obj.AddComponent<SoftLightPulse>();

        pulse.amplitude = amplitude;
        pulse.speed = speed;

        EditorUtility.SetDirty(obj);
    }
}