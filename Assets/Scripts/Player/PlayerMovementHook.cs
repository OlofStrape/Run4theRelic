using UnityEngine;

namespace Player
{
    /// <summary>
    /// Hook for integrating character movement events into gameplay systems.
    /// </summary>
    public sealed class PlayerMovementHook : MonoBehaviour
    {
        /// <summary>Reports a movement tick for external systems to consume.</summary>
        public void ReportMovement(Vector3 delta)
        {
            // Intentionally empty - used by other systems as a signal point
            if (delta.sqrMagnitude > 0f)
            {
                Debug.Log($"Player moved: {delta}");
            }
        }
    }
}
