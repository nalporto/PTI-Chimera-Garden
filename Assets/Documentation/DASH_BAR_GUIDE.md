# Guia - Dash Bar System (Barras de Preenchimento)

Sistema de dash com barras horizontais inclinadas (paralelogramo) que preenchem gradualmente durante a recarga. Cada dash tem sua própria barra visual.

---

## 🎨 Características Visuais

✅ **2 barras horizontais** (uma para cada dash)  
✅ **Formato de paralelogramo inclinado** (skewed)  
✅ **Preenchimento azul ciano** que cresce durante recarga  
✅ **Linhas diagonais divisórias** em cada barra  
✅ **Bordas pretas múltiplas** para profundidade  
✅ **Animação suave** de preenchimento  
✅ **Mudança de cor** baseada no estado:
- Azul ciano = Dash cheio e disponível
- Azul claro = Dash recarregando
- Cinza escuro = Dash vazio

---

## 🚀 Setup Rápido

### Passo 1: Criar Dash Bars

1. **Selecione o Canvas** na Hierarchy
2. Menu Unity:
   ```
   GameObject > UI > Dash Bar System (Stylized)
   ```

Isso cria automaticamente:
- ✅ 2 barras horizontais lado a lado
- ✅ Formato de paralelogramo inclinado
- ✅ Bordas pretas com camadas
- ✅ Linhas divisórias diagonais
- ✅ Conecta ao DashUI existente

---

### Passo 2: Testar

1. Enter Play Mode
2. Use seus 2 dashes
3. Observe as barras esvaziarem
4. Aguarde - as barras preenchem automaticamente! ⚡

---

## 📐 Como Funciona

### **Sistema de Recarga**

- Cada dash recarrega em **2 segundos** (configurável)
- A barra preenche gradualmente da esquerda para direita
- Quando completa, o dash fica disponível

### **Estados Visuais**

```
Estado 1: Dois dashes cheios
[████████████] [████████████]  (Azul ciano)

Estado 2: Um dash usado, um recarregando
[██████░░░░░░] [████████████]  (Azul claro + Azul ciano)

Estado 3: Dois dashes vazios, primeiro recarregando
[████░░░░░░░░] [░░░░░░░░░░░░]  (Azul claro + Cinza)
```

---

## 📐 Estrutura Criada

```
DashPanel (HorizontalLayout)
├── DashBar_1
│   └── OuterBorder (Borda preta + Skew)
│       └── InnerBorder (Borda cinza + Skew)
│           └── BarBackground (Fundo + Skew)
│               ├── BarFill (Azul ciano que preenche + Skew)
│               ├── Divider_1 (Linha diagonal)
│               └── Divider_2 (Linha diagonal)
└── DashBar_2
    └── [mesma estrutura]
```

---

## 🎨 Visual Final

```
Duas barras lado a lado (paralelogramo):

      ╔══════╗     ╔══════╗
     ║ ████  ║    ║ ████  ║  (Barras inclinadas)
    ╚══════╝     ╚══════╝

Quando uma está recarregando:

      ╔══════╗     ╔══════╗
     ║ ██░░  ║    ║ ████  ║  (Primeira recarregando)
    ╚══════╝     ╚══════╝
```

---

## 🔧 Personalização

### **Mudar Cor das Barras**

No **Canvas > DashUI**:

```
Full Color: RGB(25, 153, 230, 0.8) - Azul ciano (dash cheio)
Recharging Color: RGB(128, 204, 255, 0.8) - Azul claro (recarregando)
Empty Color: RGB(38, 38, 38, 0.8) - Cinza escuro (vazio)
```

### **Ajustar Transparência**

Mude o Alpha (último valor):
- **0.5** = mais transparente
- **0.8** = padrão (semi-transparente)
- **1.0** = totalmente opaco

### **Velocidade de Transição**

No **Canvas > DashUI**:
```
Smooth Transition: ✅ (habilitado)
Transition Speed: 5 (padrão)
• Maior = preenchimento mais rápido
• Menor = preenchimento mais suave
```

### **Tempo de Recarga**

No **Player > PlayerCharacter**:
```
Dash Recharge Time: 2 (segundos)
```

### **Ajustar Inclinação**

Selecione qualquer `BarFill` ou `OuterBorder`:
```
SkewImage > Skew X: 0.3 (padrão)
• Maior = mais inclinado
• Menor = menos inclinado
```

### **Tamanho das Barras**

Selecione **DashBar_1** ou **DashBar_2**:
```
Width: 180 (padrão)
Height: 40 (padrão)
```

### **Espaçamento Entre Barras**

Selecione **DashPanel**:
```
HorizontalLayoutGroup > Spacing: 10
```

---

## 📍 Posicionamento

### **Centro Superior** (Padrão)

```
DashPanel:
• Anchor: Top Center
• Pos X: 0, Y: -100
```

### **Abaixo da Health Bar**

```
DashPanel:
• Anchor: Top Left
• Pos X: 30, Y: -110
```

### **Canto Superior Direito**

```
DashPanel:
• Anchor: Top Right
• Pos X: -30, Y: -30
```

---

## 🎨 Esquemas de Cores

### **Ciano Clássico** (Padrão)

```
Full: RGB(25, 153, 230, 0.8) - Azul ciano
Recharging: RGB(128, 204, 255, 0.8) - Azul claro
Empty: RGB(38, 38, 38, 0.8) - Cinza escuro
```

### **Verde Energia**

```
Full: RGB(50, 230, 100, 0.8) - Verde brilhante
Recharging: RGB(150, 255, 180, 0.8) - Verde claro
Empty: RGB(30, 40, 30, 0.8) - Verde escuro
```

### **Laranja Velocidade**

