using UnityEngine;
using Run4theRelic.Core;

namespace Run4theRelic.UI
{
	/// <summary>Visar enkel textstatus för Relicen: At Pedestal / Carried / Extracted.</summary>
	public class RelicOwnerHUD : MonoBehaviour
	{
		[Tooltip("HUDController som ritar text; om null hämtas första i scenen.")]
		public HUDController hud;

		string _state = "At Pedestal";

		void Awake()
		{
			if (!hud) hud = FindFirstObjectByType<HUDController>();
		}

		void OnEnable()
		{
			GameEvents.OnRelicPickedUp += OnPicked;
			GameEvents.OnRelicDropped += OnDropped;
			GameEvents.OnRelicExtracted += OnExtracted;
		}
		void OnDisable()
		{
			GameEvents.OnRelicPickedUp -= OnPicked;
			GameEvents.OnRelicDropped -= OnDropped;
			GameEvents.OnRelicExtracted -= OnExtracted;
		}

		void OnPicked(int playerId)
		{
			_state = "Carried";
			hud?.ShowMessage("RELIC CARRIED!", 1.5f);
		}

		void OnDropped(int playerId)
		{
			_state = "At Pedestal";
			hud?.ShowMessage("RELIC DROPPED", 1.2f);
		}

		void OnExtracted(int playerId)
		{
			_state = "Extracted";
			hud?.ShowMessage("EXTRACTED!", 2.0f);
		}

		void LateUpdate()
		{
			// Minimal MVP: rely on ShowMessage flashes; extend HUDController if needed.
		}
	}
}

