using UnityEngine;
using UnityEngine.UI;

namespace Run4theRelic.UI
{
	/// <summary>
	/// Applies a ThemeConfig to a world-space HUD canvas hierarchy.
	/// Sets panel backgrounds, text colors, font, and basic outline/shadow effects.
	/// Attach this to the same GameObject as the Canvas created by UIBootstrap.
	/// </summary>
	public class ThemeApplier : MonoBehaviour
	{
		[SerializeField] private ThemeConfig theme;
		[SerializeField] private bool applyOnAwake = true;

		void Awake()
		{
			if (applyOnAwake)
			{
				ApplyTheme();
			}
		}

		public void ApplyTheme()
		{
			if (!theme) return;

			ApplyPanelBackgrounds();
			ApplyTextStyling();
		}

		void ApplyPanelBackgrounds()
		{
			// Create or update a background panel under root if missing
			var root = GetComponent<RectTransform>();
			if (!root) return;

			var bgName = "HUD_Background";
			Transform existing = transform.Find(bgName);
			GameObject bg;
			if (existing == null)
			{
				bg = new GameObject(bgName, typeof(RectTransform), typeof(Image));
				bg.transform.SetAsFirstSibling();
				bg.transform.SetParent(transform, false);
			}
			else
			{
				bg = existing.gameObject;
			}

			var rt = bg.GetComponent<RectTransform>();
			rt.anchorMin = new Vector2(0, 0);
			rt.anchorMax = new Vector2(1, 1);
			rt.offsetMin = Vector2.zero;
			rt.offsetMax = Vector2.zero;

			var img = bg.GetComponent<Image>();
			img.color = theme.hudPanelTint;
			img.sprite = theme.panelSprite;
			img.type = theme.panelSprite ? Image.Type.Sliced : Image.Type.Simple;
		}

		void ApplyTextStyling()
		{
			var texts = GetComponentsInChildren<Text>(true);
			foreach (var t in texts)
			{
				// Preserve explicit colors for specialized elements; only update if default white
				if (ApproximatelyWhite(t.color))
				{
					t.color = theme.textPrimary;
				}
				if (theme.fallbackFont)
				{
					t.font = theme.fallbackFont;
				}

				// Optional effects
				ApplyOutlineIfNeeded(t.gameObject);
				ApplyShadowIfNeeded(t.gameObject);
			}
		}

		void ApplyOutlineIfNeeded(GameObject go)
		{
			var outline = go.GetComponent<Outline>();
			if (theme.enableTextOutline)
			{
				if (!outline) outline = go.AddComponent<Outline>();
				outline.effectColor = theme.outlineColor;
				outline.effectDistance = theme.outlineDistance;
			}
			else if (outline)
			{
				Destroy(outline);
			}
		}

		void ApplyShadowIfNeeded(GameObject go)
		{
			var shadow = go.GetComponent<Shadow>();
			if (theme.enableTextShadow)
			{
				if (!shadow) shadow = go.AddComponent<Shadow>();
				shadow.effectColor = theme.shadowColor;
				shadow.effectDistance = theme.shadowDistance;
			}
			else if (shadow)
			{
				Destroy(shadow);
			}
		}

		bool ApproximatelyWhite(Color c)
		{
			return Mathf.Abs(c.r - 1f) < 0.02f && Mathf.Abs(c.g - 1f) < 0.02f && Mathf.Abs(c.b - 1f) < 0.02f;
		}
	}
}

