# Subway Flash - Audio, Interaction & Visual Effects Setup Guide

## 🎯 Recent Updates

### ✅ Enemy Spawning Changes
- **Removed Thrower enemies** - Only Lunger enemies spawn now
- **Simplified spawning** - Single spawn interval for all enemies
- Adjust `Spawn Interval` in EnemySpawner to control difficulty

### ✅ Aiming & Throwing Improvements
- **Right-click to aim** - Zooms camera for precise throwing
- **Center screen aiming** - Throws go exactly where camera is pointing
- **Smooth zoom transition** - FOV smoothly transitions between normal and aim

### ✅ Sandwich System Improvements
- **Sandwiches stay visible** - After throwing, if you have more sandwiches, a new one automatically appears in your hand
- **No more disappearing** - Your hand always shows a sandwich when you have inventory

### ✅ Rent Payment System
- **RentStation object** - New interactable object for paying rent
- **Press E to pay** - Look at the RentStation and press E when rent is due
- **Dynamic prompts** - Shows rent cost and if you can afford it
- **Game Over trigger** - If you can't pay rent, game ends

---

## ✅ What Was Created

### New Scripts:
1. **AudioManager.cs** - Handles music, jingles, and sound effects
2. **Interactable.cs** - For objects player can interact with
3. **SkyboxTransition.cs** - Gradual skybox transitions between days

### Updated Scripts:
1. **PlayerController.cs** - Added interaction system and sandwich management
2. **UIManager.cs** - Added fade effects, interaction prompts, sandwich counter
3. **GameManager.cs** - Integrated AudioManager and SkyboxTransition

---

## 🎵 1. AudioManager Setup

### Step 1: Create AudioManager GameObject
1. Create empty GameObject named `AudioManager`
2. Add `AudioManager` script
3. AudioManager will auto-create 3 child AudioSources:
   - MusicSource
   - JingleSource
   - SFXSource

### Step 2: Assign Audio Clips
In AudioManager Inspector, assign your audio clips:

**Music:**
- Gameplay Theme (looping background music)

**Jingles:**
- Day Start Jingle (plays when new day starts)
- Game Over Jingle (plays on game over)

**Sound Effects:**
- Enemy Hit Sound
- Enemy Kill Sound
- Throw Sound
- Create Sandwich Sound
- Player Hit Sound

### Step 3: Audio Volumes
Adjust these settings:
- Music Volume: 0.5
- Jingle Volume: 0.7
- SFX Volume: 0.6

---

## 🥪 2. Interaction System Setup

### Step 1: Create Sandwich Station (Interactable Object)

1. **Create GameObject** (e.g., Cube or your custom model)
   - Name it `SandwichStation`
   - Position it in your scene
   - Add a Collider (if not already present)

2. **Add Interactable Script**
   - Add Component → `Interactable`
   - Set "Interaction Prompt" to: `"Press E to make sandwiches"`
   - Set "Sandwiches Given" to: `3`

3. **Configure Layer** (Optional but recommended)
   - Create a new Layer called "Interactable"
   - Set SandwichStation's layer to "Interactable"

### Step 2: Update Player Settings

Select your **Player** GameObject:

**PlayerController Settings:**
- Current Sandwiches: `0` (starts with none)
- Max Sandwiches: `999`
- Interaction Range: `3`
- Interaction Layer: Set to "Everything" or "Interactable" layer

---

## 🎨 3. UI Updates

### New UI Elements Needed:

#### A. Sandwich Counter (HUD)
1. Right-click Canvas → **UI → Text - TextMeshPro**
2. Name: `SandwichCountText`
3. Position: Top-right corner or near other HUD elements
4. Text: "Sandwiches: 0"

#### B. Interaction Prompt
1. Right-click Canvas → **UI → Panel**
2. Name: `InteractionPromptPanel`
3. Position: Center-bottom of screen
4. Make semi-transparent background
5. Add child: **UI → Text - TextMeshPro**
   - Name: `InteractionPromptText`
   - Text: "Press E to interact"
   - Center align

#### C. Fade Images (for Day transitions)
1. Open `DayStartPanel` in Hierarchy
2. Right-click → **UI → Image**
   - Name: `DayStartFadeImage`
   - Anchor: Stretch to fill screen
   - Color: Black (RGB: 0,0,0)
   - Alpha: 0
   - Move to bottom of hierarchy (renders first/behind)

3. Repeat for `DayEndPanel`
   - Name: `DayEndFadeImage`
   - Same settings

### Link to UIManager:

