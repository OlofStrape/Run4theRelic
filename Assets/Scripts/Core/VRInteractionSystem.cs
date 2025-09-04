using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

namespace Run4theRelic.Core
{
    /// <summary>
    /// VR Interaction System för Run4theRelic
    /// Hanterar VR-interaktioner som grabbing, pointing och fysisk objektmanipulation
    /// </summary>
    public class VRInteractionSystem : MonoBehaviour
    {
        [Header("VR Interaction Settings")]
        [SerializeField] private bool enableGrabbing = true;
        [SerializeField] private bool enablePointing = true;
        [SerializeField] private bool enableRayInteraction = true;
        [SerializeField] private float grabDistance = 2.0f;
        [SerializeField] private float pointDistance = 5.0f;
        
        [Header("VR Components")]
        [SerializeField] private XRDirectInteractor leftDirectInteractor;
        [SerializeField] private XRDirectInteractor rightDirectInteractor;
        [SerializeField] private XRRayInteractor leftRayInteractor;
        [SerializeField] private XRRayInteractor rightRayInteractor;
        [SerializeField] private XRInteractionManager interactionManager;
        
        [Header("Interaction Feedback")]
        [SerializeField] private bool enableHapticFeedback = true;
        [SerializeField] private bool enableVisualFeedback = true;
        [SerializeField] private float hapticIntensity = 0.8f;
        [SerializeField] private Color highlightColor = Color.yellow;
        
        // Private fields
        private VRManager vrManager;
        private List<XRGrabInteractable> grabInteractables = new List<XRGrabInteractable>();
        private List<GameObject> highlightedObjects = new List<GameObject>();
        private Dictionary<GameObject, Material> originalMaterials = new Dictionary<GameObject, Material>();
        
        // Events
        public static event System.Action<GameObject> OnObjectGrabbed;
        public static event System.Action<GameObject> OnObjectReleased;
        public static event System.Action<GameObject> OnObjectPointed;
        public static event System.Action<Vector3> OnInteractionPoint;
        
        private void Start()
        {
            InitializeVRInteraction();
        }
        
        private void Update()
        {
            UpdateInteractionState();
        }
        
        /// <summary>
        /// Initialize VR-interaktionssystemet
        /// </summary>
        private void InitializeVRInteraction()
        {
            // Find VR Manager
            vrManager = FindObjectOfType<VRManager>();
            if (vrManager == null)
            {
                Debug.LogWarning("[VRInteractionSystem] No VRManager found!");
                return;
            }
            
            // Find interaction manager if not assigned
            if (interactionManager == null)
            {
                interactionManager = FindObjectOfType<XRInteractionManager>();
            }
            
            // Find interactors if not assigned
            FindInteractors();
            
            // Setup event listeners
            SetupEventListeners();
            
            // Find all grab interactables in scene
            FindGrabInteractables();
            
            Debug.Log("[VRInteractionSystem] VR Interaction System initialized");
        }
        
        /// <summary>
        /// Hitta alla interactors
        /// </summary>
        private void FindInteractors()
        {
            if (leftDirectInteractor == null || rightDirectInteractor == null)
            {
                var directInteractors = FindObjectsOfType<XRDirectInteractor>();
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
            }
            
            if (leftRayInteractor == null || rightRayInteractor == null)
            {
                var rayInteractors = FindObjectsOfType<XRRayInteractor>();
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
            }
        }
        
        /// <summary>
        /// Setup event listeners
        /// </summary>
        private void SetupEventListeners()
        {
            // Listen for grab events
            if (leftDirectInteractor != null)
            {
                leftDirectInteractor.selectEntered.AddListener(OnLeftHandGrabbed);
                leftDirectInteractor.selectExited.AddListener(OnLeftHandReleased);
            }
            
            if (rightDirectInteractor != null)
            {
                rightDirectInteractor.selectEntered.AddListener(OnRightHandGrabbed);
                rightDirectInteractor.selectExited.AddListener(OnRightHandReleased);
            }
            
            // Listen for ray interaction events
            if (leftRayInteractor != null)
            {
                leftRayInteractor.hoverEntered.AddListener(OnLeftHandPointed);
            }
            
            if (rightRayInteractor != null)
            {
                rightRayInteractor.hoverEntered.AddListener(OnRightHandPointed);
            }
        }
        
        /// <summary>
        /// Hitta alla grab interactables
        /// </summary>
        private void FindGrabInteractables()
        {
            grabInteractables.Clear();
            var interactables = FindObjectsOfType<XRGrabInteractable>();
            grabInteractables.AddRange(interactables);
            
            Debug.Log($"[VRInteractionSystem] Found {grabInteractables.Count} grab interactables");
        }
        
        /// <summary>
        /// Update interaction state
        /// </summary>
        private void UpdateInteractionState()
        {
            // Update interaction feedback
            UpdateInteractionFeedback();
            
            // Update interaction points
            UpdateInteractionPoints();
        }
        
