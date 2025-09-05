using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Run4theRelic.Core.Visual
{
    /// <summary>
    /// Avancerad visuell feedback för VR-pussel med real-time effekter
    /// Hanterar puzzle-progress, success/failure, och interaktiva element
    /// </summary>
    public class VRPuzzleVisualFeedback : MonoBehaviour
    {
        [Header("Puzzle Feedback Settings")]
        [SerializeField] private bool enablePuzzleFeedback = true;
        [SerializeField] private PuzzleFeedbackTheme currentTheme = PuzzleFeedbackTheme.Mystical;
        [SerializeField] private float feedbackIntensity = 1f;
        [SerializeField] private bool enableVRComfortMode = true;
        
        [Header("Progress Feedback")]
        [SerializeField] private bool enableProgressFeedback = true;
        [SerializeField] private Color progressColor = Color.blue;
        [SerializeField] private float progressIntensity = 2f;
        [SerializeField] private float progressPulseSpeed = 2f;
        
        [Header("Success Feedback")]
        [SerializeField] private bool enableSuccessFeedback = true;
        [SerializeField] private Color successColor = Color.green;
        [SerializeField] private float successIntensity = 3f;
        [SerializeField] private float successDuration = 2f;
        [SerializeField] private bool enableSuccessParticles = true;
        
        [Header("Failure Feedback")]
        [SerializeField] private bool enableFailureFeedback = true;
        [SerializeField] private Color failureColor = Color.red;
        [SerializeField] private float failureIntensity = 2.5f;
        [SerializeField] private float failureDuration = 1.5f;
        [SerializeField] private bool enableFailureParticles = true;
        
        [Header("Interactive Feedback")]
        [SerializeField] private bool enableInteractiveFeedback = true;
        [SerializeField] private Color interactiveColor = Color.yellow;
        [SerializeField] private float interactiveIntensity = 1.5f;
        [SerializeField] private float interactivePulseSpeed = 3f;
        
        [Header("Visual Effects")]
        [SerializeField] private ParticleSystem successParticles;
        [SerializeField] private ParticleSystem failureParticles;
        [SerializeField] private ParticleSystem progressParticles;
        [SerializeField] private ParticleSystem interactiveParticles;
        [SerializeField] private Light feedbackLight;
        [SerializeField] private Renderer feedbackRenderer;
        
        [Header("Shader Effects")]
        [SerializeField] private Material feedbackMaterial;
        [SerializeField] private bool enableHolographicEffect = true;
        [SerializeField] private float holographicIntensity = 0.5f;
        
        // Private fields
        private PuzzleFeedbackState currentState = PuzzleFeedbackState.Idle;
        private float currentProgress = 0f;
        private Dictionary<Renderer, Material> originalMaterials = new Dictionary<Renderer, Material>();
        private Dictionary<Renderer, Coroutine> activeCoroutines = new Dictionary<Renderer, Coroutine>();
        private Color originalColor;
        private float originalIntensity;
        
        // Events
        public static event System.Action<PuzzleFeedbackState> OnPuzzleStateChanged;
        public static event System.Action<float> OnPuzzleProgressChanged;
        public static event System.Action<PuzzleFeedbackTheme> OnPuzzleThemeChanged;
        public static event System.Action<float> OnPuzzleIntensityChanged;
        
        private void Start()
        {
            InitializePuzzleFeedbackSystem();
        }
        
        private void Update()
        {
            if (enablePuzzleFeedback)
            {
                UpdateFeedbackEffects();
            }
        }
        
        /// <summary>
        /// Initialize puzzle feedback system
        /// </summary>
        private void InitializePuzzleFeedbackSystem()
        {
            // Get feedback renderer
            if (feedbackRenderer == null)
            {
                feedbackRenderer = GetComponent<Renderer>();
            }
            
            if (feedbackRenderer != null)
            {
                // Store original material
                originalMaterials[feedbackRenderer] = feedbackRenderer.material;
                originalColor = feedbackRenderer.material.color;
            }
            
            // Get feedback light
            if (feedbackLight == null)
            {
                feedbackLight = GetComponent<Light>();
            }
            
            if (feedbackLight != null)
            {
                originalIntensity = feedbackLight.intensity;
            }
            
            // Apply initial theme
            ApplyPuzzleTheme(currentTheme);
            
            Debug.Log("[VRPuzzleVisualFeedback] Puzzle feedback system initialized");
        }
        
        /// <summary>
        /// Update feedback effects
        /// </summary>
        private void UpdateFeedbackEffects()
        {
            switch (currentState)
            {
                case PuzzleFeedbackState.Progress:
                    UpdateProgressEffect();
                    break;
                    
                case PuzzleFeedbackState.Interactive:
                    UpdateInteractiveEffect();
                    break;
                    
                case PuzzleFeedbackState.Success:
                    UpdateSuccessEffect();
                    break;
                    
                case PuzzleFeedbackState.Failure:
                    UpdateFailureEffect();
                    break;
            }
        }
        
        /// <summary>
        /// Update progress effect
        /// </summary>
        private void UpdateProgressEffect()
        {
            if (!enableProgressFeedback) return;
            
            // Pulse progress color
            float pulse = Mathf.Sin(Time.time * progressPulseSpeed) * 0.3f + 0.7f;
            Color currentColor = progressColor * pulse * feedbackIntensity;
            
            // Apply to renderer
            if (feedbackRenderer != null)
            {
                feedbackRenderer.material.color = currentColor;
            }
            
            // Apply to light
            if (feedbackLight != null)
            {
                feedbackLight.color = progressColor;
                feedbackLight.intensity = originalIntensity * pulse * feedbackIntensity;
            }
        }
        
        /// <summary>
        /// Update interactive effect
        /// </summary>
        private void UpdateInteractiveEffect()
        {
            if (!enableInteractiveFeedback) return;
            
            // Pulse interactive color
            float pulse = Mathf.Sin(Time.time * interactivePulseSpeed) * 0.5f + 0.5f;
            Color currentColor = interactiveColor * pulse * feedbackIntensity;
            
            // Apply to renderer
            if (feedbackRenderer != null)
            {
                feedbackRenderer.material.color = currentColor;
            }
            
            // Apply to light
            if (feedbackLight != null)
            {
                feedbackLight.color = interactiveColor;
                feedbackLight.intensity = originalIntensity * pulse * feedbackIntensity;
            }
        }
        
        /// <summary>
        /// Update success effect
        /// </summary>
        private void UpdateSuccessEffect()
        {
            if (!enableSuccessFeedback) return;
            
            // Success glow effect
            float glow = Mathf.Sin(Time.time * 5f) * 0.2f + 0.8f;
            Color currentColor = successColor * glow * feedbackIntensity;
            
            // Apply to renderer
            if (feedbackRenderer != null)
            {
                feedbackRenderer.material.color = currentColor;
            }
            
            // Apply to light
            if (feedbackLight != null)
            {
                feedbackLight.color = successColor;
                feedbackLight.intensity = originalIntensity * glow * feedbackIntensity;
            }
        }
        
        /// <summary>
        /// Update failure effect
        /// </summary>
        private void UpdateFailureEffect()
        {
            if (!enableFailureFeedback) return;
            
            // Failure flicker effect
            float flicker = Mathf.Sin(Time.time * 10f) * 0.3f + 0.7f;
            Color currentColor = failureColor * flicker * feedbackIntensity;
            
            // Apply to renderer
            if (feedbackRenderer != null)
            {
                feedbackRenderer.material.color = currentColor;
            }
            
            // Apply to light
            if (feedbackLight != null)
            {
                feedbackLight.color = failureColor;
                feedbackLight.intensity = originalIntensity * flicker * feedbackIntensity;
            }
        }
        
        /// <summary>
        /// Change puzzle feedback state
        /// </summary>
        public void ChangePuzzleState(PuzzleFeedbackState newState)
        {
            if (newState == currentState) return;
            
            PuzzleFeedbackState previousState = currentState;
            currentState = newState;
            
            // Handle state transition
            HandleStateTransition(previousState, newState);
            
            OnPuzzleStateChanged?.Invoke(newState);
            Debug.Log($"[VRPuzzleVisualFeedback] Puzzle state changed to: {newState}");
        }
        
        /// <summary>
        /// Handle state transition
        /// </summary>
        private void HandleStateTransition(PuzzleFeedbackState from, PuzzleFeedbackState to)
        {
            // Stop previous state effects
            StopPreviousStateEffects(from);
            
            // Start new state effects
            StartNewStateEffects(to);
        }
        
        /// <summary>
        /// Stop previous state effects
        /// </summary>
        private void StopPreviousStateEffects(PuzzleFeedbackState state)
        {
            switch (state)
            {
                case PuzzleFeedbackState.Progress:
                    if (progressParticles != null)
                    {
                        progressParticles.Stop();
                    }
                    break;
                    
                case PuzzleFeedbackState.Interactive:
                    if (interactiveParticles != null)
                    {
                        interactiveParticles.Stop();
                    }
                    break;
                    
                case PuzzleFeedbackState.Success:
                    if (successParticles != null)
                    {
                        successParticles.Stop();
                    }
                    break;
                    
                case PuzzleFeedbackState.Failure:
                    if (failureParticles != null)
                    {
                        failureParticles.Stop();
                    }
                    break;
            }
        }
        
        /// <summary>
        /// Start new state effects
        /// </summary>
        private void StartNewStateEffects(PuzzleFeedbackState state)
        {
            switch (state)
            {
                case PuzzleFeedbackState.Progress:
                    if (progressParticles != null)
                    {
                        progressParticles.Play();
                    }
                    break;
                    
                case PuzzleFeedbackState.Interactive:
                    if (interactiveParticles != null)
                    {
                        interactiveParticles.Play();
                    }
                    break;
                    
                case PuzzleFeedbackState.Success:
                    if (successParticles != null && enableSuccessParticles)
                    {
                        successParticles.Play();
                    }
                    StartCoroutine(SuccessEffectSequence());
                    break;
                    
                case PuzzleFeedbackState.Failure:
                    if (failureParticles != null && enableFailureParticles)
                    {
                        failureParticles.Play();
                    }
                    StartCoroutine(FailureEffectSequence());
                    break;
            }
        }
        
        /// <summary>
        /// Success effect sequence
        /// </summary>
        private IEnumerator SuccessEffectSequence()
        {
            float elapsed = 0f;
            float duration = successDuration;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                // Pulse success effect
                float pulse = Mathf.Sin(t * Mathf.PI * 4f) * 0.3f + 0.7f;
                Color currentColor = successColor * pulse * feedbackIntensity;
                
                if (feedbackRenderer != null)
                {
                    feedbackRenderer.material.color = currentColor;
                }
                
                if (feedbackLight != null)
                {
                    feedbackLight.intensity = originalIntensity * pulse * feedbackIntensity;
                }
                
                yield return null;
            }
            
            // Transition to idle
            ChangePuzzleState(PuzzleFeedbackState.Idle);
        }
        
        /// <summary>
        /// Failure effect sequence
        /// </summary>
        private IEnumerator FailureEffectSequence()
        {
            float elapsed = 0f;
            float duration = failureDuration;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                // Flicker failure effect
                float flicker = Mathf.Sin(t * Mathf.PI * 8f) * 0.4f + 0.6f;
                Color currentColor = failureColor * flicker * feedbackIntensity;
                
                if (feedbackRenderer != null)
                {
                    feedbackRenderer.material.color = currentColor;
                }
                
                if (feedbackLight != null)
                {
                    feedbackLight.intensity = originalIntensity * flicker * feedbackIntensity;
                }
                
                yield return null;
            }
            
            // Transition to idle
            ChangePuzzleState(PuzzleFeedbackState.Idle);
        }
        
        /// <summary>
        /// Set puzzle progress
        /// </summary>
        public void SetPuzzleProgress(float progress)
        {
            progress = Mathf.Clamp01(progress);
            
            if (Mathf.Abs(currentProgress - progress) > 0.01f)
            {
                currentProgress = progress;
                
                // Change state based on progress
                if (progress > 0f && progress < 1f)
                {
                    ChangePuzzleState(PuzzleFeedbackState.Progress);
                }
                else if (progress >= 1f)
                {
                    ChangePuzzleState(PuzzleFeedbackState.Success);
                }
                else
                {
                    ChangePuzzleState(PuzzleFeedbackState.Idle);
                }
                
                OnPuzzleProgressChanged?.Invoke(progress);
            }
        }
        
        /// <summary>
        /// Trigger puzzle failure
        /// </summary>
        public void TriggerPuzzleFailure()
        {
            ChangePuzzleState(PuzzleFeedbackState.Failure);
        }
        
        /// <summary>
        /// Set interactive state
        /// </summary>
        public void SetInteractiveState(bool interactive)
        {
            if (interactive && currentState == PuzzleFeedbackState.Idle)
            {
                ChangePuzzleState(PuzzleFeedbackState.Interactive);
            }
            else if (!interactive && currentState == PuzzleFeedbackState.Interactive)
            {
                ChangePuzzleState(PuzzleFeedbackState.Idle);
            }
        }
        
        /// <summary>
        /// Apply puzzle theme
        /// </summary>
        public void ApplyPuzzleTheme(PuzzleFeedbackTheme theme)
        {
            currentTheme = theme;
            
            switch (theme)
            {
                case PuzzleFeedbackTheme.Mystical:
                    progressColor = Color.blue;
                    successColor = Color.green;
                    failureColor = Color.red;
                    interactiveColor = Color.yellow;
                    break;
                    
                case PuzzleFeedbackTheme.Technological:
                    progressColor = Color.cyan;
                    successColor = Color.green;
                    failureColor = Color.red;
                    interactiveColor = Color.white;
                    break;
                    
                case PuzzleFeedbackTheme.Ancient:
                    progressColor = Color.yellow;
                    successColor = Color.gold;
                    failureColor = Color.orange;
                    interactiveColor = Color.amber;
                    break;
                    
                case PuzzleFeedbackTheme.Futuristic:
                    progressColor = Color.magenta;
                    successColor = Color.cyan;
                    failureColor = Color.red;
                    interactiveColor = Color.blue;
                    break;
            }
            
            OnPuzzleThemeChanged?.Invoke(theme);
            Debug.Log($"[VRPuzzleVisualFeedback] Puzzle theme applied: {theme}");
        }
        
        /// <summary>
        /// Set feedback intensity
        /// </summary>
        public void SetFeedbackIntensity(float intensity)
        {
            feedbackIntensity = Mathf.Clamp01(intensity);
            OnPuzzleIntensityChanged?.Invoke(feedbackIntensity);
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
                feedbackIntensity = Mathf.Min(feedbackIntensity, 0.7f);
                progressIntensity = Mathf.Min(progressIntensity, 1.5f);
                successIntensity = Mathf.Min(successIntensity, 2f);
                failureIntensity = Mathf.Min(failureIntensity, 2f);
            }
            
            Debug.Log($"[VRPuzzleVisualFeedback] VR comfort mode {(enable ? "enabled" : "disabled")}");
        }
        
        /// <summary>
        /// Get current state
        /// </summary>
        public PuzzleFeedbackState GetCurrentState()
        {
            return currentState;
        }
        
        /// <summary>
        /// Get current progress
        /// </summary>
        public float GetCurrentProgress()
        {
            return currentProgress;
        }
        
        /// <summary>
        /// Get current theme
        /// </summary>
        public PuzzleFeedbackTheme GetCurrentTheme()
        {
            return currentTheme;
        }
        
        /// <summary>
        /// Get current feedback intensity
        /// </summary>
        public float GetCurrentFeedbackIntensity()
        {
            return feedbackIntensity;
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
            
            // Restore original light intensity
            if (feedbackLight != null)
            {
                feedbackLight.intensity = originalIntensity;
            }
        }
    }
    
    /// <summary>
    /// Puzzle feedback states
    /// </summary>
    public enum PuzzleFeedbackState
    {
        Idle,
        Progress,
        Interactive,
        Success,
        Failure
    }
    
    /// <summary>
    /// Puzzle feedback themes
    /// </summary>
    public enum PuzzleFeedbackTheme
    {
        Mystical,
        Technological,
        Ancient,
        Futuristic,
        Natural,
        Cosmic
    }
}
