using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    [System.Serializable]
    public class TutorialStep
    {
        public string stepName;
        [TextArea(2, 5)]
        public string message;
        public float displayDuration = 3f;
        public bool waitForInput = false;
        public KeyCode requiredKey = KeyCode.None;
    }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private CanvasGroup tutorialCanvasGroup;
    [SerializeField] private GameObject tutorialPanel;

    [Header("Tutorial Steps")]
    [SerializeField] private List<TutorialStep> tutorialSteps = new List<TutorialStep>();

    [Header("Settings")]
    [SerializeField] private bool autoStart = true;
    [SerializeField] private float delayBeforeStart = 1f;
    [SerializeField] private float fadeSpeed = 3f;

    private int currentStepIndex = -1;
    private float stepTimer = 0f;
    private bool isShowingStep = false;
    private bool isPaused = false;

    private void Start()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }

        if (autoStart)
        {
            Invoke(nameof(StartTutorial), delayBeforeStart);
        }
    }

    public void StartTutorial()
    {
        if (tutorialSteps.Count > 0)
        {
            currentStepIndex = -1;
            ShowNextStep();
        }
    }

    public void ShowNextStep()
    {
        currentStepIndex++;

        if (currentStepIndex >= tutorialSteps.Count)
        {
            EndTutorial();
            return;
        }

        TutorialStep currentStep = tutorialSteps[currentStepIndex];
        ShowStep(currentStep);
    }

    private void ShowStep(TutorialStep step)
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
        }

        if (tutorialText != null)
        {
            tutorialText.text = step.message;
        }

        stepTimer = 0f;
        isShowingStep = true;
        isPaused = step.waitForInput;

        Debug.Log($"Tutorial Step: {step.stepName}");
    }

    private void Update()
    {
        if (!isShowingStep) return;

        TutorialStep currentStep = tutorialSteps[currentStepIndex];

        if (tutorialCanvasGroup != null)
        {
            tutorialCanvasGroup.alpha = Mathf.Lerp(tutorialCanvasGroup.alpha, 1f, Time.deltaTime * fadeSpeed);
        }

        if (isPaused)
        {
            if (currentStep.requiredKey != KeyCode.None && Input.GetKeyDown(currentStep.requiredKey))
            {
                isShowingStep = false;
                HideCurrentStep();
            }
        }
        else
        {
            stepTimer += Time.deltaTime;
            if (stepTimer >= currentStep.displayDuration)
            {
                isShowingStep = false;
                HideCurrentStep();
            }
        }
    }

    private void HideCurrentStep()
    {
        if (tutorialCanvasGroup != null)
        {
            tutorialCanvasGroup.alpha = 0f;
        }

        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }

        Invoke(nameof(ShowNextStep), 0.5f);
    }

    private void EndTutorial()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }

        Debug.Log("Tutorial sequence completed!");
    }

    public void SkipTutorial()
    {
        CancelInvoke();
        isShowingStep = false;
        EndTutorial();
    }
}
