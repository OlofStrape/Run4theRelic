using UnityEngine;
using Run4theRelic.Core;
using Run4theRelic.Puzzles;

namespace Run4theRelic.Puzzles
{
    /// <summary>
    /// VR-specific relic puzzle that requires physical interaction with VR hands.
    /// Players must grab and manipulate relics to solve the puzzle.
    /// </summary>
    public class VRRelicPuzzle : PuzzleControllerBase
    {
        [Header("VR Puzzle Settings")]
        [SerializeField] private bool requireBothHands = false;
        [SerializeField] private bool enableHapticFeedback = true;
        [SerializeField] private float hapticIntensity = 0.8f;
        [SerializeField] private float hapticDuration = 0.2f;
        
        [Header("Relic Requirements")]
        [SerializeField] private GameObject[] requiredRelics;
        [SerializeField] private Transform[] relicSlots;
        [SerializeField] private float slotSnapDistance = 0.1f;
        [SerializeField] private bool autoSnapToSlots = true;
        
        [Header("VR Interaction")]
        [SerializeField] private LayerMask interactableLayers = -1;
        [SerializeField] private bool showInteractionHints = true;
        [SerializeField] private Material highlightMaterial;
        
        // VR State
        private bool _isVRMode = false;
        private bool _leftHandGrabbingRelic = false;
        private bool _rightHandGrabbingRelic = false;
        private GameObject _leftGrabbedRelic = null;
        private GameObject _rightGrabbedRelic = null;
        
        // Puzzle State
        private bool[] _relicsInCorrectSlots;
        private bool[] _slotsOccupied;
        private Material[] _originalMaterials;
        
        // VR References
        private VRInteractionSystem _vrInteractionSystem;
        private VRManager _vrManager;
        
        // Events
        public static event System.Action<string, float, bool> OnPuzzleCompleted;
        
        protected override void Start()
        {
            base.Start();
            
            // Initialize VR puzzle
            InitializeVRPuzzle();
            
            // Subscribe to VR events
            SubscribeToVREvents();
        }
        
        /// <summary>
        /// Initialize VR-specific puzzle components.
        /// </summary>
        private void InitializeVRPuzzle()
        {
            // Find VR components
            _vrInteractionSystem = FindObjectOfType<VRInteractionSystem>();
            _vrManager = FindObjectOfType<VRManager>();
            
            if (_vrInteractionSystem == null)
            {
                Debug.LogWarning("VRRelicPuzzle: No VRInteractionSystem found!");
            }
            
            if (_vrManager == null)
            {
                Debug.LogWarning("VRRelicPuzzle: No VRManager found!");
            }
            
            // Initialize arrays
            int relicCount = requiredRelics.Length;
            _relicsInCorrectSlots = new bool[relicCount];
            _slotsOccupied = new bool[relicCount];
            _originalMaterials = new Material[relicCount];
            
            // Store original materials and setup relics
            for (int i = 0; i < relicCount; i++)
            {
                if (requiredRelics[i] != null)
                {
                    var renderer = requiredRelics[i].GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        _originalMaterials[i] = renderer.material;
                    }
                    
                    // Make relics interactable
                    SetupRelicForVR(requiredRelics[i]);
                }
            }
            
            // Check if we're in VR mode
            _isVRMode = _vrManager != null && _vrManager.IsVRMode;
            
            Debug.Log($"VRRelicPuzzle: Initialized with {relicCount} relics. VR Mode: {_isVRMode}");
        }
        
        /// <summary>
        /// Setup a relic GameObject for VR interaction.
        /// </summary>
        /// <param name="relic">Relic GameObject to setup.</param>
        private void SetupRelicForVR(GameObject relic)
        {
            if (relic == null) return;
            
            // Add XR Grab Interactable if not present
            var grabInteractable = relic.GetComponent<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>();
            if (grabInteractable == null)
            {
                grabInteractable = relic.AddComponent<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>();
            }
            
            // Add Rigidbody for physics interaction
            var rigidbody = relic.GetComponent<Rigidbody>();
            if (rigidbody == null)
            {
                rigidbody = relic.AddComponent<Rigidbody>();
                rigidbody.useGravity = true;
                rigidbody.drag = 1f;
                rigidbody.angularDrag = 5f;
            }
            
            // Add collider if not present
            var collider = relic.GetComponent<Collider>();
            if (collider == null)
            {
                // Add a basic box collider
                var boxCollider = relic.AddComponent<BoxCollider>();
                boxCollider.size = Vector3.one;
            }
            
            // Tag as interactable
            relic.layer = LayerMask.NameToLayer("Interactable");
            
            Debug.Log($"VRRelicPuzzle: Setup relic {relic.name} for VR interaction");
        }
        
