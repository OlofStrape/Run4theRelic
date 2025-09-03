using UnityEngine;
using System.Collections.Generic;
using Run4theRelic.Core;

namespace Run4theRelic.Core
{
	/// <summary>
	/// Togglar aktiva rum efter MatchPhase så du slipper göra det manuellt.
	/// </summary>
	public class PhaseActivator : MonoBehaviour
	{
		[System.Serializable]
		public struct PhaseRoom
		{
			public MatchPhase phase;
			public GameObject root;
		}

		[Header("Rooms per phase")]
		public List<PhaseRoom> rooms = new();

		void OnEnable()
		{
			GameEvents.OnMatchPhaseChanged += HandlePhaseChanged;
		}
		void OnDisable()
		{
			GameEvents.OnMatchPhaseChanged -= HandlePhaseChanged;
		}

		void Start()
		{
			var orchestrator = FindFirstObjectByType<MatchOrchestrator>();
			HandlePhaseChanged(orchestrator != null ? orchestrator.CurrentPhase : MatchPhase.Lobby);
		}

		void HandlePhaseChanged(MatchPhase phase)
		{
			for (int i = 0; i < rooms.Count; i++)
			{
				var pr = rooms[i];
				if (pr.root)
				{
					bool shouldBeActive = pr.phase == phase || (phase == MatchPhase.Final && pr.phase == MatchPhase.Final);
					pr.root.SetActive(shouldBeActive);
				}
			}
		}
	}
}

