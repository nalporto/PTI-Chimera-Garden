# Configuração do Sistema de Feedback de Dano

Este documento explica como configurar o novo sistema de feedback visual e sonoro quando o jogador toma dano.

## Scripts Criados

1. **CameraShake.cs** - Cria tremor na câmera quando o jogador toma dano
2. **DamageVignette.cs** - Mostra efeito vermelho nas bordas da tela
3. **PlayerHealth.cs** - Atualizado para integrar os sistemas de feedback

## Configuração Passo a Passo

### Passo 1: Configurar Camera Shake

1. Na Hierarchy, localize o GameObject da câmera principal (provavelmente em `/---PLAYER---/Cam/Spring/Lean/Camera`)
2. Selecione o GameObject da câmera
3. Clique em "Add Component" no Inspector
4. Adicione o componente `CameraShake`
5. Configure os parâmetros:
   - **Shake Intensity**: `0.3` (intensidade do tremor)
   - **Shake Duration**: `0.2` (duração em segundos)
   - **Shake Frequency**: `25` (velocidade da vibração)

**Dica:** Valores maiores de Intensity criam tremores mais fortes. Ajuste conforme preferir!

### Passo 2: Criar UI de Vignette de Dano

#### 2.1 - Criar Canvas (se ainda não existir)
1. Na Hierarchy, clique com botão direito > UI > Canvas
2. Renomeie para "HUD Canvas" (ou use o Canvas existente)
3. Configure o Canvas:
   - Render Mode: `Screen Space - Overlay`
   - Canvas Scaler > UI Scale Mode: `Scale With Screen Size`
   - Reference Resolution: `1920 x 1080`

#### 2.2 - Criar Imagem de Vignette
1. Clique com botão direito no Canvas > UI > Image
2. Renomeie para "DamageVignette"
3. Configure a imagem:
   - **Anchor Preset**: Clique no quadrado no canto superior esquerdo do Rect Transform, segure Alt+Shift e clique em "stretch/stretch" (canto inferior direito)
   - **Left, Right, Top, Bottom**: Todos em `0`
   - **Color**: Defina Alpha como `0` (transparente)
   - **Raycast Target**: DESABILITADO (desmarque)

#### 2.3 - Adicionar Sprite de Vignette (RECOMENDADO)

**Opção A: Criar Vignette Simples**
1. Vá em Assets > Create > Sprites > Square
2. Importe ou crie uma textura de vignette (circular escura nas bordas)
3. Arraste a textura para o campo "Source Image" do componente Image

**Opção B: Usar sem Sprite**
1. Deixe "Source Image" vazio
2. A imagem será um retângulo sólido vermelho (menos realista mas funcional)

#### 2.4 - Adicionar Script DamageVignette
1. Selecione o GameObject "DamageVignette"
2. Clique em "Add Component"
3. Adicione o componente `DamageVignette`
4. Configure os parâmetros:
   
   **Damage Flash Settings:**
   - **Damage Color**: Vermelho com alpha `(R: 255, G: 0, B: 0, A: 76)` ou `(1, 0, 0, 0.3)`
   - **Flash Duration**: `0.2` segundos
   
   **Low Health Vignette:**
   - **Enable Low Health Vignette**: ✅ HABILITADO
   - **Low Health Threshold**: `0.3` (quando vida está abaixo de 30%)
   - **Low Health Color**: Vermelho com alpha baixo `(R: 255, G: 0, B: 0, A: 38)` ou `(1, 0, 0, 0.15)`
   - **Pulse Duration**: `1.5` segundos

5. O campo "Vignette Image" deve ser automaticamente preenchido, mas se não for:
   - Arraste o componente Image do próprio GameObject para este campo

### Passo 3: Configurar PlayerHealth

1. Na Hierarchy, localize o GameObject do jogador (provavelmente em `/---PLAYER---`)
2. Selecione o GameObject que tem o componente `PlayerHealth`
3. No Inspector, localize o componente PlayerHealth
4. Configure as referências:
   - **Camera Shake**: Arraste o GameObject da câmera (que tem o script CameraShake)
   - **Damage Vignette**: Arraste o GameObject "DamageVignette" da UI
   - **Damage Sound**: (OPCIONAL) Arraste um AudioClip de som de dano
   - **Audio Source**: (OPCIONAL) Arraste ou adicione um AudioSource

### Passo 4: Criar Sprite de Vignette Profissional (OPCIONAL)

Para um efeito mais profissional, você pode criar um sprite de vignette:

1. Use um editor de imagens (Photoshop, GIMP, Photopea)
2. Crie uma imagem 1024x1024
3. Desenhe um gradiente radial:
   - Centro: Totalmente transparente
   - Bordas: Preto sólido
