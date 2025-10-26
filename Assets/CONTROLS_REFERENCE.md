# Subway Flash - Controls Reference

## 🎮 Player Controls

### Movement
- **W/A/S/D** - Move forward/left/backward/right
- **Mouse** - Look around
- **Space** - Jump (if implemented)

### Combat
- **E** - Interact with sandwich station OR create sandwich (if you have sandwiches)
- **Left Mouse Button** - Throw sandwich
- **Right Mouse Button** - Aim (zooms in for precision)

### Interaction
- **E** - When looking at interactable objects:
  - **Sandwich Station**: Makes 1 sandwich (adds to inventory + appears in hand)
  - **Rent Station**: Pay rent (when due)
  - A prompt will appear on screen when you can interact

---

## 🥪 Sandwich System

### Making Sandwiches
1. **Find a Sandwich Station** (counter, table with Interactable component)
2. **Look at it** - "Press E to make sandwich" appears
3. **Press E** - Makes ONE sandwich, adds it to inventory AND puts it in your hand
4. **Press E multiple times** - Keep making sandwiches, each press adds 1 to your count
5. **UI shows total count** - "Sandwiches: X" displays your total inventory

### Throwing Sandwiches
1. **Left-click** - Throw the sandwich in your hand
2. **Auto-reload** - If you have more sandwiches, a new one appears automatically
3. **UI updates** - Count decreases with each throw
4. **Right-click while throwing** - Zoom to aim precisely

**How It Works:**
- Each press at station = +1 sandwich to inventory + 1 appears in hand
- Each throw = -1 sandwich from inventory
- Sandwich always visible in hand as long as inventory > 0

---

## 💰 Rent Payment System

### How It Works
1. **Complete the day** by killing all enemies
2. **Day End screen appears** showing rent cost
3. **Find the Rent Station** in the level
4. **Look at Rent Station** - Shows:
   - "Press E to Pay Rent ($X)" if you can afford it
   - "Not Enough Money! (X/Y)" if you can't
5. **Press E** - Pays rent and starts next day
6. **Game Over** if you can't afford rent

### Tips
- Rent increases each day
- Kill enemies to earn points
- Plan ahead - make sure you have enough for rent!

---

## 🎯 Aiming Tips

- **Right-click** reduces FOV from 60° to 40° for precise aiming
- **Throws go exactly center screen** - where crosshair points
- **Hold right-click** while throwing for best accuracy
- **Release right-click** to return to normal view

---

## 🎵 Audio Cues

- **Background Music** - Plays throughout gameplay
- **Day Start Jingle** - Music pauses, jingle plays, music resumes
- **Game Over Jingle** - Music stops, final jingle plays
- **Sound Effects**:
  - Enemy hit
  - Enemy killed
  - Sandwich thrown
  - Sandwich created
  - Player hit

---

## 🌅 Visual Effects

- **Screen Fade** - Black fade in/out on day transitions
- **Skybox Changes** - Environment changes every 3 days
- **Throw VFX** - Particle effect when throwing
- **Explosion VFX** - Particle effect on sandwich impact

---

## ⚙️ Settings You Can Adjust

### PlayerController Inspector:
- **Move Speed** - How fast you move (default: 5)
- **Mouse Sensitivity** - Look speed (default: 2)
- **Throw Force** - How hard sandwiches are thrown (default: 15)
- **Normal FOV** - Regular field of view (default: 60)
- **Aim FOV** - Zoomed field of view (default: 40)
- **Aim Speed** - How fast zoom transitions (default: 10)
- **Max Sandwiches** - Maximum carry capacity (default: 999)

### EnemySpawner Inspector:
- **Spawn Interval** - Time between enemy spawns (default: 2 seconds)
- **Spawn Bounds** - Reference to empty GameObject with Collider that defines spawn area
- **Ground Offset** - Height offset for spawning (default: 0.5)

### AudioManager Inspector:
- **Music Volume** - Background music (default: 0.5)
- **Jingle Volume** - Day/game over jingles (default: 0.7)
- **SFX Volume** - Sound effects (default: 0.6)

### SkyboxTransition Inspector:
- **Transition Duration** - How long skybox fade takes (default: 3 seconds)
- **Rounds Per Transition** - Days between skybox changes (default: 3)

---

## 🎯 Setting Up Spawn Area

1. **Create Empty GameObject** - Right-click in Hierarchy → Create Empty, name it "SpawnBounds"
2. **Add Box Collider** - Add Component → Box Collider, adjust size to cover spawn area
3. **Disable Collider** - Uncheck "Enabled" in Box Collider (it's just for bounds reference)
4. **Assign to EnemySpawner** - Drag SpawnBounds to EnemySpawner's "Spawn Bounds" field
5. **Visualize** - Select EnemySpawner to see red wireframe of spawn area in Scene view

**Tips:**
- Make spawn area larger than playable area
- Keep enemies spawning outside player's immediate view
- Adjust Ground Offset if enemies spawn too high/low

---

## 🐛 Quick Troubleshooting

**Sandwiches disappear after throwing?**
- This is fixed! If you have sandwiches in inventory, a new one automatically appears
- If sandwich doesn't appear, you might be out of sandwiches
- Go to a Sandwich Station and press E to refill

**Can't pay rent?**
- Find the Rent Station object in the scene
- Make sure it has the RentStation component
- Check it has a Collider
- Verify it's on the correct layer for raycasting
- Make sure rent is due (after completing a day)

**Rent Station doesn't show prompt?**
- Ensure RentStation is assigned in GameManager
- Check interaction range in PlayerController (default: 3)
- Verify the Collider is enabled

**Can't aim?**
- Check Input Actions has Aim action bound to right mouse button
- Regenerate C# class if needed

**Throws go wrong direction?**
- Make sure Camera component is assigned in PlayerController
- Check throwForce isn't too low

**Only Lungers spawn?**
- This is correct! Throwers were removed per design

**Audio not playing?**
- Ensure AudioManager GameObject exists in scene
- Assign audio clips in AudioManager Inspector

**No interaction prompt?**
- Verify UIManager has InteractionPromptPanel assigned
- Check Interactable object has Interactable script
- Ensure object has a Collider

---

**Have fun defending the subway! 🥖⚔️**
