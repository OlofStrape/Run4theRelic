# VR Environment System - Run4theRelic

## Översikt

VR Environment System är ett komplett system för att skapa immersiva VR-miljöer och rum i Run4theRelic. Systemet hanterar rum, miljöer, atmosfär, ljus, partiklar och VR-specifika funktioner för att skapa en engagerande spelupplevelse.

## Komponenter

### 1. VREnvironmentSystem
**Fil:** `Assets/Scripts/Core/VREnvironmentSystem.cs`

Huvudsystemet som hanterar alla VR-miljöer och rum:
- **Room Management** - Hanterar rum, övergångar och progression
- **Dynamic Lighting** - Automatisk ljusjustering mellan rum
- **Atmospheric Effects** - Fog, partiklar och ambient ljud
- **VR Comfort Features** - Blink-effekter och comfort-settings
- **Puzzle Integration** - Automatisk aktivering av pussel

**Användning:**
```csharp
// Hitta environment system
var envSystem = FindObjectOfType<VREnvironmentSystem>();

// Byt rum
envSystem.ChangeRoom("PuzzleRoom");

// Kontrollera status
bool isTransitioning = envSystem.IsTransitioning();
float progress = envSystem.GetTransitionProgress();
VRRoom currentRoom = envSystem.GetCurrentRoom();
```

### 2. VRRoomTemplate
**Fil:** `Assets/Scripts/Core/VRRoomTemplate.cs`

Ett template-system för att snabbt skapa olika typer av rum:
- **Pre-configured Templates** - Färdiga rumstyper (Entrance, PuzzleRoom, BossRoom, etc.)
- **Auto-spawning** - Automatisk generering av ljus, partiklar och dekorationer
- **Puzzle Integration** - Automatisk setup av pussel och relic slots
- **Atmospheric Effects** - Fog, ambient ljud och partiklar

**Användning:**
```csharp
// Skapa en puzzle room från template
var roomTemplate = gameObject.AddComponent<VRRoomTemplate>();
roomTemplate.templateType = RoomTemplateType.PuzzleRoom;
roomTemplate.roomName = "Ancient Puzzle Chamber";
roomTemplate.ConfigureRoomFromTemplate();

// Prenumerera på events
VRRoomTemplate.OnRoomTemplateConfigured += (template, room) => {
    Debug.Log($"Room configured: {room.roomName}");
};
```

## Rumstyper

### Entrance Room
- **Syfte:** Spelarens första intryck av spelet
- **Funktioner:** Tutorial, grundläggande kontroller, atmosfärisk introduktion
- **Konfiguration:** Ljus atmosfär, tydliga instruktioner, enkla interaktioner

### Corridor
- **Syfte:** Övergång mellan rum, navigation
- **Funktioner:** Miljövariation, ambient ljud, navigation hints
- **Konfiguration:** Neutral ljusning, subtila partiklar, teleportation

### Puzzle Room
- **Syfte:** Huvudspel, pussel-lösning
- **Funktioner:** VR-pussel, relic placement, progress tracking
- **Konfiguration:** Fokus-ljusning, puzzle-highlighting, haptic feedback

### Relic Chamber
- **Syfte:** Relic collection, progression
- **Funktioner:** Relic slots, atmospheric effects, reward feedback
- **Konfiguration:** Dramatisk ljusning, guld-partiklar, ambient mystik

### Boss Room
- **Syfte:** Utmaning, climax
- **Funktioner:** Boss fight, intense atmosphere, special effects
- **Konfiguration:** Dramatisk ljusning, röda partiklar, spänning

### Exit Room
- **Syfte:** Avslut, belöning
- **Funktioner:** Victory celebration, completion feedback
- **Konfiguration:** Triumf-ljusning, celebration effects, ambient satisfaction

## Template Settings

### Lighting Settings
```csharp
[Header("Lighting")]
public Color ambientColor = Color.white;
public float ambientIntensity = 0.2f;
public Material skyboxMaterial;
public Light[] templateLights;
```

### Atmosphere Settings
```csharp
[Header("Atmosphere")]
public ParticleSystem[] templateParticles;
public AudioClip[] templateAmbientSounds;
public bool enableFog = false;
public Color fogColor = Color.gray;
public float fogDensity = 0.01f;
```

### Puzzle Integration
```csharp
[Header("Puzzle Integration")]
public GameObject[] templatePuzzleObjects;
public Transform[] templateRelicSlots;
public bool autoActivatePuzzles = true;
```

