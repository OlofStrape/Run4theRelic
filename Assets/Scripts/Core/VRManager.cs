using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

namespace Run4theRelic.Core
{
    /// <summary>
    /// Central VR-hantering för Run4theRelic
    /// Hanterar enhetsdetektering, setup och grundläggande VR-funktionalitet
    /// </summary>
    public class VRManager : MonoBehaviour
    {
        [Header("VR Device Settings")]
        [SerializeField] private bool autoDetectDevice = true;
        [SerializeField] private VRDeviceType preferredDevice = VRDeviceType.OculusQuest;
        
        [Header("VR Components")]
        [SerializeField] private XROrigin xrOrigin;
        [SerializeField] private XRInteractionManager interactionManager;
        [SerializeField] private XRController leftController;
        [SerializeField] private XRController rightController;
        
        [Header("VR Features")]
        [SerializeField] private bool enableHaptics = true;
        [SerializeField] private bool enableHandTracking = true;
        [SerializeField] private float hapticIntensity = 0.8f;
        
        // Private fields
        private VRDeviceType currentDevice = VRDeviceType.None;
        private bool isVRReady = false;
        private List<InputDevice> vrControllers = new List<InputDevice>();
        private List<InputDevice> vrHands = new List<InputDevice>();
        
        // Events
        public static event System.Action<VRDeviceType> OnVRDeviceConnected;
        public static event System.Action OnVRDeviceDisconnected;
        public static event System.Action<bool> OnVRReadyChanged;
        
        private void Start()
        {
            InitializeVR();
        }
        
        private void Update()
        {
            if (isVRReady)
            {
                UpdateVRInput();
            }
        }
        
        /// <summary>
        /// Initialize VR-systemet
        /// </summary>
        private void InitializeVR()
        {
            if (autoDetectDevice)
            {
                DetectVRDevice();
            }
            else
            {
                SetPreferredDevice(preferredDevice);
            }
            
            SetupVRComponents();
            SetupEventListeners();
        }
        
        /// <summary>
        /// Automatisk detektering av VR-enhet
        /// </summary>
        private void DetectVRDevice()
        {
            var devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(
                InputDeviceCharacteristics.HeadMounted, 
                devices
            );
            
            if (devices.Count > 0)
            {
                var device = devices[0];
                var deviceName = device.name.ToLower();
                
                if (deviceName.Contains("quest"))
                {
                    SetVRDevice(VRDeviceType.OculusQuest);
                }
                else if (deviceName.Contains("rift"))
                {
                    SetVRDevice(VRDeviceType.OculusRift);
                }
                else if (deviceName.Contains("vive"))
                {
                    SetVRDevice(VRDeviceType.HTCVive);
                }
                else if (deviceName.Contains("index"))
                {
                    SetVRDevice(VRDeviceType.ValveIndex);
                }
                else if (deviceName.Contains("windows") || deviceName.Contains("mr"))
                {
                    SetVRDevice(VRDeviceType.WindowsMR);
                }
                else
                {
                    SetVRDevice(VRDeviceType.Other);
                }
            }
            else
            {
                Debug.LogWarning("[VRManager] No VR device detected!");
                SetVRDevice(VRDeviceType.None);
            }
        }
        
        /// <summary>
        /// Sätt specifik VR-enhet
        /// </summary>
        private void SetPreferredDevice(VRDeviceType deviceType)
        {
            SetVRDevice(deviceType);
        }
        
        /// <summary>
        /// Uppdatera VR-enhet och notifiera listeners
        /// </summary>
        private void SetVRDevice(VRDeviceType deviceType)
        {
            if (currentDevice != deviceType)
            {
                currentDevice = deviceType;
                
                if (deviceType != VRDeviceType.None)
                {
                    OnVRDeviceConnected?.Invoke(deviceType);
                    Debug.Log($"[VRManager] VR device connected: {deviceType}");
                }
                else
                {
                    OnVRDeviceDisconnected?.Invoke();
                    Debug.Log("[VRManager] VR device disconnected");
                }
            }
        }
        
