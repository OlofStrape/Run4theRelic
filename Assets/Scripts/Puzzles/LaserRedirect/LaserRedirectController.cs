using System.Collections.Generic;
using UnityEngine;
using Run4theRelic.Puzzles;

namespace Run4theRelic.Puzzles.LaserRedirect
{
    /// <summary>
    /// Controls the Laser Redirect puzzle. Subscribes to receivers and completes on success.
    /// </summary>
    public class LaserRedirectController : PuzzleControllerBase
    {
        [Header("Laser Redirect")] 
        [SerializeField] private List<LaserReceiver> receivers = new List<LaserReceiver>();

        protected override void OnPuzzleStart()
        {
            // Subscribe to receiver events
            foreach (var r in receivers)
            {
                if (r != null)
                {
                    r.OnReceiverSatisfied += HandleReceiverSatisfied;
                }
            }
        }

        protected override void OnPuzzleComplete()
        {
            // Unsubscribe
            foreach (var r in receivers)
            {
                if (r != null)
                {
                    r.OnReceiverSatisfied -= HandleReceiverSatisfied;
                }
            }

            Debug.Log("Laser Redirect puzzle completed!");
        }

        protected override void OnPuzzleFailed()
        {
            // Unsubscribe on failure as well
            foreach (var r in receivers)
            {
                if (r != null)
                {
                    r.OnReceiverSatisfied -= HandleReceiverSatisfied;
                }
            }

            Debug.Log("Laser Redirect puzzle failed - time expired!");
        }

        protected override void OnPuzzleReset()
        {
            // Ensure detached
            foreach (var r in receivers)
            {
                if (r != null)
                {
                    r.OnReceiverSatisfied -= HandleReceiverSatisfied;
                }
            }
        }

        private void HandleReceiverSatisfied(LaserReceiver receiver)
        {
            if (!_isCompleted && !_isFailed)
            {
                Complete();
            }
        }
    }
}

