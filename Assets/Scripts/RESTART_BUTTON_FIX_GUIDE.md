# Guia - Corrigindo o Botão de Restart na Clear Screen

Este guia explica como resolver problemas comuns com o botão de restart na tela de game clear.

---

## 🔧 Correções Aplicadas

### **1. Logs de Depuração Adicionados**

Agora o sistema registra no Console:
- ✅ Se o listener foi conectado ao botão
- ✅ Quando RestartGame() é chamado
- ✅ Quando o botão é ativado e tornado interativo

### **2. Botão Configurado como Interativo**

O botão agora é explicitamente configurado:
```csharp
restartButton.interactable = true;
```

### **3. StopAllCoroutines Adicionado**

Garante que nenhuma coroutine interfira ao reiniciar:
```csharp
StopAllCoroutines();
Time.timeScale = 1f;
```

---

## 🐛 Problemas Comuns e Soluções

### **Problema 1: Botão Não Responde**

#### **Diagnóstico**
- O botão está visível mas não clicável

#### **Soluções**

**A. Verificar se o Botão está Interativo**
1. Entre em Play Mode
2. Complete o nível (alcance o GameClear)
3. Quando a tela aparecer, clique no RestartButton na Hierarchy
4. No Inspector, verifique se `Button > Interactable` está marcado ✅

**B. Verificar EventSystem**
1. Na Hierarchy, procure `EventSystem`
2. Confirme que está ativo (checkbox marcado)
3. Verifique se tem os componentes:
   - `EventSystem`
   - `InputSystemUIInputModule` (ou `StandaloneInputModule`)

**C. Verificar GraphicRaycaster**
1. Selecione o `Canvas` na Hierarchy
2. No Inspector, confirme que tem `GraphicRaycaster` ativado
3. Se não tiver, adicione: `Add Component > Graphic Raycaster`

---

### **Problema 2: Botão Invisível ou Desapareceu**

#### **Diagnóstico**
- Tela aparece mas sem botão

#### **Soluções**

**A. Verificar Hierarquia**
```
Canvas > ClearScreenPanel > RestartButton
```

**B. Verificar se RestartButton está Ativo**
1. Entre em Play Mode
2. Complete o nível
3. Aguarde 2-3 segundos após a tela aparecer
4. Na Hierarchy, expanda: `Canvas > ClearScreenPanel > RestartButton`
5. Confirme que está ativo (checkbox marcado)

**C. Verificar Referência no Inspector**
1. Selecione o `Canvas` na Hierarchy
2. No Inspector, encontre `ClearScreen` component
3. Verifique se `Restart Button` está conectado:
   - Deve mostrar `RestartButton (Button)`
   - Se estiver `None`, arraste o botão da Hierarchy

---

### **Problema 3: Botão Clica mas Nada Acontece**

#### **Diagnóstico**
- Botão responde ao hover/click mas jogo não reinicia

#### **Soluções**

**A. Verificar Logs no Console**

Entre em Play Mode e complete o nível. Procure por:

```
✅ "RestartGame listener added to button!"
✅ "Restart button activated and set to interactable!"
```

Se clicar no botão, deve aparecer:
```
✅ "RestartGame() called!"
✅ "Loading scene 'Game'..."
```

**B. Se RestartGame() não é Chamado**

1. Selecione `Canvas > ClearScreenPanel > RestartButton`
2. No Inspector, role até `Button > On Click ()`
3. Verifique se tem 1 listener:
   - **Runtime Only**
   - Target: `Canvas (ClearScreen)`
   - Function: `ClearScreen.RestartGame`

Se estiver vazio:
1. Clique em `+` para adicionar
2. Arraste o `Canvas` para o campo Object
3. Selecione: `ClearScreen > RestartGame()`

**C. Se a Cena Não Carrega**

1. Menu: `File > Build Settings`
2. Verifique se "Game" está na lista de scenes
3. Se não estiver:
   - Clique `Add Open Scenes` (com Game.unity aberto)
   - Ou arraste `Assets/Scenes/Game.unity` para a lista

