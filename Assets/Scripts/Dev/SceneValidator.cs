using UnityEngine;

namespace Dev
{
	/// <summary>Varnar i Play om viktiga länkar saknas.</summary>
	public class SceneValidator : MonoBehaviour
	{
		void Start()
		{
			var orch = FindFirstObjectByType<Core.MatchOrchestrator>();
			if (!orch) Debug.LogWarning("[SceneValidator] MatchOrchestrator saknas.");
			else
			{
				if (!orch.transform) { }
				if (!orch.GetType().GetField("relicSpawn", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(orch))
					Debug.LogWarning("[SceneValidator] relicSpawn ej satt i MatchOrchestrator.");
			}

			if (!FindFirstObjectByType<Sabotage.SabotageManager>())
				Debug.LogWarning("[SceneValidator] SabotageManager saknas.");

			if (!FindFirstObjectByType<UI.HUD.HUDController>())
				Debug.LogWarning("[SceneValidator] HUDController saknas.");

			var uiBootstrap = FindFirstObjectByType<Run4theRelic.UI.UIBootstrap>();
			if (!uiBootstrap)
			{
				Debug.LogWarning("[SceneValidator] UIBootstrap saknas. Lägg till Run4theRelic.UI.UIBootstrap i scenen.");
			}
		}
	}
}

