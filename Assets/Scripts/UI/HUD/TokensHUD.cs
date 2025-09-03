using UnityEngine;
using UnityEngine.UI;
using Run4theRelic.Sabotage;

namespace Run4theRelic.UI
{
	/// <summary>
	/// Displays sabotage token count and updates on change.
	/// </summary>
	public class TokensHUD : MonoBehaviour
	{
		[SerializeField] private Text targetText;
		[SerializeField] private string prefix = "TOKENS ";
		[SerializeField] private Color textColor = Color.cyan;
		[SerializeField] private ThemeConfig themeConfig;

		void Awake()
		{
			if (!targetText)
				targetText = GetComponentInChildren<Text>(true);
			if (themeConfig)
			{
				textColor = themeConfig.accent;
			}
		}

		void OnEnable()
		{
			if (SabotageTokenBank.Instance != null)
			{
				SabotageTokenBank.Instance.OnTokensChanged += HandleChanged;
				HandleChanged(SabotageTokenBank.Instance.CurrentTokens);
			}
		}

		void OnDisable()
		{
			if (SabotageTokenBank.Instance != null)
			{
				SabotageTokenBank.Instance.OnTokensChanged -= HandleChanged;
			}
		}

		void HandleChanged(int tokens)
		{
			if (!targetText) return;
			targetText.text = prefix + tokens;
			targetText.color = textColor;
		}
	}
}

