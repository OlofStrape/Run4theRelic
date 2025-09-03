using System.Collections.Generic;
using UnityEngine;

namespace Run4theRelic.Puzzles.LaserRedirect
{
    /// <summary>
    /// Optional gizmo to draw small spheres at the laser hit points.
    /// Attach on the same GameObject as LaserEmitter.
    /// </summary>
    [ExecuteAlways]
    public class LaserDebugGizmo : MonoBehaviour
    {
        public Color gizmoColor = new Color(1f, 0.2f, 0.2f, 0.8f);
        public float radius = 0.05f;
        public int maxPoints = 16;

        private readonly List<Vector3> _lastPoints = new List<Vector3>(16);

        private LaserEmitter _emitter;

        private void Awake()
        {
            _emitter = GetComponent<LaserEmitter>();
        }

        private void LateUpdate()
        {
            // Sample points directly from the line renderer each frame
            _lastPoints.Clear();
            if (_emitter != null && _emitter.line != null)
            {
                int count = Mathf.Min(_emitter.line.positionCount, maxPoints);
                for (int i = 0; i < count; i++)
                {
                    _lastPoints.Add(_emitter.line.GetPosition(i));
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = gizmoColor;
            for (int i = 0; i < _lastPoints.Count; i++)
            {
                Gizmos.DrawSphere(_lastPoints[i], radius);
            }
        }
    }
}

