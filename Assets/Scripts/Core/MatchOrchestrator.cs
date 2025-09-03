using UnityEngine;

namespace Core
{
    /// <summary>
    /// Coordinates match lifecycle and bridges puzzle signals to global events.
    /// </summary>
    public sealed class MatchOrchestrator : MonoBehaviour
    {
        /// <summary>Automatically start the match on play.</summary>
        [SerializeField]
        private bool autoStart = true;

        private bool hasStarted;

        private void Start()
        {
            if (autoStart)
            {
                StartMatch();
            }
        }

        /// <summary>
        /// Starts the match and raises <see cref="GameEvents.MatchStarted"/>.
        /// </summary>
        public void StartMatch()
        {
            if (hasStarted)
            {
                return;
            }

            hasStarted = true;
            Debug.Log("Match started");
            GameEvents.RaiseMatchStarted();
        }

        /// <summary>
        /// Ends the match and raises <see cref="GameEvents.MatchEnded"/>.
        /// </summary>
        public void EndMatch()
        {
            if (!hasStarted)
            {
                return;
            }

            hasStarted = false;
            Debug.Log("Match ended");
            GameEvents.RaiseMatchEnded();
        }
    }
}