```
Full: RGB(255, 150, 50, 0.8) - Laranja
Recharging: RGB(255, 200, 150, 0.8) - Laranja claro
Empty: RGB(50, 30, 20, 0.8) - Marrom escuro
```

### **Roxo Místico**

```
Full: RGB(150, 50, 230, 0.8) - Roxo
Recharging: RGB(200, 150, 255, 0.8) - Roxo claro
Empty: RGB(40, 30, 50, 0.8) - Roxo escuro
```

---

## 💡 Recursos Avançados

### **Efeito Pulse na Barra Cheia**

Adicione ao `DashUI.cs`:

```csharp
if (i < currentCharges)
{
    float pulse = Mathf.PingPong(Time.time * 2f, 1f);
    Color pulseColor = Color.Lerp(fullColor, Color.white, pulse * 0.2f);
    dashBarImages[i].color = pulseColor;
}
```

### **Som ao Completar Recarga**

No `DashUI.cs`, adicione:

```csharp
private int lastCharges = 0;

void Update()
{
    int current = playerCharacter.GetDashCharges();
    
    if (current > lastCharges)
    {
        // Toque um som de recarga completa
        AudioSource.PlayClipAtPoint(rechargeSound, Camera.main.transform.position);
    }
    
    lastCharges = current;
    UpdateDashBars();
}
```

### **Adicionar Ícones**

Adicione um Image dentro de cada DashBar:

```
DashBar_1
└── OuterBorder
    └── InnerBorder
        ├── DashIcon (Image sprite de raio/seta)
        └── BarBackground
```

---

## 🔨 Detalhes Técnicos

### **Como Funciona o Preenchimento**

1. `PlayerCharacter` rastreia:
   - `dashCharges` (número atual de dashes)
   - `dashRechargeTimer` (tempo acumulado)
   - `dashRechargeTime` (tempo total para recarregar)

2. `DashUI` acessa:
   - `GetDashCharges()` → número de dashes disponíveis
   - `GetDashRechargeProgress()` → 0.0 a 1.0 (porcentagem)

3. A barra ajusta seu `sizeDelta.x` baseado na porcentagem

### **Sistema de Múltiplas Barras**

- Array `dashBarFills[]` contém todas as barras
- Loop atualiza cada barra baseada no índice:
  - `i < currentCharges` → Barra cheia (1.0)
  - `i == currentCharges` → Barra recarregando (0.0 - 1.0)
  - `i > currentCharges` → Barra vazia (0.0)

---

## 🐛 Troubleshooting

### **Barras não aparecem**

- Verifique se `dashBarFills` está preenchido no Canvas > DashUI
- Confirme que PlayerCharacter está atribuído
- Verifique Console por erros

### **Barras não preenchem**

- Teste usar um dash primeiro
- Confirme que `GetDashRechargeProgress()` retorna valores 0-1
- Verifique se `dashRechargeTime` > 0 no PlayerCharacter

### **Barras preenchem instantaneamente**

- No Canvas > DashUI, ajuste:
  - `Smooth Transition: ✅`
  - `Transition Speed: 3` (menor = mais suave)

### **Barras encolhem para os lados errados**

- Confirme que BarFill tem:
  - Pivot: (0, 0.5)
  - Anchor: Left Center
  - Anchored Position X: -82 (metade da largura negativa)

### **Cores não mudam**

- Verifique se as cores são diferentes entre si
- Confirme que Alpha > 0 em todas as cores
- Teste mudando manualmente no Inspector

---

## 🎬 Comparação Visual

### **Antes (Hexágono com Número):**

```
   ╔═══════╦═══════╗
  ║   2   ║       ║  (Apenas mostra número)
 ╚═══════╩═══════╝
```

### **Depois (Barras de Preenchimento):**

```
   ╔══════╗     ╔══════╗
  ║ ████  ║    ║ ██░░  ║  (Mostra progresso visual)
 ╚══════╝     ╚══════╝
```

---

## 🌟 Vantagens do Sistema de Barras

✅ **Feedback visual claro** - veja exatamente quanto tempo falta  
✅ **Múltiplos dashes** - cada um tem sua barra  
✅ **Animação suave** - preenchimento gradual  
✅ **Cores diferentes** - distingue estados facilmente  
✅ **Design moderno** - formato inclinado profissional  
✅ **Combina com Health Bar** - mesmo estilo visual  

---

## 📋 Checklist Final

Antes de usar:

✅ Canvas selecionado ao criar  
✅ DashUI tem `dashBarFills` array preenchido  
✅ PlayerCharacter atribuído no DashUI  
✅ Duas barras criadas (DashBar_1 e DashBar_2)  
✅ Barras têm formato inclinado (skewed)  
✅ Linhas divisórias visíveis  
✅ BarFill tem Pivot (0, 0.5)  

---

## 🎮 Testando

1. **Enter Play Mode**
2. **Use 2 dashes** (tecla configurada)
3. **Observe:**
   - Ambas as barras esvaziam
   - Primeira barra começa a preencher (azul claro)
   - Quando completa, muda para azul ciano
   - Segunda barra começa a preencher
   - Após ~4 segundos, ambas estarão cheias

---

## ✨ Resultado Final

Você terá um **sistema de dash profissional** com:

- ✅ Barras visuais de preenchimento
- ✅ Feedback em tempo real de recarga
- ✅ Design inclinado estilizado
- ✅ Múltiplas barras (uma por dash)
- ✅ Cores diferentes por estado
- ✅ Animação suave
- ✅ Totalmente personalizável

---

Para referência visual, procure por "dash bar UI", "cooldown bar", ou "ability recharge indicator" em jogos como Overwatch, Apex Legends ou Valorant!
