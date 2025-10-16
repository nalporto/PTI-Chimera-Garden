# Sistema de UI para Pontos de Grapple

Este sistema cria um indicador visual que aparece e rotaciona ao redor dos pontos de grapple quando o jogador está próximo.

## Scripts Criados

1. **GrapplePoint.cs** - Detecta quando o jogador está próximo do ponto de grapple
2. **GrappleUIManager.cs** - Gerencia a exibição e animação da UI
3. **GrappleUISetup.cs** - Ferramenta de criação automática da UI

## Setup Rápido (Método Automático)

### Passo 1: Criar a UI Automaticamente

1. No Unity Editor, vá ao menu superior
2. Clique em: `GameObject > UI > Grapple Point Indicator System`
3. Isso criará automaticamente:
   - Canvas (se não existir)
   - GrappleUIManager
   - GrappleIndicator com 4 brackets rotativos

### Passo 2: Configurar Pontos de Grapple

1. Na Hierarchy, selecione cada objeto que será um ponto de grapple
   - No seu caso: `/---MAP---/Sphere` (e outros pontos futuros)

2. Adicione o componente `GrapplePoint`:
   - Clique em "Add Component"
   - Digite "GrapplePoint"
   - Pressione Enter

3. Configure o GrapplePoint:
   - **Detection Radius**: `15` (distância para ativar a UI)
   - **Player Layer**: Selecione a layer "Player" (opcional, mas recomendado)
   - **Gizmo Color**: Cyan (cor dos gizmos no editor)
   - **Show Gizmos**: ✅ Habilitado (para ver o raio no Scene view)

4. **IMPORTANTE**: Certifique-se que seu jogador tem a tag "Player"

### Passo 3: Testar

1. Entre em Play Mode
2. Aproxime-se de um ponto de grapple
3. Você verá o indicador aparecer e rotacionar ao redor do ponto!

---

## Setup Manual (Alternativo)

Se o método automático não funcionar, siga estas etapas:

### Criar UI Manualmente

#### 1. Canvas
```
1. Hierarchy > Botão direito > UI > Canvas
2. Configure:
   - Render Mode: Screen Space - Overlay
   - Canvas Scaler > UI Scale Mode: Scale With Screen Size
   - Reference Resolution: 1920 x 1080
```

#### 2. GrappleUIManager GameObject
```
1. Botão direito no Canvas > Create Empty
2. Renomeie para "GrappleUIManager"
3. Add Component > GrappleUIManager
```

#### 3. GrappleIndicator
```
1. Botão direito em GrappleUIManager > Create Empty
2. Renomeie para "GrappleIndicator"
3. Rect Transform:
   - Width: 100
   - Height: 100
```

#### 4. Criar Brackets (Cantos)

Crie 4 GameObjects filhos de GrappleIndicator, cada um representando um canto:

**TopLeft:**
```
- Name: TopLeft
- Add Component > Image
- Rect Transform:
  - Anchored Position: X: -50, Y: 50
  - Width: 30, Height: 30
- Image:
  - Color: Cyan (0, 0.8, 1, 1)
  - Raycast Target: Desabilitado
```

**TopRight:**
```
- Name: TopRight
- Anchored Position: X: 50, Y: 50
- Rotation Z: 90
- (resto igual ao TopLeft)
```

**BottomRight:**
```
- Name: BottomRight
- Anchored Position: X: 50, Y: -50
- Rotation Z: 180
```

**BottomLeft:**
```
- Name: BottomLeft
- Anchored Position: X: -50, Y: -50
- Rotation Z: 270
```

#### 5. Conectar Referências no GrappleUIManager
```
- Grapple Indicator: Arraste o GameObject "GrappleIndicator"
- Main Camera: Arraste a Main Camera
- Canvas: Arraste o Canvas
```

---

## Configurações e Personalização

### GrapplePoint (nos pontos de grapple)

**Detection Radius** (Raio de Detecção)
- `10` = Muito próximo (precisa estar quase em cima)
- `15` = Médio (padrão recomendado)
- `25` = Longe (detecta de muito longe)

**Player Layer** (Layer do Jogador)
- Opcional, mas melhora performance
- Selecione a layer que o jogador usa

### GrappleUIManager

**Rotation Settings (Configurações de Rotação)**

- **Rotation Speed**: `90` (graus por segundo)
  - Valores maiores = rotação mais rápida
  - Valores menores = rotação mais lenta
  - Tente: `45` (lento), `90` (médio), `180` (rápido)

- **Scale Multiplier**: `1.0` (tamanho base)
  - `0.5` = metade do tamanho
  - `1.0` = tamanho normal
  - `2.0` = dobro do tamanho

- **Min Scale / Max Scale**: `0.8` / `1.2`
  - Define quanto o indicador "pulsa"
  - Valores mais próximos = menos pulsação
  - Valores mais distantes = mais pulsação

- **Scale Pulse Speed**: `2.0`
  - Velocidade da pulsação
  - Maior = pulsa mais rápido

**Positioning (Posicionamento)**

- **Offset From Point**: `50` (pixels)
  - Distância dos brackets do centro
  - Não usado no código atual, mas pode ser implementado

---

## Estrutura Final

```
Canvas (HUD Canvas)
└── GrappleUIManager (GameObject + Script)
    └── GrappleIndicator (RectTransform)
        ├── TopLeft (Image)
        ├── TopRight (Image)
        ├── BottomRight (Image)
        └── BottomLeft (Image)

---MAP---
└── Sphere (Grapple Point)
    └── GrapplePoint (Script)
```

---

## Criando Sprites Personalizados para os Brackets

Para criar brackets mais bonitos:

