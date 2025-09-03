using UnityEngine;
using Run4theRelic.Core;
using Run4theRelic.Sabotage;
using Run4theRelic.UI;

namespace Run4theRelic.Puzzles
{
    /// <summary>
    /// Abstrakt basklass för alla pussel med gemensam timer och completion-logik.
    /// Implementera OnPuzzleStart, OnPuzzleComplete och OnPuzzleFailed i subklasser.
    /// </summary>
    public abstract class PuzzleControllerBase : MonoBehaviour
    {
        [Header("Puzzle Settings")]
        [SerializeField] protected float timeLimit = 60f;
        [SerializeField] protected float goldTimeFraction = 0.5f;
        [SerializeField] protected bool autoStartOnEnable = true;
        
        [Header("Debug")]
        [SerializeField] protected bool showDebugInfo = true;
        
        protected float _currentTime;
        protected bool _isActive;
        protected bool _isCompleted;
        protected bool _isFailed;
        
        // Tick-hantering
        private float _nextTickAt;
        
        /// <summary>
        /// Aktuell tid kvar på pusslet.
        /// </summary>
        public float CurrentTime => _currentTime;
        
        /// <summary>
        /// Är pusslet aktivt och räknar ner.
        /// </summary>
        public bool IsActive => _isActive;
        
        /// <summary>
        /// Är pusslet löst.
        /// </summary>
        public bool IsCompleted => _isCompleted;
        
        /// <summary>
        /// Är pusslet misslyckat (tid slut).
        /// </summary>
        public bool IsFailed => _isFailed;
        
        /// <summary>
        /// Gold time threshold (tid för optimal completion).
        /// </summary>
        public float GoldTimeThreshold => timeLimit * goldTimeFraction;
        
        /// <summary>
        /// Är pusslet löst inom gold time.
        /// </summary>
        public bool IsGoldTimeCompletion => _isCompleted && _currentTime >= GoldTimeThreshold;
        
        protected virtual void OnEnable()
        {
            if (autoStartOnEnable)
            {
                StartPuzzle();
            }
        }
        
        protected virtual void Update()
        {
            if (!_isActive || _isCompleted || _isFailed) return;
            
            // Uppdatera timer
            _currentTime -= Time.deltaTime;
            
            // Sänd tick en gång per sekund (avrundat uppåt för återstående tid)
            if (Time.time >= _nextTickAt)
            {
                _nextTickAt = Mathf.Floor(Time.time) + 1f;
                int secondsRemaining = Mathf.CeilToInt(Mathf.Max(0f, _currentTime));
                int secondsLimit = Mathf.RoundToInt(timeLimit);
                GameEvents.TriggerPuzzleTimerTick(secondsRemaining, secondsLimit);
            }
            
            // Kontrollera om tiden är slut
            if (_currentTime <= 0f)
            {
                OnTimeExpired();
            }
            
            // Debug-info
            if (showDebugInfo)
            {
                Debug.Log($"Puzzle {gameObject.name}: {_currentTime:F1}s remaining");
            }
        }
        
        /// <summary>
        /// Startar pusslet och aktiverar timer.
        /// </summary>
        public virtual void StartPuzzle()
        {
            if (_isActive) return;
            
            _isActive = true;
            _isCompleted = false;
            _isFailed = false;
            _currentTime = timeLimit;
            _nextTickAt = Mathf.Floor(Time.time) + 1f;
            
            OnPuzzleStart();
            
            if (showDebugInfo)
            {
                Debug.Log($"Puzzle {gameObject.name} started with {timeLimit}s time limit");
            }
        }
        
        /// <summary>
        /// Pausar pusslet (timer stannar).
        /// </summary>
        public virtual void PausePuzzle()
        {
            if (!_isActive) return;
            
            _isActive = false;
            
            if (showDebugInfo)
            {
                Debug.Log($"Puzzle {gameObject.name} paused");
            }
        }
        
