using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

namespace Run4theRelic.Core
{
    /// <summary>
    /// VR Input Manager för Run4theRelic
    /// Hanterar all VR-input inklusive controllers, hand gestures och input events
    /// </summary>
    public class VRInputManager : MonoBehaviour
    {
        [Header("VR Input Settings")]
        [SerializeField] private bool enableControllerInput = true;
        [SerializeField] private bool enableHandTracking = true;
        [SerializeField] private bool enableGestureRecognition = true;
        [SerializeField] private float gestureHoldTime = 0.5f;
        [SerializeField] private float gestureTolerance = 0.1f;
        
        [Header("Input Thresholds")]
        [SerializeField] private float triggerThreshold = 0.5f;
        [SerializeField] private float gripThreshold = 0.5f;
        [SerializeField] private float thumbstickThreshold = 0.3f;
        
        // Private fields
        private List<InputDevice> vrControllers = new List<InputDevice>();
        private List<InputDevice> vrHands = new List<InputDevice>();
        private Dictionary<InputDevice, InputDeviceState> deviceStates = new Dictionary<InputDevice, InputDeviceState>();
        
        // Input state
        private bool leftTriggerPressed = false;
        private bool rightTriggerPressed = false;
        private bool leftGripPressed = false;
        private bool rightGripPressed = false;
        private Vector2 leftThumbstick = Vector2.zero;
        private Vector2 rightThumbstick = Vector2.zero;
        
        // Gesture state
        private HandGesture leftHandGesture = HandGesture.OpenHand;
        private HandGesture rightHandGesture = HandGesture.OpenHand;
        private float leftGestureHoldTimer = 0f;
        private float rightGestureHoldTimer = 0f;
        
        // Events
        public static event System.Action<bool> OnLeftTriggerChanged;
        public static event System.Action<bool> OnRightTriggerChanged;
        public static event System.Action<bool> OnLeftGripChanged;
        public static event System.Action<bool> OnRightGripChanged;
        public static event System.Action<Vector2> OnLeftThumbstickChanged;
        public static event System.Action<Vector2> OnRightThumbstickChanged;
        public static event System.Action<HandGesture> OnLeftHandGestureChanged;
        public static event System.Action<HandGesture> OnRightHandGestureChanged;
        public static event System.Action<InputDevice> OnVRDeviceConnected;
        public static event System.Action<InputDevice> OnVRDeviceDisconnected;
        
        private void Start()
        {
            InitializeVRInput();
        }
        
        private void Update()
        {
            UpdateVRInput();
            UpdateHandGestures();
        }
        
        /// <summary>
        /// Initialize VR-inputsystemet
        /// </summary>
        private void InitializeVRInput()
        {
            // Find VR devices
            FindVRDevices();
            
            // Setup event listeners
            InputDevices.deviceConnected += OnDeviceConnected;
            InputDevices.deviceDisconnected += OnDeviceDisconnected;
            
            Debug.Log("[VRInputManager] VR Input System initialized");
        }
        
        /// <summary>
        /// Hitta VR-enheter
        /// </summary>
        private void FindVRDevices()
        {
            // Find controllers
            if (enableControllerInput)
            {
                InputDevices.GetDevicesWithCharacteristics(
                    InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.HeldInHand,
                    vrControllers
                );
                
                foreach (var controller in vrControllers)
                {
                    if (!deviceStates.ContainsKey(controller))
                    {
                        deviceStates[controller] = new InputDeviceState();
                    }
                }
            }
            
            // Find hands
            if (enableHandTracking)
            {
                InputDevices.GetDevicesWithCharacteristics(
                    InputDeviceCharacteristics.HandTracking,
                    vrHands
                );
                
                foreach (var hand in vrHands)
                {
                    if (!deviceStates.ContainsKey(hand))
                    {
                        deviceStates[hand] = new InputDeviceState();
                    }
                }
            }
            
            Debug.Log($"[VRInputManager] Found {vrControllers.Count} controllers and {vrHands.Count} hands");
        }
        
        /// <summary>
        /// Update VR-input
        /// </summary>
        private void UpdateVRInput()
        {
            if (!enableControllerInput) return;
            
            // Update controller input
            foreach (var controller in vrControllers)
            {
                if (controller.isValid)
                {
                    UpdateControllerInput(controller);
                }
            }
        }
        
