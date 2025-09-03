## Sabotage Tokens & Selection Wheel - Editor Test Guide

Follow these steps to verify the sabotage token flow and effects in the Unity Editor.

1. Scene setup
   - Ensure a `SabotageManager` exists in the scene.
   - Ensure a `SabotageTokenBank` exists in the scene (any GameObject).
   - Add a `SabotageWheel` to the scene (empty GameObject with the component). It will auto-position in front of the camera when shown.

2. Awarding a token on gold completion
   - Enter Play Mode and trigger any puzzle using a controller derived from `PuzzleControllerBase`.
   - Complete the puzzle quickly so that the remaining time is >= `GoldTimeThreshold`.
   - Expected: `SabotageTokenBank` receives +1 (watch Console for log) and the `SabotageWheel` appears with three options: Fog, TimeDrain, FakeClues.

3. Selecting an effect
   - Click any of the three options on the wheel.
   - Expected: one token is spent; the wheel hides immediately.
   - Fog: Temporary global fog toggles on for ~5s, then off.
   - TimeDrain: Active puzzle loses 5 seconds from its remaining time (visible in Console and puzzle timer if displayed).
   - FakeClues: Spawns colored quad planes near the camera for ~5s, then they despawn automatically. If you have a prefab assigned in `SabotageManager.fakeCluePrefab`, that prefab will be used instead.

4. Auto-hide behavior
   - If no selection is made, the wheel auto-hides after 5 seconds.

5. Manual testing
   - You can manually grant tokens via the inspector by calling `SabotageTokenBank.Add(1)` from a debug script, or enabling the `startingTokens` value to be > 0 and then calling `SabotageWheel.Show(new[]{ SabotageWheel.Option.Fog, SabotageWheel.Option.TimeDrain, SabotageWheel.Option.FakeClues })` from a temporary script.

Notes
 - TimeDrain targets the first active `PuzzleControllerBase` found in the scene.
 - Fog visuals depend on the `fogMaterial` configuration. Assign a suitable material and tune density/color in `SabotageManager`.
 - Fake Clues use `fakeCluePrefab` if provided; otherwise a basic quad is spawned.

