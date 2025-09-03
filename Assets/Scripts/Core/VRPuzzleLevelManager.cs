using UnityEngine;
using Run4theRelic.Core;
using System.Collections.Generic;
using System.Linq;

namespace Run4theRelic.Core
{
    /// <summary>
    /// Manages all puzzle levels, their progression, and room management in Run4theRelic.
    /// Handles level unlocking, completion tracking, and room transitions.
    /// </summary>
    public class VRPuzzleLevelManager : MonoBehaviour
    {
        [Header("Level Management")]
        [SerializeField] private bool autoCreateLevels = true;
        [SerializeField] private PuzzleLevel[] availableLevels;
        [SerializeField] private PuzzleLevel currentLevel = PuzzleLevel.Tutorial;
        
        [Header("Level Settings")]
        [SerializeField] private bool enableProgression = true;
        [SerializeField] private bool requireLevelCompletion = true;
        [SerializeField] private int startingRelicCount = 0;
        
        [Header("Room Management")]
        [SerializeField] private Transform levelParent;
        [SerializeField] private bool autoTransitionRooms = true;
        [SerializeField] private float roomTransitionDelay = 1f;
        
        // Level Data
        private Dictionary<PuzzleLevel, PuzzleLevelSetup> _levelSetups = new Dictionary<PuzzleLevel, PuzzleLevelSetup>();
        private PuzzleLevelSetup _currentLevelSetup;
        private bool _isInitialized = false;
        
        // VR References
        private VREnvironmentSystem _environmentSystem;
        private VRManager _vrManager;
        
        // Events
        public static event System.Action<PuzzleLevel> OnLevelStarted;
        public static event System.Action<PuzzleLevel> OnLevelCompleted;
        public static event System.Action<PuzzleLevel, float> OnLevelProgressChanged;
        public static event System.Action<PuzzleLevel, PuzzleLevel> OnLevelChanged;
        
        protected override void Start()
        {
            // Initialize level manager
            InitializeLevelManager();
            
            // Subscribe to VR events
            SubscribeToVREvents();
        }
        
        /// <summary>
        /// Initialize the puzzle level manager.
        /// </summary>
        private void InitializeLevelManager()
        {
            // Find VR components
            _environmentSystem = FindObjectOfType<VREnvironmentSystem>();
            _vrManager = FindObjectOfType<VRManager>();
            
            if (_environmentSystem == null)
            {
                Debug.LogWarning("VRPuzzleLevelManager: No VREnvironmentSystem found!");
            }
            
            if (_vrManager == null)
            {
                Debug.LogWarning("VRPuzzleLevelManager: No VRManager found!");
            }
            
            // Setup level parent if not assigned
            if (levelParent == null)
            {
                GameObject levelParentObject = new GameObject("PuzzleLevels");
                levelParent = levelParentObject.transform;
                levelParent.SetParent(transform);
            }
            
            // Setup available levels
            if (availableLevels == null || availableLevels.Length == 0)
            {
                availableLevels = new PuzzleLevel[]
                {
                    PuzzleLevel.Tutorial,
                    PuzzleLevel.Beginner,
                    PuzzleLevel.Intermediate,
                    PuzzleLevel.Advanced,
                    PuzzleLevel.Expert,
                    PuzzleLevel.Master
                };
            }
            
            // Create levels if auto-create is enabled
            if (autoCreateLevels)
            {
                CreateAllLevels();
            }
            
            // Set current level
            SetCurrentLevel(currentLevel);
            
            _isInitialized = true;
            
            Debug.Log($"VRPuzzleLevelManager: Initialized with {availableLevels.Length} levels");
        }
        
        /// <summary>
        /// Subscribe to VR system events.
        /// </summary>
        private void SubscribeToVREvents()
        {
            if (_environmentSystem != null)
            {
                VREnvironmentSystem.OnRoomChanged += OnRoomChanged;
                VREnvironmentSystem.OnRoomTransitionCompleted += OnRoomTransitionCompleted;
            }
        }
        
        private void Update()
        {
            if (!_isInitialized) return;
            
            // Update current level progress
            UpdateLevelProgress();
        }
        
        /// <summary>
        /// Update current level progress.
        /// </summary>
        private void UpdateLevelProgress()
        {
            if (_currentLevelSetup == null) return;
            
            float progress = _currentLevelSetup.GetLevelProgress();
            
            // Check if level is completed
            if (_currentLevelSetup.IsLevelCompleted())
            {
                OnLevelCompleted?.Invoke(_currentLevelSetup.level);
                
                // Auto-advance to next level if progression is enabled
                if (enableProgression)
                {
                    PuzzleLevel nextLevel = GetNextLevel(_currentLevelSetup.level);
                    if (nextLevel != _currentLevelSetup.level)
                    {
                        StartCoroutine(AutoAdvanceToLevel(nextLevel));
                    }
                }
            }
        }
        
