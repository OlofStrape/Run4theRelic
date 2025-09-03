using UnityEngine;

namespace Run4theRelic.Puzzles.TiltMaze
{
    public class TiltPlateController : MonoBehaviour
    {
        [Header("References")]
        public Transform leftHandle;
        public Transform rightHandle;
        public Transform plate;

        [Header("Settings")]
        [Range(2f, 20f)] public float maxTiltDeg = 10f;

        private void Update()
        {
            if (!leftHandle || !rightHandle || !plate) return;

            // ta små lutningar kring X/Z från handtagens lokala rot
            var avg = Quaternion.Slerp(leftHandle.localRotation, rightHandle.localRotation, 0.5f).eulerAngles;
            float tiltX = Mathf.DeltaAngle(0f, avg.x);
            float tiltZ = Mathf.DeltaAngle(0f, avg.z);
            tiltX = Mathf.Clamp(tiltX, -maxTiltDeg, maxTiltDeg);
            tiltZ = Mathf.Clamp(tiltZ, -maxTiltDeg, maxTiltDeg);
            plate.localRotation = Quaternion.Euler(tiltX, 0f, tiltZ);
        }
    }
}

