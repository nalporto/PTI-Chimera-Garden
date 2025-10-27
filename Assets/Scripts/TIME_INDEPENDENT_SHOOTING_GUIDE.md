# Time-Independent Shooting System

This guide explains how the shooting system now works independently of `Time.timeScale`, allowing you to shoot at normal speed even when time is slowed by the Magic Controller.

---

## 🎯 What Changed

### **Before**
- Shooting fire rate was tied to `Time.time` and `Time.deltaTime`
- When using Magic (E key) to slow time, shooting also became slower
- Bullet trails and hit markers were also affected by time scale

### **After**
- Shooting fire rate now uses `Time.unscaledTime`
- Fire rate remains constant regardless of `Time.timeScale`
- Bullet trails use `Time.unscaledDeltaTime` for smooth movement
- Hit markers use `WaitForSecondsRealtime` for consistent display

---

## ⚙️ Technical Details

### **1. Fire Rate Timing**

**Changed from:**
```csharp
if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
{
    Shoot();
    nextFireTime = Time.time + fireRate;
}
```

**Changed to:**
```csharp
if (Input.GetMouseButton(0) && Time.unscaledTime >= nextFireTime)
{
    Shoot();
    nextFireTime = Time.unscaledTime + fireRate;
}
```

**Why:** `Time.unscaledTime` continues to advance at real-world speed even when `Time.timeScale = 0.3f`.

---

### **2. Bullet Trail Movement**

**Changed from:**
```csharp
time += Time.deltaTime / trail.time;
```

**Changed to:**
```csharp
time += Time.unscaledDeltaTime / trail.time;
```

**Why:** Bullet trails now move at normal speed during slow motion, creating a dramatic "bullet time" effect where YOUR bullets travel normally but the world moves slowly.

---

### **3. Hit Marker Display**

**Changed from:**
```csharp
yield return new WaitForSeconds(hitMarkerDisplayTime);
```

**Changed to:**
```csharp
yield return new WaitForSecondsRealtime(hitMarkerDisplayTime);
```

**Why:** Hit markers now show for the correct duration regardless of time scale.

---

## 🎮 How It Works Now

### **Normal Time (Time.timeScale = 1.0)**
- Shoot at your configured fire rate (default: 0.3s between shots)
- Everything behaves normally

### **Slow Time (Hold E, Time.timeScale = 0.3)**
- **YOU** still shoot at the same fire rate (0.3s between shots)
- **Enemies** move at 30% speed
- **Physics** runs at 30% speed
- **Your bullets** travel at normal speed
- **Reload** still takes the same real-time duration (uses `WaitForSecondsRealtime`)

---

## 💡 Gameplay Impact

This creates a **tactical advantage** when using the Magic time slow:

### **Advantages:**
- ✅ Maintain normal shooting speed
- ✅ Enemies move slowly (easier to track)
- ✅ Bullets travel normally (instant feedback)
- ✅ More time to aim between enemy movements
- ✅ Dramatic "bullet time" effect

### **Balancing:**
- ⚖️ Energy drains while slowing time (25/second)
- ⚖️ Limited duration based on energy pool
- ⚖️ Energy recovers at 10/second when not in use

---

## 🔧 Already Time-Independent Systems

These systems were **already** using unscaled time:

### **Reload System**
```csharp
yield return new WaitForSecondsRealtime(reloadTime);
```
- Reloads take the same real-world time regardless of time scale
- This prevents exploiting time slow for instant reloads

### **Animation Delays**
```csharp
yield return new WaitForSecondsRealtime(switchInDelay);
yield return new WaitForSecondsRealtime(shootingBoolDuration);
```
- Weapon switch and animation states work correctly during time slow

---

## 🎬 Cinematic Effect

The combination creates a **Max Payne / Matrix style** effect:

1. **Hold E** to activate time slow
2. **World slows** to 30% speed
3. **You shoot** at normal speed
4. **Bullets fly** at normal speed through slow-motion enemies
5. **Energy drains** forcing tactical use
6. **Release E** to return to normal time

---

## 🛠️ Customization

### **Adjust Fire Rate**
In the `Shooter` component Inspector:
```
Fire Rate: 0.3  // Seconds between shots (at normal time)
```

### **Adjust Time Slow Amount**
In the `MagicController` component Inspector:
```
Slow Time Scale: 0.3  // 0.1 = very slow, 0.5 = half speed, 1.0 = normal
```