        /// <summary>
        /// Setup VR-komponenter
        /// </summary>
        private void SetupVRComponents()
        {
            // Find XR Origin if not assigned
            if (xrOrigin == null)
            {
                xrOrigin = FindObjectOfType<XROrigin>();
            }
            
            // Find Interaction Manager if not assigned
            if (interactionManager == null)
            {
                interactionManager = FindObjectOfType<XRInteractionManager>();
            }
            
            // Find controllers if not assigned
            if (leftController == null || rightController == null)
            {
                var controllers = FindObjectsOfType<XRController>();
                foreach (var controller in controllers)
                {
                    if (controller.controllerNode == XRNode.LeftHand)
                    {
                        leftController = controller;
                    }
                    else if (controller.controllerNode == XRNode.RightHand)
                    {
                        rightController = controller;
                    }
                }
            }
            
            // Check if VR is ready
            isVRReady = (xrOrigin != null && interactionManager != null);
            OnVRReadyChanged?.Invoke(isVRReady);
            
            if (isVRReady)
            {
                Debug.Log("[VRManager] VR components setup complete");
            }
            else
            {
                Debug.LogWarning("[VRManager] Some VR components are missing!");
            }
        }
        
        /// <summary>
        /// Setup event listeners
        /// </summary>
        private void SetupEventListeners()
        {
            // Listen for VR device changes
            InputDevices.deviceConnected += OnDeviceConnected;
            InputDevices.deviceDisconnected += OnDeviceDisconnected;
        }
        
        /// <summary>
        /// Update VR input
        /// </summary>
        private void UpdateVRInput()
        {
            // Update controller list
            InputDevices.GetDevicesWithCharacteristics(
                InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.HeldInHand,
                vrControllers
            );
            
            // Update hand tracking if enabled
            if (enableHandTracking)
            {
                InputDevices.GetDevicesWithCharacteristics(
                    InputDeviceCharacteristics.HandTracking,
                    vrHands
                );
            }
        }
        
        /// <summary>
        /// Handle device connection
        /// </summary>
        private void OnDeviceConnected(InputDevice device)
        {
            Debug.Log($"[VRManager] Device connected: {device.name}");
            
            if (device.characteristics.HasFlag(InputDeviceCharacteristics.HeadMounted))
            {
                DetectVRDevice();
            }
        }
        
        /// <summary>
        /// Handle device disconnection
        /// </summary>
        private void OnDeviceDisconnected(InputDevice device)
        {
            Debug.Log($"[VRManager] Device disconnected: {device.name}");
            
            if (device.characteristics.HasFlag(InputDeviceCharacteristics.HeadMounted))
            {
                SetVRDevice(VRDeviceType.None);
            }
        }
        
        /// <summary>
        /// Send haptic feedback to controller
        /// </summary>
        public void SendHapticFeedback(XRNode controllerNode, float intensity = 1.0f, float duration = 0.1f)
        {
            if (!enableHaptics) return;
            
            var controller = (controllerNode == XRNode.LeftHand) ? leftController : rightController;
            if (controller != null)
            {
                controller.SendHapticImpulse(intensity * hapticIntensity, duration);
            }
        }
        
        /// <summary>
        /// Send haptic feedback to both controllers
        /// </summary>
        public void SendHapticFeedbackToBoth(float intensity = 1.0f, float duration = 0.1f)
        {
            SendHapticFeedback(XRNode.LeftHand, intensity, duration);
            SendHapticFeedback(XRNode.RightHand, intensity, duration);
        }
        
        /// <summary>
        /// Get left controller
        /// </summary>
        public XRController GetLeftController()
        {
            return leftController;
        }
        
        /// <summary>
        /// Get right controller
        /// </summary>
        public XRController GetRightController()
        {
            return rightController;
        }
        
        /// <summary>
        /// Get XR Origin
        /// </summary>
        public XROrigin GetXROrigin()
        {
            return xrOrigin;
        }
        
        /// <summary>
        /// Get Interaction Manager
        /// </summary>
        public XRInteractionManager GetInteractionManager()
        {
            return interactionManager;
        }
        
        /// <summary>
        /// Check if VR is ready
        /// </summary>
        public bool IsVRReady()
        {
            return isVRReady;
        }
        
        /// <summary>
        /// Get current VR device type
        /// </summary>
        public VRDeviceType GetCurrentDevice()
        {
            return currentDevice;
        }
        
        /// <summary>
        /// Get VR controllers
        /// </summary>
        public List<InputDevice> GetVRControllers()
        {
            return new List<InputDevice>(vrControllers);
        }
        
        /// <summary>
        /// Get VR hands
        /// </summary>
        public List<InputDevice> GetVRHands()
        {
            return new List<InputDevice>(vrHands);
        }
        
        private void OnDestroy()
        {
            // Clean up event listeners
            InputDevices.deviceConnected -= OnDeviceConnected;
            InputDevices.deviceDisconnected -= OnDeviceDisconnected;
        }
    }
}
