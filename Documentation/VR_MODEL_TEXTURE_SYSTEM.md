# VR Model & Texture System - Run4theRelic

## Översikt

VR Model & Texture System är ett komplett system för att hantera 3D-modeller, texturer och material för olika rumstyper i Run4theRelic. Systemet inkluderar automatisk modell-generering, texture-hantering och material-skapande för att skapa immersiva VR-miljöer.

## Komponenter

### 1. VRModelTextureManager
**Fil:** `Assets/Scripts/Core/VRModelTextureManager.cs`

Huvudsystemet som hanterar alla 3D-modeller och texturer:
- **Automatisk modell-laddning** för alla rumstyper
- **Texture-hantering** med kompression och caching
- **Material-skapande** med instancing-optimering
- **Model pooling** för prestanda-optimering
- **Event-system** för modell- och texture-laddning

**Användning:**
```csharp
// Hitta model texture manager
var modelManager = FindObjectOfType<VRModelTextureManager>();

// Hämta modell-set för specifikt rum
var modelSet = modelManager.GetModelSet(RoomType.PuzzleRoom);

// Hämta modell från pool
var wallModel = modelManager.GetModelFromPool("Library_Wall_Stone");

// Hämta material från cache
var stoneMaterial = modelManager.GetMaterialFromCache("Library_Stone");

// Hämta texture från cache
var stoneTexture = modelManager.GetTextureFromCache("Library_Stone");
```

### 2. VRModelGenerator
**Fil:** `Assets/Scripts/Core/VRModelGenerator.cs`

Systemet som genererar 3D-modeller programmatiskt:
- **Procedural geometry** för väggar, golv och tak
- **Rum-specifika modeller** baserat på rumstyp
- **Complexity-baserad generering** för olika detaljnivåer
- **Automatisk material-tillämpning** på genererade modeller
- **Collision-generering** för fysiska interaktioner

**Användning:**
```csharp
// Hitta model generator
var modelGenerator = FindObjectOfType<VRModelGenerator>();

// Generera modeller för specifikt rum
modelGenerator.GenerateRoomModels(RoomType.PuzzleRoom);

// Generera alla rum-modeller
modelGenerator.GenerateAllRoomModels();

// Kontrollera genererings-progress
float progress = modelGenerator.GetGenerationProgress(RoomType.Beginner);
```

## Rumstyper och Modeller

### Tutorial Chamber (Entrance)
**Atmosfär:** Enkel och välkomnande
**Modeller:**
- **Väggar:** Tutorial_Wall_Simple, Tutorial_Wall_Plain
- **Golv:** Tutorial_Floor_Stone, Tutorial_Floor_Wood
- **Tak:** Tutorial_Ceiling_Flat
- **Dekorationer:** Tutorial_Torch, Tutorial_Banner
- **Möbler:** Tutorial_Table, Tutorial_Chair

**Texturer:**
- **Tutorial_Stone:** Ljus grå sten (0.8, 0.8, 0.8) med roughness 0.7
- **Tutorial_Wood:** Varm trä (0.6, 0.4, 0.2) med roughness 0.8

### Ancient Library (Corridor)
**Atmosfär:** Varm och inbjudande biblioteks-miljö
**Modeller:**
- **Väggar:** Library_Wall_Stone, Library_Wall_Bookcase, Library_Wall_Arch
- **Golv:** Library_Floor_Marble, Library_Floor_Carpet
- **Tak:** Library_Ceiling_Vaulted, Library_Ceiling_Beam
- **Dekorationer:** Library_Candle, Library_Scroll, Library_Plant
- **Möbler:** Library_Desk, Library_Bookshelf, Library_Chair_Leather

**Texturer:**
- **Library_Stone:** Varm sten (0.7, 0.6, 0.5) med roughness 0.6
- **Library_Wood_Dark:** Mörkt trä (0.3, 0.2, 0.1) med roughness 0.7
- **Library_Leather:** Läder (0.4, 0.3, 0.2) med roughness 0.9

### Crystal Cavern (PuzzleRoom)
**Atmosfär:** Mystisk kristall-grotta med organisk geometri
**Modeller:**
- **Väggar:** Cavern_Wall_Crystal, Cavern_Wall_Stalactite, Cavern_Wall_Geode
- **Golv:** Cavern_Floor_Crystal, Cavern_Floor_Stalagmite
- **Tak:** Cavern_Ceiling_Crystal, Cavern_Ceiling_Stalactite
- **Dekorationer:** Cavern_Crystal_Cluster, Cavern_Crystal_Shard, Cavern_Crystal_Growth
- **Möbler:** Cavern_Altar, Cavern_Pedestal

