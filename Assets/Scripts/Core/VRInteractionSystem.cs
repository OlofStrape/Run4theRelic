using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

namespace Run4theRelic.Core
{
    /// <summary>
    /// Handles VR interactions including grabbing, pointing, and physical object manipulation.
    /// Integrates with XR Interaction Toolkit for seamless VR interaction.
    /// </summary>
    public class VRInteractionSystem : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] private bool enableGrabbing = true;
        [SerializeField] private bool enablePointing = true;
        [SerializeField] private bool enableRaycasting = true;
        [SerializeField] private LayerMask interactableLayers = -1;
        
        [Header("Grabbing Settings")]
        [SerializeField] private float grabThreshold = 0.8f;
        [SerializeField] private float releaseThreshold = 0.2f;
        [SerializeField] private bool enableHapticFeedback = true;
        [SerializeField] private float grabHapticIntensity = 0.7f;
        
        [Header("Pointing Settings")]
        [SerializeField] private float maxPointDistance = 10f;
        [SerializeField] private LayerMask pointableLayers = -1;
        [SerializeField] private bool showPointingLine = true;
        [SerializeField] private Material pointingLineMaterial;
        
        [Header("References")]
        [SerializeField] private XRDirectInteractor leftDirectInteractor;
        [SerializeField] private XRDirectInteractor rightDirectInteractor;
        [SerializeField] private XRRayInteractor leftRayInteractor;
        [SerializeField] private XRRayInteractor rightRayInteractor;
        [SerializeField] private LineRenderer leftPointingLine;
        [SerializeField] private LineRenderer rightPointingLine;
        
        // Interaction State
        private bool _leftHandGrabbing = false;
        private bool _rightHandGrabbing = false;
        private GameObject _leftGrabbedObject = null;
        private GameObject _rightGrabbedObject = null;
        private Vector3 _leftGrabPoint = Vector3.zero;
        private Vector3 _rightGrabPoint = Vector3.zero;
        
        // Pointing State
        private bool _leftHandPointing = false;
        private bool _rightHandPointing = false;
        private Vector3 _leftPointTarget = Vector3.zero;
        private Vector3 _rightPointTarget = Vector3.zero;
        private GameObject _leftPointedObject = null;
        private GameObject _rightPointedObject = null;
        
        // Events
        public static event System.Action<GameObject, bool> OnLeftHandGrabbingChanged;
        public static event System.Action<GameObject, bool> OnRightHandGrabbingChanged;
        public static event System.Action<GameObject, Vector3> OnLeftHandPointingChanged;
        public static event System.Action<GameObject, Vector3> OnRightHandPointingChanged;
        
        // Properties
        public bool LeftHandGrabbing => _leftHandGrabbing;
        public bool RightHandGrabbing => _rightHandGrabbing;
        public GameObject LeftGrabbedObject => _leftGrabbedObject;
        public GameObject RightGrabbedObject => _rightGrabbedObject;
        public Vector3 LeftGrabPoint => _leftGrabPoint;
        public Vector3 RightGrabPoint => _rightGrabPoint;
        
        public bool LeftHandPointing => _leftHandPointing;
        public bool RightHandPointing => _rightHandPointing;
        public Vector3 LeftPointTarget => _leftPointTarget;
        public Vector3 RightPointTarget => _rightPointTarget;
        public GameObject LeftPointedObject => _leftPointedObject;
        public GameObject RightPointedObject => _rightPointedObject;
        
        private void Start()
        {
            // Find interactors if not assigned
            FindInteractors();
            
            // Setup pointing lines
            SetupPointingLines();
            
            // Subscribe to input events
            VRInputManager.OnLeftGripChanged += OnLeftGripChanged;
            VRInputManager.OnRightGripChanged += OnRightGripChanged;
            VRInputManager.OnLeftHandGestureChanged += OnLeftHandGestureChanged;
            VRInputManager.OnRightHandGestureChanged += OnRightHandGestureChanged;
        }
        
        private void Update()
        {
            if (enablePointing)
            {
                UpdatePointing();
            }
        }
        
        /// <summary>
        /// Find and assign VR interactors.
        /// </summary>
        private void FindInteractors()
        {
            var directInteractors = FindObjectsOfType<XRDirectInteractor>();
            var rayInteractors = FindObjectsOfType<XRRayInteractor>();
            
            foreach (var interactor in directInteractors)
            {
                if (interactor.interactionLayers == InteractionLayerMask.GetMask("LeftHand"))
                {
                    leftDirectInteractor = interactor;
                }
                else if (interactor.interactionLayers == InteractionLayerMask.GetMask("RightHand"))
                {
                    rightDirectInteractor = interactor;
                }
            }
            
            foreach (var interactor in rayInteractors)
            {
                if (interactor.interactionLayers == InteractionLayerMask.GetMask("LeftHand"))
                {
                    leftRayInteractor = interactor;
                }
                else if (interactor.interactionLayers == InteractionLayerMask.GetMask("RightHand"))
                {
                    rightRayInteractor = interactor;
                }
            }
            
            if (leftDirectInteractor == null || rightDirectInteractor == null)
            {
                Debug.LogWarning("VRInteractionSystem: Could not find both direct interactors!");
            }
        }
        
        /// <summary>
        /// Setup pointing line renderers.
        /// </summary>
        private void SetupPointingLines()
        {
            if (showPointingLine)
            {
                // Create left pointing line
                if (leftPointingLine == null)
                {
                    leftPointingLine = CreatePointingLine("LeftPointingLine");
                }
                
                // Create right pointing line
                if (rightPointingLine == null)
                {
                    rightPointingLine = CreatePointingLine("RightPointingLine");
                }
            }
        }
        
        /// <summary>
        /// Create a pointing line renderer.
        /// </summary>
        /// <param name="name">Name for the line renderer.</param>
        /// <returns>Configured LineRenderer component.</returns>
        private LineRenderer CreatePointingLine(string name)
        {
            GameObject lineObj = new GameObject(name);
            lineObj.transform.SetParent(transform);
            
            LineRenderer line = lineObj.AddComponent<LineRenderer>();
            line.material = pointingLineMaterial != null ? pointingLineMaterial : new Material(Shader.Find("Sprites/Default"));
            line.startWidth = 0.01f;
            line.endWidth = 0.005f;
            line.color = Color.cyan;
            line.positionCount = 2;
            line.useWorldSpace = true;
            line.enabled = false;
            
            return line;
        }
        
        /// <summary>
        /// Handle left grip input changes.
        /// </summary>
        /// <param name="isPressed">True if grip is pressed.</param>
        private void OnLeftGripChanged(bool isPressed)
        {
            if (enableGrabbing)
            {
                if (isPressed && !_leftHandGrabbing)
                {
                    StartLeftHandGrabbing();
                }
                else if (!isPressed && _leftHandGrabbing)
                {
                    StopLeftHandGrabbing();
                }
            }
        }
        
        /// <summary>
        /// Handle right grip input changes.
        /// </summary>
        /// <param name="isPressed">True if grip is pressed.</param>
        private void OnRightGripChanged(bool isPressed)
        {
            if (enableGrabbing)
            {
                if (isPressed && !_rightHandGrabbing)
                {
                    StartRightHandGrabbing();
                }
                else if (!isPressed && _rightHandGrabbing)
                {
                    StopRightHandGrabbing();
                }
            }
        }
        
        /// <summary>
        /// Handle left hand gesture changes.
        /// </summary>
        /// <param name="isClosed">True if hand is closed.</param>
        private void OnLeftHandGestureChanged(bool isClosed)
        {
            _leftHandPointing = !isClosed;
            UpdateLeftPointingLine();
        }
        
        /// <summary>
        /// Handle right hand gesture changes.
        /// </summary>
        /// <param name="isClosed">True if hand is closed.</param>
        private void OnRightHandGestureChanged(bool isClosed)
        {
            _rightHandPointing = !isClosed;
            UpdateRightPointingLine();
        }
        
        /// <summary>
        /// Start grabbing with left hand.
        /// </summary>
        private void StartLeftHandGrabbing()
        {
            if (leftDirectInteractor == null) return;
            
            // Find closest interactable
            var interactables = leftDirectInteractor.interactablesSelected;
            if (interactables.Count > 0)
            {
                var interactable = interactables[0];
                _leftGrabbedObject = interactable.transform.gameObject;
                _leftGrabPoint = leftDirectInteractor.transform.position;
                
                _leftHandGrabbing = true;
                
                // Trigger haptic feedback
                if (enableHapticFeedback)
                {
                    var vrManager = FindObjectOfType<VRManager>();
                    if (vrManager != null)
                    {
                        var controller = vrManager.GetLeftController();
                        if (controller != null)
                        {
                            vrManager.TriggerHapticFeedback(controller, grabHapticIntensity);
                        }
                    }
                }
                
                OnLeftHandGrabbingChanged?.Invoke(_leftGrabbedObject, true);
                
                Debug.Log($"Left hand started grabbing: {_leftGrabbedObject.name}");
            }
        }
        
        /// <summary>
        /// Stop grabbing with left hand.
        /// </summary>
        private void StopLeftHandGrabbing()
        {
            if (_leftHandGrabbing)
            {
                _leftHandGrabbing = false;
                var grabbedObject = _leftGrabbedObject;
                _leftGrabbedObject = null;
                _leftGrabPoint = Vector3.zero;
                
                OnLeftHandGrabbingChanged?.Invoke(grabbedObject, false);
                
                Debug.Log($"Left hand stopped grabbing: {grabbedObject?.name ?? "unknown"}");
            }
        }
        
        /// <summary>
        /// Start grabbing with right hand.
        /// </summary>
        private void StartRightHandGrabbing()
        {
            if (rightDirectInteractor == null) return;
            
            // Find closest interactable
            var interactables = rightDirectInteractor.interactablesSelected;
            if (interactables.Count > 0)
            {
                var interactable = interactables[0];
                _rightGrabbedObject = interactable.transform.gameObject;
                _rightGrabPoint = rightDirectInteractor.transform.position;
                
                _rightHandGrabbing = true;
                
                // Trigger haptic feedback
                if (enableHapticFeedback)
                {
                    var vrManager = FindObjectOfType<VRManager>();
                    if (vrManager != null)
                    {
                        var controller = vrManager.GetRightController();
                        if (controller != null)
                        {
                            vrManager.TriggerHapticFeedback(controller, grabHapticIntensity);
                        }
                    }
                }
                
                OnRightHandGrabbingChanged?.Invoke(_rightGrabbedObject, true);
                
                Debug.Log($"Right hand started grabbing: {_rightGrabbedObject.name}");
            }
        }
        
        /// <summary>
        /// Stop grabbing with right hand.
        /// </summary>
        private void StopRightHandGrabbing()
        {
            if (_rightHandGrabbing)
            {
                _rightHandGrabbing = false;
                var grabbedObject = _rightGrabbedObject;
                _rightGrabbedObject = null;
                _rightGrabPoint = Vector3.zero;
                
                OnRightHandGrabbingChanged?.Invoke(grabbedObject, false);
                
                Debug.Log($"Right hand stopped grabbing: {grabbedObject?.name ?? "unknown"}");
            }
        }
        
        /// <summary>
        /// Update pointing system.
        /// </summary>
        private void UpdatePointing()
        {
            if (_leftHandPointing)
            {
                UpdateLeftPointing();
            }
            
            if (_rightHandPointing)
            {
                UpdateRightPointing();
            }
        }
        
        /// <summary>
        /// Update left hand pointing.
        /// </summary>
        private void UpdateLeftPointing()
        {
            if (leftRayInteractor == null) return;
            
            // Get pointing target
            if (leftRayInteractor.TryGetCurrent3DRaycastHit(out var hit))
            {
                _leftPointTarget = hit.point;
                _leftPointedObject = hit.collider.gameObject;
                
                OnLeftHandPointingChanged?.Invoke(_leftPointedObject, _leftPointTarget);
            }
            else
            {
                _leftPointTarget = leftRayInteractor.transform.position + leftRayInteractor.transform.forward * maxPointDistance;
                _leftPointedObject = null;
            }
            
            UpdateLeftPointingLine();
        }
        
        /// <summary>
        /// Update right hand pointing.
        /// </summary>
        private void UpdateRightPointing()
        {
            if (rightRayInteractor == null) return;
            
            // Get pointing target
            if (rightRayInteractor.TryGetCurrent3DRaycastHit(out var hit))
            {
                _rightPointTarget = hit.point;
                _rightPointedObject = hit.collider.gameObject;
                
                OnRightHandPointingChanged?.Invoke(_rightPointedObject, _rightPointTarget);
            }
            else
            {
                _rightPointTarget = rightRayInteractor.transform.position + rightRayInteractor.transform.forward * maxPointDistance;
                _rightPointedObject = null;
            }
            
            UpdateRightPointingLine();
        }
        
        /// <summary>
        /// Update left pointing line.
        /// </summary>
        private void UpdateLeftPointingLine()
        {
            if (leftPointingLine == null || !showPointingLine) return;
            
            if (_leftHandPointing)
            {
                leftPointingLine.enabled = true;
                leftPointingLine.SetPosition(0, leftRayInteractor?.transform.position ?? Vector3.zero);
                leftPointingLine.SetPosition(1, _leftPointTarget);
            }
            else
            {
                leftPointingLine.enabled = false;
            }
        }
        
        /// <summary>
        /// Update right pointing line.
        /// </summary>
        private void UpdateRightPointingLine()
        {
            if (rightPointingLine == null || !showPointingLine) return;
            
            if (_rightHandPointing)
            {
                rightPointingLine.enabled = true;
                rightPointingLine.SetPosition(0, rightRayInteractor?.transform.position ?? Vector3.zero);
                rightPointingLine.SetPosition(1, _rightPointTarget);
            }
            else
            {
                rightPointingLine.enabled = false;
            }
        }
        
        /// <summary>
        /// Check if any hand is grabbing.
        /// </summary>
        /// <returns>True if any hand is grabbing.</returns>
        public bool IsAnyHandGrabbing()
        {
            return _leftHandGrabbing || _rightHandGrabbing;
        }
        
        /// <summary>
        /// Check if any hand is pointing.
        /// </summary>
        /// <returns>True if any hand is pointing.</returns>
        public bool IsAnyHandPointing()
        {
            return _leftHandPointing || _rightHandPointing;
        }
        
        /// <summary>
        /// Get grabbed object for a specific hand.
        /// </summary>
        /// <param name="isLeft">True for left hand, false for right.</param>
        /// <returns>Grabbed GameObject or null.</returns>
        public GameObject GetGrabbedObject(bool isLeft)
        {
            return isLeft ? _leftGrabbedObject : _rightGrabbedObject;
        }
        
        /// <summary>
        /// Get pointed object for a specific hand.
        /// </summary>
        /// <param name="isLeft">True for left hand, false for right.</param>
        /// <returns>Pointed GameObject or null.</returns>
        public GameObject GetPointedObject(bool isLeft)
        {
            return isLeft ? _leftPointedObject : _rightPointedObject;
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            VRInputManager.OnLeftGripChanged -= OnLeftGripChanged;
            VRInputManager.OnRightGripChanged -= OnRightGripChanged;
            VRInputManager.OnLeftHandGestureChanged -= OnLeftHandGestureChanged;
            VRInputManager.OnRightHandGestureChanged -= OnRightHandGestureChanged;
        }
    }
}
