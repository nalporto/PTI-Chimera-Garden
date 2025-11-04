using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSelectionValidator : MonoBehaviour
{
    [Header("Scene Validation")]
    [SerializeField] private string tutorialSceneName = "Tutorial";
    [SerializeField] private string gameSceneName = "Game";

    [ContextMenu("Validate Scene Setup")]
    private void ValidateSceneSetup()
    {
        Debug.Log("═══════════════════════════════════════════════════");
        Debug.Log("SCENE SELECTION SETUP VALIDATION");
        Debug.Log("═══════════════════════════════════════════════════");

        bool allValid = true;

        if (IsTutorialSceneInBuild())
        {
            Debug.Log($"✓ Tutorial scene '{tutorialSceneName}' is in Build Settings");
        }
        else
        {
            Debug.LogError($"✗ Tutorial scene '{tutorialSceneName}' NOT found in Build Settings!");
            allValid = false;
        }

        if (IsGameSceneInBuild())
        {
            Debug.Log($"✓ Game scene '{gameSceneName}' is in Build Settings");
        }
        else
        {
            Debug.LogError($"✗ Game scene '{gameSceneName}' NOT found in Build Settings!");
            allValid = false;
        }

        Debug.Log("───────────────────────────────────────────────────");
        Debug.Log($"Build Settings contains {SceneManager.sceneCountInBuildSettings} scene(s):");
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            Debug.Log($"  [{i}] {sceneName} ({scenePath})");
        }

        Debug.Log("───────────────────────────────────────────────────");

        if (allValid)
        {
            Debug.Log("✅ ALL VALIDATIONS PASSED!");
        }
        else
        {
            Debug.LogWarning("⚠️ Some validations failed. Please check Build Settings.");
        }

        Debug.Log("═══════════════════════════════════════════════════");
    }

    private bool IsTutorialSceneInBuild()
    {
        return IsSceneInBuild(tutorialSceneName);
    }

    private bool IsGameSceneInBuild()
    {
        return IsSceneInBuild(gameSceneName);
    }

    private bool IsSceneInBuild(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameInBuild = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            if (sceneNameInBuild == sceneName)
            {
                return true;
            }
        }
        return false;
    }

    [ContextMenu("Print Current Scene Info")]
    private void PrintCurrentSceneInfo()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        Debug.Log($"Current Scene: {currentScene.name}");
        Debug.Log($"Scene Path: {currentScene.path}");
        Debug.Log($"Root GameObject Count: {currentScene.rootCount}");
    }
}
