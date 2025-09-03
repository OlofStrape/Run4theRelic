using UnityEngine;
using Run4theRelic.Core;
using Run4theRelic.Puzzles;

namespace Run4theRelic.Core
{
    /// <summary>
    /// Pre-configured room presets for different puzzle levels in Run4theRelic.
    /// Each preset provides a complete room configuration with atmosphere, lighting, and puzzle setup.
    /// </summary>
    public static class VRRoomPresets
    {
        /// <summary>
        /// Create a complete room setup for a specific puzzle level.
        /// </summary>
        /// <param name="level">Puzzle level to create room for.</param>
        /// <param name="parent">Parent GameObject to attach the room to.</param>
        /// <returns>Configured VRRoomTemplate component.</returns>
        public static VRRoomTemplate CreatePuzzleLevelRoom(PuzzleLevel level, GameObject parent)
        {
            // Create room GameObject
            GameObject roomObject = new GameObject($"PuzzleLevel_{level}_Room");
            roomObject.transform.SetParent(parent.transform);
            
            // Add room template component
            var roomTemplate = roomObject.AddComponent<VRRoomTemplate>();
            
            // Configure room based on level
            ConfigureRoomForLevel(roomTemplate, level);
            
            // Configure room from template
            roomTemplate.ConfigureRoomFromTemplate();
            
            return roomTemplate;
        }
        
        /// <summary>
        /// Configure room template for a specific puzzle level.
        /// </summary>
        /// <param name="roomTemplate">Room template to configure.</param>
        /// <param name="level">Puzzle level to configure for.</param>
        private static void ConfigureRoomForLevel(VRRoomTemplate roomTemplate, PuzzleLevel level)
        {
            switch (level)
            {
                case PuzzleLevel.Tutorial:
                    ConfigureTutorialRoom(roomTemplate);
                    break;
                    
                case PuzzleLevel.Beginner:
                    ConfigureBeginnerRoom(roomTemplate);
                    break;
                    
                case PuzzleLevel.Intermediate:
                    ConfigureIntermediateRoom(roomTemplate);
                    break;
                    
                case PuzzleLevel.Advanced:
                    ConfigureAdvancedRoom(roomTemplate);
                    break;
                    
                case PuzzleLevel.Expert:
                    ConfigureExpertRoom(roomTemplate);
                    break;
                    
                case PuzzleLevel.Master:
                    ConfigureMasterRoom(roomTemplate);
                    break;
                    
                default:
                    ConfigureBeginnerRoom(roomTemplate);
                    break;
            }
        }
        
        /// <summary>
        /// Configure tutorial room - simple and welcoming.
        /// </summary>
        /// <param name="roomTemplate">Room template to configure.</param>
        private static void ConfigureTutorialRoom(VRRoomTemplate roomTemplate)
        {
            roomTemplate.templateType = VRRoomTemplate.RoomTemplateType.PuzzleRoom;
            roomTemplate.roomName = "Tutorial Chamber";
            roomTemplate.roomDescription = "A welcoming chamber to learn the basics of relic puzzles.";
            
            // Configure template settings
            var settings = roomTemplate.templateSettings;
            
            // Lighting - bright and friendly
            settings.ambientColor = new Color(0.9f, 0.95f, 1f); // Soft blue-white
            settings.ambientIntensity = 0.4f;
            
            // Atmosphere - minimal effects
            settings.enableFog = false;
            
            // VR Comfort - maximum comfort for beginners
            settings.enableComfortFeatures = true;
            settings.maxTurnAngle = 30f;
            settings.enableTeleportation = true;
            
            // Puzzle settings - simple and forgiving
            settings.autoActivatePuzzles = true;
        }
        
