using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace Run4theRelic.Core
{
    /// <summary>
    /// Manages VR camera rig for first person perspective.
    /// Handles head tracking, comfort settings, and camera positioning.
    /// </summary>
    public class VRCameraRig : MonoBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private Camera vrCamera;
        [SerializeField] private Transform cameraOffset;
        [SerializeField] private float cameraHeight = 1.6f;
        [SerializeField] private bool autoAdjustHeight = true;
        
        [Header("Comfort Settings")]
        [SerializeField] private bool enableBlink = true;
        [SerializeField] private bool enableVignette = true;
        [SerializeField] private float blinkDuration = 0.3f;
        [SerializeField] private float vignetteIntensity = 0.3f;
        [SerializeField] private float maxTurnAngle = 45f;
        [SerializeField] private bool enableSnapTurn = true;
        [SerializeField] private float snapTurnAngle = 45f;
        
        [Header("Movement Settings")]
        [SerializeField] private bool enableContinuousTurn = true;
        [SerializeField] private float continuousTurnSpeed = 60f;
        [SerializeField] private bool enableTeleportation = true;
        [SerializeField] private float teleportationRange = 10f;
        
        [Header("References")]
        [SerializeField] private XROrigin xrOrigin;
        [SerializeField] private Transform playerBody;
        [SerializeField] private Transform leftHandAnchor;
        [SerializeField] private Transform rightHandAnchor;
        
        // Camera State
        private bool _isInitialized = false;
        private Vector3 _initialCameraPosition;
        private Quaternion _initialCameraRotation;
        private float _currentTurnAngle = 0f;
        
        // Comfort Features
        private bool _isBlinking = false;
        private float _blinkTimer = 0f;
        private Material _vignetteMaterial;
        
        // Events
        public static event System.Action<Vector3> OnCameraPositionChanged;
        public static event System.Action<Quaternion> OnCameraRotationChanged;
        public static event System.Action<bool> OnBlinkStateChanged;
        
        // Properties
        public Camera VRCamera => vrCamera;
        public Transform CameraOffset => cameraOffset;
        public bool IsInitialized => _isInitialized;
        public bool IsBlinking => _isBlinking;
        
        private void Awake()
        {
            // Ensure singleton pattern
            if (FindObjectsOfType<VRCameraRig>().Length > 1)
            {
                Destroy(gameObject);
                return;
            }
            
            DontDestroyOnLoad(gameObject);
        }
        
        private void Start()
        {
            InitializeCameraRig();
        }
        
        private void Update()
        {
            if (!_isInitialized) return;
            
            // Update camera tracking
            UpdateCameraTracking();
            
            // Update comfort features
            UpdateComfortFeatures();
            
            // Update movement
            UpdateMovement();
        }
        
        /// <summary>
        /// Initialize the VR camera rig.
        /// </summary>
        private void InitializeCameraRig()
        {
            Debug.Log("VRCameraRig: Initializing camera rig...");
            
            // Find components if not assigned
            FindComponents();
            
            // Setup camera
            SetupCamera();
            
            // Setup comfort features
            SetupComfortFeatures();
            
            // Setup movement
            SetupMovement();
            
            _isInitialized = true;
            
            Debug.Log("VRCameraRig: Camera rig initialized successfully");
        }
        
        /// <summary>
        /// Find and assign required components.
        /// </summary>
        private void FindComponents()
        {
            // Find VR camera
            if (vrCamera == null)
            {
                vrCamera = Camera.main;
                if (vrCamera == null)
                {
                    vrCamera = FindObjectOfType<Camera>();
                }
            }
            
            // Find XR Origin
            if (xrOrigin == null)
            {
                xrOrigin = FindObjectOfType<XROrigin>();
            }
            
            // Find camera offset
            if (cameraOffset == null)
            {
                cameraOffset = transform.Find("CameraOffset");
                if (cameraOffset == null)
                {
                    cameraOffset = new GameObject("CameraOffset").transform;
                    cameraOffset.SetParent(transform);
                    cameraOffset.localPosition = Vector3.zero;
                }
            }
            
            // Find player body
            if (playerBody == null)
            {
                playerBody = transform.Find("PlayerBody");
                if (playerBody == null)
                {
                    playerBody = new GameObject("PlayerBody").transform;
                    playerBody.SetParent(transform);
                    playerBody.localPosition = Vector3.zero;
                }
            }
            
            // Find hand anchors
            if (leftHandAnchor == null)
            {
                leftHandAnchor = transform.Find("LeftHandAnchor");
                if (leftHandAnchor == null)
                {
                    leftHandAnchor = new GameObject("LeftHandAnchor").transform;
                    leftHandAnchor.SetParent(transform);
                    leftHandAnchor.localPosition = new Vector3(-0.3f, 0.5f, 0.5f);
                }
            }
            
            if (rightHandAnchor == null)
            {
                rightHandAnchor = transform.Find("RightHandAnchor");
                if (rightHandAnchor == null)
                {
                    rightHandAnchor = new GameObject("RightHandAnchor").transform;
                    rightHandAnchor.SetParent(transform);
                    rightHandAnchor.localPosition = new Vector3(0.3f, 0.5f, 0.5f);
                }
            }
        }
        
        /// <summary>
        /// Setup VR camera.
        /// </summary>
        private void SetupCamera()
        {
            if (vrCamera == null) return;
            
            // Store initial camera state
            _initialCameraPosition = vrCamera.transform.position;
            _initialCameraRotation = vrCamera.transform.rotation;
            
            // Setup camera for VR
            vrCamera.nearClipPlane = 0.01f;
            vrCamera.farClipPlane = 1000f;
            
            // Position camera at head height
            if (autoAdjustHeight)
            {
                AdjustCameraHeight();
            }
            
            // Parent camera to offset
            if (cameraOffset != null)
            {
                vrCamera.transform.SetParent(cameraOffset);
                vrCamera.transform.localPosition = Vector3.zero;
                vrCamera.transform.localRotation = Quaternion.identity;
            }
            
            Debug.Log($"VRCameraRig: Camera setup complete. Position: {vrCamera.transform.position}");
        }
        
        /// <summary>
        /// Setup comfort features.
        /// </summary>
        private void SetupComfortFeatures()
        {
            // Create vignette material if needed
            if (enableVignette && _vignetteMaterial == null)
            {
                _vignetteMaterial = new Material(Shader.Find("Standard"));
                _vignetteMaterial.color = new Color(0, 0, 0, vignetteIntensity);
            }
        }
        
        /// <summary>
        /// Setup movement system.
        /// </summary>
        private void SetupMovement()
        {
            // This would integrate with XR Interaction Toolkit movement providers
            // For now, we'll handle basic camera movement
        }
        
        /// <summary>
        /// Update camera tracking.
        /// </summary>
        private void UpdateCameraTracking()
        {
            if (vrCamera == null) return;
            
            // Update camera position and rotation events
            OnCameraPositionChanged?.Invoke(vrCamera.transform.position);
            OnCameraRotationChanged?.Invoke(vrCamera.transform.rotation);
            
            // Update player body position to follow camera
            if (playerBody != null)
            {
                Vector3 cameraPos = vrCamera.transform.position;
                playerBody.position = new Vector3(cameraPos.x, transform.position.y, cameraPos.z);
            }
        }
        
        /// <summary>
        /// Update comfort features.
        /// </summary>
        private void UpdateComfortFeatures()
        {
            // Update blink state
            if (_isBlinking)
            {
                _blinkTimer -= Time.deltaTime;
                if (_blinkTimer <= 0f)
                {
                    EndBlink();
                }
            }
            
            // Update vignette
            if (enableVignette && _vignetteMaterial != null)
            {
                // Apply vignette effect
                float intensity = _isBlinking ? vignetteIntensity * (1f - _blinkTimer / blinkDuration) : vignetteIntensity;
                _vignetteMaterial.color = new Color(0, 0, 0, intensity);
            }
        }
        
        /// <summary>
        /// Update movement system.
        /// </summary>
        private void UpdateMovement()
        {
            // Handle continuous turning
            if (enableContinuousTurn)
            {
                HandleContinuousTurn();
            }
            
            // Handle snap turning
            if (enableSnapTurn)
            {
                HandleSnapTurn();
            }
        }
        
        /// <summary>
        /// Handle continuous turning input.
        /// </summary>
        private void HandleContinuousTurn()
        {
            // This would integrate with XR input system
            // For now, we'll handle basic thumbstick input
        }
        
        /// <summary>
        /// Handle snap turning input.
        /// </summary>
        private void HandleSnapTurn()
        {
            // This would integrate with XR input system
            // For now, we'll handle basic button input
        }
        
        /// <summary>
        /// Adjust camera height based on player height.
        /// </summary>
        public void AdjustCameraHeight()
        {
            if (vrCamera == null || cameraOffset == null) return;
            
            // Get player height from VR system
            float playerHeight = GetPlayerHeight();
            
            // Adjust camera offset height
            Vector3 offsetPos = cameraOffset.localPosition;
            offsetPos.y = playerHeight;
            cameraOffset.localPosition = offsetPos;
            
            Debug.Log($"VRCameraRig: Adjusted camera height to {playerHeight}m");
        }
        
        /// <summary>
        /// Get current player height from VR system.
        /// </summary>
        /// <returns>Player height in meters.</returns>
        private float GetPlayerHeight()
        {
            // This would get actual player height from VR system
            // For now, return default height
            return cameraHeight;
        }
        
        /// <summary>
        /// Start blink effect.
        /// </summary>
        /// <param name="duration">Blink duration in seconds.</param>
        public void StartBlink(float duration = -1f)
        {
            if (!enableBlink) return;
            
            float blinkDurationToUse = duration >= 0f ? duration : this.blinkDuration;
            
            _isBlinking = true;
            _blinkTimer = blinkDurationToUse;
            
            OnBlinkStateChanged?.Invoke(true);
            
            Debug.Log($"VRCameraRig: Started blink for {blinkDurationToUse}s");
        }
        
        /// <summary>
        /// End blink effect.
        /// </summary>
        private void EndBlink()
        {
            _isBlinking = false;
            _blinkTimer = 0f;
            
            OnBlinkStateChanged?.Invoke(false);
        }
        
        /// <summary>
        /// Teleport camera to new position.
        /// </summary>
        /// <param name="targetPosition">Target position.</param>
        /// <param name="useBlink">Whether to use blink effect during teleport.</param>
        public void TeleportTo(Vector3 targetPosition, bool useBlink = true)
        {
            if (!enableTeleportation) return;
            
            // Check teleportation range
            float distance = Vector3.Distance(transform.position, targetPosition);
            if (distance > teleportationRange)
            {
                Debug.LogWarning($"VRCameraRig: Teleportation distance {distance}m exceeds range {teleportationRange}m");
                return;
            }
            
            // Start blink if enabled
            if (useBlink && enableBlink)
            {
                StartBlink();
            }
            
            // Move camera rig
            transform.position = targetPosition;
            
            Debug.Log($"VRCameraRig: Teleported to {targetPosition}");
        }
        
        /// <summary>
        /// Rotate camera rig by angle.
        /// </summary>
        /// <param name="angle">Rotation angle in degrees.</param>
        public void RotateBy(float angle)
        {
            if (Mathf.Abs(angle) > maxTurnAngle)
            {
                angle = Mathf.Sign(angle) * maxTurnAngle;
            }
            
            transform.Rotate(0, angle, 0);
            _currentTurnAngle += angle;
            
            Debug.Log($"VRCameraRig: Rotated by {angle}° (total: {_currentTurnAngle}°)");
        }
        
        /// <summary>
        /// Reset camera to initial position and rotation.
        /// </summary>
        public void ResetCamera()
        {
            if (vrCamera == null) return;
            
            vrCamera.transform.position = _initialCameraPosition;
            vrCamera.transform.rotation = _initialCameraRotation;
            _currentTurnAngle = 0f;
            
            Debug.Log("VRCameraRig: Camera reset to initial state");
        }
        
        /// <summary>
        /// Toggle comfort features.
        /// </summary>
        /// <param name="enable">Enable or disable comfort features.</param>
        public void ToggleComfortFeatures(bool enable)
        {
            enableBlink = enable;
            enableVignette = enable;
            
            Debug.Log($"VRCameraRig: Comfort features {(enable ? "enabled" : "disabled")}");
        }
        
        private void OnDestroy()
        {
            // Cleanup materials
            if (_vignetteMaterial != null)
            {
                DestroyImmediate(_vignetteMaterial);
            }
        }
    }
}
