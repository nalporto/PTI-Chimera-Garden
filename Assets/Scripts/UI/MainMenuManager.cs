using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject configPanel;
    
    [Header("Settings")]
    [SerializeField] private string gameSceneName = "Game";
    
    void Start()
    {
        ShowMainMenu();
    }
    
    public void OnPlayButton()
    {
        Debug.Log("[MainMenu] Loading game scene: " + gameSceneName);
        SceneManager.LoadScene(gameSceneName);
    }
    
    public void OnConfigButton()
    {
        Debug.Log("[MainMenu] Opening config panel");
        mainMenuPanel.SetActive(false);
        configPanel.SetActive(true);
    }
    
    public void OnExitButton()
    {
        Debug.Log("[MainMenu] Exiting game");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    public void OnBackButton()
    {
        Debug.Log("[MainMenu] Returning to main menu");
        ShowMainMenu();
    }
    
    private void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        configPanel.SetActive(false);
    }
}
