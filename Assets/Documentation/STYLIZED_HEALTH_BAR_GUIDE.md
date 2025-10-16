# Guia - Barra de Vida Estilizada (Stylized Health Bar)

Barra de vida com design moderno inspirado em jogos AAA - formato de paralelogramo inclinado, bordas pretas, preenchimento vermelho e linhas diagonais divisórias.

---

## 🎨 Características Visuais

✅ **Formato de paralelogramo inclinado** (skewed/sheared)  
✅ **Borda preta grossa** com camadas múltiplas  
✅ **Preenchimento vermelho** que encolhe  
✅ **Linhas diagonais divisórias** (segmentos visuais)  
✅ **Texto de porcentagem** dentro da barra (ex: "100%")  
✅ **Transição suave** com mudança de cor  

---

## 🚀 Setup Rápido

### Passo 1: Criar Barra Estilizada

1. **Selecione o Canvas** na Hierarchy
2. Menu Unity:
   ```
   GameObject > UI > Health Bar System (Stylized)
   ```

Isso cria automaticamente toda a estrutura visual com skew!

---

### Passo 2: Testar

1. Enter Play Mode
2. Tome dano
3. Veja a barra vermelha encolher com estilo! 🔥

---

## 📐 Estrutura Criada

```
HealthPanel
└── BarContainer
    └── OuterBorder (Borda preta externa + Skew)
        └── InnerBorder (Borda cinza interna + Skew)
            └── HealthBarBackground (Fundo escuro + Skew)
                ├── HealthBarFill (Vermelho que encolhe + Skew)
                ├── Divider_1 (Linha diagonal)
                ├── Divider_2 (Linha diagonal)
                ├── Divider_3 (Linha diagonal)
                ├── Divider_4 (Linha diagonal)
                └── HealthText (TMP) "100%"
```

---

## 🎨 Visual Resultado

```
┌──────────────────────────────────────┐
│  ╔════════════════════════╗          │  ← Borda preta externa
│  ║ 100% ║ ║ ║ ║ ║         ║          │  ← Vermelho + Linhas
│  ╚════════════════════════╝          │
└──────────────────────────────────────┘

Formato inclinado (paralelogramo):
    ╔═══════════════════╗
   ║ 75% ║ ║ ║ ║        ║
  ╚═══════════════════╝
```

---

## 🔧 Personalização

### **Mudar Cor da Barra**

Selecione **HealthBarFill** e mude a cor do componente Image:

- **Vermelho** (padrão): `RGB(230, 25, 25)`
- **Azul**: `RGB(25, 100, 230)`
- **Verde**: `RGB(50, 200, 50)`
- **Roxo**: `RGB(150, 50, 200)`

### **Ajustar Inclinação (Skew)**

Selecione qualquer elemento com `SkewImage`:

```
Skew X: 0.3 (padrão)
• Maior = mais inclinado
• Menor = menos inclinado
• 0 = sem inclinação (retângulo normal)
```

### **Mudar Tamanho**

Selecione **HealthPanel**:

```
Width: 400 (padrão)
Height: 60 (padrão)
```

### **Cores do Sistema**

No **Canvas > HealthUI**:

```
Full Health Color: Vermelho brilhante
Medium Health Color: Laranja
Low Health Color: Vermelho escuro
```

### **Número de Divisórias**

Edite `HealthBarSetup.cs` linha onde chama:

```csharp
CreateDividerLines(barBackground, 5);  // 5 segmentos
```

Mude para `3`, `6`, `8`, etc.

---

## 📍 Posicionamento

### **Canto Superior Esquerdo** (Padrão)

```
HealthPanel:
• Anchor: Top Left
• Pos X: 30, Y: -30
```

### **Centro Superior**

```
HealthPanel:
• Anchor: Top Center
• Pos X: 0, Y: -30
```

### **Parte Inferior Esquerda**

```
HealthPanel:
• Anchor: Bottom Left
• Pos X: 30, Y: 30
```

---

## 🎨 Esquemas de Cores Populares

### **Clássico (Doom/Quake)**

```
Barra: Vermelho RGB(220, 20, 20)
Borda: Preto RGB(10, 10, 10)
Texto: Branco
```

### **Cyber (Cyberpunk)**

```
Barra: Ciano RGB(0, 220, 255)
Borda: Magenta Escuro RGB(100, 0, 100)
Texto: Branco com Neon
```

### **Militar (Call of Duty)**

```
Barra: Verde Militar RGB(100, 150, 50)
Borda: Verde Escuro RGB(20, 30, 20)
Texto: Branco
```

### **Futurista (Halo)**

```
Barra: Azul Brilhante RGB(50, 150, 255)
Borda: Azul Escuro RGB(10, 30, 60)
Texto: Ciano Claro
```

---

