using UnityEngine;
using UnityEditor;

public static class ShopPolishBuilder
{
    [MenuItem("Tools/SmartNPC/Polish Shop Prototype")]
    public static void PolishShopPrototype()
    {
        GameObject root = GameObject.Find("ShopPrototype");
        if (root == null)
        {
            Debug.LogError("ShopPrototype introuvable. Lance d'abord : Tools > SmartNPC > Build Shop Prototype");
            return;
        }

        // Matériaux plus doux
        Material softFloor = CreateOrUpdateMat("Shop_Floor_Mat", new Color(0.63f, 0.57f, 0.50f));
        Material softWall = CreateOrUpdateMat("Shop_Wall_Mat", new Color(0.86f, 0.80f, 0.72f));
        Material softWood = CreateOrUpdateMat("Shop_Wood_Mat", new Color(0.55f, 0.35f, 0.22f));
        Material softAccent = CreateOrUpdateMat("Shop_Accent_Mat", new Color(0.78f, 0.60f, 0.30f));
        Material rugMat = CreateOrUpdateMat("Shop_Rug_Mat", new Color(0.41f, 0.22f, 0.15f));
        Material signMat = CreateOrUpdateMat("Shop_Sign_Mat", new Color(0.30f, 0.18f, 0.10f));
        Material lightMat = CreateOrUpdateMat("Shop_Light_Mat", new Color(0.95f, 0.75f, 0.45f), true);

        ApplyMaterialIfFound(root, "Floor", softFloor);
        ApplyMaterialStartsWith(root, "Wall_", softWall);
        ApplyMaterialIfFound(root, "Counter", softWood);
        ApplyMaterialStartsWith(root, "SideTable_", softWood);
        ApplyMaterialStartsWith(root, "Shelf_", softWood);
        ApplyMaterialStartsWith(root, "Prop_", softAccent);

        CreateOrReplaceRug(root.transform, rugMat);
        CreateOrReplaceSign(root.transform, signMat);
        CreateOrReplaceCeilingBeams(root.transform, softWood);
        CreateOrReplaceHangingLamps(root.transform, softWood, lightMat);
        CreateOrReplaceNpcSpot(root.transform, rugMat);

        ImproveLighting(root.transform);
        FrameMainCamera();

        Debug.Log("Polish visuel appliqué avec succès.");
    }

    private static void CreateOrReplaceRug(Transform parent, Material rugMat)
    {
        DestroyChildIfExists(parent, "Shop_Rug");
        GameObject rug = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rug.name = "Shop_Rug";
        rug.transform.SetParent(parent);
        rug.transform.position = new Vector3(0f, 0.11f, 0.6f);
        rug.transform.localScale = new Vector3(5.6f, 0.02f, 3.6f);
        rug.GetComponent<Renderer>().sharedMaterial = rugMat;
    }

    private static void CreateOrReplaceSign(Transform parent, Material signMat)
    {
        DestroyChildIfExists(parent, "Shop_Sign");

        GameObject signRoot = new GameObject("Shop_Sign");
        signRoot.transform.SetParent(parent);

        GameObject board = GameObject.CreatePrimitive(PrimitiveType.Cube);
        board.name = "Board";
        board.transform.SetParent(signRoot.transform);
        board.transform.position = new Vector3(0f, 3.15f, 5.35f);
        board.transform.localScale = new Vector3(2.6f, 0.6f, 0.08f);
        board.GetComponent<Renderer>().sharedMaterial = signMat;

#if UNITY_EDITOR
        GameObject text = new GameObject("Title");
        text.transform.SetParent(signRoot.transform);
        text.transform.position = new Vector3(0f, 3.16f, 5.30f);

        TextMesh tm = text.AddComponent<TextMesh>();
        tm.text = "SMART NPC SHOP";
        tm.fontSize = 48;
        tm.characterSize = 0.08f;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = new Color(0.95f, 0.86f, 0.68f);

        text.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
#endif
    }

    private static void CreateOrReplaceCeilingBeams(Transform parent, Material beamMat)
    {
        DestroyChildIfExists(parent, "Ceiling_Beams");
        GameObject root = new GameObject("Ceiling_Beams");
        root.transform.SetParent(parent);

        for (int i = -1; i <= 1; i++)
        {
            GameObject beam = GameObject.CreatePrimitive(PrimitiveType.Cube);
            beam.name = "Beam_" + i;
            beam.transform.SetParent(root.transform);
            beam.transform.position = new Vector3(i * 3.8f, 4.0f, 0f);
            beam.transform.localScale = new Vector3(0.18f, 0.18f, 10.6f);
            beam.GetComponent<Renderer>().sharedMaterial = beamMat;
        }
    }