        /// <summary>
        /// Configure beginner room - gentle introduction to puzzles.
        /// </summary>
        /// <param name="roomTemplate">Room template to configure.</param>
        private static void ConfigureBeginnerRoom(VRRoomTemplate roomTemplate)
        {
            roomTemplate.templateType = VRRoomTemplate.RoomTemplateType.PuzzleRoom;
            roomTemplate.roomName = "Ancient Library";
            roomTemplate.roomDescription = "A peaceful library filled with ancient knowledge and simple puzzles.";
            
            // Configure template settings
            var settings = roomTemplate.templateSettings;
            
            // Lighting - warm and inviting
            settings.ambientColor = new Color(1f, 0.95f, 0.8f); // Warm candlelight
            settings.ambientIntensity = 0.35f;
            
            // Atmosphere - subtle effects
            settings.enableFog = true;
            settings.fogColor = new Color(0.8f, 0.75f, 0.6f); // Warm fog
            settings.fogDensity = 0.008f;
            
            // VR Comfort - comfortable for new players
            settings.enableComfortFeatures = true;
            settings.maxTurnAngle = 35f;
            settings.enableTeleportation = true;
            
            // Puzzle settings - gentle progression
            settings.autoActivatePuzzles = true;
        }
        
        /// <summary>
        /// Configure intermediate room - moderate challenge.
        /// </summary>
        /// <param name="roomTemplate">Room template to configure.</param>
        private static void ConfigureIntermediateRoom(VRRoomTemplate roomTemplate)
        {
            roomTemplate.templateType = VRRoomTemplate.RoomTemplateType.PuzzleRoom;
            roomTemplate.roomName = "Crystal Cavern";
            roomTemplate.roomDescription = "A mysterious cavern where crystals pulse with ancient energy.";
            
            // Configure template settings
            var settings = roomTemplate.templateSettings;
            
            // Lighting - mystical and engaging
            settings.ambientColor = new Color(0.7f, 0.8f, 1f); // Crystal blue
            settings.ambientIntensity = 0.3f;
            
            // Atmosphere - more dramatic effects
            settings.enableFog = true;
            settings.fogColor = new Color(0.6f, 0.7f, 0.9f); // Blue fog
            settings.fogDensity = 0.012f;
            
            // VR Comfort - moderate comfort
            settings.enableComfortFeatures = true;
            settings.maxTurnAngle = 40f;
            settings.enableTeleportation = true;
            
            // Puzzle settings - moderate challenge
            settings.autoActivatePuzzles = true;
        }
        
        /// <summary>
        /// Configure advanced room - challenging puzzles.
        /// </summary>
        /// <param name="roomTemplate">Room template to configure.</param>
        private static void ConfigureAdvancedRoom(VRRoomTemplate roomTemplate)
        {
            roomTemplate.templateType = VRRoomTemplate.RoomTemplateType.PuzzleRoom;
            roomTemplate.roomName = "Shadow Sanctum";
            roomTemplate.roomDescription = "A foreboding sanctum where shadows dance and puzzles test your wit.";
            
            // Configure template settings
            var settings = roomTemplate.templateSettings;
            
            // Lighting - dark and mysterious
            settings.ambientColor = new Color(0.4f, 0.3f, 0.5f); // Deep purple
            settings.ambientIntensity = 0.25f;
            
            // Atmosphere - intense effects
            settings.enableFog = true;
            settings.fogColor = new Color(0.3f, 0.2f, 0.4f); // Dark purple fog
            settings.fogDensity = 0.018f;
            
            // VR Comfort - reduced comfort for challenge
            settings.enableComfortFeatures = true;
            settings.maxTurnAngle = 45f;
            settings.enableTeleportation = true;
            
            // Puzzle settings - challenging
            settings.autoActivatePuzzles = true;
        }
        
