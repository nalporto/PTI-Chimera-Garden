# Guia Visual - Sistema de UI para Pontos de Grapple

## Como Funciona

```
                    ┌─────┐
                    │     │  <- Bracket rotacionando
                    │     │
                    └─────┘

        ╔═══════════════════════╗
        ║                       ║
        ║     ●  PONTO DE       ║  <- Círculo azul (Grapple Point)
        ║        GRAPPLE        ║
        ║                       ║
        ╚═══════════════════════╝

                    ┌─────┐
                    │     │  <- Bracket rotacionando
                    │     │
                    └─────┘
```

**Quando o jogador está próximo:**
1. Os 4 brackets aparecem ao redor do ponto
2. Eles rotacionam continuamente
3. Pulsam suavemente (aumentam/diminuem de tamanho)

---

## Estrutura Visual da UI

```
Canvas (Screen Space Overlay)
│
└── GrappleUIManager (GameObject invisível)
    │
    └── GrappleIndicator (Container 100x100)
        │
        ├── TopLeft     ┌──   (Bracket superior esquerdo)
        ├── TopRight      ──┐ (Bracket superior direito)
        ├── BottomLeft  └──   (Bracket inferior esquerdo)
        └── BottomRight   ──┘ (Bracket inferior direito)
```

---

## Visualização dos Brackets

Cada bracket é um "L" que aponta para o centro:

```
     ┌────          ────┐
     │                  │
     │     ● GRAPPLE    │     <- Círculo central
     │                  │
     └────          ────┘
```

**Rotacionando:**
```
Frame 1:        Frame 2:        Frame 3:
┌──    ──┐      ╱──    ──╲      │      │
│  ●    │       ─   ●   ─       ─  ●  ─
└──    ──┘      ╲──    ──╱      │      │
```

---

## Cores e Estilos Sugeridos

### 1. Cyan Tech (Padrão)
```
Color: RGB(0, 204, 255) ou (0, 0.8, 1)
Estilo: Futurístico, clean
Exemplo: Apex Legends, Titanfall
```

### 2. Verde Matrix
```
Color: RGB(0, 255, 128) ou (0, 1, 0.5)
Estilo: Hacker, cyberpunk
Exemplo: Matrix, Deus Ex
```

### 3. Roxo Místico
```
Color: RGB(204, 0, 255) ou (0.8, 0, 1)
Estilo: Mágico, sobrenatural
Exemplo: League of Legends
```

### 4. Laranja Energia
```
Color: RGB(255, 128, 0) ou (1, 0.5, 0)
Estilo: Energético, dinâmico
Exemplo: Portal, Half-Life
```

---

## Animações

### Rotação
```
0° → 90° → 180° → 270° → 360° (volta ao início)

Velocidade ajustável:
- Lenta:  45°/s  (suave, elegante)
- Média:  90°/s  (padrão)
- Rápida: 180°/s (frenética)
```

### Pulsação (Scale)
```
Tamanho Base: 1.0

   1.2 ┐     ╱╲     ╱╲
       │    ╱  ╲   ╱  ╲
   1.0 ┤───╱    ╲─╱    ╲───
       │                  
   0.8 ┘                   
       └─────────────────→
             Tempo

Min Scale: 0.8 (80% do tamanho)
Max Scale: 1.2 (120% do tamanho)
```

---

## Configuração da Hierarquia

### No Scene View:
```
Hierarchy                        Inspector
│
├─ ---PLAYER---                  
│  └─ Cam
│     └─ Camera                  Tag: MainCamera ✓
│
├─ ---MAP---
│  └─ Sphere                     Tag: Grapple ✓
│     └─ [GrapplePoint]          Detection Radius: 15
│                                Show Gizmos: ✓
│
└─ HUD Canvas
   └─ GrappleUIManager
      └─ GrappleIndicator
         ├─ TopLeft     (Image)
         ├─ TopRight    (Image)
         ├─ BottomRight (Image)
         └─ BottomLeft  (Image)
```

---

## Gizmos no Scene View

Quando você seleciona um GrapplePoint:

```
     🎯 Amarelo (selecionado)
    ╱                    ╲
   ╱     Raio = 15m       ╲
  │         ●             │  <- Esfera amarela
   ╲      (ponto)        ╱
    ╲                    ╱

Cyan quando não selecionado
```

Isso ajuda a visualizar:
- Onde o jogador precisa estar
- Quanto precisa se aproximar
- Ajustar o raio visualmente

---

