using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Run4theRelic.Core.Visual
{
    /// <summary>
    /// Avancerad particle effect manager för VR med optimerad performance
    /// Hanterar atmosfäriska effekter, puzzle-feedback och VR-komfort
    /// </summary>
    public class VRParticleEffectManager : MonoBehaviour
    {
        [Header("Particle System Settings")]
        [SerializeField] private bool enableParticleEffects = true;
        [SerializeField] private int maxActiveParticles = 1000;
        [SerializeField] private float particleLODDistance = 10f;
        [SerializeField] private bool enableVRComfortMode = true;
        
        [Header("Atmospheric Effects")]
        [SerializeField] private ParticleSystem dustParticles;
        [SerializeField] private ParticleSystem fogParticles;
        [SerializeField] private ParticleSystem sparkleParticles;
        [SerializeField] private ParticleSystem emberParticles;
        
        [Header("Puzzle Effects")]
        [SerializeField] private ParticleSystem successParticles;
        [SerializeField] private ParticleSystem failureParticles;
        [SerializeField] private ParticleSystem progressParticles;
        [SerializeField] private ParticleSystem activationParticles;
        
        [Header("Relic Effects")]
        [SerializeField] private ParticleSystem relicGlowParticles;
        [SerializeField] private ParticleSystem relicAuraParticles;
        [SerializeField] private ParticleSystem relicActivationParticles;
        [SerializeField] private ParticleSystem relicExtractionParticles;
        
        [Header("Room Effects")]
        [SerializeField] private ParticleSystem roomAtmosphereParticles;
        [SerializeField] private ParticleSystem roomTransitionParticles;
        [SerializeField] private ParticleSystem roomCompletionParticles;
        
        // Private fields
        private Dictionary<ParticleSystem, ParticleEffectSettings> originalSettings = new Dictionary<ParticleSystem, ParticleEffectSettings>();
        private Dictionary<ParticleSystem, Coroutine> activeCoroutines = new Dictionary<ParticleSystem, Coroutine>();
        private List<ParticleSystem> allParticleSystems = new List<ParticleSystem>();
        private Camera vrCamera;
        private float currentLODLevel = 1f;
        
        // Events
        public static event System.Action<ParticleEffectType, Vector3> OnParticleEffectTriggered;
        public static event System.Action<float> OnLODLevelChanged;
        public static event System.Action<bool> OnVRComfortModeChanged;
        
        private void Start()
        {
            InitializeParticleSystem();
        }
        
        private void Update()
        {
            if (enableParticleEffects)
            {
                UpdateLODSystem();
                UpdateVRComfortMode();
            }
        }
        
        /// <summary>
        /// Initialize particle system
        /// </summary>
        private void InitializeParticleSystem()
        {
            // Find VR camera
            vrCamera = Camera.main;
            if (vrCamera == null)
            {
                vrCamera = FindObjectOfType<Camera>();
            }
            
            // Collect all particle systems
            CollectAllParticleSystems();
            
            // Store original settings
            StoreOriginalSettings();
            
            // Setup VR optimizations
            SetupVROptimizations();
            
            Debug.Log("[VRParticleEffectManager] Particle effect system initialized");
        }
        
        /// <summary>
        /// Collect all particle systems
        /// </summary>
        private void CollectAllParticleSystems()
        {
            allParticleSystems.Clear();
            
            // Add atmospheric effects
            if (dustParticles != null) allParticleSystems.Add(dustParticles);
            if (fogParticles != null) allParticleSystems.Add(fogParticles);
            if (sparkleParticles != null) allParticleSystems.Add(sparkleParticles);
            if (emberParticles != null) allParticleSystems.Add(emberParticles);
            
            // Add puzzle effects
            if (successParticles != null) allParticleSystems.Add(successParticles);
            if (failureParticles != null) allParticleSystems.Add(failureParticles);
            if (progressParticles != null) allParticleSystems.Add(progressParticles);
            if (activationParticles != null) allParticleSystems.Add(activationParticles);
            
            // Add relic effects
            if (relicGlowParticles != null) allParticleSystems.Add(relicGlowParticles);
            if (relicAuraParticles != null) allParticleSystems.Add(relicAuraParticles);
            if (relicActivationParticles != null) allParticleSystems.Add(relicActivationParticles);
            if (relicExtractionParticles != null) allParticleSystems.Add(relicExtractionParticles);
            
            // Add room effects
            if (roomAtmosphereParticles != null) allParticleSystems.Add(roomAtmosphereParticles);
            if (roomTransitionParticles != null) allParticleSystems.Add(roomTransitionParticles);
            if (roomCompletionParticles != null) allParticleSystems.Add(roomCompletionParticles);
        }
        
        /// <summary>
        /// Store original particle settings
        /// </summary>
        private void StoreOriginalSettings()
        {
            foreach (var ps in allParticleSystems)
            {
                if (ps != null)
                {
                    originalSettings[ps] = new ParticleEffectSettings(ps);
                }
            }
        }
        
        /// <summary>
        /// Setup VR optimizations
        /// </summary>
        private void SetupVROptimizations()
        {
            foreach (var ps in allParticleSystems)
            {
                if (ps != null)
                {
                    var main = ps.main;
                    var emission = ps.emission;
                    var shape = ps.shape;
                    
                    // Reduce particle count for VR comfort
                    if (enableVRComfortMode)
                    {
                        main.maxParticles = Mathf.Min(main.maxParticles, maxActiveParticles);
                        emission.rateOverTime = emission.rateOverTime.constant * 0.7f;
                    }
                    
                    // Optimize for VR performance
                    main.simulationSpace = ParticleSystemSimulationSpace.World;
                    main.cullingMode = ParticleSystemCullingMode.Automatic;
                }
            }
        }
        
        /// <summary>
        /// Update LOD system based on distance
        /// </summary>
        private void UpdateLODSystem()
        {
            if (vrCamera == null) return;
            
            float newLODLevel = 1f;
            
            foreach (var ps in allParticleSystems)
            {
                if (ps != null)
                {
                    float distance = Vector3.Distance(vrCamera.transform.position, ps.transform.position);
                    
                    if (distance > particleLODDistance)
                    {
                        // Reduce quality for distant particles
                        newLODLevel = Mathf.Lerp(0.3f, 1f, particleLODDistance / distance);
                        ApplyLODToParticleSystem(ps, newLODLevel);
                    }
                    else
                    {
                        // Full quality for close particles
                        ApplyLODToParticleSystem(ps, 1f);
                    }
                }
            }
            
            if (Mathf.Abs(currentLODLevel - newLODLevel) > 0.1f)
            {
                currentLODLevel = newLODLevel;
                OnLODLevelChanged?.Invoke(currentLODLevel);
            }
        }
        
        /// <summary>
        /// Apply LOD to particle system
        /// </summary>
        private void ApplyLODToParticleSystem(ParticleSystem ps, float lodLevel)
        {
            if (ps == null) return;
            
            var main = ps.main;
            var emission = ps.emission;
            
            // Adjust particle count based on LOD
            if (originalSettings.ContainsKey(ps))
            {
                var original = originalSettings[ps];
                main.maxParticles = Mathf.RoundToInt(original.maxParticles * lodLevel);
                emission.rateOverTime = original.rateOverTime * lodLevel;
            }
        }
        
        /// <summary>
        /// Update VR comfort mode
        /// </summary>
        private void UpdateVRComfortMode()
        {
            // This could be called when VR comfort settings change
        }
        
        /// <summary>
        /// Trigger particle effect
        /// </summary>
        public void TriggerParticleEffect(ParticleEffectType effectType, Vector3 position, float duration = -1f)
        {
            if (!enableParticleEffects) return;
            
            ParticleSystem targetPS = GetParticleSystemForEffect(effectType);
            
            if (targetPS != null)
            {
                // Position the particle system
                targetPS.transform.position = position;
                
                // Play the effect
                targetPS.Play();
                
                // Start duration coroutine if specified
                if (duration > 0f)
                {
                    StartCoroutine(StopParticleEffectAfterDuration(targetPS, duration));
                }
                
                OnParticleEffectTriggered?.Invoke(effectType, position);
            }
        }
        
        /// <summary>
        /// Get particle system for effect type
        /// </summary>
        private ParticleSystem GetParticleSystemForEffect(ParticleEffectType effectType)
        {
            switch (effectType)
            {
                case ParticleEffectType.Dust:
                    return dustParticles;
                case ParticleEffectType.Fog:
                    return fogParticles;
                case ParticleEffectType.Sparkle:
                    return sparkleParticles;
                case ParticleEffectType.Ember:
                    return emberParticles;
                case ParticleEffectType.Success:
                    return successParticles;
                case ParticleEffectType.Failure:
                    return failureParticles;
                case ParticleEffectType.Progress:
                    return progressParticles;
                case ParticleEffectType.Activation:
                    return activationParticles;
                case ParticleEffectType.RelicGlow:
                    return relicGlowParticles;
                case ParticleEffectType.RelicAura:
                    return relicAuraParticles;
                case ParticleEffectType.RelicActivation:
                    return relicActivationParticles;
                case ParticleEffectType.RelicExtraction:
                    return relicExtractionParticles;
                case ParticleEffectType.RoomAtmosphere:
                    return roomAtmosphereParticles;
                case ParticleEffectType.RoomTransition:
                    return roomTransitionParticles;
                case ParticleEffectType.RoomCompletion:
                    return roomCompletionParticles;
                default:
                    return null;
            }
        }
        
        /// <summary>
        /// Stop particle effect after duration
        /// </summary>
        private IEnumerator StopParticleEffectAfterDuration(ParticleSystem ps, float duration)
        {
            yield return new WaitForSeconds(duration);
            
            if (ps != null)
            {
                ps.Stop();
            }
        }
        
        /// <summary>
        /// Start continuous particle effect
        /// </summary>
        public void StartContinuousEffect(ParticleEffectType effectType, Vector3 position)
        {
            ParticleSystem targetPS = GetParticleSystemForEffect(effectType);
            
            if (targetPS != null)
            {
                targetPS.transform.position = position;
                targetPS.Play();
            }
        }
        
        /// <summary>
        /// Stop continuous particle effect
        /// </summary>
        public void StopContinuousEffect(ParticleEffectType effectType)
        {
            ParticleSystem targetPS = GetParticleSystemForEffect(effectType);
            
            if (targetPS != null)
            {
                targetPS.Stop();
            }
        }
        
        /// <summary>
        /// Change particle effect intensity
        /// </summary>
        public void SetParticleEffectIntensity(ParticleEffectType effectType, float intensity)
        {
            ParticleSystem targetPS = GetParticleSystemForEffect(effectType);
            
            if (targetPS != null)
            {
                var main = targetPS.main;
                var emission = targetPS.emission;
                
                if (originalSettings.ContainsKey(targetPS))
                {
                    var original = originalSettings[targetPS];
                    main.maxParticles = Mathf.RoundToInt(original.maxParticles * intensity);
                    emission.rateOverTime = original.rateOverTime * intensity;
                }
            }
        }
        
        /// <summary>
        /// Change particle effect color
        /// </summary>
        public void SetParticleEffectColor(ParticleEffectType effectType, Color color)
        {
            ParticleSystem targetPS = GetParticleSystemForEffect(effectType);
            
            if (targetPS != null)
            {
                var main = targetPS.main;
                main.startColor = color;
            }
        }
        
        /// <summary>
        /// Toggle VR comfort mode
        /// </summary>
        public void ToggleVRComfortMode(bool enable)
        {
            enableVRComfortMode = enable;
            
            if (enable)
            {
                SetupVROptimizations();
            }
            else
            {
                // Restore original settings
                foreach (var kvp in originalSettings)
                {
                    var ps = kvp.Key;
                    var settings = kvp.Value;
                    
                    if (ps != null)
                    {
                        var main = ps.main;
                        var emission = ps.emission;
                        
                        main.maxParticles = settings.maxParticles;
                        emission.rateOverTime = settings.rateOverTime;
                    }
                }
            }
            
            OnVRComfortModeChanged?.Invoke(enable);
            Debug.Log($"[VRParticleEffectManager] VR comfort mode {(enable ? "enabled" : "disabled")}");
        }
        
        /// <summary>
        /// Stop all particle effects
        /// </summary>
        public void StopAllParticleEffects()
        {
            foreach (var ps in allParticleSystems)
            {
                if (ps != null)
                {
                    ps.Stop();
                }
            }
        }
        
        /// <summary>
        /// Get current LOD level
        /// </summary>
        public float GetCurrentLODLevel()
        {
            return currentLODLevel;
        }
        
        /// <summary>
        /// Get active particle count
        /// </summary>
        public int GetActiveParticleCount()
        {
            int totalCount = 0;
            
            foreach (var ps in allParticleSystems)
            {
                if (ps != null)
                {
                    totalCount += ps.particleCount;
                }
            }
            
            return totalCount;
        }
        
        private void OnDestroy()
        {
            // Stop all coroutines
            foreach (var coroutine in activeCoroutines.Values)
            {
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }
            }
            
            // Stop all particle effects
            StopAllParticleEffects();
        }
    }
    
    /// <summary>
    /// Particle effect types
    /// </summary>
    public enum ParticleEffectType
    {
        // Atmospheric effects
        Dust,
        Fog,
        Sparkle,
        Ember,
        
        // Puzzle effects
        Success,
        Failure,
        Progress,
        Activation,
        
        // Relic effects
        RelicGlow,
        RelicAura,
        RelicActivation,
        RelicExtraction,
        
        // Room effects
        RoomAtmosphere,
        RoomTransition,
        RoomCompletion
    }
    
    /// <summary>
    /// Particle effect settings storage
    /// </summary>
    [System.Serializable]
    public class ParticleEffectSettings
    {
        public int maxParticles;
        public float rateOverTime;
        public Color startColor;
        public float startSize;
        public float startLifetime;
        
        public ParticleEffectSettings(ParticleSystem ps)
        {
            var main = ps.main;
            var emission = ps.emission;
            
            maxParticles = main.maxParticles;
            rateOverTime = emission.rateOverTime.constant;
            startColor = main.startColor.color;
            startSize = main.startSize.constant;
            startLifetime = main.startLifetime.constant;
        }
    }
}
