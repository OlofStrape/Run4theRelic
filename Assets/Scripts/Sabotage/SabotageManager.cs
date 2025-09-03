using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Run4theRelic.Core;

namespace Run4theRelic.Sabotage
{
    /// <summary>
    /// Manages sabotage effects like fog that can be triggered during gameplay.
    /// Provides methods to activate/deactivate fog effects on specific targets.
    /// </summary>
    public class SabotageManager : MonoBehaviour
    {
        [Header("Sabotage Settings")]
        [SerializeField] private float defaultFogDuration = 5f;
        [SerializeField] private bool enableFogByDefault = false;
        
        [Header("Fog Effect")]
        [SerializeField] private Material fogMaterial;
        [SerializeField] private Color fogColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);
        [SerializeField] private float fogDensity = 0.5f;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        private Dictionary<GameObject, FogEffect> _activeFogEffects = new Dictionary<GameObject, FogEffect>();
        private bool _globalFogEnabled;
        
        /// <summary>
        /// Is global fog currently enabled.
        /// </summary>
        public bool IsGlobalFogEnabled => _globalFogEnabled;
        
        /// <summary>
        /// Number of active fog effects.
        /// </summary>
        public int ActiveFogCount => _activeFogEffects.Count;
        
        private void Start()
        {
            // Set initial fog state
            _globalFogEnabled = enableFogByDefault;
            
            // Initialize fog material if provided
            if (fogMaterial != null)
            {
                fogMaterial.color = fogColor;
                fogMaterial.SetFloat("_FogDensity", fogDensity);
            }
            
            Debug.Log("SabotageManager initialized");
        }
        
        /// <summary>
        /// Trigger fog effect on a specific target.
        /// </summary>
        /// <param name="target">The GameObject to apply fog to.</param>
        /// <param name="duration">Duration of the fog effect in seconds.</param>
        public void TriggerFog(GameObject target, float duration = -1f)
        {
            if (target == null)
            {
                Debug.LogWarning("Cannot trigger fog: target is null");
                return;
            }
            
            // Use default duration if not specified
            if (duration < 0f)
            {
                duration = defaultFogDuration;
            }
            
            // Check if fog is already active on this target
            if (_activeFogEffects.ContainsKey(target))
            {
                // Extend existing fog effect
                _activeFogEffects[target].ExtendDuration(duration);
                
                if (showDebugInfo)
                {
                    Debug.Log($"Extended fog effect on {target.name} for {duration}s");
                }
            }
            else
            {
                // Create new fog effect
                FogEffect fogEffect = new FogEffect(target, duration, fogMaterial, fogColor, fogDensity);
                _activeFogEffects[target] = fogEffect;
                
                // Start fog effect
                StartCoroutine(ManageFogEffect(target, fogEffect));
                
                if (showDebugInfo)
                {
                    Debug.Log($"Triggered fog effect on {target.name} for {duration}s");
                }
            }
        }

        /// <summary>
        /// Apply fog sabotage and emit sabotage-event.
        /// </summary>
        /// <param name="target">Target GameObject.</param>
        /// <param name="fogDuration">Duration in seconds (uses default if < 0).</param>
        public void ApplyFog(GameObject target, float fogDuration = -1f)
        {
            if (fogDuration < 0f)
            {
                fogDuration = defaultFogDuration;
            }

            // Emit sabotage event before activating fog
            GameEvents.OnSabotaged?.Invoke("fog", fogDuration);

            // TODO: Stöd för fler sabotage-typer i framtiden (ljudstörning, blackout, etc.)

            // Delegate to existing fog trigger
            TriggerFog(target, fogDuration);
        }
        
        /// <summary>
        /// Clear fog effect from a specific target.
        /// </summary>
        /// <param name="target">The GameObject to remove fog from.</param>
        public void ClearFog(GameObject target)
        {
            if (target == null) return;
            
            if (_activeFogEffects.ContainsKey(target))
            {
                // Remove fog effect
                _activeFogEffects[target].Remove();
                _activeFogEffects.Remove(target);
                
                if (showDebugInfo)
                {
                    Debug.Log($"Cleared fog effect from {target.name}");
                }
            }
        }
        
        /// <summary>
        /// Check if fog is currently active on a target.
        /// </summary>
        /// <param name="target">The GameObject to check.</param>
        /// <returns>True if fog is active, false otherwise.</returns>
        public bool IsFogActive(GameObject target)
        {
            if (target == null) return false;
            return _activeFogEffects.ContainsKey(target);
        }
        
        /// <summary>
        /// Toggle global fog effect on/off.
        /// </summary>
        /// <param name="enabled">True to enable global fog, false to disable.</param>
        public void SetGlobalFog(bool enabled)
        {
            _globalFogEnabled = enabled;
            
            if (showDebugInfo)
            {
                Debug.Log($"Global fog {(enabled ? "enabled" : "disabled")}");
            }
        }
        
        /// <summary>
        /// Clear all active fog effects.
        /// </summary>
        public void ClearAllFog()
        {
            foreach (var kvp in _activeFogEffects)
            {
                kvp.Value.Remove();
            }
            
            _activeFogEffects.Clear();
            
            if (showDebugInfo)
            {
                Debug.Log("All fog effects cleared");
            }
        }
        
