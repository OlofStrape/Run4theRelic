using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Run4theRelic.Puzzles;

namespace Run4theRelic.Sabotage
{
    /// <summary>
    /// Manages sabotage effects like fog that can be triggered during gameplay.
    /// Provides methods to activate/deactivate fog effects on specific targets.
    /// </summary>
    public class SabotageManager : MonoBehaviour
    {
        public static SabotageManager Instance { get; private set; }
        [Header("Sabotage Settings")]
        [SerializeField] private float defaultFogDuration = 5f;
        [SerializeField] private bool enableFogByDefault = false;
        
        [Header("Fog Effect")]
        [SerializeField] private Material fogMaterial;
        [SerializeField] private Color fogColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);
        [SerializeField] private float fogDensity = 0.5f;
        
        [Header("Fake Clues")]
        [SerializeField] private GameObject fakeCluePrefab;
        [SerializeField] private int defaultFakeClueCount = 6;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        private Dictionary<GameObject, FogEffect> _activeFogEffects = new Dictionary<GameObject, FogEffect>();
        private bool _globalFogEnabled;
        private readonly List<GameObject> _spawnedFakeClues = new List<GameObject>();
        
        /// <summary>
        /// Is global fog currently enabled.
        /// </summary>
        public bool IsGlobalFogEnabled => _globalFogEnabled;
        
        /// <summary>
        /// Number of active fog effects.
        /// </summary>
        public int ActiveFogCount => _activeFogEffects.Count;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            // Init fog material if provided
            if (fogMaterial != null)
            {
                fogMaterial.color = fogColor;
                fogMaterial.SetFloat("_FogDensity", fogDensity);
            }
        }

        private void Start()
        {
            _globalFogEnabled = enableFogByDefault;
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
        /// Apply a temporary global fog effect.
        /// </summary>
        public void ApplyFog(float duration = -1f)
        {
            if (duration < 0f) duration = defaultFogDuration;
            SetGlobalFog(true);
            StartCoroutine(ClearGlobalFogAfter(duration));
            Run4theRelic.Core.GameEvents.TriggerSabotaged("fog", duration);
        }
        
        private IEnumerator ClearGlobalFogAfter(float duration)
        {
            yield return new WaitForSeconds(duration);
            SetGlobalFog(false);
        }
        
        /// <summary>
        /// Reduce time remaining on the currently active puzzle.
        /// </summary>
        public void ApplyTimeDrain(float seconds = 5f)
        {
            if (seconds <= 0f) return;
            var active = PuzzleControllerBase.Active;
            if (active != null && active.IsActive && !active.IsCompleted && !active.IsFailed)
            {
                active.ApplyTimeDrain(seconds);
                if (showDebugInfo) Debug.Log($"ApplyTimeDrain: -{seconds}s to {active.gameObject.name}");
                Run4theRelic.Core.GameEvents.TriggerSabotaged("timedrain", seconds);
                return;
            }
            if (showDebugInfo) Debug.Log("ApplyTimeDrain: No active puzzle found.");
        }
        
        /// <summary>
        /// Spawn fake clue planes near the camera or center point for a duration.
        /// </summary>
        public void ApplyFakeClues(float duration = 5f, int count = -1)
        {
            var active = PuzzleControllerBase.Active;
            if (active != null)
            {
                active.SpawnFakeClues(duration);
                Run4theRelic.Core.GameEvents.TriggerSabotaged("fakeclues", duration);
                if (showDebugInfo) Debug.Log($"ApplyFakeClues: requested on {active.gameObject.name} for {duration}s");
                return;
            }
            // Fallback to legacy visual noise spawn near camera if no active puzzle
            if (count <= 0) count = defaultFakeClueCount;
            Transform origin = Camera.main != null ? Camera.main.transform : transform;
            Vector3 center = origin.position + origin.forward * 2f;
            Quaternion faceCam = Camera.main != null ? Quaternion.LookRotation(center - Camera.main.transform.position) : Quaternion.identity;
            for (int i = 0; i < count; i++)
            {
                GameObject clue = CreateFakeClue(center, faceCam, i);
                _spawnedFakeClues.Add(clue);
            }
            StartCoroutine(ClearFakeCluesAfter(duration));
            if (showDebugInfo) Debug.Log($"ApplyFakeClues (fallback): Spawned {count} fake clues for {duration}s");
        }
        
        private GameObject CreateFakeClue(Vector3 center, Quaternion rotation, int index)
        {
            Vector3 offset = Random.onUnitSphere;
            offset.y = Mathf.Abs(offset.y) * 0.5f;
            offset = offset.normalized * Random.Range(0.5f, 1.2f);
            Vector3 pos = center + offset;
            GameObject go;
            if (fakeCluePrefab != null)
            {
                go = Instantiate(fakeCluePrefab, pos, rotation);
            }
            else
            {
                go = GameObject.CreatePrimitive(PrimitiveType.Quad);
                go.transform.SetPositionAndRotation(pos, rotation);
                go.transform.localScale = Vector3.one * Random.Range(0.2f, 0.5f);
                var renderer = go.GetComponent<Renderer>();
                if (renderer != null)
                {
                    var shader = Shader.Find("Unlit/Transparent");
                    if (shader == null) shader = Shader.Find("Unlit/Color");
                    Material m = new Material(shader);
                    m.color = new Color(Random.value, Random.value, Random.value, 0.9f);
                    renderer.material = m;
                }
            }
            go.name = $"FakeClue_{index}";
            go.layer = gameObject.layer;
            return go;
        }
        
        private IEnumerator ClearFakeCluesAfter(float duration)
        {
            yield return new WaitForSeconds(duration);
            foreach (var go in _spawnedFakeClues)
            {
                if (go != null) Destroy(go);
            }
            _spawnedFakeClues.Clear();
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