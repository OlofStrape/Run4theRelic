using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Run4theRelic.Player
{
    /// <summary>
    /// Wrapper component that adjusts player movement speed via XRI ContinuousMoveProviderBase.
    /// Provides methods to modify move speed for gameplay effects like carrying the Relic.
    /// </summary>
    public class PlayerMovementHook : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float baseMoveSpeed = 2f;
        [SerializeField] private float carrySlowMultiplier = 0.5f;
        [SerializeField] private float minMoveSpeed = 0.5f;
        [SerializeField] private float maxMoveSpeed = 5f;
        
        [Header("References")]
        [SerializeField] private ContinuousMoveProviderBase moveProvider;
        
        private float _currentMoveSpeed;
        private bool _isCarryingRelic;
        private float _customMoveSpeed = -1f; // -1 means use default
        private float? _carryMultiplierOverride; // null -> use serialized carrySlowMultiplier
        
        /// <summary>
        /// Base movement speed of the player.
        /// </summary>
        public float BaseMoveSpeed => baseMoveSpeed;
        
        /// <summary>
        /// Current movement speed (affected by carry state and custom settings).
        /// </summary>
        public float CurrentMoveSpeed => _currentMoveSpeed;
        
        /// <summary>
        /// Is the player currently carrying the Relic.
        /// </summary>
        public bool IsCarryingRelic => _isCarryingRelic;
        
        private void Start()
        {
            // Find move provider if not assigned
            if (moveProvider == null)
            {
                moveProvider = GetComponent<ContinuousMoveProviderBase>();
                
                if (moveProvider == null)
                {
                    moveProvider = GetComponentInChildren<ContinuousMoveProviderBase>();
                }
            }
            
            // Initialize move speed
            _currentMoveSpeed = baseMoveSpeed;
            UpdateMoveSpeed();
            
            // Validate setup
            ValidateSetup();
        }
        
        /// <summary>
        /// Set the carry slow effect on/off.
        /// </summary>
        /// <param name="isCarrying">True if carrying Relic, false if not.</param>
        public void SetCarrySlow(bool isCarrying)
        {
            _isCarryingRelic = isCarrying;
            if (!isCarrying)
            {
                _carryMultiplierOverride = null; // clear override when not carrying
            }
            UpdateMoveSpeed();
            
            Debug.Log($"Carry slow effect: {(isCarrying ? "ON" : "OFF")}");
        }
        
        /// <summary>
        /// Set the carry slow effect with a specific multiplier override.
        /// </summary>
        /// <param name="isCarrying">True if carrying Relic, false if not.</param>
        /// <param name="multiplierOverride">Multiplier to apply while carrying (0..1).</param>
        public void SetCarrySlow(bool isCarrying, float multiplierOverride)
        {
            _isCarryingRelic = isCarrying;
            _carryMultiplierOverride = isCarrying ? Mathf.Clamp01(multiplierOverride) : (float?)null;
            UpdateMoveSpeed();
            
            Debug.Log($"Carry slow effect: {(isCarrying ? "ON" : "OFF")} (override={(isCarrying ? _carryMultiplierOverride.ToString() : "none")})");
        }
        
        /// <summary>
        /// Set a custom move speed (overrides carry effect).
        /// </summary>
        /// <param name="speed">Custom move speed, or -1 to use default.</param>
        public void SetMoveSpeed(float speed)
        {
            _customMoveSpeed = speed;
            UpdateMoveSpeed();
            
            if (speed >= 0f)
            {
                Debug.Log($"Custom move speed set to: {speed}");
            }
            else
            {
                Debug.Log("Custom move speed reset to default");
            }
        }
        
        /// <summary>
        /// Reset move speed to default (removes custom speed and carry effect).
        /// </summary>
        public void ResetMoveSpeed()
        {
            _customMoveSpeed = -1f;
            _isCarryingRelic = false;
            UpdateMoveSpeed();
            
            Debug.Log("Move speed reset to default");
        }
        
        /// <summary>
        /// Get the effective move speed based on current state.
        /// </summary>
        /// <returns>Effective move speed.</returns>
        public float GetEffectiveMoveSpeed()
        {
            float effectiveSpeed = baseMoveSpeed;
            
            // Apply carry slow effect if no custom speed
            if (_customMoveSpeed < 0f && _isCarryingRelic)
            {
                float multiplier = _carryMultiplierOverride.HasValue ? _carryMultiplierOverride.Value : carrySlowMultiplier;
                effectiveSpeed *= multiplier;
            }
            
            // Apply custom speed if set
            if (_customMoveSpeed >= 0f)
            {
                effectiveSpeed = _customMoveSpeed;
            }
            
            // Clamp to valid range
            return Mathf.Clamp(effectiveSpeed, minMoveSpeed, maxMoveSpeed);
        }
        
        /// <summary>
        /// Update the move provider with current speed.
        /// </summary>
        private void UpdateMoveSpeed()
        {
            if (moveProvider == null) return;
            
            _currentMoveSpeed = GetEffectiveMoveSpeed();
            
            // Apply to move provider
            moveProvider.moveSpeed = _currentMoveSpeed;
            
            Debug.Log($"Move speed updated to: {_currentMoveSpeed}");
        }
        
        /// <summary>
        /// Validate that the component setup is correct.
        /// </summary>
        private void ValidateSetup()
        {
            if (moveProvider == null)
            {
                Debug.LogError("PlayerMovementHook: No ContinuousMoveProviderBase found!");
            }
            
            if (baseMoveSpeed <= 0f)
            {
                Debug.LogWarning("PlayerMovementHook: Base move speed should be positive!");
            }
            
            if (carrySlowMultiplier <= 0f || carrySlowMultiplier > 1f)
            {
                Debug.LogWarning("PlayerMovementHook: Carry slow multiplier should be between 0 and 1!");
            }
            
            if (minMoveSpeed < 0f)
            {
                Debug.LogWarning("PlayerMovementHook: Min move speed should be non-negative!");
            }
            
            if (maxMoveSpeed <= minMoveSpeed)
            {
                Debug.LogWarning("PlayerMovementHook: Max move speed should be greater than min move speed!");
            }
        }
        
        // Unity Editor support
        private void OnValidate()
        {
            // Ensure values are valid
            baseMoveSpeed = Mathf.Max(0.1f, baseMoveSpeed);
            carrySlowMultiplier = Mathf.Clamp01(carrySlowMultiplier);
            minMoveSpeed = Mathf.Max(0f, minMoveSpeed);
            maxMoveSpeed = Mathf.Max(minMoveSpeed + 0.1f, maxMoveSpeed);
            
            // Update in editor if playing
            if (Application.isPlaying)
            {
                UpdateMoveSpeed();
            }
        }
        
        // Gizmos for easier setup in editor
        private void OnDrawGizmos()
        {
            // Draw movement indicator
            Gizmos.color = _isCarryingRelic ? Color.yellow : Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
            
            // Draw speed indicator
            if (Application.isPlaying)
            {
                Gizmos.color = Color.blue;
                Vector3 speedVector = transform.forward * (_currentMoveSpeed * 0.5f);
                Gizmos.DrawRay(transform.position, speedVector);
            }
        }
        
        // Unity Editor support for easier setup
        private void OnDrawGizmosSelected()
        {
            // Draw detailed movement information when selected
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 1f);
            
            // Draw connection to move provider
            if (moveProvider != null)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(transform.position, moveProvider.transform.position);
            }
        }
    }
} 