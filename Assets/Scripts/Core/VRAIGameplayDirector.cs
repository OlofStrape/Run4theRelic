using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Run4theRelic.Core
{
    /// <summary>
    /// Central AI Director that manages the entire VR gameplay experience
    /// Handles difficulty adjustment, adaptive learning, and dynamic story generation
    /// </summary>
    public class VRAIGameplayDirector : MonoBehaviour
    {
        [Header("AI Director Settings")]
        [SerializeField] private float updateInterval = 2.0f;
        [SerializeField] private int maxDifficultyLevel = 10;
        [SerializeField] private float difficultyAdjustmentSpeed = 0.5f;
        
        [Header("Performance Tracking")]
        [SerializeField] private int performanceHistorySize = 20;
        [SerializeField] private float minPerformanceThreshold = 0.3f;
        [SerializeField] private float maxPerformanceThreshold = 0.8f;
        
        [Header("Story Generation")]
        [SerializeField] private bool enableDynamicStory = true;
        [SerializeField] private float storyUpdateInterval = 30.0f;
        [SerializeField] private int maxStoryElements = 5;
        
        // Core Systems
        private VRContentGenerator contentGenerator;
        private VRPuzzleLevelManager levelManager;
        private VREnvironmentSystem environmentSystem;
        
        // AI State
        private float currentDifficulty = 5.0f;
        private float targetDifficulty = 5.0f;
        private Queue<float> performanceHistory = new Queue<float>();
        private Dictionary<string, float> playerSkillProfile = new Dictionary<string, float>();
        private List<StoryElement> activeStoryElements = new List<StoryElement>();
        
        // Events
        public static event System.Action<float> OnDifficultyChanged;
        public static event System.Action<StoryElement> OnStoryElementGenerated;
        public static event System.Action<PlayerSkillUpdate> OnPlayerSkillUpdated;
        
        private void Start()
        {
            InitializeSystems();
            StartCoroutine(AIDirectorLoop());
            if (enableDynamicStory)
            {
                StartCoroutine(StoryGenerationLoop());
            }
        }
        
        private void InitializeSystems()
        {
            contentGenerator = FindObjectOfType<VRContentGenerator>();
            levelManager = FindObjectOfType<VRPuzzleLevelManager>();
            environmentSystem = FindObjectOfType<VREnvironmentSystem>();
            
            if (contentGenerator == null)
            {
                Debug.LogWarning("VRContentGenerator not found! AI Director will have limited functionality.");
            }
        }
        
        private IEnumerator AIDirectorLoop()
        {
            while (true)
            {
                AnalyzePlayerPerformance();
                AdjustDifficulty();
                UpdatePlayerSkillProfile();
                GenerateAdaptiveContent();
                
                yield return new WaitForSeconds(updateInterval);
            }
        }
        
        private IEnumerator StoryGenerationLoop()
        {
            while (true)
            {
                if (activeStoryElements.Count < maxStoryElements)
                {
                    GenerateStoryElement();
                }
                
                yield return new WaitForSeconds(storyUpdateInterval);
            }
        }
        
        private void AnalyzePlayerPerformance()
        {
            if (levelManager == null) return;
            
            float currentPerformance = CalculateCurrentPerformance();
            performanceHistory.Enqueue(currentPerformance);
            
            if (performanceHistory.Count > performanceHistorySize)
            {
                performanceHistory.Dequeue();
            }
            
            // Calculate average performance
            float avgPerformance = performanceHistory.Average();
            
            // Adjust target difficulty based on performance
            if (avgPerformance < minPerformanceThreshold)
            {
                targetDifficulty = Mathf.Max(1.0f, targetDifficulty - difficultyAdjustmentSpeed);
            }
            else if (avgPerformance > maxPerformanceThreshold)
            {
                targetDifficulty = Mathf.Min(maxDifficultyLevel, targetDifficulty + difficultyAdjustmentSpeed);
            }
        }
        
        private float CalculateCurrentPerformance()
        {
            if (levelManager == null) return 0.5f;
            
            // Calculate based on recent puzzle completion rates
            float completionRate = levelManager.GetRecentCompletionRate();
            float averageTime = levelManager.GetRecentAverageTime();
            float difficultyMultiplier = levelManager.GetCurrentLevelDifficulty();
            
            // Normalize completion rate (0-1)
            float normalizedCompletion = Mathf.Clamp01(completionRate);
            
            // Normalize time (faster = better, but not too fast)
            float normalizedTime = Mathf.Clamp01(1.0f - (averageTime / 300.0f)); // 5 min baseline
            
            // Combine metrics with difficulty consideration
            float performance = (normalizedCompletion * 0.6f + normalizedTime * 0.4f) * difficultyMultiplier;
            
            return Mathf.Clamp01(performance);
        }
        
        private void AdjustDifficulty()
        {
            if (Mathf.Abs(currentDifficulty - targetDifficulty) > 0.1f)
            {
                currentDifficulty = Mathf.Lerp(currentDifficulty, targetDifficulty, Time.deltaTime * 0.5f);
                OnDifficultyChanged?.Invoke(currentDifficulty);
                
                // Update content generation parameters
                if (contentGenerator != null)
                {
                    contentGenerator.SetDifficultyLevel(Mathf.RoundToInt(currentDifficulty));
                }
            }
        }
        
        private void UpdatePlayerSkillProfile()
        {
            if (levelManager == null) return;
            
            var skillUpdates = new List<PlayerSkillUpdate>();
            
            // Analyze puzzle type performance
            foreach (var puzzleType in System.Enum.GetValues(typeof(PuzzleType)))
            {
                float skillLevel = CalculateSkillLevelForPuzzleType((PuzzleType)puzzleType);
                string puzzleKey = puzzleType.ToString();
                
                if (!playerSkillProfile.ContainsKey(puzzleKey) || 
                    Mathf.Abs(playerSkillProfile[puzzleKey] - skillLevel) > 0.1f)
                {
                    playerSkillProfile[puzzleKey] = skillLevel;
                    skillUpdates.Add(new PlayerSkillUpdate
                    {
                        puzzleType = (PuzzleType)puzzleType,
                        skillLevel = skillLevel,
                        timestamp = Time.time
                    });
                }
            }
            
            // Notify listeners of skill updates
            foreach (var update in skillUpdates)
            {
                OnPlayerSkillUpdated?.Invoke(update);
            }
        }
        
        private float CalculateSkillLevelForPuzzleType(PuzzleType puzzleType)
        {
            if (levelManager == null) return 0.5f;
            
            // Get performance data for specific puzzle type
            var stats = levelManager.GetPuzzleTypeStatistics(puzzleType);
            if (stats == null) return 0.5f;
            
            float completionRate = stats.completionRate;
            float averageTime = stats.averageCompletionTime;
            int attempts = stats.totalAttempts;
            
            if (attempts < 3) return 0.5f; // Not enough data
            
            // Calculate skill level (0-1)
            float timeScore = Mathf.Clamp01(1.0f - (averageTime / 600.0f)); // 10 min baseline
            float skillLevel = (completionRate * 0.7f + timeScore * 0.3f);
            
            return Mathf.Clamp01(skillLevel);
        }
        
        private void GenerateAdaptiveContent()
        {
            if (contentGenerator == null) return;
            
            // Adjust content generation based on player skill profile
            var adaptiveSettings = new AdaptiveContentSettings
            {
                preferredPuzzleTypes = GetPreferredPuzzleTypes(),
                difficultyRange = new Vector2(
                    Mathf.Max(1.0f, currentDifficulty - 2.0f),
                    Mathf.Min(maxDifficultyLevel, currentDifficulty + 2.0f)
                ),
                complexityMultiplier = CalculateComplexityMultiplier(),
                themeVariation = CalculateThemeVariation()
            };
            
            contentGenerator.SetAdaptiveSettings(adaptiveSettings);
        }
        
        private List<PuzzleType> GetPreferredPuzzleTypes()
        {
            var preferred = new List<PuzzleType>();
            
            // Sort puzzle types by player skill (prefer types player is good at)
            var sortedTypes = playerSkillProfile
                .OrderByDescending(kvp => kvp.Value)
                .Take(3)
                .Select(kvp => System.Enum.Parse<PuzzleType>(kvp.Key))
                .ToList();
            
            preferred.AddRange(sortedTypes);
            
            // Add some variety (include types player hasn't mastered)
            var remainingTypes = System.Enum.GetValues<PuzzleType>()
                .Where(pt => !preferred.Contains(pt))
                .ToList();
            
            if (remainingTypes.Count > 0)
            {
                preferred.AddRange(remainingTypes.Take(2));
            }
            
            return preferred;
        }
        
        private float CalculateComplexityMultiplier()
        {
            float avgSkill = playerSkillProfile.Values.Count > 0 ? 
                playerSkillProfile.Values.Average() : 0.5f;
            
            // Higher skill = higher complexity
            return Mathf.Lerp(0.5f, 1.5f, avgSkill);
        }
        
        private float CalculateThemeVariation()
        {
            // Increase variation as player progresses
            float progress = levelManager?.GetPlayerProgress() ?? 0.5f;
            return Mathf.Lerp(0.3f, 1.0f, progress);
        }
        
        private void GenerateStoryElement()
        {
            if (levelManager == null) return;
            
            var storyElement = new StoryElement
            {
                id = System.Guid.NewGuid().ToString(),
                type = GetRandomStoryType(),
                content = GenerateStoryContent(),
                difficulty = currentDifficulty,
                timestamp = Time.time,
                isActive = true
            };
            
            activeStoryElements.Add(storyElement);
            OnStoryElementGenerated?.Invoke(storyElement);
            
            // Clean up old story elements
            activeStoryElements.RemoveAll(se => Time.time - se.timestamp > 300.0f); // 5 min lifetime
        }
        
        private StoryType GetRandomStoryType()
        {
            var types = System.Enum.GetValues<StoryType>();
            return types[Random.Range(0, types.Length)];
        }
        
        private string GenerateStoryContent()
        {
            var templates = new Dictionary<StoryType, string[]>
            {
                { StoryType.Environmental, new[] {
                    "The ancient walls whisper secrets of forgotten civilizations...",
                    "A mysterious energy pulses through the chamber...",
                    "Time seems to flow differently in this place..."
                }},
                { StoryType.PuzzleHint, new[] {
                    "Look for patterns in the shadows...",
                    "The solution lies in the arrangement of elements...",
                    "Connect the pieces to reveal the truth..."
                }},
                { StoryType.Atmospheric, new[] {
                    "The air crackles with anticipation...",
                    "Something ancient stirs in the depths...",
                    "The relics hold the key to understanding..."
                }},
                { StoryType.Progression, new[] {
                    "You're getting closer to the ultimate revelation...",
                    "Each puzzle solved brings new understanding...",
                    "The path forward reveals itself through persistence..."
                }}
            };
            
            var type = GetRandomStoryType();
            if (templates.ContainsKey(type))
            {
                var options = templates[type];
                return options[Random.Range(0, options.Length)];
            }
            
            return "The mystery deepens...";
        }
        
        // Public API
        public float GetCurrentDifficulty() => currentDifficulty;
        public float GetTargetDifficulty() => targetDifficulty;
        public Dictionary<string, float> GetPlayerSkillProfile() => new Dictionary<string, float>(playerSkillProfile);
        public List<StoryElement> GetActiveStoryElements() => new List<StoryElement>(activeStoryElements);
        
        public void SetDifficulty(float difficulty)
        {
            targetDifficulty = Mathf.Clamp(difficulty, 1.0f, maxDifficultyLevel);
        }
        
        public void ForceStoryGeneration()
        {
            GenerateStoryElement();
        }
    }
    
    [System.Serializable]
    public class StoryElement
    {
        public string id;
        public StoryType type;
        public string content;
        public float difficulty;
        public float timestamp;
        public bool isActive;
    }
    
    [System.Serializable]
    public class PlayerSkillUpdate
    {
        public PuzzleType puzzleType;
        public float skillLevel;
        public float timestamp;
    }
    
    [System.Serializable]
    public class AdaptiveContentSettings
    {
        public List<PuzzleType> preferredPuzzleTypes;
        public Vector2 difficultyRange;
        public float complexityMultiplier;
        public float themeVariation;
    }
    
    public enum StoryType
    {
        Environmental,
        PuzzleHint,
        Atmospheric,
        Progression
    }
}
