using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenuButton : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private KeyCode returnToMenuKey = KeyCode.Escape;
    [SerializeField] private bool showConfirmationPanel = true;

    [Header("UI References")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject confirmationPanel;

    private bool isPaused = false;

    private void Start()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(returnToMenuKey))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        isPaused = !isPaused;

        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(isPaused);
        }

        Time.timeScale = isPaused ? 0f : 1f;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;
    }

    public void OnReturnToMenuClicked()
    {
        if (showConfirmationPanel && confirmationPanel != null)
        {
            confirmationPanel.SetActive(true);
        }
        else
        {
            ReturnToMainMenu();
        }
    }

    public void OnConfirmReturnToMenu()
    {
        ReturnToMainMenu();
    }

    public void OnCancelReturnToMenu()
    {
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false);
        }
    }

    public void OnResumeClicked()
    {
        TogglePauseMenu();
    }

    private void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            Debug.LogError("Main menu scene name is not set!");
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
