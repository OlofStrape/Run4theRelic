using System;
using UnityEngine;

namespace Run4theRelic.Core
{
    /// <summary>
    /// Centrala spelhändelser för Run4theRelic
    /// Hanterar kommunikation mellan olika system
    /// </summary>
    public static class GameEvents
    {
        // Puzzle Events
        public static event System.Action<string, float, bool> OnPuzzleCompleted;
        public static event System.Action<string> OnPuzzleStarted;
        public static event System.Action<string> OnPuzzleFailed;
        
        // Room Events
        public static event System.Action<string> OnRoomEntered;
        public static event System.Action<string> OnRoomExited;
        public static event System.Action<string> OnRoomCompleted;
        
        // Relic Events
        public static event System.Action<string> OnRelicCollected;
        public static event System.Action<string> OnRelicPlaced;
        public static event System.Action<string> OnRelicActivated;
        
        // Player Events
        public static event System.Action<float> OnPlayerHealthChanged;
        public static event System.Action<int> OnPlayerScoreChanged;
        public static event System.Action<string> OnPlayerLevelUp;
        
        // Game State Events
        public static event System.Action OnGameStarted;
        public static event System.Action OnGamePaused;
        public static event System.Action OnGameResumed;
        public static event System.Action OnGameEnded;
        
        // VR Events
        public static event System.Action OnVRHeadsetConnected;
        public static event System.Action OnVRHeadsetDisconnected;
        public static event System.Action OnVRControllerConnected;
        public static event System.Action OnVRControllerDisconnected;
        
        // Content Generation Events
        public static event System.Action<string> OnRoomGenerated;
        public static event System.Action<string> OnPuzzleGenerated;
        public static event System.Action<float> OnGenerationProgress;
        public static event System.Action<string> OnGenerationCompleted;
        public static event System.Action<string> OnGenerationFailed;
        
        // AI Events
        public static event System.Action<float> OnDifficultyChanged;
        public static event System.Action<string> OnPlayerBehaviorDetected;
        public static event System.Action<float> OnPerformanceAnalyzed;
        
        // Story Events
        public static event System.Action<string> OnStoryElementGenerated;
        public static event System.Action<string> OnStoryElementCompleted;
        public static event System.Action<string> OnMoodChanged;
        
        // Utility Methods
        public static void TriggerPuzzleCompleted(string puzzleId, float completionTime, bool isPerfect)
        {
            OnPuzzleCompleted?.Invoke(puzzleId, completionTime, isPerfect);
        }
        
        public static void TriggerPuzzleStarted(string puzzleId)
        {
            OnPuzzleStarted?.Invoke(puzzleId);
        }
        
        public static void TriggerPuzzleFailed(string puzzleId)
        {
            OnPuzzleFailed?.Invoke(puzzleId);
        }
        
        public static void TriggerRoomEntered(string roomId)
        {
            OnRoomEntered?.Invoke(roomId);
        }
        
        public static void TriggerRoomExited(string roomId)
        {
            OnRoomExited?.Invoke(roomId);
        }
        
        public static void TriggerRoomCompleted(string roomId)
        {
            OnRoomCompleted?.Invoke(roomId);
        }
        
        public static void TriggerRelicCollected(string relicId)
        {
            OnRelicCollected?.Invoke(relicId);
        }
        
        public static void TriggerRelicPlaced(string relicId)
        {
            OnRelicPlaced?.Invoke(relicId);
        }
        
        public static void TriggerRelicActivated(string relicId)
        {
            OnRelicActivated?.Invoke(relicId);
        }
        
        public static void TriggerPlayerHealthChanged(float newHealth)
        {
            OnPlayerHealthChanged?.Invoke(newHealth);
        }
        
        public static void TriggerPlayerScoreChanged(int newScore)
        {
            OnPlayerScoreChanged?.Invoke(newScore);
        }
        
        public static void TriggerPlayerLevelUp(string newLevel)
        {
            OnPlayerLevelUp?.Invoke(newLevel);
        }
        
