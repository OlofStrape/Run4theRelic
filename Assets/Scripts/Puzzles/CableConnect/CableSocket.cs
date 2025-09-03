using UnityEngine;

namespace Puzzles.CableConnect
{
    /// <summary>
    /// Represents a socket that accepts a matching <see cref="CablePlug"/>.
    /// </summary>
    public sealed class CableSocket : MonoBehaviour
    {
        /// <summary>Identifier used to validate matching plugs.</summary>
        public string SocketId => socketId;

        [SerializeField]
        private string socketId = "A";
    }
}