        /// <summary>
        /// Configure expert room - very challenging puzzles.
        /// </summary>
        /// <param name="roomTemplate">Room template to configure.</param>
        private static void ConfigureExpertRoom(VRRoomTemplate roomTemplate)
        {
            roomTemplate.templateType = VRRoomTemplate.RoomTemplateType.PuzzleRoom;
            roomTemplate.roomName = "Void Nexus";
            roomTemplate.roomDescription = "A nexus of pure energy where reality bends and puzzles defy logic.";
            
            // Configure template settings
            var settings = roomTemplate.templateSettings;
            
            // Lighting - intense and otherworldly
            settings.ambientColor = new Color(0.2f, 0.8f, 0.9f); // Electric cyan
            settings.ambientIntensity = 0.2f;
            
            // Atmosphere - extreme effects
            settings.enableFog = true;
            settings.fogColor = new Color(0.1f, 0.6f, 0.7f); // Cyan fog
            settings.fogDensity = 0.025f;
            
            // VR Comfort - minimal comfort for challenge
            settings.enableComfortFeatures = true;
            settings.maxTurnAngle = 50f;
            settings.enableTeleportation = true;
            
            // Puzzle settings - very challenging
            settings.autoActivatePuzzles = true;
        }
        
        /// <summary>
        /// Configure master room - ultimate challenge.
        /// </summary>
        /// <param name="roomTemplate">Room template to configure.</param>
        private static void ConfigureMasterRoom(VRRoomTemplate roomTemplate)
        {
            roomTemplate.templateType = VRRoomTemplate.RoomTemplateType.PuzzleRoom;
            roomTemplate.roomName = "Eternal Abyss";
            roomTemplate.roomDescription = "The ultimate test of skill and determination in the depths of eternity.";
            
            // Configure template settings
            var settings = roomTemplate.templateSettings;
            
            // Lighting - apocalyptic and intense
            settings.ambientColor = new Color(0.8f, 0.2f, 0.2f); // Blood red
            settings.ambientIntensity = 0.15f;
            
            // Atmosphere - overwhelming effects
            settings.enableFog = true;
            settings.fogColor = new Color(0.6f, 0.1f, 0.1f); // Dark red fog
            settings.fogDensity = 0.035f;
            
            // VR Comfort - minimal comfort for ultimate challenge
            settings.enableComfortFeatures = true;
            settings.maxTurnAngle = 55f;
            settings.enableTeleportation = true;
            
            // Puzzle settings - ultimate challenge
            settings.autoActivatePuzzles = true;
        }
        
        /// <summary>
        /// Create a complete puzzle level setup with room and puzzles.
        /// </summary>
        /// <param name="level">Puzzle level to create.</param>
        /// <param name="parent">Parent GameObject.</param>
        /// <param name="puzzleCount">Number of puzzles to create.</param>
        /// <returns>Complete puzzle level setup.</returns>
        public static PuzzleLevelSetup CreateCompletePuzzleLevel(PuzzleLevel level, GameObject parent, int puzzleCount = 3)
        {
            // Create room
            var roomTemplate = CreatePuzzleLevelRoom(level, parent);
            
            // Create puzzle setup
            var puzzleSetup = new PuzzleLevelSetup
            {
                level = level,
                roomTemplate = roomTemplate,
                puzzleCount = puzzleCount
            };
            
            // Create puzzles based on level
            CreatePuzzlesForLevel(puzzleSetup, puzzleCount);
            
            return puzzleSetup;
        }
        
        /// <summary>
        /// Create puzzles appropriate for the given level.
        /// </summary>
        /// <param name="puzzleSetup">Puzzle setup to populate.</param>
        /// <param name="puzzleCount">Number of puzzles to create.</param>
        private static void CreatePuzzlesForLevel(PuzzleLevelSetup puzzleSetup, int puzzleCount)
        {
            var roomObject = puzzleSetup.roomTemplate.gameObject;
            
            for (int i = 0; i < puzzleCount; i++)
            {
                // Create puzzle GameObject
                GameObject puzzleObject = new GameObject($"Puzzle_{puzzleSetup.level}_{i + 1}");
                puzzleObject.transform.SetParent(roomObject.transform);
                
                // Position puzzle in room
                float angle = (360f / puzzleCount) * i;
                float radius = 3f;
                Vector3 position = new Vector3(
                    Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
                    0f,
                    Mathf.Sin(angle * Mathf.Deg2Rad) * radius
                );
                puzzleObject.transform.localPosition = position;
                
                // Add appropriate puzzle component based on level
                AddPuzzleComponent(puzzleObject, puzzleSetup.level, i);
                
                // Add to puzzle setup
                puzzleSetup.puzzles.Add(puzzleObject);
            }
        }
        