**Texturer:**
- **Cavern_Crystal:** Kristall-blå (0.6, 0.8, 1.0) med låg roughness 0.1
- **Cavern_Stone:** Mörk sten (0.4, 0.4, 0.5) med hög roughness 0.8

### Shadow Sanctum (RelicChamber)
**Atmosfär:** Skrämmande skugga-sanctum med mörk geometri
**Modeller:**
- **Väggar:** Sanctum_Wall_Shadow, Sanctum_Wall_Dark, Sanctum_Wall_Ancient
- **Golv:** Sanctum_Floor_Shadow, Sanctum_Floor_Dark
- **Tak:** Sanctum_Ceiling_Shadow, Sanctum_Ceiling_Dark
- **Dekorationer:** Sanctum_Shadow_Effect, Sanctum_Dark_Orb, Sanctum_Ancient_Rune
- **Möbler:** Sanctum_Throne, Sanctum_Altar_Dark

**Texturer:**
- **Sanctum_Shadow:** Djup lila (0.2, 0.1, 0.3) med hög roughness 0.9
- **Sanctum_Dark:** Mörk lila (0.1, 0.05, 0.15) med hög roughness 0.8

### Void Nexus (BossRoom)
**Atmosfär:** Andra världslig energi-nexus med energifält
**Modeller:**
- **Väggar:** Nexus_Wall_Void, Nexus_Wall_Energy, Nexus_Wall_Reality
- **Golv:** Nexus_Floor_Void, Nexus_Floor_Energy
- **Tak:** Nexus_Ceiling_Void, Nexus_Ceiling_Energy
- **Dekorationer:** Nexus_Energy_Field, Nexus_Reality_Shard, Nexus_Void_Portal
- **Möbler:** Nexus_Energy_Throne, Nexus_Reality_Altar

**Texturer:**
- **Nexus_Energy:** Elektrisk cyan (0.1, 0.6, 0.7) med låg roughness 0.2 och hög metallic 0.8
- **Nexus_Void:** Djup svart (0.05, 0.1, 0.15) med hög roughness 0.9

### Eternal Abyss (Exit)
**Atmosfär:** Apokalyptisk evig avgrund med kaotisk geometri
**Modeller:**
- **Väggar:** Abyss_Wall_Apocalypse, Abyss_Wall_Destruction, Abyss_Wall_Chaos
- **Golv:** Abyss_Floor_Apocalypse, Abyss_Floor_Destruction
- **Tak:** Abyss_Ceiling_Apocalypse, Abyss_Ceiling_Destruction
- **Dekorationer:** Abyss_Apocalypse_Effect, Abyss_Destruction_Field, Abyss_Chaos_Portal
- **Möbler:** Abyss_Apocalypse_Throne, Abyss_Destruction_Altar

**Texturer:**
- **Abyss_Apocalypse:** Blod-röd (0.6, 0.1, 0.1) med hög roughness 0.8
- **Abyss_Destruction:** Mörk röd (0.4, 0.05, 0.05) med hög roughness 0.9

## Modell-kategorier

### Wall Models
**Syfte:** Rummets väggar och strukturer
**Generering:** Automatisk placering runt rummets periferi
**Komplexitet:** Enkel till mycket hög baserat på rumstyp
**Collision:** Automatisk mesh collider-generering

### Floor Models
**Syfte:** Rummets golv och underlag
**Generering:** Cirkulära golv med segment-baserad geometri
**Komplexitet:** Anpassar sig efter rumstyp och detaljnivå
**UV-mapping:** Automatisk UV-generering för texturer

### Ceiling Models
**Syfte:** Rummets tak och överliggare
**Generering:** Inverterade golv-modeller med korrekta normaler
**Komplexitet:** Matchar golv-komplexitet
**Positionering:** Automatisk placering vid vägg-höjd

### Decoration Models
**Syfte:** Rummets dekorativa element
**Generering:** Sfäriska modeller med slumpmässig placering
**Komplexitet:** Varierar från enkla till komplexa former
**Skalning:** Slumpmässig storlek för variation

### Furniture Models
**Syfte:** Rummets möbler och funktionella objekt
**Generering:** Box-geometri med automatisk material-tillämpning
**Placering:** Slumpmässig positionering i rummet
**Skalning:** Anpassad storlek baserat på rumstyp

## Texture-system

