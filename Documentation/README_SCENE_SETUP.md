## Relic polish

This guide covers how to set up the Relic with hand anchor, aura glow, smarter drop, and audio hooks.

### Hand anchor

- Assign `RelicController.rightHandAnchor` manually in the inspector to the player's right-hand transform.
- If left null, a fallback search is performed at runtime using the path:
  `XR Origin/Camera Offset/RightHand Controller` from the scene root.

### Carry slow (movement)

- `RelicController.carrySpeedMultiplier` controls the carrier's move speed while carrying.
- Suggested range: 0.5–0.65 (default 0.55) for a clear but fair slowdown.
- If your `PlayerMovementHook.SetCarrySlow(bool, float)` overload exists, the multiplier is applied automatically. Otherwise, the legacy `SetCarrySlow(bool)` is used.

### Smarter drop on collisions

- Configure:
  - `dropVelocityThreshold`: speed threshold to trigger a drop on collision. Try 1.2–2.0; default 1.5.
  - `dropAngleBias`: blends the carrier forward vs. collision normal (0 = forward, 1 = normal). Default 0.25.
  - `dropImpulse`: impulse applied when force-dropping. Default 2.5.
- When the carried Relic collides over the threshold, it will force-drop in a blended forward/normal direction, with a small upward bias for clarity.

### Aura glow

- Add the `RelicAura` component to the Relic GameObject.
- `RelicAura` uses a `LineRenderer` circle that pulses scale and alpha over time.
- Parameters: `radius` (0.25 default), `pulseSpeed` (1.5), `minScale` (0.9), `maxScale` (1.1).
- For best results, use URP with Bloom enabled.

### Audio hooks

- Add an `AudioSource` component on the Relic and assign it to `RelicController.audioSource`.
- Drag audio clips into:
  - `sfxPickup`: played on pickup
  - `sfxDrop`: played on drop/force-drop
  - `sfxExtract`: played when the Relic is extracted
- If any clip is not assigned, that sound is simply skipped.

### Acceptance checklist

- Relic anchors consistently to the right hand (or uses fallback path)
- Carrier moves slower with a clear difference
- Drop occurs on collisions over threshold, with understandable forward impulse
- Aura is visible and pulses in-game
- Audio hooks play when clips are assigned

# Scen Setup Guide - Run4theRelic

## Förutsättningar
- Unity 2022.3 LTS installerat
- Projektet öppnat i Unity Hub
- OpenXR aktiverat för PC

## Steg 1: Aktivera OpenXR
1. Gå till **Edit > Project Settings**
2. Välj **XR Plug-in Management**
3. Klicka **Install XR Plug-in Management** om det inte redan är installerat
4. Under **PC Settings**, aktivera **OpenXR**
5. Klicka **Install OpenXR Package** om det behövs

## Steg 2: Importera XRI Samples
1. Öppna **Window > Package Manager**
2. Sök efter **XR Interaction Toolkit**
3. Klicka **Import** på **XR Interaction Toolkit Samples**
4. Välj alla samples och klicka **Import**

## Steg 3: Skapa XR Origin
1. Högerklicka i **Hierarchy** → **XR > XR Origin (Action-based)**
2. Detta skapar:
   - XR Origin
   - Main Camera (XR)
   - LeftHand Controller
   - RightHand Controller

## Steg 4: Skapa Pussel-rum
1. **Room 1 (CableConnect)**
   - Skapa en tom GameObject, döp till "CableConnectRoom"
   - Lägg till `CableConnectController` script
   - Skapa 3-4 `CableSocket` GameObjects
   - Skapa 3-4 `CablePlug` GameObjects

2. **Room 2 (RunicSequence)**
   - Skapa "RunicSequenceRoom"
   - Lägg till `RunicSequenceController` script
   - Skapa 4 `RunicPad` GameObjects

3. **Room 3 (BalanceBridge)**
   - Skapa "BalanceBridgeRoom"
   - Lägg till `BalanceBridgeController` script
   - Skapa en `testPoint` GameObject

## Steg 5: Lägg till Relic och Extraction
1. **Relic Arena**
   - Skapa "RelicArena" GameObject
   - Lägg till `RelicController` script
   - Skapa en "Relic" GameObject som child

2. **Extraction Zone**
   - Skapa "ExtractionZone" GameObject
   - Lägg till `ExtractionZone` script
   - Lägg till en Collider (Trigger)

## Steg 6: Koppla Scripts
1. **MatchOrchestrator**
   - Lägg till `MatchOrchestrator` script på en tom GameObject
   - Koppla alla pussel-controllers i Inspector

2. **SabotageManager**
   - Lägg till `SabotageManager` script på en tom GameObject

3. **PlayerMovementHook**
   - Lägg till `PlayerMovementHook` script på XR Origin
   - Koppla till ContinuousMoveProvider i Inspector

## Steg 7: Testa
1. Klicka **Play** i Unity
2. Verifiera att:
   - XR Origin rör sig med WASD
   - Scripts kompilerar utan fel
   - Console visar inga errors

## Felsökning
- **Scripts kompilerar inte**: Kontrollera att alla using-statements finns
- **XR Origin rör sig inte**: Verifiera att OpenXR är aktiverat
- **Missing references**: Kontrollera att alla scripts är kopplade i Inspector

## Nästa Steg
Efter setup, se `Documentation/TASKS.md` för MVP-uppgifter. 