        /// <summary>
        /// Add appropriate puzzle component based on level.
        /// </summary>
        /// <param name="puzzleObject">Puzzle GameObject to configure.</param>
        /// <param name="level">Puzzle level.</param>
        /// <param name="puzzleIndex">Index of the puzzle.</param>
        private static void AddPuzzleComponent(GameObject puzzleObject, PuzzleLevel level, int puzzleIndex)
        {
            switch (level)
            {
                case PuzzleLevel.Tutorial:
                    // Simple relic puzzle for tutorial
                    var tutorialPuzzle = puzzleObject.AddComponent<VRRelicPuzzle>();
                    ConfigureTutorialPuzzle(tutorialPuzzle, puzzleIndex);
                    break;
                    
                case PuzzleLevel.Beginner:
                    // Mix of relic and gesture puzzles
                    if (puzzleIndex % 2 == 0)
                    {
                        var relicPuzzle = puzzleObject.AddComponent<VRRelicPuzzle>();
                        ConfigureBeginnerRelicPuzzle(relicPuzzle, puzzleIndex);
                    }
                    else
                    {
                        var gesturePuzzle = puzzleObject.AddComponent<VRHandGesturePuzzle>();
                        ConfigureBeginnerGesturePuzzle(gesturePuzzle, puzzleIndex);
                    }
                    break;
                    
                case PuzzleLevel.Intermediate:
                    // More complex puzzles
                    if (puzzleIndex % 3 == 0)
                    {
                        var relicPuzzle = puzzleObject.AddComponent<VRRelicPuzzle>();
                        ConfigureIntermediateRelicPuzzle(relicPuzzle, puzzleIndex);
                    }
                    else
                    {
                        var gesturePuzzle = puzzleObject.AddComponent<VRHandGesturePuzzle>();
                        ConfigureIntermediateGesturePuzzle(gesturePuzzle, puzzleIndex);
                    }
                    break;
                    
                case PuzzleLevel.Advanced:
                case PuzzleLevel.Expert:
                case PuzzleLevel.Master:
                    // Advanced puzzles with increasing complexity
                    var advancedPuzzle = puzzleObject.AddComponent<VRRelicPuzzle>();
                    ConfigureAdvancedPuzzle(advancedPuzzle, level, puzzleIndex);
                    break;
            }
        }
        
        /// <summary>
        /// Configure tutorial puzzle - very simple.
        /// </summary>
        /// <param name="puzzle">Puzzle to configure.</param>
        /// <param name="puzzleIndex">Index of the puzzle.</param>
        private static void ConfigureTutorialPuzzle(VRRelicPuzzle puzzle, int puzzleIndex)
        {
            // Tutorial puzzles are very simple
            puzzle.requireBothHands = false;
            puzzle.enableHapticFeedback = true;
            puzzle.hapticIntensity = 0.5f;
            puzzle.slotSnapDistance = 0.15f;
            puzzle.autoSnapToSlots = true;
        }
        
        /// <summary>
        /// Configure beginner relic puzzle.
        /// </summary>
        /// <param name="puzzle">Puzzle to configure.</param>
        /// <param name="puzzleIndex">Index of the puzzle.</param>
        private static void ConfigureBeginnerRelicPuzzle(VRRelicPuzzle puzzle, int puzzleIndex)
        {
            puzzle.requireBothHands = false;
            puzzle.enableHapticFeedback = true;
            puzzle.hapticIntensity = 0.6f;
            puzzle.slotSnapDistance = 0.12f;
            puzzle.autoSnapToSlots = true;
        }
        
        /// <summary>
        /// Configure beginner gesture puzzle.
        /// </summary>
        /// <param name="puzzle">Puzzle to configure.</param>
        /// <param name="puzzleIndex">Index of the puzzle.</param>
        private static void ConfigureBeginnerGesturePuzzle(VRHandGesturePuzzle puzzle, int puzzleIndex)
        {
            puzzle.gestureHoldTime = 1.5f;
            puzzle.requireSequentialGestures = false;
            puzzle.requireBothHands = false;
            puzzle.enableHapticFeedback = true;
            puzzle.hapticIntensity = 0.5f;
        }
        