        /// <summary>
        /// Create all puzzle levels.
        /// </summary>
        private void CreateAllLevels()
        {
            foreach (var level in availableLevels)
            {
                CreateLevel(level);
            }
        }
        
        /// <summary>
        /// Create a specific puzzle level.
        /// </summary>
        /// <param name="level">Level to create.</param>
        /// <returns>Created level setup.</returns>
        public PuzzleLevelSetup CreateLevel(PuzzleLevel level)
        {
            if (_levelSetups.ContainsKey(level))
            {
                Debug.LogWarning($"VRPuzzleLevelManager: Level {level} already exists!");
                return _levelSetups[level];
            }
            
            // Get recommended puzzle count for level
            int puzzleCount = VRRoomPresets.GetRecommendedPuzzleCount(level);
            
            // Create complete level setup
            var levelSetup = VRRoomPresets.CreateCompletePuzzleLevel(level, levelParent.gameObject, puzzleCount);
            
            // Configure level-specific settings
            ConfigureLevelSetup(levelSetup);
            
            // Add to level setups
            _levelSetups[level] = levelSetup;
            
            // Initially hide all levels except tutorial
            if (level != PuzzleLevel.Tutorial)
            {
                levelSetup.roomTemplate.gameObject.SetActive(false);
            }
            
            Debug.Log($"VRPuzzleLevelManager: Created level {level} with {puzzleCount} puzzles");
            
            return levelSetup;
        }
        
        /// <summary>
        /// Configure level-specific settings.
        /// </summary>
        /// <param name="levelSetup">Level setup to configure.</param>
        private void ConfigureLevelSetup(PuzzleLevelSetup levelSetup)
        {
            // Set level info
            levelSetup.levelName = levelSetup.level.ToString();
            levelSetup.levelDescription = VRRoomPresets.GetRoomDescription(levelSetup.level);
            
            // Configure progression requirements
            switch (levelSetup.level)
            {
                case PuzzleLevel.Tutorial:
                    levelSetup.isUnlocked = true;
                    levelSetup.requiredLevels = new PuzzleLevel[0];
                    levelSetup.requiredRelicCount = 0;
                    break;
                    
                case PuzzleLevel.Beginner:
                    levelSetup.isUnlocked = true;
                    levelSetup.requiredLevels = new PuzzleLevel[0];
                    levelSetup.requiredRelicCount = 0;
                    break;
                    
                case PuzzleLevel.Intermediate:
                    levelSetup.isUnlocked = false;
                    levelSetup.requiredLevels = new PuzzleLevel[] { PuzzleLevel.Beginner };
                    levelSetup.requiredRelicCount = 2;
                    break;
                    
                case PuzzleLevel.Advanced:
                    levelSetup.isUnlocked = false;
                    levelSetup.requiredLevels = new PuzzleLevel[] { PuzzleLevel.Intermediate };
                    levelSetup.requiredRelicCount = 4;
                    break;
                    
                case PuzzleLevel.Expert:
                    levelSetup.isUnlocked = false;
                    levelSetup.requiredLevels = new PuzzleLevel[] { PuzzleLevel.Advanced };
                    levelSetup.requiredRelicCount = 6;
                    break;
                    
                case PuzzleLevel.Master:
                    levelSetup.isUnlocked = false;
                    levelSetup.requiredLevels = new PuzzleLevel[] { PuzzleLevel.Expert };
                    levelSetup.requiredRelicCount = 8;
                    break;
            }
        }
        
        /// <summary>
        /// Set the current puzzle level.
        /// </summary>
        /// <param name="level">Level to set as current.</param>
        public void SetCurrentLevel(PuzzleLevel level)
        {
            if (!_levelSetups.ContainsKey(level))
            {
                Debug.LogWarning($"VRPuzzleLevelManager: Level {level} not found!");
                return;
            }
            
            var previousLevel = currentLevel;
            currentLevel = level;
            _currentLevelSetup = _levelSetups[level];
            
            // Show current level room
            ShowLevelRoom(level);
            
            // Hide other level rooms
            HideOtherLevelRooms(level);
            
            // Notify level changed
            OnLevelChanged?.Invoke(previousLevel, level);
            OnLevelStarted?.Invoke(level);
            
            Debug.Log($"VRPuzzleLevelManager: Changed to level {level}");
        }
        
