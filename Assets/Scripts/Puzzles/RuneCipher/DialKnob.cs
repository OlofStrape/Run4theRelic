using UnityEngine;
using System;

namespace Run4theRelic.Puzzles.RuneCipher
{
    /// <summary>Ratt/knopp som mappas till diskreta steg 0..steps-1 baserat på rotation runt Y.</summary>
    public class DialKnob : MonoBehaviour
    {
        [Min(2)] public int steps = 8;
        [Tooltip("Vinkelområde som tolkas (0..360).")]
        public float angleSpan = 360f;
        [Tooltip("Lokal rotationsaxel (vanligtvis Y).")]
        public Vector3 axis = Vector3.up;

        public event Action<int> OnStepChanged;

        int _currentStep = -1;

        void Update()
        {
            float ang = GetLocalSignedAngle();
            ang = Mathf.Repeat(ang + angleSpan * 0.5f, angleSpan); // 0..span
            int step = Mathf.Clamp(Mathf.FloorToInt((ang / angleSpan) * steps), 0, steps - 1);

            if (step != _currentStep)
            {
                _currentStep = step;
                OnStepChanged?.Invoke(_currentStep);
            }
        }

        float GetLocalSignedAngle()
        {
            // Projektera lokalrotation på given axel
            var e = transform.localEulerAngles;
            Vector3 a = axis.normalized;
            if (a == Vector3.up) return e.y;
            if (a == Vector3.right) return e.x;
            if (a == Vector3.forward) return e.z;
            // fallback: Y
            return e.y;
        }

        public int CurrentStep => _currentStep;
    }
}

