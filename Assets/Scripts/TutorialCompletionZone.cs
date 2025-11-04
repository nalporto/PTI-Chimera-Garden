using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider))]
public class TutorialCompletionZone : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string nextSceneName = "Game";
    
    [Header("Completion Settings")]
    [SerializeField] private bool autoLoadNextScene = true;
    [SerializeField] private float delayBeforeLoad = 1.5f;
    
    [Header("UI Feedback")]
    [SerializeField] private GameObject completionMessage;
    
    [Header("PlayerPrefs")]
    [SerializeField] private bool markTutorialComplete = true;
    private const string TUTORIAL_COMPLETE_KEY = "TutorialCompleted";
    
    private bool hasTriggered = false;

    private void Awake()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        
        if (completionMessage != null)
        {
            completionMessage.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        
        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            OnTutorialCompleted();
        }
    }

    private void OnTutorialCompleted()
    {
        Debug.Log("Tutorial completed!");
        
        if (markTutorialComplete)
        {
            PlayerPrefs.SetInt(TUTORIAL_COMPLETE_KEY, 1);
            PlayerPrefs.Save();
        }
        
        if (completionMessage != null)
        {
            completionMessage.SetActive(true);
        }
        
        if (autoLoadNextScene)
        {
            Invoke(nameof(LoadNextScene), delayBeforeLoad);
        }
    }

    private void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("Next scene name is not set!");
        }
    }

    public static bool IsTutorialCompleted()
    {
        return PlayerPrefs.GetInt(TUTORIAL_COMPLETE_KEY, 0) == 1;
    }

    public static void ResetTutorialCompletion()
    {
        PlayerPrefs.DeleteKey(TUTORIAL_COMPLETE_KEY);
        PlayerPrefs.Save();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(boxCollider.center, boxCollider.size);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
        }
    }
}