    private static void CreateOrReplaceHangingLamps(Transform parent, Material supportMat, Material bulbMat)
    {
        DestroyChildIfExists(parent, "Hanging_Lamps");
        GameObject root = new GameObject("Hanging_Lamps");
        root.transform.SetParent(parent);

        CreateLamp(new Vector3(-2.8f, 3.5f, 1.2f), root.transform, supportMat, bulbMat, "Lamp_Left");
        CreateLamp(new Vector3(2.8f, 3.5f, 1.2f), root.transform, supportMat, bulbMat, "Lamp_Right");
    }

    private static void CreateLamp(Vector3 pos, Transform parent, Material supportMat, Material bulbMat, string name)
    {
        GameObject lampRoot = new GameObject(name);
        lampRoot.transform.SetParent(parent);

        GameObject rod = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rod.name = "Rod";
        rod.transform.SetParent(lampRoot.transform);
        rod.transform.position = pos + new Vector3(0, 0.2f, 0);
        rod.transform.localScale = new Vector3(0.03f, 0.25f, 0.03f);
        rod.GetComponent<Renderer>().sharedMaterial = supportMat;

        GameObject shade = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        shade.name = "Shade";
        shade.transform.SetParent(lampRoot.transform);
        shade.transform.position = pos;
        shade.transform.localScale = new Vector3(0.35f, 0.2f, 0.35f);
        shade.GetComponent<Renderer>().sharedMaterial = bulbMat;
    }

    private static void CreateOrReplaceNpcSpot(Transform parent, Material mat)
    {
        DestroyChildIfExists(parent, "NPC_Spot");

        GameObject spot = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        spot.name = "NPC_Spot";
        spot.transform.SetParent(parent);
        spot.transform.position = new Vector3(0f, 0.02f, 2.6f);
        spot.transform.localScale = new Vector3(0.8f, 0.02f, 0.8f);
        spot.GetComponent<Renderer>().sharedMaterial = mat;
    }

    private static void ImproveLighting(Transform parent)
    {
        Light[] lights = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
        foreach (Light l in lights)
        {
            if (l.gameObject.name == "Directional Light")
            {
                l.intensity = 0.8f;
                l.color = new Color(1f, 0.95f, 0.88f);
                l.shadows = LightShadows.Soft;
            }

            if (l.gameObject.name == "ShopLight_Warm_Left")
            {
                l.intensity = 16f;
                l.range = 9f;
                l.color = new Color(1.0f, 0.80f, 0.62f);
            }

            if (l.gameObject.name == "ShopLight_Warm_Right")
            {
                l.intensity = 14f;
                l.range = 9f;
                l.color = new Color(1.0f, 0.78f, 0.60f);
            }
        }

        DestroyImmediateIfFound("NPC_KeyLight");
        GameObject key = new GameObject("NPC_KeyLight");
        key.transform.SetParent(parent);
        key.transform.position = new Vector3(0f, 2.2f, 1.3f);

        Light k = key.AddComponent<Light>();
        k.type = LightType.Spot;
        k.range = 8f;
        k.intensity = 18f;
        k.spotAngle = 55f;
        k.color = new Color(1.0f, 0.84f, 0.68f);
        key.transform.rotation = Quaternion.Euler(55f, 0f, 0f);
    }

    private static void FrameMainCamera()
    {
        GameObject cam = GameObject.Find("Main Camera");
        if (cam == null) return;

        cam.transform.position = new Vector3(0f, 2.1f, -5.8f);
        cam.transform.rotation = Quaternion.Euler(14f, 0f, 0f);
    }

    private static void ApplyMaterialIfFound(GameObject root, string childName, Material mat)
    {
        Transform t = root.transform.Find(childName);
        if (t != null)
        {
            Renderer r = t.GetComponent<Renderer>();
            if (r != null) r.sharedMaterial = mat;
        }
    }

    private static void ApplyMaterialStartsWith(GameObject root, string prefix, Material mat)
    {
        Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer r in renderers)
        {
            if (r.gameObject.name.StartsWith(prefix))
                r.sharedMaterial = mat;
        }
    }

    private static void DestroyChildIfExists(Transform parent, string name)
    {
        Transform child = parent.Find(name);
        if (child != null)
            Object.DestroyImmediate(child.gameObject);
    }

    private static void DestroyImmediateIfFound(string objectName)
    {
        GameObject go = GameObject.Find(objectName);
        if (go != null)
            Object.DestroyImmediate(go);
    }

    private static Material CreateOrUpdateMat(string name, Color color, bool emission = false)
    {
        string path = $"Assets/{name}.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);

        if (mat == null)
        {
            mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            AssetDatabase.CreateAsset(mat, path);
        }

        mat.color = color;

        if (emission)
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", color * 1.5f);
        }

        EditorUtility.SetDirty(mat);
        AssetDatabase.SaveAssets();
        return mat;
    }
}   