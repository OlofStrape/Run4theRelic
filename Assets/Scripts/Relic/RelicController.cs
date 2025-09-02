using UnityEngine;
using Run4theRelic.Core;

namespace Run4theRelic.Relic
{
    /// <summary>
    /// Controls the Relic object that players must pick up and extract to win.
    /// Handles pickup, drop, collision detection and player movement effects.
    /// </summary>
    public class RelicController : MonoBehaviour
    {
        [Header("Relic Settings")]
        [SerializeField] private float pickupRadius = 1f;
        [SerializeField] private float dropForce = 5f;
        [SerializeField] private LayerMask playerLayerMask = -1;
        
        [Header("Movement Effects")]
        [SerializeField] private float carrySlowMultiplier = 0.5f;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        [SerializeField] private bool drawGizmos = true;
        
        private Transform _carrier;
        private bool _isCarried;
        private Vector3 _originalPosition;
        private Rigidbody _rigidbody;
        private Collider _collider;
        
        /// <summary>
        /// Is the Relic currently being carried by a player.
        /// </summary>
        public bool IsCarried => _isCarried;
        
        /// <summary>
        /// The player currently carrying the Relic, or null if dropped.
        /// </summary>
        public Transform Carrier => _carrier;
        
        /// <summary>
        /// The original spawn position of the Relic.
        /// </summary>
        public Vector3 OriginalPosition => _originalPosition;
        
        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            _originalPosition = transform.position;
            
            // Ensure we have required components
            if (_rigidbody == null)
            {
                _rigidbody = gameObject.AddComponent<Rigidbody>();
            }
            
            if (_collider == null)
            {
                _collider = gameObject.AddComponent<SphereCollider>();
            }
        }
        
        private void Update()
        {
            if (_isCarried && _carrier != null)
            {
                // Follow the carrier
                transform.position = _carrier.position + Vector3.up * 0.5f;
            }
        }
        
        /// <summary>
        /// Pick up the Relic by a player.
        /// </summary>
        /// <param name="player">The player picking up the Relic.</param>
        public void PickUp(Transform player)
        {
            if (_isCarried) return;
            
            if (player == null)
            {
                Debug.LogWarning("Cannot pick up Relic: player is null");
                return;
            }
            
            // Check if player is in range
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance > pickupRadius)
            {
                Debug.LogWarning($"Player too far to pick up Relic: {distance:F2}m > {pickupRadius}m");
                return;
            }
            
            // Pick up the Relic
            _carrier = player;
            _isCarried = true;
            
            // Disable physics while carried
            if (_rigidbody != null)
            {
                _rigidbody.isKinematic = true;
            }
            
            // Disable collider while carried
            if (_collider != null)
            {
                _collider.enabled = false;
            }
            
            // Apply movement slow effect
            ApplyCarrySlowEffect(true);
            
            // Trigger pickup event
            GameEvents.TriggerRelicPickedUp(-1); // -1 = singleplayer
            
            if (showDebugInfo)
            {
                Debug.Log($"Relic picked up by {player.name}");
            }
        }
        
        /// <summary>
        /// Drop the Relic from the current carrier.
        /// </summary>
        public void Drop()
        {
            if (!_isCarried) return;
            
            // Remove movement slow effect
            ApplyCarrySlowEffect(false);
            
            // Re-enable physics
            if (_rigidbody != null)
            {
                _rigidbody.isKinematic = false;
                
                // Apply drop force
                if (_carrier != null)
                {
                    Vector3 dropDirection = (_carrier.forward + Vector3.up * 0.5f).normalized;
                    _rigidbody.AddForce(dropDirection * dropForce, ForceMode.Impulse);
                }
            }
            
            // Re-enable collider
            if (_collider != null)
            {
                _collider.enabled = true;
            }
            
            // Trigger drop event
            GameEvents.TriggerRelicDropped(-1); // -1 = singleplayer
            
            if (showDebugInfo)
            {
                Debug.Log($"Relic dropped by {_carrier?.name ?? "unknown"}");
            }
            
            // Reset state
            _carrier = null;
            _isCarried = false;
        }
        
        /// <summary>
        /// Force drop the Relic (useful for puzzle reset).
        /// </summary>
        public void ForceDrop()
        {
            if (!_isCarried) return;
            
            // Remove movement slow effect
            ApplyCarrySlowEffect(false);
            
            // Re-enable physics and collider
            if (_rigidbody != null)
            {
                _rigidbody.isKinematic = false;
            }
            
            if (_collider != null)
            {
                _collider.enabled = true;
            }
            
            // Reset state
            _carrier = null;
            _isCarried = false;
            
            if (showDebugInfo)
            {
                Debug.Log("Relic force dropped");
            }
        }
        
        /// <summary>
        /// Return the Relic to its original position.
        /// </summary>
        public void ReturnToSpawn()
        {
            // Force drop if being carried
            if (_isCarried)
            {
                ForceDrop();
            }
            
            // Return to original position
            transform.position = _originalPosition;
            
            // Reset physics
            if (_rigidbody != null)
            {
                _rigidbody.velocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            }
            
            if (showDebugInfo)
            {
                Debug.Log("Relic returned to spawn position");
            }
        }
        
        /// <summary>
        /// Apply or remove the carry slow effect on the player.
        /// </summary>
        /// <param name="isCarrying">True if player is carrying, false if not.</param>
        private void ApplyCarrySlowEffect(bool isCarrying)
        {
            if (_carrier == null) return;
            
            // Find PlayerMovementHook component
            PlayerMovementHook movementHook = _carrier.GetComponent<PlayerMovementHook>();
            if (movementHook != null)
            {
                movementHook.SetCarrySlow(isCarrying);
            }
            else
            {
                Debug.LogWarning($"Player {_carrier.name} has no PlayerMovementHook component!");
            }
        }
        
        // Collision detection for hard impacts
        private void OnCollisionEnter(Collision collision)
        {
            if (!_isCarried) return;
            
            // Check if collision is hard enough to drop the Relic
            float impactForce = collision.relativeVelocity.magnitude;
            if (impactForce > dropForce)
            {
                if (showDebugInfo)
                {
                    Debug.Log($"Hard collision detected ({impactForce:F2} force), dropping Relic");
                }
                
                Drop();
            }
        }
        
        // Trigger detection for pickup
        private void OnTriggerEnter(Collider other)
        {
            if (_isCarried) return;
            
            // Check if this is a player
            if (((1 << other.gameObject.layer) & playerLayerMask) != 0)
            {
                Transform player = other.transform;
                
                // Try to pick up
                PickUp(player);
            }
        }
        
        // Unity Editor support
        private void OnValidate()
        {
            // Ensure values are positive
            pickupRadius = Mathf.Max(0.1f, pickupRadius);
            dropForce = Mathf.Max(0.1f, dropForce);
            carrySlowMultiplier = Mathf.Clamp01(carrySlowMultiplier);
        }
        
        // Gizmos for easier setup in editor
        private void OnDrawGizmos()
        {
            if (!drawGizmos) return;
            
            // Draw pickup radius
            Gizmos.color = _isCarried ? Color.green : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, pickupRadius);
            
            // Draw connection to carrier
            if (_isCarried && _carrier != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, _carrier.position);
            }
            
            // Draw original spawn position
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(_originalPosition, Vector3.one * 0.3f);
        }
    }
} 