## Fluxo de Execução

```
┌──────────────────────────────────────────────────────┐
│ JOGADOR APROXIMA DO PONTO DE GRAPPLE                 │
└──────────────────────────────────────────────────────┘
                       │
                       ↓
┌──────────────────────────────────────────────────────┐
│ GrapplePoint detecta no Update():                    │
│ • Calcula distância até jogador                      │
│ • Distância <= 15m ?                                 │
└──────────────────────────────────────────────────────┘
                       │
                    SIM ↓
┌──────────────────────────────────────────────────────┐
│ GrapplePoint.OnPlayerEnterRange()                    │
│ • Chama GrappleUIManager.Instance.ShowGrappleUI()    │
└──────────────────────────────────────────────────────┘
                       │
                       ↓
┌──────────────────────────────────────────────────────┐
│ GrappleUIManager.ShowGrappleUI()                     │
│ • Ativa o GameObject GrappleIndicator                │
│ • Inicia rotação e pulsação                          │
└──────────────────────────────────────────────────────┘
                       │
                       ↓
┌──────────────────────────────────────────────────────┐
│ A cada frame (Update):                               │
│ • UpdateUIPosition() - segue o ponto no mundo        │
│ • UpdateRotation() - rotaciona os brackets           │
│ • UpdateScale() - pulsa o tamanho                    │
└──────────────────────────────────────────────────────┘
```

---

## Exemplo de Uso em Código

### Detectar quando jogador pode usar grapple:

```csharp
using UnityEngine;

public class PlayerGrappleController : MonoBehaviour
{
    void Update()
    {
        // Verifica se jogador pressionou espaço E está próximo de um ponto
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (GrappleUIManager.Instance != null && 
                GrappleUIManager.Instance.IsUIActive)
            {
                GrapplePoint target = GrappleUIManager.Instance.CurrentGrapplePoint;
                
                if (target != null)
                {
                    // Executar grapple para target.Position
                    Debug.Log($"Grappling to: {target.Position}");
                    // TODO: Implementar movimento de grapple
                }
            }
        }
    }
}
```

---

## Múltiplos Pontos de Grapple

```
Mapa com 5 pontos:

   Ponto A                 Ponto B
      ●                       ●
        ╲                   ╱
         ╲                 ╱
          ╲               ╱
           ● Ponto C    ╱
          ╱             ╱
         ╱             ╱
        ╱             ●
       ●           Ponto D
   Ponto E

Comportamento:
• Jogador próximo de A → UI aparece em A
• Jogador se move para B → UI desaparece de A, aparece em B
• Apenas 1 UI ativa por vez
• Troca suavemente entre pontos
```

---

## Exemplo Visual da Gameplay

```
[JOGADOR]                    ╔════════╗
   🚶                        ║ MURO   ║
    │                        ║        ║
    │  15m                   ║        ║
    │──────→ ┌──    ──┐      ║        ║
    │        │    ●   │  ← UI║        ║
    │        └──    ──┘      ║        ║
    ↓                        ║        ║
 Aproxima                    ╚════════╝
                                ↑
                          Ponto de Grapple
                          no topo do muro

Quando jogador está próximo:
1. UI aparece ao redor do ponto
2. Brackets rotacionam
3. Jogador pressiona Space
4. Grapple executa!
```

---

## Dicas de Design

### ✅ BOM:
- Cor contrastante com o ambiente
- Animação suave e constante
- Tamanho adequado (nem muito grande, nem pequeno)
- Aparecer apenas quando realmente próximo

### ❌ EVITE:
- Cores muito escuras (difícil de ver)
- Animação muito rápida (causa enjoo)
- UI muito grande (polui a tela)
- Aparecer de longe demais (confuso)

---

## Customizações Populares

### 1. Adicionar Texto de Distância
```
     ┌──    ──┐
     │    ●   │
     └──    ──┘
      [12.5m]  ← Mostra distância
```

### 2. Adicionar Linha de Conexão
```
[JOGADOR]
    │╲
    │ ╲ Linha tracejada
    │  ╲
    │   ╲
    │    ● PONTO
```

### 3. Diferentes Cores por Tipo
```
● Verde  = Grapple Normal
● Azul   = Grapple de Velocidade
● Roxo   = Grapple Alto
● Laranja = Grapple Swing
```

---

Para implementação detalhada, consulte:
- `GRAPPLE_UI_QUICK_START.md` - Setup rápido
- `GRAPPLE_UI_SETUP.md` - Documentação completa
