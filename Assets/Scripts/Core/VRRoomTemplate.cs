using UnityEngine;
using Run4theRelic.Core;
using Run4theRelic.Puzzles;
using System.Collections.Generic;

namespace Run4theRelic.Core
{
    /// <summary>
    /// Provides pre-configured room templates for different room types in Run4theRelic.
    /// Makes it easy to create consistent and immersive VR environments.
    /// </summary>
    public class VRRoomTemplate : MonoBehaviour
    {
        [Header("Room Template")]
        [SerializeField] private RoomTemplateType templateType;
        [SerializeField] private string roomName = "New Room";
        [SerializeField] private string roomDescription = "A mysterious room";
        
        [Header("Room Configuration")]
        [SerializeField] private bool autoConfigureOnStart = true;
        [SerializeField] private bool enablePuzzleIntegration = true;
        [SerializeField] private bool enableAtmosphericEffects = true;
        
        [Header("Template Settings")]
        [SerializeField] private RoomTemplateSettings templateSettings;
        
        // Room Components
        private VREnvironmentSystem.VRRoom _configuredRoom;
        private List<GameObject> _spawnedObjects = new List<GameObject>();
        
        // Events
        public static event System.Action<VRRoomTemplate, VREnvironmentSystem.VRRoom> OnRoomTemplateConfigured;
        
        [System.Serializable]
        public class RoomTemplateSettings
        {
            [Header("Lighting")]
            public Color ambientColor = Color.white;
            public float ambientIntensity = 0.2f;
            public Material skyboxMaterial;
            public Light[] templateLights;
            
            [Header("Atmosphere")]
            public ParticleSystem[] templateParticles;
            public AudioClip[] templateAmbientSounds;
            public bool enableFog = false;
            public Color fogColor = Color.gray;
            public float fogDensity = 0.01f;
            
            [Header("Puzzle Integration")]
            public GameObject[] templatePuzzleObjects;
            public Transform[] templateRelicSlots;
            public bool autoActivatePuzzles = true;
            
            [Header("VR Comfort")]
            public bool enableComfortFeatures = true;
            public float maxTurnAngle = 45f;
            public bool enableTeleportation = true;
            public Transform[] templateTeleportTargets;
            
            [Header("Decoration")]
            public GameObject[] templateDecorations;
            public Transform[] decorationPositions;
        }
        
        public enum RoomTemplateType
        {
            Entrance,
            Corridor,
            PuzzleRoom,
            RelicChamber,
            BossRoom,
            Exit,
            Custom
        }
        
        protected override void Start()
        {
            if (autoConfigureOnStart)
            {
                ConfigureRoomFromTemplate();
            }
        }
        
        /// <summary>
        /// Configure room from template settings.
        /// </summary>
        public void ConfigureRoomFromTemplate()
        {
            // Create room configuration
            _configuredRoom = CreateRoomFromTemplate();
            
            // Spawn template objects
            SpawnTemplateObjects();
            
            // Setup puzzle integration
            if (enablePuzzleIntegration)
            {
                SetupPuzzleIntegration();
            }
            
            // Setup atmospheric effects
            if (enableAtmosphericEffects)
            {
                SetupAtmosphericEffects();
            }
            
            // Notify configuration complete
            OnRoomTemplateConfigured?.Invoke(this, _configuredRoom);
            
            Debug.Log($"VRRoomTemplate: Configured room '{roomName}' from template '{templateType}'");
        }
        
        /// <summary>
        /// Create a VRRoom from template settings.
        /// </summary>
        /// <returns>Configured VRRoom.</returns>
        private VREnvironmentSystem.VRRoom CreateRoomFromTemplate()
        {
            var room = new VREnvironmentSystem.VRRoom();
            
            // Basic room info
            room.roomName = roomName;
            room.roomDescription = roomDescription;
            room.roomType = GetRoomTypeFromTemplate();
            
            // Room settings
            room.isPuzzleRoom = templateType == RoomTemplateType.PuzzleRoom;
            room.requireRelicToEnter = templateType == RoomTemplateType.RelicChamber;
            
            // Environment settings
            room.skyboxMaterial = templateSettings.skyboxMaterial;
            room.ambientColor = templateSettings.ambientColor;
            room.ambientIntensity = templateSettings.ambientIntensity;
            
            // VR comfort settings
            room.enableComfortFeatures = templateSettings.enableComfortFeatures;
            room.maxTurnAngle = templateSettings.maxTurnAngle;
            room.enableTeleportation = templateSettings.enableTeleportation;
            
            // Puzzle integration
            room.autoActivatePuzzles = templateSettings.autoActivatePuzzles;
            
            return room;
        }
        
