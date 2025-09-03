using UnityEngine;

namespace Run4theRelic.UI
{
	/// <summary>
	/// Optional enhancement: ensure emission keyword is enabled on the material instance.
	/// </summary>
	[DisallowMultipleComponent]
	public class GlowPulse : MonoBehaviour
	{
		[SerializeField] private Renderer targetRenderer;
		[SerializeField] private string colorProperty = "_EmissionColor";
		[SerializeField] private Color baseEmission = new Color(0.3f, 0.9f, 1f);
		[SerializeField] private float speed = 2f;
		[SerializeField] private float strength = 1f;

		private Material _materialInstance;
		private float _t;

		private void Awake()
		{
			if (targetRenderer == null)
			{
				targetRenderer = GetComponent<Renderer>();
			}
			if (targetRenderer != null)
			{
				_materialInstance = targetRenderer.material;
			}
		}

		private void Start()
		{
			if (_materialInstance != null && !_materialInstance.IsKeywordEnabled("_EMISSION"))
			{
				_materialInstance.EnableKeyword("_EMISSION");
			}
		}

		private void Update()
		{
			if (_materialInstance == null) return;
			_t += Time.deltaTime * speed;
			float pulse = (Mathf.Sin(_t) + 1f) * 0.5f * strength;
			Color c = baseEmission * Mathf.LinearToGammaSpace(pulse);
			_materialInstance.SetColor(colorProperty, c);
		}
	}
}