### Texture Sets
Varje rumstyp har sina egna texture-sets med:
- **Base Color:** Grundfärg för materialet
- **Roughness:** Ytans grovhet (0-1)
- **Metallic:** Metallisk kvalitet (0-1)
- **Normal Strength:** Normal map-intensitet

### Automatisk Material-skapande
Systemet skapar automatiskt material med:
- **Standard Shader:** Unity's standard shader
- **Automatisk property-setting:** Färg, roughness, metallic
- **Texture-tillämpning:** Automatisk texture-tillämpning
- **Material caching:** Återanvändning av material

### Texture-optimering
- **Kompression:** Automatisk texture-kompression
- **Mip-map generering:** Automatisk mip-map skapande
- **Storleks-begränsning:** Konfigurerbar max-texture-storlek
- **Memory management:** Automatisk texture-caching

## Modell-generering

### Procedural Geometry
Systemet genererar geometri programmatiskt för:

#### Enkla Väggar
- **Geometri:** Rektangulära väggar med 4 vertices
- **Trianglar:** 2 trianglar för front- och baksida
- **UV-mapping:** Enkel UV-mapping för texturer

#### Biblioteks-väggar
- **Geometri:** Väggar med arkade toppar
- **Trianglar:** 4 trianglar för vägg och ark
- **Detaljer:** Arkad-överlagring för arkitektonisk stil

#### Grotta-väggar
- **Geometri:** Organiska, oregelbundna former
- **Trianglar:** Segment-baserad triangulering
- **Variation:** Höjd-variation med sinus-funktioner

#### Sanctum-väggar
- **Geometri:** Väggar med skugga-effekter
- **Trianglar:** Dubbla lager för skugga-överlagring
- **Atmosfär:** Mörk, skrämmande utseende

#### Nexus-väggar
- **Geometri:** Energifält med djup-effekt
- **Trianglar:** 4-vertex system för energifält
- **Effekter:** 3D-energifält med variation

#### Abyss-väggar
- **Geometri:** Kaotiska, apokalyptiska former
- **Trianglar:** Perlin noise-baserad variation
- **Komplexitet:** Mycket hög detaljnivå

### Automatisk Placering
- **Väggar:** Cirkulär placering runt rummets centrum
- **Golv/Tak:** Centrerade i rummet
- **Dekorationer:** Slumpmässig placering inom rummet
- **Möbler:** Slumpmässig placering med kollisions-undantag

### Skalning och Rotation
- **Komplexitet-baserad skalning:** Högre komplexitet = större modeller
- **Automatisk rotation:** Väggar roteras för att vända mot rummet
- **Slumpmässig variation:** Dekorationer och möbler får slumpmässig storlek

## Performance Optimering

### Model Pooling
- **Automatisk pooling:** Modeller lagras i pool för återanvändning
- **Memory management:** Begränsad pool-storlek
- **Lazy loading:** Modeller laddas endast vid behov

### LOD System
- **Automatisk LOD-generering:** Olika detaljnivåer baserat på avstånd
- **Complexity-baserad LOD:** Högre komplexitet = fler LOD-nivåer
- **Performance-optimering:** Enklare modeller på avstånd

### Collision Optimization
- **Automatisk collider-generering:** Mesh colliders för alla modeller
- **Collision-kategorier:** Olika collider-typer för olika modell-kategorier
- **Performance-tuning:** Konfigurerbar collision-generering

### Material Instancing
- **Automatisk instancing:** Samma material återanvänds mellan modeller
- **Memory reduction:** Minska material-memory-användning
- **Batch rendering:** Förbättra rendering-prestanda

## Setup i Unity

### 1. Skapa Model Texture Manager
1. Skapa en tom GameObject i scenen
2. Lägg till `VRModelTextureManager` komponenten
3. Konfigurera `autoLoadModels` till true
4. Sätt `enableModelPooling` till true

### 2. Skapa Model Generator
1. Skapa en tom GameObject i scenen
2. Lägg till `VRModelGenerator` komponenten
3. Konfigurera `enableProceduralGeneration` till true
4. Sätt `enableCollisionGeneration` till true

### 3. Konfigurera Settings
1. **Model Management:**
   - `autoLoadModels`: Automatisk modell-laddning
   - `enableModelPooling`: Modell-pooling för prestanda
   - `maxPooledModels`: Maximalt antal poolade modeller

2. **Texture Management:**
   - `autoLoadTextures`: Automatisk texture-laddning
   - `enableTextureCompression`: Texture-kompression
   - `maxTextureSize`: Maximal texture-storlek

