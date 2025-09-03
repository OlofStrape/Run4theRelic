using UnityEngine;
using Run4theRelic.Core;
using Run4theRelic.Sabotage;

namespace Run4theRelic.Dev
{
	/// <summary>
	/// Editor/dev-only: spawns N simple capsule players with SabotageReceiver and registers them.
	/// One will be flagged as local for HUD event routing.
	/// </summary>
	public class SimulatedOpponentSpawner : MonoBehaviour
	{
		[Range(2, 4)] public int numPlayers = 3;
		public int localPlayerIndex = 0; // 0..numPlayers-1

		private void Start()
		{
			SpawnAndRegister();
		}

		private void SpawnAndRegister()
		{
			for (int i = 0; i < numPlayers; i++)
			{
				var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
				go.name = $"SimPlayer_{i+1}";
				go.transform.position = transform.position + new Vector3(i * 1.5f, 0f, 0f);
				var receiver = go.AddComponent<SabotageReceiver>();
				receiver.ConfigureLocal(i == localPlayerIndex);

				var info = new PlayerRegistry.PlayerInfo
				{
					id = i + 1,
					name = go.name,
					isLocal = i == localPlayerIndex,
					root = go.transform,
					receiver = receiver
				};
				PlayerRegistry.Register(info);
			}
		}
	}
}

