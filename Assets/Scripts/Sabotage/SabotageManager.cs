using UnityEngine;

namespace Sabotage
{
    /// <summary>
    /// Central manager for handling sabotage effects.
    /// </summary>
    public sealed class SabotageManager : MonoBehaviour
    {
        [SerializeField, Min(0f)]
        private float sabotageDurationSeconds = 5f;

        private float endTime;

        /// <summary>Triggers a sabotage effect for the configured duration.</summary>
        public void TriggerSabotage()
        {
            endTime = Time.time + sabotageDurationSeconds;
            Debug.Log($"Sabotage triggered for {sabotageDurationSeconds} seconds");
        }

        private void Update()
        {
            if (endTime > 0f && Time.time >= endTime)
            {
                endTime = 0f;
                Debug.Log("Sabotage ended");
            }
        }
    }
}