        /// <summary>
        /// Update interaction feedback
        /// </summary>
        private void UpdateInteractionFeedback()
        {
            if (!enableVisualFeedback) return;
            
            // Update highlighting for objects being pointed at
            UpdateObjectHighlighting();
        }
        
        /// <summary>
        /// Update object highlighting
        /// </summary>
        private void UpdateObjectHighlighting()
        {
            // Clear old highlights
            ClearHighlights();
            
            // Add new highlights for objects being pointed at
            if (leftRayInteractor != null && leftRayInteractor.TryGetCurrent3DRaycastHit(out var leftHit))
            {
                HighlightObject(leftHit.collider.gameObject);
            }
            
            if (rightRayInteractor != null && rightRayInteractor.TryGetCurrent3DRaycastHit(out var rightHit))
            {
                HighlightObject(rightHit.collider.gameObject);
            }
        }
        
        /// <summary>
        /// Highlight object
        /// </summary>
        private void HighlightObject(GameObject obj)
        {
            if (obj == null || highlightedObjects.Contains(obj)) return;
            
            var renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                // Store original material
                if (!originalMaterials.ContainsKey(obj))
                {
                    originalMaterials[obj] = renderer.material;
                }
                
                // Create highlight material
                var highlightMaterial = new Material(renderer.material);
                highlightMaterial.color = highlightColor;
                renderer.material = highlightMaterial;
                
                highlightedObjects.Add(obj);
            }
        }
        
        /// <summary>
        /// Clear all highlights
        /// </summary>
        private void ClearHighlights()
        {
            foreach (var obj in highlightedObjects)
            {
                if (obj != null && originalMaterials.ContainsKey(obj))
                {
                    var renderer = obj.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material = originalMaterials[obj];
                    }
                }
            }
            
