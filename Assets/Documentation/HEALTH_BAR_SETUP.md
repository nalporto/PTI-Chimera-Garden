# Guia - Barra de Vida (Health Bar)

Sistema de barra de vida com transição suave, mudança de cor por nível de HP e texto atualizado.

---

## 🚀 Setup Rápido (Método Automático)

### Passo 1: Criar a Barra Automaticamente

1. **Selecione o Canvas** na Hierarchy (`/---UI---/Canvas`)
2. No menu Unity:
   ```
   GameObject > UI > Health Bar System
   ```

Isso cria automaticamente:
- HealthPanel (container)
- HealthText (texto "Health: 100")
- HealthBarBackground (fundo cinza)
- HealthBarFill (barra verde que encolhe)

E conecta tudo automaticamente ao componente `HealthUI` existente!

---

## 📋 Setup Manual (Se o automático não funcionar)

### Estrutura da UI

Crie esta hierarquia dentro do Canvas:

```
Canvas
└── HealthPanel (GameObject vazio)
    ├── HealthText (TextMeshProUGUI)
    ├── HealthBarBackground (Image)
    │   └── HealthBarFill (Image)
```

### Passo a Passo Detalhado

#### 1. Criar HealthPanel

```
Hierarchy > Botão direito no Canvas > Create Empty
Renomeie para "HealthPanel"

RectTransform:
• Anchor Preset: Top Left
• Pos X: 20
• Pos Y: -20
• Width: 200
• Height: 60
```

#### 2. Criar HealthText

```
Botão direito em HealthPanel > UI > Text - TextMeshPro
Renomeie para "HealthText"

RectTransform:
• Anchor Preset: Top Stretch
• Pos Y: 0
• Height: 25

TextMeshProUGUI:
• Text: "Health: 100"
• Font Size: 18
• Color: White
• Alignment: Left
```

#### 3. Criar HealthBarBackground

```
Botão direito em HealthPanel > UI > Image
Renomeie para "HealthBarBackground"

RectTransform:
• Anchor Preset: Bottom Stretch
• Pos Y: 0
• Height: 25

Image:
• Color: RGB(51, 51, 51) ou (0.2, 0.2, 0.2, 0.8)
• Raycast Target: Desabilitado
```

#### 4. Criar HealthBarFill

```
Botão direito em HealthBarBackground > UI > Image
Renomeie para "HealthBarFill"

RectTransform:
• Anchor Preset: Left Stretch
• Pivot X: 0, Y: 0.5
• Pos X: 0
• Width: 200

Image:
• Color: Verde (0, 255, 0)
• Raycast Target: Desabilitado
```

#### 5. Conectar ao HealthUI

```
Selecione o Canvas
No Inspector > HealthUI:
• Health Text: Arraste HealthText
• Health Bar Fill: Arraste HealthBarFill
• Player Health: Arraste o GameObject do jogador (se não estiver)
```

---

## 🎨 Personalização

### Cores da Barra

No componente `HealthUI` do Canvas:

**Full Health Color (Verde):**
```
RGB(0, 255, 0) ou (0, 1, 0)
```

**Medium Health Color (Amarelo):**
```
RGB(255, 255, 0) ou (1, 1, 0)
```

**Low Health Color (Vermelho):**
```
RGB(255, 0, 0) ou (1, 0, 0)
```

### Thresholds (Limites)

```
Medium Health Threshold: 50
• Abaixo de 50 HP = Amarelo

Low Health Threshold: 25
• Abaixo de 25 HP = Vermelho
```

### Transição Suave

```
Smooth Transition: ✅ Habilitado
Transition Speed: 5
• Maior = transição mais rápida
• Menor = transição mais suave
```

---

## 📐 Exemplos de Layout

### Canto Superior Esquerdo (Padrão)

```
HealthPanel:
• Anchor: Top Left
• Pos X: 20, Y: -20
```

### Canto Superior Direito

```
HealthPanel:
• Anchor: Top Right
• Pos X: -20, Y: -20
```

### Centro Superior

```
HealthPanel:
• Anchor: Top Center
• Pos X: 0, Y: -20
```

### Parte Inferior (Como outros jogos)

```
HealthPanel:
• Anchor: Bottom Left
• Pos X: 20, Y: 20
```

---

## 🎯 Estilos de Barra

### Estilo Minimalista

