using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Run4theRelic.Core.Visual
{
    /// <summary>
    /// Fantastiska relic-visuella effekter för VR med avancerade shader-effekter
    /// Hanterar relic-glow, aura, activation och extraction-effekter
    /// </summary>
    public class VRRelicVisualEnhancement : MonoBehaviour
    {
        [Header("Relic Visual Settings")]
        [SerializeField] private bool enableRelicEffects = true;
        [SerializeField] private RelicVisualTheme currentTheme = RelicVisualTheme.Mystical;
        [SerializeField] private float effectIntensity = 1f;
        [SerializeField] private bool enableVRComfortMode = true;
        
        [Header("Glow Effects")]
        [SerializeField] private bool enableGlowEffect = true;
        [SerializeField] private Color glowColor = Color.cyan;
        [SerializeField] private float glowIntensity = 2f;
        [SerializeField] private float glowPulseSpeed = 1f;
        [SerializeField] private float glowRange = 5f;
        
        [Header("Aura Effects")]
        [SerializeField] private bool enableAuraEffect = true;
        [SerializeField] private Color auraColor = Color.blue;
        [SerializeField] private float auraIntensity = 1.5f;
        [SerializeField] private float auraRadius = 3f;
        [SerializeField] private float auraRotationSpeed = 30f;
        
        [Header("Activation Effects")]
        [SerializeField] private bool enableActivationEffect = true;
        [SerializeField] private Color activationColor = Color.yellow;
        [SerializeField] private float activationIntensity = 3f;
        [SerializeField] private float activationDuration = 2f;
        
        [Header("Extraction Effects")]
        [SerializeField] private bool enableExtractionEffect = true;
        [SerializeField] private Color extractionColor = Color.green;
        [SerializeField] private float extractionIntensity = 4f;
        [SerializeField] private float extractionDuration = 3f;
        
        [Header("Shader Effects")]
        [SerializeField] private Material relicMaterial;
        [SerializeField] private Material glowMaterial;
        [SerializeField] private Material auraMaterial;
        [SerializeField] private bool enableHolographicEffect = true;
        [SerializeField] private float holographicIntensity = 0.5f;
        
        [Header("Particle Effects")]
        [SerializeField] private ParticleSystem relicParticles;
        [SerializeField] private ParticleSystem activationParticles;
        [SerializeField] private ParticleSystem extractionParticles;
        [SerializeField] private bool enableParticleTrails = true;
        
        // Private fields
        private Renderer relicRenderer;
        private List<Renderer> effectRenderers = new List<Renderer>();
        private Dictionary<Renderer, Material> originalMaterials = new Dictionary<Renderer, Material>();
        private Dictionary<Renderer, Coroutine> activeCoroutines = new Dictionary<Renderer, Coroutine>();
        private RelicVisualState currentState = RelicVisualState.Idle;
        private float currentGlowIntensity = 0f;
        private float currentAuraIntensity = 0f;
        private Vector3 originalScale;
        private Color originalColor;
        
        // Events
        public static event System.Action<RelicVisualState> OnRelicStateChanged;
        public static event System.Action<RelicVisualTheme> OnRelicThemeChanged;
        public static event System.Action<float> OnRelicIntensityChanged;
        
        private void Start()
        {
            InitializeRelicVisualSystem();
        }
        
        private void Update()
        {
            if (enableRelicEffects)
            {
                UpdateGlowEffect();
                UpdateAuraEffect();
                UpdateHolographicEffect();
            }
        }
        
        /// <summary>
        /// Initialize relic visual system
        /// </summary>
        private void InitializeRelicVisualSystem()
        {
            // Get relic renderer
            relicRenderer = GetComponent<Renderer>();
            if (relicRenderer == null)
            {
                relicRenderer = GetComponentInChildren<Renderer>();
            }
            
            if (relicRenderer != null)
            {
                // Store original material
                originalMaterials[relicRenderer] = relicRenderer.material;
                originalColor = relicRenderer.material.color;
            }
            
            // Store original scale
            originalScale = transform.localScale;
            
            // Setup effect renderers
            SetupEffectRenderers();
            
            // Apply initial theme
            ApplyRelicTheme(currentTheme);
            
            Debug.Log("[VRRelicVisualEnhancement] Relic visual system initialized");
        }
        
        /// <summary>
        /// Setup effect renderers
        /// </summary>
        private void SetupEffectRenderers()
        {
            // Find all child renderers for effects
            Renderer[] childRenderers = GetComponentsInChildren<Renderer>();
            
            foreach (var renderer in childRenderers)
            {
                if (renderer != relicRenderer)
                {
                    effectRenderers.Add(renderer);
                    originalMaterials[renderer] = renderer.material;
                }
            }
        }
        
        /// <summary>
        /// Update glow effect
        /// </summary>
        private void UpdateGlowEffect()
        {
            if (!enableGlowEffect || relicRenderer == null) return;
            
            // Pulse glow intensity
            currentGlowIntensity = Mathf.Lerp(currentGlowIntensity, glowIntensity * effectIntensity, Time.deltaTime * glowPulseSpeed);
            
            // Apply glow to material
            if (relicMaterial != null)
            {
                relicMaterial.SetFloat("_GlowIntensity", currentGlowIntensity);
                relicMaterial.SetColor("_GlowColor", glowColor);
                relicMaterial.SetFloat("_GlowRange", glowRange);
            }
        }
        
        /// <summary>
        /// Update aura effect
        /// </summary>
        private void UpdateAuraEffect()
        {
            if (!enableAuraEffect) return;
            
            // Pulse aura intensity
            currentAuraIntensity = Mathf.Lerp(currentAuraIntensity, auraIntensity * effectIntensity, Time.deltaTime * 2f);
            
            // Rotate aura
            transform.Rotate(0, auraRotationSpeed * Time.deltaTime, 0);
            
            // Apply aura to effect renderers
            foreach (var renderer in effectRenderers)
            {
                if (renderer != null && auraMaterial != null)
                {
                    auraMaterial.SetFloat("_AuraIntensity", currentAuraIntensity);
                    auraMaterial.SetColor("_AuraColor", auraColor);
                    auraMaterial.SetFloat("_AuraRadius", auraRadius);
                }
            }
        }
        
        /// <summary>
        /// Update holographic effect
        /// </summary>
        private void UpdateHolographicEffect()
        {
            if (!enableHolographicEffect || relicRenderer == null) return;
            
            // Holographic flicker effect
            float flicker = Mathf.Sin(Time.time * 10f) * 0.1f + 0.9f;
            
            if (relicMaterial != null)
            {
                relicMaterial.SetFloat("_HolographicIntensity", holographicIntensity * flicker);
            }
        }
        
        /// <summary>
        /// Change relic visual state
        /// </summary>
        public void ChangeRelicState(RelicVisualState newState)
        {
            if (newState == currentState) return;
            
            RelicVisualState previousState = currentState;
            currentState = newState;
            
            // Handle state transition
            HandleStateTransition(previousState, newState);
            
            OnRelicStateChanged?.Invoke(newState);
            Debug.Log($"[VRRelicVisualEnhancement] Relic state changed to: {newState}");
        }
        
        /// <summary>
        /// Handle state transition
        /// </summary>
        private void HandleStateTransition(RelicVisualState from, RelicVisualState to)
        {
            switch (to)
            {
                case RelicVisualState.Idle:
                    StartCoroutine(TransitionToIdle());
                    break;
                    
                case RelicVisualState.Activating:
                    StartCoroutine(TransitionToActivating());
                    break;
                    
                case RelicVisualState.Active:
                    StartCoroutine(TransitionToActive());
                    break;
                    
                case RelicVisualState.Extracting:
                    StartCoroutine(TransitionToExtracting());
                    break;
                    
                case RelicVisualState.Extracted:
                    StartCoroutine(TransitionToExtracted());
                    break;
            }
        }
        
        /// <summary>
        /// Transition to idle state
        /// </summary>
        private IEnumerator TransitionToIdle()
        {
            // Fade out effects
            float elapsed = 0f;
            float duration = 1f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                // Fade glow
                currentGlowIntensity = Mathf.Lerp(currentGlowIntensity, 0f, t);
                
                // Fade aura
                currentAuraIntensity = Mathf.Lerp(currentAuraIntensity, 0f, t);
                
                yield return null;
            }
            
            // Stop particles
            if (relicParticles != null)
            {
                relicParticles.Stop();
            }
        }
        
        /// <summary>
        /// Transition to activating state
        /// </summary>
        private IEnumerator TransitionToActivating()
        {
            // Start activation particles
            if (activationParticles != null)
            {
                activationParticles.Play();
            }
            
            // Pulse activation effect
            float elapsed = 0f;
            float duration = activationDuration;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                // Pulse activation color
                Color currentColor = Color.Lerp(originalColor, activationColor, t);
                if (relicRenderer != null)
                {
                    relicRenderer.material.color = currentColor;
                }
                
                // Pulse scale
                float scale = Mathf.Lerp(1f, 1.2f, Mathf.Sin(t * Mathf.PI));
                transform.localScale = originalScale * scale;
                
                yield return null;
            }
            
            // Transition to active state
            ChangeRelicState(RelicVisualState.Active);
        }
        
        /// <summary>
        /// Transition to active state
        /// </summary>
        private IEnumerator TransitionToActive()
        {
            // Enable glow and aura
            currentGlowIntensity = glowIntensity * effectIntensity;
            currentAuraIntensity = auraIntensity * effectIntensity;
            
            // Start relic particles
            if (relicParticles != null)
            {
                relicParticles.Play();
            }
            
            // Continuous pulsing effect
            while (currentState == RelicVisualState.Active)
            {
                // Pulse glow
                float pulse = Mathf.Sin(Time.time * glowPulseSpeed) * 0.3f + 0.7f;
                currentGlowIntensity = glowIntensity * effectIntensity * pulse;
                
                yield return null;
            }
        }
        
        /// <summary>
        /// Transition to extracting state
        /// </summary>
        private IEnumerator TransitionToExtracting()
        {
            // Start extraction particles
            if (extractionParticles != null)
            {
                extractionParticles.Play();
            }
            
            // Extraction effect
            float elapsed = 0f;
            float duration = extractionDuration;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                // Fade to extraction color
                Color currentColor = Color.Lerp(activationColor, extractionColor, t);
                if (relicRenderer != null)
                {
                    relicRenderer.material.color = currentColor;
                }
                
                // Scale up for extraction
                float scale = Mathf.Lerp(1f, 1.5f, t);
                transform.localScale = originalScale * scale;
                
                // Increase glow
                currentGlowIntensity = Mathf.Lerp(currentGlowIntensity, extractionIntensity * effectIntensity, t);
                
                yield return null;
            }
            
            // Transition to extracted state
            ChangeRelicState(RelicVisualState.Extracted);
        }
        
        /// <summary>
        /// Transition to extracted state
        /// </summary>
        private IEnumerator TransitionToExtracted()
        {
            // Fade out all effects
            float elapsed = 0f;
            float duration = 2f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                // Fade out glow
                currentGlowIntensity = Mathf.Lerp(currentGlowIntensity, 0f, t);
                
                // Fade out aura
                currentAuraIntensity = Mathf.Lerp(currentAuraIntensity, 0f, t);
                
                // Fade out color
                if (relicRenderer != null)
                {
                    Color currentColor = Color.Lerp(relicRenderer.material.color, originalColor, t);
                    relicRenderer.material.color = currentColor;
                }
                
                // Scale back to original
                float scale = Mathf.Lerp(transform.localScale.x, originalScale.x, t);
                transform.localScale = originalScale * scale;
                
                yield return null;
            }
            
            // Stop all particles
            if (relicParticles != null) relicParticles.Stop();
            if (activationParticles != null) activationParticles.Stop();
            if (extractionParticles != null) extractionParticles.Stop();
        }
        
        /// <summary>
        /// Apply relic theme
        /// </summary>
        public void ApplyRelicTheme(RelicVisualTheme theme)
        {
            currentTheme = theme;
            
            switch (theme)
            {
                case RelicVisualTheme.Mystical:
                    glowColor = Color.cyan;
                    auraColor = Color.blue;
                    activationColor = Color.yellow;
                    extractionColor = Color.green;
                    break;
                    
                case RelicVisualTheme.Technological:
                    glowColor = Color.blue;
                    auraColor = Color.cyan;
                    activationColor = Color.white;
                    extractionColor = Color.green;
                    break;
                    
                case RelicVisualTheme.Ancient:
                    glowColor = Color.yellow;
                    auraColor = Color.orange;
                    activationColor = Color.red;
                    extractionColor = Color.gold;
                    break;
                    
                case RelicVisualTheme.Futuristic:
                    glowColor = Color.magenta;
                    auraColor = Color.cyan;
                    activationColor = Color.white;
                    extractionColor = Color.blue;
                    break;
            }
            
            OnRelicThemeChanged?.Invoke(theme);
            Debug.Log($"[VRRelicVisualEnhancement] Relic theme applied: {theme}");
        }
        
        /// <summary>
        /// Set effect intensity
        /// </summary>
        public void SetEffectIntensity(float intensity)
        {
            effectIntensity = Mathf.Clamp01(intensity);
            OnRelicIntensityChanged?.Invoke(effectIntensity);
        }
        
        /// <summary>
        /// Toggle VR comfort mode
        /// </summary>
        public void ToggleVRComfortMode(bool enable)
        {
            enableVRComfortMode = enable;
            
            if (enable)
            {
                // Reduce effect intensity for VR comfort
                effectIntensity = Mathf.Min(effectIntensity, 0.7f);
                glowIntensity = Mathf.Min(glowIntensity, 1.5f);
                auraIntensity = Mathf.Min(auraIntensity, 1f);
            }
            
            Debug.Log($"[VRRelicVisualEnhancement] VR comfort mode {(enable ? "enabled" : "disabled")}");
        }
        
        /// <summary>
        /// Get current state
        /// </summary>
        public RelicVisualState GetCurrentState()
        {
            return currentState;
        }
        
        /// <summary>
        /// Get current theme
        /// </summary>
        public RelicVisualTheme GetCurrentTheme()
        {
            return currentTheme;
        }
        
        /// <summary>
        /// Get current effect intensity
        /// </summary>
        public float GetCurrentEffectIntensity()
        {
            return effectIntensity;
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
            
            // Restore original materials
            foreach (var kvp in originalMaterials)
            {
                var renderer = kvp.Key;
                var material = kvp.Value;
                
                if (renderer != null && material != null)
                {
                    renderer.material = material;
                }
            }
            
            // Restore original scale
            transform.localScale = originalScale;
        }
    }
    
    /// <summary>
    /// Relic visual states
    /// </summary>
    public enum RelicVisualState
    {
        Idle,
        Activating,
        Active,
        Extracting,
        Extracted
    }
    
    /// <summary>
    /// Relic visual themes
    /// </summary>
    public enum RelicVisualTheme
    {
        Mystical,
        Technological,
        Ancient,
        Futuristic,
        Natural,
        Cosmic
    }
}
