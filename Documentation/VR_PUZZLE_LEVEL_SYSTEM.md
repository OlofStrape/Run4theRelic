# VR Puzzle Level System - Run4theRelic

## Översikt

VR Puzzle Level System är ett komplett system för att skapa och hantera olika pussel-nivåer i Run4theRelic. Varje nivå har sitt eget rum med unik atmosfär, ljusning och pussel-konfiguration. Systemet hanterar progression, låsning av nivåer och automatisk skapande av rum.

## Komponenter

### 1. VRRoomPresets
**Fil:** `Assets/Scripts/Core/VRRoomPresets.cs`

Ett statiskt system som innehåller färdiga konfigurationer för olika pussel-nivåer:
- **6 pussel-nivåer:** Tutorial, Beginner, Intermediate, Advanced, Expert, Master
- **Automatisk rum-konfiguration** med ljusning, atmosfär och VR-comfort
- **Pussel-generering** baserat på nivå-komplexitet
- **Rum-beskrivningar** och rekommenderade pussel-antal

**Användning:**
```csharp
// Skapa en komplett pussel-nivå
var levelSetup = VRRoomPresets.CreateCompletePuzzleLevel(PuzzleLevel.Beginner, parentObject);

// Skapa bara rummet
var roomTemplate = VRRoomPresets.CreatePuzzleLevelRoom(PuzzleLevel.Intermediate, parentObject);

// Hämta nivå-information
string description = VRRoomPresets.GetRoomDescription(PuzzleLevel.Advanced);
int puzzleCount = VRRoomPresets.GetRecommendedPuzzleCount(PuzzleLevel.Expert);
```

### 2. VRPuzzleLevelManager
**Fil:** `Assets/Scripts/Core/VRPuzzleLevelManager.cs`

Huvudsystemet som hanterar alla pussel-nivåer:
- **Automatisk skapande** av alla nivåer
- **Progression-system** med låsning av nivåer
- **Rum-hantering** med automatiska övergångar
- **Progress-tracking** och statistik
- **Event-system** för nivå-ändringar

**Användning:**
```csharp
// Hitta level manager
var levelManager = FindObjectOfType<VRPuzzleLevelManager>();

// Byt till specifik nivå
levelManager.SetCurrentLevel(PuzzleLevel.Intermediate);

// Kontrollera nivå-status
bool isUnlocked = levelManager.IsLevelUnlocked(PuzzleLevel.Advanced);
bool isCompleted = levelManager.IsLevelCompleted(PuzzleLevel.Beginner);
float progress = levelManager.GetLevelProgress(PuzzleLevel.Expert);

// Hämta statistik
var stats = levelManager.GetLevelStatistics();
```

## Pussel-nivåer

### Tutorial
- **Syfte:** Introduktion till VR-kontroller och grundläggande pussel
- **Atmosfär:** Ljus och välkomnande
- **Ljusning:** Mjuk blå-vit (0.9, 0.95, 1.0) med hög intensitet (0.4)
- **VR Comfort:** Maximal comfort (30° vändning)
- **Pussel:** 2 enkla relic-pussel
- **Krav:** Inga

### Beginner
- **Syfte:** Lätt introduktion till pussel-koncept
- **Atmosfär:** Fredlig biblioteks-miljö
- **Ljusning:** Varm ljusstake (1.0, 0.95, 0.8) med medel intensitet (0.35)
- **VR Comfort:** Hög comfort (35° vändning)
- **Pussel:** 3 pussel (blandning av relic och gesture)
- **Krav:** Inga

### Intermediate
- **Syfte:** Måttlig utmaning med mer komplexa pussel
- **Atmosfär:** Mystisk kristall-grotta
- **Ljusning:** Kristall-blå (0.7, 0.8, 1.0) med medel intensitet (0.3)
- **VR Comfort:** Måttlig comfort (40° vändning)
- **Pussel:** 4 pussel (mer komplexa konfigurationer)
- **Krav:** Slutför Beginner + 2 relics

### Advanced
- **Syfte:** Utmanande pussel för erfarna spelare
- **Atmosfär:** Skrämmande skugga-sanctum
- **Ljusning:** Djup lila (0.4, 0.3, 0.5) med låg intensitet (0.25)
- **VR Comfort:** Reducerad comfort (45° vändning)
- **Pussel:** 5 avancerade pussel
- **Krav:** Slutför Intermediate + 4 relics

### Expert
- **Syfte:** Mycket utmanande pussel
- **Atmosfär:** Energi-nexus där verkligheten böjs
- **Ljusning:** Elektrisk cyan (0.2, 0.8, 0.9) med låg intensitet (0.2)
- **VR Comfort:** Minimal comfort (50° vändning)
- **Pussel:** 6 expert-pussel
- **Krav:** Slutför Advanced + 6 relics

### Master
- **Syfte:** Ultimat test av skicklighet
- **Atmosfär:** Evig avgrund
- **Ljusning:** Blod-röd (0.8, 0.2, 0.2) med mycket låg intensitet (0.15)
- **VR Comfort:** Minimal comfort (55° vändning)
- **Pussel:** 7 master-pussel
- **Krav:** Slutför Expert + 8 relics

