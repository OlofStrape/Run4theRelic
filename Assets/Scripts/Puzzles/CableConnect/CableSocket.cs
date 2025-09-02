using UnityEngine;

namespace Run4theRelic.Puzzles.CableConnect
{
    /// <summary>
    /// Represents a cable socket that can accept plugs with matching colorId.
    /// Only one plug can be connected to a socket at a time.
    /// </summary>
    public class CableSocket : MonoBehaviour
    {
        [Header("Socket Settings")]
        [SerializeField] private int acceptsColorId = 0;
        [SerializeField] private Color socketColor = Color.red;
        [SerializeField] private bool isRequired = true;
        
        [Header("Visual")]
        [SerializeField] private Renderer socketRenderer;
        [SerializeField] private Material emptyMaterial;
        [SerializeField] private Material occupiedMaterial;
        [SerializeField] private Material requiredMaterial;
        
        private CablePlug _connectedPlug;
        private bool _isOccupied;
        
        /// <summary>
        /// The color ID that this socket accepts.
        /// </summary>
        public int AcceptsColorId => acceptsColorId;
        
        /// <summary>
        /// Is this socket currently occupied by a plug.
        /// </summary>
        public bool IsOccupied => _isOccupied;
        
        /// <summary>
        /// The plug currently connected to this socket, or null if empty.
        /// </summary>
        public CablePlug ConnectedPlug => _connectedPlug;
        
        /// <summary>
        /// Is this socket required for puzzle completion.
        /// </summary>
        public bool IsRequired => isRequired;
        
        private void Start()
        {
            UpdateVisual();
        }
        
        /// <summary>
        /// Called when a plug connects to this socket.
        /// </summary>
        /// <param name="plug">The plug that connected.</param>
        public void OnPlugConnected(CablePlug plug)
        {
            if (_isOccupied)
            {
                Debug.LogWarning($"Socket {acceptsColorId} is already occupied!");
                return;
            }
            
            if (plug.ColorId != acceptsColorId)
            {
                Debug.LogWarning($"Color mismatch: socket {acceptsColorId} cannot accept plug {plug.ColorId}");
                return;
            }
            
            _connectedPlug = plug;
            _isOccupied = true;
            
            UpdateVisual();
            
            Debug.Log($"Plug {plug.ColorId} connected to socket {acceptsColorId}");
        }
        
        /// <summary>
        /// Called when a plug disconnects from this socket.
        /// </summary>
        public void OnPlugDisconnected()
        {
            if (!_isOccupied) return;
            
            _connectedPlug = null;
            _isOccupied = false;
            
            UpdateVisual();
            
            Debug.Log($"Plug disconnected from socket {acceptsColorId}");
        }
        
        /// <summary>
        /// Force disconnect any connected plug (useful for puzzle reset).
        /// </summary>
        public void ForceDisconnect()
        {
            if (!_isOccupied) return;
            
            if (_connectedPlug != null)
            {
                _connectedPlug.Disconnect();
            }
            
            _connectedPlug = null;
            _isOccupied = false;
            UpdateVisual();
        }
        
        /// <summary>
        /// Check if this socket is correctly connected (occupied and color matches).
        /// </summary>
        /// <returns>True if correctly connected, false otherwise.</returns>
        public bool IsCorrectlyConnected()
        {
            return _isOccupied && _connectedPlug != null && _connectedPlug.ColorId == acceptsColorId;
        }
        
        private void UpdateVisual()
        {
            if (socketRenderer == null) return;
            
            Material targetMaterial;
            
            if (_isOccupied)
            {
                targetMaterial = occupiedMaterial;
            }
            else if (isRequired)
            {
                targetMaterial = requiredMaterial;
            }
            else
            {
                targetMaterial = emptyMaterial;
            }
            
            if (targetMaterial != null)
            {
                socketRenderer.material = targetMaterial;
                socketRenderer.material.color = socketColor;
            }
        }
        
        // Unity Editor support
        private void OnValidate()
        {
            // Ensure acceptsColorId is non-negative
            acceptsColorId = Mathf.Max(0, acceptsColorId);
            
            // Update visual in editor
            if (Application.isPlaying && socketRenderer != null)
            {
                UpdateVisual();
            }
        }
        
        // Gizmos for easier setup in editor
        private void OnDrawGizmos()
        {
            Gizmos.color = socketColor;
            Gizmos.DrawWireSphere(transform.position, 0.1f);
            
            if (isRequired)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(transform.position, Vector3.one * 0.2f);
            }
        }
    }
} 