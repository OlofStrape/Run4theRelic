using System;
using UnityEngine;

namespace Puzzles
{
    /// <summary>
    /// Base class for all puzzle controllers.
    /// </summary>
    public abstract class PuzzleControllerBase : MonoBehaviour
    {
        /// <summary>Raised when the puzzle is solved.</summary>
        public event Action? PuzzleSolved;

        /// <summary>Raised when the puzzle is reset.</summary>
        public event Action? PuzzleReset;

        /// <summary>True if the puzzle is currently solved.</summary>
        public bool IsSolved { get; protected set; }

        /// <summary>Resets the puzzle to its initial state.</summary>
        public virtual void ResetPuzzle()
        {
            IsSolved = false;
            PuzzleReset?.Invoke();
        }

        /// <summary>Marks the puzzle as solved and raises events.</summary>
        protected void MarkSolved()
        {
            if (IsSolved) return;
            IsSolved = true;
            PuzzleSolved?.Invoke();
        }
    }
}
