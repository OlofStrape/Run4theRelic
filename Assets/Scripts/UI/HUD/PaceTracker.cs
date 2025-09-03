using UnityEngine;
using System;
using Run4theRelic.Core;

namespace Run4theRelic.UI
{
	/// <summary>
	/// Räknar ut om spelaren ligger före/efter "gold time" för aktuellt pussel
	/// och skickar ut ett enkelt pace-värde i sekunder (+ = före, - = efter).
	/// Kopplar mot GameEvents.OnPuzzleTimerTick (som skickas varje sekund).
	/// </summary>
	public class PaceTracker : MonoBehaviour
	{
		[Tooltip("Andel av timern som räknas som 'gold'. 0.5 = 50% av timeLimit.")]
		[Range(0.1f, 0.9f)]
		public float goldTimeFraction = 0.5f;

		/// <summary>
		/// Triggas när pace ändras. Argument: deltaSeconds (+/-).
		/// </summary>
		public event Action<int> OnPaceChanged;

		int _lastSecondsRemaining = int.MaxValue;
		int _secondsLimit = 0;
		int _lastDeltaReported = int.MinValue;

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
			_secondsLimit = secondsLimit;
			_lastSecondsRemaining = secondsRemaining;

			// Gold-time-gräns i sekunder (avrundat uppåt för säkerhets skull)
			int goldSeconds = Mathf.CeilToInt(_secondsLimit * Mathf.Clamp01(goldTimeFraction));

			// Pace = hur många sekunder före/efter gold-gränsen du ligger just nu
			int delta = secondsRemaining - goldSeconds;

			if (delta != _lastDeltaReported)
			{
				_lastDeltaReported = delta;
				OnPaceChanged?.Invoke(delta);
				Debug.Log($"[PaceTracker] Pace: {(delta >= 0 ? "+" : "")}{delta}s (gold @ {goldSeconds}s)");
			}
		}

		/// <summary>Senaste kända pace (kan vara int.MinValue om ej init).</summary>
		public int CurrentDeltaSeconds => _lastDeltaReported;

		/// <summary>Senaste kända limit (sekunder) för aktuell pussel-timer.</summary>
		public int CurrentSecondsLimit => _secondsLimit;
	}
}