3. **Material Management:**
   - `autoCreateMaterials`: Automatisk material-skapande
   - `enableMaterialInstancing`: Material-instancing

4. **Generation Settings:**
   - `enableProceduralGeneration`: Procedural modell-generering
   - `enableLODGeneration`: LOD-system
   - `enableCollisionGeneration`: Automatisk collision-generering

### 4. Integrera med VR Core System
1. Se till att VR Core System är korrekt uppsatt
2. Verifiera att VREnvironmentSystem finns
3. Testa modell-generering och texture-hantering

## Exempel på Användning

### Skapa Anpassade Modell-sets
```csharp
public class CustomModelCreator : MonoBehaviour
{
    [SerializeField] private VRModelTextureManager modelManager;
    
    public void CreateCustomRoomModels()
    {
        // Skapa anpassat modell-set
        var customModelSet = new RoomModelSet
        {
            roomType = RoomType.PuzzleRoom,
            roomName = "Custom Puzzle Room",
            description = "A custom puzzle room with unique models",
            
            wallModels = new ModelInfo[]
            {
                new ModelInfo 
                { 
                    name = "Custom_Wall", 
                    category = ModelCategory.Wall, 
                    complexity = ModelComplexity.High 
                }
            },
            
            textureSets = new TextureSet[]
            {
                new TextureSet
                {
                    name = "Custom_Texture",
                    category = TextureCategory.Wall,
                    baseColor = Color.cyan,
                    roughness = 0.3f,
                    metallic = 0.7f
                }
            }
        };
        
        // Lägg till anpassat modell-set
        // Detta skulle integrera med VRModelTextureManager
        Debug.Log("Created custom model set");
    }
}
```

### Hantera Modell-generering
```csharp
public class ModelGenerationController : MonoBehaviour
{
    private VRModelGenerator modelGenerator;
    private VRModelTextureManager modelManager;
    
    private void Start()
    {
        modelGenerator = FindObjectOfType<VRModelGenerator>();
        modelManager = FindObjectOfType<VRModelTextureManager>();
        
        // Prenumerera på events
        VRModelGenerator.OnModelGenerated += OnModelGenerated;
        VRModelGenerator.OnRoomModelsGenerated += OnRoomModelsGenerated;
    }
    
    private void OnModelGenerated(string modelName)
    {
        Debug.Log($"Generated model: {modelName}");
        // Hantera genererad modell
    }
    
    private void OnRoomModelsGenerated(RoomType roomType)
    {
        Debug.Log($"Generated all models for room type: {roomType}");
        // Hantera komplett rum-generering
    }
    
    public void GenerateSpecificRoom(RoomType roomType)
    {
        if (modelGenerator != null)
        {
            modelGenerator.GenerateRoomModels(roomType);
        }
    }
    
    public void CheckGenerationProgress(RoomType roomType)
    {
        if (modelGenerator != null)
        {
            float progress = modelGenerator.GetGenerationProgress(roomType);
            Debug.Log($"Generation progress for {roomType}: {progress}%");
        }
    }
}
```

### Dynamisk Modell-hantering
```csharp
public class DynamicModelManager : MonoBehaviour
{
    [SerializeField] private VRModelTextureManager modelManager;
    [SerializeField] private Transform modelParent;
    
    public void SpawnModelInRoom(string modelName, RoomType roomType)
    {
        if (modelManager == null) return;
        
        // Hämta modell från pool
        GameObject model = modelManager.GetModelFromPool(modelName);
        
        if (model != null)
        {
            // Placera modell i rummet
            model.transform.SetParent(modelParent);
            
            // Anpassa position baserat på rumstyp
            Vector3 position = GetPositionForRoomType(roomType);
            model.transform.position = position;
            
            Debug.Log($"Spawned model {modelName} in {roomType} room");
        }
    }
    
    private Vector3 GetPositionForRoomType(RoomType roomType)
    {
        // Anpassa position baserat på rumstyp
        switch (roomType)
        {
            case RoomType.Entrance:
                return new Vector3(0f, 0f, 0f);
            case RoomType.PuzzleRoom:
                return new Vector3(2f, 0f, 2f);
            default:
                return Vector3.zero;
        }
    }
    
    public void ApplyCustomMaterial(string modelName, string materialName)
    {
        if (modelManager == null) return;
        
        // Hämta material från cache
        Material material = modelManager.GetMaterialFromCache(materialName);
        
        if (material != null)
        {
            // Hitta modell och applicera material
            GameObject model = modelManager.GetModelFromPool(modelName);
            if (model != null)
            {
                var renderer = model.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.material = material;
                    Debug.Log($"Applied material {materialName} to {modelName}");
                }
            }
        }
    }
}
```

