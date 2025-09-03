using UnityEngine;

namespace Puzzles.CableConnect
{
    /// <summary>
    /// Represents a plug that can connect to a <see cref="CableSocket"/>.
    /// </summary>
    public sealed class CablePlug : MonoBehaviour
    {
        /// <summary>Identifier used to validate matching sockets.</summary>
        public string PlugId => plugId;

        [SerializeField]
        private string plugId = "A";
    }
}
