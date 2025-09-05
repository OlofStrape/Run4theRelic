using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Run4theRelic.Core.Visual
{
    /// <summary>
    /// Dynamiskt ljussystem för VR-rum med real-time skuggning och atmosfär
    /// </summary>
    public class VRDynamicLightingSystem : MonoBehaviour
    {
        [Header("Lighting Settings")]
        [SerializeField] private bool enableDynamicLighting = true;
        [SerializeField] private bool enableRealTimeShadows = true;
        [SerializeField] private float lightingTransitionSpeed = 2f;
        [SerializeField] private bool enableAtmosphericLighting = true;
        
        [Header("Room Lighting")]
        [SerializeField] private Light mainDirectionalLight;
        [SerializeField] private List<Light> roomLights = new List<Light>();
        [SerializeField] private List<Light> accentLights = new List<Light>();
        [SerializeField] private List<Light> emergencyLights = new List<Light>();
        
        [Header("Atmospheric Effects")]
        [SerializeField] private Color ambientColor = Color.blue;
        [SerializeField] private float ambientIntensity = 0.3f;
        [SerializeField] private Color skyboxTint = Color.white;
        [SerializeField] private float fogDensity = 0.01f;
        
        [Header("Dynamic Lighting")]
        [SerializeField] private bool enableLightFlicker = true;
        [SerializeField] private float flickerIntensity = 0.1f;
        [SerializeField] private float flickerSpeed = 5f;
        [SerializeField] private bool enableLightPulse = true;
        [SerializeField] private float pulseSpeed = 2f;
        
        // Private fields
        private Dictionary<Light, LightSettings> originalLightSettings = new Dictionary<Light, LightSettings>();
        private Dictionary<Light, Coroutine> activeCoroutines = new Dictionary<Light, Coroutine>();
        private Color originalAmbientColor;
        private float originalAmbientIntensity;
        private Color originalSkyboxTint;
        private float originalFogDensity;
        
        // Events
        public static event System.Action<Color> OnAmbientColorChanged;
        public static event System.Action<float> OnAmbientIntensityChanged;
        public static event System.Action<Light, Color> OnLightColorChanged;
        public static event System.Action<Light, float> OnLightIntensityChanged;
        
        private void Start()
        {
            InitializeLightingSystem();
        }
        
        private void Update()
        {
            if (enableDynamicLighting)
            {
                UpdateAtmosphericLighting();
            }
        }
        
        /// <summary>
        /// Initialize lighting system
        /// </summary>
        private void InitializeLightingSystem()
        {
            // Find main directional light if not assigned
            if (mainDirectionalLight == null)
            {
                mainDirectionalLight = FindObjectOfType<Light>();
            }
            
            // Find all lights in scene
            FindAllLights();
            
            // Store original light settings
            StoreOriginalLightSettings();
            
            // Setup atmospheric lighting
            if (enableAtmosphericLighting)
            {
                SetupAtmosphericLighting();
            }
            
            Debug.Log("[VRDynamicLightingSystem] Dynamic lighting system initialized");
        }
        
        /// <summary>
        /// Find all lights in scene
        /// </summary>
        private void FindAllLights()
        {
            var allLights = FindObjectsOfType<Light>();
            
            foreach (var light in allLights)
            {
                if (light.type == LightType.Directional)
                {
                    if (mainDirectionalLight == null)
                    {
                        mainDirectionalLight = light;
                    }
                }
                else if (light.type == LightType.Point || light.type == LightType.Spot)
                {
                    if (light.name.ToLower().Contains("accent"))
                    {
                        accentLights.Add(light);
                    }
                    else if (light.name.ToLower().Contains("emergency"))
                    {
                        emergencyLights.Add(light);
                    }
                    else
                    {
                        roomLights.Add(light);
                    }
                }
            }
        }
        
        /// <summary>
        /// Store original light settings
        /// </summary>
        private void StoreOriginalLightSettings()
        {
            // Store main directional light
            if (mainDirectionalLight != null)
            {
                originalLightSettings[mainDirectionalLight] = new LightSettings(mainDirectionalLight);
            }
            
            // Store room lights
            foreach (var light in roomLights)
            {
                originalLightSettings[light] = new LightSettings(light);
            }
            
            // Store accent lights
            foreach (var light in accentLights)
            {
                originalLightSettings[light] = new LightSettings(light);
            }
            
            // Store emergency lights
            foreach (var light in emergencyLights)
            {
                originalLightSettings[light] = new LightSettings(light);
            }
            
            // Store ambient settings
            originalAmbientColor = RenderSettings.ambientLight;
            originalAmbientIntensity = RenderSettings.ambientIntensity;
            originalSkyboxTint = RenderSettings.skybox != null ? Color.white : Color.white;
            originalFogDensity = RenderSettings.fogDensity;
        }
        
        /// <summary>
        /// Setup atmospheric lighting
        /// </summary>
        private void SetupAtmosphericLighting()
        {
            // Set ambient color
            RenderSettings.ambientLight = ambientColor;
            RenderSettings.ambientIntensity = ambientIntensity;
            
            // Set skybox tint
            if (RenderSettings.skybox != null)
            {
                // This would require custom skybox material handling
            }
            
            // Set fog
            RenderSettings.fog = true;
            RenderSettings.fogDensity = fogDensity;
            RenderSettings.fogColor = ambientColor * 0.5f;
        }
        
        /// <summary>
        /// Update atmospheric lighting
        /// </summary>
        private void UpdateAtmosphericLighting()
        {
            // Update fog based on ambient color
            if (RenderSettings.fog)
            {
                RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, ambientColor * 0.5f, Time.deltaTime * lightingTransitionSpeed);
            }
        }
        
        /// <summary>
        /// Change room lighting theme
        /// </summary>
        public void ChangeRoomLightingTheme(RoomLightingTheme theme)
        {
            StartCoroutine(TransitionToLightingTheme(theme));
        }
        
        /// <summary>
        /// Transition to lighting theme
        /// </summary>
        private IEnumerator TransitionToLightingTheme(RoomLightingTheme theme)
        {
            var targetSettings = GetLightingThemeSettings(theme);
            
            // Transition ambient lighting
            float elapsed = 0f;
            float duration = 2f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                // Transition ambient color
                RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight, targetSettings.ambientColor, t);
                RenderSettings.ambientIntensity = Mathf.Lerp(RenderSettings.ambientIntensity, targetSettings.ambientIntensity, t);
                
                // Transition fog
                RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, targetSettings.fogDensity, t);
                RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, targetSettings.fogColor, t);
                
                yield return null;
            }
            
            // Apply final settings
            RenderSettings.ambientLight = targetSettings.ambientColor;
            RenderSettings.ambientIntensity = targetSettings.ambientIntensity;
            RenderSettings.fogDensity = targetSettings.fogDensity;
            RenderSettings.fogColor = targetSettings.fogColor;
            
            OnAmbientColorChanged?.Invoke(targetSettings.ambientColor);
            OnAmbientIntensityChanged?.Invoke(targetSettings.ambientIntensity);
        }
        
        /// <summary>
        /// Get lighting theme settings
        /// </summary>
        private LightingThemeSettings GetLightingThemeSettings(RoomLightingTheme theme)
        {
            switch (theme)
            {
                case RoomLightingTheme.Mystical:
                    return new LightingThemeSettings
                    {
                        ambientColor = new Color(0.2f, 0.1f, 0.4f),
                        ambientIntensity = 0.4f,
                        fogDensity = 0.02f,
                        fogColor = new Color(0.1f, 0.05f, 0.2f)
                    };
                    
                case RoomLightingTheme.Dangerous:
                    return new LightingThemeSettings
                    {
                        ambientColor = new Color(0.4f, 0.1f, 0.1f),
                        ambientIntensity = 0.2f,
                        fogDensity = 0.03f,
                        fogColor = new Color(0.2f, 0.05f, 0.05f)
                    };
                    
                case RoomLightingTheme.Serene:
                    return new LightingThemeSettings
                    {
                        ambientColor = new Color(0.1f, 0.3f, 0.4f),
                        ambientIntensity = 0.5f,
                        fogDensity = 0.01f,
                        fogColor = new Color(0.05f, 0.15f, 0.2f)
                    };
                    
                case RoomLightingTheme.Energetic:
                    return new LightingThemeSettings
                    {
                        ambientColor = new Color(0.4f, 0.3f, 0.1f),
                        ambientIntensity = 0.6f,
                        fogDensity = 0.005f,
                        fogColor = new Color(0.2f, 0.15f, 0.05f)
                    };
                    
                default:
                    return new LightingThemeSettings
                    {
                        ambientColor = Color.white,
                        ambientIntensity = 0.3f,
                        fogDensity = 0.01f,
                        fogColor = Color.gray
                    };
            }
        }
        
        /// <summary>
        /// Start light flicker effect
        /// </summary>
        public void StartLightFlicker(Light light, float duration = -1f)
        {
            if (!enableLightFlicker) return;
            
            if (activeCoroutines.ContainsKey(light))
            {
                StopCoroutine(activeCoroutines[light]);
            }
            
            var coroutine = StartCoroutine(FlickerLight(light, duration));
            activeCoroutines[light] = coroutine;
        }
        
        /// <summary>
        /// Flicker light effect
        /// </summary>
        private IEnumerator FlickerLight(Light light, float duration)
        {
            float elapsed = 0f;
            float flickerTimer = 0f;
            
            while (duration < 0f || elapsed < duration)
            {
                elapsed += Time.deltaTime;
                flickerTimer += Time.deltaTime * flickerSpeed;
                
                if (flickerTimer >= 1f)
                {
                    flickerTimer = 0f;
                    
                    // Random flicker
                    float flicker = 1f + Mathf.Sin(Random.Range(0f, Mathf.PI * 2f)) * flickerIntensity;
                    light.intensity = originalLightSettings[light].intensity * flicker;
                }
                
                yield return null;
            }
            
            // Restore original intensity
            light.intensity = originalLightSettings[light].intensity;
            activeCoroutines.Remove(light);
        }
        
        /// <summary>
        /// Start light pulse effect
        /// </summary>
        public void StartLightPulse(Light light, float duration = -1f)
        {
            if (!enableLightPulse) return;
            
            if (activeCoroutines.ContainsKey(light))
            {
                StopCoroutine(activeCoroutines[light]);
            }
            
            var coroutine = StartCoroutine(PulseLight(light, duration));
            activeCoroutines[light] = coroutine;
        }
        
        /// <summary>
        /// Pulse light effect
        /// </summary>
        private IEnumerator PulseLight(Light light, float duration)
        {
            float elapsed = 0f;
            
            while (duration < 0f || elapsed < duration)
            {
                elapsed += Time.deltaTime;
                
                // Smooth pulse
                float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * 0.3f;
                light.intensity = originalLightSettings[light].intensity * pulse;
                
                yield return null;
            }
            
            // Restore original intensity
            light.intensity = originalLightSettings[light].intensity;
            activeCoroutines.Remove(light);
        }
        
        /// <summary>
        /// Stop all light effects
        /// </summary>
        public void StopAllLightEffects()
        {
            foreach (var coroutine in activeCoroutines.Values)
            {
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }
            }
            activeCoroutines.Clear();
            
            // Restore all lights to original settings
            foreach (var kvp in originalLightSettings)
            {
                var light = kvp.Key;
                var settings = kvp.Value;
                
                if (light != null)
                {
                    light.intensity = settings.intensity;
                    light.color = settings.color;
                }
            }
        }
        
        /// <summary>
        /// Get current ambient color
        /// </summary>
        public Color GetCurrentAmbientColor()
        {
            return RenderSettings.ambientLight;
        }
        
        /// <summary>
        /// Get current ambient intensity
        /// </summary>
        public float GetCurrentAmbientIntensity()
        {
            return RenderSettings.ambientIntensity;
        }
        
        private void OnDestroy()
        {
            // Stop all coroutines
            StopAllLightEffects();
            
            // Restore original settings
            RenderSettings.ambientLight = originalAmbientColor;
            RenderSettings.ambientIntensity = originalAmbientIntensity;
            RenderSettings.fogDensity = originalFogDensity;
        }
    }
    
    /// <summary>
    /// Room lighting themes
    /// </summary>
    public enum RoomLightingTheme
    {
        Default,
        Mystical,
        Dangerous,
        Serene,
        Energetic,
        Calm,
        Intense
    }
    
    /// <summary>
    /// Lighting theme settings
    /// </summary>
    [System.Serializable]
    public class LightingThemeSettings
    {
        public Color ambientColor = Color.white;
        public float ambientIntensity = 0.3f;
        public float fogDensity = 0.01f;
        public Color fogColor = Color.gray;
    }
    
    /// <summary>
    /// Light settings storage
    /// </summary>
    [System.Serializable]
    public class LightSettings
    {
        public float intensity;
        public Color color;
        public float range;
        public float spotAngle;
        
        public LightSettings(Light light)
        {
            intensity = light.intensity;
            color = light.color;
            range = light.range;
            spotAngle = light.spotAngle;
        }
    }
}