## Events System

### Model Events
```csharp
// Modell-events
VRModelGenerator.OnModelGenerated += OnModelGenerated;
VRModelGenerator.OnRoomModelsGenerated += OnRoomModelsGenerated;

// Event handlers
private void OnModelGenerated(string modelName)
{
    Debug.Log($"Generated model: {modelName}");
    // Hantera genererad modell
}

private void OnRoomModelsGenerated(RoomType roomType)
{
    Debug.Log($"Generated all models for room type: {roomType}");
    // Hantera komplett rum-generering
}
```

### Texture Events
```csharp
// Texture-events från VRModelTextureManager
VRModelTextureManager.OnModelsLoaded += OnModelsLoaded;
VRModelTextureManager.OnModelLoaded += OnModelLoaded;
VRModelTextureManager.OnTextureLoaded += OnTextureLoaded;
VRModelTextureManager.OnMaterialCreated += OnMaterialCreated;

private void OnModelsLoaded(RoomType roomType)
{
    Debug.Log($"Models loaded for room type: {roomType}");
    // Starta modell-generering
}

private void OnTextureLoaded(string textureName)
{
    Debug.Log($"Loaded texture: {textureName}");
    // Hantera laddad texture
}

private void OnMaterialCreated(string materialName)
{
    Debug.Log($"Created material: {materialName}");
    // Hantera skapat material
}
```

## Performance Optimering

### 1. Model Management
- **Lazy Loading:** Ladda modeller endast när de behövs
- **Object Pooling:** Återanvänd modell-objekt
- **Memory Management:** Rensa oanvända modeller

### 2. Texture Optimization
- **Compression:** Använd texture-kompression
- **Mip-maps:** Generera mip-maps för bättre prestanda
- **Size Limits:** Begränsa texture-storlekar

### 3. Material Optimization
- **Instancing:** Använd material-instancing
- **Shader Optimization:** Optimerade shaders för VR
- **Batch Rendering:** Gruppera rendering-anrop

### 4. Geometry Optimization
- **LOD System:** Olika detaljnivåer baserat på avstånd
- **Culling:** Inaktivera modeller utanför synfält
- **Triangle Reduction:** Minska antalet trianglar på avstånd

## Troubleshooting

### Vanliga problem:

1. **Modeller genereras inte**
   - Kontrollera `enableProceduralGeneration` setting
   - Verifiera att VRModelTextureManager finns
   - Kontrollera console för felmeddelanden

2. **Texturer laddas inte**
   - Kontrollera `autoLoadTextures` setting
   - Verifiera texture-filer och sökvägar
   - Kontrollera texture-komprimering

3. **Material skapas inte**
   - Kontrollera `autoCreateMaterials` setting
   - Verifiera att Standard shader finns
   - Kontrollera texture-referenser

4. **Prestanda-problem**
   - Aktivera model pooling
   - Reducera texture-storlekar
   - Använd LOD-system

## Nästa steg

Med VR Model & Texture System på plats kan du nu:

1. **Skapa kompletta VR-rum** - Automatisk generering av alla rumselement
2. **Implementera custom modeller** - Lägg till egna 3D-modeller
3. **Skapa avancerade texturer** - Anpassa material och texturer
4. **Bygga VR-miljöer** - Skapa stora, detaljerade VR-världar
5. **Optimera prestanda** - Använd LOD och pooling-system

## Integration med Andra System

### VR Core System
- **VRManager:** VR-mode detection och setup
- **VREnvironmentSystem:** Rum-hantering och miljöer
- **VRCameraRig:** Comfort-settings och övergångar

### VR Environment System
- **VRRoomTemplate:** Rum-templates och konfiguration
- **VRRoomPresets:** Pre-konfigurerade rum-mallar
- **VRPuzzleLevelManager:** Nivå-hantering och progression

### VR Puzzle System
- **VRRelicPuzzle:** Fysiska pussel i rummen
- **VRHandGesturePuzzle:** Gesture-baserade pussel
- **PuzzleControllerBase:** Grundläggande pussel-funktionalitet

## Support

För frågor eller problem med VR Model & Texture System, kontakta utvecklingsteamet eller konsultera VR Core System och VR Environment System dokumentationen.
