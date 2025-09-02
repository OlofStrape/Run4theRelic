using UnityEngine;
using UnityEngine.Events;

namespace Run4theRelic.Puzzles.RunicSequence
{
    /// <summary>
    /// Represents a runic pad that can be pressed in the sequence puzzle.
    /// Each pad has a unique ID and can trigger press events.
    /// </summary>
    public class RunicPad : MonoBehaviour
    {
        [Header("Pad Settings")]
        [SerializeField] private int padId = 0;
        [SerializeField] private Color padColor = Color.blue;
        [SerializeField] private float pressAnimationDuration = 0.2f;
        
        [Header("Visual")]
        [SerializeField] private Renderer padRenderer;
        [SerializeField] private Material normalMaterial;
        [SerializeField] private Material pressedMaterial;
        [SerializeField] private Material highlightedMaterial;
        
        [Header("Events")]
        [SerializeField] private UnityEvent<int> onPadPressed;
        
        private bool _isPressed;
        private bool _isHighlighted;
        private Material _currentMaterial;
        private float _pressTimer;
        
        /// <summary>
        /// Unique identifier for this pad.
        /// </summary>
        public int PadId => padId;
        
        /// <summary>
        /// Is this pad currently being pressed.
        /// </summary>
        public bool IsPressed => _isPressed;
        
        /// <summary>
        /// Is this pad currently highlighted (part of current sequence).
        /// </summary>
        public bool IsHighlighted => _isHighlighted;
        
        /// <summary>
        /// Event triggered when this pad is pressed.
        /// </summary>
        public UnityEvent<int> OnPadPressed => onPadPressed;
        
        private void Start()
        {
            // Set initial material
            if (padRenderer != null)
            {
                _currentMaterial = normalMaterial;
                padRenderer.material = _currentMaterial;
                padRenderer.material.color = padColor;
            }
        }
        
        private void Update()
        {
            // Handle press animation
            if (_isPressed)
            {
                _pressTimer -= Time.deltaTime;
                if (_pressTimer <= 0f)
                {
                    _isPressed = false;
                    UpdateVisual();
                }
            }
        }
        
        /// <summary>
        /// Press this pad, triggering the press event.
        /// </summary>
        public void Press()
        {
            if (_isPressed) return;
            
            _isPressed = true;
            _pressTimer = pressAnimationDuration;
            
            // Trigger press event
            onPadPressed?.Invoke(padId);
            
            // Update visual
            UpdateVisual();
            
            Debug.Log($"Runic Pad {padId} pressed");
        }
        
        /// <summary>
        /// Highlight this pad (show it's part of current sequence).
        /// </summary>
        public void Highlight()
        {
            _isHighlighted = true;
            UpdateVisual();
        }
        
        /// <summary>
        /// Remove highlight from this pad.
        /// </summary>
        public void Unhighlight()
        {
            _isHighlighted = false;
            UpdateVisual();
        }
        
        /// <summary>
        /// Reset pad to normal state.
        /// </summary>
        public void Reset()
        {
            _isPressed = false;
            _isHighlighted = false;
            _pressTimer = 0f;
            UpdateVisual();
        }
        
        private void UpdateVisual()
        {
            if (padRenderer == null) return;
            
            Material targetMaterial;
            
            if (_isPressed)
            {
                targetMaterial = pressedMaterial;
            }
            else if (_isHighlighted)
            {
                targetMaterial = highlightedMaterial;
            }
            else
            {
                targetMaterial = normalMaterial;
            }
            
            if (targetMaterial != null && targetMaterial != _currentMaterial)
            {
                _currentMaterial = targetMaterial;
                padRenderer.material = _currentMaterial;
                padRenderer.material.color = padColor;
            }
        }
        
        // Unity Editor support
        private void OnValidate()
        {
            // Ensure padId is non-negative
            padId = Mathf.Max(0, padId);
            
            // Ensure pressAnimationDuration is positive
            pressAnimationDuration = Mathf.Max(0.1f, pressAnimationDuration);
            
            // Update visual in editor
            if (Application.isPlaying && padRenderer != null)
            {
                UpdateVisual();
            }
        }
        
        // Gizmos for easier setup in editor
        private void OnDrawGizmos()
        {
            Gizmos.color = padColor;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.3f);
            
            // Draw pad ID
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.2f, $"Pad {padId}");
            #endif
        }
        
        // Input handling (can be extended for actual input system)
        private void OnMouseDown()
        {
            if (Application.isPlaying)
            {
                Press();
            }
        }
    }
} 