        /// <summary>
        /// Get room type from template type.
        /// </summary>
        /// <returns>Corresponding VREnvironmentSystem.RoomType.</returns>
        private VREnvironmentSystem.RoomType GetRoomTypeFromTemplate()
        {
            switch (templateType)
            {
                case RoomTemplateType.Entrance:
                    return VREnvironmentSystem.RoomType.Entrance;
                    
                case RoomTemplateType.Corridor:
                    return VREnvironmentSystem.RoomType.Corridor;
                    
                case RoomTemplateType.PuzzleRoom:
                    return VREnvironmentSystem.RoomType.PuzzleRoom;
                    
                case RoomTemplateType.RelicChamber:
                    return VREnvironmentSystem.RoomType.RelicChamber;
                    
                case RoomTemplateType.BossRoom:
                    return VREnvironmentSystem.RoomType.BossRoom;
                    
                case RoomTemplateType.Exit:
                    return VREnvironmentSystem.RoomType.Exit;
                    
                default:
                    return VREnvironmentSystem.RoomType.Corridor;
            }
        }
        
        /// <summary>
        /// Spawn template objects in the room.
        /// </summary>
        private void SpawnTemplateObjects()
        {
            // Spawn lights
            SpawnTemplateLights();
            
            // Spawn particles
            SpawnTemplateParticles();
            
            // Spawn decorations
            SpawnTemplateDecorations();
            
            // Spawn teleport targets
            SpawnTeleportTargets();
        }
        
        /// <summary>
        /// Spawn template lights in the room.
        /// </summary>
        private void SpawnTemplateLights()
        {
            if (templateSettings.templateLights == null) return;
            
            var roomLights = new List<Light>();
            
            foreach (var templateLight in templateSettings.templateLights)
            {
                if (templateLight != null)
                {
                    // Instantiate light at template position
                    GameObject spawnedLight = Instantiate(templateLight.gameObject, transform);
                    spawnedLight.name = $"TemplateLight_{templateLight.name}";
                    
                    // Add to spawned objects list
                    _spawnedObjects.Add(spawnedLight);
                    
                    // Get light component
                    var light = spawnedLight.GetComponent<Light>();
                    if (light != null)
                    {
                        roomLights.Add(light);
                    }
                }
            }
            
            // Update room configuration with spawned lights
            _configuredRoom.roomLights = roomLights.ToArray();
        }
        
        /// <summary>
        /// Spawn template particles in the room.
        /// </summary>
        private void SpawnTemplateParticles()
        {
            if (templateSettings.templateParticles == null) return;
            
            var roomParticles = new List<ParticleSystem>();
            
            foreach (var templateParticle in templateSettings.templateParticles)
            {
                if (templateParticle != null)
                {
                    // Instantiate particle system at template position
                    GameObject spawnedParticle = Instantiate(templateParticle.gameObject, transform);
                    spawnedParticle.name = $"TemplateParticle_{templateParticle.name}";
                    
                    // Add to spawned objects list
                    _spawnedObjects.Add(spawnedParticle);
                    
                    // Get particle system component
                    var particleSystem = spawnedParticle.GetComponent<ParticleSystem>();
                    if (particleSystem != null)
                    {
                        roomParticles.Add(particleSystem);
                    }
                }
            }
            
            // Update room configuration with spawned particles
            _configuredRoom.roomParticles = roomParticles.ToArray();
        }
        
        /// <summary>
        /// Spawn template decorations in the room.
        /// </summary>
        private void SpawnTemplateDecorations()
        {
            if (templateSettings.templateDecorations == null || templateSettings.decorationPositions == null) return;
            
            for (int i = 0; i < templateSettings.templateDecorations.Length && i < templateSettings.decorationPositions.Length; i++)
            {
                var decoration = templateSettings.templateDecorations[i];
                var position = templateSettings.decorationPositions[i];
                
                if (decoration != null && position != null)
                {
                    // Instantiate decoration at specified position
                    GameObject spawnedDecoration = Instantiate(decoration, position.position, position.rotation, transform);
                    spawnedDecoration.name = $"TemplateDecoration_{decoration.name}_{i}";
                    
                    // Add to spawned objects list
                    _spawnedObjects.Add(spawnedDecoration);
                }
            }
        }
        
