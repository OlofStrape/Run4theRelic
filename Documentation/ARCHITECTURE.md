# Arkitektur - Run4theRelic

## Moduler

### Core
- **GameEvents** - Statiska events för systemkommunikation
- **MatchOrchestrator** - Hanterar matchfaserna och progression

### Player
- **PlayerMovementHook** - Wrapper för XRI ContinuousMoveProviderBase
- **PlayerController** - Huvudlogik för spelarkontroll

### Puzzles
- **PuzzleControllerBase** - Basklass för alla pussel
- **CableConnect** - Färgmatchande kabelpussel
- **RunicSequence** - Simon-sekvenspussel
- **BalanceBridge** - Balanspussel med timer

### Sabotage
- **SabotageManager** - Hanterar fog-effekt och sabotage

### Relic
- **RelicController** - Relic-logik och pickup
- **ExtractionZone** - Vinstzon för extraktion

### UI
- **GameUI** - Huvudgränssnitt
- **PuzzleUI** - Pussel-specifikt UI

## Dataflöde

```
MatchOrchestrator → PuzzleControllerBase → GameEvents
                                      ↓
PlayerMovementHook ← RelicController ← SabotageManager
```

## Scenstruktur
```
XR Origin
├── Player
│   ├── XR Camera
│   └── XR Controller
├── Puzzle Rooms
│   ├── Room 1 (CableConnect)
│   ├── Room 2 (RunicSequence)
│   └── Room 3 (BalanceBridge)
├── Relic Arena
└── Extraction Zone
```

## Kommunikation
- **Events** - Löst kopplade system via GameEvents
- **Direct References** - Endast för nära kopplade komponenter
- **ScriptableObjects** - Konfiguration och data 