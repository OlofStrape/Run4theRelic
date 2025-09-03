using UnityEngine;

namespace Run4theRelic.UI
{
	/// <summary>
	/// Scriptable theme definition for UI colors and optional font references.
	/// </summary>
	[CreateAssetMenu(fileName = "ThemeConfig", menuName = "Run4theRelic/Theme Config", order = 10)]
	public class ThemeConfig : ScriptableObject
	{
		[Header("Colors")]
		public Color textPrimary = Color.white;
		public Color textSecondary = new Color(0.8f, 0.9f, 1f);
		public Color accent = new Color(0.2f, 0.8f, 1f);
		public Color warning = new Color(1f, 0.85f, 0.25f);
		public Color danger = new Color(1f, 0.4f, 0.4f);

		[Header("Backgrounds")]
		public Color backgroundPrimary = new Color(0.04f, 0.05f, 0.08f, 0.95f); // Deep space blue-black
		public Color backgroundSecondary = new Color(0.08f, 0.10f, 0.14f, 0.85f);
		public Color hudPanelTint = new Color(0.10f, 0.14f, 0.18f, 0.55f); // Semi-transparent panel
		public Color runeAccent = new Color(0.50f, 0.85f, 1f, 0.8f); // Frosty cyan for runes
		public Sprite panelSprite; // Optional 9-sliced sprite for panels

		[Header("UI Effects")]
		public bool enableTextOutline = true;
		public Color outlineColor = new Color(0.35f, 0.85f, 1f, 0.55f);
		public Vector2 outlineDistance = new Vector2(1f, -1f);
		public bool enableTextShadow = true;
		public Color shadowColor = new Color(0f, 0f, 0f, 0.6f);
		public Vector2 shadowDistance = new Vector2(2f, -2f);

		[Header("Fonts (optional)")]
		public Font fallbackFont;
	}
}

