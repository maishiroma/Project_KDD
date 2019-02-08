# How to Play the Protoype
> Welcome! This .md file will guide you through on the various elements that are present in the prototype

## Controls
- `A`: Move Left
- `D`: Move Right
- `S`: Duck/Swallow (if something is in the player's mouth)
- `W`: Enter door
- `H`: Inhale/Exhale
- `Space`: Jump/Fly

## Basic Actions
- Moving
  - To move the player, simple press and hold down either `A` or `D` to move left or right respectively.
  > If the player is in the air, movement speed is reduced, and if the player is on slopes, their speed will reflect the slope's direction.
- Ducking
  - While on the ground, press and hold `S` to crouch.
    - The player will have a smaller hitbox, making them dodge certain attacks easier.
    - If the player is `stuffed` and ducks, they will `swallow` whatever was in their mouth and not be `stuffed` anymore.
- Jumping
  - Press `space` on the ground to jump. Press `space` again while airborn to fly.
    - While flying, the player will fall slower.
    - To stop flying, press `H` to exhale an airpuff that can damage enemies and blocks
- Inhaling/Exhaling
  - Press and hold `H` to begin inhaling. Any enemy or block that is within range will be sucked into your mouth.
    - When this happens, the player will be `stuffed`.
    - While `stuffed`, you can exhale out a star by hitting `H` again.
- Sliding
  - While ducking, press `H` and the player will slide on the ground for a set distance
    - Any enemy/block that is in front of the player will take damage.
- High fall
  - When the player is airborn and not flying, the player will eventually enter a `high fall` state

## Special States
- `stuffed`: The player will be unable to fly or inhale. However, the player can exhale out a damaging star projectile outward once.
- `high fall`: The player's sprite will change, and anything that is below the player will take damage. When this happens, the player will bounce off of the enemy and fall normally again, until more time passes.

## How to Attack
There's five ways of attacking enemies in this prototype:
1. Inhale an enemy
2. Shooting a star projectile towards another enemy
3. Spitting an airpuff at an enemy
4. Sliding into an enemy
5. High Falling onto an enemy from above

## Enemy Types
- Patrols: These enemies move around a given area. Some either move forward continuously until they run into a wall, and then turn around. Others turn around after a set amount of time.
- Flying: These enemies are similar to Patrols, only that they are airborn. They move in a wavelike pattern.
- Stills: These enemies do not move at all. Don't run into them!

## Level Geometry
- Pass through platforms: These gray platforms allow the player to pass through the bottom of them, and allows the player to go through them from the bottom. To do this, simply duck while on top of these platforms.
- Blocks: These star blocks do not hurt the player upon contact, but they can be inhaled and destroyed by the player.

## Collectibles
- Health Packs: These restore the player's health to a certain extent
- 1ups: These grant the player an extra life.