        /// <summary>
        /// Configure intermediate relic puzzle.
        /// </summary>
        /// <param name="puzzle">Puzzle to configure.</param>
        /// <param name="puzzleIndex">Index of the puzzle.</param>
        private static void ConfigureIntermediateRelicPuzzle(VRRelicPuzzle puzzle, int puzzleIndex)
        {
            puzzle.requireBothHands = puzzleIndex > 0; // Second puzzle requires both hands
            puzzle.enableHapticFeedback = true;
            puzzle.hapticIntensity = 0.7f;
            puzzle.slotSnapDistance = 0.1f;
            puzzle.autoSnapToSlots = true;
        }
        
        /// <summary>
        /// Configure intermediate gesture puzzle.
        /// </summary>
        /// <param name="puzzle">Puzzle to configure.</param>
        /// <param name="puzzleIndex">Index of the puzzle.</param>
        private static void ConfigureIntermediateGesturePuzzle(VRHandGesturePuzzle puzzle, int puzzleIndex)
        {
            puzzle.gestureHoldTime = 2f;
            puzzle.requireSequentialGestures = puzzleIndex > 0; // Second puzzle is sequential
            puzzle.requireBothHands = false;
            puzzle.enableHapticFeedback = true;
            puzzle.hapticIntensity = 0.6f;
        }
        
        /// <summary>
        /// Configure advanced puzzle for higher levels.
        /// </summary>
        /// <param name="puzzle">Puzzle to configure.</param>
        /// <param name="level">Puzzle level.</param>
        /// <param name="puzzleIndex">Index of the puzzle.</param>
        private static void ConfigureAdvancedPuzzle(VRRelicPuzzle puzzle, PuzzleLevel level, int puzzleIndex)
        {
            // Increase difficulty with level
            float difficultyMultiplier = GetDifficultyMultiplier(level);
            
            puzzle.requireBothHands = true;
            puzzle.enableHapticFeedback = true;
            puzzle.hapticIntensity = 0.8f * difficultyMultiplier;
            puzzle.slotSnapDistance = 0.08f / difficultyMultiplier; // Smaller snap distance
            puzzle.autoSnapToSlots = false; // No auto-snap for advanced puzzles
        }
        
        /// <summary>
        /// Get difficulty multiplier for a level.
        /// </summary>
        /// <param name="level">Puzzle level.</param>
        /// <returns>Difficulty multiplier.</returns>
        private static float GetDifficultyMultiplier(PuzzleLevel level)
        {
            switch (level)
            {
                case PuzzleLevel.Advanced: return 1.2f;
                case PuzzleLevel.Expert: return 1.5f;
                case PuzzleLevel.Master: return 2.0f;
                default: return 1.0f;
            }
        }
        
        /// <summary>
        /// Get room description for a level.
        /// </summary>
        /// <param name="level">Puzzle level.</param>
        /// <returns>Room description.</returns>
        public static string GetRoomDescription(PuzzleLevel level)
        {
            switch (level)
            {
                case PuzzleLevel.Tutorial: return "A welcoming chamber to learn the basics of relic puzzles.";
                case PuzzleLevel.Beginner: return "A peaceful library filled with ancient knowledge and simple puzzles.";
                case PuzzleLevel.Intermediate: return "A mysterious cavern where crystals pulse with ancient energy.";
                case PuzzleLevel.Advanced: return "A foreboding sanctum where shadows dance and puzzles test your wit.";
                case PuzzleLevel.Expert: return "A nexus of pure energy where reality bends and puzzles defy logic.";
                case PuzzleLevel.Master: return "The ultimate test of skill and determination in the depths of eternity.";
                default: return "A mysterious room filled with ancient puzzles.";
            }
        }
        
