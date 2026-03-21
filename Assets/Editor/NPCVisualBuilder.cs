using UnityEngine;
using UnityEditor;

public static class NPCVisualBuilder
{
    [MenuItem("Tools/SmartNPC/Build NPC Visual")]
    public static void BuildNPCVisual()
    {
        GameObject npc = GameObject.Find("NPC");
        if (npc == null)
        {
            Debug.LogError("Objet 'NPC' introuvable dans la scène.");
            return;
        }

        Transform oldVisual = npc.transform.Find("NPC_Visual");
        if (oldVisual != null)
        {
            Object.DestroyImmediate(oldVisual.gameObject);
        }

        Material robeMat = CreateOrUpdateMat("NPC_Robe_Mat", new Color(0.22f, 0.45f, 0.28f));
        Material skinMat = CreateOrUpdateMat("NPC_Skin_Mat", new Color(0.86f, 0.78f, 0.68f));
        Material apronMat = CreateOrUpdateMat("NPC_Apron_Mat", new Color(0.62f, 0.48f, 0.30f));
        Material hairMat = CreateOrUpdateMat("NPC_Hair_Mat", new Color(0.16f, 0.10f, 0.07f));

        GameObject root = new GameObject("NPC_Visual");
        root.transform.SetParent(npc.transform);
        root.transform.localPosition = Vector3.zero;
        root.transform.localRotation = Quaternion.identity;

        // Corps
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        body.name = "Body";
        body.transform.SetParent(root.transform);
        body.transform.localPosition = new Vector3(0f, 0.9f, 0f);
        body.transform.localScale = new Vector3(0.7f, 0.9f, 0.55f);
        body.GetComponent<Renderer>().sharedMaterial = robeMat;

        // Tablier
        GameObject apron = GameObject.CreatePrimitive(PrimitiveType.Cube);
        apron.name = "Apron";
        apron.transform.SetParent(root.transform);
        apron.transform.localPosition = new Vector3(0f, 0.85f, 0.24f);
        apron.transform.localScale = new Vector3(0.42f, 0.7f, 0.08f);
        apron.GetComponent<Renderer>().sharedMaterial = apronMat;

        // Tête
        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        head.name = "Head";
        head.transform.SetParent(root.transform);
        head.transform.localPosition = new Vector3(0f, 1.75f, 0f);
        head.transform.localScale = new Vector3(0.42f, 0.42f, 0.42f);
        head.GetComponent<Renderer>().sharedMaterial = skinMat;

        // Cheveux
        GameObject hair = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        hair.name = "Hair";
        hair.transform.SetParent(head.transform);
        hair.transform.localPosition = new Vector3(0f, 0.26f, -0.02f);
        hair.transform.localScale = new Vector3(1.05f, 0.66f, 1.05f);
        hair.GetComponent<Renderer>().sharedMaterial = hairMat;

        // Bras
        CreateLimb(root.transform, "Arm_Left", new Vector3(-0.38f, 1.02f, 0f), new Vector3(0.14f, 0.45f, 0.14f), skinMat);
        CreateLimb(root.transform, "Arm_Right", new Vector3(0.38f, 1.02f, 0f), new Vector3(0.14f, 0.45f, 0.14f), skinMat);

        // Manches
        CreateLimb(root.transform, "Sleeve_Left", new Vector3(-0.30f, 1.08f, 0f), new Vector3(0.20f, 0.22f, 0.20f), robeMat);
        CreateLimb(root.transform, "Sleeve_Right", new Vector3(0.30f, 1.08f, 0f), new Vector3(0.20f, 0.22f, 0.20f), robeMat);

        // Base / pieds fusionnés
        GameObject basePart = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        basePart.name = "Base";
        basePart.transform.SetParent(root.transform);
        basePart.transform.localPosition = new Vector3(0f, 0.08f, 0f);
        basePart.transform.localScale = new Vector3(0.34f, 0.08f, 0.34f);
        basePart.GetComponent<Renderer>().sharedMaterial = robeMat;

        // Yeux : maintenant parentés à la tête pour suivre le regard
        CreateEye(head.transform, "Eye_Left", new Vector3(-0.19f, 0.05f, 0.43f));
        CreateEye(head.transform, "Eye_Right", new Vector3(0.19f, 0.05f, 0.43f));

        // Désactiver le renderer capsule d'origine si présent
        Renderer npcRenderer = npc.GetComponent<Renderer>();
        if (npcRenderer != null)
        {
            npcRenderer.enabled = false;
        }

        AttachLifeController(npc);

        Debug.Log("Visuel NPC généré avec succès.");
    }

    private static void AttachLifeController(GameObject npc)
    {
        NPCSimpleLife life = npc.GetComponent<NPCSimpleLife>();
        if (life == null)
        {
            life = npc.AddComponent<NPCSimpleLife>();
        }

        life.AutoBind();

        EditorUtility.SetDirty(npc);
    }

    private static void CreateLimb(Transform parent, string name, Vector3 localPos, Vector3 localScale, Material mat)
    {
        GameObject limb = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        limb.name = name;
        limb.transform.SetParent(parent);
        limb.transform.localPosition = localPos;
        limb.transform.localScale = localScale;
        limb.GetComponent<Renderer>().sharedMaterial = mat;
    }

    private static void CreateEye(Transform parent, string name, Vector3 localPos)
    {
        GameObject eye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        eye.name = name;
        eye.transform.SetParent(parent);
        eye.transform.localPosition = localPos;
        eye.transform.localScale = new Vector3(0.04f, 0.04f, 0.02f);

        Material eyeMat = CreateOrUpdateMat("NPC_Eye_Mat", new Color(0.05f, 0.05f, 0.05f));
        eye.GetComponent<Renderer>().sharedMaterial = eyeMat;
    }

    private static Material CreateOrUpdateMat(string name, Color color)
    {
        string path = $"Assets/{name}.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);

        if (mat == null)
        {
            mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            AssetDatabase.CreateAsset(mat, path);
        }

        mat.color = color;
        EditorUtility.SetDirty(mat);
        AssetDatabase.SaveAssets();

        return mat;
    }
}