using UnityEngine;
using UnityEngine.UI;
using Run4theRelic.Core;

namespace Run4theRelic.UI
{
	/// <summary>
	/// Displays the active puzzle timer with simple color feedback.
	/// Subscribes to GameEvents.OnPuzzleTimerTick.
	/// </summary>
	public class TimerHUD : MonoBehaviour
	{
		[SerializeField] private Text targetText;
		[SerializeField] private Color normalColor = Color.white;
		[SerializeField] private Color warningColor = new Color(1f, 0.85f, 0.25f);
		[SerializeField] private Color dangerColor = new Color(1f, 0.4f, 0.4f);
		[SerializeField] private ThemeConfig themeConfig;
		[SerializeField] [Range(0.05f, 0.5f)] private float warningThreshold = 0.3f;
		[SerializeField] [Range(0.01f, 0.3f)] private float dangerThreshold = 0.1f;

		void Awake()
		{
			if (!targetText)
			{
				// Try find a child Text (e.g., from UIBootstrap TimerRoot/Text)
				targetText = GetComponentInChildren<Text>(true);
			}
			if (themeConfig)
			{
				normalColor = themeConfig.textPrimary;
				warningColor = themeConfig.warning;
				dangerColor = themeConfig.danger;
			}
		}

		void OnEnable()
		{
			GameEvents.OnPuzzleTimerTick += HandleTick;
		}

		void OnDisable()
		{
			GameEvents.OnPuzzleTimerTick -= HandleTick;
		}

		void HandleTick(int secondsRemaining, int secondsLimit)
		{
			if (!targetText) return;
			targetText.text = FormatTime(secondsRemaining);

			float fraction = secondsLimit > 0 ? Mathf.Clamp01((float)secondsRemaining / secondsLimit) : 0f;
			if (fraction <= dangerThreshold)
			{
				targetText.color = dangerColor;
			}
			else if (fraction <= warningThreshold)
			{
				targetText.color = warningColor;
			}
			else
			{
				targetText.color = normalColor;
			}
		}

		static string FormatTime(int seconds)
		{
			seconds = Mathf.Max(0, seconds);
			int m = seconds / 60;
			int s = seconds % 60;
			return m > 0 ? $"{m}:{s:00}" : s.ToString();
		}
	}
}

