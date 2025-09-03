using System;

namespace Run4theRelic.Core
{
    /// <summary>
    /// Central kommunikationshub för alla system via statiska events.
    /// Löst kopplade system kan kommunicera utan direkta referenser.
    /// </summary>
    public static class GameEvents
    {
        // Pussel-events
        /// <summary>
        /// Triggas när ett pussel är löst. playerId = -1 för singleplayer.
        /// </summary>
        public static event Action<int, float> OnPuzzleCompleted; // playerId, clearTime
        /// <summary>
        /// Triggas varje sekund-tick för ett aktivt pussel (HUD-timer mm).
        /// </summary>
        public static event Action<int, int> OnPuzzleTimerTick; // remainingSeconds, limitSeconds
        
        /// <summary>
        /// Triggas när en spelare elimineras.
        /// </summary>
        public static event Action<int> OnPlayerEliminated; // playerId
        
        // Relic-events
        /// <summary>
        /// Triggas när Relic:en spawnas i Final-fasen.
        /// </summary>
        public static event Action OnRelicSpawned;
        
        /// <summary>
        /// Triggas när en spelare plockar upp Relic:en.
        /// </summary>
        public static event Action<int> OnRelicPickedUp; // playerId
        
        /// <summary>
        /// Triggas när Relic:en släpps.
        /// </summary>
        public static event Action<int> OnRelicDropped; // playerId
        
        /// <summary>
        /// Triggas när Relic:en extraheras för vinst.
        /// </summary>
        public static event Action<int> OnRelicExtracted; // playerId
        
        // Match-events
        /// <summary>
        /// Triggas när match-fasen ändras.
        /// </summary>
        public static event Action<MatchPhase> OnMatchPhaseChanged;
        /// <summary>
        /// Triggas när en sabotage-effekt appliceras (för HUD/logg): key och värde (sekunder mm).
        /// </summary>
        public static event Action<string, float> OnSabotaged;
        
        /// <summary>
        /// Triggas när matchen är slut.
        /// </summary>
        public static event Action OnMatchEnded;
        
        // Event-triggers (endast för interna system)
        internal static void TriggerPuzzleCompleted(int playerId, float clearTime)
        {
            OnPuzzleCompleted?.Invoke(playerId, clearTime);
        }
        /// <summary>
        /// Publik helper för att skicka timer-ticks (HUD).
        /// </summary>
        public static void TriggerPuzzleTimerTick(int remainingSeconds, int limitSeconds)
        {
            OnPuzzleTimerTick?.Invoke(remainingSeconds, limitSeconds);
        }
        
        internal static void TriggerPlayerEliminated(int playerId)
        {
            OnPlayerEliminated?.Invoke(playerId);
        }
        
        internal static void TriggerRelicSpawned()
        {
            OnRelicSpawned?.Invoke();
        }
        
        internal static void TriggerRelicPickedUp(int playerId)
        {
            OnRelicPickedUp?.Invoke(playerId);
        }
        
        internal static void TriggerRelicDropped(int playerId)
        {
            OnRelicDropped?.Invoke(playerId);
        }
        
        internal static void TriggerRelicExtracted(int playerId)
        {
            OnRelicExtracted?.Invoke(playerId);
        }
        
        internal static void TriggerMatchPhaseChanged(MatchPhase newPhase)
        {
            OnMatchPhaseChanged?.Invoke(newPhase);
        }
        /// <summary>
        /// Publik helper för att skicka sabotage-notiser.
        /// </summary>
        public static void TriggerSabotaged(string key, float value)
        {
            OnSabotaged?.Invoke(key, value);
        }
        
        internal static void TriggerMatchEnded()
        {
            OnMatchEnded?.Invoke();
        }
    }
    
    /// <summary>
    /// Match-faser som definierar spelflödet.
    /// </summary>
    public enum MatchPhase
    {
        Lobby,           // Vänteläge
        Countdown,       // 3-2-1 nedräkning
        Puzzle1,         // Första pussel
        Puzzle2,         // Andra pussel
        Puzzle3,         // Tredje pussel
        GoldTimeSabotage, // Sabotage-fas
        Final,           // Relic-spawning och sista strid
        PostMatch        // Resultat och statistik
    }
} 