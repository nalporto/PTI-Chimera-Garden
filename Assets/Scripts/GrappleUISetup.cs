using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GrappleUISetup : MonoBehaviour
{
    #if UNITY_EDITOR
    [MenuItem("GameObject/UI/Grapple Point Indicator System", false, 11)]
    static void CreateGrappleIndicatorSystem()
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
            
            Debug.Log("Created new Canvas for grapple indicator");
        }
        
        GameObject managerObj = new GameObject("GrappleUIManager");
        managerObj.transform.SetParent(canvas.transform, false);
        GrappleUIManager manager = managerObj.AddComponent<GrappleUIManager>();
        
        GameObject indicatorObj = new GameObject("GrappleIndicator");
        indicatorObj.transform.SetParent(managerObj.transform, false);
        
        RectTransform indicatorRect = indicatorObj.AddComponent<RectTransform>();
        indicatorRect.sizeDelta = new Vector2(100, 100);
        
        CreateBrackets(indicatorObj);
        
        SerializedObject serializedManager = new SerializedObject(manager);
        serializedManager.FindProperty("grappleIndicator").objectReferenceValue = indicatorRect;
        serializedManager.FindProperty("mainCamera").objectReferenceValue = Camera.main;
        serializedManager.FindProperty("canvas").objectReferenceValue = canvas;
        serializedManager.ApplyModifiedProperties();
        
        Selection.activeGameObject = managerObj;
        
        Debug.Log("✅ Grapple Indicator System created! Now add GrapplePoint component to your grapple points (e.g., /---MAP---/Sphere)");
        EditorGUIUtility.PingObject(managerObj);
    }
    
    static void CreateBrackets(GameObject parent)
    {
        float bracketSize = 30f;
        float distance = 50f;
        Color bracketColor = Color.white;
        
        CreateBracket(parent, "TopLeft", new Vector2(-distance, distance), 180f, bracketSize, bracketColor);
        CreateBracket(parent, "TopRight", new Vector2(distance, distance), 270f, bracketSize, bracketColor);
        CreateBracket(parent, "BottomRight", new Vector2(distance, -distance), 0f, bracketSize, bracketColor);
        CreateBracket(parent, "BottomLeft", new Vector2(-distance, -distance), 90f, bracketSize, bracketColor);
    }
    
    static void CreateBracket(GameObject parent, string name, Vector2 position, float rotation, float size, Color color)
    {
        GameObject bracket = new GameObject(name);
        bracket.transform.SetParent(parent.transform, false);
        
        RectTransform rect = bracket.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(size, size);
        rect.localRotation = Quaternion.Euler(0f, 0f, rotation);
        
        Image image = bracket.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        
        Texture2D texture = CreateBracketTexture();
        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            100f
        );
        image.sprite = sprite;
    }
    
    static Texture2D CreateBracketTexture()
    {
        int size = 64;
        int thickness = 8;
        int length = 48;
        
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.clear;
        
        for (int x = 0; x < length; x++)
        {
            for (int y = 0; y < thickness; y++)
            {
                pixels[y * size + x] = Color.white;
            }
        }
        
        for (int y = 0; y < length; y++)
        {
            for (int x = 0; x < thickness; x++)
            {
                pixels[y * size + x] = Color.white;
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        texture.filterMode = FilterMode.Point;
        
        return texture;
    }
    #endif
}
