using System.Collections.Generic;
using UnityEngine;
using Puzzles;

namespace Puzzles.RunicSequence
{
    /// <summary>
    /// Validates activation order of rune pads.
    /// </summary>
    public sealed class RunicSequenceController : PuzzleControllerBase
    {
        [SerializeField]
        private List<string> requiredOrder = new List<string> { "ᚠ", "ᚢ", "ᚦ" };

        private int currentIndex;

        /// <summary>Registers a rune activation attempt.</summary>
        public void Activate(string rune)
        {
            if (IsSolved)
            {
                return;
            }

            if (currentIndex < 0 || currentIndex >= requiredOrder.Count)
            {
                currentIndex = 0;
            }

            if (requiredOrder[currentIndex] == rune)
            {
                currentIndex++;
                if (currentIndex >= requiredOrder.Count)
                {
                    Debug.Log("Runic sequence solved");
                    MarkSolved();
                }
            }
            else
            {
                currentIndex = 0;
            }
        }

        /// <inheritdoc />
        public override void ResetPuzzle()
        {
            base.ResetPuzzle();
            currentIndex = 0;
        }
    }
}