        public static void TriggerGameStarted()
        {
            OnGameStarted?.Invoke();
        }
        
        public static void TriggerGamePaused()
        {
            OnGamePaused?.Invoke();
        }
        
        public static void TriggerGameResumed()
        {
            OnGameResumed?.Invoke();
        }
        
        public static void TriggerGameEnded()
        {
            OnGameEnded?.Invoke();
        }
        
        public static void TriggerVRHeadsetConnected()
        {
            OnVRHeadsetConnected?.Invoke();
        }
        
        public static void TriggerVRHeadsetDisconnected()
        {
            OnVRHeadsetDisconnected?.Invoke();
        }
        
        public static void TriggerVRControllerConnected()
        {
            OnVRControllerConnected?.Invoke();
        }
        
        public static void TriggerVRControllerDisconnected()
        {
            OnVRControllerDisconnected?.Invoke();
        }
        
        public static void TriggerRoomGenerated(string roomId)
        {
            OnRoomGenerated?.Invoke(roomId);
        }
        
        public static void TriggerPuzzleGenerated(string puzzleId)
        {
            OnPuzzleGenerated?.Invoke(puzzleId);
        }
        
        public static void TriggerGenerationProgress(float progress)
        {
            OnGenerationProgress?.Invoke(progress);
        }
        
        public static void TriggerGenerationCompleted(string result)
        {
            OnGenerationCompleted?.Invoke(result);
        }
        
        public static void TriggerGenerationFailed(string error)
        {
            OnGenerationFailed?.Invoke(error);
        }
        
        public static void TriggerDifficultyChanged(float newDifficulty)
        {
            OnDifficultyChanged?.Invoke(newDifficulty);
        }
        
        public static void TriggerPlayerBehaviorDetected(string behavior)
        {
            OnPlayerBehaviorDetected?.Invoke(behavior);
        }
        
        public static void TriggerPerformanceAnalyzed(float performance)
        {
            OnPerformanceAnalyzed?.Invoke(performance);
        }
        
        public static void TriggerStoryElementGenerated(string elementId)
        {
            OnStoryElementGenerated?.Invoke(elementId);
        }
        
        public static void TriggerStoryElementCompleted(string elementId)
        {
            OnStoryElementCompleted?.Invoke(elementId);
        }
        
        public static void TriggerMoodChanged(string newMood)
        {
            OnMoodChanged?.Invoke(newMood);
        }
    }
    
    /// <summary>
    /// Pusseltyper som stöds av systemet
    /// </summary>
    public enum PuzzleType
    {
        RelicPlacement,    // Placera reliker i rätt platser
        HandGesture,        // Utför handgester
        PatternMatching,    // Matcha mönster
        Sequence,           // Sekvensbaserade pussel
        Logic,              // Logikpussel
        Physics,            // Fysikbaserade pussel
        Combination         // Kombinerade pusseltyper
    }
    
    /// <summary>
    /// Rumtyper som stöds av systemet
    /// </summary>
    public enum RoomType
    {
        Entrance,           // Ingångsrum
        Corridor,           // Korridor
        PuzzleRoom,         // Pusselrum
        RelicChamber,       // Relikkammare
        BossRoom,           // Bossrum
        Exit                // Utgångsrum
    }
    
    /// <summary>
    /// Spelnivåer för progression
    /// </summary>
    public enum GameLevel
    {
        Tutorial,           // Handledning
        Beginner,           // Nybörjare
        Intermediate,       // Mellannivå
        Advanced,           // Avancerad
        Expert,             // Expert
        Master              // Mästare
    }
    
    /// <summary>
    /// VR-enhetstyper som stöds
    /// </summary>
    public enum VRDeviceType
    {
        None,               // Ingen VR-enhet
        OculusQuest,        // Oculus Quest
        OculusRift,         // Oculus Rift
        HTCVive,            // HTC Vive
        ValveIndex,         // Valve Index
        WindowsMR,          // Windows Mixed Reality
        Other               // Annan VR-enhet
    }
} 