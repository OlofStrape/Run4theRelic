using UnityEngine;

namespace UI.HUD
{
    /// <summary>
    /// Keeps this transform facing a target camera each frame for readable world-space UI.
    /// </summary>
    public class Billboard : MonoBehaviour
    {
        /// <summary>
        /// Target camera. If null, will attempt to resolve via Camera.main.
        /// </summary>
        public Camera targetCamera;

        private void LateUpdate()
        {
            Camera cam = targetCamera != null ? targetCamera : Camera.main;
            if (cam == null)
            {
                return;
            }

            Vector3 forward = (transform.position - cam.transform.position).normalized;
            if (forward.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
            }
        }
    }
}

