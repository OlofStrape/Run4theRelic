using UnityEngine;
using Run4theRelic.Core;
using System.Text;

namespace Run4theRelic.Core
{
	public class StatsTracker : MonoBehaviour
	{
		[Range(0.1f,0.9f)] public float goldTimeFraction = 0.5f;

		float _matchStartTime;
		float _totalTime;
		int _goldCount;
		int _sabotageCount;
		int _puzzleIndex; // 1..3
		float[] _puzzleTimes = new float[3];
		int _currentSecondsLimit;

		void OnEnable()
		{
			GameEvents.OnPhaseChanged += OnPhaseChanged;
			GameEvents.OnPuzzleTimerTick += OnTick;
			GameEvents.OnPuzzleCompleted += OnPuzzleCompleted;
			GameEvents.OnSabotaged += OnSabotaged;
			GameEvents.OnRelicExtracted += OnRelicExtracted;
		}
		void OnDisable()
		{
			GameEvents.OnPhaseChanged -= OnPhaseChanged;
			GameEvents.OnPuzzleTimerTick -= OnTick;
			GameEvents.OnPuzzleCompleted -= OnPuzzleCompleted;
			GameEvents.OnSabotaged -= OnSabotaged;
			GameEvents.OnRelicExtracted -= OnRelicExtracted;
		}

		void OnPhaseChanged(MatchPhase phase, float duration)
		{
			if (phase == MatchPhase.Countdown) _matchStartTime = Time.time;
			if (phase == MatchPhase.Puzzle1) _puzzleIndex = 1;
			if (phase == MatchPhase.Puzzle2) _puzzleIndex = 2;
			if (phase == MatchPhase.Puzzle3) _puzzleIndex = 3;
		}

		void OnTick(int secondsRemaining, int secondsLimit) => _currentSecondsLimit = secondsLimit;

		void OnPuzzleCompleted(int playerId, float clearTime)
		{
			if (_puzzleIndex is >=1 and <=3)
			{
				_puzzleTimes[_puzzleIndex-1] = clearTime;
				float goldThreshold = _currentSecondsLimit > 0 ? _currentSecondsLimit * goldTimeFraction : 0f;
				if (goldThreshold > 0f && clearTime <= goldThreshold + 0.0001f) _goldCount++;
			}
		}

		void OnSabotaged(string _, float __) => _sabotageCount++;

		void OnRelicExtracted(int _)
		{
			_totalTime = Time.time - _matchStartTime;
			// Lås in resultat – valfritt
		}

		public string GetSummaryText()
		{
			var sb = new StringBuilder();
			sb.AppendLine("RESULT");
			sb.AppendLine($"Puzzle 1: {Format(_puzzleTimes[0])}");
			sb.AppendLine($"Puzzle 2: {Format(_puzzleTimes[1])}");
			sb.AppendLine($"Puzzle 3: {Format(_puzzleTimes[2])}");
			sb.AppendLine($"Gold clears: {_goldCount}");
			sb.AppendLine($"Sabotages used: {_sabotageCount}");
			sb.AppendLine($"Total: {Format(_totalTime)}");
			return sb.ToString();
		}

		static string Format(float t)
		{
			if (t <= 0f) return "--:--";
			int m = Mathf.FloorToInt(t / 60f);
			int s = Mathf.FloorToInt(t % 60f);
			return $"{m:00}:{s:00}";
		}
	}
}

