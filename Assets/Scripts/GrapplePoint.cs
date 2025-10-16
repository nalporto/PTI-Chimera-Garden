using UnityEngine;

public class GrapplePoint : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 15f;
    [SerializeField] private LayerMask playerLayer;
    
    [Header("Visual Feedback")]
    [SerializeField] private Color gizmoColor = Color.cyan;
    [SerializeField] private bool showGizmos = true;

    private Transform playerTransform;
    private bool isPlayerInRange = false;

    public float DetectionRadius => detectionRadius;
    public bool IsPlayerInRange => isPlayerInRange;
    public Vector3 Position => transform.position;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning($"GrapplePoint '{gameObject.name}': Player not found! Make sure player has 'Player' tag.");
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        bool wasInRange = isPlayerInRange;
        isPlayerInRange = distance <= detectionRadius;

        if (isPlayerInRange && !wasInRange)
        {
            OnPlayerEnterRange();
        }
        else if (!isPlayerInRange && wasInRange)
        {
            OnPlayerExitRange();
        }
    }

    private void OnPlayerEnterRange()
    {
        if (GrappleUIManager.Instance != null)
        {
            GrappleUIManager.Instance.ShowGrappleUI(this);
        }
    }

    private void OnPlayerExitRange()
    {
        if (GrappleUIManager.Instance != null)
        {
            GrappleUIManager.Instance.HideGrappleUI(this);
        }
    }

    void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.1f);
        Gizmos.DrawSphere(transform.position, 0.5f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.DrawSphere(transform.position, 0.3f);
    }
}
