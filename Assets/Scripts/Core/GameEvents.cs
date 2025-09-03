using System;

namespace Core
{
    /// <summary>
    /// Provides global game events for high-level systems to subscribe to.
    /// </summary>
    public static class GameEvents
    {
        /// <summary>Raised when the match begins.</summary>
        public static event Action? MatchStarted;

        /// <summary>Raised when the match ends.</summary>
        public static event Action? MatchEnded;

        /// <summary>Raised when the relic is successfully extracted.</summary>
        public static event Action? RelicExtracted;

        /// <summary>
        /// Invoke the start of a match.
        /// </summary>
        public static void RaiseMatchStarted() => MatchStarted?.Invoke();

        /// <summary>
        /// Invoke that the match has ended.
        /// </summary>
        public static void RaiseMatchEnded() => MatchEnded?.Invoke();

        /// <summary>
        /// Invoke that the relic has been extracted.
        /// </summary>
        public static void RaiseRelicExtracted() => RelicExtracted?.Invoke();
    }
}
