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
        /// Triggas när matchen är slut.
        /// </summary>
        public static event Action OnMatchEnded;
        
        // Event-triggers (endast för interna system)
        internal static void TriggerPuzzleCompleted(int playerId, float clearTime)
        {
            OnPuzzleCompleted?.Invoke(playerId, clearTime);
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