using System.Collections.Generic;
using UnityEngine;
using Run4theRelic.Sabotage;

namespace Run4theRelic.Core
{
	/// <summary>
	/// Central registry for players in the current scene. In dev/editor, this is populated
	/// by the simulated spawner. Networking integration can later feed real players here.
	/// </summary>
	public static class PlayerRegistry
	{
		public class PlayerInfo
		{
			public int id;
			public string name;
			public bool isLocal;
			public Transform root;
			public SabotageReceiver receiver;
		}

		private static readonly Dictionary<int, PlayerInfo> _idToInfo = new Dictionary<int, PlayerInfo>();
		private static readonly List<int> _registrationOrder = new List<int>();

		/// <summary>
		/// Register a player and its primary components for sabotage/targeting.
		/// </summary>
		public static void Register(PlayerInfo info)
		{
			if (info == null || info.root == null)
			{
				Debug.LogWarning("PlayerRegistry.Register: invalid PlayerInfo provided");
				return;
			}

			_idToInfo[info.id] = info;
			if (!_registrationOrder.Contains(info.id))
			{
				_registrationOrder.Add(info.id);
			}
		}

		/// <summary>
		/// Try get player info by id.
		/// </summary>
		public static bool TryGet(int id, out PlayerInfo info)
		{
			return _idToInfo.TryGetValue(id, out info);
		}

		/// <summary>
		/// Get the SabotageReceiver for a player id, or null if missing.
		/// </summary>
		public static SabotageReceiver GetReceiver(int id)
		{
			return _idToInfo.TryGetValue(id, out var info) ? info.receiver : null;
		}

		/// <summary>
		/// Get a list of opponents (everyone except selfId).
		/// </summary>
		public static List<PlayerInfo> GetOpponents(int selfId)
		{
			var results = new List<PlayerInfo>();
			foreach (var kv in _idToInfo)
			{
				if (kv.Key != selfId)
				{
					results.Add(kv.Value);
				}
			}
			return results;
		}

		/// <summary>
		/// Try to get the local player (dev/editor multi-setup chooses one local).
		/// </summary>
		public static bool TryGetLocal(out PlayerInfo local)
		{
			foreach (var kv in _idToInfo)
			{
				if (kv.Value.isLocal)
				{
					local = kv.Value;
					return true;
				}
			}
			local = null;
			return false;
		}

		/// <summary>
		/// Internal helper for deterministic ordering (used by leading-target stub).
		/// </summary>
		internal static IEnumerable<PlayerInfo> EnumerateInRegistrationOrder()
		{
			for (int i = 0; i < _registrationOrder.Count; i++)
			{
				int id = _registrationOrder[i];
				if (_idToInfo.TryGetValue(id, out var info))
				{
					yield return info;
				}
			}
		}
	}
}

