# AI-Driven Gameplay System

## Översikt

AI-Driven Gameplay System är kärnan i Run4theRelic's intelligenta spelupplevelse. Systemet använder avancerad AI för att anpassa spelet i realtid baserat på spelarens beteende, prestanda och preferenser.

## Systemkomponenter

### 1. VRAIGameplayDirector
**Huvudkomponent** som styr hela AI-systemet och koordinerar alla andra komponenter.

#### Funktioner:
- **Automatisk svårighetsjustering** baserat på spelarprestanda
- **Adaptivt lärande** som bygger spelarprofil över tid
- **Dynamisk innehållsgenerering** anpassad till spelarens färdigheter
- **Realtidsanalys** av spelarinteraktioner och framsteg

#### Konfiguration:
```csharp
[Header("AI Director Settings")]
[SerializeField] private float updateInterval = 2.0f;
[SerializeField] private int maxDifficultyLevel = 10;
[SerializeField] private float difficultyAdjustmentSpeed = 0.5f;
```

#### API:
```csharp
public float GetCurrentDifficulty() => currentDifficulty;
public float GetTargetDifficulty() => targetDifficulty;
public Dictionary<string, float> GetPlayerSkillProfile() => playerSkillProfile;
public void SetDifficulty(float difficulty);
```

### 2. VRPerformanceAnalytics
**Avancerad analysmotor** som spårar detaljerade spelarmätvärden och genererar insikter.

#### Funktioner:
- **Realtidsprestandaspårning** (engagemang, frustration, mästerskap)
- **Beteendemönsterigenkänning** (rörelse, interaktion, pussellösning)
- **Prediktiv analys** för framtida prestanda
- **Automatisk insiktsgenerering** baserat på tröskelvärden

#### Mätvärden:
- **Engagemangsnivå**: Baserat på rörelse, interaktion och framsteg
- **Frustrationsnivå**: Identifierar när spelaren fastnar eller kämpar
- **Mästerskapsnivå**: Spårar utveckling över olika pusseltyper
- **Beteendemönster**: Konsistent aktivitet, låg interaktion, fastna på pussel

#### API:
```csharp
public void RecordPuzzleAttempt(PuzzleType puzzleType, bool success, float completionTime, float difficulty);
public float GetEngagementLevel() => currentEngagementLevel;
public PerformancePrediction PredictPerformance(PuzzleType puzzleType, float difficulty);
```

### 3. VRDynamicStoryGenerator
**Dynamiskt berättarsystem** som skapar kontextuella berättelseelement baserat på spelarens handlingar.

#### Funktioner:
- **Kontextuell berättelsegenerering** baserat på spelarstatus
- **Känslomässig berättelse** som anpassar ton efter spelarens humör
- **Mallbaserad generering** med flera variationer per berättelse
- **Automatisk livslängd** för berättelseelement

#### Berättelsekategorier:
- **Environmental**: Miljöbeskrivningar och atmosfär
- **PuzzleHint**: Ledtrådar och vägledning
- **Atmospheric**: Känslomässig ton och spänning
- **Progression**: Framsteg och mästerskap

#### API:
```csharp
public List<DynamicStoryElement> GetActiveStories() => activeStories;
public StoryMood GetCurrentMood() => currentMood;
public void ForceStoryGeneration(StoryCategory category = StoryCategory.Atmospheric);
```

## Integration och Kommunikation

### Event-System
Alla komponenter kommunicerar via Unity Events för lös koppling:

```csharp
// AI Director Events
public static event System.Action<float> OnDifficultyChanged;
public static event System.Action<StoryElement> OnStoryElementGenerated;
public static event System.Action<PlayerSkillUpdate> OnPlayerSkillUpdated;

// Performance Analytics Events
public static event Action<PerformanceInsight> OnNewInsight;
public static event Action<float> OnEngagementChanged;
public static event Action<float> OnFrustrationChanged;

// Story Generator Events
public static event Action<DynamicStoryElement> OnStoryGenerated;
public static event Action<StoryMood> OnMoodChanged;
```

### Dataflöde
1. **VRPerformanceAnalytics** samlar in data kontinuerligt
2. **VRAIGameplayDirector** analyserar data och justerar svårighet
3. **VRDynamicStoryGenerator** skapar berättelser baserat på AI-beslut
4. **VRContentGenerator** anpassar innehåll baserat på AI-direktiv

## Konfiguration och Anpassning

### AI Director Inställningar
```csharp
[Header("AI Director Settings")]
[SerializeField] private float updateInterval = 2.0f;           // AI-uppdateringsfrekvens
[SerializeField] private int maxDifficultyLevel = 10;           // Max svårighetsnivå
[SerializeField] private float difficultyAdjustmentSpeed = 0.5f; // Justeringshastighet

[Header("Performance Tracking")]
[SerializeField] private float minPerformanceThreshold = 0.3f;  // Låg prestanda tröskel
[SerializeField] private float maxPerformanceThreshold = 0.8f;  // Hög prestanda tröskel
```

