using UnityEngine;
using Run4theRelic.Core;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Run4theRelic.Core
{
    /// <summary>
    /// Manages VR environments, rooms, atmosphere, and environmental effects.
    /// Handles lighting, fog, particles, and VR-specific environmental features.
    /// </summary>
    public class VREnvironmentSystem : MonoBehaviour
    {
        [Header("Environment Settings")]
        [SerializeField] private bool enableDynamicLighting = true;
        [SerializeField] private bool enableAtmosphericEffects = true;
        [SerializeField] private bool enableVRComfortFeatures = true;
        
        [Header("Room Management")]
        [SerializeField] private VRRoom[] availableRooms;
        [SerializeField] private VRRoom currentRoom;
        [SerializeField] private Transform playerSpawnPoint;
        
        [Header("Lighting")]
        [SerializeField] private Light[] mainLights;
        [SerializeField] private Light[] ambientLights;
        [SerializeField] private Material[] skyboxMaterials;
        [SerializeField] private float lightingTransitionDuration = 2f;
        
        [Header("Atmosphere")]
        [SerializeField] private FogSettings fogSettings;
        [SerializeField] private ParticleSystem[] atmosphericParticles;
        [SerializeField] private AudioSource[] ambientAudioSources;
        
        [Header("VR Comfort")]
        [SerializeField] private bool enableBlinkTransitions = true;
        [SerializeField] private float blinkDuration = 0.3f;
        [SerializeField] private bool enableVignetteDuringTransitions = true;
        
        // Environment State
        private bool _isInitialized = false;
        private VRRoom _previousRoom = null;
        private float _lightingTransitionTimer = 0f;
        private bool _isTransitioning = false;
        
        // VR References
        private VRManager _vrManager;
        private VRCameraRig _vrCameraRig;
        private Volume _postProcessVolume;
        
        // Events
        public static event System.Action<VRRoom> OnRoomChanged;
        public static event System.Action<VRRoom, VRRoom> OnRoomTransitionStarted;
        public static event System.Action<VRRoom, VRRoom> OnRoomTransitionCompleted;
        
        [System.Serializable]
        public class FogSettings
        {
            [Header("Fog Configuration")]
            public bool enableFog = true;
            public Color fogColor = Color.gray;
            public float fogDensity = 0.01f;
            public float fogStartDistance = 10f;
            public float fogEndDistance = 100f;
            
            [Header("Fog Animation")]
            public bool animateFog = false;
            public float fogAnimationSpeed = 0.5f;
            public float fogDensityVariation = 0.005f;
        }
        
        [System.Serializable]
        public class VRRoom
        {
            [Header("Room Info")]
            public string roomName;
            public string roomDescription;
            public RoomType roomType;
            
            [Header("Room Settings")]
            public bool isPuzzleRoom = false;
            public bool requireRelicToEnter = false;
            public GameObject[] requiredRelics;
            
            [Header("Environment")]
            public Material skyboxMaterial;
            public Color ambientColor = Color.white;
            public float ambientIntensity = 0.2f;
            public Light[] roomLights;
            public ParticleSystem[] roomParticles;
            public AudioClip[] roomAmbientSounds;
            
            [Header("VR Comfort")]
            public bool enableComfortFeatures = true;
            public float maxTurnAngle = 45f;
            public bool enableTeleportation = true;
            public Transform[] teleportTargets;
            
            [Header("Puzzle Integration")]
            public GameObject[] puzzleObjects;
            public Transform[] relicSlots;
            public bool autoActivatePuzzles = true;
        }
        
        public enum RoomType
        {
            Entrance,
            Corridor,
            PuzzleRoom,
            RelicChamber,
            BossRoom,
            Exit
        }
        
        protected override void Start()
        {
            // Initialize environment system
            InitializeEnvironmentSystem();
            
            // Subscribe to VR events
            SubscribeToVREvents();
        }
        
        /// <summary>
        /// Initialize the VR environment system.
        /// </summary>
        private void InitializeEnvironmentSystem()
        {
            // Find VR components
            _vrManager = FindObjectOfType<VRManager>();
            _vrCameraRig = FindObjectOfType<VRCameraRig>();
            
            if (_vrManager == null)
            {
                Debug.LogWarning("VREnvironmentSystem: No VRManager found!");
            }
            
            if (_vrCameraRig == null)
            {
                Debug.LogWarning("VREnvironmentSystem: No VRCameraRig found!");
            }
            
            // Find post-processing volume
            _postProcessVolume = FindObjectOfType<Volume>();
            
            // Setup initial environment
            if (availableRooms.Length > 0)
            {
                currentRoom = availableRooms[0];
                SetupRoom(currentRoom);
            }
            
            // Initialize lighting
            InitializeLighting();
            
            // Initialize atmosphere
            InitializeAtmosphere();
            
            _isInitialized = true;
            
            Debug.Log("VREnvironmentSystem: Environment system initialized successfully");
        }
        
        /// <summary>
        /// Subscribe to VR system events.
        /// </summary>
        private void SubscribeToVREvents()
        {
            if (_vrManager != null)
            {
                VRManager.OnVRModeChanged += OnVRModeChanged;
            }
        }
        
        private void Update()
        {
            if (!_isInitialized) return;
            
            // Update lighting transitions
            UpdateLightingTransitions();
            
            // Update atmospheric effects
            UpdateAtmosphericEffects();
            
            // Update VR comfort features
            UpdateVRComfortFeatures();
        }
        
        /// <summary>
        /// Update lighting transitions between rooms.
        /// </summary>
        private void UpdateLightingTransitions()
        {
            if (_isTransitioning)
            {
                _lightingTransitionTimer += Time.deltaTime;
                float progress = _lightingTransitionTimer / lightingTransitionDuration;
                
                if (progress >= 1f)
                {
                    CompleteRoomTransition();
                }
                else
                {
                    // Interpolate lighting during transition
                    InterpolateLighting(progress);
                }
            }
        }
        
        /// <summary>
        /// Update atmospheric effects like fog and particles.
        /// </summary>
        private void UpdateAtmosphericEffects()
        {
            if (!enableAtmosphericEffects) return;
            
            // Update fog animation
            if (fogSettings.animateFog)
            {
                UpdateFogAnimation();
            }
            
            // Update atmospheric particles
            UpdateAtmosphericParticles();
        }
        
        /// <summary>
        /// Update VR comfort features.
        /// </summary>
        private void UpdateVRComfortFeatures()
        {
            if (!enableVRComfortFeatures || _vrCameraRig == null) return;
            
            // Update comfort settings based on current room
            if (currentRoom != null)
            {
                UpdateRoomComfortSettings();
            }
        }
        
        /// <summary>
        /// Initialize lighting system.
        /// </summary>
        private void InitializeLighting()
        {
            // Find main lights if not assigned
            if (mainLights == null || mainLights.Length == 0)
            {
                mainLights = FindObjectsOfType<Light>();
            }
            
            // Setup ambient lighting
            if (ambientLights == null || ambientLights.Length == 0)
            {
                var ambientLightObjects = GameObject.FindGameObjectsWithTag("AmbientLight");
                ambientLights = new Light[ambientLightObjects.Length];
                for (int i = 0; i < ambientLightObjects.Length; i++)
                {
                    ambientLights[i] = ambientLightObjects[i].GetComponent<Light>();
                }
            }
            
            Debug.Log($"VREnvironmentSystem: Initialized lighting with {mainLights.Length} main lights and {ambientLights.Length} ambient lights");
        }
        
        /// <summary>
        /// Initialize atmospheric effects.
        /// </summary>
        private void InitializeAtmosphere()
        {
            // Find atmospheric particles if not assigned
            if (atmosphericParticles == null || atmosphericParticles.Length == 0)
            {
                var particleObjects = GameObject.FindGameObjectsWithTag("AtmosphericParticle");
                atmosphericParticles = new ParticleSystem[particleObjects.Length];
                for (int i = 0; i < particleObjects.Length; i++)
                {
                    atmosphericParticles[i] = particleObjects[i].GetComponent<ParticleSystem>();
                }
            }
            
            // Find ambient audio sources if not assigned
            if (ambientAudioSources == null || ambientAudioSources.Length == 0)
            {
                var audioObjects = GameObject.FindGameObjectsWithTag("AmbientAudio");
                ambientAudioSources = new AudioSource[audioObjects.Length];
                for (int i = 0; i < audioObjects.Length; i++)
                {
                    ambientAudioSources[i] = audioObjects[i].GetComponent<AudioSource>();
                }
            }
            
            // Setup fog
            SetupFog();
            
            Debug.Log($"VREnvironmentSystem: Initialized atmosphere with {atmosphericParticles.Length} particle systems and {ambientAudioSources.Length} audio sources");
        }
        
        /// <summary>
        /// Setup fog based on current settings.
        /// </summary>
        private void SetupFog()
        {
            if (!fogSettings.enableFog) return;
            
            RenderSettings.fog = true;
            RenderSettings.fogColor = fogSettings.fogColor;
            RenderSettings.fogDensity = fogSettings.fogDensity;
            RenderSettings.fogStartDistance = fogSettings.fogStartDistance;
            RenderSettings.fogEndDistance = fogSettings.fogEndDistance;
            
            Debug.Log("VREnvironmentSystem: Fog setup complete");
        }
        
        /// <summary>
        /// Update fog animation.
        /// </summary>
        private void UpdateFogAnimation()
        {
            if (!fogSettings.animateFog) return;
            
            float time = Time.time * fogSettings.fogAnimationSpeed;
            float variation = Mathf.Sin(time) * fogSettings.fogDensityVariation;
            float newDensity = fogSettings.fogDensity + variation;
            
            RenderSettings.fogDensity = Mathf.Max(0f, newDensity);
        }
        
        /// <summary>
        /// Update atmospheric particles.
        /// </summary>
        private void UpdateAtmosphericParticles()
        {
            foreach (var particleSystem in atmosphericParticles)
            {
                if (particleSystem != null && particleSystem.isPlaying)
                {
                    // Update particle effects based on room
                    UpdateParticleForRoom(particleSystem);
                }
            }
        }
        
        /// <summary>
        /// Update particle system for current room.
        /// </summary>
        /// <param name="particleSystem">Particle system to update.</param>
        private void UpdateParticleForRoom(ParticleSystem particleSystem)
        {
            if (currentRoom == null) return;
            
            // Adjust particle intensity based on room type
            var emission = particleSystem.emission;
            var main = particleSystem.main;
            
            switch (currentRoom.roomType)
            {
                case RoomType.PuzzleRoom:
                    emission.rateOverTime = 50f;
                    main.startColor = Color.cyan;
                    break;
                    
                case RoomType.RelicChamber:
                    emission.rateOverTime = 100f;
                    main.startColor = Color.gold;
                    break;
                    
                case RoomType.BossRoom:
                    emission.rateOverTime = 200f;
                    main.startColor = Color.red;
                    break;
                    
                default:
                    emission.rateOverTime = 25f;
                    main.startColor = Color.white;
                    break;
            }
        }
        
        /// <summary>
        /// Update room comfort settings.
        /// </summary>
        private void UpdateRoomComfortSettings()
        {
            if (_vrCameraRig == null) return;
            
            // Update camera comfort settings based on room
            if (currentRoom.enableComfortFeatures)
            {
                // Apply room-specific comfort settings
                // This would integrate with VRCameraRig comfort features
            }
        }
        
        /// <summary>
        /// Change to a different room.
        /// </summary>
        /// <param name="roomName">Name of the room to change to.</param>
        public void ChangeRoom(string roomName)
        {
            VRRoom targetRoom = FindRoomByName(roomName);
            if (targetRoom != null)
            {
                ChangeRoom(targetRoom);
            }
            else
            {
                Debug.LogWarning($"VREnvironmentSystem: Room '{roomName}' not found!");
            }
        }
        
        /// <summary>
        /// Change to a specific room.
        /// </summary>
        /// <param name="targetRoom">Room to change to.</param>
        public void ChangeRoom(VRRoom targetRoom)
        {
            if (targetRoom == null || targetRoom == currentRoom) return;
            
            // Check if player can enter the room
            if (!CanEnterRoom(targetRoom)) return;
            
            // Start room transition
            StartRoomTransition(targetRoom);
        }
        
        /// <summary>
        /// Check if player can enter a room.
        /// </summary>
        /// <param name="room">Room to check.</param>
        /// <returns>True if player can enter the room.</returns>
        private bool CanEnterRoom(VRRoom room)
        {
            if (room == null) return false;
            
            // Check if room requires relics
            if (room.requireRelicToEnter)
            {
                return CheckRequiredRelics(room.requiredRelics);
            }
            
            return true;
        }
        
        /// <summary>
        /// Check if required relics are available.
        /// </summary>
        /// <param name="requiredRelics">Array of required relic GameObjects.</param>
        /// <returns>True if all required relics are available.</returns>
        private bool CheckRequiredRelics(GameObject[] requiredRelics)
        {
            if (requiredRelics == null || requiredRelics.Length == 0) return true;
            
            foreach (var relic in requiredRelics)
            {
                if (relic == null) continue;
                
                // Check if relic is in player's inventory or nearby
                // This would integrate with inventory system
                if (!IsRelicAvailable(relic))
                {
                    return false;
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// Check if a relic is available to the player.
        /// </summary>
        /// <param name="relic">Relic to check.</param>
        /// <returns>True if relic is available.</returns>
        private bool IsRelicAvailable(GameObject relic)
        {
            // This would integrate with inventory or proximity system
            // For now, assume all relics are available
            return true;
        }
        
        /// <summary>
        /// Start room transition.
        /// </summary>
        /// <param name="targetRoom">Room to transition to.</param>
        private void StartRoomTransition(VRRoom targetRoom)
        {
            if (_isTransitioning) return;
            
            _previousRoom = currentRoom;
            _isTransitioning = true;
            _lightingTransitionTimer = 0f;
            
            // Trigger blink effect if enabled
            if (enableBlinkTransitions && _vrCameraRig != null)
            {
                _vrCameraRig.StartBlink(blinkDuration);
            }
            
            // Notify transition started
            OnRoomTransitionStarted?.Invoke(_previousRoom, targetRoom);
            
            Debug.Log($"VREnvironmentSystem: Starting transition from {_previousRoom?.roomName ?? "none"} to {targetRoom.roomName}");
        }
        
        /// <summary>
        /// Complete room transition.
        /// </summary>
        private void CompleteRoomTransition()
        {
            if (!_isTransitioning) return;
            
            // Setup new room
            SetupRoom(currentRoom);
            
            // Complete transition
            _isTransitioning = false;
            _lightingTransitionTimer = 0f;
            
            // Notify transition completed
            OnRoomTransitionCompleted?.Invoke(_previousRoom, currentRoom);
            
            Debug.Log($"VREnvironmentSystem: Completed transition to {currentRoom.roomName}");
        }
        
        /// <summary>
        /// Setup a room with its environment settings.
        /// </summary>
        /// <param name="room">Room to setup.</param>
        private void SetupRoom(VRRoom room)
        {
            if (room == null) return;
            
            // Update current room
            currentRoom = room;
            
            // Setup skybox
            if (room.skyboxMaterial != null)
            {
                RenderSettings.skybox = room.skyboxMaterial;
            }
            
            // Setup lighting
            SetupRoomLighting(room);
            
            // Setup particles
            SetupRoomParticles(room);
            
            // Setup audio
            SetupRoomAudio(room);
            
            // Setup puzzles if auto-activate is enabled
            if (room.autoActivatePuzzles)
            {
                SetupRoomPuzzles(room);
            }
            
            // Notify room changed
            OnRoomChanged?.Invoke(room);
            
            Debug.Log($"VREnvironmentSystem: Setup room '{room.roomName}' ({room.roomType})");
        }
        
        /// <summary>
        /// Setup room lighting.
        /// </summary>
        /// <param name="room">Room to setup lighting for.</param>
        private void SetupRoomLighting(VRRoom room)
        {
            // Update ambient lighting
            RenderSettings.ambientLight = room.ambientColor;
            RenderSettings.ambientIntensity = room.ambientIntensity;
            
            // Update room-specific lights
            if (room.roomLights != null)
            {
                foreach (var light in room.roomLights)
                {
                    if (light != null)
                    {
                        light.enabled = true;
                    }
                }
            }
            
            // Disable lights from other rooms
            foreach (var otherRoom in availableRooms)
            {
                if (otherRoom != room && otherRoom.roomLights != null)
                {
                    foreach (var light in otherRoom.roomLights)
                    {
                        if (light != null)
                        {
                            light.enabled = false;
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Setup room particles.
        /// </summary>
        /// <param name="room">Room to setup particles for.</param>
        private void SetupRoomParticles(VRRoom room)
        {
            // Stop all atmospheric particles
            foreach (var particleSystem in atmosphericParticles)
            {
                if (particleSystem != null && particleSystem.isPlaying)
                {
                    particleSystem.Stop();
                }
            }
            
            // Start room-specific particles
            if (room.roomParticles != null)
            {
                foreach (var particleSystem in room.roomParticles)
                {
                    if (particleSystem != null)
                    {
                        particleSystem.Play();
                    }
                }
            }
        }
        
        /// <summary>
        /// Setup room audio.
        /// </summary>
        /// <param name="room">Room to setup audio for.</param>
        private void SetupRoomAudio(VRRoom room)
        {
            // Stop all ambient audio
            foreach (var audioSource in ambientAudioSources)
            {
                if (audioSource != null && audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }
            
            // Start room-specific audio
            if (room.roomAmbientSounds != null && room.roomAmbientSounds.Length > 0)
            {
                // Play random ambient sound
                AudioClip randomSound = room.roomAmbientSounds[Random.Range(0, room.roomAmbientSounds.Length)];
                if (randomSound != null)
                {
                    PlayAmbientAudio(randomSound);
                }
            }
        }
        
        /// <summary>
        /// Setup room puzzles.
        /// </summary>
        /// <param name="room">Room to setup puzzles for.</param>
        private void SetupRoomPuzzles(VRRoom room)
        {
            if (room.puzzleObjects == null) return;
            
            foreach (var puzzleObject in room.puzzleObjects)
            {
                if (puzzleObject != null)
                {
                    // Activate puzzle object
                    puzzleObject.SetActive(true);
                    
                    // This would integrate with puzzle system
                    Debug.Log($"VREnvironmentSystem: Activated puzzle object: {puzzleObject.name}");
                }
            }
        }
        
        /// <summary>
        /// Play ambient audio.
        /// </summary>
        /// <param name="audioClip">Audio clip to play.</param>
        private void PlayAmbientAudio(AudioClip audioClip)
        {
            if (audioClip == null) return;
            
            // Find available audio source
            foreach (var audioSource in ambientAudioSources)
            {
                if (audioSource != null && !audioSource.isPlaying)
                {
                    audioSource.clip = audioClip;
                    audioSource.Play();
                    break;
                }
            }
        }
        
        /// <summary>
        /// Interpolate lighting during room transition.
        /// </summary>
        /// <param name="progress">Transition progress (0-1).</param>
        private void InterpolateLighting(float progress)
        {
            if (_previousRoom == null || currentRoom == null) return;
            
            // Interpolate ambient color
            Color interpolatedColor = Color.Lerp(_previousRoom.ambientColor, currentRoom.ambientColor, progress);
            RenderSettings.ambientLight = interpolatedColor;
            
            // Interpolate ambient intensity
            float interpolatedIntensity = Mathf.Lerp(_previousRoom.ambientIntensity, currentRoom.ambientIntensity, progress);
            RenderSettings.ambientIntensity = interpolatedIntensity;
        }
        
        /// <summary>
        /// Find room by name.
        /// </summary>
        /// <param name="roomName">Name of the room to find.</param>
        /// <returns>VRRoom with matching name, or null if not found.</returns>
        private VRRoom FindRoomByName(string roomName)
        {
            foreach (var room in availableRooms)
            {
                if (room.roomName == roomName)
                {
                    return room;
                }
            }
            return null;
        }
        
        /// <summary>
        /// Handle VR mode changes.
        /// </summary>
        /// <param name="isVRMode">True if VR mode is active.</param>
        private void OnVRModeChanged(bool isVRMode)
        {
            if (isVRMode)
            {
                // Enable VR-specific features
                EnableVRFeatures();
            }
            else
            {
                // Disable VR-specific features
                DisableVRFeatures();
            }
        }
        
        /// <summary>
        /// Enable VR-specific features.
        /// </summary>
        private void EnableVRFeatures()
        {
            // Enable VR comfort features
            enableVRComfortFeatures = true;
            
            // Adjust lighting for VR
            AdjustLightingForVR();
            
            Debug.Log("VREnvironmentSystem: VR features enabled");
        }
        
        /// <summary>
        /// Disable VR-specific features.
        /// </summary>
        private void DisableVRFeatures()
        {
            // Disable VR comfort features
            enableVRComfortFeatures = false;
            
            // Restore desktop lighting
            RestoreDesktopLighting();
            
            Debug.Log("VREnvironmentSystem: VR features disabled");
        }
        
        /// <summary>
        /// Adjust lighting for VR.
        /// </summary>
        private void AdjustLightingForVR()
        {
            // Increase ambient lighting for VR comfort
            RenderSettings.ambientIntensity *= 1.5f;
            
            // Adjust main lights for VR
            foreach (var light in mainLights)
            {
                if (light != null)
                {
                    light.intensity *= 1.2f;
                }
            }
        }
        
        /// <summary>
        /// Restore desktop lighting.
        /// </summary>
        private void RestoreDesktopLighting()
        {
            // Restore original lighting values
            if (currentRoom != null)
            {
                RenderSettings.ambientIntensity = currentRoom.ambientIntensity;
            }
            
            // Restore main light intensities
            // This would store and restore original values
        }
        
        protected override void OnDestroy()
        {
            // Unsubscribe from VR events
            if (_vrManager != null)
            {
                VRManager.OnVRModeChanged -= OnVRModeChanged;
            }
            
            base.OnDestroy();
        }
        
        /// <summary>
        /// Get current room.
        /// </summary>
        /// <returns>Current VRRoom or null.</returns>
        public VRRoom GetCurrentRoom()
        {
            return currentRoom;
        }
        
        /// <summary>
        /// Get all available rooms.
        /// </summary>
        /// <returns>Array of available rooms.</returns>
        public VRRoom[] GetAvailableRooms()
        {
            return availableRooms;
        }
        
        /// <summary>
        /// Check if system is transitioning between rooms.
        /// </summary>
        /// <returns>True if transition is in progress.</returns>
        public bool IsTransitioning()
        {
            return _isTransitioning;
        }
        
        /// <summary>
        /// Get transition progress.
        /// </summary>
        /// <returns>Transition progress (0-1).</returns>
        public float GetTransitionProgress()
        {
            if (!_isTransitioning) return 0f;
            return _lightingTransitionTimer / lightingTransitionDuration;
        }
    }
}