        /// <summary>
        /// Update controller input
        /// </summary>
        private void UpdateControllerInput(InputDevice controller)
        {
            // Determine if this is left or right controller
            bool isLeftController = IsLeftController(controller);
            
            // Update trigger
            if (controller.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
            {
                bool triggerPressed = triggerValue > triggerThreshold;
                
                if (isLeftController)
                {
                    if (leftTriggerPressed != triggerPressed)
                    {
                        leftTriggerPressed = triggerPressed;
                        OnLeftTriggerChanged?.Invoke(triggerPressed);
                    }
                }
                else
                {
                    if (rightTriggerPressed != triggerPressed)
                    {
                        rightTriggerPressed = triggerPressed;
                        OnRightTriggerChanged?.Invoke(triggerPressed);
                    }
                }
            }
            
            // Update grip
            if (controller.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
            {
                bool gripPressed = gripValue > gripThreshold;
                
                if (isLeftController)
                {
                    if (leftGripPressed != gripPressed)
                    {
                        leftGripPressed = gripPressed;
                        OnLeftGripChanged?.Invoke(gripPressed);
                    }
                }
                else
                {
                    if (rightGripPressed != gripPressed)
                    {
                        rightGripPressed = gripPressed;
                        OnRightGripChanged?.Invoke(gripPressed);
                    }
                }
            }
            
            // Update thumbstick
            if (controller.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 thumbstick))
            {
                if (Mathf.Abs(thumbstick.x) > thumbstickThreshold || Mathf.Abs(thumbstick.y) > thumbstickThreshold)
                {
                    if (isLeftController)
                    {
                        if (leftThumbstick != thumbstick)
                        {
                            leftThumbstick = thumbstick;
                            OnLeftThumbstickChanged?.Invoke(thumbstick);
                        }
                    }
                    else
                    {
                        if (rightThumbstick != thumbstick)
                        {
                            rightThumbstick = thumbstick;
                            OnRightThumbstickChanged?.Invoke(thumbstick);
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Update hand gestures
        /// </summary>
        private void UpdateHandGestures()
        {
            if (!enableGestureRecognition) return;
            
            // Update left hand gesture
            UpdateHandGesture(XRNode.LeftHand, ref leftHandGesture, ref leftGestureHoldTimer);
            
            // Update right hand gesture
            UpdateHandGesture(XRNode.RightHand, ref rightHandGesture, ref rightGestureHoldTimer);
        }
        
        /// <summary>
        /// Update hand gesture for specific hand
        /// </summary>
        private void UpdateHandGesture(XRNode handNode, ref HandGesture currentGesture, ref float holdTimer)
        {
            var hand = GetHandDevice(handNode);
            if (!hand.isValid) return;
            
            // Get hand pose data
            if (hand.TryGetFeatureValue(CommonUsages.handData, out Hand handData))
            {
                var newGesture = RecognizeHandGesture(handData);
                
                if (newGesture != currentGesture)
                {
                    // Check if gesture should change
                    if (newGesture == HandGesture.OpenHand)
                    {
                        // Reset hold timer for open hand
                        holdTimer = 0f;
                    }
                    else
                    {
                        // Increment hold timer for closed gestures
                        holdTimer += Time.deltaTime;
                        
                        if (holdTimer >= gestureHoldTime)
                        {
                            // Gesture held long enough, change it
                            currentGesture = newGesture;
                            
                            if (handNode == XRNode.LeftHand)
                            {
                                OnLeftHandGestureChanged?.Invoke(currentGesture);
                            }
                            else
                            {
                                OnRightHandGestureChanged?.Invoke(currentGesture);
                            }
                            
                            Debug.Log($"[VRInputManager] {handNode} gesture changed to: {currentGesture}");
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Recognize hand gesture from hand data
        /// </summary>
        private HandGesture RecognizeHandGesture(Hand handData)
        {
            // This is a simplified gesture recognition
            // In a real implementation, you would analyze finger positions more carefully
            
            bool thumbExtended = handData.GetFingerIsPinching(HandFinger.Thumb);
            bool indexExtended = handData.GetFingerIsPinching(HandFinger.Index);
            bool middleExtended = handData.GetFingerIsPinching(HandFinger.Middle);
            bool ringExtended = handData.GetFingerIsPinching(HandFinger.Ring);
            bool pinkyExtended = handData.GetFingerIsPinching(HandFinger.Little);
            
            // Count extended fingers
            int extendedFingers = 0;
            if (thumbExtended) extendedFingers++;
            if (indexExtended) extendedFingers++;
            if (middleExtended) extendedFingers++;
            if (ringExtended) extendedFingers++;
            if (pinkyExtended) extendedFingers++;
            
            // Determine gesture based on finger count and positions
            if (extendedFingers == 0)
            {
                return HandGesture.Fist;
            }
            else if (extendedFingers == 1 && indexExtended)
            {
                return HandGesture.Point;
            }
            else if (extendedFingers == 2 && thumbExtended && indexExtended)
            {
                return HandGesture.ThumbsUp;
            }
            else if (extendedFingers == 5)
            {
                return HandGesture.OpenHand;
            }
            else if (extendedFingers == 3 && !thumbExtended && !pinkyExtended)
            {
                return HandGesture.Grab;
            }
            else
            {
                return HandGesture.OpenHand;
            }
        }
        
        /// <summary>
        /// Get hand device for specific hand node
        /// </summary>
        private InputDevice GetHandDevice(XRNode handNode)
        {
            foreach (var hand in vrHands)
            {
                if (hand.characteristics.HasFlag(InputDeviceCharacteristics.HandTracking))
                {
                    if ((handNode == XRNode.LeftHand && hand.characteristics.HasFlag(InputDeviceCharacteristics.Left)) ||
                        (handNode == XRNode.RightHand && hand.characteristics.HasFlag(InputDeviceCharacteristics.Right)))
                    {
                        return hand;
                    }
                }
            }
            
            return default;
        }
        
        /// <summary>
        /// Check if controller is left controller
        /// </summary>
        private bool IsLeftController(InputDevice controller)
        {
            return controller.characteristics.HasFlag(InputDeviceCharacteristics.Left);
        }
        
        /// <summary>
        /// Handle device connection
        /// </summary>
        private void OnDeviceConnected(InputDevice device)
        {
            Debug.Log($"[VRInputManager] Device connected: {device.name}");
            
            if (device.characteristics.HasFlag(InputDeviceCharacteristics.Controller))
            {
                if (!vrControllers.Contains(device))
                {
                    vrControllers.Add(device);
                    deviceStates[device] = new InputDeviceState();
                }
            }
            else if (device.characteristics.HasFlag(InputDeviceCharacteristics.HandTracking))
            {
                if (!vrHands.Contains(device))
                {
                    vrHands.Add(device);
                    deviceStates[device] = new InputDeviceState();
                }
            }
            
            OnVRDeviceConnected?.Invoke(device);
        }
        
        /// <summary>
        /// Handle device disconnection
        /// </summary>
        private void OnDeviceDisconnected(InputDevice device)
        {
            Debug.Log($"[VRInputManager] Device disconnected: {device.name}");
            
            if (vrControllers.Contains(device))
            {
                vrControllers.Remove(device);
                deviceStates.Remove(device);
            }
            
            if (vrHands.Contains(device))
            {
                vrHands.Remove(device);
                deviceStates.Remove(device);
            }
            
            OnVRDeviceDisconnected?.Invoke(device);
        }
        
        /// <summary>
        /// Get left trigger state
        /// </summary>
        public bool IsLeftTriggerPressed()
        {
            return leftTriggerPressed;
        }
        
        /// <summary>
        /// Get right trigger state
        /// </summary>
        public bool IsRightTriggerPressed()
        {
            return rightTriggerPressed;
        }
        
        /// <summary>
        /// Get left grip state
        /// </summary>
        public bool IsLeftGripPressed()
        {
            return leftGripPressed;
        }
        
        /// <summary>
        /// Get right grip state
        /// </summary>
        public bool IsRightGripPressed()
        {
            return rightGripPressed;
        }
        
        /// <summary>
        /// Get left thumbstick value
        /// </summary>
        public Vector2 GetLeftThumbstick()
        {
            return leftThumbstick;
        }
        
        /// <summary>
        /// Get right thumbstick value
        /// </summary>
        public Vector2 GetRightThumbstick()
        {
            return rightThumbstick;
        }
        
        /// <summary>
        /// Get left hand gesture
        /// </summary>
        public HandGesture GetLeftHandGesture()
        {
            return leftHandGesture;
        }
        
        /// <summary>
        /// Get right hand gesture
        /// </summary>
        public HandGesture GetRightHandGesture()
        {
            return rightHandGesture;
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
        
        /// <summary>
        /// Check if any trigger is pressed
        /// </summary>
        public bool IsAnyTriggerPressed()
        {
            return leftTriggerPressed || rightTriggerPressed;
        }
        
        /// <summary>
        /// Check if any grip is pressed
        /// </summary>
        public bool IsAnyGripPressed()
        {
            return leftGripPressed || rightGripPressed;
        }
        
        /// <summary>
        /// Toggle input features
        /// </summary>
        public void ToggleInputFeatures(bool enable)
        {
            enableControllerInput = enable;
            enableHandTracking = enable;
            enableGestureRecognition = enable;
            
            Debug.Log($"[VRInputManager] Input features {(enable ? "enabled" : "disabled")}");
        }
        
        private void OnDestroy()
        {
            // Clean up event listeners
            InputDevices.deviceConnected -= OnDeviceConnected;
            InputDevices.deviceDisconnected -= OnDeviceDisconnected;
        }
    }
    
    /// <summary>
    /// Hand gesture types
    /// </summary>
    public enum HandGesture
    {
        OpenHand,
        Fist,
        Point,
        Grab,
        ThumbsUp,
        Wave
    }
    
    /// <summary>
    /// Hand side
    /// </summary>
    public enum HandSide
    {
        Left,
        Right,
        Both
    }
    
    /// <summary>
    /// Input device state
    /// </summary>
    public class InputDeviceState
    {
        public bool isConnected = false;
        public Vector3 position = Vector3.zero;
        public Quaternion rotation = Quaternion.identity;
        public bool isTracked = false;
    }
}
