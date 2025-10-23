using UnityEngine;

public class LockedDoorSetupHelper : MonoBehaviour
{
    [Header("Auto Setup - Preencha estas referências")]
    [Tooltip("Parent da porta (ex: ParentDoor1)")]
    public GameObject doorParent;
    
    [Tooltip("Nome da sala/área para o RoomManager")]
    public string roomName = "Room1";
    
    [Tooltip("Parent dos inimigos desta sala (deixe vazio para auto-detect)")]
    public Transform enemiesParent;
    
    [Header("Configurações do Painel")]
    public Vector3 panelLocalPosition = new Vector3(2f, 1.5f, 0f);
    public Vector3 panelScale = new Vector3(0.5f, 0.5f, 0.1f);
    public Vector3 interactionPointOffset = new Vector3(0f, 0f, 1f);
    
    [Header("Configurações da Porta")]
    public float interactionDistance = 3f;
    public string interactKey = "F";
    
    [Header("Debug")]
    public bool showDebugInfo = true;
    
    [ContextMenu("Setup Locked Door System")]
    public void SetupLockedDoorSystem()
    {
        if (doorParent == null)
        {
            Debug.LogError("Door Parent não foi atribuído!");
            return;
        }
        
        Debug.Log($"Iniciando setup do sistema de porta trancada para: {doorParent.name}");
        
        GameObject roomManager = CreateRoomManager();
        GameObject doorPanel = CreateDoorPanel();
        GameObject interactionPoint = CreateInteractionPoint(doorPanel);
        
        ConfigureDoorAnimation();
        ConfigureLockedDoorPanel(doorPanel, interactionPoint, roomManager);
        
        Debug.Log($"✓ Setup completo! Agora configure manualmente:");
        Debug.Log($"  1. Adicione Tag 'Enemy' a todos os inimigos");
        Debug.Log($"  2. Configure a UI 'Pressione F' no Canvas");
        Debug.Log($"  3. Arraste a UI no LockedDoorPanel");
        Debug.Log($"  4. Adicione sons (opcional)");
    }
    
    private GameObject CreateRoomManager()
    {
        string managerName = $"RoomManager_{roomName}";
        GameObject manager = GameObject.Find(managerName);
        
        if (manager == null)
        {
            manager = new GameObject(managerName);
            manager.transform.SetParent(doorParent.transform.parent);
            manager.transform.position = doorParent.transform.position;
        }
        
        RoomEnemyManager roomManager = manager.GetComponent<RoomEnemyManager>();
        if (roomManager == null)
        {
            roomManager = manager.AddComponent<RoomEnemyManager>();
        }
        
        if (showDebugInfo)
            Debug.Log($"✓ RoomManager criado: {managerName}");
        
        return manager;
    }
    
    private GameObject CreateDoorPanel()
    {
        string panelName = $"DoorPanel_{doorParent.name}";
        Transform existingPanel = doorParent.transform.Find(panelName);
        
        if (existingPanel != null)
        {
            if (showDebugInfo)
                Debug.Log($"⚠ Painel já existe: {panelName}");
            return existingPanel.gameObject;
        }
        
        GameObject panel = GameObject.CreatePrimitive(PrimitiveType.Cube);
        panel.name = panelName;
        panel.transform.SetParent(doorParent.transform);
        panel.transform.localPosition = panelLocalPosition;
        panel.transform.localScale = panelScale;
        
        MeshRenderer renderer = panel.GetComponent<MeshRenderer>();
        Material panelMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        panelMat.name = $"DoorPanelMat_{roomName}";
        panelMat.SetColor("_BaseColor", Color.red);
        panelMat.EnableKeyword("_EMISSION");
        panelMat.SetColor("_EmissionColor", Color.red * 2f);
        renderer.material = panelMat;
        
        if (showDebugInfo)
            Debug.Log($"✓ Painel criado: {panelName}");
        
        return panel;
    }
    
    private GameObject CreateInteractionPoint(GameObject panel)
    {
        Transform existingPoint = panel.transform.Find("InteractionPoint");
        
        if (existingPoint != null)
        {
            if (showDebugInfo)
                Debug.Log($"⚠ InteractionPoint já existe");
            return existingPoint.gameObject;
        }
        
        GameObject point = new GameObject("InteractionPoint");
        point.transform.SetParent(panel.transform);
        point.transform.localPosition = interactionPointOffset;
        
        if (showDebugInfo)
            Debug.Log($"✓ InteractionPoint criado");
        
        return point;
    }
    
    private void ConfigureDoorAnimation()
    {
        DoorAnimation[] doors = doorParent.GetComponentsInChildren<DoorAnimation>();
        
        if (doors.Length == 0)
        {
            Debug.LogWarning($"⚠ Nenhum DoorAnimation encontrado em {doorParent.name}");
            return;
        }
        
        foreach (DoorAnimation door in doors)
        {
            door.isLocked = true;
            door.interactionDistance = interactionDistance;
            
            if (door.player == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    door.player = player;
                }
            }
            
            if (showDebugInfo)
                Debug.Log($"✓ DoorAnimation configurado: {door.gameObject.name}");
        }
    }
    
    private void ConfigureLockedDoorPanel(GameObject panel, GameObject interactionPoint, GameObject roomManager)
    {
        LockedDoorPanel lockPanel = panel.GetComponent<LockedDoorPanel>();
        if (lockPanel == null)
        {
            lockPanel = panel.AddComponent<LockedDoorPanel>();
        }
        
        AudioSource audioSource = panel.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = panel.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0.5f;
        }
        
        if (showDebugInfo)
            Debug.Log($"✓ LockedDoorPanel configurado");
        
        Debug.Log($"⚠ CONFIGURE MANUALMENTE no Inspector:");
        Debug.Log($"  - Room Manager: {roomManager.name}");
        Debug.Log($"  - Door Animation: A porta dentro de {doorParent.name}");
        Debug.Log($"  - Interact Prompt UI: Canvas UI com InteractPrompt");
    }
}
