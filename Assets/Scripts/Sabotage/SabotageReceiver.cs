using UnityEngine;
using Run4theRelic.Core;

namespace Run4theRelic.Sabotage
{
	/// <summary>
	/// Component attached to each player root that can receive sabotage effects.
	/// Handles local feedback like HUD/notifications and toggles visuals like fog.
	/// </summary>
	public class SabotageReceiver : MonoBehaviour
	{
		[SerializeField] private Renderer targetRenderer;
		[SerializeField] private Material fogMaterial;
		[SerializeField] private Color fogColor = new Color(0.4f, 0.4f, 0.4f, 0.85f);
		[SerializeField] private float fogDensity = 0.6f;
		[SerializeField] private bool isLocal;

		private Material _originalMaterial;
		private bool _fogActive;

		private void Awake()
		{
			if (targetRenderer == null)
			{
				targetRenderer = GetComponentInChildren<Renderer>();
			}
			if (targetRenderer != null)
			{
				_originalMaterial = targetRenderer.material;
			}
		}

		/// <summary>
		/// Toggle fog effect on this player root. Duration is handled by caller.
		/// </summary>
		public void SetFog(bool enabled)
		{
			if (enabled == _fogActive) return;
			_fogActive = enabled;

			if (targetRenderer == null) return;

			if (enabled)
			{
				if (fogMaterial != null)
				{
					targetRenderer.material = fogMaterial;
					targetRenderer.material.color = fogColor;
					targetRenderer.material.SetFloat("_FogDensity", fogDensity);
				}
			}
			else
			{
				if (_originalMaterial != null)
				{
					targetRenderer.material = _originalMaterial;
				}
			}
		}

		/// <summary>
		/// Called when this player is sabotaged. Only invoke HUD/feedback locally.
		/// </summary>
		public void NotifySabotaged(string type, float duration)
		{
			if (isLocal)
			{
				GameEvents.TriggerSabotaged(type, duration);
			}
		}

		public void ConfigureLocal(bool value)
		{
			isLocal = value;
		}
	}
}

