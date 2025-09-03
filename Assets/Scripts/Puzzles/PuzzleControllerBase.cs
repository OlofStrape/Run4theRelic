using UnityEngine;
using Run4theRelic.Core;
using Run4theRelic.UI;           // SabotageWheel
using Run4theRelic.Sabotage;     // TokenBank + Manager

namespace Run4theRelic.Puzzles
{
    /// <summary>Base puzzle with a simple countdown timer and completion hooks.</summary>
    public abstract class PuzzleControllerBase : MonoBehaviour
    {
        [SerializeField] protected float timeLimit = 60f;
        [SerializeField, Tooltip("Completion within this time triggers 'gold'. Default = 50% of timeLimit.")]
        protected float goldTimeFraction = 0.5f;

        // === Nya kopplingar (kan lämnas tomma i Inspector; vi hittar dem automatiskt) ===
        [Header("Integration (optional)")]
        [SerializeField] SabotageTokenBank tokenBank;
        [SerializeField] SabotageWheel sabotageWheel;
        [SerializeField] SabotageManager sabotageManager;

        protected float _timer;
        bool _running;
        protected bool _isCompleted;
        protected bool _isFailed;

        public static PuzzleControllerBase Active { get; private set; }

        protected float GoldTime => timeLimit * Mathf.Clamp01(goldTimeFraction);

        // Back-compat properties
        public float CurrentTime => _timer;
        public bool IsActive => _running;
        public bool IsCompleted => _isCompleted;
        public bool IsFailed => _isFailed;

        protected virtual void Awake()
        {
            if (!tokenBank) tokenBank = FindObjectOfType<SabotageTokenBank>(true);
            if (!sabotageWheel) sabotageWheel = FindObjectOfType<SabotageWheel>(true);
            if (!sabotageManager) sabotageManager = FindObjectOfType<SabotageManager>(true);
        }

        protected virtual void Start()
        {
            _timer = timeLimit;
            _running = true;
            _isCompleted = false;
            _isFailed = false;
            Active = this;
            OnStartPuzzle();
            OnPuzzleStart(); // back-compat
            // Init tick för HUD/pace
            GameEvents.TriggerPuzzleTimerTick(Mathf.CeilToInt(_timer), Mathf.CeilToInt(timeLimit));
        }

        protected virtual void Update()
        {
            if (!_running) return;
            _timer -= Time.deltaTime;

            // Skicka sekundticks till HUD
            int secondsRemaining = Mathf.Max(0, Mathf.CeilToInt(_timer));
            int secondsLimit = Mathf.CeilToInt(timeLimit);
            GameEvents.TriggerPuzzleTimerTick(secondsRemaining, secondsLimit);

            if (_timer <= 0f)
            {
                _running = false;
                _isFailed = true;
                if (Active == this) Active = null;
                OnFailed();
                OnPuzzleFailed(); // back-compat
            }
        }

        /// <summary>Call when the puzzle is solved successfully.</summary>
        protected void Complete()
        {
            if (!_running) return;

            _running = false;
            _isCompleted = true;
            float clearTime = timeLimit - Mathf.Max(_timer, 0f);
            bool gold = clearTime <= GoldTime + 0.0001f;

            // Notify core
            GameEvents.TriggerPuzzleCompleted(-1, clearTime);

            // GOLD → token + valhjul (Fog/TimeDrain/FakeClues)
            if (gold)
            {
                if (tokenBank) tokenBank.Add(1);

                if (sabotageWheel && sabotageManager)
                {
                    var options = new[] { SabotageWheel.Option.Fog, SabotageWheel.Option.TimeDrain, SabotageWheel.Option.FakeClues };
                    sabotageWheel.Show(options);
                }
            }

            if (Active == this) Active = null;
            OnCompletePuzzle(clearTime, gold);
            OnPuzzleComplete(); // back-compat
        }

        // === Sabotage-hooks som Manager kan anropa ===

        /// <summary>Dra av sekunder från timern (kan anropas av SabotageManager).</summary>
        public virtual void ApplyTimeDrain(float seconds)
        {
            if (!_running || seconds <= 0f) return;
            _timer -= seconds;
            // Skicka direkt tick så HUD uppdateras
            GameEvents.TriggerPuzzleTimerTick(Mathf.Max(0, Mathf.CeilToInt(_timer)), Mathf.CeilToInt(timeLimit));
            Debug.Log($"[{name}] TimeDrain: -{seconds:0.##}s → {_timer:0.##}s kvar");
        }

        /// <summary>Stub för att spawna "falska ledtrådar". Override:a i specifika pussel om du vill göra mer.</summary>
        public virtual void SpawnFakeClues(float durationSeconds)
        {
            Debug.Log($"[{name}] FakeClues for {durationSeconds:0.##}s (stub). Override in puzzle to implement visuals/logic.");
        }

        // === Hooks att override:a i pussel ===
        protected virtual void OnStartPuzzle() { }
        protected virtual void OnCompletePuzzle(float clearTime, bool gold) { }
        protected virtual void OnFailed() { Debug.LogWarning($"{name}: puzzle failed (timeout)"); }

        // === Back-compat hooks (existing subclasses override these) ===
        protected virtual void OnPuzzleStart() { }
        protected virtual void OnPuzzleComplete() { }
        protected virtual void OnPuzzleFailed() { }
        protected virtual void OnPuzzleReset() { }

        // Back-compat API used by older SabotageManager implementations
        public void ReduceTimeRemaining(float seconds)
        {
            ApplyTimeDrain(seconds);
        }
    }
}