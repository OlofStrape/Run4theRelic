using UnityEngine;

namespace Run4theRelic.Core
{
	/// <summary>
	/// Simple helper to auto-start the match on Play for MVP testing.
	/// Attach this to any GameObject in the scene.
	/// </summary>
	public class AutoStartMatch : MonoBehaviour
	{
		[SerializeField] private float delaySeconds = 0.25f;
		MatchOrchestrator _orchestrator;

		void Awake()
		{
			_orchestrator = FindFirstObjectByType<MatchOrchestrator>();
		}

		void Start()
		{
			if (_orchestrator != null)
			{
				Invoke(nameof(Begin), Mathf.Max(0f, delaySeconds));
			}
			else
			{
				Debug.LogWarning("AutoStartMatch: No MatchOrchestrator found in scene.");
			}
		}

		void Begin()
		{
			if (_orchestrator != null && !_orchestrator.IsMatchActive)
			{
				_orchestrator.StartMatch();
			}
		}
	}
}

