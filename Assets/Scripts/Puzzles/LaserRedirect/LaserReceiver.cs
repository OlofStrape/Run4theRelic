using System;
using UnityEngine;

namespace Run4theRelic.Puzzles.LaserRedirect
{
    /// <summary>
    /// Accumulates laser contact time until threshold is reached.
    /// </summary>
    [DisallowMultipleComponent]
    public class LaserReceiver : MonoBehaviour
    {
        [Header("Receiver Settings")]
        public float holdSeconds = 1.5f;

        [Header("Debug")]
        [SerializeField] private bool showDebug;

        private float _accumulatedSeconds;
        private int _lastContactFrame = -1;

        /// <summary>
        /// Raised when this receiver reaches the hold threshold.
        /// </summary>
        public event Action<LaserReceiver> OnReceiverSatisfied;

        /// <summary>
        /// Called by LaserEmitter when the laser hits this receiver for this frame.
        /// </summary>
        public void OnLaserContact(float deltaTime)
        {
            _lastContactFrame = Time.frameCount;
            _accumulatedSeconds += deltaTime;

            if (showDebug)
            {
                Debug.DrawRay(transform.position, Vector3.up * 0.25f, Color.green, 0f, false);
            }

            if (_accumulatedSeconds >= holdSeconds)
            {
                _accumulatedSeconds = holdSeconds;
                OnReceiverSatisfied?.Invoke(this);
            }
        }

        private void Update()
        {
            // If we did not receive contact this frame, decay the timer slightly
            if (_lastContactFrame != Time.frameCount)
            {
                float decay = Time.deltaTime * 0.5f;
                _accumulatedSeconds = Mathf.Max(0f, _accumulatedSeconds - decay);
            }
        }

        public float GetProgress01()
        {
            return holdSeconds <= 0f ? 1f : Mathf.Clamp01(_accumulatedSeconds / holdSeconds);
        }
    }
}

