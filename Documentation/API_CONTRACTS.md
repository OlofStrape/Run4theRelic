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
    public bool IsCarried { get; }
    public Transform Carrier { get; }
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
    public void SetMoveSpeed(float speed);
    public float BaseMoveSpeed { get; }
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
}
```

## Ansvarsfördelning

- **GameEvents**: Central kommunikationshub, inga tillstånd
- **MatchOrchestrator**: Matchfas-hantering, pussel-progression
- **PuzzleControllerBase**: Timer, gold-time, completion-logik
- **RelicController**: Pickup/drop, kollisionshantering
- **PlayerMovementHook**: MoveSpeed-justering via XRI
- **SabotageManager**: Fog-effekt, sabotage-timing 