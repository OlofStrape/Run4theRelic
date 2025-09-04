using UnityEngine;

namespace Run4theRelic.Puzzles.RelicPlacement.ColorRuneSwap
{
	public enum ColorRuneKey { Red, Green, Blue, Yellow, Magenta, Cyan }

	/// <summary>
	/// Relic representing a colored rune. Supports locking after snap and exposes the rune key.
	/// </summary>
	[RequireComponent(typeof(Collider))]
	public class ColorRuneRelic : MonoBehaviour
	{
		[SerializeField] private ColorRuneKey key = ColorRuneKey.Red;
		public ColorRuneKey Key => key;

		public bool IsLocked { get; private set; }

		public void Lock()
		{
			IsLocked = true;
			var col = GetComponent<Collider>();
			if (col) col.enabled = false;
			var rb = GetComponent<Rigidbody>();
			if (rb) rb.isKinematic = true;
		}
	}
}

