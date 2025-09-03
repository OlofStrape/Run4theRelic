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
        /// Triggas varje sekund medan ett pussel är aktivt. Parametrar: sekunder kvar (avrundat uppåt), samt ursprunglig limit i sekunder.
        /// </summary>
        public static event Action<int, int> OnPuzzleTimerTick; // secondsRemaining, secondsLimit
        
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
        /// Triggas när fas byts, inklusive en varaktighet (sekunder). 0 om ingen timer.
        /// </summary>
        public static event Action<MatchPhase, float> OnPhaseChanged;
        /// <summary>
        /// Triggas när matchen är slut.
        /// </summary>
        public static event Action OnMatchEnded;
        
        // Sabotage-events
        /// <summary>
        /// Triggas när ett sabotage aktiveras. Arg1 = typ ("fog"/"timedrain"/"fakeclues"), Arg2 = värde (t.ex. varaktighet eller sekunder).
        /// </summary>
        public static event Action<string, float> OnSabotaged;
        
        // Event-triggers (endast för interna system)
        internal static void TriggerPuzzleCompleted(int playerId, float clearTime)
        {
            OnPuzzleCompleted?.Invoke(playerId, clearTime);
        }
        internal static void TriggerPuzzleTimerTick(int secondsRemaining, int secondsLimit)
        {
            OnPuzzleTimerTick?.Invoke(secondsRemaining, secondsLimit);
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
        internal static void TriggerPhaseChanged(MatchPhase phase, float duration)
        {
            OnPhaseChanged?.Invoke(phase, duration);
        }
        internal static void TriggerMatchEnded()
        {
            OnMatchEnded?.Invoke();
        }
        
        internal static void TriggerSabotaged(string type, float value)
        {
            OnSabotaged?.Invoke(type, value);
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