using UnityEngine;
using Run4theRelic.Core;
using Run4theRelic.Puzzles;
using System.Collections.Generic;

namespace Run4theRelic.Puzzles
{
    /// <summary>
    /// VR puzzle that requires specific hand gestures and movements to solve.
    /// Players must perform hand gestures like pointing, grabbing, or specific hand positions.
    /// </summary>
    public class VRHandGesturePuzzle : PuzzleControllerBase
    {
        [Header("Gesture Requirements")]
        [SerializeField] private HandGesture[] requiredGestures;
        [SerializeField] private float gestureHoldTime = 2f;
        [SerializeField] private bool requireSequentialGestures = false;
        [SerializeField] private bool requireBothHands = false;
        
        [Header("Gesture Detection")]
        [SerializeField] private float gestureTolerance = 0.1f;
        [SerializeField] private bool enableGestureVisualization = true;
        [SerializeField] private Material gestureHighlightMaterial;
        
        [Header("VR Feedback")]
        [SerializeField] private bool enableHapticFeedback = true;
        [SerializeField] private float hapticIntensity = 0.6f;
        [SerializeField] private float hapticDuration = 0.1f;
        
        // Gesture State
        private bool[] _gesturesCompleted;
        private float[] _gestureHoldTimers;
        private int _currentGestureIndex = 0;
        private bool _isGestureInProgress = false;
        
        // VR References
        private VRInputManager _vrInputManager;
        private VRManager _vrManager;
        private VRInteractionSystem _vrInteractionSystem;
        
        // Hand Tracking
        private bool _leftHandInPosition = false;
        private bool _rightHandInPosition = false;
        private Vector3 _leftHandStartPos = Vector3.zero;
        private Vector3 _rightHandStartPos = Vector3.zero;
        
        // Events
        public static event System.Action<int, HandGesture> OnGestureStarted;
        public static event System.Action<int, HandGesture> OnGestureCompleted;
        public static event System.Action<int, HandGesture> OnGestureFailed;
        public static event System.Action<string, float, bool> OnPuzzleCompleted;
        
        [System.Serializable]
        public class HandGesture
        {
            [Header("Gesture Type")]
            public GestureType gestureType;
            public HandSide requiredHand;
            
            [Header("Position Requirements")]
            public bool requireSpecificPosition = false;
            public Transform targetPosition;
            public float positionTolerance = 0.2f;
            
            [Header("Movement Requirements")]
            public bool requireMovement = false;
            public Vector3 movementDirection = Vector3.up;
            public float minMovementDistance = 0.5f;
            
            [Header("Timing")]
            public float requiredHoldTime = 1f;
            public bool requireContinuousHold = true;
        }
        
        public enum GestureType
        {
            Point,
            Grab,
            OpenHand,
            Fist,
            ThumbsUp,
            Wave,
            Custom
        }
        
        public enum HandSide
        {
            Left,
            Right,
            Both,
            Either
        }
        
        protected override void Start()
        {
            base.Start();
            
            // Initialize gesture puzzle
            InitializeGesturePuzzle();
            
            // Subscribe to VR events
            SubscribeToVREvents();
        }
        
        /// <summary>
        /// Initialize the gesture puzzle system.
        /// </summary>
        private void InitializeGesturePuzzle()
        {
            // Find VR components
            _vrInputManager = FindObjectOfType<VRInputManager>();
            _vrManager = FindObjectOfType<VRManager>();
            _vrInteractionSystem = FindObjectOfType<VRInteractionSystem>();
            
            if (_vrInputManager == null)
            {
                Debug.LogWarning("VRHandGesturePuzzle: No VRInputManager found!");
            }
            
            if (_vrManager == null)
            {
                Debug.LogWarning("VRHandGesturePuzzle: No VRManager found!");
            }
            
            if (_vrInteractionSystem == null)
            {
                Debug.LogWarning("VRHandGesturePuzzle: No VRInteractionSystem found!");
            }
            
            // Initialize gesture arrays
            int gestureCount = requiredGestures.Length;
            _gesturesCompleted = new bool[gestureCount];
            _gestureHoldTimers = new float[gestureCount];
            
            // Reset all gestures
            for (int i = 0; i < gestureCount; i++)
            {
                _gesturesCompleted[i] = false;
                _gestureHoldTimers[i] = 0f;
            }
            
            Debug.Log($"VRHandGesturePuzzle: Initialized with {gestureCount} gestures");
        }
        
