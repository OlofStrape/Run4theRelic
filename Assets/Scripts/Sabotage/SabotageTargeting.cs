using System.Linq;
using UnityEngine;
using Run4theRelic.Core;

namespace Run4theRelic.Sabotage
{
	/// <summary>
	/// Targeting helpers for choosing opponents to sabotage in dev/editor.
	/// Networking/progression data will be wired later.
	/// </summary>
	public static class SabotageTargeting
	{
		/// <summary>
		/// Pick a random opponent that is not the given self id.
		/// Returns -1 if none available.
		/// </summary>
		public static int PickRandomOpponent(int selfId)
		{
			var opponents = PlayerRegistry.GetOpponents(selfId);
			if (opponents == null || opponents.Count == 0) return -1;
			int index = Random.Range(0, opponents.Count);
			return opponents[index].id;
		}

		/// <summary>
		/// Pick the "leading" opponent by progression. Stub: returns first registered not self.
		/// Later, wire actual match/progress metrics and network ids.
		/// </summary>
		public static int PickLeadingOpponent(int selfId)
		{
			foreach (var info in PlayerRegistry.EnumerateInRegistrationOrder())
			{
				if (info.id != selfId)
				{
					return info.id;
				}
			}
			return -1;
		}
	}
}

