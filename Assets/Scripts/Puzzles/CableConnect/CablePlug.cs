using UnityEngine;

namespace Run4theRelic.Puzzles.CableConnect
{
    /// <summary>
    /// Represents a cable plug that can be connected to matching sockets.
    /// ColorId must match acceptsColorId on the socket for connection.
    /// </summary>
    public class CablePlug : MonoBehaviour
    {
        [Header("Cable Settings")]
        [SerializeField] private int colorId = 0;
        [SerializeField] private Color plugColor = Color.red;
        [SerializeField] private float snapDistance = 0.1f;
        
        [Header("Visual")]
        [SerializeField] private Renderer plugRenderer;
        [SerializeField] private Material connectedMaterial;
        [SerializeField] private Material disconnectedMaterial;
        
        private CableSocket _connectedSocket;
        private Vector3 _originalPosition;
        private bool _isDragging;
        private bool _isConnected;
        
        /// <summary>
        /// Unique color identifier for this plug.
        /// </summary>
        public int ColorId => colorId;
        
        /// <summary>
        /// Is this plug currently connected to a socket.
        /// </summary>
        public bool IsConnected => _isConnected;
        
        /// <summary>
        /// The socket this plug is connected to, or null if disconnected.
        /// </summary>
        public CableSocket ConnectedSocket => _connectedSocket;
        
        private void Start()
        {
            _originalPosition = transform.position;
            
            // Set initial color
            if (plugRenderer != null)
            {
                plugRenderer.material = disconnectedMaterial;
                plugRenderer.material.color = plugColor;
            }
        }
        
        private void Update()
        {
            if (_isDragging)
            {
                HandleDragging();
            }
        }
        
        /// <summary>
        /// Start dragging this plug (called by input system).
        /// </summary>
        public void StartDrag()
        {
            if (_isConnected) return;
            
            _isDragging = true;
            
            if (showDebugInfo)
            {
                Debug.Log($"Started dragging plug {colorId}");
            }
        }
        
        /// <summary>
        /// Stop dragging this plug.
        /// </summary>
        public void StopDrag()
        {
            if (!_isDragging) return;
            
            _isDragging = false;
            
            // Try to snap to nearby socket
            TrySnapToSocket();
            
            if (showDebugInfo)
            {
                Debug.Log($"Stopped dragging plug {colorId}");
            }
        }
        
        /// <summary>
        /// Connect this plug to a specific socket.
        /// </summary>
        /// <param name="socket">The socket to connect to.</param>
        public void ConnectTo(CableSocket socket)
        {
            if (_isConnected)
            {
                Disconnect();
            }
            
            if (socket == null) return;
            
            // Check if colors match
            if (socket.AcceptsColorId != colorId)
            {
                Debug.LogWarning($"Color mismatch: plug {colorId} cannot connect to socket {socket.AcceptsColorId}");
                return;
            }
            
            // Check if socket is available
            if (socket.IsOccupied)
            {
                Debug.LogWarning($"Socket {socket.AcceptsColorId} is already occupied");
                return;
            }
            
            // Connect
            _connectedSocket = socket;
            _isConnected = true;
            
            // Move plug to socket position
            transform.position = socket.transform.position;
            
            // Update visual
            UpdateVisual();
            
            // Notify socket
            socket.OnPlugConnected(this);
            
            if (showDebugInfo)
            {
                Debug.Log($"Plug {colorId} connected to socket {socket.AcceptsColorId}");
            }
        }
        
        /// <summary>
        /// Disconnect this plug from its current socket.
        /// </summary>
        public void Disconnect()
        {
            if (!_isConnected) return;
            
            // Notify socket
            if (_connectedSocket != null)
            {
                _connectedSocket.OnPlugDisconnected();
            }
            
            // Reset state
            _connectedSocket = null;
            _isConnected = false;
            
            // Return to original position
            transform.position = _originalPosition;
            
            // Update visual
            UpdateVisual();
            
            if (showDebugInfo)
            {
                Debug.Log($"Plug {colorId} disconnected");
            }
        }
        
        private void HandleDragging()
        {
            // This would be implemented with actual input system
            // For now, just log that dragging is happening
            if (showDebugInfo)
            {
                Debug.Log($"Dragging plug {colorId}");
            }
        }
        
        private void TrySnapToSocket()
        {
            // Find nearby sockets
            CableSocket[] sockets = FindObjectsOfType<CableSocket>();
            CableSocket closestSocket = null;
            float closestDistance = float.MaxValue;
            
            foreach (CableSocket socket in sockets)
            {
                if (socket.IsOccupied) continue;
                if (socket.AcceptsColorId != colorId) continue;
                
                float distance = Vector3.Distance(transform.position, socket.transform.position);
                if (distance < snapDistance && distance < closestDistance)
                {
                    closestSocket = socket;
                    closestDistance = distance;
                }
            }
            
            // Snap to closest socket
            if (closestSocket != null)
            {
                ConnectTo(closestSocket);
            }
        }
        
        private void UpdateVisual()
        {
            if (plugRenderer == null) return;
            
            if (_isConnected)
            {
                plugRenderer.material = connectedMaterial;
            }
            else
            {
                plugRenderer.material = disconnectedMaterial;
            }
            
            plugRenderer.material.color = plugColor;
        }
        
        // Unity Editor support
        private void OnValidate()
        {
            // Ensure colorId is non-negative
            colorId = Mathf.Max(0, colorId);
            
            // Update visual in editor
            if (Application.isPlaying && plugRenderer != null)
            {
                UpdateVisual();
            }
        }
        
        // Debug info
        private bool showDebugInfo => true; // Could be made configurable
    }
} 