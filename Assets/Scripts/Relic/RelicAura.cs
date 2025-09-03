using UnityEngine;

namespace Run4theRelic.Relic
{
    /// <summary>
    /// Simple pulsing ring aura using LineRenderer. Attach to the Relic.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class RelicAura : MonoBehaviour
    {
        [Header("Ring Shape")]
        [SerializeField] private int segments = 64;
        [SerializeField] private float baseRadius = 0.25f;
        [SerializeField] private float pulseAmplitude = 0.05f;
        [SerializeField] private float pulseSpeed = 2.0f;
        [SerializeField] private float heightOffset = 0.05f;

        [Header("Appearance")]
        [SerializeField] private Gradient colorOverTime;
        [SerializeField] private float colorCycleSpeed = 0.25f;

        private LineRenderer _lineRenderer;
        private float _time;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            if (_lineRenderer != null)
            {
                _lineRenderer.loop = true;
                _lineRenderer.useWorldSpace = false;
                _lineRenderer.positionCount = segments;
            }
        }

        private void Update()
        {
            _time += Time.deltaTime;
            float radius = baseRadius + Mathf.Sin(_time * Mathf.PI * 2f * pulseSpeed) * pulseAmplitude;
            DrawRing(radius);

            if (_lineRenderer != null && colorOverTime != null)
            {
                float t = Mathf.Repeat(_time * colorCycleSpeed, 1f);
                Color c = colorOverTime.Evaluate(t);
                _lineRenderer.startColor = c;
                _lineRenderer.endColor = c;
            }
        }

        private void OnValidate()
        {
            segments = Mathf.Max(8, segments);
            baseRadius = Mathf.Max(0.01f, baseRadius);
            pulseAmplitude = Mathf.Max(0f, pulseAmplitude);
            if (_lineRenderer != null)
            {
                _lineRenderer.positionCount = segments;
                DrawRing(baseRadius);
            }
        }

        private void DrawRing(float radius)
        {
            if (_lineRenderer == null) return;

            float angleStep = 360f / segments;
            for (int i = 0; i < segments; i++)
            {
                float angle = angleStep * i * Mathf.Deg2Rad;
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;
                _lineRenderer.SetPosition(i, new Vector3(x, heightOffset, z));
            }
        }
    }
}

