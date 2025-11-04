using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject configPanel;
    [SerializeField] private GameObject sceneSelectionPanel;

    [Header("Scene Names")]
    [SerializeField] private string tutorialSceneName = "Tutorial";
    [SerializeField] private string gameSceneName = "Game";

    [Header("Tutorial Settings")]
    [SerializeField] private bool autoSkipTutorialIfCompleted = true;

    private void Start()
    {
        ShowMainMenu();
    }

    public void OnPlayButtonClicked()
    {
        if (autoSkipTutorialIfCompleted && TutorialCompletionZone.IsTutorialCompleted())
        {
            LoadScene(gameSceneName);
        }
        else
        {
            ShowSceneSelectionPanel();
        }
    }

    public void OnStartTutorialClicked()
    {
        LoadScene(tutorialSceneName);
    }

    public void OnSkipToLevelClicked()
    {
        LoadScene(gameSceneName);
    }

    public void OnConfigButtonClicked()
    {
        ShowConfigPanel();
    }

    public void OnBackToMainMenuClicked()
    {
        ShowMainMenu();
    }

    public void OnExitButtonClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void ShowMainMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (configPanel != null) configPanel.SetActive(false);
        if (sceneSelectionPanel != null) sceneSelectionPanel.SetActive(false);
    }

    private void ShowConfigPanel()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (configPanel != null) configPanel.SetActive(true);
        if (sceneSelectionPanel != null) sceneSelectionPanel.SetActive(false);
    }

    private void ShowSceneSelectionPanel()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (configPanel != null) configPanel.SetActive(false);
        if (sceneSelectionPanel != null) sceneSelectionPanel.SetActive(true);
    }

    private void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene name is empty or null!");
        }
    }
}
