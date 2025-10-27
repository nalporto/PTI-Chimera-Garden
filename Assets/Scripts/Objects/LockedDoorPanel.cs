using UnityEngine;
using TMPro;

public class LockedDoorPanel : MonoBehaviour
{
    public enum DoorPanelState
    {
        Locked,
        Unlocked,
        Opened
    }
    
    [Header("References")]
    [SerializeField] private RoomEnemyManager roomManager;
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private MeshRenderer panelRenderer;
    [SerializeField] private GameObject interactPromptUI;
    [SerializeField] private TextMeshProUGUI interactText;
    
    [Header("Panel Settings")]
    [SerializeField] private Transform interactionPoint;
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private KeyCode interactKey = KeyCode.F;
    
    [Header("Panel Colors")]
    [SerializeField] private Color lockedColor = Color.red;
    [SerializeField] private Color unlockedColor = Color.blue;
    [SerializeField] private Color openedColor = Color.green;
    [SerializeField] private int materialIndex = 0;
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip lockedSound;
    [SerializeField] private AudioClip unlockSound;
    [SerializeField] private AudioClip openSound;
    
    private DoorPanelState currentState = DoorPanelState.Locked;
    private GameObject player;
    private bool playerInRange = false;
    private Material panelMaterial;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        
        if (player == null)
            Debug.LogError($"[{gameObject.name}] Player not found! Make sure PlayerObj has tag 'Player'");
        
        if (panelRenderer == null)
        {
            panelRenderer = GetComponent<MeshRenderer>();
            Debug.LogWarning($"[{gameObject.name}] Panel Renderer was null, auto-assigned from self");
        }
        
        if (panelRenderer != null)
        {
            Material[] materials = panelRenderer.materials;
            if (materialIndex < materials.Length)
            {
                panelMaterial = materials[materialIndex];
                Debug.Log($"[{gameObject.name}] Panel material assigned: {panelMaterial.name}");
            }
            else
            {
                Debug.LogError($"[{gameObject.name}] Material index {materialIndex} out of range! Only {materials.Length} materials.");
            }
        }
        else
        {
            Debug.LogError($"[{gameObject.name}] Panel Renderer is null! Cannot change colors.");
        }
        
        if (roomManager != null)
        {
            roomManager.onAllEnemiesKilled.AddListener(OnRoomCleared);
            Debug.Log($"[{gameObject.name}] Connected to RoomManager: {roomManager.gameObject.name}");
        }
        else
        {
            Debug.LogError($"[{gameObject.name}] Room Manager not assigned! Panel won't unlock.");
        }
        
        if (interactionPoint == null)
        {
            interactionPoint = transform;
            Debug.LogWarning($"[{gameObject.name}] Interaction Point was null, using self position");
        }
        
        if (interactText != null)
        {
            interactText.text = $"Pressione {interactKey.ToString()}";
        }
        
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        
        UpdatePanelState();
        Debug.Log($"[{gameObject.name}] Initial state: {currentState}, Color: {lockedColor}");
        
        if (interactPromptUI != null)
        {
            interactPromptUI.SetActive(false);
            Debug.Log($"[{gameObject.name}] InteractPrompt UI hidden initially");
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] InteractPrompt UI not assigned! Won't show 'Press F' message.");
        }
    }
    
    void Update()
    {
        CheckPlayerDistance();
        HandleInteraction();
    }
    
    private void CheckPlayerDistance()
    {
        if (player == null || interactionPoint == null)
            return;
        
        float distance = Vector3.Distance(player.transform.position, interactionPoint.position);
        bool wasInRange = playerInRange;
        playerInRange = distance <= interactionDistance;
        
        if (playerInRange != wasInRange)
        {
            UpdateInteractPrompt();
        }
    }
    
    private void UpdateInteractPrompt()
    {
        if (interactPromptUI == null)
            return;
        
        bool showPrompt = playerInRange && currentState == DoorPanelState.Unlocked;
        
        if (showPrompt)
        {
            interactPromptUI.SetActive(true);
            var promptScript = interactPromptUI.GetComponent<InteractPrompt>();
            if (promptScript != null)
                promptScript.Show();
            
            Debug.Log($"[{gameObject.name}] Showing interact prompt (playerInRange: {playerInRange}, state: {currentState})");
        }
        else
        {
            var promptScript = interactPromptUI.GetComponent<InteractPrompt>();
            if (promptScript != null)
                promptScript.Hide();
            else
                interactPromptUI.SetActive(false);
        }
    }
    
    private void HandleInteraction()
    {
        if (!playerInRange || currentState != DoorPanelState.Unlocked)
            return;
        
        if (Input.GetKeyDown(interactKey))
        {
            OpenDoor();
        }
    }
    
    private void OnRoomCleared()
    {
        Debug.Log($"[{gameObject.name}] OnRoomCleared called! Current state: {currentState}");
        
        if (currentState == DoorPanelState.Locked)
        {
            currentState = DoorPanelState.Unlocked;
            UpdatePanelState();
            PlaySound(unlockSound);
            Debug.Log($"[{gameObject.name}] Door panel unlocked! Color changed to {unlockedColor}");
        }
    }
    
    private void OpenDoor()
    {
        if (currentState != DoorPanelState.Unlocked)
            return;
        
        currentState = DoorPanelState.Opened;
        UpdatePanelState();
        PlaySound(openSound);
        
        if (doorAnimator != null)
        {
            doorAnimator.SetBool("IsOpened", true);
            doorAnimator.SetBool("IsLocked", false);
            Debug.Log($"[{gameObject.name}] Door opened! IsOpened=true, IsLocked=false");
        }
        else
        {
            Debug.LogError($"[{gameObject.name}] Cannot open door - Door Animator not assigned!");
        }
        
        Debug.Log($"[{gameObject.name}] Panel interaction complete. Door should stay open.");
    }
    
    private void UpdatePanelState()
    {
        Color targetColor = currentState switch
        {
            DoorPanelState.Locked => lockedColor,
            DoorPanelState.Unlocked => unlockedColor,
            DoorPanelState.Opened => openedColor,
            _ => lockedColor
        };
        
        if (panelMaterial != null)
        {
            panelMaterial.SetColor("_BaseColor", targetColor);
            panelMaterial.SetColor("_EmissionColor", targetColor * 2f);
            Debug.Log($"[{gameObject.name}] Panel color updated to {targetColor} (State: {currentState})");
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] Cannot update color - panelMaterial is null!");
        }
        
        UpdateInteractPrompt();
    }
    
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (interactionPoint == null)
            return;
        
        Gizmos.color = currentState switch
        {
            DoorPanelState.Locked => Color.red,
            DoorPanelState.Unlocked => Color.blue,
            DoorPanelState.Opened => Color.green,
            _ => Color.yellow
        };
        
        Gizmos.DrawWireSphere(interactionPoint.position, interactionDistance);
    }
}