```
HealthBarBackground:
• Color: (0, 0, 0, 0.5) - Preto transparente
• Height: 15

HealthBarFill:
• Width: 150
• Height: 15 (mesmo do background)
```

### Estilo Bold/Grosso

```
HealthBarBackground:
• Height: 35

HealthBarFill:
• Width: 250
• Height: 35

HealthText:
• Font Size: 22
```

### Estilo com Borda

```
Adicione outro Image como child de HealthBarBackground:
• Name: "Border"
• Color: Branco
• Ajuste para ficar 2px maior que o background
```

---

## 🌈 Esquemas de Cores Populares

### Clássico (RPG)

```
Full: Verde (0, 255, 0)
Medium: Amarelo (255, 255, 0)
Low: Vermelho (255, 0, 0)
```

### Moderno (Sci-Fi)

```
Full: Ciano (0, 255, 255)
Medium: Laranja (255, 165, 0)
Low: Rosa (255, 0, 127)
```

### Escuro (Souls-like)

```
Full: Cinza Claro (200, 200, 200)
Medium: Amarelo Escuro (180, 150, 0)
Low: Vermelho Escuro (180, 0, 0)
```

### Neon

```
Full: Verde Neon (0, 255, 127)
Medium: Amarelo Neon (255, 255, 0)
Low: Vermelho Neon (255, 0, 127)
```

---

## 🔧 Recursos Avançados

### 1. Adicionar Animação de Pulse quando Low HP

No HealthUI.cs, você pode adicionar:

```csharp
if (current <= lowHealthThreshold)
{
    float pulse = Mathf.PingPong(Time.time * 2f, 1f);
    healthBarImage.color = Color.Lerp(lowHealthColor, Color.white, pulse * 0.3f);
}
```

### 2. Mostrar HP Numérico na Barra

Mude o texto para:
```csharp
healthText.text = $"{current} / {max}";
```

### 3. Barra com Ícone de Coração

Adicione um Image antes do HealthText:
```
HealthPanel
├── HeartIcon (Image) - Use um sprite de coração
├── HealthText
└── HealthBarBackground
```

### 4. Segunda Barra (Damage Preview)

Crie uma cópia de HealthBarFill:
```
HealthBarFill_Delayed
• Mesma cor mas com Alpha 0.5
• Atualiza mais lento que a principal
• Mostra quanto HP você acabou de perder
```

---

## 📊 Comparação Visual

### Antes (Texto Apenas):
```
Health: 75
```

### Depois (Barra):
```
Health: 75
█████████████░░░░░░░  (75%)
```

---

## ✅ Checklist de Verificação

Antes de testar:

✅ HealthPanel está dentro do Canvas  
✅ HealthBarFill está dentro de HealthBarBackground  
✅ HealthText tem componente TextMeshProUGUI  
✅ HealthBarFill tem Pivot X = 0, Y = 0.5  
✅ HealthBarFill tem Anchor = Left Stretch  
✅ Canvas > HealthUI tem as referências preenchidas  
✅ PlayerHealth está atribuído no HealthUI  

---

## 🐛 Troubleshooting

### Barra não aparece
- Verifique se HealthBarFill tem componente Image
- Confirme que a cor não está transparente (Alpha > 0)
- Verifique se Width > 0

### Barra não encolhe
- Confirme que Anchor é "Left Stretch"
- Pivot deve ser (0, 0.5)
- Verifique se Health Bar Fill está atribuído no HealthUI

### Barra encolhe errado (para os lados)
- Pivot X deve ser 0 (não 0.5)
- Anchor deve ser Left Stretch (não Stretch/Stretch)

### Cor não muda
- Verifique os thresholds (Medium = 50, Low = 25)
- Confirme que as cores estão diferentes entre si
- Teste com valores de HP diferentes

---

## 🎮 Testando

1. Entre em Play Mode
2. Deixe inimigos atirarem em você
3. Observe:
   - Barra diminui suavemente (se Smooth Transition ativo)
   - Cor muda de Verde → Amarelo → Vermelho
   - Texto atualiza com HP atual

---

## 💡 Dicas

- Use **Smooth Transition** para feedback visual mais agradável
- Ajuste **Transition Speed** para controlar velocidade
- **Low Health Threshold** combina bem com vignette de dano (já implementado!)
- Considere adicionar som quando HP fica baixo

---

Para mais informações sobre sistemas de UI, consulte a documentação oficial do Unity sobre UI Toolkit e Canvas!