### VR Comfort
```csharp
[Header("VR Comfort")]
public bool enableComfortFeatures = true;
public float maxTurnAngle = 45f;
public bool enableTeleportation = true;
public Transform[] templateTeleportTargets;
```

## Setup i Unity

### 1. Skapa Environment System
1. Skapa en tom GameObject i scenen
2. Lägg till `VREnvironmentSystem` komponenten
3. Konfigurera `availableRooms` array
4. Sätt `playerSpawnPoint` för spelarens startposition

### 2. Skapa Room Templates
1. Skapa en tom GameObject för varje rum
2. Lägg till `VRRoomTemplate` komponenten
3. Välj `templateType` (Entrance, PuzzleRoom, etc.)
4. Konfigurera `templateSettings` för rummet
5. Sätt `autoConfigureOnStart` till true

### 3. Konfigurera Room Settings
1. **Lighting:** Sätt ambient färg, intensitet och skybox
2. **Atmosphere:** Lägg till partiklar, ambient ljud och fog
3. **Puzzles:** Konfigurera pussel-objekt och relic slots
4. **VR Comfort:** Sätt comfort-settings och teleportation

### 4. Integrera med VR Core System
1. Se till att VR Core System är korrekt uppsatt
2. Verifiera att VREnvironmentSystem hittar VR-komponenter
3. Testa rum-övergångar och VR-funktioner

## Exempel på Room-konfigurationer

### Enkel Puzzle Room
```csharp
// Skapa puzzle room template
var puzzleRoom = gameObject.AddComponent<VRRoomTemplate>();
puzzleRoom.templateType = RoomTemplateType.PuzzleRoom;
puzzleRoom.roomName = "Crystal Puzzle Chamber";

// Konfigurera template settings
puzzleRoom.templateSettings.ambientColor = Color.cyan;
puzzleRoom.templateSettings.ambientIntensity = 0.3f;
puzzleRoom.templateSettings.enableFog = true;
puzzleRoom.templateSettings.fogColor = Color.blue;
puzzleRoom.templateSettings.fogDensity = 0.02f;

// Konfigurera rummet
puzzleRoom.ConfigureRoomFromTemplate();
```

### Dramatisk Boss Room
```csharp
// Skapa boss room template
var bossRoom = gameObject.AddComponent<VRRoomTemplate>();
bossRoom.templateType = RoomTemplateType.BossRoom;
bossRoom.roomName = "Shadow Lord's Throne";

// Konfigurera template settings
bossRoom.templateSettings.ambientColor = Color.red;
bossRoom.templateSettings.ambientIntensity = 0.1f;
bossRoom.templateSettings.enableFog = true;
bossRoom.templateSettings.fogColor = Color.darkRed;
bossRoom.templateSettings.fogDensity = 0.05f;

// Konfigurera rummet
bossRoom.ConfigureRoomFromTemplate();
```

## Events System

### Environment Events
```csharp
// Room events
VREnvironmentSystem.OnRoomChanged += OnRoomChanged;
VREnvironmentSystem.OnRoomTransitionStarted += OnRoomTransitionStarted;
VREnvironmentSystem.OnRoomTransitionCompleted += OnRoomTransitionCompleted;

// Template events
VRRoomTemplate.OnRoomTemplateConfigured += OnRoomTemplateConfigured;
```

### Event Handlers
```csharp
private void OnRoomChanged(VREnvironmentSystem.VRRoom newRoom)
{
    Debug.Log($"Entered room: {newRoom.roomName} ({newRoom.roomType})");
    
    // Update UI, spawn enemies, etc.
    if (newRoom.roomType == VREnvironmentSystem.RoomType.BossRoom)
    {
        StartBossFight();
    }
}

private void OnRoomTransitionStarted(VREnvironmentSystem.VRRoom fromRoom, VREnvironmentSystem.VRRoom toRoom)
{
    Debug.Log($"Transitioning from {fromRoom?.roomName ?? "none"} to {toRoom.roomName}");
    
    // Start transition effects
    StartTransitionEffects();
}
```

## Performance Optimering

### 1. Room Management
- **Lazy Loading:** Ladda rum endast när de behövs
- **Object Pooling:** Återanvänd rum-objekt
- **LOD System:** Enklare versioner av rum på avstånd

