using UnityEngine;
using UnityEditor;

public static class ShopSceneBuilder
{
    [MenuItem("Tools/SmartNPC/Build Shop Prototype")]
    public static void BuildShopPrototype()
    {
        // Nettoyage ancien prototype
        GameObject existing = GameObject.Find("ShopPrototype");
        if (existing != null)
        {
            Object.DestroyImmediate(existing);
        }

        GameObject root = new GameObject("ShopPrototype");

        // Matériaux simples
        Material floorMat = CreateMat("Shop_Floor_Mat", new Color(0.45f, 0.40f, 0.35f));
        Material wallMat = CreateMat("Shop_Wall_Mat", new Color(0.72f, 0.67f, 0.60f));
        Material woodMat = CreateMat("Shop_Wood_Mat", new Color(0.36f, 0.23f, 0.14f));
        Material accentMat = CreateMat("Shop_Accent_Mat", new Color(0.58f, 0.42f, 0.22f));

        // Dimensions
        float roomWidth = 14f;
        float roomDepth = 11f;
        float wallHeight = 4.2f;
        float wallThickness = 0.2f;

        // Sol
        GameObject floor = CreateCube("Floor", new Vector3(0, 0, 0), new Vector3(roomWidth, 0.2f, roomDepth), floorMat, root.transform);

        // Murs
        CreateCube("Wall_Back", new Vector3(0, wallHeight / 2f, roomDepth / 2f), new Vector3(roomWidth, wallHeight, wallThickness), wallMat, root.transform);
        CreateCube("Wall_Front", new Vector3(0, wallHeight / 2f, -roomDepth / 2f), new Vector3(roomWidth, wallHeight, wallThickness), wallMat, root.transform);
        CreateCube("Wall_Left", new Vector3(-roomWidth / 2f, wallHeight / 2f, 0), new Vector3(wallThickness, wallHeight, roomDepth), wallMat, root.transform);
        CreateCube("Wall_Right", new Vector3(roomWidth / 2f, wallHeight / 2f, 0), new Vector3(wallThickness, wallHeight, roomDepth), wallMat, root.transform);

        // Comptoir
        CreateCube("Counter", new Vector3(0, 0.6f, 2.2f), new Vector3(4.2f, 1.2f, 0.9f), woodMat, root.transform);

        // Etagères arrière
        CreateShelf(new Vector3(-4.2f, 0, 4.6f), root.transform, woodMat, accentMat);
        CreateShelf(new Vector3(0f, 0, 4.6f), root.transform, woodMat, accentMat);
        CreateShelf(new Vector3(4.2f, 0, 4.6f), root.transform, woodMat, accentMat);

        // Tables d’appoint
        CreateCube("SideTable_Left", new Vector3(-5.0f, 0.35f, 1.2f), new Vector3(1.2f, 0.7f, 1.2f), woodMat, root.transform);
        CreateCube("SideTable_Right", new Vector3(5.0f, 0.35f, 1.2f), new Vector3(1.2f, 0.7f, 1.2f), woodMat, root.transform);

        // Props simples
        CreatePropCluster(new Vector3(-5.0f, 0.75f, 1.2f), root.transform, accentMat);
        CreatePropCluster(new Vector3(5.0f, 0.75f, 1.2f), root.transform, accentMat);
        CreatePropCluster(new Vector3(0f, 1.25f, 2.2f), root.transform, accentMat);

        // Point d’ancrage NPC
        GameObject npcAnchor = new GameObject("NPC_Anchor");
        npcAnchor.transform.SetParent(root.transform);
        npcAnchor.transform.position = new Vector3(0, 0, 2.6f);

        // Point d’ancrage Player
        GameObject playerAnchor = new GameObject("Player_Anchor");
        playerAnchor.transform.SetParent(root.transform);
        playerAnchor.transform.position = new Vector3(0, 0, -4.3f);

        // Lumières
        SetupLighting(root.transform);

        // Reposition auto des objets existants
        GameObject npc = GameObject.Find("NPC");
        if (npc != null)
        {
            npc.transform.position = npcAnchor.transform.position;
            npc.transform.rotation = Quaternion.Euler(0, 180f, 0);
        }

        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            player.transform.position = playerAnchor.transform.position;
        }

