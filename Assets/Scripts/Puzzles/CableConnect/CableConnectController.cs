using System.Collections.Generic;
using UnityEngine;
using Run4theRelic.Puzzles;

namespace Run4theRelic.Puzzles.CableConnect
{
    /// <summary>
    /// Controls the cable connection puzzle by monitoring plug-socket connections.
    /// Puzzle is completed when all required sockets are filled with matching plugs.
    /// </summary>
    public class CableConnectController : PuzzleControllerBase
    {
        [Header("Cable Connect Settings")]
        [SerializeField] private List<CableSocket> sockets = new List<CableSocket>();
        [SerializeField] private List<CablePlug> plugs = new List<CablePlug>();
        [SerializeField] private bool checkCompletionOnUpdate = true;
        
        [Header("Debug")]
        [SerializeField] private bool showConnectionStatus = true;
        
        private bool _wasCompleted;
        
        protected override void OnPuzzleStart()
        {
            // Reset all connections
            ResetAllConnections();
            
            // Verify setup
            ValidateSetup();
            
            Debug.Log($"Cable Connect puzzle started with {sockets.Count} sockets and {plugs.Count} plugs");
        }
        
        protected override void OnPuzzleComplete()
        {
            // Puzzle logic handled in base class
            Debug.Log("Cable Connect puzzle completed!");
        }
        
        protected override void OnPuzzleFailed()
        {
            // Puzzle logic handled in base class
            Debug.Log("Cable Connect puzzle failed - time expired!");
        }
        
        protected override void OnPuzzleReset()
        {
            ResetAllConnections();
            _wasCompleted = false;
            
            Debug.Log("Cable Connect puzzle reset");
        }
        
        private void Update()
        {
            if (checkCompletionOnUpdate && IsActive && !IsCompleted)
            {
                CheckCompletion();
            }
        }
        
        /// <summary>
        /// Check if all required sockets are correctly connected.
        /// </summary>
        public void CheckCompletion()
        {
            if (IsCompleted || IsFailed) return;
            
            bool allConnected = true;
            int connectedCount = 0;
            
            foreach (CableSocket socket in sockets)
            {
                if (socket.IsRequired)
                {
                    if (!socket.IsCorrectlyConnected())
                    {
                        allConnected = false;
                    }
                    else
                    {
                        connectedCount++;
                    }
                }
            }
            
            // Update debug info
            if (showConnectionStatus)
            {
                Debug.Log($"Cable Connect: {connectedCount}/{GetRequiredSocketCount()} sockets connected");
            }
            
            // Check if puzzle is complete
            if (allConnected && !_wasCompleted)
            {
                _wasCompleted = true;
                Complete();
            }
        }
        
        /// <summary>
        /// Reset all plug-socket connections.
        /// </summary>
        public void ResetAllConnections()
        {
            foreach (CablePlug plug in plugs)
            {
                if (plug.IsConnected)
                {
                    plug.Disconnect();
                }
            }
        }
        
        /// <summary>
        /// Get the number of required sockets.
        /// </summary>
        /// <returns>Count of required sockets.</returns>
        public int GetRequiredSocketCount()
        {
            int count = 0;
            foreach (CableSocket socket in sockets)
            {
                if (socket.IsRequired)
                {
                    count++;
                }
            }
            return count;
        }
        
        /// <summary>
        /// Get the number of currently connected sockets.
        /// </summary>
        /// <returns>Count of connected sockets.</returns>
        public int GetConnectedSocketCount()
        {
            int count = 0;
            foreach (CableSocket socket in sockets)
            {
                if (socket.IsRequired && socket.IsCorrectlyConnected())
                {
                    count++;
                }
            }
            return count;
        }
        
        /// <summary>
        /// Validate that the puzzle setup is correct.
        /// </summary>
        private void ValidateSetup()
        {
            if (sockets.Count == 0)
            {
                Debug.LogWarning("Cable Connect: No sockets assigned!");
            }
            
            if (plugs.Count == 0)
            {
                Debug.LogWarning("Cable Connect: No plugs assigned!");
            }
            
            // Check for color mismatches
            HashSet<int> socketColors = new HashSet<int>();
            HashSet<int> plugColors = new HashSet<int>();
            
            foreach (CableSocket socket in sockets)
            {
                if (socket.IsRequired)
                {
                    socketColors.Add(socket.AcceptsColorId);
                }
            }
            
            foreach (CablePlug plug in plugs)
            {
                plugColors.Add(plug.ColorId);
            }
            
            // Check if all socket colors have matching plugs
            foreach (int socketColor in socketColors)
            {
                if (!plugColors.Contains(socketColor))
                {
                    Debug.LogWarning($"Cable Connect: Socket color {socketColor} has no matching plug!");
                }
            }
            
            // Check if all plug colors have matching sockets
            foreach (int plugColor in plugColors)
            {
                if (!socketColors.Contains(plugColor))
                {
                    Debug.LogWarning($"Cable Connect: Plug color {plugColor} has no matching socket!");
                }
            }
        }
        
        // Unity Editor support
        private void OnValidate()
        {
            // Remove null entries
            sockets.RemoveAll(socket => socket == null);
            plugs.RemoveAll(plug => plug == null);
        }
        
        // Gizmos for easier setup in editor
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            
            // Draw lines between connected plugs and sockets
            foreach (CableSocket socket in sockets)
            {
                if (socket.IsCorrectlyConnected() && socket.ConnectedPlug != null)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(socket.transform.position, socket.ConnectedPlug.transform.position);
                }
            }
        }
    }
} 