# Guia Rápido - Barra de Vida

## ⚡ Setup em 2 Passos

### Passo 1: Criar Barra Automaticamente

1. **Selecione o Canvas** na Hierarchy
2. Menu Unity:
   ```
   GameObject > UI > Health Bar System
   ```

Pronto! ✅

---

### Passo 2: Testar

1. Enter Play Mode
2. Tome dano
3. A barra encolhe e muda de cor! 🎉

---

## 🎨 Resultado Visual

```
Health: 100
██████████████████████  (100% - Verde)

Health: 50
███████████░░░░░░░░░░░  (50% - Amarelo)

Health: 20
████░░░░░░░░░░░░░░░░░░  (20% - Vermelho)
```

---

## 🔧 Personalização Rápida

**No Canvas > HealthUI:**

### Mudar Cores
```
Full Health Color: Verde
Medium Health Color: Amarelo  
Low Health Color: Vermelho
```

### Ajustar Limites
```
Medium Health Threshold: 50
Low Health Threshold: 25
```

### Velocidade de Transição
```
Smooth Transition: ✅
Transition Speed: 5 (maior = mais rápido)
```

---

## 📍 Mudar Posição da Barra

Selecione **HealthPanel** e mude o Anchor Preset:

- **Top Left** = Canto superior esquerdo (padrão)
- **Top Right** = Canto superior direito
- **Bottom Left** = Canto inferior esquerdo

---

## ✨ Recursos

✅ Transição suave da barra  
✅ Muda cor por nível de HP  
✅ Mostra HP no texto  
✅ Funciona automaticamente com PlayerHealth  

---

## 🐛 Problema?

**Barra não aparece?**
- Verifique se PlayerHealth está atribuído no Canvas > HealthUI

**Barra não encolhe?**
- Confirme que Health Bar Fill está atribuído no HealthUI

---

Para documentação completa, veja `HEALTH_BAR_SETUP.md`!
