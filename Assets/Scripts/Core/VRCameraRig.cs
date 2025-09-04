using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace Run4theRelic.Core
{
    /// <summary>
    /// VR Camera Rig för first-person perspektiv
    /// Hanterar head tracking, comfort settings och kamerapositionering
    /// </summary>
    public class VRCameraRig : MonoBehaviour
    {
        [Header("VR Camera Settings")]
        [SerializeField] private float cameraHeight = 1.7f;
        [SerializeField] private float cameraOffset = 0.1f;
        [SerializeField] private bool autoAdjustHeight = true;
        
        [Header("VR Comfort Settings")]
        [SerializeField] private bool enableBlink = true;
        [SerializeField] private bool enableVignette = true;
        [SerializeField] private bool enableSnapTurn = true;
        [SerializeField] private bool enableContinuousTurn = false;
        [SerializeField] private bool enableTeleportation = true;
        
        [Header("Comfort Values")]
        [SerializeField] private float blinkDuration = 0.3f;
        [SerializeField] private float vignetteIntensity = 0.3f;
        [SerializeField] private float snapTurnAngle = 45f;
        [SerializeField] private float continuousTurnSpeed = 60f;
        [SerializeField] private float teleportationRange = 10f;
        
        [Header("References")]
        [SerializeField] private Camera vrCamera;
        [SerializeField] private XROrigin xrOrigin;
        [SerializeField] private Transform cameraOffsetTransform;
        
        // Private fields
        private bool isVRMode = false;
        private Vector3 initialCameraPosition;
        private Quaternion initialCameraRotation;
        private float currentBlinkTime = 0f;
        private bool isBlinking = false;
        
        // Events
        public static event System.Action<bool> OnVRModeChanged;
        public static event System.Action OnCameraPositionChanged;
        
        private void Start()
        {
            InitializeVRCamera();
        }
        
        private void Update()
        {
            if (isVRMode)
            {
                UpdateVRComfort();
                UpdateCameraPosition();
            }
        }
        
        /// <summary>
        /// Initialize VR-kameran
        /// </summary>
        private void InitializeVRCamera()
        {
            // Find VR camera if not assigned
            if (vrCamera == null)
            {
                vrCamera = Camera.main;
                if (vrCamera == null)
                {
                    vrCamera = FindObjectOfType<Camera>();
                }
            }
            
            // Find XR Origin if not assigned
            if (xrOrigin == null)
            {
                xrOrigin = FindObjectOfType<XROrigin>();
            }
            
            // Find camera offset transform if not assigned
            if (cameraOffsetTransform == null)
            {
                cameraOffsetTransform = transform.Find("CameraOffset");
                if (cameraOffsetTransform == null)
                {
                    // Create camera offset if it doesn't exist
                    GameObject offsetGO = new GameObject("CameraOffset");
                    offsetGO.transform.SetParent(transform);
                    offsetGO.transform.localPosition = Vector3.zero;
                    cameraOffsetTransform = offsetGO.transform;
                }
            }
            
            // Check if VR is active
            CheckVRMode();
            
            // Setup initial camera position
            if (vrCamera != null)
            {
                initialCameraPosition = vrCamera.transform.localPosition;
                initialCameraRotation = vrCamera.transform.localRotation;
            }
            
            Debug.Log("[VRCameraRig] VR Camera initialized");
        }
        
        /// <summary>
        /// Kontrollera om VR-läget är aktivt
        /// </summary>
        private void CheckVRMode()
        {
            bool wasVRMode = isVRMode;
            isVRMode = XRSettings.isDeviceActive && XRSettings.enabled;
            
            if (wasVRMode != isVRMode)
            {
                OnVRModeChanged?.Invoke(isVRMode);
                
                if (isVRMode)
                {
                    Debug.Log("[VRCameraRig] VR mode activated");
                    SetupVRMode();
                }
                else
                {
                    Debug.Log("[VRCameraRig] VR mode deactivated");
                    SetupDesktopMode();
                }
            }
        }
        
        /// <summary>
        /// Setup VR-läge
        /// </summary>
        private void SetupVRMode()
        {
            if (vrCamera != null)
            {
                // Set camera to camera offset
                vrCamera.transform.SetParent(cameraOffsetTransform);
                vrCamera.transform.localPosition = Vector3.zero;
                vrCamera.transform.localRotation = Quaternion.identity;
                
                // Adjust camera height
                if (autoAdjustHeight)
                {
                    AdjustCameraHeight();
                }
            }
            
            // Setup XR Origin
            if (xrOrigin != null)
            {
                xrOrigin.CameraFloorOffsetObject = cameraOffsetTransform.gameObject;
            }
        }
        
        /// <summary>
        /// Setup desktop-läge
        /// </summary>
        private void SetupDesktopMode()
        {
            if (vrCamera != null)
            {
                // Reset camera to original position
                vrCamera.transform.SetParent(null);
                vrCamera.transform.localPosition = initialCameraPosition;
                vrCamera.transform.localRotation = initialCameraRotation;
            }
        }
        
        /// <summary>
        /// Justera kamerans höjd baserat på spelarens höjd
        /// </summary>
        private void AdjustCameraHeight()
        {
            if (cameraOffsetTransform != null)
            {
                Vector3 position = cameraOffsetTransform.localPosition;
                position.y = cameraHeight;
                cameraOffsetTransform.localPosition = position;
                
                Debug.Log($"[VRCameraRig] Camera height adjusted to {cameraHeight}");
            }
        }
        
        /// <summary>
        /// Update VR comfort features
        /// </summary>
        private void UpdateVRComfort()
        {
            if (enableBlink)
            {
                UpdateBlink();
            }
            
            if (enableVignette)
            {
                UpdateVignette();
            }
        }
        
        /// <summary>
        /// Update blink effect
        /// </summary>
        private void UpdateBlink()
        {
            if (isBlinking)
            {
                currentBlinkTime += Time.deltaTime;
                
                if (currentBlinkTime >= blinkDuration)
                {
                    isBlinking = false;
                    currentBlinkTime = 0f;
                    // End blink effect
                }
            }
        }
        
        /// <summary>
        /// Update vignette effect
        /// </summary>
        private void UpdateVignette()
        {
            // Vignette effect would be implemented here
            // This could involve post-processing effects or UI overlays
        }
        
        /// <summary>
        /// Update camera position
        /// </summary>
        private void UpdateCameraPosition()
        {
            if (vrCamera != null && cameraOffsetTransform != null)
            {
                // Apply camera offset
                Vector3 targetPosition = cameraOffsetTransform.position;
                targetPosition.y += cameraOffset;
                
                if (vrCamera.transform.position != targetPosition)
                {
                    vrCamera.transform.position = targetPosition;
                    OnCameraPositionChanged?.Invoke();
                }
            }
        }
        
        /// <summary>
        /// Trigger blink effect
        /// </summary>
        public void TriggerBlink()
        {
            if (enableBlink && !isBlinking)
            {
                isBlinking = true;
                currentBlinkTime = 0f;
                // Start blink effect
                Debug.Log("[VRCameraRig] Blink triggered");
            }
        }
        
        /// <summary>
        /// Set camera height manually
        /// </summary>
        public void SetCameraHeight(float height)
        {
            cameraHeight = height;
            if (isVRMode && autoAdjustHeight)
            {
                AdjustCameraHeight();
            }
        }
        
        /// <summary>
        /// Toggle VR comfort features
        /// </summary>
        public void ToggleComfortFeatures(bool enable)
        {
            enableBlink = enable;
            enableVignette = enable;
            enableSnapTurn = enable;
            enableContinuousTurn = enable;
            enableTeleportation = enable;
            
            Debug.Log($"[VRCameraRig] Comfort features {(enable ? "enabled" : "disabled")}");
        }
        
        /// <summary>
        /// Get current camera position
        /// </summary>
        public Vector3 GetCameraPosition()
        {
            return vrCamera != null ? vrCamera.transform.position : Vector3.zero;
        }
        
        /// <summary>
        /// Get current camera rotation
        /// </summary>
        public Quaternion GetCameraRotation()
        {
            return vrCamera != null ? vrCamera.transform.rotation : Quaternion.identity;
        }
        
        /// <summary>
        /// Check if VR mode is active
        /// </summary>
        public bool IsVRMode()
        {
            return isVRMode;
        }
        
        /// <summary>
        /// Get camera offset transform
        /// </summary>
        public Transform GetCameraOffsetTransform()
        {
            return cameraOffsetTransform;
        }
        
        /// <summary>
        /// Get VR camera
        /// </summary>
        public Camera GetVRCamera()
        {
            return vrCamera;
        }
        
        /// <summary>
        /// Get XR Origin
        /// </summary>
        public XROrigin GetXROrigin()
        {
            return xrOrigin;
        }
        
        private void OnDestroy()
        {
            // Cleanup
            if (vrCamera != null)
            {
                vrCamera.transform.SetParent(null);
            }
        }
    }
}
