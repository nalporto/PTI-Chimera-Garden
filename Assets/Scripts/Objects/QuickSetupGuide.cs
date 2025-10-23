using UnityEngine;

public class QuickSetupGuide : MonoBehaviour
{
    [Header("═══════════════════════════════════════════")]
    [Header("    GUIA RÁPIDO - PORTA TRANCADA")]
    [Header("═══════════════════════════════════════════")]
    
    [Space(10)]
    [Header("📋 PASSO 1: Configure os Inimigos")]
    [Tooltip("Certifique-se que todos os inimigos têm Tag = 'Enemy'")]
    public bool step1_EnemiesTagged = false;
    
    [Space(10)]
    [Header("📋 PASSO 2: Crie o Room Manager")]
    [Tooltip("Crie um GameObject vazio e adicione RoomEnemyManager")]
    public RoomEnemyManager step2_RoomManager;
    
    [Space(10)]
    [Header("📋 PASSO 3: Crie o Painel da Porta")]
    [Tooltip("Crie um Cube (escala 0.5, 0.5, 0.1) com Material emissor")]
    public GameObject step3_DoorPanel;
    
    [Space(10)]
    [Header("📋 PASSO 4: Adicione LockedDoorPanel")]
    [Tooltip("Adicione o script LockedDoorPanel ao painel")]
    public LockedDoorPanel step4_LockedDoorPanel;
    
    [Space(10)]
    [Header("📋 PASSO 5: Configure a Porta")]
    [Tooltip("Configure DoorAnimation com isLocked = TRUE")]
    public DoorAnimation step5_DoorAnimation;
    
    [Space(10)]
    [Header("📋 PASSO 6: Crie a UI 'Pressione F'")]
    [Tooltip("Crie um Panel no Canvas com InteractPrompt")]
    public GameObject step6_InteractPromptUI;
    
    [Space(10)]
    [Header("═══════════════════════════════════════════")]
    [Header("    ✅ VERIFICAÇÃO AUTOMÁTICA")]
    [Header("═══════════════════════════════════════════")]
    
    [Space(10)]
    public bool allStepsComplete = false;
    
    [ContextMenu("Verificar Setup Completo")]
    public void CheckSetup()
    {
        Debug.Log("════════════════════════════════════════");
        Debug.Log("   VERIFICANDO CONFIGURAÇÃO DA PORTA");
        Debug.Log("════════════════════════════════════════");
        
        bool allGood = true;
        
        if (step2_RoomManager == null)
        {
            Debug.LogError("❌ PASSO 2: RoomManager não configurado!");
            allGood = false;
        }
        else
        {
            Debug.Log("✓ PASSO 2: RoomManager OK");
        }
        
        if (step3_DoorPanel == null)
        {
            Debug.LogError("❌ PASSO 3: Door Panel não configurado!");
            allGood = false;
        }
        else
        {
            Debug.Log("✓ PASSO 3: Door Panel OK");
        }
        
        if (step4_LockedDoorPanel == null)
        {
            Debug.LogError("❌ PASSO 4: LockedDoorPanel não configurado!");
            allGood = false;
        }
        else
        {
            Debug.Log("✓ PASSO 4: LockedDoorPanel OK");
            
            if (step4_LockedDoorPanel.GetType().GetField("roomManager", 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance) != null)
            {
                Debug.Log("  → Verifique referências no Inspector");
            }
        }
        
        if (step5_DoorAnimation == null)
        {
            Debug.LogError("❌ PASSO 5: DoorAnimation não configurado!");
            allGood = false;
        }
        else
        {
            Debug.Log("✓ PASSO 5: DoorAnimation OK");
            if (!step5_DoorAnimation.isLocked)
            {
                Debug.LogWarning("⚠ DoorAnimation.isLocked deveria ser TRUE!");
            }
        }
        
        if (step6_InteractPromptUI == null)
        {
            Debug.LogError("❌ PASSO 6: Interact Prompt UI não configurado!");
            allGood = false;
        }
        else
        {
            Debug.Log("✓ PASSO 6: Interact Prompt UI OK");
        }
        
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
        {
            Debug.LogWarning("⚠ PASSO 1: Nenhum inimigo com Tag 'Enemy' encontrado!");
            allGood = false;
        }
        else
        {
            Debug.Log($"✓ PASSO 1: {enemies.Length} inimigos encontrados");
        }
        
        Debug.Log("════════════════════════════════════════");
        
        if (allGood)
        {
            Debug.Log("🎉 TUDO CERTO! Sistema pronto para testar!");
            allStepsComplete = true;
        }
        else
        {
            Debug.Log("❌ Há problemas na configuração. Veja acima.");
            allStepsComplete = false;
        }
        
        Debug.Log("════════════════════════════════════════");
    }
    
    [ContextMenu("Mostrar Estrutura Recomendada")]
    public void ShowRecommendedStructure()
    {
        Debug.Log(@"
════════════════════════════════════════════════
        ESTRUTURA RECOMENDADA
════════════════════════════════════════════════

/---MAP---
  /Map
    /RoomManager_1                [RoomEnemyManager]
      └─ Auto Detect Enemies: ✓
      
    /ParentDoor1
      ├─ /DoorPanel_1             [LockedDoorPanel]
      │   └─ /InteractionPoint
      │
      ├─ /Porta1                  [DoorAnimation]
      │   └─ isLocked: TRUE ✓
      │
      └─ /DistanceCheck1

/---IA---
  ├─ /Enemy_M                     [Tag: Enemy]
  ├─ /Enemy_R (1)                 [Tag: Enemy]
  └─ ...

/---UI---
  /Canvas
    └─ /InteractPromptPanel       [InteractPrompt]
        └─ /InteractText          [TextMeshPro]

════════════════════════════════════════════════
        ");
    }
}
