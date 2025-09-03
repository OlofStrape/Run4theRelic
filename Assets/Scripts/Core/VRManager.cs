using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

namespace Run4theRelic.Core
{
    /// <summary>
    /// Central manager for all VR functionality in Run4theRelic.
    /// Handles VR input, camera setup, comfort settings, and device detection.
    /// </summary>
    public class VRManager : MonoBehaviour
    {
        [Header("VR Setup")]
        [SerializeField] private bool autoSetupVR = true;
        [SerializeField] private GameObject vrRigPrefab;
        [SerializeField] private Transform playerSpawnPoint;
        
        [Header("VR Comfort Settings")]
        [SerializeField] private bool enableBlink = true;
        [SerializeField] private bool enableVignette = true;
        [SerializeField] private float blinkDuration = 0.3f;
        [SerializeField] private float vignetteIntensity = 0.3f;
        
        [Header("VR Input")]
        [SerializeField] private bool enableHandTracking = true;
        [SerializeField] private bool enableHapticFeedback = true;
        [SerializeField] private float hapticIntensity = 0.5f;
        
        [Header("References")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private XRInteractionManager interactionManager;
        [SerializeField] private XROrigin xrOrigin;
        
        // VR State
        private bool _isVRMode = false;
        private bool _isInitialized = false;
        private XRDevice _currentDevice = XRDevice.Unknown;
        
        // VR Components
        private GameObject _vrRig;
        private XRController _leftController;
        private XRController _rightController;
        private XRHandSubsystem _handSubsystem;
        
        // Events
        public static event System.Action<bool> OnVRModeChanged;
        public static event System.Action<XRDevice> OnVRDeviceChanged;
        
        /// <summary>
        /// Is the game currently running in VR mode.
        /// </summary>
        public bool IsVRMode => _isVRMode;
        
        /// <summary>
        /// Current VR device being used.
        /// </summary>
        public XRDevice CurrentDevice => _currentDevice;
        
        /// <summary>
        /// Is VR system fully initialized.
        /// </summary>
        public bool IsInitialized => _isInitialized;
        
        private void Awake()
        {
            // Ensure singleton pattern
            if (FindObjectsOfType<VRManager>().Length > 1)
            {
                Destroy(gameObject);
                return;
            }
            
            DontDestroyOnLoad(gameObject);
        }
        
        private void Start()
        {
            if (autoSetupVR)
            {
                InitializeVR();
            }
        }
        
        /// <summary>
        /// Initialize VR system and setup.
        /// </summary>
        public void InitializeVR()
        {
            if (_isInitialized) return;
            
            Debug.Log("VRManager: Initializing VR system...");
            
            // Check if VR is available
            if (!XRSettings.isDeviceActive)
            {
                Debug.LogWarning("VRManager: No VR device detected, running in desktop mode");
                _isVRMode = false;
                _isInitialized = true;
                return;
            }
            
            // Setup VR
            SetupVRMode();
            
            // Initialize components
            InitializeVRComponents();
            
            // Setup comfort settings
            SetupComfortSettings();
            
            _isInitialized = true;
            _isVRMode = true;
            
            OnVRModeChanged?.Invoke(true);
            OnVRDeviceChanged?.Invoke(_currentDevice);
            
            Debug.Log($"VRManager: VR system initialized successfully. Device: {_currentDevice}");
        }
        
        /// <summary>
        /// Setup VR mode and create VR rig.
        /// </summary>
        private void SetupVRMode()
        {
            // Create VR rig if prefab is assigned
            if (vrRigPrefab != null)
            {
                _vrRig = Instantiate(vrRigPrefab);
                if (playerSpawnPoint != null)
                {
                    _vrRig.transform.position = playerSpawnPoint.position;
                    _vrRig.transform.rotation = playerSpawnPoint.rotation;
                }
            }
            
            // Setup XR Origin
            if (xrOrigin == null)
            {
                xrOrigin = FindObjectOfType<XROrigin>();
            }
            
            if (xrOrigin == null)
            {
                Debug.LogError("VRManager: No XROrigin found! VR setup may not work correctly.");
            }
            
            // Setup main camera
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
            
            // Detect VR device
            DetectVRDevice();
        }
        
        /// <summary>
        /// Initialize VR-specific components.
        /// </summary>
        private void InitializeVRComponents()
        {
            // Setup interaction manager
            if (interactionManager == null)
            {
                interactionManager = FindObjectOfType<XRInteractionManager>();
            }
            
            // Setup controllers
            SetupControllers();
            
            // Setup hand tracking
            if (enableHandTracking)
            {
                SetupHandTracking();
            }
        }
        
        /// <summary>
        /// Setup VR controllers.
        /// </summary>
        private void SetupControllers()
        {
            if (_vrRig != null)
            {
                _leftController = _vrRig.GetComponentInChildren<XRController>();
                _rightController = _vrRig.GetComponentInChildren<XRController>();
            }
            
            if (_leftController == null || _rightController == null)
            {
                Debug.LogWarning("VRManager: Controllers not found, some VR features may not work");
            }
        }
        
        /// <summary>
        /// Setup hand tracking system.
        /// </summary>
        private void SetupHandTracking()
        {
            var subsystems = new List<XRHandSubsystem>();
            SubsystemManager.GetInstances(subsystems);
            
            if (subsystems.Count > 0)
            {
                _handSubsystem = subsystems[0];
                Debug.Log("VRManager: Hand tracking system initialized");
            }
            else
            {
                Debug.LogWarning("VRManager: Hand tracking not available on this device");
            }
        }
        
        /// <summary>
        /// Setup VR comfort settings.
        /// </summary>
        private void SetupComfortSettings()
        {
            // Apply comfort settings based on device
            switch (_currentDevice)
            {
                case XRDevice.Oculus:
                    // Oculus-specific comfort settings
                    break;
                case XRDevice.Vive:
                    // Vive-specific comfort settings
                    break;
                case XRDevice.WindowsMR:
                    // Windows MR-specific comfort settings
                    break;
            }
        }
        
        /// <summary>
        /// Detect current VR device.
        /// </summary>
        private void DetectVRDevice()
        {
            string deviceName = XRSettings.loadedDeviceName.ToLower();
            
            if (deviceName.Contains("oculus") || deviceName.Contains("quest"))
            {
                _currentDevice = XRDevice.Oculus;
            }
            else if (deviceName.Contains("vive"))
            {
                _currentDevice = XRDevice.Vive;
            }
            else if (deviceName.Contains("windows") || deviceName.Contains("mr"))
            {
                _currentDevice = XRDevice.WindowsMR;
            }
            else
            {
                _currentDevice = XRDevice.Unknown;
            }
            
            Debug.Log($"VRManager: Detected VR device: {_currentDevice} ({deviceName})");
        }
        
        /// <summary>
        /// Trigger haptic feedback on controller.
        /// </summary>
        /// <param name="controller">Controller to trigger haptics on.</param>
        /// <param name="intensity">Haptic intensity (0-1).</param>
        /// <param name="duration">Haptic duration in seconds.</param>
        public void TriggerHapticFeedback(XRController controller, float intensity = -1f, float duration = 0.1f)
        {
            if (!enableHapticFeedback || controller == null) return;
            
            float hapticIntensityToUse = intensity >= 0f ? intensity : this.hapticIntensity;
            
            if (controller.inputDevice.isValid)
            {
                HapticCapabilities capabilities;
                if (controller.inputDevice.TryGetHapticCapabilities(out capabilities))
                {
                    if (capabilities.supportsImpulse)
                    {
                        controller.inputDevice.SendHapticImpulse(0, hapticIntensityToUse, duration);
                    }
                }
            }
        }
        
        /// <summary>
        /// Trigger haptic feedback on both controllers.
        /// </summary>
        /// <param name="intensity">Haptic intensity (0-1).</param>
        /// <param name="duration">Haptic duration in seconds.</param>
        public void TriggerHapticFeedbackBoth(float intensity = -1f, float duration = 0.1f)
        {
            if (_leftController != null)
            {
                TriggerHapticFeedback(_leftController, intensity, duration);
            }
            
            if (_rightController != null)
            {
                TriggerHapticFeedback(_rightController, intensity, duration);
            }
        }
        
        /// <summary>
        /// Get VR rig GameObject.
        /// </summary>
        /// <returns>VR rig GameObject or null if not created.</returns>
        public GameObject GetVRRig()
        {
            return _vrRig;
        }
        
        /// <summary>
        /// Get left controller.
        /// </summary>
        /// <returns>Left XRController or null if not found.</returns>
        public XRController GetLeftController()
        {
            return _leftController;
        }
        
        /// <summary>
        /// Get right controller.
        /// </summary>
        /// <returns>Right XRController or null if not found.</returns>
        public XRController GetRightController()
        {
            return _rightController;
        }
        
        /// <summary>
        /// Toggle VR comfort settings.
        /// </summary>
        /// <param name="enable">Enable or disable comfort settings.</param>
        public void ToggleComfortSettings(bool enable)
        {
            enableBlink = enable;
            enableVignette = enable;
            
            Debug.Log($"VRManager: Comfort settings {(enable ? "enabled" : "disabled")}");
        }
        
        private void OnDestroy()
        {
            // Cleanup
            if (_handSubsystem != null)
            {
                _handSubsystem = null;
            }
        }
    }
    
    /// <summary>
    /// VR device types for device-specific optimizations.
    /// </summary>
    public enum XRDevice
    {
        Unknown,
        Oculus,
        Vive,
        WindowsMR
    }
}