            highlightedObjects.Clear();
        }
        
        /// <summary>
        /// Update interaction points
        /// </summary>
        private void UpdateInteractionPoints()
        {
            if (!enableRayInteraction) return;
            
            // Update left hand interaction point
            if (leftRayInteractor != null && leftRayInteractor.TryGetCurrent3DRaycastHit(out var leftHit))
            {
                OnInteractionPoint?.Invoke(leftHit.point);
            }
            
            // Update right hand interaction point
            if (rightRayInteractor != null && rightRayInteractor.TryGetCurrent3DRaycastHit(out var rightHit))
            {
                OnInteractionPoint?.Invoke(rightHit.point);
            }
        }
        
        /// <summary>
        /// Left hand grabbed object
        /// </summary>
        private void OnLeftHandGrabbed(SelectEnterEventArgs args)
        {
            var interactable = args.interactableObject as XRGrabInteractable;
            if (interactable != null)
            {
                OnObjectGrabbed?.Invoke(interactable.gameObject);
                
                if (enableHapticFeedback)
                {
                    SendHapticFeedback(XRNode.LeftHand);
                }
                
                Debug.Log($"[VRInteractionSystem] Left hand grabbed: {interactable.name}");
            }
        }
        
        /// <summary>
        /// Left hand released object
        /// </summary>
        private void OnLeftHandReleased(SelectExitEventArgs args)
        {
            var interactable = args.interactableObject as XRGrabInteractable;
            if (interactable != null)
            {
                OnObjectReleased?.Invoke(interactable.gameObject);
                
                if (enableHapticFeedback)
                {
                    SendHapticFeedback(XRNode.LeftHand, 0.5f, 0.1f);
                }
                
                Debug.Log($"[VRInteractionSystem] Left hand released: {interactable.name}");
            }
        }
        
        /// <summary>
        /// Right hand grabbed object
        /// </summary>
        private void OnRightHandGrabbed(SelectEnterEventArgs args)
        {
            var interactable = args.interactableObject as XRGrabInteractable;
            if (interactable != null)
            {
                OnObjectGrabbed?.Invoke(interactable.gameObject);
                
                if (enableHapticFeedback)
                {
                    SendHapticFeedback(XRNode.RightHand);
                }
                
                Debug.Log($"[VRInteractionSystem] Right hand grabbed: {interactable.name}");
            }
        }
        
        /// <summary>
        /// Right hand released object
        /// </summary>
        private void OnRightHandReleased(SelectExitEventArgs args)
        {
            var interactable = args.interactableObject as XRGrabInteractable;
            if (interactable != null)
            {
                OnObjectReleased?.Invoke(interactable.gameObject);
                
                if (enableHapticFeedback)
                {
                    SendHapticFeedback(XRNode.RightHand, 0.5f, 0.1f);
                }
                
                Debug.Log($"[VRInteractionSystem] Right hand released: {interactable.name}");
            }
        }
        
        /// <summary>
        /// Left hand pointed at object
        /// </summary>
        private void OnLeftHandPointed(HoverEnterEventArgs args)
        {
            var interactable = args.interactableObject as XRGrabInteractable;
            if (interactable != null)
            {
                OnObjectPointed?.Invoke(interactable.gameObject);
                
                if (enableHapticFeedback)
                {
                    SendHapticFeedback(XRNode.LeftHand, 0.3f, 0.05f);
                }
            }
        }
        
        /// <summary>
        /// Right hand pointed at object
        /// </summary>
        private void OnRightHandPointed(HoverEnterEventArgs args)
        {
            var interactable = args.interactableObject as XRGrabInteractable;
            if (interactable != null)
            {
                OnObjectPointed?.Invoke(interactable.gameObject);
                
                if (enableHapticFeedback)
                {
                    SendHapticFeedback(XRNode.RightHand, 0.3f, 0.05f);
                }
            }
        }
        
        /// <summary>
        /// Send haptic feedback
        /// </summary>
        private void SendHapticFeedback(XRNode controllerNode, float intensity = 1.0f, float duration = 0.1f)
        {
            if (!enableHapticFeedback || vrManager == null) return;
            
            vrManager.SendHapticFeedback(controllerNode, intensity * hapticIntensity, duration);
        }
        
        /// <summary>
        /// Add grab interactable
        /// </summary>
        public void AddGrabInteractable(XRGrabInteractable interactable)
        {
            if (interactable != null && !grabInteractables.Contains(interactable))
            {
                grabInteractables.Add(interactable);
                Debug.Log($"[VRInteractionSystem] Added grab interactable: {interactable.name}");
            }
        }
        
        /// <summary>
        /// Remove grab interactable
        /// </summary>
        public void RemoveGrabInteractable(XRGrabInteractable interactable)
        {
            if (interactable != null && grabInteractables.Contains(interactable))
            {
                grabInteractables.Remove(interactable);
                Debug.Log($"[VRInteractionSystem] Removed grab interactable: {interactable.name}");
            }
        }
        
        /// <summary>
        /// Get all grab interactables
        /// </summary>
        public List<XRGrabInteractable> GetGrabInteractables()
        {
            return new List<XRGrabInteractable>(grabInteractables);
        }
        
        /// <summary>
        /// Check if object is being grabbed
        /// </summary>
        public bool IsObjectGrabbed(GameObject obj)
        {
            if (obj == null) return false;
            
            var interactable = obj.GetComponent<XRGrabInteractable>();
            if (interactable != null)
            {
                return interactable.isSelected;
            }
            
            return false;
        }
        
        /// <summary>
        /// Get object being grabbed by hand
        /// </summary>
        public GameObject GetGrabbedObject(XRNode handNode)
        {
            XRDirectInteractor interactor = null;
            
            if (handNode == XRNode.LeftHand)
            {
                interactor = leftDirectInteractor;
            }
            else if (handNode == XRNode.RightHand)
            {
                interactor = rightDirectInteractor;
            }
            
            if (interactor != null && interactor.hasSelection)
            {
                var interactable = interactor.interactablesSelected[0] as XRGrabInteractable;
                return interactable?.gameObject;
            }
            
            return null;
        }
        
        /// <summary>
        /// Toggle interaction features
        /// </summary>
        public void ToggleInteractionFeatures(bool enable)
        {
            enableGrabbing = enable;
            enablePointing = enable;
            enableRayInteraction = enable;
            enableHapticFeedback = enable;
            enableVisualFeedback = enable;
            
            Debug.Log($"[VRInteractionSystem] Interaction features {(enable ? "enabled" : "disabled")}");
        }
        
        /// <summary>
        /// Get left direct interactor
        /// </summary>
        public XRDirectInteractor GetLeftDirectInteractor()
        {
            return leftDirectInteractor;
        }
        
        /// <summary>
        /// Get right direct interactor
        /// </summary>
        public XRDirectInteractor GetRightDirectInteractor()
        {
            return rightDirectInteractor;
        }
        
        /// <summary>
        /// Get left ray interactor
        /// </summary>
        public XRRayInteractor GetLeftRayInteractor()
        {
            return leftRayInteractor;
        }
        
        /// <summary>
        /// Get right ray interactor
        /// </summary>
        public XRRayInteractor GetRightRayInteractor()
        {
            return rightRayInteractor;
        }
        
        /// <summary>
        /// Get interaction manager
        /// </summary>
        public XRInteractionManager GetInteractionManager()
        {
            return interactionManager;
        }
        
        private void OnDestroy()
        {
            // Clean up highlights
            ClearHighlights();
            
            // Clean up materials
            foreach (var material in originalMaterials.Values)
            {
                if (material != null)
                {
                    DestroyImmediate(material);
                }
            }
            originalMaterials.Clear();
        }
    }
}
