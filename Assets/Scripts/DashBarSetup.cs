using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DashBarSetup : MonoBehaviour
{
    #if UNITY_EDITOR
    [MenuItem("GameObject/UI/Dash Bar System (Stylized)", false, 13)]
    static void CreateStylizedDashBarSystem()
    {
        Canvas canvas = Selection.activeGameObject?.GetComponent<Canvas>();
        
        if (canvas == null)
        {
            canvas = Object.FindFirstObjectByType<Canvas>();
        }
        
        if (canvas == null)
        {
            Debug.LogError("Please select a Canvas or create one first!");
            return;
        }

        int maxDashes = 2;

        GameObject dashPanel = new GameObject("DashPanel");
        dashPanel.transform.SetParent(canvas.transform, false);
        
        RectTransform panelRect = dashPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 1f);
        panelRect.anchorMax = new Vector2(0.5f, 1f);
        panelRect.pivot = new Vector2(0.5f, 1f);
        panelRect.anchoredPosition = new Vector2(0f, -100f);
        panelRect.sizeDelta = new Vector2(400f, 80f);

        HorizontalLayoutGroup layout = dashPanel.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 10f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        RectTransform[] dashBarFills = new RectTransform[maxDashes];

        for (int i = 0; i < maxDashes; i++)
        {
            GameObject barContainer = new GameObject($"DashBar_{i + 1}");
            barContainer.transform.SetParent(dashPanel.transform, false);
            
            RectTransform containerRect = barContainer.AddComponent<RectTransform>();
            containerRect.sizeDelta = new Vector2(180f, 40f);

            GameObject outerBorder = new GameObject("OuterBorder");
            outerBorder.transform.SetParent(barContainer.transform, false);
            
            RectTransform outerRect = outerBorder.AddComponent<RectTransform>();
            outerRect.anchorMin = new Vector2(0.5f, 0.5f);
            outerRect.anchorMax = new Vector2(0.5f, 0.5f);
            outerRect.pivot = new Vector2(0.5f, 0.5f);
            outerRect.anchoredPosition = Vector2.zero;
            outerRect.sizeDelta = new Vector2(180f, 40f);
            
            Image outerImage = outerBorder.AddComponent<Image>();
            outerImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
            
            SkewImage outerSkew = outerBorder.AddComponent<SkewImage>();
            outerSkew.SkewX = 0.3f;

            GameObject innerBorder = new GameObject("InnerBorder");
            innerBorder.transform.SetParent(outerBorder.transform, false);
            
            RectTransform innerRect = innerBorder.AddComponent<RectTransform>();
            innerRect.anchorMin = new Vector2(0.5f, 0.5f);
            innerRect.anchorMax = new Vector2(0.5f, 0.5f);
            innerRect.pivot = new Vector2(0.5f, 0.5f);
            innerRect.anchoredPosition = Vector2.zero;
            innerRect.sizeDelta = new Vector2(172f, 32f);
            
            Image innerImage = innerBorder.AddComponent<Image>();
            innerImage.color = new Color(0.3f, 0.3f, 0.3f, 0.9f);
            
            SkewImage innerSkew = innerBorder.AddComponent<SkewImage>();
            innerSkew.SkewX = 0.3f;

            GameObject barBackground = new GameObject("BarBackground");
            barBackground.transform.SetParent(innerBorder.transform, false);
            
            RectTransform bgRect = barBackground.AddComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0.5f, 0.5f);
            bgRect.anchorMax = new Vector2(0.5f, 0.5f);
            bgRect.pivot = new Vector2(0.5f, 0.5f);
            bgRect.anchoredPosition = Vector2.zero;
            bgRect.sizeDelta = new Vector2(164f, 24f);
            
            Image bgImage = barBackground.AddComponent<Image>();
            bgImage.color = new Color(0.15f, 0.15f, 0.15f, 0.9f);
            
            SkewImage bgSkew = barBackground.AddComponent<SkewImage>();
            bgSkew.SkewX = 0.3f;

            GameObject barFill = new GameObject("BarFill");
            barFill.transform.SetParent(barBackground.transform, false);
            
            RectTransform fillRect = barFill.AddComponent<RectTransform>();
            fillRect.anchorMin = new Vector2(0f, 0.5f);
            fillRect.anchorMax = new Vector2(0f, 0.5f);
            fillRect.pivot = new Vector2(0f, 0.5f);
            fillRect.anchoredPosition = new Vector2(-82f, 0f);
            fillRect.sizeDelta = new Vector2(164f, 24f);
            
            Image fillImage = barFill.AddComponent<Image>();
            fillImage.color = new Color(0.1f, 0.6f, 0.9f, 0.8f);
            
            SkewImage fillSkew = barFill.AddComponent<SkewImage>();
            fillSkew.SkewX = 0.3f;

            CreateDividerLines(barBackground, 3);

            dashBarFills[i] = fillRect;
        }

        DashUI existingDashUI = canvas.GetComponent<DashUI>();
        if (existingDashUI != null)
        {
            SerializedObject serializedUI = new SerializedObject(existingDashUI);
            SerializedProperty dashBarFillsProperty = serializedUI.FindProperty("dashBarFills");
            dashBarFillsProperty.arraySize = maxDashes;
            
            for (int i = 0; i < maxDashes; i++)
            {
                dashBarFillsProperty.GetArrayElementAtIndex(i).objectReferenceValue = dashBarFills[i];
            }
            
            serializedUI.ApplyModifiedProperties();
            
            Debug.Log("✅ Stylized Dash Bars created and connected to existing DashUI component!");
        }
        else
        {
            Debug.LogWarning("DashUI component not found on Canvas. Please assign references manually.");
        }

        Selection.activeGameObject = dashPanel;
        EditorGUIUtility.PingObject(dashPanel);
    }

    static void CreateDividerLines(GameObject parent, int count)
    {
        RectTransform parentRect = parent.GetComponent<RectTransform>();
        float barWidth = parentRect.sizeDelta.x;
        float spacing = barWidth / count;

        for (int i = 1; i < count; i++)
        {
            GameObject line = new GameObject($"Divider_{i}");
            line.transform.SetParent(parent.transform, false);
            
            RectTransform lineRect = line.AddComponent<RectTransform>();
            lineRect.anchorMin = new Vector2(0.5f, 0.5f);
            lineRect.anchorMax = new Vector2(0.5f, 0.5f);
            lineRect.pivot = new Vector2(0.5f, 0.5f);
            lineRect.anchoredPosition = new Vector2((spacing * i) - (barWidth / 2f), 0f);
            lineRect.sizeDelta = new Vector2(2f, 30f);
            lineRect.localRotation = Quaternion.Euler(0f, 0f, 73f);
            
            Image lineImage = line.AddComponent<Image>();
            lineImage.color = new Color(0.25f, 0.25f, 0.25f, 0.6f);
        }
    }
    #endif
}
