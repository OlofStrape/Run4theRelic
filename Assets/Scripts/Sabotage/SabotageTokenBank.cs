using System;
using UnityEngine;

namespace Run4theRelic.Sabotage
{
	/// <summary>
	/// Central bank for sabotage tokens. Emits OnTokensChanged when balance updates.
	/// </summary>
	public class SabotageTokenBank : MonoBehaviour
	{
		public static SabotageTokenBank Instance { get; private set; }
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
		/// <summary>
		/// Alias för nuvarande antal tokens.
		/// </summary>
		public int Tokens => _currentTokens;

		private void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(gameObject);
				return;
			}
			Instance = this;
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
		/// Återställ antal tokens till ett givet värde.
		/// </summary>
		public void ResetTokens(int value = 0)
		{
			_currentTokens = Mathf.Max(0, value);
			OnTokensChanged?.Invoke(_currentTokens);
			if (showDebugInfo)
			{
				Debug.Log($"SabotageTokenBank: reset -> {_currentTokens}");
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

		/// <summary>
		/// Kan vi spendera angivet antal tokens?
		/// </summary>
		public bool CanSpend(int amount = 1) => amount > 0 && _currentTokens >= amount;
	}
}

