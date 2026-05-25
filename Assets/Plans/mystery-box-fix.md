# Project Overview
- Game Title: Polytechnic Outbreak
- High-Level Concept: Zombie survival with Mystery Box weapon rolling.
- Players: Single player.
- Interaction System: Decentralized input checks with a centralized `HUDManager` for prompts.

# Game Mechanics
## Mystery Box Interaction
1. **Idle State**: Player sees "Press E to use Mystery Box [950]".
2. **Rolling State**: Player pays 950 points. Box lid opens, weapons cycle (visually).
3. **Pickup State**: A random weapon is presented. Player sees "Hold E to get [Weapon Name]". 
4. **Acquisition**: Interacting gives the weapon to the player and clears the box.
5. **Moving State**: After a timeout or "Teddy Bear" result, the box moves to a new location.

# UI
- `HUDManager`: Used to display the interaction prompts.
- `PlayerPoints`: Used to deduct cost and verify affordability.

# Key Asset & Context
- `Assets/MysteryBox/Script/MysteryBox.cs`: The core script to be refactored.
- `Assets/Scripts/Player/PlayerWeapons.cs`: Used to add the weapon to the player.
- `Assets/Scripts/Player/PlayerPoints.cs`: Used for the point economy.
- `Assets/UI/Scripts/HUD/HUDManager.cs`: Used for the UI feedback.

# Implementation Steps
## 1. Refactor MysteryBox.cs
- **States**: Implement an `enum BoxState { ReadyToRoll, Rolling, ReadyToPickup, Moving }`.
- **Proximity Detection**: Use `OnTriggerEnter/Exit` to track the player and show/hide HUD text.
- **Rolling Logic**:
    - Deduct points using `PlayerPoints.SpendPoints(950)`.
    - Handle the "Empty" (Teddy Bear) case.
    - Start a visual roll animation (cycling weapon models).
- **Pickup Logic**:
    - After the roll, display the specific weapon name in the HUD prompt.
    - On interaction, call `PlayerWeapons.TryAddWeapon(spawnedWeaponPrefab)`.
    - Cleanup the spawned weapon and close the box.
- **Weapon References**: Ensure the `weapons` array in the Mystery Box contains prefabs with the `Gun` component (from `Assets/Guns/`).

## 2. Update HUD Prompts
- Ensure the prompt updates dynamically based on the state.
- "ReadyToRoll" -> "Press E to use Mystery Box [Cost: 950]"
- "ReadyToPickup" -> "Hold E to get [Weapon Name]"

## 3. Sync with Player Systems
- Ensure `MysteryBox` finds the `Player` and its subsystems (`PlayerPoints`, `PlayerWeapons`, `PlayerInputHandler`).
- Use `PlayerInputHandler.InteractPressed` instead of raw `Input.GetKeyDown`.

# Verification & Testing
- **Test Affordability**: Try to use the box with < 950 points. Verify no roll occurs and prompt remains.
- **Test Roll**: Use the box with >= 950 points. Verify points are deducted and lid opens.
- **Test Pickup**: Wait for the weapon to appear. Verify HUD says "Hold E to get [Weapon]". Press E and verify the weapon is equipped and HUD clears.
- **Test Moving**: Trigger the "Empty" case and verify the box moves to a new `spawnPoint`.
