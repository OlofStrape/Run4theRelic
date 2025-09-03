using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace Run4theRelic.Core
{
    /// <summary>
    /// Manages all VR input including controller buttons, hand gestures, and input events.
    /// Provides a unified interface for VR input across different devices.
    /// </summary>
    public class VRInputManager : MonoBehaviour
    {
        [Header("Input Settings")]
        [SerializeField] private bool enableControllerInput = true;
        [SerializeField] private bool enableHandGestures = true;
        [SerializeField] private float gestureThreshold = 0.8f;
        
        [Header("Controller References")]
        [SerializeField] private XRController leftController;
        [SerializeField] private XRController rightController;
        
        [Header("Input Events")]
        [SerializeField] private bool enableInputEvents = true;
        
        // Input States
        private bool _leftTriggerPressed = false;
        private bool _rightTriggerPressed = false;
        private bool _leftGripPressed = false;
        private bool _rightGripPressed = false;
        private bool _leftPrimaryButtonPressed = false;
        private bool _rightPrimaryButtonPressed = false;
        private bool _leftSecondaryButtonPressed = false;
        private bool _rightSecondaryButtonPressed = false;
        
        // Hand Gestures
        private bool _leftHandClosed = false;
        private bool _rightHandClosed = false;
        private bool _leftHandPointing = false;
        private bool _rightHandPointing = false;
        
        // Input Values
        private float _leftTriggerValue = 0f;
        private float _rightTriggerValue = 0f;
        private float _leftGripValue = 0f;
        private float _rightGripValue = 0f;
        private Vector2 _leftThumbstickValue = Vector2.zero;
        private Vector2 _rightThumbstickValue = Vector2.zero;
        
        // Events
        public static event System.Action<bool> OnLeftTriggerChanged;
        public static event System.Action<bool> OnRightTriggerChanged;
        public static event System.Action<bool> OnLeftGripChanged;
        public static event System.Action<bool> OnRightGripChanged;
        public static event System.Action<bool> OnLeftPrimaryButtonChanged;
        public static event System.Action<bool> OnRightPrimaryButtonChanged;
        public static event System.Action<bool> OnLeftSecondaryButtonChanged;
        public static event System.Action<bool> OnRightSecondaryButtonChanged;
        public static event System.Action<bool> OnLeftHandGestureChanged;
        public static event System.Action<bool> OnRightHandGestureChanged;
        
        // Properties
        public bool LeftTriggerPressed => _leftTriggerPressed;
        public bool RightTriggerPressed => _rightTriggerPressed;
        public bool LeftGripPressed => _leftGripPressed;
        public bool RightGripPressed => _rightGripPressed;
        public bool LeftPrimaryButtonPressed => _leftPrimaryButtonPressed;
        public bool RightPrimaryButtonPressed => _rightPrimaryButtonPressed;
        public bool LeftSecondaryButtonPressed => _leftSecondaryButtonPressed;
        public bool RightSecondaryButtonPressed => _rightSecondaryButtonPressed;
        public bool LeftHandClosed => _leftHandClosed;
        public bool RightHandClosed => _rightHandClosed;
        public bool LeftHandPointing => _leftHandPointing;
        public bool RightHandPointing => _rightHandPointing;
        
        public float LeftTriggerValue => _leftTriggerValue;
        public float RightTriggerValue => _rightTriggerValue;
        public float LeftGripValue => _leftGripValue;
        public float RightGripValue => _rightGripValue;
        public Vector2 LeftThumbstickValue => _leftThumbstickValue;
        public Vector2 RightThumbstickValue => _rightThumbstickValue;
        
        private void Start()
        {
            // Find controllers if not assigned
            if (leftController == null || rightController == null)
            {
                FindControllers();
            }
            
            // Subscribe to VR manager events
            VRManager.OnVRModeChanged += OnVRModeChanged;
        }
        
        private void Update()
        {
            if (!enableControllerInput) return;
            
            // Update controller input
            UpdateControllerInput();
            
            // Update hand gestures
            if (enableHandGestures)
            {
                UpdateHandGestures();
            }
        }
        
        /// <summary>
        /// Find and assign VR controllers.
        /// </summary>
        private void FindControllers()
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
            
            if (leftController == null || rightController == null)
            {
                Debug.LogWarning("VRInputManager: Could not find both controllers!");
            }
        }
        
        /// <summary>
        /// Update controller input states and values.
        /// </summary>
        private void UpdateControllerInput()
        {
            if (leftController != null && leftController.inputDevice.isValid)
            {
                UpdateLeftControllerInput();
            }
            
            if (rightController != null && rightController.inputDevice.isValid)
            {
                UpdateRightControllerInput();
            }
        }
        
        /// <summary>
        /// Update left controller input.
        /// </summary>
        private void UpdateLeftControllerInput()
        {
            // Trigger
            bool newLeftTriggerPressed = _leftTriggerValue > gestureThreshold;
            if (newLeftTriggerPressed != _leftTriggerPressed)
            {
                _leftTriggerPressed = newLeftTriggerPressed;
                OnLeftTriggerChanged?.Invoke(_leftTriggerPressed);
            }
            
            // Grip
            bool newLeftGripPressed = _leftGripValue > gestureThreshold;
            if (newLeftGripPressed != _leftGripPressed)
            {
                _leftGripPressed = newLeftGripPressed;
                OnLeftGripChanged?.Invoke(_leftGripPressed);
            }
            
            // Primary button (A/X)
            bool newLeftPrimaryPressed = false;
            if (leftController.inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out newLeftPrimaryPressed))
            {
                if (newLeftPrimaryPressed != _leftPrimaryButtonPressed)
                {
                    _leftPrimaryButtonPressed = newLeftPrimaryPressed;
                    OnLeftPrimaryButtonChanged?.Invoke(_leftPrimaryButtonPressed);
                }
            }
            
            // Secondary button (B/Y)
            bool newLeftSecondaryPressed = false;
            if (leftController.inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out newLeftSecondaryPressed))
            {
                if (newLeftSecondaryPressed != _leftSecondaryButtonPressed)
                {
                    _leftSecondaryButtonPressed = newLeftSecondaryPressed;
                    OnLeftSecondaryButtonChanged?.Invoke(_leftSecondaryButtonPressed);
                }
            }
            
            // Thumbstick
            if (leftController.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out _leftThumbstickValue))
            {
                // Thumbstick value updated
            }
        }
        
        /// <summary>
        /// Update right controller input.
        /// </summary>
        private void UpdateRightControllerInput()
        {
            // Trigger
            bool newRightTriggerPressed = _rightTriggerValue > gestureThreshold;
            if (newRightTriggerPressed != _rightTriggerPressed)
            {
                _rightTriggerPressed = newRightTriggerPressed;
                OnRightTriggerChanged?.Invoke(_rightTriggerPressed);
            }
            
            // Grip
            bool newRightGripPressed = _rightGripValue > gestureThreshold;
            if (newRightGripPressed != _rightGripPressed)
            {
                _rightGripPressed = newRightGripPressed;
                OnRightGripChanged?.Invoke(_rightGripPressed);
            }
            
            // Primary button (A/X)
            bool newRightPrimaryPressed = false;
            if (rightController.inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out newRightPrimaryPressed))
            {
                if (newRightPrimaryPressed != _rightPrimaryButtonPressed)
                {
                    _rightPrimaryButtonPressed = newRightPrimaryPressed;
                    OnRightPrimaryButtonChanged?.Invoke(_rightPrimaryButtonPressed);
                }
            }
            
            // Secondary button (B/Y)
            bool newRightSecondaryPressed = false;
            if (rightController.inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out newRightSecondaryPressed))
            {
                if (newRightSecondaryPressed != _rightSecondaryButtonPressed)
                {
                    _rightSecondaryButtonPressed = newRightSecondaryPressed;
                    OnRightSecondaryButtonChanged?.Invoke(_rightSecondaryButtonPressed);
                }
            }
            
            // Thumbstick
            if (rightController.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out _rightThumbstickValue))
            {
                // Thumbstick value updated
            }
        }
        
        /// <summary>
        /// Update hand gesture states.
        /// </summary>
        private void UpdateHandGestures()
        {
            // This would integrate with hand tracking system
            // For now, we'll use grip values as a proxy for hand gestures
            
            // Left hand
            bool newLeftHandClosed = _leftGripValue > gestureThreshold;
            if (newLeftHandClosed != _leftHandClosed)
            {
                _leftHandClosed = newLeftHandClosed;
                OnLeftHandGestureChanged?.Invoke(_leftHandClosed);
            }
            
            // Right hand
            bool newRightHandClosed = _rightGripValue > gestureThreshold;
            if (newRightHandClosed != _rightHandClosed)
            {
                _rightHandClosed = newRightHandClosed;
                OnRightHandGestureChanged?.Invoke(_rightHandClosed);
            }
        }
        
        /// <summary>
        /// Get trigger value for a specific controller.
        /// </summary>
        /// <param name="isLeft">True for left controller, false for right.</param>
        /// <returns>Trigger value (0-1).</returns>
        public float GetTriggerValue(bool isLeft)
        {
            return isLeft ? _leftTriggerValue : _rightTriggerValue;
        }
        
        /// <summary>
        /// Get grip value for a specific controller.
        /// </summary>
        /// <param name="isLeft">True for left controller, false for right.</param>
        /// <returns>Grip value (0-1).</returns>
        public float GetGripValue(bool isLeft)
        {
            return isLeft ? _leftGripValue : _rightGripValue;
        }
        
        /// <summary>
        /// Get thumbstick value for a specific controller.
        /// </summary>
        /// <param name="isLeft">True for left controller, false for right.</param>
        /// <returns>Thumbstick value as Vector2.</returns>
        public Vector2 GetThumbstickValue(bool isLeft)
        {
            return isLeft ? _leftThumbstickValue : _rightThumbstickValue;
        }
        
        /// <summary>
        /// Check if any trigger is pressed.
        /// </summary>
        /// <returns>True if any trigger is pressed.</returns>
        public bool IsAnyTriggerPressed()
        {
            return _leftTriggerPressed || _rightTriggerPressed;
        }
        
        /// <summary>
        /// Check if any grip is pressed.
        /// </summary>
        /// <returns>True if any grip is pressed.</returns>
        public bool IsAnyGripPressed()
        {
            return _leftGripPressed || _rightGripPressed;
        }
        
        /// <summary>
        /// Check if any primary button is pressed.
        /// </summary>
        /// <returns>True if any primary button is pressed.</returns>
        public bool IsAnyPrimaryButtonPressed()
        {
            return _leftPrimaryButtonPressed || _rightPrimaryButtonPressed;
        }
        
        /// <summary>
        /// Check if any secondary button is pressed.
        /// </summary>
        /// <returns>True if any secondary button is pressed.</returns>
        public bool IsAnySecondaryButtonPressed()
        {
            return _leftSecondaryButtonPressed || _rightSecondaryButtonPressed;
        }
        
        /// <summary>
        /// Handle VR mode changes.
        /// </summary>
        /// <param name="isVRMode">True if VR mode is active.</param>
        private void OnVRModeChanged(bool isVRMode)
        {
            if (isVRMode)
            {
                FindControllers();
            }
            else
            {
                // Disable VR input when not in VR mode
                enableControllerInput = false;
            }
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            VRManager.OnVRModeChanged -= OnVRModeChanged;
        }
    }
}