## 💡 Dicas de Design

### **Texto Mais Legível**

Adicione uma sombra ao texto:

```
HealthText:
• Font Size: 32 (padrão)
• Font Style: Bold
• Color: Branco
• Outline: Preto, Width 2
```

### **Animação de Pulse em Low HP**

O sistema já suporta mudança de cor. Para adicionar pulsação:

Edite `HealthUI.cs` e adicione no `UpdateHealthDisplay`:

```csharp
if (current <= lowHealthThreshold)
{
    float pulse = Mathf.PingPong(Time.time * 3f, 1f);
    float alpha = Mathf.Lerp(0.7f, 1f, pulse);
    Color pulseColor = lowHealthColor;
    pulseColor.a = alpha;
    healthBarImage.color = pulseColor;
}
```

### **Glow Effect (Brilho)**

Adicione um `Outline` component ao HealthBarFill:

```
UI > Effects > Outline
• Color: Branco com Alpha baixo (0.3)
• Distance: (2, -2)
```

---

## 🔨 Ajustes Técnicos

### **Componente SkewImage**

Cada elemento visual tem o componente `SkewImage` que distorce a mesh da UI.

**Propriedades:**
- `Skew X`: Inclinação horizontal (0.3 = ângulo de ~17°)
- `Skew Y`: Inclinação vertical (0 = sem inclinação vertical)

### **Como Funciona o Skew**

O script `SkewImage` modifica os vértices da UI:
- Distorce cada vértice baseado na posição Y
- Cria efeito de "cisalhamento" (shear)
- Preserva proporções e textos

### **Performance**

- O skew é calculado na mesh, não em shader
- Impacto mínimo de performance
- Funciona com qualquer Image/Text

---

## 🐛 Troubleshooting

### **Barra aparece como retângulo normal (sem inclinação)**

- Verifique se os componentes `SkewImage` estão presentes
- Confirme que `Skew X` não está em 0
- Reimporte o script `SkewImage.cs`

### **Texto aparece distorcido**

- O texto dentro da barra herdará o skew
- Isso é intencional para manter o visual coeso
- Se preferir texto reto, coloque fora do BarContainer

### **Linhas divisórias não aparecem**

- Verifique se foram criadas (Divider_1, Divider_2, etc.)
- Confirme que a cor não está transparente
- Ajuste a rotação das linhas (73° padrão)

### **Barra encolhe incorretamente**

- Pivot de HealthBarFill deve ser (0, 0.5)
- Anchor deve ser Left Center
- Verifique se Width está correta (380 padrão)

---

## 🎬 Comparação Visual

### **Antes (Barra Simples):**

```
Health: 75
█████████░░░░  (retângulo simples)
```

### **Depois (Barra Estilizada):**

```
   ╔═════════════╗
  ║ 75% ║ ║ ║ ║  ║  (paralelogramo inclinado)
 ╚═════════════╝
```

---

## 🌟 Exemplos de Uso

### **Barra Pequena (HUD Minimalista)**

```
HealthPanel:
• Width: 250
• Height: 40

HealthText:
• Font Size: 24
```

### **Barra Grande (Barra de Boss)**

```
HealthPanel:
• Width: 800
• Height: 80

HealthText:
• Font Size: 48
• Alignment: Center
```

### **Múltiplas Barras (Escudo + Vida)**

Crie duas instâncias:
```
HealthPanel_HP (Vermelha)
HealthPanel_Shield (Azul)

Posicione uma abaixo da outra
```

---

## 📋 Checklist Final

Antes de usar:

✅ Canvas selecionado antes de criar  
✅ Script `SkewImage.cs` compilado sem erros  
✅ HealthUI component tem referências preenchidas  
✅ PlayerHealth atribuído  
✅ Texto mostra porcentagem (ex: "100%")  
✅ Barra tem formato de paralelogramo  
✅ Linhas diagonais visíveis  
✅ Bordas pretas aparecem  

---

## 🎮 Testando

1. **Enter Play Mode**
2. **Tome dano**
3. **Observe:**
   - Barra vermelha encolhe da direita para esquerda
   - Texto atualiza (100% → 75% → 50%...)
   - Cor muda gradualmente (Vermelho → Laranja → Vermelho Escuro)
   - Linhas divisórias permanecem fixas
   - Formato inclinado é mantido

---

## 🚀 Resultado Final

Você terá uma barra de vida **profissional e estilizada** semelhante a jogos AAA modernos, com:

- ✅ Design visual único e marcante
- ✅ Animação suave e responsiva
- ✅ Fácil de ler em qualquer situação
- ✅ Totalmente personalizável
- ✅ Performance otimizada

---

Para referência adicional sobre UI design, procure por "skewed UI design", "parallelogram health bar", ou "stylized game UI" no Google Images!