---

### **Problema 4: Time.timeScale = 0 Bloqueia o Botão**

#### **Diagnóstico**
- Com `Time.timeScale = 0`, alguns componentes param

#### **Solução Já Aplicada**

O sistema usa `WaitForSecondsRealtime` ao invés de `WaitForSeconds`:
```csharp
yield return new WaitForSecondsRealtime(1f);
```

Isso funciona mesmo com `Time.timeScale = 0`.

---

### **Problema 5: Cursor Travado**

#### **Diagnóstico**
- Cursor não aparece ou está travado

#### **Solução Já Aplicada**

O sistema desbloqueia o cursor automaticamente:
```csharp
Cursor.lockState = CursorLockMode.None;
Cursor.visible = true;
```

Se ainda estiver travado:
1. Pressione `Escape` durante Play Mode
2. Ou adicione ao `Update()` de `ClearScreen`:

```csharp
void Update()
{
    if (clearScreenPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
```

---

## 📋 Checklist Completo

Use este checklist para verificar todos os pontos:

### **1. Hierarquia**
- ✅ Canvas existe e está ativo
- ✅ Canvas > ClearScreen component presente
- ✅ Canvas > GraphicRaycaster presente
- ✅ Canvas > ClearScreenPanel existe
- ✅ ClearScreenPanel > RestartButton existe
- ✅ EventSystem existe na cena

### **2. Referências no Inspector (Canvas > ClearScreen)**
- ✅ Clear Screen Panel: `ClearScreenPanel`
- ✅ Congratulations Text: texto do título
- ✅ Final Time Text: texto do tempo
- ✅ Restart Button: `RestartButton`
- ✅ Fade Image: imagem de fade

### **3. Componentes do RestartButton**
- ✅ RectTransform
- ✅ Canvas Renderer
- ✅ Image
- ✅ Button (Interactable ✅)

### **4. Listener do Botão (RestartButton > Button)**
- ✅ On Click () tem 1 listener
- ✅ Runtime Only
- ✅ Target: Canvas (ClearScreen)
- ✅ Function: ClearScreen.RestartGame

### **5. Build Settings**
- ✅ Scene "Game" está na lista
- ✅ Scene "Game" tem index correto

### **6. Durante Play Mode**
- ✅ Console mostra "RestartGame listener added to button!"
- ✅ Quando completar: "ClearSequence coroutine started!"
- ✅ Após 2-3 segundos: "Restart button activated and set to interactable!"
- ✅ Ao clicar: "RestartGame() called!"
- ✅ Ao clicar: "Loading scene 'Game'..."

---

## 🎮 Testando Passo a Passo

### **Teste 1: Verificação Básica**

1. Entre em Play Mode
2. Complete o nível (chegue no GameClear)
3. Aguarde a tela aparecer (~2 segundos)
4. **Observe o Console**:
   - Deve mostrar logs de ativação
5. **Mova o mouse** sobre o botão:
   - Deve mudar de cor (transition)
6. **Clique no botão**:
   - Console deve mostrar "RestartGame() called!"
   - Jogo deve reiniciar

### **Teste 2: Verificação do Cursor**

1. Entre em Play Mode
2. Complete o nível
3. **Verifique o cursor**:
   - Deve estar visível
   - Deve poder mover livremente
4. Se não estiver visível, pressione `Escape`

### **Teste 3: Verificação Manual**

Se o botão ainda não funcionar:

1. Entre em Play Mode
2. Complete o nível
3. Na Hierarchy, **manualmente ative/desative** o RestartButton
4. No Inspector, **manualmente marque** `Button > Interactable`
5. Tente clicar novamente

---

## 🔨 Fix Manual (Se Ainda Não Funcionar)

Se todas as verificações falharem, recrie o botão:

### **1. Delete o Botão Antigo**
```
Hierarchy > Canvas > ClearScreenPanel > RestartButton
Right-click > Delete
```

