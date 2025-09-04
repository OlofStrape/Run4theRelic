using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

namespace Run4theRelic.Core
{
    /// <summary>
    /// Dynamic Story Generation System for VR gameplay
    /// Creates contextual story elements based on player actions and progress
    /// </summary>
    public class VRDynamicStoryGenerator : MonoBehaviour
    {
        [Header("Story Generation Settings")]
        [SerializeField] private float storyUpdateInterval = 15.0f;
        [SerializeField] private int maxActiveStories = 8;
        [SerializeField] private bool enableContextualGeneration = true;
        [SerializeField] private bool enableEmotionalStorytelling = true;
        
        [Header("Story Templates")]
        [SerializeField] private TextAsset storyTemplatesFile;
        [SerializeField] private bool useCustomTemplates = false;
        
        [Header("Generation Parameters")]
        [SerializeField] private float storyRelevanceThreshold = 0.6f;
        [SerializeField] private float emotionalIntensityMultiplier = 1.0f;
        [SerializeField] private int maxStoryVariations = 5;
        
        // Story Management
        private List<DynamicStoryElement> activeStories = new List<DynamicStoryElement>();
        private Dictionary<string, StoryTemplate> storyTemplates = new Dictionary<string, StoryTemplate>();
        private Queue<StoryContext> storyContextHistory = new Queue<StoryContext>();
        
        // Integration Systems
        private VRAIGameplayDirector aiDirector;
        private VRPerformanceAnalytics performanceAnalytics;
        private VREnvironmentSystem environmentSystem;
        private VRPuzzleLevelManager levelManager;
        
        // Story State
        private float currentEmotionalIntensity = 0.5f;
        private StoryMood currentMood = StoryMood.Neutral;
        private List<string> recentStoryThemes = new List<string>();
        
        // Events
        public static event Action<DynamicStoryElement> OnStoryGenerated;
        public static event Action<DynamicStoryElement> OnStoryCompleted;
        public static event Action<StoryMood> OnMoodChanged;
        public static event Action<float> OnEmotionalIntensityChanged;
        
        private void Start()
        {
            InitializeStorySystem();
            StartCoroutine(StoryGenerationLoop());
            SubscribeToEvents();
        }
        
        private void InitializeStorySystem()
        {
            LoadStoryTemplates();
            InitializeDefaultTemplates();
            
            aiDirector = FindObjectOfType<VRAIGameplayDirector>();
            performanceAnalytics = FindObjectOfType<VRPerformanceAnalytics>();
            environmentSystem = FindObjectOfType<VREnvironmentSystem>();
            levelManager = FindObjectOfType<VRPuzzleLevelManager>();
        }
        
        private void LoadStoryTemplates()
        {
            if (useCustomTemplates && storyTemplatesFile != null)
            {
                // Load custom templates from JSON file
                var templateData = JsonUtility.FromJson<StoryTemplateCollection>(storyTemplatesFile.text);
                foreach (var template in templateData.templates)
                {
                    storyTemplates[template.id] = template;
                }
            }
        }
        
