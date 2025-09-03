# VR Puzzle System - Run4theRelic

## Översikt

VR Puzzle System är en samling avancerade VR-specifika pussel som använder VR Core System för att skapa immersiva och interaktiva upplevelser. Systemet är byggt för att utnyttja VR:s unika möjligheter som hand-tracking, grabbing, pointing och fysiska interaktioner.

## Komponenter

### 1. VRRelicPuzzle
**Fil:** `Assets/Scripts/Puzzles/VRRelicPuzzle.cs`

Ett fysiskt VR-pussel där spelare måste gripa och placera relics i rätt positioner:
- **Fysisk grabbing** med VR-händer
- **Auto-snap** till slot-positioner
- **Haptic feedback** vid interaktioner
- **Visual feedback** med highlighting
- **Progress tracking** för pussel-lösning

**Användning:**
```csharp
// Skapa en VR Relic Puzzle
var relicPuzzle = gameObject.AddComponent<VRRelicPuzzle>();

// Konfigurera relics och slots
relicPuzzle.requiredRelics = new GameObject[] { relic1, relic2, relic3 };
relicPuzzle.relicSlots = new Transform[] { slot1, slot2, slot3 };

// Prenumerera på events
VRRelicPuzzle.OnLeftHandGrabbingChanged += (obj, grabbing) => {
    if (grabbing) Debug.Log($"Grabbing: {obj.name}");
};

// Kontrollera progress
float progress = relicPuzzle.GetPuzzleProgress();
Debug.Log($"Puzzle progress: {progress}%");
```

### 2. VRHandGesturePuzzle
**Fil:** `Assets/Scripts/Puzzles/VRHandGesturePuzzle.cs`

Ett avancerat hand-gesture pussel som kräver specifika hand-rörelser:
- **Hand gesture detection** (point, grab, fist, thumbs up, wave)
- **Sequential gestures** med timing
- **Movement tracking** i specifika riktningar
- **Position requirements** för exakta hand-positioner
- **Real-time feedback** med haptics

**Användning:**
```csharp
// Skapa en VR Hand Gesture Puzzle
var gesturePuzzle = gameObject.AddComponent<VRHandGesturePuzzle>();

// Konfigurera required gestures
gesturePuzzle.requiredGestures = new HandGesture[] {
    new HandGesture {
        gestureType = GestureType.Point,
        requiredHand = HandSide.Left,
        requiredHoldTime = 2f
    },
    new HandGesture {
        gestureType = GestureType.Grab,
        requiredHand = HandSide.Right,
        requireMovement = true,
        movementDirection = Vector3.up,
        minMovementDistance = 0.5f
    }
};

// Prenumerera på gesture events
VRHandGesturePuzzle.OnGestureCompleted += (index, gesture) => {
    Debug.Log($"Completed gesture {index}: {gesture.gestureType}");
};

// Kontrollera progress
float progress = gesturePuzzle.GetPuzzleProgress();
int currentGesture = gesturePuzzle.GetCurrentGestureIndex();
float remainingTime = gesturePuzzle.GetCurrentGestureRemainingTime();
```

## Gesture Types

### Point Gesture
- **Trigger:** Trigger-knappen på controller
- **Användning:** Pointing, selection, activation
- **Konfiguration:** `GestureType.Point`

### Grab Gesture
- **Trigger:** Grip-knappen på controller
- **Användning:** Grabbing objects, holding items
- **Konfiguration:** `GestureType.Grab`

### Open Hand Gesture
- **Trigger:** Hand är öppen (inte grip)
- **Användning:** Releasing, showing empty hands
- **Konfiguration:** `GestureType.OpenHand`

### Fist Gesture
- **Trigger:** Hand är stängd (grip)
- **Användning:** Punching, hitting, power actions
- **Konfiguration:** `GestureType.Fist`

### Thumbs Up Gesture
- **Trigger:** Grip utan trigger
- **Användning:** Approval, confirmation
- **Konfiguration:** `GestureType.ThumbsUp`

### Wave Gesture
- **Trigger:** Hand-rörelse i specifik riktning
- **Användning:** Greeting, signaling
- **Konfiguration:** `GestureType.Wave` med `requireMovement = true`

## Hand Side Options

### Left Hand
- **Användning:** Vänster hand specifikt
- **Konfiguration:** `HandSide.Left`

### Right Hand
- **Användning:** Höger hand specifikt
- **Konfiguration:** `HandSide.Right`

### Both Hands
- **Användning:** Båda händerna samtidigt
- **Konfiguration:** `HandSide.Both`

### Either Hand
- **Användning:** Antingen hand
- **Konfiguration:** `HandSide.Either`

## Setup i Unity

### 1. Skapa VR Relic Puzzle
1. Skapa en tom GameObject i scenen
2. Lägg till `VRRelicPuzzle` komponenten
3. Konfigurera `requiredRelics` array med relic GameObjects
4. Konfigurera `relicSlots` array med slot Transform-positioner
5. Sätt `slotSnapDistance` för auto-snap funktionalitet

### 2. Skapa VR Hand Gesture Puzzle
1. Skapa en tom GameObject i scenen
2. Lägg till `VRHandGesturePuzzle` komponenten
3. Konfigurera `requiredGestures` array med HandGesture-objekt
4. Sätt `requireSequentialGestures` för sekventiella pussel
5. Konfigurera `gestureHoldTime` för timing-krav

### 3. Konfigurera VR Core System
1. Se till att VR Core System är korrekt uppsatt
2. Verifiera att XR Interaction Toolkit är konfigurerat
3. Kontrollera att Layer "Interactable" finns
4. Testa VR-input och interaction system

