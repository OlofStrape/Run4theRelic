using UnityEngine;

namespace Run4theRelic.UI
{
	/// <summary>
	/// Pulses emission color on a target material over time using a sine curve.
	/// Example: Använd på Relicen (M_RelicCore) och run-ytor (M_RuneGlow).
	/// </summary>
	public class GlowPulse : MonoBehaviour
	{
		[Header("Target")]
		public Renderer target;
		public string emissionProperty = "_EmissionColor";

		[Header("Emission Settings")]
		public Color baseColor = Color.white;
		public float minIntensity = 0.8f;
		public float maxIntensity = 2.0f;
		public float speed = 2.0f;

		private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

		private void Reset()
		{
			if (target == null)
			{
				target = GetComponent<Renderer>();
			}
		}

		private void Update()
		{
			if (target == null) return;

			// Compute t from sine in [0..1]
			float t = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f;
			float intensity = Mathf.Lerp(minIntensity, maxIntensity, t);

			// Ensure keyword is enabled on the material instance
			var mat = target.material;
			mat.EnableKeyword("_EMISSION");

			// Respect custom emission property name if provided
			int propertyId = emissionProperty == "_EmissionColor" ? EmissionColorId : Shader.PropertyToID(emissionProperty);
			mat.SetColor(propertyId, baseColor * intensity);
		}
	}
}

