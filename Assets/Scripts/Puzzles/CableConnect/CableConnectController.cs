using UnityEngine;
using Puzzles;

namespace Puzzles.CableConnect
{
    /// <summary>
    /// Controls cable plug/socket matching logic.
    /// </summary>
    public sealed class CableConnectController : PuzzleControllerBase
    {
        /// <summary>Optional: expected pair identifier.</summary>
        [SerializeField]
        private string expectedId = "A";

        /// <summary>
        /// Simulate a connection between a plug and socket.
        /// </summary>
        public void TryConnect(CablePlug plug, CableSocket socket)
        {
            if (plug == null || socket == null)
            {
                return;
            }

            if (!IsSolved && plug.PlugId == socket.SocketId && plug.PlugId == expectedId)
            {
                Debug.Log($"Cable connected: {plug.PlugId}");
                MarkSolved();
            }
        }
    }
}
