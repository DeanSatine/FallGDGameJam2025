# Sandwich System - How It Works

## 🥪 Overview

The sandwich system is simple and intuitive:
1. **Make sandwiches** at a station (E key)
2. **Each press = +1 sandwich** to inventory
3. **Sandwich appears in hand** immediately
4. **UI shows total count** 
5. **Throw to attack** (Left-click)
6. **Auto-reload** when you throw

---

## 🎮 Player Experience

### Making Sandwiches
```
Player at Station → Press E → +1 Sandwich → Appears in Hand → UI Updates
         ↓
    Press E again → +2 Sandwiches → Still holding 1 → UI shows 2
         ↓
    Press E again → +3 Sandwiches → Still holding 1 → UI shows 3
```

### Throwing Sandwiches
```
Player has 3 sandwiches → Throw (Left-click) → -1 Sandwich → New one appears
         ↓
    UI shows 2 → Throw again → -1 Sandwich → New one appears
         ↓
    UI shows 1 → Throw again → -1 Sandwich → Hand becomes empty
         ↓
    UI shows 0 → Can't throw → Need to make more!
```

---

## 📊 Technical Flow

### When Player Presses E at Station:

1. **Interactable.cs** detects interaction
2. Calls `player.AddSandwiches(1)` → Adds 1 to inventory
3. Calls `player.CreateSandwichInHand()` → Spawns sandwich model in hand
4. Plays creation sound via AudioManager
5. UIManager updates sandwich count display

### When Player Throws Sandwich:

1. **PlayerController.cs** detects Left-click
2. Checks if `currentSandwiches > 0`
3. Detaches sandwich from hand
4. Adds physics and SandwichProjectile component
5. Decrements `currentSandwiches--`
6. Updates UI
7. If `currentSandwiches > 0` → Auto-creates new sandwich in hand
8. If `currentSandwiches == 0` → Sets `heldSandwich = null`

---

## 🔧 Key Components

### PlayerController.cs
```csharp
[SerializeField] private int currentSandwiches = 0;    // Total in inventory
private GameObject heldSandwich;                        // Visual in hand

// Called by Interactable when pressing E
public void AddSandwiches(int amount)
{
    currentSandwiches += amount;
    uiManager.UpdateSandwichCount(currentSandwiches);
}

// Creates visible sandwich in hand
public void CreateSandwichInHand()
{
    if (heldSandwich == null && sandwichPrefab != null)
    {
        CreateSandwich();  // Instantiates at holdPoint
    }
}
```

### Interactable.cs
```csharp
[SerializeField] private int sandwichesGiven = 1;      // 1 per press

public void Interact(PlayerController player)
{
    player.AddSandwiches(sandwichesGiven);             // Add to inventory
    player.CreateSandwichInHand();                      // Show in hand
    AudioManager.Instance.PlayCreateSandwichSound();    // Sound feedback
}
```

---

## 🎨 UI Integration

### UIManager.cs
```csharp
[SerializeField] private TextMeshProUGUI sandwichCountText;

public void UpdateSandwichCount(int count)
{
    sandwichCountText.text = $"Sandwiches: {count}";
}
```

**Setup in Unity:**
1. Create UI Text element
2. Assign to UIManager's "Sandwich Count Text" field
3. Position in HUD (top-left, top-right, etc.)

---

## ✅ Design Benefits

### Player Satisfaction
- ✅ **Instant Feedback** - See sandwich appear immediately
- ✅ **Clear Counter** - Always know how many you have
- ✅ **No Confusion** - One press = one sandwich (simple!)
- ✅ **Smooth Flow** - Auto-reload keeps action flowing

### Gameplay Depth
- ✅ **Resource Management** - Players must balance making vs. fighting
- ✅ **Risk/Reward** - Leave combat to make more sandwiches?
- ✅ **Strategic Stations** - Station placement matters
- ✅ **Tension** - Running low creates urgency

---

## 🎯 Balancing Tips

### Making Sandwiches Faster
```csharp
// In PlayerController.cs
[SerializeField] private float sandwichCreationTime = 0.5f;
```
- Lower value = faster creation
- Consider 0.3f for arcade feel
- Consider 1.0f for tactical gameplay

### Sandwich Capacity
```csharp
// In PlayerController.cs
[SerializeField] private int maxSandwiches = 999;
```
- Set to 10-20 for resource management challenge
- Set to 999 for unlimited inventory
- Balance around station accessibility

### Station Output
```csharp
// In Interactable.cs
[SerializeField] private int sandwichesGiven = 1;
```
- Keep at 1 for consistent pacing
- Increase to 3-5 for "power stations"
- Create different station types with different outputs

---

## 🐛 Troubleshooting

### "Getting multiple sandwiches per press"
- This has been fixed! Now uses `context.performed` check
- Only triggers once per button press
- If still happening, check Input Actions aren't calling started/performed/canceled all at once

### "Sandwich doesn't appear in hand"
- Check `holdPoint` is assigned in PlayerController
- Verify `sandwichPrefab` is assigned
- Ensure CreateSandwich() is being called

### "UI doesn't update"
- Check `sandwichCountText` is assigned in UIManager
- Verify UIManager reference in PlayerController
- Check UpdateSandwichCount() is called

### "Can't throw sandwiches"
- Verify `currentSandwiches > 0`
- Check heldSandwich is not null
- Ensure Input Actions has Throw bound to Left Mouse

### "Sandwiches accumulate infinitely"
- This is intended! MaxSandwiches is 999 by default
- Set lower limit if you want resource management
- Add cost system (pay for sandwiches?)

---

## 🚀 Advanced Features

### Add Sandwich Types
```csharp
public enum SandwichType { Normal, Explosive, Freezing }

[SerializeField] private SandwichType sandwichType;
```

### Add Making Animation
```csharp
// In CreateSandwichCoroutine()
animator.SetTrigger("MakeSandwich");
yield return new WaitForSeconds(animationLength);
```

### Add Combo System
```csharp
private int throwCombo = 0;
private float comboTimer = 0f;

// On successful hit
throwCombo++;
comboTimer = 3f;  // Reset timer
```

### Add Reload Sounds
```csharp
// In CreateSandwichCoroutine()
AudioManager.Instance.PlayReloadSound();
```

You already have `/Assets/Sound/Reload.wav` - perfect for this!

---

**The sandwich system is ready! Players can spam E to make sandwiches and fight with unlimited ammo as long as they visit stations! 🥖⚔️**
