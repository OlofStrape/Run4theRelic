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

		[Header("Fonts (optional)")]
		public Font fallbackFont;
	}
}

