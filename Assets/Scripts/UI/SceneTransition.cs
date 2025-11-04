using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    private static SceneTransition instance;

    [Header("Transition Settings")]
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private CanvasGroup fadeCanvasGroup;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void LoadSceneWithFade(string sceneName, float delay = 0f)
    {
        if (instance != null)
        {
            instance.StartCoroutine(instance.LoadSceneCoroutine(sceneName, delay));
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, float delay)
    {
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        if (fadeCanvasGroup != null)
        {
            float elapsed = 0f;
            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeOutDuration);
                yield return null;
            }
            fadeCanvasGroup.alpha = 1f;
        }

        SceneManager.LoadScene(sceneName);
    }
}
