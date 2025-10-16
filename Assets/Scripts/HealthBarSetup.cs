using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class HealthBarSetup : MonoBehaviour
{
    #if UNITY_EDITOR
    [MenuItem("GameObject/UI/Health Bar System (Stylized)", false, 12)]
    static void CreateStylizedHealthBarSystem()
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

        GameObject healthPanel = new GameObject("HealthPanel");
        healthPanel.transform.SetParent(canvas.transform, false);
        
        RectTransform panelRect = healthPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0f, 1f);
        panelRect.anchorMax = new Vector2(0f, 1f);
        panelRect.pivot = new Vector2(0f, 1f);
        panelRect.anchoredPosition = new Vector2(30f, -30f);
        panelRect.sizeDelta = new Vector2(400f, 60f);

        GameObject barContainer = new GameObject("BarContainer");
        barContainer.transform.SetParent(healthPanel.transform, false);
        
        RectTransform containerRect = barContainer.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0f, 0f);
        containerRect.anchorMax = new Vector2(1f, 1f);
        containerRect.pivot = new Vector2(0f, 0.5f);
        containerRect.anchoredPosition = Vector2.zero;
        containerRect.sizeDelta = Vector2.zero;

        GameObject outerBorder = new GameObject("OuterBorder");
        outerBorder.transform.SetParent(barContainer.transform, false);
        
        RectTransform outerRect = outerBorder.AddComponent<RectTransform>();
        outerRect.anchorMin = new Vector2(0f, 0.5f);
        outerRect.anchorMax = new Vector2(0f, 0.5f);
        outerRect.pivot = new Vector2(0f, 0.5f);
        outerRect.anchoredPosition = Vector2.zero;
        outerRect.sizeDelta = new Vector2(400f, 60f);
        
        Image outerImage = outerBorder.AddComponent<Image>();
        outerImage.color = new Color(0.1f, 0.1f, 0.1f, 1f);
        
        SkewImage outerSkew = outerBorder.AddComponent<SkewImage>();
        outerSkew.SkewX = 0.3f;

        GameObject innerBorder = new GameObject("InnerBorder");
        innerBorder.transform.SetParent(outerBorder.transform, false);
        
        RectTransform innerBorderRect = innerBorder.AddComponent<RectTransform>();
        innerBorderRect.anchorMin = new Vector2(0f, 0.5f);
        innerBorderRect.anchorMax = new Vector2(0f, 0.5f);
        innerBorderRect.pivot = new Vector2(0f, 0.5f);
        innerBorderRect.anchoredPosition = new Vector2(6f, 0f);
        innerBorderRect.sizeDelta = new Vector2(388f, 48f);
        
        Image innerBorderImage = innerBorder.AddComponent<Image>();
        innerBorderImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        SkewImage innerSkew = innerBorder.AddComponent<SkewImage>();
        innerSkew.SkewX = 0.3f;

        GameObject barBackground = new GameObject("HealthBarBackground");
        barBackground.transform.SetParent(innerBorder.transform, false);
        
        RectTransform bgRect = barBackground.AddComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0f, 0.5f);
        bgRect.anchorMax = new Vector2(0f, 0.5f);
        bgRect.pivot = new Vector2(0f, 0.5f);
        bgRect.anchoredPosition = new Vector2(4f, 0f);
        bgRect.sizeDelta = new Vector2(380f, 40f);
        
        Image bgImage = barBackground.AddComponent<Image>();
        bgImage.color = new Color(0.15f, 0.15f, 0.15f, 1f);
        
        SkewImage bgSkew = barBackground.AddComponent<SkewImage>();
        bgSkew.SkewX = 0.3f;

        GameObject barFill = new GameObject("HealthBarFill");
        barFill.transform.SetParent(barBackground.transform, false);
        
        RectTransform fillRect = barFill.AddComponent<RectTransform>();
        fillRect.anchorMin = new Vector2(0f, 0.5f);
        fillRect.anchorMax = new Vector2(0f, 0.5f);
        fillRect.pivot = new Vector2(0f, 0.5f);
        fillRect.anchoredPosition = Vector2.zero;
        fillRect.sizeDelta = new Vector2(380f, 40f);
        
        Image fillImage = barFill.AddComponent<Image>();
        fillImage.color = new Color(0.9f, 0.1f, 0.1f, 1f);
        
        SkewImage fillSkew = barFill.AddComponent<SkewImage>();
        fillSkew.SkewX = 0.3f;

        CreateDividerLines(barBackground, 5);

        GameObject healthText = new GameObject("HealthText");
        healthText.transform.SetParent(barBackground.transform, false);
        
        RectTransform textRect = healthText.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0f, 0f);
        textRect.anchorMax = new Vector2(1f, 1f);
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.anchoredPosition = new Vector2(20f, 0f);
        textRect.sizeDelta = Vector2.zero;
        
        TextMeshProUGUI tmpText = healthText.AddComponent<TextMeshProUGUI>();
        tmpText.text = "100%";
        tmpText.fontSize = 32;
        tmpText.fontStyle = FontStyles.Bold;
        tmpText.color = Color.white;
        tmpText.alignment = TextAlignmentOptions.Left;
        tmpText.enableWordWrapping = false;

        HealthUI existingHealthUI = canvas.GetComponent<HealthUI>();
        if (existingHealthUI != null)
        {
            SerializedObject serializedUI = new SerializedObject(existingHealthUI);
            serializedUI.FindProperty("healthText").objectReferenceValue = tmpText;
            serializedUI.FindProperty("healthBarFill").objectReferenceValue = fillRect;
            serializedUI.ApplyModifiedProperties();
            
            Debug.Log("✅ Stylized Health Bar created and connected to existing HealthUI component!");
        }
        else
        {
            Debug.LogWarning("HealthUI component not found on Canvas. Please assign references manually.");
        }

        Selection.activeGameObject = healthPanel;
        EditorGUIUtility.PingObject(healthPanel);
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
            lineRect.anchorMin = new Vector2(0f, 0.5f);
            lineRect.anchorMax = new Vector2(0f, 0.5f);
            lineRect.pivot = new Vector2(0.5f, 0.5f);
            lineRect.anchoredPosition = new Vector2(spacing * i, 0f);
            lineRect.sizeDelta = new Vector2(3f, 50f);
            lineRect.localRotation = Quaternion.Euler(0f, 0f, 73f);
            
            Image lineImage = line.AddComponent<Image>();
            lineImage.color = new Color(0.25f, 0.25f, 0.25f, 0.8f);
        }
    }
    #endif
}
