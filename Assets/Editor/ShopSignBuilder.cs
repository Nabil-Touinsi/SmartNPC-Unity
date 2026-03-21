using UnityEngine;
using UnityEditor;

public static class ShopSignBuilder
{
    [MenuItem("Tools/SmartNPC/Rebuild Shop Sign")]
    public static void RebuildShopSign()
    {
        GameObject existingSign = GameObject.Find("Shop_Sign");
        if (existingSign != null)
        {
            Object.DestroyImmediate(existingSign);
        }

        Material woodMat = CreateOrUpdateMat("Shop_Sign_Wood_Mat", new Color(0.42f, 0.27f, 0.18f));

        GameObject signRoot = new GameObject("Shop_Sign");
        signRoot.transform.position = new Vector3(0f, 2.75f, 5.35f);
        signRoot.transform.rotation = Quaternion.identity;

        // Board
        GameObject board = GameObject.CreatePrimitive(PrimitiveType.Cube);
        board.name = "Board";
        board.transform.SetParent(signRoot.transform);
        board.transform.localPosition = Vector3.zero;
        board.transform.localRotation = Quaternion.identity;
        board.transform.localScale = new Vector3(3.6f, 0.7f, 0.18f);
        board.GetComponent<Renderer>().sharedMaterial = woodMat;

        // Hangers
        CreateHanger(signRoot.transform, "Hanger_Left", new Vector3(-1.15f, 0.55f, 0f), woodMat);
        CreateHanger(signRoot.transform, "Hanger_Right", new Vector3(1.15f, 0.55f, 0f), woodMat);

        // Text
        CreateLegacy3DText(signRoot.transform);

        EditorUtility.SetDirty(signRoot);
        AssetDatabase.SaveAssets();

        Debug.Log("Nouvelle enseigne générée avec succès.");
    }

    private static void CreateLegacy3DText(Transform parent)
    {
        GameObject textObj = new GameObject("Shop_Sign_Text");
        textObj.transform.SetParent(parent);

        // Côté intérieur boutique, bien devant la planche
        textObj.transform.localPosition = new Vector3(0f, 0f, -0.16f);
        textObj.transform.localRotation = Quaternion.identity;
        textObj.transform.localScale = Vector3.one;

        TextMesh textMesh = textObj.AddComponent<TextMesh>();
        textMesh.text = "SMART NPC SHOP";
        textMesh.fontSize = 120;
        textMesh.characterSize = 0.03f;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.color = new Color(0.97f, 0.93f, 0.84f);

        Font builtInFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (builtInFont == null)
        {
            builtInFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        if (builtInFont != null)
        {
            textMesh.font = builtInFont;

            MeshRenderer renderer = textObj.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = builtInFont.material;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.receiveShadows = false;
            }
        }

        // petit fond de secours derrière le texte pour vérifier visuellement sa zone
        GameObject debugPlate = GameObject.CreatePrimitive(PrimitiveType.Cube);
        debugPlate.name = "Text_Backplate";
        debugPlate.transform.SetParent(parent);
        debugPlate.transform.localPosition = new Vector3(0f, 0f, -0.145f);
        debugPlate.transform.localRotation = Quaternion.identity;
        debugPlate.transform.localScale = new Vector3(3.1f, 0.42f, 0.01f);

        Material debugMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        debugMat.color = new Color(0.30f, 0.20f, 0.14f);
        debugPlate.GetComponent<Renderer>().sharedMaterial = debugMat;
    }

    private static void CreateHanger(Transform parent, string name, Vector3 localPos, Material mat)
    {
        GameObject hanger = GameObject.CreatePrimitive(PrimitiveType.Cube);
        hanger.name = name;
        hanger.transform.SetParent(parent);
        hanger.transform.localPosition = localPos;
        hanger.transform.localRotation = Quaternion.identity;
        hanger.transform.localScale = new Vector3(0.08f, 0.35f, 0.08f);
        hanger.GetComponent<Renderer>().sharedMaterial = mat;
    }

    private static Material CreateOrUpdateMat(string name, Color color)
    {
        string path = $"Assets/{name}.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);

        if (mat == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
                shader = Shader.Find("Standard");

            mat = new Material(shader);
            AssetDatabase.CreateAsset(mat, path);
        }

        mat.color = color;
        EditorUtility.SetDirty(mat);

        return mat;
    }
}