        /// <summary>
        /// Get remaining duration of fog effect on a target.
        /// </summary>
        /// <param name="target">The GameObject to check.</param>
        /// <returns>Remaining duration in seconds, or 0 if no fog active.</returns>
        public float GetFogRemainingDuration(GameObject target)
        {
            if (target == null || !_activeFogEffects.ContainsKey(target))
            {
                return 0f;
            }
            
            return _activeFogEffects[target].RemainingDuration;
        }
        
        /// <summary>
        /// Manage a fog effect over time.
        /// </summary>
        /// <param name="target">The target GameObject.</param>
        /// <param name="fogEffect">The fog effect to manage.</param>
        private IEnumerator ManageFogEffect(GameObject target, FogEffect fogEffect)
        {
            // Wait for fog duration
            yield return new WaitForSeconds(fogEffect.Duration);
            
            // Remove fog effect if still active
            if (_activeFogEffects.ContainsKey(target))
            {
                fogEffect.Remove();
                _activeFogEffects.Remove(target);
                
                if (showDebugInfo)
                {
                    Debug.Log($"Fog effect expired on {target.name}");
                }
            }
        }
        
        // Unity Editor support
        private void OnValidate()
        {
            // Ensure duration is positive
            defaultFogDuration = Mathf.Max(0.1f, defaultFogDuration);
            
            // Ensure fog density is valid
            fogDensity = Mathf.Clamp01(fogDensity);
        }
        
        // Gizmos for easier setup in editor
        private void OnDrawGizmos()
        {
            // Draw sabotage manager indicator
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
            
            // Draw connections to active fog targets
            if (Application.isPlaying)
            {
                Gizmos.color = Color.gray;
                foreach (var kvp in _activeFogEffects)
                {
                    if (kvp.Key != null)
                    {
                        Gizmos.DrawLine(transform.position, kvp.Key.transform.position);
                    }
                }
            }
        }
        
        // Cleanup on destroy
        private void OnDestroy()
        {
            ClearAllFog();
        }
    }
    
    /// <summary>
    /// Represents a fog effect applied to a specific GameObject.
    /// Handles the visual and timing aspects of the fog effect.
    /// </summary>
    public class FogEffect
    {
        private GameObject _target;
        private float _duration;
        private float _remainingDuration;
        private Material _originalMaterial;
        private Material _fogMaterial;
        private Renderer _renderer;
        private bool _isActive;
        
        /// <summary>
        /// The target GameObject this fog effect is applied to.
        /// </summary>
        public GameObject Target => _target;
        
        /// <summary>
        /// Total duration of the fog effect.
        /// </summary>
        public float Duration => _duration;
        
        /// <summary>
        /// Remaining duration of the fog effect.
        /// </summary>
        public float RemainingDuration => _remainingDuration;
        
        /// <summary>
        /// Is this fog effect currently active.
        /// </summary>
        public bool IsActive => _isActive;
        
        /// <summary>
        /// Create a new fog effect.
        /// </summary>
        /// <param name="target">Target GameObject.</param>
        /// <param name="duration">Duration in seconds.</param>
        /// <param name="fogMaterial">Material to use for fog effect.</param>
        /// <param name="fogColor">Color of the fog.</param>
        /// <param name="fogDensity">Density of the fog.</param>
        public FogEffect(GameObject target, float duration, Material fogMaterial, Color fogColor, float fogDensity)
        {
            _target = target;
            _duration = duration;
            _remainingDuration = duration;
            _fogMaterial = fogMaterial;
            _isActive = true;
            
            // Get renderer component
            _renderer = target.GetComponent<Renderer>();
            if (_renderer != null)
            {
                // Store original material
                _originalMaterial = _renderer.material;
                
                // Apply fog material
                if (_fogMaterial != null)
                {
                    _renderer.material = _fogMaterial;
                    _renderer.material.color = fogColor;
                    _renderer.material.SetFloat("_FogDensity", fogDensity);
                }
            }
        }
        
        /// <summary>
        /// Extend the duration of this fog effect.
        /// </summary>
        /// <param name="additionalDuration">Additional duration in seconds.</param>
        public void ExtendDuration(float additionalDuration)
        {
            if (!_isActive) return;
            
            _duration += additionalDuration;
            _remainingDuration += additionalDuration;
        }
        
        /// <summary>
        /// Remove this fog effect.
        /// </summary>
        public void Remove()
        {
            if (!_isActive) return;
            
            // Restore original material
            if (_renderer != null && _originalMaterial != null)
            {
                _renderer.material = _originalMaterial;
            }
            
            _isActive = false;
        }
        
        /// <summary>
        /// Update the remaining duration.
        /// </summary>
        /// <param name="deltaTime">Time since last update.</param>
        public void UpdateDuration(float deltaTime)
        {
            if (!_isActive) return;
            
            _remainingDuration -= deltaTime;
            if (_remainingDuration <= 0f)
            {
                _remainingDuration = 0f;
            }
        }
    }
} 