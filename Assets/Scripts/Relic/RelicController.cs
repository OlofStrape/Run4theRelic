using UnityEngine;
using Core;

namespace Relic
{
    /// <summary>
    /// Controls relic pickup and extraction lifecycle.
    /// </summary>
    public sealed class RelicController : MonoBehaviour
    {
        /// <summary>Whether the relic is currently held.</summary>
        public bool IsHeld { get; private set; }

        /// <summary>Attempts to pick up the relic.</summary>
        public void PickUp()
        {
            IsHeld = true;
            Debug.Log("Relic picked up");
        }

        /// <summary>Drops the relic at the current position.</summary>
        public void Drop()
        {
            IsHeld = false;
            Debug.Log("Relic dropped");
        }

        /// <summary>Marks the relic as extracted and raises a global event.</summary>
        public void Extract()
        {
            Debug.Log("Relic extracted");
            GameEvents.RaiseRelicExtracted();
        }
    }
}