        /// <summary>
        /// Subscribe to VR interaction events.
        /// </summary>
        private void SubscribeToVREvents()
        {
            if (_vrInteractionSystem == null) return;
            
            // Subscribe to grabbing events
            VRInteractionSystem.OnLeftHandGrabbingChanged += OnLeftHandGrabbingChanged;
            VRInteractionSystem.OnRightHandGrabbingChanged += OnRightHandGrabbingChanged;
            
            // Subscribe to pointing events
            VRInteractionSystem.OnLeftHandPointingChanged += OnLeftHandPointingChanged;
            VRInteractionSystem.OnRightHandPointingChanged += OnRightHandPointingChanged;
        }
        
        private void Update()
        {
            if (!_isVRMode) return;
            
            // Update puzzle state
            UpdatePuzzleState();
            
            // Check for puzzle completion
            CheckPuzzleCompletion();
        }
        
        /// <summary>
        /// Update puzzle state based on VR interactions.
        /// </summary>
        private void UpdatePuzzleState()
        {
            // Update relic positions and check slot placement
            for (int i = 0; i < requiredRelics.Length; i++)
            {
                if (requiredRelics[i] != null && relicSlots[i] != null)
                {
                    float distance = Vector3.Distance(requiredRelics[i].transform.position, relicSlots[i].position);
                    
                    if (distance <= slotSnapDistance)
                    {
                        // Relic is close to slot
                        if (!_slotsOccupied[i])
                        {
                            _slotsOccupied[i] = true;
                            OnRelicEnteredSlot(i);
                        }
                        
                        // Auto-snap if enabled
                        if (autoSnapToSlots && !_relicsInCorrectSlots[i])
                        {
                            SnapRelicToSlot(i);
                        }
                    }
                    else
                    {
                        // Relic moved away from slot
                        if (_slotsOccupied[i])
                        {
                            _slotsOccupied[i] = false;
                            OnRelicLeftSlot(i);
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Snap a relic to its designated slot.
        /// </summary>
        /// <param name="slotIndex">Index of the slot to snap to.</param>
        private void SnapRelicToSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= relicSlots.Length || 
                slotIndex >= requiredRelics.Length || requiredRelics[slotIndex] == null)
                return;
            
            var relic = requiredRelics[slotIndex];
            var slot = relicSlots[slotIndex];
            
            // Snap position and rotation
            relic.transform.position = slot.position;
            relic.transform.rotation = slot.rotation;
            
            // Disable physics temporarily
            var rigidbody = relic.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.isKinematic = true;
            }
            
            // Mark as correctly placed
            _relicsInCorrectSlots[slotIndex] = true;
            
            // Trigger haptic feedback
            if (enableHapticFeedback && _vrManager != null)
            {
                _vrManager.TriggerHapticFeedbackBoth(hapticIntensity, hapticDuration);
            }
            
            // Visual feedback
            HighlightRelic(slotIndex, true);
            
            Debug.Log($"VRRelicPuzzle: Relic {relic.name} snapped to slot {slotIndex}");
        }
        
        /// <summary>
        /// Handle relic entering a slot.
        /// </summary>
        /// <param name="slotIndex">Index of the slot.</param>
        private void OnRelicEnteredSlot(int slotIndex)
        {
            Debug.Log($"VRRelicPuzzle: Relic entered slot {slotIndex}");
            
            // Show visual feedback
            if (showInteractionHints)
            {
                HighlightSlot(slotIndex, true);
            }
        }
        
        /// <summary>
        /// Handle relic leaving a slot.
        /// </summary>
        /// <param name="slotIndex">Index of the slot.</param>
        private void OnRelicLeftSlot(int slotIndex)
        {
            Debug.Log($"VRRelicPuzzle: Relic left slot {slotIndex}");
            
            // Remove visual feedback
            if (showInteractionHints)
            {
                HighlightSlot(slotIndex, false);
            }
            
            // Re-enable physics
            if (slotIndex < requiredRelics.Length && requiredRelics[slotIndex] != null)
            {
                var rigidbody = requiredRelics[slotIndex].GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    rigidbody.isKinematic = false;
                }
            }
            
            // Mark as not correctly placed
            _relicsInCorrectSlots[slotIndex] = false;
        }
        
        /// <summary>
        /// Highlight a relic with visual feedback.
        /// </summary>
        /// <param name="relicIndex">Index of the relic to highlight.</param>
        /// <param name="highlight">Whether to highlight or remove highlight.</param>
        private void HighlightRelic(int relicIndex, bool highlight)
        {
            if (relicIndex < 0 || relicIndex >= requiredRelics.Length || 
                requiredRelics[relicIndex] == null) return;
            
            var renderer = requiredRelics[relicIndex].GetComponent<Renderer>();
            if (renderer == null) return;
            
            if (highlight && highlightMaterial != null)
            {
                renderer.material = highlightMaterial;
            }
            else if (_originalMaterials[relicIndex] != null)
            {
                renderer.material = _originalMaterials[relicIndex];
            }
        }
        
        /// <summary>
        /// Highlight a slot with visual feedback.
        /// </summary>
        /// <param name="slotIndex">Index of the slot to highlight.</param>
        /// <param name="highlight">Whether to highlight or remove highlight.</param>
        private void HighlightSlot(int slotIndex, bool highlight)
        {
            if (slotIndex < 0 || slotIndex >= relicSlots.Length || 
                relicSlots[slotIndex] == null) return;
            
            // This could add visual effects to the slot
            // For now, we'll just log the action
            if (highlight)
            {
                Debug.Log($"VRRelicPuzzle: Highlighting slot {slotIndex}");
            }
        }
        
        /// <summary>
        /// Check if the puzzle is completed.
        /// </summary>
        private void CheckPuzzleCompletion()
        {
            bool allRelicsPlaced = true;
            
            for (int i = 0; i < _relicsInCorrectSlots.Length; i++)
            {
                if (!_relicsInCorrectSlots[i])
                {
                    allRelicsPlaced = false;
                    break;
                }
            }
            
            if (allRelicsPlaced && !IsPuzzleSolved)
            {
                SolvePuzzle();
            }
        }
        
        /// <summary>
        /// Handle left hand grabbing changes.
        /// </summary>
        /// <param name="obj">Object being grabbed.</param>
        /// <param name="grabbing">Whether grabbing or releasing.</param>
        private void OnLeftHandGrabbingChanged(GameObject obj, bool grabbing)
        {
            _leftHandGrabbingRelic = grabbing;
            _leftGrabbedRelic = grabbing ? obj : null;
            
            if (grabbing && obj != null)
            {
                Debug.Log($"VRRelicPuzzle: Left hand grabbed {obj.name}");
                
                // Check if it's a required relic
                int relicIndex = GetRelicIndex(obj);
                if (relicIndex >= 0)
                {
                    OnRelicGrabbed(relicIndex, true);
                }
            }
            else
            {
                Debug.Log("VRRelicPuzzle: Left hand released object");
            }
        }
        
        /// <summary>
        /// Handle right hand grabbing changes.
        /// </summary>
        /// <param name="obj">Object being grabbed.</param>
        /// <param name="grabbing">Whether grabbing or releasing.</param>
        private void OnRightHandGrabbingChanged(GameObject obj, bool grabbing)
        {
            _rightHandGrabbingRelic = grabbing;
            _rightGrabbedRelic = grabbing ? obj : null;
            
            if (grabbing && obj != null)
            {
                Debug.Log($"VRRelicPuzzle: Right hand grabbed {obj.name}");
                
                // Check if it's a required relic
                int relicIndex = GetRelicIndex(obj);
                if (relicIndex >= 0)
                {
                    OnRelicGrabbed(relicIndex, false);
                }
            }
            else
            {
                Debug.Log("VRRelicPuzzle: Right hand released object");
            }
        }
        
        /// <summary>
        /// Handle left hand pointing changes.
        /// </summary>
        /// <param name="obj">Object being pointed at.</param>
        /// <param name="point">Pointing target position.</param>
        private void OnLeftHandPointingChanged(GameObject obj, Vector3 point)
        {
            if (obj != null && showInteractionHints)
            {
                // Show interaction hints for pointed objects
                ShowInteractionHint(obj, true);
            }
        }
        
        /// <summary>
        /// Handle right hand pointing changes.
        /// </summary>
        /// <param name="obj">Object being pointed at.</param>
        /// <param name="point">Pointing target position.</param>
        private void OnRightHandPointingChanged(GameObject obj, Vector3 point)
        {
            if (obj != null && showInteractionHints)
            {
                // Show interaction hints for pointed objects
                ShowInteractionHint(obj, false);
            }
        }
        
        /// <summary>
        /// Get the index of a relic in the required relics array.
        /// </summary>
        /// <param name="relic">Relic GameObject to find.</param>
        /// <returns>Index of the relic, or -1 if not found.</returns>
        private int GetRelicIndex(GameObject relic)
        {
            for (int i = 0; i < requiredRelics.Length; i++)
            {
                if (requiredRelics[i] == relic)
                {
                    return i;
                }
            }
            return -1;
        }
        
        /// <summary>
        /// Handle a relic being grabbed.
        /// </summary>
        /// <param name="relicIndex">Index of the grabbed relic.</param>
        /// <param name="isLeftHand">Whether it was grabbed by the left hand.</param>
        private void OnRelicGrabbed(int relicIndex, bool isLeftHand)
        {
            Debug.Log($"VRRelicPuzzle: Relic {relicIndex} grabbed by {(isLeftHand ? "left" : "right")} hand");
            
            // Trigger haptic feedback
            if (enableHapticFeedback && _vrManager != null)
            {
                var controller = isLeftHand ? _vrManager.GetLeftController() : _vrManager.GetRightController();
                if (controller != null)
                {
                    _vrManager.TriggerHapticFeedback(controller, hapticIntensity * 0.5f, hapticDuration * 0.5f);
                }
            }
            
            // Remove from correct slot if it was placed
            if (_relicsInCorrectSlots[relicIndex])
            {
                _relicsInCorrectSlots[relicIndex] = false;
                HighlightRelic(relicIndex, false);
            }
        }
        
        /// <summary>
        /// Show interaction hint for a pointed object.
        /// </summary>
        /// <param name="obj">Object to show hint for.</param>
        /// <param name="isLeftHand">Whether hint is for left hand.</param>
        private void ShowInteractionHint(GameObject obj, bool isLeftHand)
        {
            // This could show UI hints or visual effects
            // For now, we'll just log the interaction
            if (obj.CompareTag("Relic"))
            {
                Debug.Log($"VRRelicPuzzle: Showing interaction hint for {obj.name} ({(isLeftHand ? "left" : "right")} hand)");
            }
        }
        
        protected override void OnDestroy()
        {
            // Unsubscribe from VR events
            if (_vrInteractionSystem != null)
            {
                VRInteractionSystem.OnLeftHandGrabbingChanged -= OnLeftHandGrabbingChanged;
                VRInteractionSystem.OnRightHandGrabbingChanged -= OnRightHandGrabbingChanged;
                VRInteractionSystem.OnLeftHandPointingChanged -= OnLeftHandPointingChanged;
                VRInteractionSystem.OnRightHandPointingChanged -= OnRightHandPointingChanged;
            }
            
            base.OnDestroy();
        }
        
        /// <summary>
        /// Get current puzzle progress.
        /// </summary>
        /// <returns>Progress as a percentage (0-100).</returns>
        public float GetPuzzleProgress()
        {
            if (_relicsInCorrectSlots == null) return 0f;
            
            int correctCount = 0;
            for (int i = 0; i < _relicsInCorrectSlots.Length; i++)
            {
                if (_relicsInCorrectSlots[i])
                {
                    correctCount++;
                }
            }
            
            return (float)correctCount / _relicsInCorrectSlots.Length * 100f;
        }
        
        /// <summary>
        /// Check if a specific relic is correctly placed.
        /// </summary>
        /// <param name="relicIndex">Index of the relic to check.</param>
        /// <returns>True if the relic is correctly placed.</returns>
        public bool IsRelicCorrectlyPlaced(int relicIndex)
        {
            if (relicIndex < 0 || relicIndex >= _relicsInCorrectSlots.Length)
                return false;
            
            return _relicsInCorrectSlots[relicIndex];
        }
        
        /// <summary>
        /// Reset the puzzle to initial state.
        /// </summary>
        public void ResetPuzzle()
        {
            for (int i = 0; i < requiredRelics.Length; i++)
            {
                if (requiredRelics[i] != null)
                {
                    // Reset relic position and physics
                    var rigidbody = requiredRelics[i].GetComponent<Rigidbody>();
                    if (rigidbody != null)
                    {
                        rigidbody.isKinematic = false;
                        rigidbody.velocity = Vector3.zero;
                        rigidbody.angularVelocity = Vector3.zero;
                    }
                    
                    // Reset materials
                    HighlightRelic(i, false);
                }
                
                _relicsInCorrectSlots[i] = false;
                _slotsOccupied[i] = false;
            }
            
            Debug.Log("VRRelicPuzzle: Puzzle reset to initial state");
        }
    }
}
