using UnityEngine;
using UnityEngine.UI;

namespace Run4theRelic.UI
{
	/// <summary>
	/// Bootstraps a simple VR-friendly HUD as a world-space canvas parented to the main camera.
	/// Spawns child roots for timer/tokens and aligns a panel in front of the camera.
	/// </summary>
	public class UIBootstrap : MonoBehaviour
	{
		[SerializeField] private Vector3 localPosition = new Vector3(0, -0.1f, 0.6f);
		[SerializeField] private Vector2 canvasSize = new Vector2(800, 450);
		[SerializeField] private float pixelsPerUnit = 200f;
		[SerializeField] private Font fallbackFont;
		[SerializeField] private ThemeConfig themeConfig;

		private Canvas _canvas;
		private RectTransform _root;

		void Awake()
		{
			EnsureCanvas();
			CreateChildRoots();
			ApplyThemeIfAvailable();
		}

		void EnsureCanvas()
		{
			// Parent to main camera for stable VR attachment
			var cam = Camera.main;
			Transform parent = cam ? cam.transform : transform;

			if (GetComponent<Canvas>() == null)
			{
				_canvas = gameObject.AddComponent<Canvas>();
			}
			else
			{
				_canvas = GetComponent<Canvas>();
			}

			if (transform.parent != parent)
			{
				transform.SetParent(parent, false);
			}

			_canvas.renderMode = RenderMode.WorldSpace;
			_canvas.worldCamera = cam;
			if (GetComponent<GraphicRaycaster>() == null) gameObject.AddComponent<GraphicRaycaster>();

			var scaler = GetComponent<CanvasScaler>();
			if (scaler == null) scaler = gameObject.AddComponent<CanvasScaler>();
			scaler.dynamicPixelsPerUnit = pixelsPerUnit;

			_root = GetComponent<RectTransform>();
			_root.sizeDelta = canvasSize;
			_root.localPosition = localPosition;
			_root.localRotation = Quaternion.identity;
		}

		void ApplyThemeIfAvailable()
		{
			if (!themeConfig) return;
			var applier = GetComponent<ThemeApplier>();
			if (applier == null) applier = gameObject.AddComponent<ThemeApplier>();
			var so = new SerializedObject(applier);
			so.FindProperty("theme").objectReferenceValue = themeConfig;
			so.FindProperty("applyOnAwake").boolValue = true;
			so.ApplyModifiedPropertiesWithoutUndo();
		}

		void CreateChildRoots()
		{
			// Timer root (top center)
			CreateLabelRoot("TimerRoot", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-150, -60), new Vector2(150, -10), TextAnchor.UpperCenter, 36);
			// Tokens root (top right)
			CreateLabelRoot("TokensRoot", new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-220, -60), new Vector2(-10, -10), TextAnchor.UpperRight, 28);
		}

		GameObject CreateLabelRoot(string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax, TextAnchor align, int fontSize)
		{
			GameObject go = new GameObject(name, typeof(RectTransform));
			go.transform.SetParent(_root, false);
			var rt = go.GetComponent<RectTransform>();
			rt.anchorMin = anchorMin;
			rt.anchorMax = anchorMax;
			rt.offsetMin = offsetMin;
			rt.offsetMax = offsetMax;

			// Add a default Text child to easily hook up from HUD scripts
			var textGo = new GameObject("Text", typeof(RectTransform), typeof(Text));
			textGo.transform.SetParent(go.transform, false);
			var trt = textGo.GetComponent<RectTransform>();
			trt.anchorMin = new Vector2(0, 0);
			trt.anchorMax = new Vector2(1, 1);
			trt.offsetMin = new Vector2(0, 0);
			trt.offsetMax = new Vector2(0, 0);
			var text = textGo.GetComponent<Text>();
			text.alignment = align;
			text.fontSize = fontSize;
			text.text = "";
			text.color = Color.white;
			if (fallbackFont != null) text.font = fallbackFont;
			return go;
		}
	}
}

