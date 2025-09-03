using UnityEngine;
using Run4theRelic.Core;

namespace Run4theRelic.Core
{
    /// <summary>
    /// Hanterar match-progression genom alla faser från Lobby till PostMatch.
    /// Avancerar automatiskt baserat på pussel-completion events.
    /// </summary>
    public class MatchOrchestrator : MonoBehaviour
    {
        [Header("Match Settings")]
        [SerializeField] private float countdownDuration = 3f;
        [SerializeField] private float phaseTransitionDelay = 1f;
        
        [Header("Puzzle References")]
        [SerializeField] private MonoBehaviour[] puzzleControllers;
        
        [Header("Relic Settings")]
        [SerializeField] private Transform relicSpawnPoint;
        [SerializeField] private GameObject relicPrefab;
        
        private MatchPhase _currentPhase = MatchPhase.Lobby;
        private float _phaseTimer;
        private bool _isMatchActive;
        
        /// <summary>
        /// Aktuell match-fas.
        /// </summary>
        public MatchPhase CurrentPhase => _currentPhase;
        
        /// <summary>
        /// Är matchen aktiv (Countdown eller senare).
        /// </summary>
        public bool IsMatchActive => _isMatchActive;
        
        private void Start()
        {
            // Prenumerera på pussel-events
            GameEvents.OnPuzzleCompleted += OnPuzzleCompleted;
            
            // Starta i Lobby-fasen
            SetPhase(MatchPhase.Lobby);
        }
        
        private void OnDestroy()
        {
            GameEvents.OnPuzzleCompleted -= OnPuzzleCompleted;
        }
        
        private void Update()
        {
            if (!_isMatchActive) return;
            
            // Hantera fas-specifik logik
            switch (_currentPhase)
            {
                case MatchPhase.Countdown:
                    HandleCountdown();
                    break;
                case MatchPhase.GoldTimeSabotage:
                    HandleSabotagePhase();
                    break;
            }
        }
        
        /// <summary>
        /// Startar matchen med countdown.
        /// </summary>
        public void StartMatch()
        {
            if (_isMatchActive) return;
            
            _isMatchActive = true;
            SetPhase(MatchPhase.Countdown);
            _phaseTimer = countdownDuration;
            
            Debug.Log("Match started! Countdown beginning...");
        }
        
        /// <summary>
        /// Avancerar till nästa match-fas.
        /// </summary>
        public void AdvancePhase()
        {
            MatchPhase nextPhase = GetNextPhase(_currentPhase);
            SetPhase(nextPhase);
            
            // Hantera fas-specifik logik
            if (nextPhase == MatchPhase.Final)
            {
                SpawnRelic();
            }
        }
        
        /// <summary>
        /// Spawnar Relic:en vid Final-fasens start.
        /// </summary>
        public void SpawnRelic()
        {
            if (relicPrefab == null || relicSpawnPoint == null)
            {
                Debug.LogWarning("Relic prefab or spawn point not set!");
                return;
            }
            
            GameObject relic = Instantiate(relicPrefab, relicSpawnPoint.position, relicSpawnPoint.rotation);
            Debug.Log("Relic spawned at Final phase!");
            
            // Trigga RelicSpawned event
            GameEvents.TriggerRelicSpawned();
        }
        
        private void SetPhase(MatchPhase newPhase)
        {
            _currentPhase = newPhase;
            GameEvents.TriggerMatchPhaseChanged(newPhase);
            
            Debug.Log($"Match phase changed to: {newPhase}");
            
            // Hantera fas-specifik setup
            switch (newPhase)
            {
                case MatchPhase.Lobby:
                    // Vänta på spelare
                    break;
                case MatchPhase.Countdown:
                    _phaseTimer = countdownDuration;
                    break;
                case MatchPhase.Puzzle1:
                case MatchPhase.Puzzle2:
                case MatchPhase.Puzzle3:
                    // Aktivera motsvarande pussel
                    break;
                case MatchPhase.GoldTimeSabotage:
                    _phaseTimer = 5f; // Sabotage-duration
                    break;
                case MatchPhase.Final:
                    // Relic kommer spawnas via SpawnRelic()
                    break;
                case MatchPhase.PostMatch:
                    _isMatchActive = false;
                    GameEvents.TriggerMatchEnded();
                    break;
            }

            // Rapportera fas och eventuell timer till HUD
            float durationSeconds = 0f;
            if (newPhase == MatchPhase.Countdown || newPhase == MatchPhase.GoldTimeSabotage)
            {
                durationSeconds = _phaseTimer;
            }
            else if (newPhase == MatchPhase.Final)
            {
                durationSeconds = 0f; // Final-fasen har ingen timer
            }

            GameEvents.OnPhaseChanged?.Invoke(_currentPhase, durationSeconds);
        }
        
        private MatchPhase GetNextPhase(MatchPhase currentPhase)
        {
            return currentPhase switch
            {
                MatchPhase.Lobby => MatchPhase.Countdown,
                MatchPhase.Countdown => MatchPhase.Puzzle1,
                MatchPhase.Puzzle1 => MatchPhase.Puzzle2,
                MatchPhase.Puzzle2 => MatchPhase.Puzzle3,
                MatchPhase.Puzzle3 => MatchPhase.GoldTimeSabotage,
                MatchPhase.GoldTimeSabotage => MatchPhase.Final,
                MatchPhase.Final => MatchPhase.PostMatch,
                _ => MatchPhase.Lobby
            };
        }
        
        private void HandleCountdown()
        {
            _phaseTimer -= Time.deltaTime;
            
            if (_phaseTimer <= 0f)
            {
                AdvancePhase();
            }
        }
        
        private void HandleSabotagePhase()
        {
            _phaseTimer -= Time.deltaTime;
            
            if (_phaseTimer <= 0f)
            {
                AdvancePhase();
            }
        }
        
        private void OnPuzzleCompleted(int playerId, float clearTime)
        {
            // Avancera till nästa fas efter en kort fördröjning
            Invoke(nameof(AdvancePhase), phaseTransitionDelay);
            
            Debug.Log($"Puzzle completed by player {playerId} in {clearTime:F2}s. Advancing to next phase...");
        }
    }
} 