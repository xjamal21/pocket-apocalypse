[Pocket Apocalypse]
A 2D top down **Bullet Heaven** survival game built in Unity where players survive escalating enemy waves, build out their character through a MOBA-style item crafting shop, and manage tactical support and economy systems through an interactive in-game Smartphone OS HUD.

Gameplay & Core Systems 
1. Player & Combat Core
   Responsive 2D top down player movement with health management, collision handling, and character selection.
   Automated bullet heaven combat, nearest enemy auto targeting, auto firing weapon mechanics and configurable projectile trajectories.
   Player Stat Manager : Centralized stat system supporting dynamic scaling across combat and utility attributes.

2. Enemy Engine & Wave Spawner
   Structured wave spawning driven by scaling difficulty curves.
   Standard chase enemy AI alongside specialized behaviors for elite enemies and bosses.
   Drops collectible gold coins on enemy defeat to fuel progression and player economy.

3. Smartphone OS & In Game Apps
  Players can open an interactive in game smartphone interface featuring multiple tactical and economy apps:
  Bitedash : Air-strike drone pathing and tactical delivery systems.
  Bounty Board : Dynamic missions tracked by event listeners
  Swiperight : Tinder style swipe left right interface to scout and spawn mercenary companions.
  Music Player App (MP3) : Built-in soundtrack playback, local audio importing.
  Casino : Yeah Gamble.

## Project Structure
```
Assets/
├── Textures/          # Pixel-art sprites, characters, enemies, and Smartphone OS UI assets
├── Audio/        # Built-in soundtrack and sound effects
├── Data/         # ScriptableObject items, recipes, waves, and character configs
├── Prefabs/      # Player, weapons, projectiles, enemies, drops, and UI prefabs
├── Scenes/       # Main menu and gameplay scenes
└── Scripts/      # Modular C# scripts (one class per file)
```

Tech Stack 
**ENGINE**: UNITY (2D)
**LANGUAGE**: C#

Contributors
Dixon
Po Keng
Yao Hong



      
