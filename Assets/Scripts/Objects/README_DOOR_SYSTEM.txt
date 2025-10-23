╔══════════════════════════════════════════════════════════════════════╗
║          SISTEMA DE PORTAS TRANCADAS - REFERÊNCIA RÁPIDA            ║
╚══════════════════════════════════════════════════════════════════════╝

📋 RESUMO DO SISTEMA
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
O sistema funciona em 3 etapas:

1. 🎯 RoomEnemyManager conta inimigos na sala
2. 🔴 Painel começa VERMELHO (trancado)
3. ⚔️  Jogador mata todos os inimigos
4. 🔵 Painel fica AZUL (destrancado) + mostra "Pressione F"
5. ✋ Jogador pressiona F no painel
6. 🟢 Painel fica VERDE (aberto) + porta destranca
7. 🚪 Porta abre quando jogador se aproxima

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

⚡ SETUP RÁPIDO (MÉTODO AUTOMÁTICO)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

1. Adicione LockedDoorSetupHelper a qualquer GameObject
2. Configure:
   - Door Parent: ParentDoor1 (ou sua porta)
   - Room Name: "Room1" (nome da sala)
   - Enemies Parent: /---IA--- (opcional)
3. Clique com botão direito no script > "Setup Locked Door System"
4. Configure manualmente o resto (veja abaixo)

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

✅ CHECKLIST DE CONFIGURAÇÃO MANUAL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

INIMIGOS:
☐ Todos inimigos têm Tag = "Enemy"
☐ Inimigos estão dentro da área da sala

DOOR ANIMATION:
☐ Is Locked = TRUE (✓ marcado)
☐ Player = PlayerObj (arraste da hierarquia)
☐ Interaction Point = DistanceCheck ou InteractionPoint

LOCKED DOOR PANEL:
☐ Room Manager = RoomManager_X (arraste da hierarquia)
☐ Door Animation = Porta1 (arraste a porta)
☐ Panel Renderer = MeshRenderer do painel
☐ Interaction Point = InteractionPoint (child do painel)
☐ Interact Prompt UI = InteractPromptPanel (Canvas)
☐ Interact Text = TextMeshPro do prompt

UI "PRESSIONE F":
☐ Canvas criado (Screen Space - Overlay)
☐ Panel criado com InteractPrompt component
☐ TextMeshPro configurado
☐ CanvasGroup adicionado
☐ Alpha inicial = 0

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

🎨 CRIANDO A UI "PRESSIONE F"
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

No Canvas principal (/---UI---/Canvas):

1. Criar Panel:
   Right-Click > UI > Panel
   Nome: InteractPromptPanel
   
   RectTransform:
   - Anchor Preset: Bottom-Center
   - Pos X: 0, Pos Y: 200
   - Width: 300, Height: 80
   
   Image:
   - Color: Preto semi-transparente (0, 0, 0, 150)
   
   Add Component:
   - Canvas Group (Alpha = 0)
   - Interact Prompt script

2. Criar Texto:
   Right-Click em InteractPromptPanel > UI > Text - TextMeshPro
   Nome: InteractText
   
   TextMeshPro:
   - Text: "Pressione F"
   - Font Size: 36
   - Alignment: Center/Middle
   - Color: Branco (255, 255, 255)

3. Configurar InteractPrompt:
   - Prompt Text: InteractText
   - Fade Speed: 5
   - Animate Scale: ✓
   - Pulse Speed: 2
   - Pulse Amount: 0.1

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

🎨 CORES DO PAINEL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Estado         | Cor RGB        | Significado
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
🔴 LOCKED      | (255, 0, 0)    | Porta trancada - mate os inimigos
🔵 UNLOCKED    | (0, 150, 255)  | Pode interagir - pressione F
🟢 OPENED      | (0, 255, 0)    | Porta aberta - pode passar

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

🔧 TROUBLESHOOTING
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

PROBLEMA: Painel não muda de cor
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
✓ Verificar Tag "Enemy" em todos os inimigos
✓ Verificar RoomManager configurado corretamente
✓ Verificar Material do painel tem Emission habilitado
✓ Ver Console para mensagens de debug

PROBLEMA: "Pressione F" não aparece
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
✓ Verificar InteractPromptPanel.Alpha inicial = 0
✓ Verificar CanvasGroup adicionado
✓ Verificar referências no LockedDoorPanel
✓ Verificar distância de interação (padrão: 3m)

PROBLEMA: Porta não abre após pressionar F
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
✓ Verificar DoorAnimation.isLocked começa TRUE
✓ Verificar referência doorAnimation no LockedDoorPanel
✓ Verificar Animator na porta tem animação configurada
✓ Ver Console para mensagens "Door opened!"

PROBLEMA: Contagem de inimigos errada
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
✓ Verificar Auto Detect Enemies = TRUE
✓ Verificar Tag "Enemy" em TODOS os inimigos
✓ Ver Console: "Room 'X' has Y enemies"
✓ Configurar Enemies Parent se necessário

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

📊 MENSAGENS DE DEBUG NO CONSOLE
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Ao iniciar:
"Room 'RoomManager_1' has 3 enemies."

Quando todos morrem:
"Room 'RoomManager_1' cleared!"
"Door panel 'DoorPanel_1' unlocked!"

Quando jogador interage:
"Door 'DoorPanel_1' opened!"

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

🎮 TESTANDO PASSO A PASSO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

1. ▶️  Play Mode
2. 👀 Verificar painel VERMELHO
3. 📍 Ver no Console quantos inimigos foram detectados
4. ⚔️  Matar todos os inimigos
5. 👀 Verificar painel ficou AZUL
6. 🚶 Aproximar do painel (distância < 3m)
7. 👀 Verificar "Pressione F" apareceu
8. ⌨️  Pressionar F
9. 👀 Verificar painel ficou VERDE
10. 🚶 Aproximar da porta
11. 🚪 Porta deve abrir automaticamente

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

💡 DICAS E BOAS PRÁTICAS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

✓ Use um RoomManager por sala/área
✓ Agrupe inimigos por sala para facilitar
✓ Mantenha nomes consistentes (Room1, Room2, etc.)
✓ Use a mesma UI para todas as portas (reaproveitável)
✓ Adicione sons para melhor feedback
✓ Ajuste cores conforme tema do seu jogo
✓ Coloque painéis em locais visíveis
✓ Teste cada porta individualmente

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

📁 ARQUIVOS CRIADOS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

/Assets/Scripts/Objects/
├── RoomEnemyManager.cs         - Gerencia contagem de inimigos
├── LockedDoorPanel.cs          - Painel interativo com estados
├── DoorAnimation.cs            - ATUALIZADO com lock/unlock
└── LockedDoorSetupHelper.cs    - Helper para setup automático

/Assets/Scripts/UI/
└── InteractPrompt.cs           - UI "Pressione F" animada

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

🚀 EXPANSÕES FUTURAS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

□ Adicionar partículas quando destrancar
□ Som ambiente quando painel muda de estado
□ Animação de "scan" no painel
□ Contador visual de inimigos restantes
□ Portas que trancam atrás do jogador
□ Múltiplos painéis para mesma porta
□ Requisitos além de matar inimigos (chaves, puzzles)
□ Sistema de desafio (tempo limite)

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

📞 PRECISA DE AJUDA?
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Consulte o guia detalhado: SETUP_DOOR_SYSTEM.txt

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
