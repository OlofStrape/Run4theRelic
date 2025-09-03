using UnityEngine;

namespace Run4theRelic.UI
{
	/// <summary>
	/// Displays world-space text that always faces the active camera.
	/// Provides Show(message, seconds) to display temporary notifications like "SABOTAGED!" near the player.
	/// If TextMeshPro is not available, falls back to Unity's TextMesh.
	/// Note: To add TextMeshPro, open Package Manager and install "TextMeshPro".
	/// </summary>
	public class WorldspaceBillboardText : MonoBehaviour
	{
		[Header("Display")]
		public string defaultMessage = "";
		public float defaultDuration = 2f;
		public Color textColor = Color.white;

		private float _remaining;
		private Camera _cam;
		private Component _textComponent; // TMP_Text or TextMesh

		private void Awake()
		{
			_cam = Camera.main;
			_textComponent = TryGetTextComponent();
			SetText(defaultMessage);
			SetColor(textColor);
		}

		private void LateUpdate()
		{
			if (_cam == null) _cam = Camera.main;
			if (_cam != null)
			{
				transform.rotation = Quaternion.LookRotation(transform.position - _cam.transform.position);
			}

			if (_remaining > 0f)
			{
				_remaining -= Time.deltaTime;
				if (_remaining <= 0f)
				{
					SetText("");
				}
			}
		}

		/// <summary>
		/// Show a message for a given number of seconds.
		/// </summary>
		public void Show(string message, float seconds)
		{
			SetText(message);
			_remaining = seconds;
		}

		private Component TryGetTextComponent()
		{
			// Try TMP
#if TMP_PRESENT
			var tmp = GetComponent<TMPro.TMP_Text>();
			if (tmp != null) return tmp;
#endif
			// Fallback TextMesh
			var tm = GetComponent<TextMesh>();
			if (tm == null)
			{
				tm = gameObject.AddComponent<TextMesh>();
				tm.anchor = TextAnchor.MiddleCenter;
				tm.characterSize = 0.25f;
			}
			return tm;
		}

		private void SetText(string message)
		{
#if TMP_PRESENT
			var tmp = _textComponent as TMPro.TMP_Text;
			if (tmp != null)
			{
				tmp.text = message;
				return;
			}
#endif
			var tm = _textComponent as TextMesh;
			if (tm != null)
			{
				tm.text = message;
			}
		}

		private void SetColor(Color color)
		{
#if TMP_PRESENT
			var tmp = _textComponent as TMPro.TMP_Text;
			if (tmp != null)
			{
				tmp.color = color;
				return;
			}
#endif
			var tm = _textComponent as TextMesh;
			if (tm != null)
			{
				tm.color = color;
			}
		}
	}
}

