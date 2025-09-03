using UnityEngine;
using Run4theRelic.Core;

namespace Run4theRelic.Core
{
	/// <summary>
	/// En enkel gate som öppnar/stänger en dörr (GameObject) när given fas uppnås.
	/// </summary>
	public class PuzzleGate : MonoBehaviour
	{
		[Header("Gate Settings")]
		public MatchPhase gateForPhase = MatchPhase.Puzzle1;
		public GameObject door;
		public bool openOnPhase = true;

		void OnEnable()
		{
			GameEvents.OnMatchPhaseChanged += HandlePhase;
		}

		void OnDisable()
		{
			GameEvents.OnMatchPhaseChanged -= HandlePhase;
		}

		void Start()
		{
			var orchestrator = FindFirstObjectByType<MatchOrchestrator>();
			HandlePhase(orchestrator != null ? orchestrator.CurrentPhase : MatchPhase.Lobby);
		}

		void HandlePhase(MatchPhase phase)
		{
			if (door == null) return;
			bool isTarget = phase == gateForPhase;
			door.SetActive(openOnPhase ? !isTarget : isTarget);
		}
	}
}

