# Project Overview
- Game Title: Polytechnic Outbreak
- High-Level Concept: A zombie survival game set in a polytechnic food court.
- Players: Single player.
- Render Pipeline: URP (Ultra_PipelineAsset).

# Game Mechanics
## Core Gameplay Loop
The player fights off zombies using various weapons, gains points, and survives as long as possible.
## Controls and Input Methods
- Movement: WASD
- Look: Mouse
- Shoot: Left Mouse Button (currently Space in Gun.cs, which is broken)
- Reload: R
- Interact: E

# UI
- HUD displays health, floor number, points, and current/reserve ammo.
- Ammo UI is managed by `HUDManager` and updated via events from `Player.cs` listening to `PlayerWeapons.cs`.

# Key Asset & Context
- `Assets/Characters/Player/Scripts/Gun.cs`: Handles shooting logic and ammo management for individual weapons.
- `Assets/Scripts/Player/PlayerWeapons.cs`: Manages the player's weapon inventory and equipped weapon.
- `Assets/Scripts/Player/WeaponInstance.cs`: Runtime data container for a weapon's state (ammo, level, etc.).
- `Assets/Scripts/Player/PlayerInputHandler.cs`: Centralized input handling using the New Input System.

# Implementation Steps
## 1. Fix Gun.cs Shooting Logic and Input
- **File**: `Assets/Characters/Player/Scripts/Gun.cs`
- **Changes**:
    - Remove the dependency on `WeaponSwitcher` (which doesn't exist on the Player object).
    - Remove the hardcoded `KeyCode.Space` input for firing.
    - Reference `PlayerInputHandler` to get `ShootPressed` and `ReloadPressed` states.
    - Remove local `currentAmmo` and `reserveAmmo` variables and instead use the state from a linked `WeaponInstance`.
    - Add a `public void Initialize(WeaponInstance instance, PlayerWeapons manager)` method to link the gun to the player's weapon system.
- **Dependencies**: None.

## 2. Update PlayerWeapons.cs to Link Guns
- **File**: `Assets/Scripts/Player/PlayerWeapons.cs`
- **Changes**:
    - Make `NotifyAmmoChanged()` public so it can be called by the `Gun` component when ammo changes.
    - In `EnsureSpawned()`, after instantiating the weapon prefab, get the `Gun` component and call its `Initialize()` method.
- **Dependencies**: Step 1.

## 3. Sync Ammo State and Trigger UI Updates
- **File**: `Assets/Characters/Player/Scripts/Gun.cs`
- **Changes**:
    - In `Shoot()` and `Reload()`, update the linked `WeaponInstance`'s ammo fields (`CurrentMagazineAmmo`, `CurrentReserveAmmo`).
    - Call `playerWeapons.NotifyAmmoChanged()` after any ammo change to trigger the UI update via `Player.cs`.
- **Dependencies**: Step 2.

# Verification & Testing
- **Manual Test**:
    1. Start the game.
    2. Try to shoot using the Left Mouse Button. Verify that muzzle flash and recoil occur.
    3. Check the HUD ammo display. Verify that the magazine ammo decreases with each shot.
    4. Press 'R' to reload. Verify that the ammo is transferred from reserve to magazine and the HUD updates.
    5. Swap weapons (if possible) and verify that the UI updates to show the correct ammo for the new weapon.
- **Edge Cases**:
    - Shooting until the magazine is empty (should trigger auto-reload).
    - Reloading with no reserve ammo.
    - Picking up or switching to a new weapon.