### Performance Analytics Inställningar
```csharp
[Header("Analytics Settings")]
[SerializeField] private float dataCollectionInterval = 1.0f;   // Datainsamlingsfrekvens
[SerializeField] private int maxDataPoints = 1000;              // Max datapunkter
[SerializeField] private bool enableRealTimeAnalysis = true;    // Realtidsanalys
[SerializeField] private bool enablePerformancePrediction = true; // Prediktiv analys

[Header("Performance Thresholds")]
[SerializeField] private float frustrationThreshold = 0.2f;     // Frustrationströskel
[SerializeField] private float engagementThreshold = 0.7f;      // Engagemangströskel
[SerializeField] private float masteryThreshold = 0.9f;         // Mästerskapströskel
```

### Story Generator Inställningar
```csharp
[Header("Story Generation Settings")]
[SerializeField] private float storyUpdateInterval = 15.0f;     // Berättelseuppdateringsfrekvens
[SerializeField] private int maxActiveStories = 8;              // Max aktiva berättelser
[SerializeField] private bool enableContextualGeneration = true; // Kontextuell generering
[SerializeField] private bool enableEmotionalStorytelling = true; // Känslomässig berättelse

[Header("Generation Parameters")]
[SerializeField] private float storyRelevanceThreshold = 0.6f;  // Relevanstroskel
[SerializeField] private float emotionalIntensityMultiplier = 1.0f; // Känslointensitet
[SerializeField] private int maxStoryVariations = 5;            // Max berättelsevariationer
```

## Användning i Unity

### Setup
1. Lägg till `VRAIGameplayDirector` på ett GameObject i scenen
2. Lägg till `VRPerformanceAnalytics` på samma eller separat GameObject
3. Lägg till `VRDynamicStoryGenerator` för berättelsefunktionalitet
4. Konfigurera inställningarna enligt dina behov

### Integration med Befintliga System
```csharp
// I dina pussel-scripts
private void OnPuzzleCompleted()
{
    // Registrera prestanda för AI-systemet
    var analytics = FindObjectOfType<VRPerformanceAnalytics>();
    if (analytics != null)
    {
        analytics.RecordPuzzleAttempt(
            PuzzleType.RelicPlacement, 
            true, 
            Time.time - startTime, 
            currentDifficulty
        );
    }
}
```

### Anpassning av Berättelser
```csharp
// Lägg till anpassade berättelsemallar
var storyGenerator = FindObjectOfType<VRDynamicStoryGenerator>();
if (storyGenerator != null)
{
    storyGenerator.ForceStoryGeneration(StoryCategory.Atmospheric);
}
```

## Prestanda och Optimering

### Datainsamling
- **Intervall**: Justerbart från 0.5s till 5.0s
- **Lagring**: Automatisk rensning av gamla datapunkter
- **Minne**: Konfigurerbar max datapunkter (standard: 1000)

### AI-beräkningar
- **Uppdateringsfrekvens**: Justerbart från 1s till 10s
- **Komplexitet**: O(n) för de flesta algoritmer
- **Caching**: Automatisk cachning av beräkningsresultat

### Berättelsegenerering
- **Frekvens**: Justerbart från 10s till 60s
- **Variation**: Automatisk rotation av berättelseteman
- **Livslängd**: Kontextbaserad livslängd för berättelser

## Felsökning

### Vanliga Problem
1. **AI reagerar inte**: Kontrollera att `updateInterval` inte är för hög
2. **Berättelser upprepas**: Justera `maxStoryVariations` och `storyRelevanceThreshold`
3. **Prestanda låg**: Minska `dataCollectionInterval` och `maxDataPoints`

### Debug-information
```csharp
// Aktivera debug-loggning
Debug.Log($"AI Difficulty: {aiDirector.GetCurrentDifficulty()}");
Debug.Log($"Player Engagement: {analytics.GetEngagementLevel()}");
Debug.Log($"Active Stories: {storyGenerator.GetActiveStories().Count}");
```

## Framtida Utveckling

### Planerade Funktioner
- **Machine Learning Integration**: Djupare analys av spelarbeteende
- **Emotional AI**: Mer sofistikerad känslomässig anpassning
- **Multiplayer AI**: AI-styrning för flera spelare
- **Voice Integration**: Röstbaserade berättelser och interaktioner

### Utbyggbarhet
Systemet är designat för enkel utbyggning:
- Nya berättelsekategorier kan läggas till
- Fler prestandamätvärden kan integreras
- AI-algoritmer kan ersättas eller utökas
- Berättelsemallar kan laddas från externa källor

## Teknisk Arkitektur

### Designmönster
- **Observer Pattern**: Event-baserad kommunikation
- **Strategy Pattern**: Konfigurerbara AI-algoritmer
- **Factory Pattern**: Berättelsegenerering
- **Singleton Pattern**: Centrala systemkomponenter

### Datastrukturer
- **Queue**: Prestandahistorik och kontexthistorik
- **Dictionary**: Pusselanalys och berättelsemallar
- **List**: Aktiva berättelser och insikter
- **Struct**: Prestandadata och berättelseelement

### Minne och Prestanda
- **Garbage Collection**: Minimala allokeringar under runtime
- **Object Pooling**: Återanvändning av berättelseelement
- **Lazy Loading**: Berättelsemallar laddas vid behov
- **Efficient Algorithms**: O(n) eller bättre för alla operationer