### **Adjust Energy Settings**
```
Max Energy: 100
Energy Drain Rate: 25  // Per second while active
```
With these defaults:
- 100 / 25 = **4 seconds** of slow time per full energy bar
- Recovers at 10/second = **10 seconds** to fully recharge

---

## 📊 Time.timeScale Reference

Here's what happens to different Unity time values:

| Unity Time Property | During Slow Time (0.3x) | What Uses It |
|---------------------|-------------------------|--------------|
| `Time.time` | Advances at 30% speed | Physics, animations, most game systems |
| `Time.deltaTime` | ~0.005s per frame | Physics updates, movement |
| `Time.unscaledTime` | ✅ Advances normally | **Your shooting**, UI timers |
| `Time.unscaledDeltaTime` | ✅ ~0.0167s per frame | **Bullet trails**, UI animations |
| `Time.fixedDeltaTime` | 0.3 × normal | Physics simulation |

---

## ⚠️ Systems Still Affected by Time Scale

These systems **intentionally** remain affected:

### **Enemy Movement & AI**
- Enemies move at 30% speed during time slow
- Navigation and behavior updates are slower
- **Why:** This is the tactical advantage

### **Physics**
- Rigidbodies move at 30% speed
- Gravity applies at 30% strength
- **Why:** Creates dramatic slow-motion effect

### **Particle Systems** (set to Scaled)
- Most effects play at 30% speed
- **Why:** Visual consistency with slow motion

### **Animations** (if using Animator.speed = 1)
- Character animations play at 30% speed
- **Why:** Matches world time scale

---

## 🎯 Best Practices

### **Do:**
- ✅ Use time slow when surrounded by enemies
- ✅ Activate before entering dangerous areas
- ✅ Let energy recharge between encounters
- ✅ Combine with dashing for maximum mobility

### **Don't:**
- ❌ Leave time slow on constantly (energy drains)
- ❌ Rely on it as the only combat strategy
- ❌ Forget it drains energy while active

---

## 🐛 Troubleshooting

### **Shooting still feels slow**

**Check:** Your `Fire Rate` setting
- Lower = faster shooting
- 0.1 = very fast
- 0.5 = slower
- Current default: **0.3 seconds**

**Solution:** Adjust in Inspector or code:
```csharp
[SerializeField] private float fireRate = 0.2f; // Faster
```

---

### **Time slow doesn't activate**

**Check Console** for errors related to MagicController

**Verify:**
1. `MagicController` is attached to your Magic object
2. Energy is above 0 (check Energy UI)
3. Pressing and **holding** `E` key

---

### **Bullets move too fast/slow**

The bullets are **hitscan** (instant), but the **trails** animate.

**To adjust trail speed:**
1. Select your `bulletTrailPrefab`
2. Adjust `Trail Renderer > Time` (duration)
3. Lower = faster trails (e.g., 0.1)
4. Higher = slower trails (e.g., 0.5)

---

## 🎨 Visual Enhancements (Optional)

### **Add Slow Motion Audio Effect**

In `MagicController.cs`, add audio pitch change:

```csharp
[SerializeField] private AudioSource musicAudioSource;

void Start()
{
    musicAudioSource = GameObject.Find("MusicHandler")?.GetComponent<AudioSource>();
}

// In the time slow activation:
if (musicAudioSource != null)
    musicAudioSource.pitch = 0.7f;

// In StopSlowTime():
if (musicAudioSource != null)
    musicAudioSource.pitch = 1f;
```

### **Add Chromatic Aberration**

Use URP Post-Processing:
1. Add Volume to scene
2. Add Chromatic Aberration override
3. Increase intensity when time slows

### **Screen Vignette**

Add Vignette to Post-Processing Volume:
```csharp
// Darken edges during time slow for focus
vignette.intensity.value = 0.4f;
```

---

## 🎯 Summary

Your shooting system now operates **independently** of time scale:

| Feature | Status | Notes |
|---------|--------|-------|
| Fire Rate | ✅ Time-independent | Uses `Time.unscaledTime` |
| Reload Speed | ✅ Time-independent | Uses `WaitForSecondsRealtime` |
| Bullet Trails | ✅ Time-independent | Uses `Time.unscaledDeltaTime` |
| Hit Markers | ✅ Time-independent | Uses `WaitForSecondsRealtime` |
| Damage | ✅ Time-independent | Instant hitscan |
| Animations | ⏰ Partial | Switch delays independent, triggers scaled |

**Result:** A smooth, tactical slow-motion combat system where you maintain full control while enemies struggle in slowed time!

---

Enjoy your new bullet-time abilities! 🎯⏱️