        /// <summary>
        /// Get recommended puzzle count for a level.
        /// </summary>
        /// <param name="level">Puzzle level.</param>
        /// <returns>Recommended number of puzzles.</returns>
        public static int GetRecommendedPuzzleCount(PuzzleLevel level)
        {
            switch (level)
            {
                case PuzzleLevel.Tutorial: return 2;
                case PuzzleLevel.Beginner: return 3;
                case PuzzleLevel.Intermediate: return 4;
                case PuzzleLevel.Advanced: return 5;
                case PuzzleLevel.Expert: return 6;
                case PuzzleLevel.Master: return 7;
                default: return 3;
            }
        }
    }
    
    /// <summary>
    /// Puzzle levels in Run4theRelic.
    /// </summary>
    public enum PuzzleLevel
    {
        Tutorial,
        Beginner,
        Intermediate,
        Advanced,
        Expert,
        Master
    }
    
    /// <summary>
    /// Complete setup for a puzzle level.
    /// </summary>
    [System.Serializable]
    public class PuzzleLevelSetup
    {
        [Header("Level Info")]
        public PuzzleLevel level;
        public string levelName;
        public string levelDescription;
        
        [Header("Room")]
        public VRRoomTemplate roomTemplate;
        
        [Header("Puzzles")]
        public int puzzleCount;
        public List<GameObject> puzzles = new List<GameObject>();
        
        [Header("Level Settings")]
        public bool isUnlocked = true;
        public PuzzleLevel[] requiredLevels; // Levels that must be completed first
        public int requiredRelicCount; // Relics needed to unlock this level
        
        /// <summary>
        /// Check if the level is unlocked.
        /// </summary>
        /// <returns>True if the level is unlocked.</returns>
        public bool IsLevelUnlocked()
        {
            if (!isUnlocked) return false;
            
            // Check required levels
            if (requiredLevels != null)
            {
                foreach (var requiredLevel in requiredLevels)
                {
                    // This would integrate with level completion system
                    if (!IsLevelCompleted(requiredLevel))
                    {
                        return false;
                    }
                }
            }
            
            // Check required relic count
            if (requiredRelicCount > 0)
            {
                // This would integrate with relic collection system
                int collectedRelics = GetCollectedRelicCount();
                if (collectedRelics < requiredRelicCount)
                {
                    return false;
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// Check if a level is completed.
        /// </summary>
        /// <param name="level">Level to check.</param>
        /// <returns>True if the level is completed.</returns>
        private bool IsLevelCompleted(PuzzleLevel level)
        {
            // This would integrate with level completion system
            // For now, assume all levels are completed
            return true;
        }
        
        /// <summary>
        /// Get the number of collected relics.
        /// </summary>
        /// <returns>Number of collected relics.</returns>
        private int GetCollectedRelicCount()
        {
            // This would integrate with relic collection system
            // For now, assume all relics are collected
            return 10;
        }
        
        /// <summary>
        /// Get level progress as percentage.
        /// </summary>
        /// <returns>Progress as percentage (0-100).</returns>
        public float GetLevelProgress()
        {
            if (puzzles.Count == 0) return 0f;
            
            int completedPuzzles = 0;
            foreach (var puzzle in puzzles)
            {
                if (puzzle != null)
                {
                    // Check if puzzle is completed
                    var relicPuzzle = puzzle.GetComponent<VRRelicPuzzle>();
                    if (relicPuzzle != null && relicPuzzle.IsPuzzleSolved)
                    {
                        completedPuzzles++;
                    }
                    
                    var gesturePuzzle = puzzle.GetComponent<VRHandGesturePuzzle>();
                    if (gesturePuzzle != null && gesturePuzzle.IsPuzzleSolved)
                    {
                        completedPuzzles++;
                    }
                }
            }
            
            return (float)completedPuzzles / puzzles.Count * 100f;
        }
        
        /// <summary>
        /// Check if the level is completed.
        /// </summary>
        /// <returns>True if all puzzles are solved.</returns>
        public bool IsLevelCompleted()
        {
            return GetLevelProgress() >= 100f;
        }
    }
}