        private void InitializeDefaultTemplates()
        {
            // Environmental Story Templates
            AddStoryTemplate("env_ancient_walls", StoryCategory.Environmental, new[] {
                "The ancient walls whisper secrets of forgotten civilizations...",
                "Time-worn stones hold memories of those who came before...",
                "Ancient runes pulse with mysterious energy..."
            });
            
            AddStoryTemplate("env_energy_pulse", StoryCategory.Environmental, new[] {
                "A mysterious energy pulses through the chamber...",
                "The air crackles with arcane power...",
                "Something ancient stirs in the depths..."
            });
            
            // Puzzle Hint Templates
            AddStoryTemplate("puzzle_pattern_shadow", StoryCategory.PuzzleHint, new[] {
                "Look for patterns in the shadows...",
                "The solution lies in the arrangement of elements...",
                "Connect the pieces to reveal the truth..."
            });
            
            AddStoryTemplate("puzzle_time_pressure", StoryCategory.PuzzleHint, new[] {
                "Time flows differently here...",
                "The ancient mechanism awaits your touch...",
                "Patience reveals the path forward..."
            });
            
            // Atmospheric Templates
            AddStoryTemplate("atmo_tension", StoryCategory.Atmospheric, new[] {
                "The air crackles with anticipation...",
                "Something ancient stirs in the depths...",
                "The relics hold the key to understanding..."
            });
            
            AddStoryTemplate("atmo_discovery", StoryCategory.Atmospheric, new[] {
                "Each step reveals new mysteries...",
                "The past and present converge in this place...",
                "Ancient knowledge awaits the worthy..."
            });
            
            // Progression Templates
            AddStoryTemplate("prog_advancement", StoryCategory.Progression, new[] {
                "You're getting closer to the ultimate revelation...",
                "Each puzzle solved brings new understanding...",
                "The path forward reveals itself through persistence..."
            });
            
            AddStoryTemplate("prog_mastery", StoryCategory.Progression, new[] {
                "Your skills grow with each challenge...",
                "The ancient ones would be impressed...",
                "Mastery comes through dedication and insight..."
            });
        }
        
        private void AddStoryTemplate(string id, StoryCategory category, string[] variations)
        {
            storyTemplates[id] = new StoryTemplate
            {
                id = id,
                category = category,
                variations = variations,
                emotionalRange = new Vector2(0.3f, 0.8f),
                difficultyRange = new Vector2(1f, 10f),
                contextTags = GetDefaultContextTags(category)
            };
        }
        
        private string[] GetDefaultContextTags(StoryCategory category)
        {
            switch (category)
            {
                case StoryCategory.Environmental:
                    return new[] { "environment", "atmosphere", "ancient", "mysterious" };
                case StoryCategory.PuzzleHint:
                    return new[] { "puzzle", "hint", "solution", "guidance" };
                case StoryCategory.Atmospheric:
                    return new[] { "mood", "emotion", "tension", "anticipation" };
                case StoryCategory.Progression:
                    return new[] { "progress", "advancement", "mastery", "achievement" };
                default:
                    return new[] { "general" };
            }
        }
        
        private void SubscribeToEvents()
        {
            if (performanceAnalytics != null)
            {
                performanceAnalytics.OnEngagementChanged += OnPlayerEngagementChanged;
                performanceAnalytics.OnFrustrationChanged += OnPlayerFrustrationChanged;
                performanceAnalytics.OnNewInsight += OnPerformanceInsight;
            }
            
            if (aiDirector != null)
            {
                aiDirector.OnDifficultyChanged += OnDifficultyChanged;
            }
        }
        
        private IEnumerator StoryGenerationLoop()
        {
            while (true)
            {
                if (activeStories.Count < maxActiveStories)
                {
                    GenerateContextualStory();
                }
                
                UpdateStoryElements();
                yield return new WaitForSeconds(storyUpdateInterval);
            }
        }
        
        private void GenerateContextualStory()
        {
            if (!enableContextualGeneration) return;
            
            var context = GatherStoryContext();
            var relevantTemplates = FindRelevantTemplates(context);
            
            if (relevantTemplates.Count == 0) return;
            
            // Select best template based on context relevance
            var selectedTemplate = SelectBestTemplate(relevantTemplates, context);
            var storyElement = CreateStoryElement(selectedTemplate, context);
            
            activeStories.Add(storyElement);
            OnStoryGenerated?.Invoke(storyElement);
            
            // Update context history
            storyContextHistory.Enqueue(context);
            if (storyContextHistory.Count > 20)
            {
                storyContextHistory.Dequeue();
            }
        }
        
        private StoryContext GatherStoryContext()
        {
            var context = new StoryContext
            {
                timestamp = Time.time,
                playerEngagement = performanceAnalytics?.GetEngagementLevel() ?? 0.5f,
                playerFrustration = performanceAnalytics?.GetFrustrationLevel() ?? 0.0f,
                playerMastery = performanceAnalytics?.GetMasteryLevel() ?? 0.0f,
                currentDifficulty = aiDirector?.GetCurrentDifficulty() ?? 5.0f,
                activePuzzleCount = GetActivePuzzleCount(),
                roomType = GetCurrentRoomType(),
                recentActions = GetRecentPlayerActions(),
                environmentalFactors = GetEnvironmentalFactors()
            };
            
            return context;
        }
        
