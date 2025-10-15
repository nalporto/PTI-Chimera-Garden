using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DamageVignetteSetup : MonoBehaviour
{
    #if UNITY_EDITOR
    [MenuItem("GameObject/UI/Damage Feedback System", false, 10)]
    static void CreateDamageFeedbackSystem()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("HUD Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            canvasObj.AddComponent<GraphicRaycaster>();
            
            if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
            
            Debug.Log("Created new Canvas for damage feedback system");
        }
        
        GameObject vignetteObj = new GameObject("DamageVignette");
        vignetteObj.transform.SetParent(canvas.transform, false);
        
        RectTransform rectTransform = vignetteObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        
        Image image = vignetteObj.AddComponent<Image>();
        image.color = new Color(1f, 0f, 0f, 0f);
        image.raycastTarget = false;
        
        DamageVignette vignetteScript = vignetteObj.AddComponent<DamageVignette>();
        
        Selection.activeGameObject = vignetteObj;
        
        Debug.Log("Damage Feedback System created successfully! Configure the DamageVignette component and assign it to PlayerHealth.");
        EditorGUIUtility.PingObject(vignetteObj);
    }
    #endif
}
