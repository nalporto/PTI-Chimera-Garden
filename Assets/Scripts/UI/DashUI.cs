using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DashUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform[] dashBarFills;
    [SerializeField] private PlayerCharacter playerCharacter;

    [Header("Visual Settings")]
    [SerializeField] private Color fullColor = new Color(0.1f, 0.6f, 0.9f, 0.8f);
    [SerializeField] private Color emptyColor = new Color(0.15f, 0.15f, 0.15f, 0.8f);
    [SerializeField] private Color rechargingColor = new Color(0.5f, 0.8f, 1f, 0.8f);
    [SerializeField] private bool smoothTransition = true;
    [SerializeField] private float transitionSpeed = 5f;

    private Image[] dashBarImages;
    private Vector2[] originalBarSizes;
    private float[] currentFillAmounts;
    private float[] targetFillAmounts;

    void Start()
    {
        if (playerCharacter == null)
        {
            playerCharacter = Object.FindFirstObjectByType<PlayerCharacter>();
        }

        if (dashBarFills != null && dashBarFills.Length > 0)
        {
            dashBarImages = new Image[dashBarFills.Length];
            originalBarSizes = new Vector2[dashBarFills.Length];
            currentFillAmounts = new float[dashBarFills.Length];
            targetFillAmounts = new float[dashBarFills.Length];

            for (int i = 0; i < dashBarFills.Length; i++)
            {
                if (dashBarFills[i] != null)
                {
                    dashBarImages[i] = dashBarFills[i].GetComponent<Image>();
                    originalBarSizes[i] = dashBarFills[i].sizeDelta;
                    currentFillAmounts[i] = 1f;
                    targetFillAmounts[i] = 1f;
                }
            }
        }
    }

    void Update()
    {
        if (playerCharacter != null && dashBarFills != null)
        {
            UpdateDashBars();
        }
    }

    private void UpdateDashBars()
    {
        int currentCharges = playerCharacter.GetDashCharges();
        int maxCharges = playerCharacter.GetMaxDashCharges();
        float rechargeProgress = playerCharacter.GetDashRechargeProgress();

        for (int i = 0; i < dashBarFills.Length && i < maxCharges; i++)
        {
            if (dashBarFills[i] == null) continue;

            if (i < currentCharges)
            {
                targetFillAmounts[i] = 1f;
                if (dashBarImages[i] != null)
                    dashBarImages[i].color = fullColor;
            }
            else if (i == currentCharges)
            {
                targetFillAmounts[i] = rechargeProgress;
                if (dashBarImages[i] != null)
                    dashBarImages[i].color = rechargingColor;
            }
            else
            {
                targetFillAmounts[i] = 0f;
                if (dashBarImages[i] != null)
                    dashBarImages[i].color = emptyColor;
            }

            if (smoothTransition)
            {
                currentFillAmounts[i] = Mathf.Lerp(currentFillAmounts[i], targetFillAmounts[i], Time.deltaTime * transitionSpeed);
            }
            else
            {
                currentFillAmounts[i] = targetFillAmounts[i];
            }

            Vector2 newSize = originalBarSizes[i];
            newSize.x = originalBarSizes[i].x * currentFillAmounts[i];
            dashBarFills[i].sizeDelta = newSize;
        }
    }
}