        private List<StoryTemplate> FindRelevantTemplates(StoryContext context)
        {
            var relevantTemplates = new List<StoryTemplate>();
            
            foreach (var template in storyTemplates.Values)
            {
                float relevance = CalculateTemplateRelevance(template, context);
                if (relevance >= storyRelevanceThreshold)
                {
                    relevantTemplates.Add(template);
                }
            }
            
            return relevantTemplates.OrderByDescending(t => CalculateTemplateRelevance(t, context)).ToList();
        }
        
        private float CalculateTemplateRelevance(StoryTemplate template, StoryContext context)
        {
            float relevance = 0.5f; // Base relevance
            
            // Category relevance
            if (template.category == StoryCategory.PuzzleHint && context.activePuzzleCount > 0)
            {
                relevance += 0.3f;
            }
            
            if (template.category == StoryCategory.Environmental)
            {
                relevance += 0.2f;
            }
            
            // Difficulty relevance
            if (context.currentDifficulty >= template.difficultyRange.x && 
                context.currentDifficulty <= template.difficultyRange.y)
            {
                relevance += 0.2f;
            }
            
            // Emotional relevance
            float emotionalDistance = Mathf.Abs(currentEmotionalIntensity - 
                (template.emotionalRange.x + template.emotionalRange.y) * 0.5f);
            relevance += (1f - emotionalDistance) * 0.2f;
            
            // Context tag relevance
            relevance += CalculateContextTagRelevance(template.contextTags, context);
            
            return Mathf.Clamp01(relevance);
        }
        
        private float CalculateContextTagRelevance(string[] templateTags, StoryContext context)
        {
            float relevance = 0f;
            
            foreach (var tag in templateTags)
            {
                if (context.recentActions.Contains(tag) || context.environmentalFactors.Contains(tag))
                {
                    relevance += 0.1f;
                }
            }
            
            return Mathf.Clamp01(relevance);
        }
        
        private StoryTemplate SelectBestTemplate(List<StoryTemplate> templates, StoryContext context)
        {
            // Avoid repeating recent themes
            var filteredTemplates = templates.Where(t => 
                !recentStoryThemes.Contains(t.id)).ToList();
            
            if (filteredTemplates.Count == 0)
            {
                filteredTemplates = templates; // Reset if all themes used
                recentStoryThemes.Clear();
            }
            
            // Select template with highest relevance and variety
            var selectedTemplate = filteredTemplates.First();
            recentStoryThemes.Add(selectedTemplate.id);
            
            // Keep recent themes list manageable
            if (recentStoryThemes.Count > maxStoryVariations)
            {
                recentStoryThemes.RemoveAt(0);
            }
            
            return selectedTemplate;
        }
        
        private DynamicStoryElement CreateStoryElement(StoryTemplate template, StoryContext context)
        {
            var storyElement = new DynamicStoryElement
            {
                id = Guid.NewGuid().ToString(),
                templateId = template.id,
                category = template.category,
                content = SelectStoryVariation(template, context),
                emotionalIntensity = CalculateEmotionalIntensity(template, context),
                relevance = CalculateTemplateRelevance(template, context),
                context = context,
                timestamp = Time.time,
                isActive = true,
                lifetime = CalculateStoryLifetime(template, context)
            };
            
            return storyElement;
        }
        
        private string SelectStoryVariation(StoryTemplate template, StoryContext context)
        {
            if (template.variations.Length == 0) return "The mystery deepens...";
            
            // Select variation based on context
            int variationIndex = 0;
            
            if (context.playerFrustration > 0.7f)
            {
                variationIndex = Mathf.Min(1, template.variations.Length - 1);
            }
            else if (context.playerMastery > 0.8f)
            {
                variationIndex = Mathf.Min(2, template.variations.Length - 1);
            }
            
            return template.variations[variationIndex];
        }
        