## Rum-konfigurationer

### Automatisk Setup
Varje rum konfigureras automatiskt med:
- **Ljusning:** Ambient färg, intensitet och skybox
- **Atmosfär:** Fog, partiklar och ambient ljud
- **VR Comfort:** Vändnings-vinklar och teleportation
- **Pussel-integration:** Automatisk aktivering av pussel

### Template-system
Rum använder `VRRoomTemplate` för:
- **Pre-konfigurerade inställningar** baserat på nivå
- **Automatisk spawning** av ljus, partiklar och dekorationer
- **Pussel-setup** med lämpliga komponenter
- **Relic slots** och teleportation-mål

## Pussel-konfiguration

### Baserat på Nivå
- **Tutorial:** Enkla relic-pussel med auto-snap
- **Beginner:** Blandning av relic och gesture-pussel
- **Intermediate:** Mer komplexa pussel med sekventiella gestures
- **Advanced+:** Avancerade pussel som kräver båda händerna

### Automatisk Placering
Pussel placeras automatiskt i rummet:
- **Cirkulär placering** runt rummets centrum
- **Avstånd:** 3 enheter från centrum
- **Rotation:** Jämnt fördelade vinklar

### Pussel-typer
- **VRRelicPuzzle:** Fysiska pussel med grabbing
- **VRHandGesturePuzzle:** Hand-gesture pussel
- **Kombinationer:** Blandning av olika pussel-typer

## Progression System

### Nivå-låsning
Nivåer låses upp baserat på:
- **Slutförda nivåer:** Måste slutföra föregående nivå
- **Relic-krav:** Måste samla in ett visst antal relics
- **Spelar-level:** Framtida integration med level-system

### Automatisk Framsteg
- **Auto-advance:** Automatisk övergång till nästa nivå
- **Progress-tracking:** Spårar framsteg i varje nivå
- **Completion-events:** Triggar events när nivåer slutförs

## Setup i Unity

### 1. Skapa Level Manager
1. Skapa en tom GameObject i scenen
2. Lägg till `VRPuzzleLevelManager` komponenten
3. Konfigurera `availableLevels` array
4. Sätt `autoCreateLevels` till true

### 2. Konfigurera Level Settings
1. **Level Management:**
   - `autoCreateLevels`: Automatisk skapande av nivåer
   - `availableLevels`: Vilka nivåer som ska skapas
   - `currentLevel`: Startnivå

2. **Level Settings:**
   - `enableProgression`: Automatisk framsteg mellan nivåer
   - `requireLevelCompletion`: Kräv slutförande för framsteg
   - `startingRelicCount`: Startantal relics

3. **Room Management:**
   - `levelParent`: Parent för alla nivå-rum
   - `autoTransitionRooms`: Automatiska rum-övergångar
   - `roomTransitionDelay`: Fördröjning mellan övergångar

### 3. Integrera med VR Core System
1. Se till att VR Core System är korrekt uppsatt
2. Verifiera att VREnvironmentSystem finns
3. Testa nivå-övergångar och rum-funktioner

## Exempel på Användning

### Skapa Enskild Nivå
```csharp
public class LevelCreator : MonoBehaviour
{
    [SerializeField] private Transform levelParent;
    
    public void CreateBeginnerLevel()
    {
        var levelSetup = VRRoomPresets.CreateCompletePuzzleLevel(
            PuzzleLevel.Beginner, 
            levelParent.gameObject
        );
        
        Debug.Log($"Created Beginner level with {levelSetup.puzzleCount} puzzles");
    }
}
```

### Hantera Nivå-övergångar
```csharp
public class LevelUI : MonoBehaviour
{
    private VRPuzzleLevelManager levelManager;
    
    private void Start()
    {
        levelManager = FindObjectOfType<VRPuzzleLevelManager>();
        
        // Prenumerera på events
        VRPuzzleLevelManager.OnLevelChanged += OnLevelChanged;
        VRPuzzleLevelManager.OnLevelCompleted += OnLevelCompleted;
    }
    
    private void OnLevelChanged(PuzzleLevel fromLevel, PuzzleLevel toLevel)
    {
        Debug.Log($"Level changed from {fromLevel} to {toLevel}");
        UpdateLevelUI(toLevel);
    }
    
    private void OnLevelCompleted(PuzzleLevel level)
    {
        Debug.Log($"Level {level} completed!");
        ShowCompletionMessage(level);
    }
    
    public void GoToNextLevel()
    {
        var currentLevel = levelManager.GetCurrentLevelSetup().level;
        var nextLevel = GetNextLevel(currentLevel);
        levelManager.SetCurrentLevel(nextLevel);
    }
}
```

