using UnityEngine;

namespace Relic
{
    /// <summary>
    /// Represents an extraction zone that triggers relic completion when entered.
    /// </summary>
    public sealed class ExtractionZone : MonoBehaviour
    {
        /// <summary>Simulates entering extraction with a relic controller.</summary>
        public void EnterZone(RelicController relic)
        {
            if (relic == null)
            {
                return;
            }

            relic.Extract();
        }
    }
}