        /// <summary>
        /// Show a specific level room.
        /// </summary>
        /// <param name="level">Level to show.</param>
        private void ShowLevelRoom(PuzzleLevel level)
        {
            if (_levelSetups.ContainsKey(level))
            {
                var levelSetup = _levelSetups[level];
                levelSetup.roomTemplate.gameObject.SetActive(true);
                
                // Trigger room change if auto-transition is enabled
                if (autoTransitionRooms && _environmentSystem != null)
                {
                    // This would integrate with VREnvironmentSystem room changes
                    Debug.Log($"VRPuzzleLevelManager: Showing room for level {level}");
                }
            }
        }
        
        /// <summary>
        /// Hide other level rooms.
        /// </summary>
        /// <param name="currentLevel">Current level to keep visible.</param>
        private void HideOtherLevelRooms(PuzzleLevel currentLevel)
        {
            foreach (var kvp in _levelSetups)
            {
                if (kvp.Key != currentLevel)
                {
                    kvp.Value.roomTemplate.gameObject.SetActive(false);
                }
            }
        }
        
        /// <summary>
        /// Get the next level in progression.
        /// </summary>
        /// <param name="currentLevel">Current level.</param>
        /// <returns>Next level, or current level if no next level exists.</returns>
        private PuzzleLevel GetNextLevel(PuzzleLevel currentLevel)
        {
            int currentIndex = System.Array.IndexOf(availableLevels, currentLevel);
            if (currentIndex >= 0 && currentIndex < availableLevels.Length - 1)
            {
                return availableLevels[currentIndex + 1];
            }
            return currentLevel;
        }
        
        /// <summary>
        /// Get the previous level in progression.
        /// </summary>
        /// <param name="currentLevel">Current level.</param>
        /// <returns>Previous level, or current level if no previous level exists.</returns>
        private PuzzleLevel GetPreviousLevel(PuzzleLevel currentLevel)
        {
            int currentIndex = System.Array.IndexOf(availableLevels, currentLevel);
            if (currentIndex > 0)
            {
                return availableLevels[currentIndex - 1];
            }
            return currentLevel;
        }
        
        /// <summary>
        /// Auto-advance to a specific level.
        /// </summary>
        /// <param name="level">Level to advance to.</param>
        private System.Collections.IEnumerator AutoAdvanceToLevel(PuzzleLevel level)
        {
            // Wait for transition delay
            yield return new WaitForSeconds(roomTransitionDelay);
            
            // Change to new level
            SetCurrentLevel(level);
        }
        
        /// <summary>
        /// Check if a level is unlocked.
        /// </summary>
        /// <param name="level">Level to check.</param>
        /// <returns>True if the level is unlocked.</returns>
        public bool IsLevelUnlocked(PuzzleLevel level)
        {
            if (_levelSetups.ContainsKey(level))
            {
                return _levelSetups[level].IsLevelUnlocked();
            }
            return false;
        }
        
        /// <summary>
        /// Check if a level is completed.
        /// </summary>
        /// <param name="level">Level to check.</param>
        /// <returns>True if the level is completed.</returns>
        public bool IsLevelCompleted(PuzzleLevel level)
        {
            if (_levelSetups.ContainsKey(level))
            {
                return _levelSetups[level].IsLevelCompleted();
            }
            return false;
        }
        
        /// <summary>
        /// Get level progress.
        /// </summary>
        /// <param name="level">Level to get progress for.</param>
        /// <returns>Progress as percentage (0-100).</returns>
        public float GetLevelProgress(PuzzleLevel level)
        {
            if (_levelSetups.ContainsKey(level))
            {
                return _levelSetups[level].GetLevelProgress();
            }
            return 0f;
        }
        
        /// <summary>
        /// Get all available levels.
        /// </summary>
        /// <returns>Array of available levels.</returns>
        public PuzzleLevel[] GetAvailableLevels()
        {
            return availableLevels;
        }
        
        /// <summary>
        /// Get current level setup.
        /// </summary>
        /// <returns>Current level setup.</returns>
        public PuzzleLevelSetup GetCurrentLevelSetup()
        {
            return _currentLevelSetup;
        }
        
        /// <summary>
        /// Get level setup for a specific level.
        /// </summary>
        /// <param name="level">Level to get setup for.</param>
        /// <returns>Level setup, or null if not found.</returns>
        public PuzzleLevelSetup GetLevelSetup(PuzzleLevel level)
        {
            if (_levelSetups.ContainsKey(level))
            {
                return _levelSetups[level];
            }
            return null;
        }
        
