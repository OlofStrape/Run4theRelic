# API-kontrakt - Run4theRelic

## Core API:er

### GameEvents
```csharp
public static class GameEvents
{
    // Pussel-events
    public static event Action<int, float> OnPuzzleCompleted; // playerId, clearTime
    public static event Action<int> OnPlayerEliminated; // playerId
    
    // Relic-events
    public static event Action OnRelicSpawned;
    public static event Action<int> OnRelicPickedUp; // playerId
    public static event Action<int> OnRelicDropped; // playerId
    public static event Action<int> OnRelicExtracted; // playerId
    
    // Match-events
    public static event Action<MatchPhase> OnMatchPhaseChanged;
    public static event Action OnMatchEnded;
}
```

### MatchOrchestrator
```csharp
public class MatchOrchestrator : MonoBehaviour
{
    public void StartMatch(); // Startar countdown
    public void AdvancePhase(); // Avancerar till nästa fas
    public void SpawnRelic(); // Spawnar Relic:en vid Final-fas
    public MatchPhase CurrentPhase { get; }
}

public enum MatchPhase
{
    Lobby, Countdown, Puzzle1, Puzzle2, Puzzle3, 
    GoldTimeSabotage, Final, PostMatch
}
```

## Pussel API:er

### PuzzleControllerBase
```csharp
public abstract class PuzzleControllerBase : MonoBehaviour
{
    [SerializeField] protected float timeLimit = 60f;
    [SerializeField] protected float goldTimeFraction = 0.5f;
    
    protected abstract void OnPuzzleStart();
    protected abstract void OnPuzzleComplete();
    protected abstract void OnPuzzleFailed();
    
    protected void Complete(); // Triggar OnPuzzleCompleted
    protected void Fail(); // Triggar OnPuzzleFailed
}
```

### CableConnect
```csharp
public class CablePlug : MonoBehaviour
{
    public int colorId;
    public bool IsConnected { get; }
    public void ConnectTo(CableSocket socket);
}

public class CableSocket : MonoBehaviour
{
    public int acceptsColorId;
    public bool IsOccupied { get; }
}

public class CableConnectController : PuzzleControllerBase
{
    public List<CableSocket> sockets;
    public List<CablePlug> plugs;
}
```

### RunicSequence
```csharp
public class RunicPad : MonoBehaviour
{
    public int padId;
    public void Press();
}

public class RunicSequenceController : PuzzleControllerBase
{
    public List<RunicPad> pads;
    public int sequenceLength = 4;
    public float stepDelay = 1f;
}
```

### BalanceBridge
```csharp
public class BalanceBridgeController : PuzzleControllerBase
{
    public Transform testPoint;
    public float threshold = 0.1f;
    public float balanceTime = 3f;
}
```

## Relic API:er

### RelicController
```csharp
public class RelicController : MonoBehaviour
{
    public void PickUp(Transform player);
    public void Drop();
    public void ForceDrop();
    public bool IsCarried { get; }
    public Transform Carrier { get; }
    public Vector3 OriginalPosition { get; }
    // Inspector fields:
    // - Transform rightHandAnchor, fallbackAnchor
    // - float carrySpeedMultiplier = 0.55f
    // - float dropVelocityThreshold, dropAngleBias, dropImpulse
    // - AudioSource audioSource; AudioClip sfxPickup, sfxDrop, sfxExtract
}
```

### ExtractionZone
```csharp
public class ExtractionZone : MonoBehaviour
{
    public void OnPlayerEnter(PlayerController player);
    public bool CanExtract(PlayerController player);
}
```

## Player API:er

### PlayerMovementHook
```csharp
public class PlayerMovementHook : MonoBehaviour
{
    public void SetCarrySlow(bool isCarrying);
    public void SetCarrySlow(bool isCarrying, float multiplierOverride);
    public void SetMoveSpeed(float speed);
    public float BaseMoveSpeed { get; }
}
```

### RelicAura
```csharp
public class RelicAura : MonoBehaviour
{
    // Attach with LineRenderer; draws a pulsing ring around Relic
}
```

## Sabotage API:er

### SabotageManager
```csharp
public class SabotageManager : MonoBehaviour
{
    public void TriggerFog(GameObject target, float duration = 5f);
    public void ClearFog(GameObject target);
    public bool IsFogActive(GameObject target);
    
    // Extended
    public void ApplyFog(float duration = 5f); // Global fog for duration
    public void ApplyTimeDrain(float seconds = 5f); // Reduce time on active puzzle
    public void ApplyFakeClues(float duration = 5f, int count = 6); // Spawn decoys
}
```

### SabotageTokenBank
```csharp
public class SabotageTokenBank : MonoBehaviour
{
    public int CurrentTokens { get; }
    public event Action<int> OnTokensChanged;
    public void Add(int amount);
    public bool Spend(int amount);
}
```

### SabotageWheel
```csharp
public class SabotageWheel : MonoBehaviour
{
    public enum Option { Fog, TimeDrain, FakeClues }
    public void Show(Option[] options); // Auto-hides after 5s or on selection
    public void Hide();
}
```

## Ansvarsfördelning

- **GameEvents**: Central kommunikationshub, inga tillstånd
- **MatchOrchestrator**: Matchfas-hantering, pussel-progression
- **PuzzleControllerBase**: Timer, gold-time, completion-logik
- **RelicController**: Pickup/drop, kollisionshantering
- **PlayerMovementHook**: MoveSpeed-justering via XRI
- **SabotageManager**: Fog-effekt, sabotage-timing 