        /// <summary>
        /// Spawn teleport targets in the room.
        /// </summary>
        private void SpawnTeleportTargets()
        {
            if (templateSettings.templateTeleportTargets == null) return;
            
            var teleportTargets = new List<Transform>();
            
            foreach (var templateTarget in templateSettings.templateTeleportTargets)
            {
                if (templateTarget != null)
                {
                    // Create teleport target at template position
                    GameObject teleportTarget = new GameObject($"TeleportTarget_{templateTarget.name}");
                    teleportTarget.transform.SetParent(transform);
                    teleportTarget.transform.position = templateTarget.position;
                    teleportTarget.transform.rotation = templateTarget.rotation;
                    
                    // Add teleport target component
                    var teleportArea = teleportTarget.AddComponent<UnityEngine.XR.Interaction.Toolkit.TeleportationArea>();
                    
                    // Add to spawned objects list
                    _spawnedObjects.Add(teleportTarget);
                    
                    // Add to teleport targets list
                    teleportTargets.Add(teleportTarget.transform);
                }
            }
            
            // Update room configuration with spawned teleport targets
            _configuredRoom.teleportTargets = teleportTargets.ToArray();
        }
        
        /// <summary>
        /// Setup puzzle integration for the room.
        /// </summary>
        private void SpawnTemplatePuzzles()
        {
            if (templateSettings.templatePuzzleObjects == null) return;
            
            var puzzleObjects = new List<GameObject>();
            
            foreach (var templatePuzzle in templateSettings.templatePuzzleObjects)
            {
                if (templatePuzzle != null)
                {
                    // Instantiate puzzle object at template position
                    GameObject spawnedPuzzle = Instantiate(templatePuzzle, transform);
                    spawnedPuzzle.name = $"TemplatePuzzle_{templatePuzzle.name}";
                    
                    // Add to spawned objects list
                    _spawnedObjects.Add(spawnedPuzzle);
                    
                    // Add to puzzle objects list
                    puzzleObjects.Add(spawnedPuzzle);
                }
            }
            
            // Update room configuration with spawned puzzles
            _configuredRoom.puzzleObjects = puzzleObjects.ToArray();
        }
        
        /// <summary>
        /// Setup puzzle integration for the room.
        /// </summary>
        private void SetupPuzzleIntegration()
        {
            // Spawn template puzzles
            SpawnTemplatePuzzles();
            
            // Setup relic slots
            SetupRelicSlots();
            
            // Configure puzzle types based on template
            ConfigurePuzzleTypes();
        }
        
        /// <summary>
        /// Setup relic slots for the room.
        /// </summary>
        private void SetupRelicSlots()
        {
            if (templateSettings.templateRelicSlots == null) return;
            
            var relicSlots = new List<Transform>();
            
            foreach (var templateSlot in templateSettings.templateRelicSlots)
            {
                if (templateSlot != null)
                {
                    // Create relic slot at template position
                    GameObject relicSlot = new GameObject($"RelicSlot_{templateSlot.name}");
                    relicSlot.transform.SetParent(transform);
                    relicSlot.transform.position = templateSlot.position;
                    relicSlot.transform.rotation = templateSlot.rotation;
                    
                    // Add relic slot component
                    var slotRenderer = relicSlot.AddComponent<MeshRenderer>();
                    var slotCollider = relicSlot.AddComponent<BoxCollider>();
                    
                    // Configure slot appearance
                    slotRenderer.material = new Material(Shader.Find("Standard"));
                    slotRenderer.material.color = Color.yellow;
                    slotCollider.isTrigger = true;
                    
                    // Add to spawned objects list
                    _spawnedObjects.Add(relicSlot);
                    
                    // Add to relic slots list
                    relicSlots.Add(relicSlot.transform);
                }
            }
            
            // Update room configuration with spawned relic slots
            _configuredRoom.relicSlots = relicSlots.ToArray();
        }
        
