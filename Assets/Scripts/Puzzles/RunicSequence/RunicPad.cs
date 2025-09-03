using UnityEngine;

namespace Puzzles.RunicSequence
{
    /// <summary>
    /// Represents a rune pad that can be activated in sequence.
    /// </summary>
    public sealed class RunicPad : MonoBehaviour
    {
        /// <summary>Rune character or identifier.</summary>
        public string Rune => rune;

        [SerializeField]
        private string rune = "ᚠ";
    }
}