### Dynamisk Nivå-skapande
```csharp
public class DynamicLevelSystem : MonoBehaviour
{
    [SerializeField] private VRPuzzleLevelManager levelManager;
    [SerializeField] private Transform dynamicLevelParent;
    
    public void CreateCustomLevel(PuzzleLevel baseLevel, int customPuzzleCount)
    {
        // Skapa anpassad nivå baserat på basnivå
        var customSetup = VRRoomPresets.CreateCompletePuzzleLevel(
            baseLevel, 
            dynamicLevelParent.gameObject, 
            customPuzzleCount
        );
        
        // Anpassa nivå-specifika inställningar
        CustomizeLevel(customSetup, baseLevel);
        
        Debug.Log($"Created custom {baseLevel} level with {customPuzzleCount} puzzles");
    }
    
    private void CustomizeLevel(PuzzleLevelSetup setup, PuzzleLevel baseLevel)
    {
        // Anpassa rum-atmosfär baserat på spelarens preferenser
        var roomTemplate = setup.roomTemplate;
        var settings = roomTemplate.templateSettings;
        
        // Anpassa ljusning baserat på spelarens VR-comfort
        if (PlayerPrefs.GetInt("VRComfortLevel") == 1)
        {
            settings.ambientIntensity *= 1.5f;
            settings.maxTurnAngle = 30f;
        }
    }
}
```

## Events System

### Level Events
```csharp
// Nivå-events
VRPuzzleLevelManager.OnLevelStarted += OnLevelStarted;
VRPuzzleLevelManager.OnLevelCompleted += OnLevelCompleted;
VRPuzzleLevelManager.OnLevelProgressChanged += OnLevelProgressChanged;
VRPuzzleLevelManager.OnLevelChanged += OnLevelChanged;

// Event handlers
private void OnLevelStarted(PuzzleLevel level)
{
    Debug.Log($"Started level: {level}");
    // Starta nivå-specifika system
    StartLevelSystems(level);
}

private void OnLevelCompleted(PuzzleLevel level)
{
    Debug.Log($"Completed level: {level}");
    // Visa completion UI, spela ljud, etc.
    ShowLevelCompletion(level);
}
```

### Room Events
```csharp
// Rum-events från VREnvironmentSystem
VREnvironmentSystem.OnRoomChanged += OnRoomChanged;
VREnvironmentSystem.OnRoomTransitionStarted += OnRoomTransitionStarted;
VREnvironmentSystem.OnRoomTransitionCompleted += OnRoomTransitionCompleted;

private void OnRoomChanged(VREnvironmentSystem.VRRoom newRoom)
{
    Debug.Log($"Entered room: {newRoom.roomName}");
    // Uppdatera UI, spawn enemies, etc.
}
```

## Performance Optimering

### 1. Level Management
- **Lazy Loading:** Ladda nivåer endast när de behövs
- **Object Pooling:** Återanvänd nivå-objekt
- **Memory Management:** Rensa oanvända nivåer

### 2. Room Optimization
- **Room Culling:** Inaktivera rum utanför spelarens synfält
- **LOD System:** Enklare versioner av rum på avstånd
- **Texture Streaming:** Ladda texturer baserat på behov

### 3. Puzzle Optimization
- **Puzzle Limits:** Begränsa antalet aktiva pussel
- **Distance Culling:** Stoppa pussel på avstånd
- **Component Pooling:** Återanvänd pussel-komponenter

## Troubleshooting

### Vanliga problem:

1. **Nivåer skapas inte**
   - Kontrollera `autoCreateLevels` setting
   - Verifiera att `levelParent` är tilldelad
   - Kontrollera console för felmeddelanden

2. **Rum visas inte**
   - Kontrollera att nivåer är aktiverade
   - Verifiera `SetCurrentLevel` anrop
   - Kontrollera GameObject aktivering

3. **Pussel fungerar inte**
   - Verifiera att pussel-komponenter är tillagda
   - Kontrollera pussel-konfiguration
   - Se till att VR Core System är uppsatt

4. **Progression fungerar inte**
   - Kontrollera `enableProgression` setting
   - Verifiera nivå-krav och relic-räkning
   - Kontrollera event-prenumerationer

## Nästa steg

Med VR Puzzle Level System på plats kan du nu:

1. **Skapa kompletta VR-spel** - Hela spelvärldar med progression
2. **Implementera save/load system** - Spara spelar-framsteg
3. **Skapa VR-tutorials** - Interaktiva lärande-upplevelser
4. **Bygga VR-campaigns** - Långa spel-upplevelser
5. **Implementera multiplayer** - Samarbeta eller tävla i pussel

## Integration med Andra System

### VR Core System
- **VRManager:** VR-mode detection och setup
- **VREnvironmentSystem:** Rum-hantering och miljöer
- **VRCameraRig:** Comfort-settings och övergångar

### VR Puzzle System
- **VRRelicPuzzle:** Fysiska pussel i rummen
- **VRHandGesturePuzzle:** Gesture-baserade pussel
- **PuzzleControllerBase:** Grundläggande pussel-funktionalitet

### Game Systems
- **PlayerManager:** Spelarens progression och relics
- **GameManager:** Spelstatus och level-hantering
- **AudioManager:** Nivå-specifikt ljud och atmosfär

## Support

För frågor eller problem med VR Puzzle Level System, kontakta utvecklingsteamet eller konsultera VR Core System och VR Environment System dokumentationen.
