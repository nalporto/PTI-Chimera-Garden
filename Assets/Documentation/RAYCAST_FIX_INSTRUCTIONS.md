# Instruções para Corrigir Problemas de Raycast e Colisão

## Problema Corrigido
Os tiros não estavam acertando os inimigos corretamente devido a problemas com layers, raycast e colisões.

## Alterações Realizadas nos Scripts

### 1. Shooter.cs (Tiros do Jogador)
**Melhorias implementadas:**
- Mudança de `QueryTriggerInteraction.Collide` para `QueryTriggerInteraction.Ignore` para evitar acertar triggers
- Layer mask agora ignora as layers "Player" e "Weapons" para evitar acertar objetos errados
- Simplificação da lógica de detecção de HitReceiver (removido código redundante)
- Logs melhorados mostrando nome da layer ao invés do número
- Log de distância do tiro para debugging

### 2. EnemyProjectile.cs (Tiros dos Inimigos)
**Melhorias implementadas:**
- Adicionado Linecast para detecção contínua de colisão (evita atravessar paredes em alta velocidade)
- Layer mask ignora "Enemy" e "Weapons" para evitar acertar outros inimigos
- Ignora colisões com a layer "Enemy" no Start()
- Mantido OnTriggerEnter como fallback
- Logs melhorados para debugging

## Configurações Recomendadas no Unity Editor

### Passo 1: Verificar Tags dos Inimigos
1. Selecione todos os GameObjects de inimigos na hierarquia
2. Verifique se a tag está definida como "Enemy"
3. Certifique-se que o componente `HitReceiver` está no mesmo GameObject ou em um pai

### Passo 2: Configurar Layers (OPCIONAL mas RECOMENDADO)
Atualmente os inimigos estão na layer "Default". Considere criar uma layer "Enemy" dedicada:

1. Vá em Edit > Project Settings > Tags and Layers
2. Crie uma nova layer chamada "Enemy" (se ainda não existir)
3. Selecione todos os inimigos na cena
4. Defina a Layer para "Enemy"

### Passo 3: Configurar Layer Collision Matrix
1. Vá em Edit > Project Settings > Physics
2. Role até "Layer Collision Matrix"
3. Configure as seguintes interações:
   - ✅ Player x Enemy (deve colidir)
   - ✅ Player x Ground (deve colidir)
   - ✅ Weapons x Enemy (deve colidir)
   - ✅ Weapons x Ground (deve colidir)
   - ❌ Weapons x Player (NÃO deve colidir - projéteis do inimigo)
   - ❌ Enemy x Enemy (NÃO deve colidir - evita inimigos bloquearem uns aos outros)

### Passo 4: Verificar Colliders dos Inimigos
1. Selecione um inimigo
2. Verifique se o CapsuleCollider está configurado:
   - `Is Trigger`: DESABILITADO (false)
   - `Center`, `Radius`, `Height` cobrem o modelo corretamente
3. Se houver múltiplos colliders, certifique-se que pelo menos um não é trigger

### Passo 5: Verificar Configuração dos Projéteis
Para os projéteis dos inimigos:
1. Verifique se o prefab do projétil tem um Collider
2. Configure o Collider como `Is Trigger`: HABILITADO (true)
3. Certifique-se que o projétil está em uma layer apropriada (ex: "Weapons")

## Testando as Correções

### Teste 1: Tiros do Jogador
1. Entre em Play Mode
2. Atire em um inimigo
3. Verifique o Console:
   - Deve aparecer: "Bullet HIT: [nome] (layer Default) at distance X.XXm - Tag: Enemy"
   - Deve aparecer: "Damage applied to [nome]"
   - A linha de debug deve ser VERDE (acerto em Enemy)

### Teste 2: Tiros dos Inimigos
1. Entre em Play Mode
2. Deixe um inimigo atirar em você
3. Verifique se:
   - O projétil NÃO atravessa paredes (tag "Ground" ou "Wall")
   - O projétil acerta o jogador e causa dano
   - Aparece log no Console confirmando o acerto

### Teste 3: Colisão com Paredes
1. Atire em uma parede
2. Verifique se:
   - A linha de debug é AZUL (Ground) ou AMARELA (outros)
   - O efeito de impacto aparece
   - Não há mensagens de erro

## Problemas Conhecidos e Soluções

### Problema: Tiros ainda atravessam paredes
**Solução:**
- Verifique se as paredes têm colliders não-trigger
- Certifique-se que as paredes têm a tag "Ground" ou "Wall"
- Verifique o Layer Collision Matrix

### Problema: Inimigos não tomam dano
**Solução:**
- Verifique se o GameObject do inimigo tem a tag "Enemy"
- Certifique-se que HitReceiver está no GameObject raiz do inimigo
- Use o log do Console para verificar qual objeto está sendo atingido

### Problema: Projéteis dos inimigos atravessam paredes
**Solução:**
- Reduza a velocidade do projétil (field `speed`)
- Certifique-se que as paredes têm colliders apropriados
- Verifique se a tag "Wall" está aplicada nas paredes

### Problema: Performance ruim
**Solução:**
- O Linecast em EnemyProjectile pode causar overhead se houver muitos projéteis
- Considere usar um pool de objetos para projéteis
- Limite o número máximo de projéteis ativos simultaneamente

## Próximos Passos Sugeridos

1. Implementar sistema de health bar para inimigos
2. Adicionar feedback visual de dano (flash vermelho)
3. Implementar sistema de hit markers melhorado
4. Adicionar sons de impacto diferentes para cada tipo de superfície
5. Implementar sistema de dano por área (headshot, corpo, etc.)

## Notas Técnicas

- **Raycast vs Linecast**: Shooter usa Raycast (direção infinita com alcance máximo), EnemyProjectile usa Linecast (entre dois pontos específicos)
- **QueryTriggerInteraction.Ignore**: Importante para evitar que raycasts acertem colliders configurados como trigger
- **Layer Mask (~LayerMask.GetMask(...))**: O operador ~ inverte a máscara, então estamos ignorando as layers especificadas
- **GetComponentInParent**: Busca o componente no GameObject atual e em todos os pais na hierarquia
