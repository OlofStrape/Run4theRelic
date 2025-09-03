using UnityEngine;
using UnityEngine.InputSystem;
using Run4theRelic.Core;
using Run4theRelic.Player;

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
        [SerializeField] private LayerMask playerLayerMask = -1;
        
        [Header("Carry & Anchor")]
        [Tooltip("Primary anchor on the player's right hand.")]
        [SerializeField] private Transform rightHandAnchor;
        [Tooltip("Fallback anchor if right hand is missing; typically the player's root or head.")]
        [SerializeField] private Transform fallbackAnchor;
        [Tooltip("Multiplier applied to player move speed while carrying.")]
        [SerializeField] private float carrySpeedMultiplier = 0.55f;
        
        [Header("Drop Behaviour")]
        [Tooltip("Relative velocity magnitude threshold to auto-drop on collision.")]
        [SerializeField] private float dropVelocityThreshold = 4.0f;
        [Tooltip("Dot product angle bias; lower dot (more opposing) increases drop likelihood.")]
        [Range(-1f, 1f)]
        [SerializeField] private float dropAngleBias = -0.2f;
        [Tooltip("Impulse applied to the Relic when dropped (forward + slight up).")]
        [SerializeField] private float dropImpulse = 5f;
        
        [Header("Audio")] 
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip sfxPickup;
        [SerializeField] private AudioClip sfxDrop;
        [SerializeField] private AudioClip sfxExtract;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        [SerializeField] private bool drawGizmos = true;
        
        private Transform _carrier;
        private bool _isCarried;
        private Vector3 _originalPosition;
        private Rigidbody _rigidbody;
        private Collider _collider;
        private float _gripReleaseTimer;
        private bool _gripWasReleasedTogether;
        private Transform _carryAnchor; // resolved hand anchor on the current carrier
        
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
        
        private void OnEnable()
        {
            GameEvents.OnRelicExtracted += HandleRelicExtracted;
        }
        
        private void OnDisable()
        {
            GameEvents.OnRelicExtracted -= HandleRelicExtracted;
        }
        
        private void Update()
        {
            if (_isCarried && _carrier != null)
            {
                // Follow the carrier via anchor if available
                Transform anchor = ResolveAnchor();
                if (anchor != null)
                {
                    transform.position = anchor.position;
                    transform.rotation = anchor.rotation;
                }
                else
                {
                    transform.position = _carrier.position + Vector3.up * 0.5f;
                }
                
                // Grip-check while carrying
                UpdateGripCheckAndMaybeForceDrop();
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
            _carryAnchor = FindBestAnchorOnCarrier(player);
            
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
            _gripReleaseTimer = 0f;
            _gripWasReleasedTogether = false;
            
            // Trigger pickup event
            GameEvents.TriggerRelicPickedUp(-1); // -1 = singleplayer
            PlayClip(sfxPickup);
            
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
                
                // Apply drop impulse
                if (_carrier != null)
                {
                    Vector3 forward = _carrier.forward;
                    Vector3 dropDirection = (forward + Vector3.up * 0.25f).normalized;
                    _rigidbody.AddForce(dropDirection * dropImpulse, ForceMode.Impulse);
                }
            }
            
            // Re-enable collider
            if (_collider != null)
            {
                _collider.enabled = true;
            }
            
            // Trigger drop event
            GameEvents.TriggerRelicDropped(-1); // -1 = singleplayer
            PlayClip(sfxDrop);
            
            if (showDebugInfo)
            {
                Debug.Log($"Relic dropped by {_carrier?.name ?? "unknown"}");
            }
            
            // Reset state
            _carrier = null;
            _isCarried = false;
            _carryAnchor = null;
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
            _gripReleaseTimer = 0f;
            _gripWasReleasedTogether = false;
            _carryAnchor = null;
            
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
                if (isCarrying)
                {
                    movementHook.SetCarrySlow(true, carrySpeedMultiplier);
                }
                else
                {
                    movementHook.SetCarrySlow(false);
                }
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
            float relativeSpeed = collision.relativeVelocity.magnitude;
            Vector3 carrierForward = _carrier != null ? _carrier.forward : Vector3.forward;
            Vector3 impactDirection = -collision.relativeVelocity.normalized; // direction towards carrier
            float angleDot = Vector3.Dot(carrierForward, impactDirection);
            bool angleOpposingEnough = angleDot <= dropAngleBias; // more opposing or sideways
            
            if (relativeSpeed >= dropVelocityThreshold && angleOpposingEnough)
            {
                if (showDebugInfo)
                {
                    Debug.Log($"Collision drop: speed={relativeSpeed:F2} dot={angleDot:F2} -> dropping");
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
            dropImpulse = Mathf.Max(0.1f, dropImpulse);
            carrySpeedMultiplier = Mathf.Clamp01(carrySpeedMultiplier);
            dropVelocityThreshold = Mathf.Max(0.1f, dropVelocityThreshold);
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
                Transform anchor = ResolveAnchor();
                Vector3 anchorPos = anchor != null ? anchor.position : _carrier.position;
                Gizmos.DrawLine(transform.position, anchorPos);
            }
            
            // Draw original spawn position
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(_originalPosition, Vector3.one * 0.3f);
        }

        private Transform ResolveAnchor()
        {
            if (_carrier == null) return null;
            if (_carryAnchor != null) return _carryAnchor;
            if (rightHandAnchor != null) return rightHandAnchor;
            if (fallbackAnchor != null) return fallbackAnchor;
            return _carrier;
        }

        private Transform FindBestAnchorOnCarrier(Transform player)
        {
            if (player == null) return null;
            string[] candidateNames =
            {
                "RightHand",
                "Right Hand",
                "RightController",
                "Right Controller",
                "RightHandAnchor",
                "RightHand Controller",
                "Right"
            };
            Transform[] allChildren = player.GetComponentsInChildren<Transform>(true);
            foreach (var t in allChildren)
            {
                string n = t.name;
                for (int i = 0; i < candidateNames.Length; i++)
                {
                    if (n.Equals(candidateNames[i], System.StringComparison.OrdinalIgnoreCase))
                    {
                        return t;
                    }
                }
            }
            foreach (var t in allChildren)
            {
                string lower = t.name.ToLowerInvariant();
                if (lower.Contains("right") && (lower.Contains("hand") || lower.Contains("controller") || lower.Contains("anchor")))
                {
                    return t;
                }
            }
            return player;
        }

        private void UpdateGripCheckAndMaybeForceDrop()
        {
            // Expect an InputAction named "Grip" on the carrier or its children
            if (_carrier == null) return;
            
            bool leftGripPressed = false;
            bool rightGripPressed = false;
            
            // Try find PlayerInput for action map reading
            PlayerInput playerInput = _carrier.GetComponentInParent<PlayerInput>();
            if (playerInput != null)
            {
                // Actions may be named "GripLeft"/"GripRight" or a composite "Grip"
                InputAction gripLeft = playerInput.actions.FindAction("GripLeft", false);
                InputAction gripRight = playerInput.actions.FindAction("GripRight", false);
                InputAction grip = playerInput.actions.FindAction("Grip", false);
                if (gripLeft != null) leftGripPressed = gripLeft.ReadValue<float>() > 0.5f;
                if (gripRight != null) rightGripPressed = gripRight.ReadValue<float>() > 0.5f;
                if (grip != null)
                {
                    float v = grip.ReadValue<float>();
                    // If a single composite, treat as both grips
                    leftGripPressed |= v > 0.5f;
                    rightGripPressed |= v > 0.5f;
                }
            }
            
            bool bothReleased = !leftGripPressed && !rightGripPressed;
            if (bothReleased)
            {
                _gripReleaseTimer += Time.deltaTime;
                _gripWasReleasedTogether = true;
                if (_gripReleaseTimer > 0.5f)
                {
                    if (showDebugInfo)
                    {
                        Debug.Log("Grip released for >0.5s while carrying → ForceDrop()");
                    }
                    ForceDrop();
                }
            }
            else
            {
                // Reset if any grip is pressed again
                _gripReleaseTimer = 0f;
                _gripWasReleasedTogether = false;
            }
        }

        private void PlayClip(AudioClip clip)
        {
            if (audioSource == null || clip == null) return;
            audioSource.PlayOneShot(clip);
        }
        
        private void HandleRelicExtracted(int playerId)
        {
            PlayClip(sfxExtract);
        }
    }
} 