        Selection.activeGameObject = root;
        Debug.Log("Boutique prototype générée avec succès.");
    }

    private static GameObject CreateCube(string name, Vector3 pos, Vector3 scale, Material mat, Transform parent)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.SetParent(parent);
        go.transform.position = pos;
        go.transform.localScale = scale;

        Renderer r = go.GetComponent<Renderer>();
        if (r != null && mat != null)
            r.sharedMaterial = mat;

        return go;
    }

    private static void CreateShelf(Vector3 basePos, Transform parent, Material woodMat, Material accentMat)
    {
        GameObject shelfRoot = new GameObject("Shelf");
        shelfRoot.transform.SetParent(parent);
        shelfRoot.transform.position = basePos;

        CreateCube("Shelf_Left", basePos + new Vector3(-0.7f, 1.2f, 0), new Vector3(0.1f, 2.4f, 0.5f), woodMat, shelfRoot.transform);
        CreateCube("Shelf_Right", basePos + new Vector3(0.7f, 1.2f, 0), new Vector3(0.1f, 2.4f, 0.5f), woodMat, shelfRoot.transform);

        CreateCube("Shelf_Board_1", basePos + new Vector3(0, 0.35f, 0), new Vector3(1.5f, 0.08f, 0.5f), woodMat, shelfRoot.transform);
        CreateCube("Shelf_Board_2", basePos + new Vector3(0, 1.1f, 0), new Vector3(1.5f, 0.08f, 0.5f), woodMat, shelfRoot.transform);
        CreateCube("Shelf_Board_3", basePos + new Vector3(0, 1.85f, 0), new Vector3(1.5f, 0.08f, 0.5f), woodMat, shelfRoot.transform);

        CreatePropCluster(basePos + new Vector3(0, 0.45f, 0), shelfRoot.transform, accentMat);
        CreatePropCluster(basePos + new Vector3(0, 1.2f, 0), shelfRoot.transform, accentMat);
        CreatePropCluster(basePos + new Vector3(0, 1.95f, 0), shelfRoot.transform, accentMat);
    }

    private static void CreatePropCluster(Vector3 center, Transform parent, Material mat)
    {
        GameObject a = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        a.name = "Prop_Cylinder";
        a.transform.SetParent(parent);
        a.transform.position = center + new Vector3(-0.18f, 0, 0);
        a.transform.localScale = new Vector3(0.12f, 0.22f, 0.12f);
        a.GetComponent<Renderer>().sharedMaterial = mat;

        GameObject b = GameObject.CreatePrimitive(PrimitiveType.Cube);
        b.name = "Prop_Box";
        b.transform.SetParent(parent);
        b.transform.position = center + new Vector3(0.12f, 0, 0.04f);
        b.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        b.GetComponent<Renderer>().sharedMaterial = mat;

        GameObject c = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        c.name = "Prop_Sphere";
        c.transform.SetParent(parent);
        c.transform.position = center + new Vector3(0.0f, 0.08f, -0.12f);
        c.transform.localScale = new Vector3(0.16f, 0.16f, 0.16f);
        c.GetComponent<Renderer>().sharedMaterial = mat;
    }

    private static void SetupLighting(Transform parent)
    {
        Light[] existingLights = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
        foreach (Light l in existingLights)
        {
            if (l.type == LightType.Point && l.gameObject.name.StartsWith("ShopLight"))
                Object.DestroyImmediate(l.gameObject);
        }

        GameObject lightA = new GameObject("ShopLight_Warm_Left");
        lightA.transform.SetParent(parent);
        lightA.transform.position = new Vector3(-2.2f, 2.4f, 0.8f);
        Light la = lightA.AddComponent<Light>();
        la.type = LightType.Point;
        la.range = 8f;
        la.intensity = 12f;
        la.color = new Color(1.0f, 0.82f, 0.62f);

        GameObject lightB = new GameObject("ShopLight_Warm_Right");
        lightB.transform.SetParent(parent);
        lightB.transform.position = new Vector3(2.2f, 2.4f, 0.8f);
        Light lb = lightB.AddComponent<Light>();
        lb.type = LightType.Point;
        lb.range = 8f;
        lb.intensity = 10f;
        lb.color = new Color(1.0f, 0.78f, 0.58f);
    }

    private static Material CreateMat(string name, Color color)
    {
        Material existing = AssetDatabase.LoadAssetAtPath<Material>($"Assets/{name}.mat");
        if (existing != null)
            return existing;

        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = color;

        AssetDatabase.CreateAsset(mat, $"Assets/{name}.mat");
        AssetDatabase.SaveAssets();

        return mat;
    }
}