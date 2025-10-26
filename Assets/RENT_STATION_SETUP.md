# Rent Station - Quick Setup Guide

## 📋 What You Need

The RentStation is an interactable object that allows players to pay rent at the end of each day. Without paying rent, the game ends!

---

## 🔧 Setup Steps

### 1. Create the Rent Station GameObject

Choose one of these options:

**Option A: Use existing object** (recommended)
- Find a logical object in your scene (cash register, door, safe, etc.)
- This object should be near spawn or easily accessible

**Option B: Create new object**
1. Right-click in Hierarchy → 3D Object → Cube
2. Name it "RentStation"
3. Position it in an accessible location
4. Add visual model (optional but recommended)

---

### 2. Add Required Components

1. **Select your RentStation GameObject**
2. **Add RentStation Script**:
   - Click "Add Component"
   - Search for "RentStation"
   - Click to add
3. **Ensure it has a Collider**:
   - If using primitive (Cube, Sphere), collider is automatic
   - If using custom model, add Box Collider or Mesh Collider
   - Make sure "Is Trigger" is **UNCHECKED**

---

### 3. Configure RentStation Component

In the Inspector:

```
RentStation (Script)
├─ Interaction Prompt: "Press E to Pay Rent"
├─ Insufficient Funds Prompt: "Not Enough Money!"
└─ Is Rent Due: false (unchecked)
```

**Note**: "Is Rent Due" is controlled by code, leave it unchecked.

---

### 4. Set the Correct Layer

1. With RentStation selected, look at top of Inspector
2. Find **Layer** dropdown (next to Tag)
3. Set to **Default** (or whatever layer matches PlayerController's Interaction Layer)

---

### 5. Link to GameManager

1. **Find GameManager** in Hierarchy
2. **Select it** to see Inspector
3. **Find "Rent Station" field** under References section
4. **Drag your RentStation GameObject** into this field

---

## 🎮 How It Works

### Game Flow
1. Player completes a day (kills all enemies)
2. GameManager calls `EndRound()`
3. Day End panel shows rent cost
4. RentStation's `isRentDue` becomes `true`
5. Player finds RentStation and looks at it
6. Prompt shows: "Press E to Pay Rent ($X)"
7. Player presses E
8. If affordable: Rent paid, next day starts, `isRentDue` becomes `false`
9. If not affordable: Game Over

### Dynamic Prompts

The RentStation shows different messages:

| Situation | Prompt |
|-----------|--------|
| Rent not due yet | "Rent not due yet" |
| Can afford rent | "Press E to Pay Rent ($50)" |
| Can't afford rent | "Not Enough Money! (30/50)" |

---

## 🎨 Visual Feedback (Optional)

Make your RentStation visually obvious:

### Add a Light
1. Right-click RentStation → Light → Point Light
2. Set color to green (can afford) or red (can't afford)
3. Use code to toggle color based on rent status

### Add a Sign
1. Create 3D Text or TextMeshPro
2. Position above RentStation
3. Set text to "RENT PAYMENT HERE"

### Add Particles
1. Right-click RentStation → Effects → Particle System
2. Make it glow or shimmer when rent is due

---

## 📝 Testing Checklist

- [ ] RentStation GameObject exists in scene
- [ ] Has RentStation component
- [ ] Has Collider (not trigger)
- [ ] Layer is set correctly (Default or interaction layer)
- [ ] Assigned to GameManager's Rent Station field
- [ ] Can see interaction prompt when looking at it (after day ends)
- [ ] Pressing E pays rent and starts next day
- [ ] Game Over triggers if you can't afford rent

---

## 🐛 Common Issues

### "No interaction prompt appears"
- Check RentStation is assigned in GameManager
- Verify Collider exists and is enabled
- Confirm layer matches PlayerController's Interaction Layer
- Ensure `isRentDue` is true (happens after day ends)

### "Prompt shows but E doesn't work"
- Make sure RentStation script is attached
- Check GameManager reference is set
- Verify you have enough points to pay rent

### "Rent is always due"
- This shouldn't happen - check GameManager's PayRent() method
- Make sure `rentStation.SetRentDue(false)` is called after payment

### "Multiple RentStations in scene"
- GameManager only uses one RentStation
- You can have multiples, but assign the main one to GameManager
- All RentStations will activate when rent is due

---

## 🎯 Advanced: Multiple Payment Locations

Want players to pay rent at different locations?

1. Create multiple RentStation objects
2. Assign ONE to GameManager (this is the "official" one)
3. Other RentStations will show prompts but won't track state
4. OR: Modify RentStation.cs to reference a shared static state

---

## 💡 Tips

- **Place near spawn** so players can easily find it after each day
- **Make it visually distinct** with unique materials or lighting
- **Add audio feedback** when rent is paid (already handled by AudioManager)
- **Test the flow** by playing through a full day and paying rent
- **Increase visibility** with glowing materials or particle effects

---

**Your RentStation is ready! Players can now pay rent to survive another day! 💰**
