using UnityEngine;
using UnityEngine.UI;

public class GrappleUIManager : MonoBehaviour
{
    public static GrappleUIManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private RectTransform grappleIndicator;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Canvas canvas;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float scaleMultiplier = 1f;
    [SerializeField] private float minScale = 0.8f;
    [SerializeField] private float maxScale = 1.2f;
    [SerializeField] private float scalePulseSpeed = 2f;

    [Header("Positioning")]
    [SerializeField] private float offsetFromPoint = 50f;

    private GrapplePoint currentGrapplePoint;
    private bool isUIActive = false;
    private float rotationAngle = 0f;
    private float scaleTimer = 0f;

    public GrapplePoint CurrentGrapplePoint => currentGrapplePoint;
    public bool IsUIActive => isUIActive;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        if (grappleIndicator != null)
            grappleIndicator.gameObject.SetActive(false);
        else
            Debug.LogError("GrappleUIManager: Grapple Indicator UI not assigned!");
    }

    void Update()
    {
        if (isUIActive && currentGrapplePoint != null && grappleIndicator != null)
        {
            UpdateUIPosition();
            UpdateRotation();
            UpdateScale();
        }
    }

    public void ShowGrappleUI(GrapplePoint grapplePoint)
    {
        currentGrapplePoint = grapplePoint;
        isUIActive = true;
        
        if (grappleIndicator != null)
        {
            grappleIndicator.gameObject.SetActive(true);
            rotationAngle = 0f;
            scaleTimer = 0f;
        }
    }

    public void HideGrappleUI(GrapplePoint grapplePoint)
    {
        if (currentGrapplePoint == grapplePoint)
        {
            currentGrapplePoint = null;
            isUIActive = false;
            
            if (grappleIndicator != null)
                grappleIndicator.gameObject.SetActive(false);
        }
    }

    private void UpdateUIPosition()
    {
        if (mainCamera == null || currentGrapplePoint == null) return;

        Vector3 worldPosition = currentGrapplePoint.Position;
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

        if (screenPosition.z > 0)
        {
            grappleIndicator.position = screenPosition;
            
            if (!grappleIndicator.gameObject.activeSelf)
                grappleIndicator.gameObject.SetActive(true);
        }
        else
        {
            if (grappleIndicator.gameObject.activeSelf)
                grappleIndicator.gameObject.SetActive(false);
        }
    }

    private void UpdateRotation()
    {
        rotationAngle += rotationSpeed * Time.deltaTime;
        if (rotationAngle >= 360f)
            rotationAngle -= 360f;

        grappleIndicator.rotation = Quaternion.Euler(0f, 0f, rotationAngle);
    }

    private void UpdateScale()
    {
        scaleTimer += scalePulseSpeed * Time.deltaTime;
        float scaleFactor = Mathf.Lerp(minScale, maxScale, (Mathf.Sin(scaleTimer) + 1f) / 2f);
        grappleIndicator.localScale = Vector3.one * scaleFactor * scaleMultiplier;
    }
}