        private float CalculateEmotionalIntensity(StoryTemplate template, StoryContext context)
        {
            float baseIntensity = (template.emotionalRange.x + template.emotionalRange.y) * 0.5f;
            
            // Adjust based on player state
            if (context.playerFrustration > 0.6f)
            {
                baseIntensity *= 1.3f;
            }
            
            if (context.playerMastery > 0.7f)
            {
                baseIntensity *= 1.2f;
            }
            
            return Mathf.Clamp01(baseIntensity * emotionalIntensityMultiplier);
        }
        
        private float CalculateStoryLifetime(StoryTemplate template, StoryContext context)
        {
            float baseLifetime = 60f; // 1 minute base
            
            // Adjust based on category and context
            switch (template.category)
            {
                case StoryCategory.PuzzleHint:
                    baseLifetime = context.activePuzzleCount > 0 ? 120f : 60f;
                    break;
                case StoryCategory.Environmental:
                    baseLifetime = 90f;
                    break;
                case StoryCategory.Atmospheric:
                    baseLifetime = 150f;
                    break;
                case StoryCategory.Progression:
                    baseLifetime = 180f;
                    break;
            }
            
            return baseLifetime;
        }
        
        private void UpdateStoryElements()
        {
            var completedStories = new List<DynamicStoryElement>();
            
            foreach (var story in activeStories)
            {
                if (Time.time - story.timestamp > story.lifetime)
                {
                    story.isActive = false;
                    completedStories.Add(story);
                }
            }
            
            // Remove completed stories
            foreach (var completedStory in completedStories)
            {
                activeStories.Remove(completedStory);
                OnStoryCompleted?.Invoke(completedStory);
            }
        }
        
        // Event Handlers
        private void OnPlayerEngagementChanged(float engagement)
        {
            UpdateEmotionalState(engagement, currentEmotionalIntensity);
        }
        
        private void OnPlayerFrustrationChanged(float frustration)
        {
            UpdateEmotionalState(currentEmotionalIntensity, frustration);
        }
        
        private void OnPerformanceInsight(PerformanceInsight insight)
        {
            // Generate story based on performance insight
            if (insight.priority == InsightPriority.High)
            {
                GenerateInsightBasedStory(insight);
            }
        }
        
        private void OnDifficultyChanged(float difficulty)
        {
            // Adjust story generation based on difficulty
            if (difficulty > 7f)
            {
                // High difficulty - more encouraging stories
                GenerateEncouragingStory();
            }
        }
        
        private void UpdateEmotionalState(float engagement, float frustration)
        {
            float newIntensity = (engagement + (1f - frustration)) * 0.5f;
            bool intensityChanged = Mathf.Abs(currentEmotionalIntensity - newIntensity) > 0.1f;
            
            if (intensityChanged)
            {
                currentEmotionalIntensity = newIntensity;
                OnEmotionalIntensityChanged?.Invoke(currentEmotionalIntensity);
                
                // Update mood based on emotional state
                UpdateStoryMood();
            }
        }
        
        private void UpdateStoryMood()
        {
            StoryMood newMood;
            
            if (currentEmotionalIntensity > 0.7f)
            {
                newMood = StoryMood.Excited;
            }
            else if (currentEmotionalIntensity > 0.4f)
            {
                newMood = StoryMood.Calm;
            }
            else
            {
                newMood = StoryMood.Tense;
            }
            
            if (newMood != currentMood)
            {
                currentMood = newMood;
                OnMoodChanged?.Invoke(currentMood);
            }
        }
        
