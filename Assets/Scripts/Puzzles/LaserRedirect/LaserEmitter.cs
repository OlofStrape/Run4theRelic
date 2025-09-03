using System.Collections.Generic;
using UnityEngine;

namespace Run4theRelic.Puzzles.LaserRedirect
{
    /// <summary>
    /// Emits a laser ray with reflections, updates a LineRenderer with segments.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class LaserEmitter : MonoBehaviour
    {
        [Header("Laser Settings")]
        public float maxDistance = 30f;
        public int maxBounces = 5;
        public LayerMask hitMask = ~0;
        public LineRenderer line;

        [Header("Debug")] 
        [SerializeField] private bool showDebug;

        private readonly List<Vector3> _points = new List<Vector3>(16);

        private void Reset()
        {
            line = GetComponent<LineRenderer>();
            if (line != null)
            {
                line.positionCount = 0;
                line.useWorldSpace = true;
            }
        }

        private void Awake()
        {
            if (line == null)
            {
                line = GetComponent<LineRenderer>();
            }
        }

        private void Update()
        {
            SimulateAndRender();
        }

        private void SimulateAndRender()
        {
            _points.Clear();

            Vector3 currentOrigin = transform.position;
            Vector3 currentDir = transform.forward;
            float remainingDistance = maxDistance;
            _points.Add(currentOrigin);

            for (int bounce = 0; bounce < maxBounces; bounce++)
            {
                if (remainingDistance <= 0f)
                {
                    break;
                }

                if (Physics.Raycast(currentOrigin, currentDir, out RaycastHit hit, remainingDistance, hitMask, QueryTriggerInteraction.Ignore))
                {
                    _points.Add(hit.point);

                    // Check receiver
                    if (hit.collider.TryGetComponent<LaserReceiver>(out var receiver))
                    {
                        receiver.OnLaserContact(Time.deltaTime);
                        // End the laser at the receiver
                        break;
                    }

                    // Check mirror
                    if (hit.collider.GetComponent<LaserMirror>() != null)
                    {
                        // Reflect and continue
                        Vector3 reflectDir = Vector3.Reflect(currentDir, hit.normal).normalized;
                        remainingDistance -= hit.distance;
                        currentOrigin = hit.point + reflectDir * 0.001f; // small offset to avoid immediate re-hit
                        currentDir = reflectDir;
                        continue;
                    }

                    // Hit something else - end here
                    break;
                }
                else
                {
                    // No hit, end at max distance along remaining path
                    Vector3 endPoint = currentOrigin + currentDir * remainingDistance;
                    _points.Add(endPoint);
                    break;
                }
            }

            // Update line renderer
            if (line != null)
            {
                line.positionCount = _points.Count;
                for (int i = 0; i < _points.Count; i++)
                {
                    line.SetPosition(i, _points[i]);
                }
            }

            if (showDebug)
            {
                for (int i = 0; i < _points.Count - 1; i++)
                {
                    Debug.DrawLine(_points[i], _points[i + 1], Color.red);
                }
            }
        }
    }
}

