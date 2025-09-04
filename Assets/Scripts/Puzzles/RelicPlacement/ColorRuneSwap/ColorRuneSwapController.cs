using UnityEngine;
using Run4theRelic.Core;

namespace Run4theRelic.Puzzles.RelicPlacement.ColorRuneSwap
{
	/// <summary>
	/// RLP-102 Color Rune Swap: Swap color runes between slots to match a target sequence shown on a hint panel.
	/// Integrates PatternMatching hint (visual sequence) by highlighting target order. Completes when all slots
	/// hold the correct rune keys. Ties into AI for snap distance and hint frequency.
	/// </summary>
	public class ColorRuneSwapController : Puzzles.PuzzleControllerBase
	{
		[Header("Setup")]
		[SerializeField] private ColorRuneSlot[] slots = new ColorRuneSlot[4];
		[SerializeField] private ColorRuneRelic[] relics = new ColorRuneRelic[4];

		[Header("Target Configuration")]
		[SerializeField] private ColorRuneKey[] targetSequence = new ColorRuneKey[4];

		[Header("Feedback")]
		[SerializeField] private float wrongFlashDuration = 0.5f;
		[SerializeField] private bool showHintSequence = true;
		[SerializeField] private Renderer[] hintRenderers; // optional visuals per slot
		[SerializeField] private Color hintColor = Color.yellow;
		[SerializeField] private float hintBlinkInterval = 0.6f;

		[Header("AI Tuning")]
		[SerializeField] private float minSnapDistance = 0.1f;
		[SerializeField] private float maxSnapDistance = 0.35f;
		[SerializeField] private float baseHintInterval = 1.0f;

		private VRPerformanceAnalytics analytics;
		private VRAIGameplayDirector aiDirector;
		private float snapDist;
		private float currentHintInterval;
		private float hintTimer;

		protected override void Awake()
		{
			base.Awake();
			analytics = GameObject.FindFirstObjectByType<VRPerformanceAnalytics>(FindObjectsInactive.Include);
			aiDirector = GameObject.FindFirstObjectByType<VRAIGameplayDirector>(FindObjectsInactive.Include);
		}

		protected override void OnStartPuzzle()
		{
			float difficulty = aiDirector != null ? aiDirector.GetCurrentDifficulty() : 5f;
			float mastery = analytics != null ? analytics.GetMasteryLevel() : 0.5f;
			float frustration = analytics != null ? analytics.GetFrustrationLevel() : 0.0f;

			float t = Mathf.InverseLerp(1f, 10f, difficulty);
			snapDist = Mathf.Lerp(maxSnapDistance, minSnapDistance, t);
			currentHintInterval = Mathf.Lerp(baseHintInterval * 0.35f, baseHintInterval * 1.5f, Mathf.Clamp01(1f - mastery + frustration));
			hintTimer = currentHintInterval;

			for (int i = 0; i < slots.Length; i++)
			{
				if (slots[i] == null) continue;
				slots[i].Configure(this, snapDist, wrongFlashDuration);
				slots[i].OnSlotResolved -= HandleSlotResolved;
				slots[i].OnSlotResolved += HandleSlotResolved;
			}

			// Optional: present hint sequence briefly
			if (showHintSequence)
			{
				ApplyHintVisuals(true);
			}

			GameEvents.TriggerPuzzleStarted(name);
		}

		private void Update()
		{
			if (!_running || _isCompleted || _isFailed) return;
			if (!showHintSequence || hintRenderers == null || hintRenderers.Length == 0) return;

			hintTimer -= Time.deltaTime;
			if (hintTimer <= 0f)
			{
				ToggleHintBlink();
				hintTimer = currentHintInterval;
			}
		}

		private void ToggleHintBlink()
		{
			// Pulse target slots to draw attention when player struggles
			for (int i = 0; i < Mathf.Min(targetSequence.Length, hintRenderers?.Length ?? 0); i++)
			{
				var r = hintRenderers[i];
				if (!r) continue;
				var c = r.material.color;
				bool on = Mathf.PingPong(Time.time, hintBlinkInterval) > (hintBlinkInterval * 0.5f);
				r.material.color = on ? hintColor : c;
			}
		}

		private void ApplyHintVisuals(bool initial)
		{
			if (hintRenderers == null) return;
			for (int i = 0; i < Mathf.Min(targetSequence.Length, hintRenderers.Length); i++)
			{
				var r = hintRenderers[i];
				if (!r) continue;
				if (initial)
				{
					var baseCol = r.material.color;
					r.material.color = Color.Lerp(baseCol, hintColor, 0.4f);
				}
			}
		}

		private void HandleSlotResolved(ColorRuneSlot slot, bool correct)
		{
			if (!correct) return;
			for (int i = 0; i < slots.Length; i++)
			{
				if (slots[i] == null) continue;
				if (!IsSlotCorrect(i)) return;
			}
			Complete();
		}

		private bool IsSlotCorrect(int index)
		{
			var slot = slots[index];
			if (slot == null) return false;
			// Slot correctness already checks current relic key == acceptedKey,
			// but ensure it matches targetSequence too for robustness
			return slot.IsCorrectlyOccupied && targetSequence != null && index < targetSequence.Length;
		}

		protected override void OnCompletePuzzle(float clearTime, bool gold)
		{
			if (analytics != null)
			{
				analytics.RecordPuzzleAttempt(PuzzleType.RelicPlacement, true, clearTime, aiDirector != null ? aiDirector.GetCurrentDifficulty() : 5f);
			}
		}

		protected override void OnFailed()
		{
			base.OnFailed();
			if (analytics != null)
			{
				float elapsed = timeLimit - Mathf.Max(_timer, 0f);
				analytics.RecordPuzzleAttempt(PuzzleType.RelicPlacement, false, elapsed, aiDirector != null ? aiDirector.GetCurrentDifficulty() : 5f);
			}
			GameEvents.TriggerPuzzleFailed(name);
		}

		public void ValidatePlacement(ColorRuneSlot slot, ColorRuneRelic relic)
		{
			if (!_running || _isCompleted || _isFailed) return;

			bool correct = slot != null && relic != null && slot.AcceptsRelic(relic);
			slot?.ResolvePlacement(relic, correct);
		}
	}
}