        /// <summary>
        /// Configure puzzle types based on template.
        /// </summary>
        private void ConfigurePuzzleTypes()
        {
            switch (templateType)
            {
                case RoomTemplateType.PuzzleRoom:
                    ConfigurePuzzleRoom();
                    break;
                    
                case RoomTemplateType.RelicChamber:
                    ConfigureRelicChamber();
                    break;
                    
                case RoomTemplateType.BossRoom:
                    ConfigureBossRoom();
                    break;
            }
        }
        
        /// <summary>
        /// Configure puzzle room specific settings.
        /// </summary>
        private void ConfigurePuzzleRoom()
        {
            // Add puzzle room components
            var puzzleRoom = gameObject.AddComponent<VRPuzzleRoom>();
            
            // Configure puzzle room settings
            if (puzzleRoom != null)
            {
                puzzleRoom.roomName = roomName;
                puzzleRoom.autoActivatePuzzles = templateSettings.autoActivatePuzzles;
            }
        }
        
        /// <summary>
        /// Configure relic chamber specific settings.
        /// </summary>
        private void ConfigureRelicChamber()
        {
            // Add relic chamber components
            var relicChamber = gameObject.AddComponent<VRRelicChamber>();
            
            // Configure relic chamber settings
            if (relicChamber != null)
            {
                relicChamber.roomName = roomName;
                relicChamber.requireRelicToEnter = true;
            }
        }
        
        /// <summary>
        /// Configure boss room specific settings.
        /// </summary>
        private void ConfigureBossRoom()
        {
            // Add boss room components
            var bossRoom = gameObject.AddComponent<VRBossRoom>();
            
            // Configure boss room settings
            if (bossRoom != null)
            {
                bossRoom.roomName = roomName;
                bossRoom.enableBossFight = true;
            }
        }
        
        /// <summary>
        /// Setup atmospheric effects for the room.
        /// </summary>
        private void SetupAtmosphericEffects()
        {
            // Setup fog if enabled
            if (templateSettings.enableFog)
            {
                SetupRoomFog();
            }
            
            // Setup ambient audio
            SetupAmbientAudio();
        }
        
        /// <summary>
        /// Setup room fog.
        /// </summary>
        private void SetupRoomFog()
        {
            // Create fog settings
            var fogSettings = new VREnvironmentSystem.FogSettings();
            fogSettings.enableFog = true;
            fogSettings.fogColor = templateSettings.fogColor;
            fogSettings.fogDensity = templateSettings.fogDensity;
            fogSettings.animateFog = true;
            fogSettings.fogAnimationSpeed = 0.3f;
            fogSettings.fogDensityVariation = 0.002f;
            
            // Apply fog settings
            RenderSettings.fog = true;
            RenderSettings.fogColor = fogSettings.fogColor;
            RenderSettings.fogDensity = fogSettings.fogDensity;
        }
        
        /// <summary>
        /// Setup ambient audio for the room.
        /// </summary>
        private void SetupAmbientAudio()
        {
            if (templateSettings.templateAmbientSounds == null) return;
            
            // Create audio source for ambient sounds
            GameObject audioObject = new GameObject("RoomAmbientAudio");
            audioObject.transform.SetParent(transform);
            audioObject.transform.localPosition = Vector3.zero;
            
            var audioSource = audioObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = true;
            audioSource.loop = true;
            audioSource.spatialBlend = 1f; // 3D audio
            audioSource.volume = 0.3f;
            
            // Set random ambient sound
            if (templateSettings.templateAmbientSounds.Length > 0)
            {
                AudioClip randomSound = templateSettings.templateAmbientSounds[Random.Range(0, templateSettings.templateAmbientSounds.Length)];
                audioSource.clip = randomSound;
                audioSource.Play();
            }
            
            // Add to spawned objects list
            _spawnedObjects.Add(audioObject);
            
            // Update room configuration with ambient sounds
            _configuredRoom.roomAmbientSounds = templateSettings.templateAmbientSounds;
        }
        
        /// <summary>
        /// Get the configured room.
        /// </summary>
        /// <returns>Configured VRRoom or null if not configured.</returns>
        public VREnvironmentSystem.VRRoom GetConfiguredRoom()
        {
            return _configuredRoom;
        }
        
