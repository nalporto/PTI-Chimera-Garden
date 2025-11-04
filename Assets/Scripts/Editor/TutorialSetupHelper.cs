using UnityEngine;
using UnityEditor;

public class TutorialSetupHelper : EditorWindow
{
    private string tutorialSceneName = "Tutorial";
    private string gameSceneName = "Game";
    private bool tutorialInBuild = false;
    private bool gameInBuild = false;

    [MenuItem("Tools/Tutorial Setup Helper")]
    public static void ShowWindow()
    {
        GetWindow<TutorialSetupHelper>("Tutorial Helper");
    }

    private void OnGUI()
    {
        GUILayout.Label("Tutorial Scene Setup Helper", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "This tool helps you verify your tutorial setup and provides quick actions.",
            MessageType.Info
        );

        GUILayout.Space(10);

        GUILayout.Label("Scene Names", EditorStyles.boldLabel);
        tutorialSceneName = EditorGUILayout.TextField("Tutorial Scene:", tutorialSceneName);
        gameSceneName = EditorGUILayout.TextField("Game Scene:", gameSceneName);

        GUILayout.Space(10);

        if (GUILayout.Button("Check Build Settings", GUILayout.Height(30)))
        {
            CheckBuildSettings();
        }

        GUILayout.Space(10);

        EditorGUILayout.LabelField("Build Settings Status:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Tutorial Scene:", tutorialInBuild ? "✓ In Build" : "✗ Not Found");
        EditorGUILayout.LabelField("Game Scene:", gameInBuild ? "✓ In Build" : "✗ Not Found");

        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "Make sure both scenes are added to Build Settings!\n" +
            "Go to File → Build Settings and add your scenes.",
            MessageType.Warning
        );

        GUILayout.Space(10);

        if (GUILayout.Button("Open Build Settings", GUILayout.Height(25)))
        {
            EditorApplication.ExecuteMenuItem("File/Build Settings...");
        }

        GUILayout.Space(20);

        GUILayout.Label("Player Progress", EditorStyles.boldLabel);

        if (GUILayout.Button("Reset Tutorial Completion", GUILayout.Height(25)))
        {
            PlayerPrefs.DeleteKey("TutorialCompleted");
            PlayerPrefs.Save();
            Debug.Log("Tutorial completion has been reset!");
            ShowNotification(new GUIContent("Tutorial Reset!"));
        }

        EditorGUILayout.HelpBox(
            "Use this to test the tutorial again after marking it as complete.",
            MessageType.Info
        );

        GUILayout.Space(20);

        GUILayout.Label("Quick Links", EditorStyles.boldLabel);

        if (GUILayout.Button("Open Tutorial Scene", GUILayout.Height(25)))
        {
            OpenScene(tutorialSceneName);
        }

        if (GUILayout.Button("Open Game Scene", GUILayout.Height(25)))
        {
            OpenScene(gameSceneName);
        }

        if (GUILayout.Button("Open Main Menu Scene", GUILayout.Height(25)))
        {
            OpenScene("MainMenu");
        }
    }

    private void CheckBuildSettings()
    {
        tutorialInBuild = IsSceneInBuild(tutorialSceneName);
        gameInBuild = IsSceneInBuild(gameSceneName);

        if (tutorialInBuild && gameInBuild)
        {
            Debug.Log("✓ All scenes are properly configured in Build Settings!");
        }
        else
        {
            Debug.LogWarning("⚠ Some scenes are missing from Build Settings!");
            if (!tutorialInBuild)
            {
                Debug.LogWarning($"  - '{tutorialSceneName}' scene not found");
            }
            if (!gameInBuild)
            {
                Debug.LogWarning($"  - '{gameSceneName}' scene not found");
            }
        }
    }

    private bool IsSceneInBuild(string sceneName)
    {
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.path.Contains(sceneName))
            {
                return true;
            }
        }
        return false;
    }

    private void OpenScene(string sceneName)
    {
        string scenePath = $"Assets/Scenes/{sceneName}.unity";
        
        if (System.IO.File.Exists(scenePath))
        {
            if (UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath);
            }
        }
        else
        {
            Debug.LogError($"Scene not found at path: {scenePath}");
            ShowNotification(new GUIContent("Scene Not Found!"));
        }
    }
}
