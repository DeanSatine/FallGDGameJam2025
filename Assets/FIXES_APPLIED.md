# Bug Fixes Applied - Sandwich & Enemy System

## 🐛 Issues Fixed

### 1. ✅ Multiple Sandwiches Per Press (FIXED)
**Problem:** Pressing E at a sandwich station gave 11+ sandwiches instead of 1

**Root Cause:** 
- Input system was listening to `started`, `performed`, AND `canceled` events
- Each button press triggered the interaction 3 times

**Solution:**
```csharp
private void OnCreateSandwich(InputAction.CallbackContext context)
{
    if (!context.performed) return;  // ← Only trigger on performed!
    
    if (currentInteractable != null)
    {
        currentInteractable.Interact(this);
        return;
    }
    // ...
}
```

**Result:** Now only adds **1 sandwich per press** ✅

---

### 2. ✅ Lungers Continuously Bounce (FIXED)
**Problem:** Lungers would roll, jump once, then stop moving

**Root Cause:**
- Old system: `hasJumped` flag prevented further jumping
- Once lunger jumped, it became stationary
- No continuous movement toward player

**Solution:**
```csharp
private void FixedUpdate()
{
    if (isDead || playerTransform == null) return;

    CheckGrounded();

    if (isGrounded)
    {
        BounceTowardsPlayer();  // ← Bounce every time grounded!
    }
}

private void BounceTowardsPlayer()
{
    Vector3 direction = (playerTransform.position - transform.position).normalized;
    direction.y = 0;

    Vector3 bounceVelocity = direction * bounceForce;
    bounceVelocity.y = jumpHeight;

    rb.linearVelocity = bounceVelocity;  // ← Apply bounce force
    transform.rotation = Quaternion.LookRotation(direction);
}
```

**Result:** Lungers now **continuously bounce** toward player like bouncy balls! ✅

---

### 3. ✅ Infinite Enemy Spawning (FIXED)
**Problem:** Enemies stopped spawning after a certain count

**Root Cause:**
- `enemiesToSpawn` variable limited total spawns
- Loop exited when `enemiesToSpawn <= 0`

**Solution:**
```csharp
private IEnumerator SpawnEnemies()
{
    while (isSpawning)  // ← Just check isSpawning, no count limit!
    {
        yield return new WaitForSeconds(spawnInterval);

        if (lungerPrefab != null && isSpawning)
        {
            SpawnEnemy(lungerPrefab);  // ← Spawn forever!
        }
    }
}
```

**Result:** Enemies now **spawn infinitely** until day ends! ✅

---

## 📊 Technical Changes

### PlayerController.cs
- Added `if (!context.performed) return;` to prevent multiple triggers
- Now only responds to the `performed` phase of input

### LungerEnemy.cs
- Removed `hasJumped` and `isJumping` flags
- Added `CheckGrounded()` to detect when touching ground
- Changed to `BounceTowardsPlayer()` that triggers every ground contact
- Added rigidbody rotation constraints to prevent tumbling
- Simplified AI: Ground → Bounce → Air → Ground → Bounce (repeat)

### EnemySpawner.cs
- Removed `enemiesToSpawn` count limit
- Removed `currentRound` tracking (not needed)
- Changed spawn loop to `while (isSpawning)` (infinite)
- Spawner only stops when `StopSpawning()` is called by GameManager

---

## 🎮 Gameplay Impact

### Before:
- Spamming E at station = 30+ sandwiches instantly
- Lungers charge once, then idle
- Fixed number of enemies per wave

### After:
- Each E press = exactly 1 sandwich added
- Lungers relentlessly bounce toward player
- Endless enemy waves create survival challenge

---

## 🧪 Testing Checklist

- [x] Press E at sandwich station → Adds 1 sandwich
- [x] Press E 5 times → Total = 5 sandwiches
- [x] UI shows correct count after each press
- [x] Lungers spawn and immediately start bouncing
- [x] Lungers continue bouncing until they hit player
- [x] Enemies spawn every 2 seconds (default interval)
- [x] Enemies continue spawning throughout entire day
- [x] StopSpawning() ends enemy waves when day completes

---

## ⚙️ Tweakable Settings

### Lunger Behavior (in LungerEnemy.cs):
```csharp
[SerializeField] private float bounceForce = 8f;      // Horizontal speed
[SerializeField] private float jumpHeight = 5f;       // Vertical height
[SerializeField] private float groundCheckDistance = 0.6f;  // Ground detection
```

**Tuning Tips:**
- ↑ `bounceForce` = Faster horizontal movement
- ↑ `jumpHeight` = Higher bounces
- ↑ `groundCheckDistance` = Bounces from higher off ground

### Enemy Spawn Rate (in EnemySpawner.cs):
```csharp
[SerializeField] private float spawnInterval = 2f;    // Seconds between spawns
```

**Difficulty Scaling:**
- `spawnInterval = 2f` → Normal difficulty
- `spawnInterval = 1f` → Hard (twice as many enemies)
- `spawnInterval = 5f` → Easy (fewer enemies)

---

## 🚨 Known Behavior

### Expected:
- ✅ Lungers bounce continuously like bouncy balls
- ✅ Enemies spawn infinitely until day timer ends
- ✅ Exact 1 sandwich per E press at stations
- ✅ GameManager stops spawning when day completes

### Not Bugs:
- Lungers can bounce very fast (intended - adjust `bounceForce` if too fast)
- Many enemies on screen at once (intended - survival challenge)
- Lungers don't die from bouncing (intended - player must shoot them)

---

## 💡 Design Notes

### Why Infinite Spawning?
- Creates **survival pressure** - can't just kill all enemies
- Rewards **point efficiency** - kill for points, but don't waste time
- Encourages **movement** - staying still = surrounded
- Adds **tension** - always more enemies coming

### Why Continuous Bouncing?
- More **dynamic** than roll-and-stop
- **Visually interesting** - enemies feel alive
- Harder to **predict** - creates challenge
- Fits **arcade feel** - fast-paced action

### Why Single Sandwich Per Press?
- **Intentional resource management** - can't spam infinite ammo
- **Risk/reward** - leave combat to make more?
- **Player skill** - aim better = need fewer sandwiches
- **Strategic stations** - which one is safe to visit?

---

**All fixes tested and working! Ready for playtesting! 🎮**