        /// <summary>
        /// Reset a level to initial state.
        /// </summary>
        /// <param name="level">Level to reset.</param>
        public void ResetLevel(PuzzleLevel level)
        {
            if (_levelSetups.ContainsKey(level))
            {
                var levelSetup = _levelSetups[level];
                levelSetup.roomTemplate.ResetRoom();
                
                Debug.Log($"VRPuzzleLevelManager: Reset level {level}");
            }
        }
        
        /// <summary>
        /// Reset all levels to initial state.
        /// </summary>
        public void ResetAllLevels()
        {
            foreach (var kvp in _levelSetups)
            {
                kvp.Value.roomTemplate.ResetRoom();
            }
            
            Debug.Log("VRPuzzleLevelManager: Reset all levels");
        }
        
        /// <summary>
        /// Handle room changes.
        /// </summary>
        /// <param name="newRoom">New room that was entered.</param>
        private void OnRoomChanged(VREnvironmentSystem.VRRoom newRoom)
        {
            // This would integrate with room system
            Debug.Log($"VRPuzzleLevelManager: Room changed to {newRoom.roomName}");
        }
        
        /// <summary>
        /// Handle room transition completion.
        /// </summary>
        /// <param name="fromRoom">Room transitioned from.</param>
        /// <param name="toRoom">Room transitioned to.</param>
        private void OnRoomTransitionCompleted(VREnvironmentSystem.VRRoom fromRoom, VREnvironmentSystem.VRRoom toRoom)
        {
            // This would integrate with room transition system
            Debug.Log($"VRPuzzleLevelManager: Room transition completed to {toRoom.roomName}");
        }
        
        /// <summary>
        /// Get total game progress across all levels.
        /// </summary>
        /// <returns>Total progress as percentage (0-100).</returns>
        public float GetTotalGameProgress()
        {
            if (_levelSetups.Count == 0) return 0f;
            
            float totalProgress = 0f;
            foreach (var kvp in _levelSetups)
            {
                totalProgress += kvp.Value.GetLevelProgress();
            }
            
            return totalProgress / _levelSetups.Count;
        }
        
        /// <summary>
        /// Get completed level count.
        /// </summary>
        /// <returns>Number of completed levels.</returns>
        public int GetCompletedLevelCount()
        {
            int completedCount = 0;
            foreach (var kvp in _levelSetups)
            {
                if (kvp.Value.IsLevelCompleted())
                {
                    completedCount++;
                }
            }
            return completedCount;
        }
        
        /// <summary>
        /// Get unlocked level count.
        /// </summary>
        /// <returns>Number of unlocked levels.</returns>
        public int GetUnlockedLevelCount()
        {
            int unlockedCount = 0;
            foreach (var kvp in _levelSetups)
            {
                if (kvp.Value.IsLevelUnlocked())
                {
                    unlockedCount++;
                }
            }
            return unlockedCount;
        }
        
        protected override void OnDestroy()
        {
            // Unsubscribe from VR events
            if (_environmentSystem != null)
            {
                VREnvironmentSystem.OnRoomChanged -= OnRoomChanged;
                VREnvironmentSystem.OnRoomTransitionCompleted -= OnRoomTransitionCompleted;
            }
            
            base.OnDestroy();
        }
        
        /// <summary>
        /// Get level statistics for UI display.
        /// </summary>
        /// <returns>Level statistics.</returns>
        public LevelStatistics GetLevelStatistics()
        {
            return new LevelStatistics
            {
                totalLevels = availableLevels.Length,
                unlockedLevels = GetUnlockedLevelCount(),
                completedLevels = GetCompletedLevelCount(),
                totalProgress = GetTotalGameProgress(),
                currentLevel = currentLevel,
                currentLevelProgress = GetLevelProgress(currentLevel)
            };
        }
    }
    
    /// <summary>
    /// Statistics for puzzle levels.
    /// </summary>
    [System.Serializable]
    public class LevelStatistics
    {
        [Header("Level Counts")]
        public int totalLevels;
        public int unlockedLevels;
        public int completedLevels;
        
        [Header("Progress")]
        public float totalProgress;
        public PuzzleLevel currentLevel;
        public float currentLevelProgress;
        
        /// <summary>
        /// Get completion percentage.
        /// </summary>
        /// <returns>Completion percentage (0-100).</returns>
        public float GetCompletionPercentage()
        {
            if (totalLevels == 0) return 0f;
            return (float)completedLevels / totalLevels * 100f;
        }
        
        /// <summary>
        /// Get unlock percentage.
        /// </summary>
        /// <returns>Unlock percentage (0-100).</returns>
        public float GetUnlockPercentage()
        {
            if (totalLevels == 0) return 0f;
            return (float)unlockedLevels / totalLevels * 100f;
        }
    }
}