### 2. Lighting
- **Light Culling:** Inaktivera ljus utanför spelarens synfält
- **Shadow Distance:** Begränsa shadow rendering
- **Light Baking:** Förbaka statisk ljusning

### 3. Particles
- **Particle Limits:** Begränsa antalet aktiva partiklar
- **Distance Culling:** Stoppa partiklar på avstånd
- **LOD Particles:** Enklare partiklar på avstånd

### 4. Audio
- **Spatial Audio:** 3D-ljud endast för nära objekt
- **Audio Pooling:** Återanvänd audio sources
- **Volume Scaling:** Skala ljud baserat på avstånd

## Troubleshooting

### Vanliga problem:

1. **Rum laddas inte**
   - Kontrollera VREnvironmentSystem setup
   - Verifiera att rum finns i `availableRooms` array
   - Kontrollera console för felmeddelanden

2. **Ljus fungerar inte**
   - Verifiera att Light-komponenter finns
   - Kontrollera Layer-settings för ljus
   - Se till att RenderSettings är korrekt konfigurerade

3. **Partiklar visas inte**
   - Kontrollera ParticleSystem-komponenter
   - Verifiera att partiklar är aktiverade
   - Kontrollera material och texture-settings

4. **VR Comfort fungerar inte**
   - Verifiera VRCameraRig setup
   - Kontrollera VR Manager integration
   - Se till att comfort-settings är aktiverade

5. **Performance problem**
   - Begränsa antalet aktiva rum
   - Optimera partiklar och ljus
   - Använd LOD-system för komplexa objekt

## Nästa steg

Med VR Environment System på plats kan du nu:

1. **Skapa komplexa VR-miljöer** - Kombinera olika rumstyper
2. **Implementera dynamiska miljöer** - Rum som ändras baserat på progression
3. **Bygga VR-levels** - Skapa hela spelvärldar med rum-systemet
4. **Skapa VR-tutorials** - Interaktiva miljöer för att lära spelare
5. **Implementera VR-storytelling** - Miljöer som berättar historier

## Exempel på Avancerade Miljöer

### Dynamisk Miljö
```csharp
public class DynamicVREnvironment : MonoBehaviour
{
    [SerializeField] private VREnvironmentSystem environmentSystem;
    [SerializeField] private VRRoomTemplate[] roomTemplates;
    
    private void Start()
    {
        // Skapa dynamisk miljö baserat på spelarens progression
        CreateDynamicEnvironment();
    }
    
    private void CreateDynamicEnvironment()
    {
        // Skapa rum baserat på spelarens level
        int playerLevel = GameManager.Instance.GetPlayerLevel();
        
        if (playerLevel >= 5)
        {
            // Skapa boss room
            CreateBossRoom();
        }
        
        if (playerLevel >= 3)
        {
            // Skapa relic chamber
            CreateRelicChamber();
        }
    }
}
```

### Miljö som Reagerar på Spelaren
```csharp
public class ResponsiveVREnvironment : MonoBehaviour
{
    [SerializeField] private VREnvironmentSystem environmentSystem;
    
    private void Start()
    {
        // Prenumerera på spelar-events
        PlayerManager.OnPlayerHealthChanged += OnPlayerHealthChanged;
        PlayerManager.OnPlayerRelicCollected += OnPlayerRelicCollected;
    }
    
    private void OnPlayerHealthChanged(float healthPercentage)
    {
        // Ändra miljö baserat på spelarens hälsa
        if (healthPercentage < 0.3f)
        {
            // Aktivera varningseffekter
            ActivateWarningEffects();
        }
    }
    
    private void OnPlayerRelicCollected(GameObject relic)
    {
        // Öppna nya rum när relics samlas in
        UnlockNewRooms();
    }
}
```

## Support

För frågor eller problem med VR Environment System, kontakta utvecklingsteamet eller konsultera VR Core System dokumentationen.

## Integration med Andra System

### VR Core System
- **VRManager:** Rum-övergångar och VR-mode detection
- **VRCameraRig:** Comfort-settings och blink-effekter
- **VRInputManager:** Input för rum-navigation

### VR Puzzle System
- **VRRelicPuzzle:** Automatisk aktivering i puzzle rooms
- **VRHandGesturePuzzle:** Gesture-baserade pussel i miljön

### Game Systems
- **PlayerManager:** Spelarens position och progression
- **GameManager:** Spelstatus och level progression
- **AudioManager:** Ambient ljud och miljö-ljud
