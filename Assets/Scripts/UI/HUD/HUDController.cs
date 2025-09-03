using UnityEngine;
using UnityEngine.UI;

namespace Run4theRelic.UI
{
	/// <summary>
	/// Minimal HUD-controller som kan visa snabba textmeddelanden (flash).
	/// </summary>
	public class HUDController : MonoBehaviour
	{
		[SerializeField] private Text messageText;
		[SerializeField] private Canvas canvas;
		[SerializeField] private float defaultMessageSeconds = 1.5f;

		float _timer;

		void Awake()
		{
			if (canvas == null) canvas = GetComponentInChildren<Canvas>(true);
			if (messageText == null && canvas != null) messageText = canvas.GetComponentInChildren<Text>(true);
			if (canvas == null)
			{
				var go = new GameObject("HUDCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
				go.transform.SetParent(transform, false);
				canvas = go.GetComponent<Canvas>();
				canvas.renderMode = RenderMode.ScreenSpaceOverlay;
				var msgGo = new GameObject("Message", typeof(Text));
				msgGo.transform.SetParent(go.transform, false);
				messageText = msgGo.GetComponent<Text>();
				messageText.alignment = TextAnchor.MiddleCenter;
				messageText.fontSize = 32;
				messageText.color = Color.yellow;
				var rt = messageText.rectTransform;
				rt.anchorMin = new Vector2(0, 0);
				rt.anchorMax = new Vector2(1, 0);
				rt.pivot = new Vector2(0.5f, 0);
				rt.offsetMin = new Vector2(0, 20);
				rt.offsetMax = new Vector2(0, 80);
			}
			SetMessage("");
		}

		void Update()
		{
			if (_timer > 0f)
			{
				_timer -= Time.deltaTime;
				if (_timer <= 0f)
				{
					SetMessage("");
				}
			}
		}

		public void ShowMessage(string text, float seconds = -1f)
		{
			SetMessage(text);
			_timer = seconds > 0f ? seconds : defaultMessageSeconds;
		}

		public void SetMessage(string text)
		{
			if (messageText != null) messageText.text = text;
		}
	}
}