### **2. Crie um Novo Botão**
```
Right-click ClearScreenPanel > UI > Button - TextMeshPro
Rename para "RestartButton"
```

### **3. Configure o Novo Botão**

**A. RectTransform:**
```
Anchor: Bottom Center
Pos X: 0, Y: 60
Width: 200, Height: 50
```

**B. Image (Button):**
```
Color: RGB(50, 200, 50)
```

**C. TextMeshPro - Text:**
```
Text: "RESTART"
Font Size: 24
Alignment: Center
Color: White
```

**D. Button:**
```
Interactable: ✅
Transition: Color Tint
Normal Color: RGB(50, 200, 50)
Highlighted Color: RGB(70, 255, 70)
Pressed Color: RGB(30, 150, 30)
```

### **4. Reconecte no ClearScreen**

1. Selecione `Canvas`
2. No Inspector, `ClearScreen` component
3. Arraste o novo `RestartButton` para `Restart Button`

### **5. Adicione o Listener**

1. Selecione `RestartButton`
2. No Inspector, `Button > On Click ()`
3. Clique `+`
4. Arraste `Canvas` para o campo
5. Selecione `ClearScreen > RestartGame()`

---

## 💡 Dicas Extras

### **Adicionar Tecla de Atalho (R para Restart)**

No `ClearScreen.cs`, adicione:

```csharp
void Update()
{
    if (clearScreenPanel.activeSelf && Input.GetKeyDown(KeyCode.R))
    {
        Debug.Log("R key pressed - restarting game!");
        RestartGame();
    }
}
```

### **Adicionar Fade Out ao Reiniciar**

Crie uma coroutine:

```csharp
public void RestartGame()
{
    StartCoroutine(RestartWithFade());
}

private IEnumerator RestartWithFade()
{
    Debug.Log("RestartGame() called!");
    
    float duration = 0.5f;
    float elapsed = 0f;
    
    Color startColor = fadeImage.color;
    Color targetColor = new Color(0f, 0f, 0f, 1f);
    
    while (elapsed < duration)
    {
        elapsed += Time.unscaledDeltaTime;
        fadeImage.color = Color.Lerp(startColor, targetColor, elapsed / duration);
        yield return null;
    }
    
    Time.timeScale = 1f;
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
    
    SceneManager.LoadScene("Game");
}
```

### **Som ao Clicar no Botão**

Adicione um AudioSource ao Canvas:

```csharp
[SerializeField] private AudioClip buttonClickSound;
private AudioSource audioSource;

void Awake()
{
    audioSource = GetComponent<AudioSource>();
    // ... resto do código
}

public void RestartGame()
{
    if (buttonClickSound != null && audioSource != null)
    {
        audioSource.PlayOneShot(buttonClickSound);
    }
    
    // ... resto do código
}
```

---

## 🎯 Resultado Esperado

Após aplicar as correções:

1. **Tela aparece** com fade suave
2. **Texto anima** gradualmente
3. **Botão aparece** após 2 segundos
4. **Cursor está visível** e livre
5. **Hover no botão** muda a cor
6. **Click no botão**:
   - Console mostra "RestartGame() called!"
   - Jogo reinicia imediatamente
   - Cursor volta a travar
   - Time.timeScale volta a 1

---

## 📞 Debug Rápido

Se nada funcionar, teste este código temporário no `Update()` de `ClearScreen`:

```csharp
void Update()
{
    if (Input.GetKeyDown(KeyCode.F5))
    {
        Debug.Log("F5 pressed - Force restart!");
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game");
    }
    
    if (clearScreenPanel.activeSelf)
    {
        Debug.Log($"Button active: {restartButton.gameObject.activeSelf}, Interactable: {restartButton.interactable}");
    }
}
```

Pressione `F5` durante Play Mode para forçar restart e verificar se o problema é só o botão.

---

Siga este guia passo a passo e o botão de restart deve funcionar perfeitamente! Se o problema persistir, verifique os logs no Console para identificar exatamente onde está falhando.