        /// <summary>
        /// Get all spawned objects.
        /// </summary>
        /// <returns>List of spawned GameObjects.</returns>
        public List<GameObject> GetSpawnedObjects()
        {
            return _spawnedObjects;
        }
        
        /// <summary>
        /// Clear all spawned objects.
        /// </summary>
        public void ClearSpawnedObjects()
        {
            foreach (var spawnedObject in _spawnedObjects)
            {
                if (spawnedObject != null)
                {
                    DestroyImmediate(spawnedObject);
                }
            }
            
            _spawnedObjects.Clear();
            Debug.Log($"VRRoomTemplate: Cleared all spawned objects for room '{roomName}'");
        }
        
        /// <summary>
        /// Reset room to template state.
        /// </summary>
        public void ResetRoom()
        {
            // Clear spawned objects
            ClearSpawnedObjects();
            
            // Reconfigure room
            ConfigureRoomFromTemplate();
            
            Debug.Log($"VRRoomTemplate: Reset room '{roomName}' to template state");
        }
        
        /// <summary>
        /// Apply template settings to existing room.
        /// </summary>
        /// <param name="existingRoom">Existing room to apply template to.</param>
        public void ApplyTemplateToRoom(VREnvironmentSystem.VRRoom existingRoom)
        {
            if (existingRoom == null) return;
            
            // Update room settings with template values
            existingRoom.ambientColor = templateSettings.ambientColor;
            existingRoom.ambientIntensity = templateSettings.ambientIntensity;
            existingRoom.skyboxMaterial = templateSettings.skyboxMaterial;
            existingRoom.enableComfortFeatures = templateSettings.enableComfortFeatures;
            existingRoom.maxTurnAngle = templateSettings.maxTurnAngle;
            existingRoom.enableTeleportation = templateSettings.enableTeleportation;
            
            Debug.Log($"VRRoomTemplate: Applied template settings to room '{existingRoom.roomName}'");
        }
    }
    
    /// <summary>
    /// Base class for puzzle room functionality.
    /// </summary>
    public class VRPuzzleRoom : MonoBehaviour
    {
        [Header("Puzzle Room Settings")]
        public string roomName = "Puzzle Room";
        public bool autoActivatePuzzles = true;
        public float puzzleActivationDelay = 1f;
        
        private void Start()
        {
            if (autoActivatePuzzles)
            {
                Invoke(nameof(ActivatePuzzles), puzzleActivationDelay);
            }
        }
        
        private void ActivatePuzzles()
        {
            // Find and activate puzzle objects
            var puzzles = GetComponentsInChildren<VRRelicPuzzle>();
            foreach (var puzzle in puzzles)
            {
                if (puzzle != null)
                {
                    puzzle.gameObject.SetActive(true);
                }
            }
            
            Debug.Log($"VRPuzzleRoom: Activated {puzzles.Length} puzzles in room '{roomName}'");
        }
    }
    
    /// <summary>
    /// Base class for relic chamber functionality.
    /// </summary>
    public class VRRelicChamber : MonoBehaviour
    {
        [Header("Relic Chamber Settings")]
        public string roomName = "Relic Chamber";
        public bool requireRelicToEnter = true;
        public GameObject[] requiredRelics;
        
        private void Start()
        {
            // Setup relic requirements
            if (requireRelicToEnter)
            {
                SetupRelicRequirements();
            }
        }
        
        private void SetupRelicRequirements()
        {
            // This would integrate with inventory system
            Debug.Log($"VRRelicChamber: Setup relic requirements for room '{roomName}'");
        }
    }
    
    /// <summary>
    /// Base class for boss room functionality.
    /// </summary>
    public class VRBossRoom : MonoBehaviour
    {
        [Header("Boss Room Settings")]
        public string roomName = "Boss Room";
        public bool enableBossFight = true;
        public GameObject bossPrefab;
        public Transform bossSpawnPoint;
        
        private void Start()
        {
            if (enableBossFight)
            {
                SetupBossFight();
            }
        }
        
        private void SetupBossFight()
        {
            // This would integrate with boss system
            Debug.Log($"VRBossRoom: Setup boss fight for room '{roomName}'");
        }
    }
}
