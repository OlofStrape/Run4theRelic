using UnityEngine;

namespace Run4theRelic.Relic
{
	/// <summary>
	/// Simple pulsing aura ring using a LineRenderer. Scales and fades over time.
	/// Attach to the Relic object for a subtle glow/aura effect.
	/// </summary>
	[RequireComponent(typeof(LineRenderer))]
	public class RelicAura : MonoBehaviour
	{
		[Header("Aura Shape")]
		public float radius = 0.25f;
		[SerializeField] private int segments = 48;

		[Header("Pulse")]
		public float pulseSpeed = 1.5f;
		public float minScale = 0.9f;
		public float maxScale = 1.1f;
		[SerializeField] private float minAlpha = 0.35f;
		[SerializeField] private float maxAlpha = 0.85f;

		private LineRenderer _lineRenderer;
		private Vector3[] _points;
		private float _time;

		private void Awake()
		{
			_lineRenderer = GetComponent<LineRenderer>();
			SetupCircle();
		}

		private void OnValidate()
		{
			segments = Mathf.Clamp(segments, 8, 256);
			radius = Mathf.Max(0.01f, radius);
			minScale = Mathf.Max(0.1f, minScale);
			maxScale = Mathf.Max(minScale, maxScale);
			minAlpha = Mathf.Clamp01(minAlpha);
			maxAlpha = Mathf.Clamp01(maxAlpha);
			if (_lineRenderer != null)
			{
				SetupCircle();
			}
		}

		private void SetupCircle()
		{
			if (_lineRenderer == null) return;
			if (_points == null || _points.Length != segments + 1)
			{
				_points = new Vector3[segments + 1];
			}

			float step = Mathf.PI * 2f / segments;
			for (int i = 0; i <= segments; i++)
			{
				float angle = step * i;
				_points[i] = new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
			}

			_lineRenderer.useWorldSpace = false;
			_lineRenderer.loop = true;
			_lineRenderer.positionCount = _points.Length;
			_lineRenderer.SetPositions(_points);
		}

		private void Update()
		{
			_time += Time.deltaTime * pulseSpeed;
			float t = (Mathf.Sin(_time) + 1f) * 0.5f;
			float scale = Mathf.Lerp(minScale, maxScale, t);
			float alpha = Mathf.Lerp(minAlpha, maxAlpha, t);

			transform.localScale = new Vector3(scale, 1f, scale);

			if (_lineRenderer != null)
			{
				Color start = _lineRenderer.startColor;
				Color end = _lineRenderer.endColor;
				start.a = alpha;
				end.a = alpha;
				_lineRenderer.startColor = start;
				_lineRenderer.endColor = end;
			}
		}
	}
}

