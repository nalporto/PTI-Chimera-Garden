# Guia Rápido - Sistema de Feedback de Dano

## Setup Rápido (5 minutos)

### Método Automático (RECOMENDADO)

1. **Criar UI de Vignette Automaticamente**
   - No Unity Editor, vá no menu superior
   - Clique em: `GameObject > UI > Damage Feedback System`
   - Isso criará automaticamente o Canvas e a DamageVignette

2. **Adicionar Camera Shake**
   - Na Hierarchy, localize sua câmera principal
   - Caminho provável: `/---PLAYER---/Cam/Spring/Lean/Camera`
   - Selecione o GameObject da câmera
   - Clique em "Add Component" > `CameraShake`

3. **Conectar no PlayerHealth**
   - Na Hierarchy, localize o GameObject do jogador
   - Selecione o GameObject que tem o `PlayerHealth`
   - No Inspector, no componente PlayerHealth:
     - **Camera Shake**: Arraste a câmera (que agora tem CameraShake)
     - **Damage Vignette**: Arraste o GameObject "DamageVignette" da UI

4. **Testar**
   - Entre em Play Mode
   - Deixe um inimigo atirar em você
   - Você verá o tremor de câmera e flash vermelho!

---

### Método Manual (Alternativo)

Se o método automático não funcionar:

#### Passo 1: Camera Shake
```
1. Selecione: /---PLAYER---/Cam/Spring/Lean/Camera
2. Add Component > CameraShake
3. Deixe os valores padrão
```

#### Passo 2: UI Vignette
```
1. Hierarchy > Botão direito > UI > Canvas (se não tiver)
2. Botão direito no Canvas > UI > Image
3. Renomeie para "DamageVignette"
4. Configure a Image:
   - Anchor Preset: Stretch/Stretch (Alt+Shift + canto inferior direito)
   - Color Alpha: 0 (transparente)
   - Raycast Target: DESABILITADO
5. Add Component > DamageVignette
```

#### Passo 3: Conectar PlayerHealth
```
1. Selecione o GameObject do jogador (com PlayerHealth)
2. No Inspector > PlayerHealth:
   - Camera Shake: Arraste a câmera
   - Damage Vignette: Arraste o GameObject DamageVignette
```

---

## Ajustes Rápidos

### Camera Shake mais forte
No componente `CameraShake` da câmera:
- Shake Intensity: `0.5` (padrão é `0.3`)

### Flash vermelho mais visível
No componente `DamageVignette`:
- Damage Color > Alpha: `128` ou `0.5` (padrão é `76` ou `0.3`)

### Desabilitar efeito de vida baixa
No componente `DamageVignette`:
- Desmarque "Enable Low Health Vignette"

---

## Verificação Rápida

Antes de testar, confirme:

✅ CameraShake está na câmera REAL (não no parent)  
✅ DamageVignette (GameObject) está dentro do Canvas  
✅ DamageVignette tem componente Image E DamageVignette  
✅ PlayerHealth tem as duas referências preenchidas  
✅ Image da vignette tem Alpha = 0 inicialmente  
✅ Image da vignette tem Raycast Target = DESABILITADO  

---

## Testando

1. Play Mode
2. Deixe inimigo atirar em você
3. Deve ver:
   - Camera tremendo
   - Flash vermelho rápido
4. Com vida < 30%:
   - Vignette vermelha pulsando

---

## Problema Comum

**"Não vejo nada acontecendo!"**

Verifique no Console se aparecem erros. Se não houver erros:

1. Aumente o Alpha do Damage Color para `1.0` (totalmente opaco) temporariamente
2. Aumente Shake Intensity para `1.0`
3. Se ainda não funcionar, verifique se as referências em PlayerHealth estão corretas
4. Confirme que o método `TakeDamage` está sendo chamado (adicione um `Debug.Log`)

---

Para mais detalhes, veja o arquivo completo: `DAMAGE_FEEDBACK_SETUP.md`
