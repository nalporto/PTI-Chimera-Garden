# Guia Rápido - UI de Pontos de Grapple

## Setup em 3 Passos (5 minutos)

### ⚡ Passo 1: Criar UI Automaticamente

**No menu Unity:**
```
GameObject > UI > Grapple Point Indicator System
```

Isso cria tudo automaticamente! ✅

---

### 🎯 Passo 2: Configurar seus Pontos de Grapple

1. **Selecione o Sphere (ou qualquer objeto que será ponto de grapple)**
   - Na Hierarchy: `/---MAP---/Sphere`

2. **Adicione o componente:**
   - Clique em "Add Component"
   - Digite: `GrapplePoint`
   - Pressione Enter

3. **Configure (valores recomendados):**
   ```
   Detection Radius: 15
   Show Gizmos: ✅ Habilitado
   Gizmo Color: Cyan
   ```

4. **IMPORTANTE:** Certifique-se que seu jogador tem a tag **"Player"**

---

### 🎮 Passo 3: Testar!

1. Enter Play Mode
2. Aproxime-se do ponto de grapple
3. Você verá o indicador aparecer e rotacionar! 🎉

---

## Personalizações Rápidas

### Rotação Mais Rápida
```
GrappleUIManager > Rotation Speed: 180
```

### UI Maior
```
GrappleUIManager > Scale Multiplier: 1.5
```

### Detecção Mais Longe
```
GrapplePoint > Detection Radius: 25
```

### Mudar Cor dos Brackets
```
Selecione: Canvas > GrappleUIManager > GrappleIndicator > TopLeft (e outros)
Image > Color: Escolha sua cor favorita!
```

---

## Adicionando Mais Pontos de Grapple

Para cada novo ponto:
1. Selecione o GameObject
2. Add Component > `GrapplePoint`
3. Pronto! ✅

---

## Troubleshooting Rápido

❌ **UI não aparece?**
- Verifique se jogador tem tag "Player"
- Aumente Detection Radius para 50 (teste)

❌ **UI não rotaciona?**
- Verifique GrappleUIManager > Rotation Speed > 0

❌ **UI fica no lugar errado?**
- Certifique-se que Canvas está em "Screen Space - Overlay"
- Verifique se Main Camera está atribuída no GrappleUIManager

---

## Próximos Passos

Consulte `GRAPPLE_UI_SETUP.md` para:
- Personalização avançada
- Integração com sistema de grapple
- Adicionar sons e partículas
- E muito mais!

---

✨ **Dica:** Use os gizmos cyan no Scene view para ajustar o Detection Radius visualmente!