Select **UIManager** GameObject and assign:
- Sandwich Count Text → `SandwichCountText`
- Interaction Prompt Panel → `InteractionPromptPanel`
- Interaction Prompt Text → `InteractionPromptText`
- Day Start Fade Image → `DayStartFadeImage`
- Day End Fade Image → `DayEndFadeImage`
- Fade Duration: `1` (seconds)

---

## 🌅 4. Skybox Transition Setup

### Step 1: Create SkyboxTransition GameObject
1. Create empty GameObject named `SkyboxTransition`
2. Add `SkyboxTransition` script

### Step 2: Assign Skybox Materials
In SkyboxTransition Inspector:

**Skybox Materials (Array):**
- Size: 2 or more
- Element 0: Your first skybox (e.g., FS003)
- Element 1: Your second skybox (e.g., FS004)
- Element 2+: Additional skyboxes (optional)

**Settings:**
- Transition Duration: `3` seconds
- Rounds Per Transition: `3` (changes every 3 days)

### Step 3: Link to GameManager
Select **GameManager** GameObject:
- Skybox Transition → Drag `SkyboxTransition` GameObject

---

## 🎮 5. Testing Checklist

### Test Audio:
- ✅ Background music plays on game start
- ✅ Day jingle plays when round starts (music pauses)
- ✅ Music resumes after jingle
- ✅ Game over jingle plays on death/lose

### Test Interaction:
- ✅ Walk up to Sandwich Station
- ✅ "Press E to make sandwiches" appears
- ✅ Press E to get 3 sandwiches
- ✅ Sandwich count updates in UI
- ✅ Press E again to create sandwich (uses 1)
- ✅ Can throw sandwich with Left Click

### Test UI:
- ✅ Sandwich counter displays correctly
- ✅ Interaction prompt shows/hides properly
- ✅ Screen fades to black when day starts
- ✅ Day text appears during fade
- ✅ Screen fades back to normal

### Test Skybox:
- ✅ Skybox changes after 3 rounds
- ✅ Transition is smooth (3 seconds)
- ✅ Continues cycling through skyboxes

---

## 🔧 Common Issues

### "AudioManager.Instance is null"
- Make sure AudioManager GameObject exists in scene
- AudioManager should survive scene loads (DontDestroyOnLoad)

### Player can't interact
- Check Interaction Range is high enough (default 3)
- Verify Interactable object has a Collider
- Check Interaction Layer matches object's layer

### Fade images blocking UI
- Ensure fade images are at the BOTTOM of Canvas hierarchy
- Check alpha starts at 0
- Verify they're children of the correct panels

### Skybox not transitioning
- Ensure SkyboxTransition is linked in GameManager
- Check you have 2+ skybox materials assigned
- Verify materials are valid Skybox materials

### No interaction prompt showing
- Link InteractionPromptPanel and Text to UIManager
- Check panel starts deactivated (disabled)
- Verify PlayerController found UIManager

---

## 📝 Gameplay Flow

1. **Game Starts** → Background music plays
2. **Player spawns** → No sandwiches (must find station)
3. **Walk to Sandwich Station** → Prompt appears
4. **Press E at station** → Get 3 sandwiches, sound plays
5. **Press E elsewhere** → Creates sandwich in hand (uses 1)
6. **Left Click** → Throws sandwich
7. **Round completes** → Screen fades, day jingle plays
8. **Every 3 days** → Skybox gradually changes
9. **Game Over** → Music stops, game over jingle plays

---

## 🎨 Optional Enhancements

### Add More Interactables:
- Health station (heals player)
- Upgrade station (increase throw power)
- Shop (buy items with points)

### Audio Improvements:
- Add more sound variations
- Random pitch for variety
- Music layers that add/remove

### Visual Polish:
- Particle effects on interaction
- Glow effect on interactable objects
- Better fade effects (blur, color)

---

## 🚀 Quick Start Order

1. ✅ Create AudioManager → Assign audio clips
2. ✅ Create SandwichStation → Add Interactable script
3. ✅ Update UI → Add new text and panels
4. ✅ Link everything in UIManager Inspector
5. ✅ Create SkyboxTransition → Assign skyboxes
6. ✅ Link SkyboxTransition to GameManager
7. ✅ Test everything!

---

**Your game now has:**
- 🎵 Dynamic audio system
- 🥪 Interactive sandwich creation
- 💬 UI interaction prompts
- 🎬 Smooth day transitions with fades
- 🌅 Progressive skybox changes

Good luck with your game jam! 🎮
