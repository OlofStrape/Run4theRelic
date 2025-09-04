using UnityEngine;
using Run4theRelic.Core;

namespace Run4theRelic.Puzzles.RelicPlacement
{
    /// <summary>
    /// RLP-101 Triad Slots: Place three relics in matching shaped slots. Auto-snap near slot.
    /// Completes when all three are correctly placed. Wrong placement flashes red for 0.5s.
    /// </summary>
    public class TriadSlotsController : Puzzles.PuzzleControllerBase
    {
        [Header("Triad Slots Setup")]
        [SerializeField] private TriadSlot[] slots = new TriadSlot[3];
        [SerializeField] private TriadRelic[] relics = new TriadRelic[3];

        [Header("Feedback")]
        [SerializeField] private float wrongFlashDuration = 0.5f;

        [Header("AI Tuning")]
        [SerializeField] private float baseSnapDistance = 0.2f;
        [SerializeField] private float minSnapDistance = 0.1f;
        [SerializeField] private float maxSnapDistance = 0.35f;

        private VRPerformanceAnalytics analytics;
        private VRAIGameplayDirector aiDirector;

        protected override void Awake()
        {
            base.Awake();
            analytics = GameObject.FindFirstObjectByType<VRPerformanceAnalytics>(FindObjectsInactive.Include);
            aiDirector = GameObject.FindFirstObjectByType<VRAIGameplayDirector>(FindObjectsInactive.Include);
        }

        protected override void OnStartPuzzle()
        {
            float difficulty = aiDirector != null ? aiDirector.GetCurrentDifficulty() : 5f;
            float t = Mathf.InverseLerp(1f, 10f, difficulty);
            float snapDist = Mathf.Lerp(maxSnapDistance, minSnapDistance, t);
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == null) continue;
                slots[i].Configure(this, snapDist, wrongFlashDuration);
                slots[i].OnSlotResolved -= HandleSlotResolved;
                slots[i].OnSlotResolved += HandleSlotResolved;
            }

            GameEvents.TriggerPuzzleStarted(name);
        }

        private void HandleSlotResolved(TriadSlot slot, bool correct)
        {
            if (!correct)
            {
                // Nothing more to do here; visual handled by slot
                return;
            }

            // Check completion: all slots occupied correctly
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == null || !slots[i].IsCorrectlyOccupied)
                {
                    return;
                }
            }

            // All good → complete
            Complete();
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

        public void ValidatePlacement(TriadSlot slot, TriadRelic relic)
        {
            if (!_running || _isCompleted || _isFailed) return;

            bool correct = slot != null && relic != null && slot.AcceptsRelic(relic);
            slot?.ResolvePlacement(relic, correct);
        }
    }
}