4. Exporte como PNG com transparência
5. Importe para Unity (pasta `/Assets/UI` ou `/Assets/Sprites`)
6. Configure a textura:
   - Texture Type: `Sprite (2D and UI)`
   - Alpha Is Transparency: ✅ HABILITADO
7. Arraste para o campo "Source Image" do componente Image da DamageVignette

## Testando o Sistema

1. Entre em Play Mode
2. Deixe um inimigo atirar em você
3. Você deve ver:
   - ✅ Câmera tremendo rapidamente
   - ✅ Flash vermelho nas bordas da tela
   - ✅ (Opcional) Som de dano tocando
4. Quando a vida estiver baixa (< 30%):
   - ✅ Vignette vermelha pulsando continuamente

## Personalizando o Efeito

### Ajustar Intensidade do Camera Shake
No componente `CameraShake`:
- **Shake suave**: Intensity `0.1`, Duration `0.15`
- **Shake médio**: Intensity `0.3`, Duration `0.2` (padrão)
- **Shake forte**: Intensity `0.5`, Duration `0.3`

### Ajustar Cor do Vignette
No componente `DamageVignette`:
- **Vermelho clássico**: `(1, 0, 0, 0.3)`
- **Vermelho escuro**: `(0.5, 0, 0, 0.4)`
- **Roxo (veneno)**: `(0.5, 0, 0.5, 0.3)`
- **Azul (frio)**: `(0, 0.3, 1, 0.3)`

### Desabilitar Low Health Vignette
Se você não quiser o efeito de vida baixa:
1. No componente `DamageVignette`
2. Desmarque "Enable Low Health Vignette"

### Criar Diferentes Tipos de Dano

Você pode criar variações do método `TakeDamage` para diferentes tipos de dano:

```csharp
public void TakeDamage(int amount, float shakeIntensity = 0.3f)
{
    // ... código existente ...
    
    if (cameraShake != null)
        cameraShake.TriggerShake(shakeIntensity, 0.2f);
}

// Dano explosivo - shake mais forte
public void TakeExplosiveDamage(int amount)
{
    TakeDamage(amount, 0.6f);
}

// Dano de fogo - shake prolongado
public void TakeFireDamage(int amount)
{
    if (cameraShake != null)
        cameraShake.TriggerShake(0.2f, 0.5f);
    // ... resto do código ...
}
```

## Estrutura da UI Recomendada

```
Canvas (HUD Canvas)
├── DamageVignette (Image + DamageVignette script)
├── HealthBar (sua UI de vida)
├── AmmoDisplay (sua UI de munição)
└── Crosshair (sua mira)
```

**IMPORTANTE:** A DamageVignette deve estar ACIMA dos outros elementos na hierarquia para renderizar por cima de tudo.

## Troubleshooting

### Problema: Vignette não aparece
**Soluções:**
- Verifique se o Canvas está em "Screen Space - Overlay"
- Verifique se a Image tem o componente DamageVignette
- Verifique se a referência "Vignette Image" está preenchida
- Tente aumentar o Alpha do Damage Color para `0.5` ou `0.8` para teste

### Problema: Camera Shake não funciona
**Soluções:**
- Verifique se o script está no GameObject da câmera REAL (não no parent)
- Verifique se a referência em PlayerHealth está correta
- Tente aumentar Shake Intensity para `1.0` para teste
- Verifique se o GameObject da câmera não tem outros scripts que resetam a posição

### Problema: Vignette fica presa na tela
**Soluções:**
- Verifique se não há corrotinas sendo chamadas múltiplas vezes
- Chame `damageVignette.ClearVignette()` manualmente no Console durante Play Mode
- Verifique se PlayerHealth está chamando ClearVignette() no Respawn

### Problema: Low Health Vignette não aparece
**Soluções:**
- Verifique se "Enable Low Health Vignette" está marcado
- Reduza sua vida para menos de 30% para testar
- Aumente o Alpha do Low Health Color para teste
- Verifique se PlayerHealth está sendo encontrado corretamente

## Melhorias Futuras

Ideias para expandir o sistema:

1. **Diferentes cores para tipos de dano**
   - Fogo: Laranja/Vermelho
   - Veneno: Verde/Roxo
   - Gelo: Azul claro
   - Elétrico: Azul/Branco

2. **Chromatic Aberration**
   - Usar URP Post Processing para adicionar aberração cromática no dano

3. **Motion Blur**
   - Adicionar motion blur temporário quando toma dano

4. **Tela rachada**
   - Sprite de vidro rachado que aparece quando vida está crítica

5. **Heartbeat Audio**
   - Som de batimento cardíaco quando vida está baixa

6. **Controller Rumble**
   - Vibração no controle quando toma dano (se aplicável)
