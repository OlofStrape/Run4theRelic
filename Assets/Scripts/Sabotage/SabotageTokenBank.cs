using System;
using UnityEngine;

namespace Run4theRelic.Sabotage
{
	/// <summary>
	/// Central bank for sabotage tokens. Emits OnTokensChanged when balance updates.
	/// </summary>
	public class SabotageTokenBank : MonoBehaviour
	{
		[SerializeField] private int startingTokens = 0;
		[SerializeField] private bool showDebugInfo = true;
		private int _currentTokens;

		/// <summary>
		/// Fired whenever the token count changes. Provides new token count.
		/// </summary>
		public event Action<int> OnTokensChanged;

		/// <summary>
		/// Current number of available tokens.
		/// </summary>
		public int CurrentTokens => _currentTokens;

		private void Awake()
		{
			_currentTokens = Mathf.Max(0, startingTokens);
		}

		/// <summary>
		/// Add tokens to the bank.
		/// </summary>
		public void Add(int amount)
		{
			if (amount <= 0) return;
			_currentTokens += amount;
			OnTokensChanged?.Invoke(_currentTokens);
			if (showDebugInfo)
			{
				Debug.Log($"SabotageTokenBank: +{amount} (total: {_currentTokens})");
			}
		}

		/// <summary>
		/// Try to spend tokens from the bank.
		/// </summary>
		/// <returns>True if the spend succeeded.</returns>
		public bool Spend(int amount)
		{
			if (amount <= 0) return true;
			if (_currentTokens < amount) return false;
			_currentTokens -= amount;
			OnTokensChanged?.Invoke(_currentTokens);
			if (showDebugInfo)
			{
				Debug.Log($"SabotageTokenBank: -{amount} (total: {_currentTokens})");
			}
			return true;
		}
	}
}