        private void GenerateInsightBasedStory(PerformanceInsight insight)
        {
            // Generate specific story based on insight type
            string storyContent = "";
            
            switch (insight.type)
            {
                case InsightType.HighFrustration:
                    storyContent = "Take a moment to breathe. The ancient ones faced similar challenges...";
                    break;
                case InsightType.LowEngagement:
                    storyContent = "Sometimes the greatest discoveries come from unexpected places...";
                    break;
                case InsightType.HighMastery:
                    storyContent = "Your skill has grown beyond what the ancients could have imagined...";
                    break;
            }
            
            if (!string.IsNullOrEmpty(storyContent))
            {
                var storyElement = new DynamicStoryElement
                {
                    id = Guid.NewGuid().ToString(),
                    templateId = "insight_based",
                    category = StoryCategory.Atmospheric,
                    content = storyContent,
                    emotionalIntensity = 0.8f,
                    relevance = 1.0f,
                    timestamp = Time.time,
                    isActive = true,
                    lifetime = 90f
                };
                
                activeStories.Add(storyElement);
                OnStoryGenerated?.Invoke(storyElement);
            }
        }
        
        private void GenerateEncouragingStory()
        {
            var encouragingTemplates = new[] {
                "The path may be challenging, but you have the strength to overcome...",
                "Each obstacle overcome brings you closer to mastery...",
                "The ancients designed these trials to test the worthy..."
            };
            
            var storyElement = new DynamicStoryElement
            {
                id = Guid.NewGuid().ToString(),
                templateId = "encouragement",
                category = StoryCategory.Progression,
                content = encouragingTemplates[UnityEngine.Random.Range(0, encouragingTemplates.Length)],
                emotionalIntensity = 0.7f,
                relevance = 0.9f,
                timestamp = Time.time,
                isActive = true,
                lifetime = 120f
            };
            
            activeStories.Add(storyElement);
            OnStoryGenerated?.Invoke(storyElement);
        }
        
        // Helper Methods (Placeholders for integration)
        private int GetActivePuzzleCount() => UnityEngine.Random.Range(1, 5);
        private RoomType GetCurrentRoomType() => RoomType.PuzzleRoom;
        private string[] GetRecentPlayerActions() => new[] { "movement", "interaction" };
        private string[] GetEnvironmentalFactors() => new[] { "ancient", "mysterious" };
        
        // Public API
        public List<DynamicStoryElement> GetActiveStories() => new List<DynamicStoryElement>(activeStories);
        public StoryMood GetCurrentMood() => currentMood;
        public float GetEmotionalIntensity() => currentEmotionalIntensity;
        
        public void ForceStoryGeneration(StoryCategory category = StoryCategory.Atmospheric)
        {
            var context = GatherStoryContext();
            var templates = storyTemplates.Values.Where(t => t.category == category).ToList();
            
            if (templates.Count > 0)
            {
                var template = templates[UnityEngine.Random.Range(0, templates.Count)];
                var storyElement = CreateStoryElement(template, context);
                
                activeStories.Add(storyElement);
                OnStoryGenerated?.Invoke(storyElement);
            }
        }
    }
    
    [System.Serializable]
    public class DynamicStoryElement
    {
        public string id;
        public string templateId;
        public StoryCategory category;
        public string content;
        public float emotionalIntensity;
        public float relevance;
        public StoryContext context;
        public float timestamp;
        public bool isActive;
        public float lifetime;
    }
    
    [System.Serializable]
    public class StoryTemplate
    {
        public string id;
        public StoryCategory category;
        public string[] variations;
        public Vector2 emotionalRange;
        public Vector2 difficultyRange;
        public string[] contextTags;
    }
    
    [System.Serializable]
    public class StoryContext
    {
        public float timestamp;
        public float playerEngagement;
        public float playerFrustration;
        public float playerMastery;
        public float currentDifficulty;
        public int activePuzzleCount;
        public RoomType roomType;
        public string[] recentActions;
        public string[] environmentalFactors;
    }
    
    [System.Serializable]
    public class StoryTemplateCollection
    {
        public StoryTemplate[] templates;
    }
    
    public enum StoryCategory
    {
        Environmental,
        PuzzleHint,
        Atmospheric,
        Progression
    }
    
    public enum StoryMood
    {
        Tense,
        Calm,
        Excited,
        Neutral
    }
}