        /// <summary>
        /// Subscribe to VR input and interaction events.
        /// </summary>
        private void SubscribeToVREvents()
        {
            if (_vrInputManager == null) return;
            
            // Subscribe to hand gesture events
            VRInputManager.OnLeftHandGestureChanged += OnLeftHandGestureChanged;
            VRInputManager.OnRightHandGestureChanged += OnRightHandGestureChanged;
            
            // Subscribe to grip events for grab gestures
            VRInputManager.OnLeftGripChanged += OnLeftGripChanged;
            VRInputManager.OnRightGripChanged += OnRightGripChanged;
            
            // Subscribe to trigger events for point gestures
            VRInputManager.OnLeftTriggerChanged += OnLeftTriggerChanged;
            VRInputManager.OnRightTriggerChanged += OnRightTriggerChanged;
        }
        
        private void Update()
        {
            // Update gesture detection
            UpdateGestureDetection();
            
            // Check for puzzle completion
            CheckPuzzleCompletion();
        }
        
        /// <summary>
        /// Update gesture detection and timing.
        /// </summary>
        private void UpdateGestureDetection()
        {
            if (_isGestureInProgress)
            {
                int currentIndex = _currentGestureIndex;
                if (currentIndex >= 0 && currentIndex < requiredGestures.Length)
                {
                    var currentGesture = requiredGestures[currentIndex];
                    
                    // Check if gesture is still being performed
                    if (IsGestureBeingPerformed(currentGesture))
                    {
                        _gestureHoldTimers[currentIndex] += Time.deltaTime;
                        
                        // Check if gesture has been held long enough
                        if (_gestureHoldTimers[currentIndex] >= currentGesture.requiredHoldTime)
                        {
                            CompleteGesture(currentIndex);
                        }
                    }
                    else
                    {
                        // Gesture stopped being performed
                        if (currentGesture.requireContinuousHold)
                        {
                            // Reset timer if continuous hold is required
                            _gestureHoldTimers[currentIndex] = 0f;
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Check if a specific gesture is currently being performed.
        /// </summary>
        /// <param name="gesture">Gesture to check.</param>
        /// <returns>True if the gesture is being performed.</returns>
        private bool IsGestureBeingPerformed(HandGesture gesture)
        {
            switch (gesture.gestureType)
            {
                case GestureType.Grab:
                    return IsGrabGesturePerformed(gesture);
                    
                case GestureType.Point:
                    return IsPointGesturePerformed(gesture);
                    
                case GestureType.OpenHand:
                    return IsOpenHandGesturePerformed(gesture);
                    
                case GestureType.Fist:
                    return IsFistGesturePerformed(gesture);
                    
                case GestureType.ThumbsUp:
                    return IsThumbsUpGesturePerformed(gesture);
                    
                case GestureType.Wave:
                    return IsWaveGesturePerformed(gesture);
                    
                default:
                    return false;
            }
        }
        
        /// <summary>
        /// Check if grab gesture is being performed.
        /// </summary>
        /// <param name="gesture">Grab gesture to check.</param>
        /// <returns>True if grab gesture is performed.</returns>
        private bool IsGrabGesturePerformed(HandGesture gesture)
        {
            switch (gesture.requiredHand)
            {
                case HandSide.Left:
                    return _vrInputManager.LeftGripPressed;
                    
                case HandSide.Right:
                    return _vrInputManager.RightGripPressed;
                    
                case HandSide.Both:
                    return _vrInputManager.LeftGripPressed && _vrInputManager.RightGripPressed;
                    
                case HandSide.Either:
                    return _vrInputManager.LeftGripPressed || _vrInputManager.RightGripPressed;
                    
                default:
                    return false;
            }
        }
        
        /// <summary>
        /// Check if point gesture is being performed.
        /// </summary>
        /// <param name="gesture">Point gesture to check.</param>
        /// <returns>True if point gesture is performed.</returns>
        private bool IsPointGesturePerformed(HandGesture gesture)
        {
            bool leftPointing = _vrInputManager.LeftTriggerPressed;
            bool rightPointing = _vrInputManager.RightTriggerPressed;
            
            switch (gesture.requiredHand)
            {
                case HandSide.Left:
                    return leftPointing;
                    
                case HandSide.Right:
                    return rightPointing;
                    
                case HandSide.Both:
                    return leftPointing && rightPointing;
                    
                case HandSide.Either:
                    return leftPointing || rightPointing;
                    
                default:
                    return false;
            }
        }
        
        /// <summary>
        /// Check if open hand gesture is being performed.
        /// </summary>
        /// <param name="gesture">Open hand gesture to check.</param>
        /// <returns>True if open hand gesture is performed.</returns>
        private bool IsOpenHandGesturePerformed(HandGesture gesture)
        {
            bool leftOpen = !_vrInputManager.LeftHandClosed;
            bool rightOpen = !_vrInputManager.RightHandClosed;
            
            switch (gesture.requiredHand)
            {
                case HandSide.Left:
                    return leftOpen;
                    
                case HandSide.Right:
                    return rightOpen;
                    
                case HandSide.Both:
                    return leftOpen && rightOpen;
                    
                case HandSide.Either:
                    return leftOpen || rightOpen;
                    
                default:
                    return false;
            }
        }
        
        /// <summary>
        /// Check if fist gesture is being performed.
        /// </summary>
        /// <param name="gesture">Fist gesture to check.</param>
        /// <returns>True if fist gesture is performed.</returns>
        private bool IsFistGesturePerformed(HandGesture gesture)
        {
            bool leftFist = _vrInputManager.LeftHandClosed;
            bool rightFist = _vrInputManager.RightHandClosed;
            
            switch (gesture.requiredHand)
            {
                case HandSide.Left:
                    return leftFist;
                    
                case HandSide.Right:
                    return rightFist;
                    
                case HandSide.Both:
                    return leftFist && rightFist;
                    
                case HandSide.Either:
                    return leftFist || rightFist;
                    
                default:
                    return false;
            }
        }
        
        /// <summary>
        /// Check if thumbs up gesture is being performed.
        /// </summary>
        /// <param name="gesture">Thumbs up gesture to check.</param>
        /// <returns>True if thumbs up gesture is performed.</returns>
        private bool IsThumbsUpGesturePerformed(HandGesture gesture)
        {
            // This would require more sophisticated hand tracking
            // For now, we'll use a combination of grip and trigger
            bool leftThumbsUp = _vrInputManager.LeftGripPressed && !_vrInputManager.LeftTriggerPressed;
            bool rightThumbsUp = _vrInputManager.RightGripPressed && !_vrInputManager.RightTriggerPressed;
            
            switch (gesture.requiredHand)
            {
                case HandSide.Left:
                    return leftThumbsUp;
                    
                case HandSide.Right:
                    return rightThumbsUp;
                    
                case HandSide.Both:
                    return leftThumbsUp && rightThumbsUp;
                    
                case HandSide.Either:
                    return leftThumbsUp || rightThumbsUp;
                    
                default:
                    return false;
            }
        }
        
        /// <summary>
        /// Check if wave gesture is being performed.
        /// </summary>
        /// <param name="gesture">Wave gesture to check.</param>
        /// <returns>True if wave gesture is performed.</returns>
        private bool IsWaveGesturePerformed(HandGesture gesture)
        {
            // This would require tracking hand movement over time
            // For now, we'll use a simple position-based check
            if (gesture.requireMovement)
            {
                // Check if hand has moved in the required direction
                return CheckHandMovement(gesture);
            }
            
            return false;
        }
        
        /// <summary>
        /// Check if hand has moved in the required direction.
        /// </summary>
        /// <param name="gesture">Gesture with movement requirements.</param>
        /// <returns>True if movement requirement is met.</returns>
        private bool CheckHandMovement(HandGesture gesture)
        {
            Vector3 currentLeftPos = _vrInteractionSystem?.LeftPointTarget ?? Vector3.zero;
            Vector3 currentRightPos = _vrInteractionSystem?.RightPointTarget ?? Vector3.zero;
            
            if (gesture.requiredHand == HandSide.Left || gesture.requiredHand == HandSide.Both)
            {
                float leftMovement = Vector3.Distance(currentLeftPos, _leftHandStartPos);
                if (leftMovement >= gesture.minMovementDistance)
                {
                    Vector3 leftDirection = (currentLeftPos - _leftHandStartPos).normalized;
                    float dotProduct = Vector3.Dot(leftDirection, gesture.movementDirection.normalized);
                    if (dotProduct >= 0.7f) // 45-degree tolerance
                    {
                        return true;
                    }
                }
            }
            
            if (gesture.requiredHand == HandSide.Right || gesture.requiredHand == HandSide.Both)
            {
                float rightMovement = Vector3.Distance(currentRightPos, _rightHandStartPos);
                if (rightMovement >= gesture.minMovementDistance)
                {
                    Vector3 rightDirection = (currentRightPos - _rightHandStartPos).normalized;
                    float dotProduct = Vector3.Dot(rightDirection, gesture.movementDirection.normalized);
                    if (dotProduct >= 0.7f) // 45-degree tolerance
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// Start the next gesture in sequence.
        /// </summary>
        private void StartNextGesture()
        {
            if (_currentGestureIndex >= requiredGestures.Length - 1)
            {
                // All gestures completed
                return;
            }
            
            _currentGestureIndex++;
            _isGestureInProgress = true;
            
            var gesture = requiredGestures[_currentGestureIndex];
            OnGestureStarted?.Invoke(_currentGestureIndex, gesture);
            
            Debug.Log($"VRHandGesturePuzzle: Started gesture {_currentGestureIndex}: {gesture.gestureType}");
        }
        
        /// <summary>
        /// Complete a specific gesture.
        /// </summary>
        /// <param name="gestureIndex">Index of the completed gesture.</param>
        private void CompleteGesture(int gestureIndex)
        {
            if (gestureIndex < 0 || gestureIndex >= requiredGestures.Length)
                return;
            
            _gesturesCompleted[gestureIndex] = true;
            _isGestureInProgress = false;
            
            var gesture = requiredGestures[gestureIndex];
            OnGestureCompleted?.Invoke(gestureIndex, gesture);
            
            // Trigger haptic feedback
            if (enableHapticFeedback && _vrManager != null)
            {
                _vrManager.TriggerHapticFeedbackBoth(hapticIntensity, hapticDuration);
            }
            
            Debug.Log($"VRHandGesturePuzzle: Completed gesture {gestureIndex}: {gesture.gestureType}");
            
            // Start next gesture if sequential
            if (requireSequentialGestures)
            {
                StartNextGesture();
            }
        }
        
        /// <summary>
        /// Check if the puzzle is completed.
        /// </summary>
        private void CheckPuzzleCompletion()
        {
            bool allGesturesCompleted = true;
            
            for (int i = 0; i < _gesturesCompleted.Length; i++)
            {
                if (!_gesturesCompleted[i])
                {
                    allGesturesCompleted = false;
                    break;
                }
            }
            
            if (allGesturesCompleted && !IsPuzzleSolved)
            {
                SolvePuzzle();
            }
        }
        
        /// <summary>
        /// Handle left hand gesture changes.
        /// </summary>
        /// <param name="isClosed">Whether left hand is closed.</param>
        private void OnLeftHandGestureChanged(bool isClosed)
        {
            // Update hand state
            _leftHandInPosition = !isClosed;
            
            // Start gesture tracking if needed
            if (!_isGestureInProgress && !requireSequentialGestures)
            {
                StartGestureTracking();
            }
        }
        
        /// <summary>
        /// Handle right hand gesture changes.
        /// </summary>
        /// <param name="isClosed">Whether right hand is closed.</param>
        private void OnRightHandGestureChanged(bool isClosed)
        {
            // Update hand state
            _rightHandInPosition = !isClosed;
            
            // Start gesture tracking if needed
            if (!_isGestureInProgress && !requireSequentialGestures)
            {
                StartGestureTracking();
            }
        }
        
        /// <summary>
        /// Handle left grip changes.
        /// </summary>
        /// <param name="isPressed">Whether left grip is pressed.</param>
        private void OnLeftGripChanged(bool isPressed)
        {
            if (isPressed && !_isGestureInProgress && !requireSequentialGestures)
            {
                StartGestureTracking();
            }
        }
        
        /// <summary>
        /// Handle right grip changes.
        /// </summary>
        /// <param name="isPressed">Whether right grip is pressed.</param>
        private void OnRightGripChanged(bool isPressed)
        {
            if (isPressed && !_isGestureInProgress && !requireSequentialGestures)
            {
                StartGestureTracking();
            }
        }
        
        /// <summary>
        /// Handle left trigger changes.
        /// </summary>
        /// <param name="isPressed">Whether left trigger is pressed.</param>
        private void OnLeftTriggerChanged(bool isPressed)
        {
            if (isPressed && !_isGestureInProgress && !requireSequentialGestures)
            {
                StartGestureTracking();
            }
        }
        
        /// <summary>
        /// Handle right trigger changes.
        /// </summary>
        /// <param name="isPressed">Whether right trigger is pressed.</param>
        private void OnRightTriggerChanged(bool isPressed)
        {
            if (isPressed && !_isGestureInProgress && !requireSequentialGestures)
            {
                StartGestureTracking();
            }
        }
        
        /// <summary>
        /// Start tracking gestures.
        /// </summary>
        private void StartGestureTracking()
        {
            if (_isGestureInProgress) return;
            
            _currentGestureIndex = 0;
            _isGestureInProgress = true;
            
            // Store starting positions for movement tracking
            _leftHandStartPos = _vrInteractionSystem?.LeftPointTarget ?? Vector3.zero;
            _rightHandStartPos = _vrInteractionSystem?.RightPointTarget ?? Vector3.zero;
            
            var gesture = requiredGestures[_currentGestureIndex];
            OnGestureStarted?.Invoke(_currentGestureIndex, gesture);
            
            Debug.Log($"VRHandGesturePuzzle: Started tracking gestures. First: {gesture.gestureType}");
        }
        
        protected override void OnDestroy()
        {
            // Unsubscribe from VR events
            if (_vrInputManager != null)
            {
                VRInputManager.OnLeftHandGestureChanged -= OnLeftHandGestureChanged;
                VRInputManager.OnRightHandGestureChanged -= OnRightHandGestureChanged;
                VRInputManager.OnLeftGripChanged -= OnLeftGripChanged;
                VRInputManager.OnRightGripChanged -= OnRightGripChanged;
                VRInputManager.OnLeftTriggerChanged -= OnLeftTriggerChanged;
                VRInputManager.OnRightTriggerChanged -= OnRightTriggerChanged;
            }
            
            base.OnDestroy();
        }
        
        /// <summary>
        /// Get current puzzle progress.
        /// </summary>
        /// <returns>Progress as a percentage (0-100).</returns>
        public float GetPuzzleProgress()
        {
            if (_gesturesCompleted == null) return 0f;
            
            int completedCount = 0;
            for (int i = 0; i < _gesturesCompleted.Length; i++)
            {
                if (_gesturesCompleted[i])
                {
                    completedCount++;
                }
            }
            
            return (float)completedCount / _gesturesCompleted.Length * 100f;
        }
        
        /// <summary>
        /// Check if a specific gesture is completed.
        /// </summary>
        /// <param name="gestureIndex">Index of the gesture to check.</param>
        /// <returns>True if the gesture is completed.</returns>
        public bool IsGestureCompleted(int gestureIndex)
        {
            if (gestureIndex < 0 || gestureIndex >= _gesturesCompleted.Length)
                return false;
            
            return _gesturesCompleted[gestureIndex];
        }
        
        /// <summary>
        /// Reset the puzzle to initial state.
        /// </summary>
        public void ResetPuzzle()
        {
            for (int i = 0; i < _gesturesCompleted.Length; i++)
            {
                _gesturesCompleted[i] = false;
                _gestureHoldTimers[i] = 0f;
            }
            
            _currentGestureIndex = 0;
            _isGestureInProgress = false;
            
            Debug.Log("VRHandGesturePuzzle: Puzzle reset to initial state");
        }
        
        /// <summary>
        /// Get the current gesture being tracked.
        /// </summary>
        /// <returns>Current gesture index, or -1 if none.</returns>
        public int GetCurrentGestureIndex()
        {
            return _isGestureInProgress ? _currentGestureIndex : -1;
        }
        
        /// <summary>
        /// Get the remaining time for the current gesture.
        /// </summary>
        /// <returns>Remaining time in seconds, or 0 if no gesture in progress.</returns>
        public float GetCurrentGestureRemainingTime()
        {
            if (!_isGestureInProgress || _currentGestureIndex < 0 || _currentGestureIndex >= requiredGestures.Length)
                return 0f;
            
            var gesture = requiredGestures[_currentGestureIndex];
            return Mathf.Max(0f, gesture.requiredHoldTime - _gestureHoldTimers[_currentGestureIndex]);
        }
    }
}
