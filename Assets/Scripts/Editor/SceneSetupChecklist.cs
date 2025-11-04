using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class SceneSetupChecklist : EditorWindow
{
    private Vector2 scrollPosition;
    private bool[] checklistItems = new bool[15];

    [MenuItem("Tools/Scene Setup Checklist")]
    public static void ShowWindow()
    {
        GetWindow<SceneSetupChecklist>("Setup Checklist");
    }

    private void OnGUI()
    {
        GUILayout.Label("Tutorial System Setup Checklist", EditorStyles.boldLabel);
        
        EditorGUILayout.HelpBox(
            "Check off each item as you complete it. This helps ensure you don't miss any setup steps!",
            MessageType.Info
        );

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        GUILayout.Space(10);
        GUILayout.Label("SCENE SETUP", EditorStyles.boldLabel);
        GUILayout.Space(5);

        checklistItems[0] = EditorGUILayout.ToggleLeft("✓ Duplicated Game.unity to create Tutorial.unity", checklistItems[0]);
        checklistItems[1] = EditorGUILayout.ToggleLeft("✓ Removed FirstLevel_Map from Tutorial scene", checklistItems[1]);
        checklistItems[2] = EditorGUILayout.ToggleLeft("✓ Removed TutorialMAP from Game scene", checklistItems[2]);
        checklistItems[3] = EditorGUILayout.ToggleLeft("✓ Added all scenes to Build Settings", checklistItems[3]);

        GUILayout.Space(10);
        GUILayout.Label("MAIN MENU UI", EditorStyles.boldLabel);
        GUILayout.Space(5);

        checklistItems[4] = EditorGUILayout.ToggleLeft("✓ Created SceneSelectionPanel in MainMenu", checklistItems[4]);
        checklistItems[5] = EditorGUILayout.ToggleLeft("✓ Added TutorialButton (Start Tutorial)", checklistItems[5]);
        checklistItems[6] = EditorGUILayout.ToggleLeft("✓ Added SkipButton (Skip to Level 1)", checklistItems[6]);
        checklistItems[7] = EditorGUILayout.ToggleLeft("✓ Added BackButton", checklistItems[7]);
        checklistItems[8] = EditorGUILayout.ToggleLeft("✓ Connected all button events to MenuManager", checklistItems[8]);
        checklistItems[9] = EditorGUILayout.ToggleLeft("✓ Set SceneSelectionPanel inactive by default", checklistItems[9]);

        GUILayout.Space(10);
        GUILayout.Label("TUTORIAL SCENE", EditorStyles.boldLabel);
        GUILayout.Space(5);

        checklistItems[10] = EditorGUILayout.ToggleLeft("✓ Added TutorialCompletionZone at tutorial exit", checklistItems[10]);
        checklistItems[11] = EditorGUILayout.ToggleLeft("✓ Created tutorial prompt UI (optional)", checklistItems[11]);
        checklistItems[12] = EditorGUILayout.ToggleLeft("✓ Added tutorial prompt triggers (optional)", checklistItems[12]);

        GUILayout.Space(10);
        GUILayout.Label("TESTING", EditorStyles.boldLabel);
        GUILayout.Space(5);

        checklistItems[13] = EditorGUILayout.ToggleLeft("✓ Tested: MainMenu → Tutorial → Game flow", checklistItems[13]);
        checklistItems[14] = EditorGUILayout.ToggleLeft("✓ Tested: Tutorial auto-skip after completion", checklistItems[14]);

        GUILayout.Space(20);

        int completedCount = 0;
        foreach (bool item in checklistItems)
        {
            if (item) completedCount++;
        }

        float progress = (float)completedCount / checklistItems.Length;
        
        EditorGUILayout.LabelField("Progress:", $"{completedCount}/{checklistItems.Length} ({progress * 100:F0}%)");
        
        Rect progressRect = GUILayoutUtility.GetRect(18, 18, GUILayout.ExpandWidth(true));
        EditorGUI.ProgressBar(progressRect, progress, $"{progress * 100:F0}% Complete");

        GUILayout.Space(10);

        if (progress >= 1f)
        {
            EditorGUILayout.HelpBox("🎉 Setup Complete! Your tutorial system is ready!", MessageType.Info);
        }
        else if (progress >= 0.5f)
        {
            EditorGUILayout.HelpBox("You're halfway there! Keep going!", MessageType.Warning);
        }
        else
        {
            EditorGUILayout.HelpBox("Check the documentation guides to get started.", MessageType.Warning);
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Reset Checklist", GUILayout.Height(25)))
        {
            for (int i = 0; i < checklistItems.Length; i++)
            {
                checklistItems[i] = false;
            }
        }

        if (GUILayout.Button("Open Documentation", GUILayout.Height(25)))
        {
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>("Assets/TUTORIAL_SYSTEM_COMPLETE_OVERVIEW.txt");
            EditorGUIUtility.PingObject(Selection.activeObject);
        }

        EditorGUILayout.EndScrollView();
    }
}