### Opção 1: Usar Sprites Existentes
1. Importe sprites de brackets (L shapes) para `/Assets/Sprites`
2. Configure como "Sprite (2D and UI)"
3. Arraste para o campo "Source Image" de cada Image dos brackets

### Opção 2: Criar no Editor de Imagens
1. Crie uma imagem 64x64 pixels
2. Desenhe um "L" (canto) em branco
3. Exporte como PNG com transparência
4. Importe para Unity
5. Use nos brackets

### Opção 3: Usar Fonte de Ícones
Se você tiver um font icon pack (Font Awesome, Material Icons):
1. Use caracteres como "┌", "┐", "└", "┘"
2. Ou crie Text ao invés de Image

---

## Personalizando as Cores

### Cores Recomendadas

**Cyan Clássico:**
```
Color: (0, 0.8, 1, 1) ou RGB(0, 204, 255)
```

**Verde Alien:**
```
Color: (0, 1, 0.5, 1) ou RGB(0, 255, 128)
```

**Roxo Místico:**
```
Color: (0.8, 0, 1, 1) ou RGB(204, 0, 255)
```

**Laranja Energia:**
```
Color: (1, 0.5, 0, 1) ou RGB(255, 128, 0)
```

### Adicionar Efeito de Brilho

Para fazer os brackets brilharem:

1. Selecione cada Image dos brackets
2. Add Component > Shadow
3. Configure:
   - Effect Color: Mesma cor mas mais brilhante
   - Effect Distance: (2, -2)
   - Use Graphic Alpha: Habilitado

Ou melhor ainda:

1. Use material com shader Unlit
2. Configure Emission para brilhar

---

## Adicionando Múltiplos Pontos de Grapple

1. **Para cada novo ponto de grapple:**
   - Crie ou selecione o GameObject na cena
   - Add Component > GrapplePoint
   - Ajuste o Detection Radius conforme necessário

2. **Tag "Grapple":**
   - Recomendo adicionar a tag "Grapple" em todos os pontos
   - Facilita encontrá-los depois

3. **Organização:**
   - Crie um GameObject pai vazio chamado "GrapplePoints"
   - Coloque todos os pontos dentro dele

---

## Testando no Scene View

Com "Show Gizmos" habilitado no GrapplePoint:

- **Esfera Azul (Cyan)** = Raio de detecção
- **Esfera pequena** = Centro do ponto de grapple
- **Amarelo quando selecionado** = Facilita visualizar o raio

Use isso para ajustar o Detection Radius visualmente!

---

## Troubleshooting

### Problema: UI não aparece

**Soluções:**
1. Verifique se o jogador tem a tag "Player"
2. Confirme que GrappleUIManager.Instance não é nulo
3. Verifique Console por erros
4. Certifique-se que a Main Camera está atribuída
5. Teste com Detection Radius maior (ex: 50)

### Problema: UI aparece mas não rotaciona

**Soluções:**
1. Verifique se Rotation Speed > 0
2. Confirme que GrappleIndicator está atribuído corretamente
3. Verifique se os brackets são filhos de GrappleIndicator

### Problema: UI fica em posição errada

**Soluções:**
1. Confirme que Canvas está em "Screen Space - Overlay"
2. Verifique se a Main Camera é a correta
3. Teste ajustando Canvas Scaler > Reference Resolution

### Problema: UI não desaparece ao sair do raio

**Soluções:**
1. Verifique se Update() está sendo chamado no GrapplePoint
2. Aumente/diminua o Detection Radius para testar
3. Adicione Debug.Log para verificar quando OnPlayerExitRange é chamado

### Problema: Performance ruim com muitos pontos

**Soluções:**
1. Use Player Layer no GrapplePoint
2. Considere usar trigger colliders ao invés de Distance checks
3. Implemente um sistema de culling (desabilitar pontos muito distantes)

---

## Melhorias Futuras

### 1. Adicionar Partículas
```csharp
[SerializeField] private ParticleSystem grappleParticles;

void OnPlayerEnterRange()
{
    if (grappleParticles != null)
        grappleParticles.Play();
}
```

### 2. Adicionar Som
```csharp
[SerializeField] private AudioClip detectionSound;
[SerializeField] private AudioSource audioSource;

void OnPlayerEnterRange()
{
    if (audioSource != null && detectionSound != null)
        audioSource.PlayOneShot(detectionSound);
}
```

### 3. Mostrar Distância
```csharp
[SerializeField] private Text distanceText;

void Update()
{
    if (isUIActive && distanceText != null)
    {
        float distance = Vector3.Distance(
            currentGrapplePoint.Position, 
            playerTransform.position
        );
        distanceText.text = $"{distance:F1}m";
    }
}
```

### 4. Integração com Input System
```csharp
// No GrappleUIManager, detectar quando jogador pressiona botão de grapple
if (isUIActive && Input.GetKeyDown(KeyCode.Space))
{
    // Executar grapple para currentGrapplePoint
}
```

### 5. Diferentes Tipos de Grapple Points
```csharp
public enum GrappleType { Normal, Speed, High, Swing }

[SerializeField] private GrappleType grappleType;
```

E use cores diferentes para cada tipo!

---

## Integração com Sistema de Grapple

Para conectar com seu sistema de grapple/magic:

```csharp
// No MagicController.cs ou novo script GrappleController.cs

void Update()
{
    if (Input.GetKeyDown(KeyCode.Space))
    {
        if (GrappleUIManager.Instance != null && 
            GrappleUIManager.Instance.CurrentGrapplePoint != null)
        {
            Vector3 targetPos = GrappleUIManager.Instance.CurrentGrapplePoint.Position;
            // Executar grapple para targetPos
        }
    }
}
```

---

Para mais detalhes sobre implementação avançada, consulte os scripts criados!