## Exempel på Puzzle-konfigurationer

### Enkelt Relic Puzzle
```csharp
// 3 relics som ska placeras i slots
relicPuzzle.requiredRelics = new GameObject[] { 
    relic1, relic2, relic3 
};
relicPuzzle.relicSlots = new Transform[] { 
    slot1, slot2, slot3 
};
relicPuzzle.autoSnapToSlots = true;
relicPuzzle.slotSnapDistance = 0.15f;
```

### Avancerat Gesture Puzzle
```csharp
// Sekventiellt pussel med timing
gesturePuzzle.requireSequentialGestures = true;
gesturePuzzle.requiredGestures = new HandGesture[] {
    // 1. Point med vänster hand i 2 sekunder
    new HandGesture {
        gestureType = GestureType.Point,
        requiredHand = HandSide.Left,
        requiredHoldTime = 2f,
        requireContinuousHold = true
    },
    // 2. Grab med höger hand och röra uppåt
    new HandGesture {
        gestureType = GestureType.Grab,
        requiredHand = HandSide.Right,
        requireMovement = true,
        movementDirection = Vector3.up,
        minMovementDistance = 0.5f,
        requiredHoldTime = 1f
    },
    // 3. Båda händerna som thumbs up
    new HandGesture {
        gestureType = GestureType.ThumbsUp,
        requiredHand = HandSide.Both,
        requiredHoldTime = 3f
    }
};
```

## Events System

### VR Relic Puzzle Events
```csharp
// Grabbing events
VRInteractionSystem.OnLeftHandGrabbingChanged += OnLeftHandGrabbingChanged;
VRInteractionSystem.OnRightHandGrabbingChanged += OnRightHandGrabbingChanged;

// Pointing events
VRInteractionSystem.OnLeftHandPointingChanged += OnLeftHandPointingChanged;
VRInteractionSystem.OnRightHandPointingChanged += OnRightHandPointingChanged;
```

### VR Hand Gesture Puzzle Events
```csharp
// Gesture events
VRHandGesturePuzzle.OnGestureStarted += OnGestureStarted;
VRHandGesturePuzzle.OnGestureCompleted += OnGestureCompleted;
VRHandGesturePuzzle.OnGestureFailed += OnGestureFailed;

// Input events
VRInputManager.OnLeftHandGestureChanged += OnLeftHandGestureChanged;
VRInputManager.OnRightHandGestureChanged += OnRightHandGestureChanged;
VRInputManager.OnLeftGripChanged += OnLeftGripChanged;
VRInputManager.OnRightGripChanged += OnRightGripChanged;
```

## Performance Optimering

### 1. Gesture Detection
- **Update Frequency:** Endast när pussel är aktiva
- **Gesture Complexity:** Begränsa antalet samtidiga gestures
- **Position Checks:** Använd cached positions när möjligt

### 2. Visual Feedback
- **Material Pooling:** Återanvänd highlight materials
- **LOD System:** Enklare visualiseringer på avstånd
- **Particle Effects:** Begränsa antalet aktiva partiklar

### 3. Haptic Feedback
- **Intensity Scaling:** Anpassa baserat på device
- **Frequency Limiting:** Undvik för många haptic calls
- **Device-specific:** Olika inställningar för olika VR-headsets

## Troubleshooting

### Vanliga problem:

1. **Gestures inte detekterade**
   - Kontrollera VR Input Manager setup
   - Verifiera controller input
   - Kontrollera gesture thresholds

2. **Relics inte grabable**
   - Verifiera XR Grab Interactable komponenter
   - Kontrollera Layer "Interactable"
   - Se till att Rigidbody finns

3. **Haptic feedback inte fungerar**
   - Kontrollera VR Manager setup
   - Verifiera controller capabilities
   - Testa med enkla haptic calls

4. **Performance problem**
   - Begränsa antalet aktiva pussel
   - Optimera gesture detection loops
   - Använd object pooling för visual effects

## Nästa steg

Med VR Puzzle System på plats kan du nu:

1. **Skapa komplexa VR-pussel** - Kombinera grabbing, gestures och timing
2. **Implementera multiplayer VR-pussel** - Samarbete mellan VR-spelare
3. **Bygga VR-puzzle-sekvenser** - Kedjade pussel med progression
4. **Skapa VR-tutorials** - Interaktiva VR-instruktioner

## Exempel på Avancerade Pussel

### Multiplayer Relic Puzzle
```csharp
public class MultiplayerVRRelicPuzzle : VRRelicPuzzle
{
    [SerializeField] private bool requireBothPlayers = true;
    
    protected override void OnRelicGrabbed(int relicIndex, bool isLeftHand)
    {
        base.OnRelicGrabbed(relicIndex, isLeftHand);
        
        // Notify other players
        NetworkManager.Instance.NotifyRelicGrabbed(relicIndex, isLeftHand);
    }
}
```

### VR Puzzle Chain
```csharp
public class VRPuzzleChain : MonoBehaviour
{
    [SerializeField] private VRRelicPuzzle[] relicPuzzles;
    [SerializeField] private VRHandGesturePuzzle[] gesturePuzzles;
    
    private void Start()
    {
        // Link puzzles together
        foreach (var puzzle in relicPuzzles)
        {
            puzzle.OnPuzzleSolved += OnPuzzleSolved;
        }
    }
    
    private void OnPuzzleSolved()
    {
        // Activate next puzzle in chain
        ActivateNextPuzzle();
    }
}
```

## Support

För frågor eller problem med VR Puzzle System, kontakta utvecklingsteamet eller konsultera VR Core System dokumentationen.
