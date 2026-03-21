using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ShopPropsVariationBuilder
{
    [MenuItem("Tools/SmartNPC/Apply Props Variation++")]
    public static void ApplyVariationPlusPlus()
    {
        Random.InitState(System.DateTime.Now.Ticks.GetHashCode());

        int shelvesUpdated = 0;
        int propsUpdated = 0;

        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject obj in allObjects)
        {
            if (obj == null)
                continue;

            // On ne traite QUE les vraies étagères
            if (!IsShelfRoot(obj.transform))
                continue;

            List<Transform> directProps = GetDirectPropChildren(obj.transform);
            if (directProps.Count == 0)
                continue;

            List<Transform> boards = GetShelfBoards(obj.transform);
            if (boards.Count == 0)
                continue;

            LayoutPropsOnShelf(obj.transform, boards, directProps);

            shelvesUpdated++;
            propsUpdated += directProps.Count;
            EditorUtility.SetDirty(obj);
        }

        Debug.Log($"Props Variation++ appliquée aux étagères. Shelves: {shelvesUpdated}, Props: {propsUpdated}");
    }

    private static bool IsShelfRoot(Transform t)
    {
        if (t == null)
            return false;

        // On cible les objets du type Shelf, Shelf_Left, Shelf_Right
        if (t.name == "Shelf")
            return true;

        if (t.name.StartsWith("Shelf_"))
            return true;

        return false;
    }

    private static List<Transform> GetDirectPropChildren(Transform root)
    {
        List<Transform> props = new List<Transform>();

        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            if (child.name.StartsWith("Prop_"))
            {
                props.Add(child);
            }
        }

        return props;
    }

    private static List<Transform> GetShelfBoards(Transform root)
    {
        List<Transform> boards = new List<Transform>();

        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            if (child.name.StartsWith("Shelf_Board"))
            {
                boards.Add(child);
            }
        }

        boards.Sort((a, b) => b.localPosition.y.CompareTo(a.localPosition.y));
        return boards;
    }

    private static void LayoutPropsOnShelf(Transform shelfRoot, List<Transform> boards, List<Transform> props)
    {
        List<Transform>[] propsPerBoard = new List<Transform>[boards.Count];
        for (int i = 0; i < boards.Count; i++)
            propsPerBoard[i] = new List<Transform>();

        for (int i = 0; i < props.Count; i++)
        {
            int boardIndex = i % boards.Count;
            propsPerBoard[boardIndex].Add(props[i]);
        }

        for (int b = 0; b < boards.Count; b++)
        {
            Transform board = boards[b];
            List<Transform> boardProps = propsPerBoard[b];

            if (boardProps.Count == 0)
                continue;

            float boardWidth = GetVisualWidth(board, 1.2f);
            float usableWidth = Mathf.Max(0.35f, boardWidth * 0.72f);
            float startX = -usableWidth * 0.5f;
            float step = boardProps.Count == 1 ? 0f : usableWidth / (boardProps.Count - 1);

            for (int i = 0; i < boardProps.Count; i++)
            {
                Transform prop = boardProps[i];

                float x = board.localPosition.x + startX + (step * i) + Random.Range(-0.03f, 0.03f);
                float y = board.localPosition.y + 0.16f + Random.Range(-0.005f, 0.015f);
                float z = board.localPosition.z + Random.Range(-0.02f, 0.02f);

                prop.localPosition = new Vector3(x, y, z);
                prop.localRotation = Quaternion.Euler(0f, Random.Range(-18f, 18f), 0f);

                float uniformScale = Random.Range(0.9f, 1.08f);
                prop.localScale = GetBaseScaleByType(prop.name) * uniformScale;
            }
        }
    }

    private static Vector3 GetBaseScaleByType(string propName)
    {
        if (propName.Contains("Cylinder"))
            return new Vector3(0.16f, 0.22f, 0.16f);

        if (propName.Contains("Sphere"))
            return new Vector3(0.18f, 0.18f, 0.18f);

        return new Vector3(0.22f, 0.18f, 0.18f);
    }

    private static float GetVisualWidth(Transform t, float fallback)
    {
        Renderer r = t.GetComponent<Renderer>();
        if (r != null)
            return r.bounds.size.x;

        return fallback;
    }
}