using UnityEngine;
using Puzzles;

namespace Puzzles.BalanceBridge
{
    /// <summary>
    /// Simple weight-balance puzzle controller.
    /// </summary>
    public sealed class BalanceBridgeController : PuzzleControllerBase
    {
        [SerializeField, Min(0f)]
        private float targetWeight = 10f;

        private float accumulatedWeight;

        /// <summary>Adds weight to the bridge. Solves when threshold reached.</summary>
        public void AddWeight(float amount)
        {
            if (IsSolved)
            {
                return;
            }

            accumulatedWeight = Mathf.Max(0f, accumulatedWeight + amount);
            if (accumulatedWeight >= targetWeight)
            {
                Debug.Log("Balance bridge solved");
                MarkSolved();
            }
        }

        /// <inheritdoc />
        public override void ResetPuzzle()
        {
            base.ResetPuzzle();
            accumulatedWeight = 0f;
        }
    }
}