        /// <summary>
        /// Återupptar pausat pussel.
        /// </summary>
        public virtual void ResumePuzzle()
        {
            if (_isActive || _isCompleted || _isFailed) return;
            
            _isActive = true;
            
            if (showDebugInfo)
            {
                Debug.Log($"Puzzle {gameObject.name} resumed");
            }
        }
        
        /// <summary>
        /// Markerar pusslet som löst och triggar completion event.
        /// </summary>
        protected virtual void Complete()
        {
            if (_isCompleted || _isFailed) return;
            
            _isCompleted = true;
            _isActive = false;
            
            float clearTime = timeLimit - _currentTime;
            bool isGoldTime = IsGoldTimeCompletion;
            
            // Trigga completion event
            GameEvents.TriggerPuzzleCompleted(-1, clearTime); // -1 = singleplayer
            
            OnPuzzleComplete();
            
            if (isGoldTime)
            {
                var tokenBank = Object.FindObjectOfType<SabotageTokenBank>(true);
                if (tokenBank != null) tokenBank.Add(1);
                var wheel = Object.FindObjectOfType<SabotageWheel>(true);
                if (wheel != null)
                {
                    wheel.Show(new[]
                    {
                        SabotageWheel.Option.Fog,
                        SabotageWheel.Option.TimeDrain,
                        SabotageWheel.Option.FakeClues
                    });
                }
            }
            
            if (showDebugInfo)
            {
                string timeStatus = isGoldTime ? "GOLD TIME!" : "Normal completion";
                Debug.Log($"Puzzle {gameObject.name} completed in {clearTime:F2}s - {timeStatus}");
            }
        }
        
        /// <summary>
        /// Markerar pusslet som misslyckat.
        /// </summary>
        protected virtual void Fail()
        {
            if (_isCompleted || _isFailed) return;
            
            _isFailed = true;
            _isActive = false;
            
            OnPuzzleFailed();
            
            if (showDebugInfo)
            {
                Debug.Log($"Puzzle {gameObject.name} failed - time expired");
            }
        }
        
        /// <summary>
        /// Återställer pusslet till ursprungligt tillstånd.
        /// </summary>
        public virtual void ResetPuzzle()
        {
            _isActive = false;
            _isCompleted = false;
            _isFailed = false;
            _currentTime = timeLimit;
            
            OnPuzzleReset();
            
            if (showDebugInfo)
            {
                Debug.Log($"Puzzle {gameObject.name} reset");
            }
        }
        
        private void OnTimeExpired()
        {
            Fail();
        }
        
        // Abstrakta metoder som subklasser måste implementera
        /// <summary>
        /// Anropas när pusslet startar.
        /// </summary>
        protected abstract void OnPuzzleStart();
        
        /// <summary>
        /// Anropas när pusslet är löst.
        /// </summary>
        protected abstract void OnPuzzleComplete();
        
        /// <summary>
        /// Anropas när pusslet misslyckas.
        /// </summary>
        protected abstract void OnPuzzleFailed();
        
        /// <summary>
        /// Anropas när pusslet återställs.
        /// </summary>
        protected virtual void OnPuzzleReset()
        {
            // Standard-implementation - kan överskrivas
        }
        
        // Unity Editor support
        protected virtual void OnValidate()
        {
            // Säkerställ att goldTimeFraction är mellan 0 och 1
            goldTimeFraction = Mathf.Clamp01(goldTimeFraction);
            
            // Säkerställ att timeLimit är positivt
            timeLimit = Mathf.Max(0.1f, timeLimit);
        }
        
        /// <summary>
        /// Reducerar återstående tid om pusslet är aktivt.
        /// </summary>
        public void ReduceTimeRemaining(float seconds)
        {
            if (seconds <= 0f) return;
            if (!_isActive || _isCompleted || _isFailed) return;
            _currentTime = Mathf.Max(0f, _currentTime - seconds);
        }
    }
} 