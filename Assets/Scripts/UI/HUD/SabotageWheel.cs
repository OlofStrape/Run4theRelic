using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Run4theRelic.Sabotage;

namespace Run4theRelic.UI
{
	/// <summary>
	/// Simple world-space radial select wheel for sabotage choices.
	/// Auto-hides after a timeout or on selection.
	/// </summary>
	public class SabotageWheel : MonoBehaviour
	{
		public enum Option
		{
			Fog,
			TimeDrain,
			FakeClues
		}

		[SerializeField] private float autoHideSeconds = 5f;
		[SerializeField] private float radius = 0.25f;
		[SerializeField] private Vector3 worldOffset = new Vector3(0, 0, 2f);
		[SerializeField] private Font defaultFont;
		[SerializeField] private bool showDebugInfo = true;

		private Canvas _canvas;
		private RectTransform _root;
		private readonly List<GameObject> _spawnedUi = new List<GameObject>();
		private Coroutine _hideRoutine;
		private SabotageManager _sabotageManager;
		private SabotageTokenBank _tokenBank;
		private bool _isVisible;

		private void EnsureRefs()
		{
			if (_sabotageManager == null)
			{
				_sabotageManager = FindObjectOfType<SabotageManager>(true);
			}
			if (_tokenBank == null)
			{
				_tokenBank = FindObjectOfType<SabotageTokenBank>(true);
			}
		}

		/// <summary>
		/// Show the wheel with provided options.
		/// </summary>
		public void Show(Option[] options)
		{
			EnsureRefs();
			if (_tokenBank != null && _tokenBank.CurrentTokens <= 0)
			{
				if (showDebugInfo) Debug.Log("SabotageWheel: No tokens available. Not showing wheel.");
				return;
			}

			CreateUiIfNeeded();
			ClearButtons();

			// Position in front of camera in world space
			Camera cam = Camera.main;
			if (cam != null)
			{
				Vector3 pos = cam.transform.position + cam.transform.TransformVector(worldOffset);
				transform.position = pos;
				transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
			}

			// Build buttons
			float step = Mathf.PI * 2f / Mathf.Max(1, options.Length);
			for (int i = 0; i < options.Length; i++)
			{
				CreateButton(options[i], i * step);
			}

			SetVisible(true);
			if (_hideRoutine != null) StopCoroutine(_hideRoutine);
			_hideRoutine = StartCoroutine(AutoHide());
		}

		public void Hide()
		{
			SetVisible(false);
		}

		private void SetVisible(bool visible)
		{
			_isVisible = visible;
			if (_canvas != null)
			{
				_canvas.enabled = visible;
			}
		}

		private IEnumerator AutoHide()
		{
			yield return new WaitForSeconds(autoHideSeconds);
			Hide();
		}

		private void CreateUiIfNeeded()
		{
			if (_canvas != null) return;
			_canvas = gameObject.GetComponent<Canvas>();
			if (_canvas == null) _canvas = gameObject.AddComponent<Canvas>();
			_canvas.renderMode = RenderMode.WorldSpace;
			_canvas.worldCamera = Camera.main;
			CanvasScaler scaler = gameObject.GetComponent<CanvasScaler>();
			if (scaler == null) scaler = gameObject.AddComponent<CanvasScaler>();
			scaler.dynamicPixelsPerUnit = 200f;
			if (gameObject.GetComponent<GraphicRaycaster>() == null) gameObject.AddComponent<GraphicRaycaster>();

			_root = gameObject.GetComponent<RectTransform>();
			_root.sizeDelta = new Vector2(300, 300);
		}

		private void ClearButtons()
		{
			foreach (var go in _spawnedUi)
			{
				if (go != null) Destroy(go);
			}
			_spawnedUi.Clear();
		}

		private void CreateButton(Option option, float angle)
		{
			GameObject buttonGo = new GameObject(option.ToString(), typeof(RectTransform), typeof(Image), typeof(Button));
			buttonGo.transform.SetParent(_root, false);
			_spawnedUi.Add(buttonGo);

			RectTransform rt = buttonGo.GetComponent<RectTransform>();
			rt.sizeDelta = new Vector2(100, 140);
			Vector2 pos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius * 300f;
			rt.anchoredPosition = pos;

			Image image = buttonGo.GetComponent<Image>();
			image.color = new Color(0.12f, 0.12f, 0.12f, 0.9f);

			// Label
			GameObject labelGo = new GameObject("Label", typeof(RectTransform), typeof(Text));
			labelGo.transform.SetParent(buttonGo.transform, false);
			RectTransform lrt = labelGo.GetComponent<RectTransform>();
			lrt.anchorMin = new Vector2(0, 0);
			lrt.anchorMax = new Vector2(1, 1);
			lrt.offsetMin = new Vector2(8, 8);
			lrt.offsetMax = new Vector2(-8, -8);
			Text label = labelGo.GetComponent<Text>();
			label.text = option.ToString();
			label.color = Color.white;
			label.alignment = TextAnchor.MiddleCenter;
			label.fontSize = 24;
			if (defaultFont != null) label.font = defaultFont;

			Button button = buttonGo.GetComponent<Button>();
			button.onClick.AddListener(() => OnSelect(option));
		}

		private void OnSelect(Option option)
		{
			if (!_isVisible) return;
			EnsureRefs();
			if (_tokenBank == null || !_tokenBank.Spend(1))
			{
				if (showDebugInfo) Debug.Log("SabotageWheel: Not enough tokens to spend.");
				Hide();
				return;
			}

			switch (option)
			{
				case Option.Fog:
					_sabotageManager?.ApplyFog();
					break;
				case Option.TimeDrain:
					_sabotageManager?.ApplyTimeDrain(5f);
					break;
				case Option.FakeClues:
					_sabotageManager?.ApplyFakeClues(5f, 6);
					break;
			}

			Hide();
		}
	}
}

