# Project Overview
- Game Title: Polytechnic Outbreak (Zombie Survival)
- High-Level Concept: Zombies chase the player in various environments and attack on contact.
- Players: Single player.
- Render Pipeline: URP.

# Game Mechanics
## Core Gameplay Loop
The player survives against waves of zombies. Zombies spawn and chase the player relentlessly.
## Controls and Input Methods
- AI controlled zombies using NavMesh system.

# UI
- Player health UI (already exists).

# Key Asset & Context
- `Assets/Characters/Zombies/Lvl 1/Lv1 1 zombie.prefab`: Base zombie prefab.
- `Assets/Scripts/Player/PlayerHealth.cs`: Target for zombie attacks.
- `Assets/Characters/Player/Scripts/ZombieHealth.cs`: Handles zombie death.
- Animation Clips: Idle, Walking, Attack, Death (from FBX files).

# Implementation Steps
## 1. Create Zombie Animator Controller
- **Asset**: `Assets/Characters/Zombies/ZombieAnimatorController.controller`
- **Parameters**: 
    - `Speed` (Float)
    - `Attack` (Trigger)
    - `IsDead` (Trigger)
- **States**:
    - **Locomotion**: 1D Blend Tree using `Speed`.
        - 0.0: `Idle` clip.
        - 1.0: `Walking` clip.
    - **Attack**: Play `Attack` clip. Transition from `Locomotion` via `Attack` trigger. Return to `Locomotion` when done (Has Exit Time).
    - **Death**: Play `Death` clip. Transition from Any State via `IsDead` trigger.
- **Dependencies**: Animation clips from `Assets/Characters/Zombies/Lvl 1/`.

## 2. Implement ZombieAI Script
- **File**: `Assets/Scripts/AI/ZombieAI.cs`
- **Features**:
    - Reference to `NavMeshAgent` and `Animator`.
    - Relentless chase: Set `agent.destination` to Player's position every frame or at a set interval.
    - Attack logic:
        - Check distance to player.
        - If within `attackRange` (e.g., 1.5m) and `attackCooldown` is ready:
            - Trigger `Attack` animation.
            - Apply damage to `PlayerHealth`.
            - Set cooldown.
    - Animation syncing: Update `Speed` parameter based on `agent.velocity.magnitude`.
    - Stop AI on death: Check if a `ZombieHealth` component indicates death or listen for a disable.
- **Dependencies**: `UnityEngine.AI`, `PlayerHealth.cs`.

## 3. Configure Zombie Prefabs
- **Assets**: `Assets/Characters/Zombies/Lvl 1/Lv1 1 zombie.prefab` (and others).
- **Changes**:
    - Add `NavMeshAgent` component.
    - Add `ZombieAI` component.
    - Assign the new `ZombieAnimatorController` to the `Animator` component.
    - Ensure `ZombieHealth` references the `Animator`.
- **Dependencies**: Step 1 & 2.

# Verification & Testing
- **Manual Test**:
    1. Place a zombie in the `FoodCourt` or `office building` scene (on the NavMesh).
    2. Start the game.
    3. Verify the zombie moves toward the player.
    4. Verify the zombie transitions to the walking animation.
    5. Verify the zombie stops and plays the attack animation when near the player.
    6. Verify the player's health decreases on attack.
    7. Shoot the zombie and verify it plays the death animation and stops moving.
- **Edge Cases**:
    - Player is unreachable (zombie should try to get as close as possible).
    - Zombie dies while attacking.
