using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

namespace Run4theRelic.Core
{
    /// <summary>
    /// Advanced Performance Analytics Engine for VR gameplay
    /// Tracks detailed metrics and provides insights for AI-driven gameplay
    /// </summary>
    public class VRPerformanceAnalytics : MonoBehaviour
    {
        [Header("Analytics Settings")]
        [SerializeField] private float dataCollectionInterval = 1.0f;
        [SerializeField] private int maxDataPoints = 1000;
        [SerializeField] private bool enableRealTimeAnalysis = true;
        [SerializeField] private bool enablePerformancePrediction = true;
        
        [Header("Performance Thresholds")]
        [SerializeField] private float frustrationThreshold = 0.2f;
        [SerializeField] private float engagementThreshold = 0.7f;
        [SerializeField] private float masteryThreshold = 0.9f;
        
        // Data Storage
        private Queue<PerformanceDataPoint> performanceHistory = new Queue<PerformanceDataPoint>();
        private Dictionary<PuzzleType, PuzzleTypeAnalytics> puzzleTypeAnalytics = new Dictionary<PuzzleType, PuzzleTypeAnalytics>();
        private Dictionary<string, PlayerBehaviorPattern> behaviorPatterns = new Dictionary<string, PlayerBehaviorPattern>();
        
        // Real-time Analysis
        private float currentEngagementLevel = 0.5f;
        private float currentFrustrationLevel = 0.0f;
        private float currentMasteryLevel = 0.0f;
        private List<PerformanceInsight> activeInsights = new List<PerformanceInsight>();
        
        // Events
        public static event Action<PerformanceInsight> OnNewInsight;
        public static event Action<float> OnEngagementChanged;
        public static event Action<float> OnFrustrationChanged;
        public static event Action<float> OnMasteryChanged;
        public static event Action<PlayerBehaviorPattern> OnBehaviorPatternDetected;
        
        private void Start()
        {
            InitializeAnalytics();
            StartCoroutine(DataCollectionLoop());
            if (enableRealTimeAnalysis)
            {
                StartCoroutine(RealTimeAnalysisLoop());
            }
        }
        
        private void InitializeAnalytics()
        {
            // Initialize analytics for each puzzle type
            foreach (PuzzleType puzzleType in Enum.GetValues(typeof(PuzzleType)))
            {
                puzzleTypeAnalytics[puzzleType] = new PuzzleTypeAnalytics
                {
                    puzzleType = puzzleType,
                    totalAttempts = 0,
                    successfulCompletions = 0,
                    averageCompletionTime = 0f,
                    bestCompletionTime = float.MaxValue,
                    difficultyProgression = new List<float>(),
                    playerSatisfaction = new List<float>()
                };
            }
        }
        
        private IEnumerator DataCollectionLoop()
        {
            while (true)
            {
                CollectPerformanceData();
                yield return new WaitForSeconds(dataCollectionInterval);
            }
        }
        
        private IEnumerator RealTimeAnalysisLoop()
        {
            while (true)
            {
                AnalyzePerformanceTrends();
                DetectBehaviorPatterns();
                GenerateInsights();
                yield return new WaitForSeconds(dataCollectionInterval * 2);
            }
        }
        
        private void CollectPerformanceData()
        {
            var dataPoint = new PerformanceDataPoint
            {
                timestamp = Time.time,
                engagementLevel = CalculateCurrentEngagement(),
                frustrationLevel = CalculateCurrentFrustration(),
                masteryLevel = CalculateCurrentMastery(),
                activePuzzleCount = GetActivePuzzleCount(),
                playerMovement = GetPlayerMovementMetrics(),
                interactionFrequency = GetInteractionFrequency(),
                puzzleProgress = GetOverallPuzzleProgress()
            };
            
            performanceHistory.Enqueue(dataPoint);
            
            // Maintain data size limit
            while (performanceHistory.Count > maxDataPoints)
            {
                performanceHistory.Dequeue();
            }
            
            // Update real-time metrics
            UpdateRealTimeMetrics(dataPoint);
        }
        
        private float CalculateCurrentEngagement()
        {
            if (performanceHistory.Count < 5) return 0.5f;
            
            var recentData = performanceHistory.TakeLast(5).ToList();
            
            float movementEngagement = recentData.Average(dp => dp.playerMovement.intensity);
            float interactionEngagement = recentData.Average(dp => dp.interactionFrequency);
            float progressEngagement = recentData.Average(dp => dp.puzzleProgress);
            
            // Weighted combination
            float engagement = (movementEngagement * 0.3f + 
                              interactionEngagement * 0.4f + 
                              progressEngagement * 0.3f);
            
            return Mathf.Clamp01(engagement);
        }
        
        private float CalculateCurrentFrustration()
        {
            if (performanceHistory.Count < 10) return 0.0f;
            
            var recentData = performanceHistory.TakeLast(10).ToList();
            
            // Detect frustration indicators
            float failedAttempts = recentData.Count(dp => dp.puzzleProgress < 0.1f);
            float slowProgress = recentData.Count(dp => dp.puzzleProgress < 0.3f);
            float lowInteraction = recentData.Count(dp => dp.interactionFrequency < 0.2f);
            
            float frustrationScore = (failedAttempts * 0.4f + slowProgress * 0.3f + lowInteraction * 0.3f) / 10f;
            
            return Mathf.Clamp01(frustrationScore);
        }
        
        private float CalculateCurrentMastery()
        {
            if (puzzleTypeAnalytics.Count == 0) return 0.0f;
            
            var masteryScores = new List<float>();
            
            foreach (var analytics in puzzleTypeAnalytics.Values)
            {
                if (analytics.totalAttempts < 3) continue;
                
                float completionRate = (float)analytics.successfulCompletions / analytics.totalAttempts;
                float timeEfficiency = Mathf.Clamp01(1.0f - (analytics.averageCompletionTime / 600f)); // 10 min baseline
                float difficultyHandling = analytics.difficultyProgression.Count > 0 ? 
                    analytics.difficultyProgression.Average() : 0.5f;
                
                float mastery = (completionRate * 0.5f + timeEfficiency * 0.3f + difficultyHandling * 0.2f);
                masteryScores.Add(mastery);
            }
            
            return masteryScores.Count > 0 ? masteryScores.Average() : 0.0f;
        }
        
        private int GetActivePuzzleCount()
        {
            // This would integrate with the puzzle system
            return UnityEngine.Random.Range(1, 5); // Placeholder
        }
        
        private PlayerMovementMetrics GetPlayerMovementMetrics()
        {
            // This would integrate with VR input system
            return new PlayerMovementMetrics
            {
                intensity = UnityEngine.Random.Range(0.1f, 1.0f), // Placeholder
                direction = UnityEngine.Random.insideUnitSphere,
                speed = UnityEngine.Random.Range(0.5f, 2.0f)
            };
        }
        
        private float GetInteractionFrequency()
        {
            // This would integrate with VR interaction system
            return UnityEngine.Random.Range(0.1f, 1.0f); // Placeholder
        }
        
        private float GetOverallPuzzleProgress()
        {
            // This would integrate with level manager
            return UnityEngine.Random.Range(0.0f, 1.0f); // Placeholder
        }
        
        private void UpdateRealTimeMetrics(PerformanceDataPoint dataPoint)
        {
            bool engagementChanged = Mathf.Abs(currentEngagementLevel - dataPoint.engagementLevel) > 0.1f;
            bool frustrationChanged = Mathf.Abs(currentFrustrationLevel - dataPoint.frustrationLevel) > 0.1f;
            bool masteryChanged = Mathf.Abs(currentMasteryLevel - dataPoint.masteryLevel) > 0.1f;
            
            currentEngagementLevel = dataPoint.engagementLevel;
            currentFrustrationLevel = dataPoint.frustrationLevel;
            currentMasteryLevel = dataPoint.masteryLevel;
            
            if (engagementChanged) OnEngagementChanged?.Invoke(currentEngagementLevel);
            if (frustrationChanged) OnFrustrationChanged?.Invoke(currentFrustrationLevel);
            if (masteryChanged) OnMasteryChanged?.Invoke(currentMasteryLevel);
        }
        
        private void AnalyzePerformanceTrends()
        {
            if (performanceHistory.Count < 20) return;
            
            var recentData = performanceHistory.TakeLast(20).ToList();
            var olderData = performanceHistory.TakeLast(40).Take(20).ToList();
            
            if (olderData.Count < 20) return;
            
            // Calculate trend changes
            float recentEngagement = recentData.Average(dp => dp.engagementLevel);
            float olderEngagement = olderData.Average(dp => dp.engagementLevel);
            float engagementTrend = recentEngagement - olderEngagement;
            
            float recentFrustration = recentData.Average(dp => dp.frustrationLevel);
            float olderFrustration = olderData.Average(dp => dp.frustrationLevel);
            float frustrationTrend = recentFrustration - olderFrustration;
            
            // Generate insights based on trends
            if (engagementTrend < -0.2f)
            {
                GenerateInsight(InsightType.EngagementDecline, "Player engagement is declining", engagementTrend);
            }
            
            if (frustrationTrend > 0.3f)
            {
                GenerateInsight(InsightType.FrustrationIncrease, "Player frustration is increasing", frustrationTrend);
            }
            
            if (engagementTrend > 0.2f && frustrationTrend < -0.1f)
            {
                GenerateInsight(InsightType.OptimalExperience, "Player is in optimal flow state", engagementTrend);
            }
        }
        
        private void DetectBehaviorPatterns()
        {
            if (performanceHistory.Count < 50) return;
            
            var data = performanceHistory.ToList();
            
            // Detect movement patterns
            DetectMovementPatterns(data);
            
            // Detect interaction patterns
            DetectInteractionPatterns(data);
            
            // Detect puzzle-solving patterns
            DetectPuzzleSolvingPatterns(data);
        }
        
        private void DetectMovementPatterns(List<PerformanceDataPoint> data)
        {
            // Analyze movement intensity patterns
            var movementIntensities = data.Select(dp => dp.playerMovement.intensity).ToList();
            var averageIntensity = movementIntensities.Average();
            var intensityVariance = CalculateVariance(movementIntensities);
            
            if (intensityVariance < 0.1f && averageIntensity > 0.7f)
            {
                var pattern = new PlayerBehaviorPattern
                {
                    type = BehaviorPatternType.ConsistentHighActivity,
                    confidence = 0.8f,
                    description = "Player maintains consistent high activity level",
                    timestamp = Time.time
                };
                
                if (!behaviorPatterns.ContainsKey(pattern.type.ToString()) || 
                    Time.time - behaviorPatterns[pattern.type.ToString()].timestamp > 300f)
                {
                    behaviorPatterns[pattern.type.ToString()] = pattern;
                    OnBehaviorPatternDetected?.Invoke(pattern);
                }
            }
        }
        
        private void DetectInteractionPatterns(List<PerformanceDataPoint> data)
        {
            // Analyze interaction frequency patterns
            var interactionFrequencies = data.Select(dp => dp.interactionFrequency).ToList();
            var averageFrequency = interactionFrequencies.Average();
            var frequencyVariance = CalculateVariance(interactionFrequencies);
            
            if (frequencyVariance < 0.15f && averageFrequency < 0.3f)
            {
                var pattern = new PlayerBehaviorPattern
                {
                    type = BehaviorPatternType.LowInteraction,
                    confidence = 0.7f,
                    description = "Player shows consistently low interaction",
                    timestamp = Time.time
                };
                
                if (!behaviorPatterns.ContainsKey(pattern.type.ToString()) || 
                    Time.time - behaviorPatterns[pattern.type.ToString()].timestamp > 300f)
                {
                    behaviorPatterns[pattern.type.ToString()] = pattern;
                    OnBehaviorPatternDetected?.Invoke(pattern);
                }
            }
        }
        
        private void DetectPuzzleSolvingPatterns(List<PerformanceDataPoint> data)
        {
            // Analyze puzzle progress patterns
            var progressData = data.Select(dp => dp.puzzleProgress).ToList();
            var progressVariance = CalculateVariance(progressData);
            
            if (progressVariance < 0.05f && progressData.Last() < 0.2f)
            {
                var pattern = new PlayerBehaviorPattern
                {
                    type = BehaviorPatternType.StuckOnPuzzle,
                    confidence = 0.9f,
                    description = "Player appears stuck on current puzzle",
                    timestamp = Time.time
                };
                
                if (!behaviorPatterns.ContainsKey(pattern.type.ToString()) || 
                    Time.time - behaviorPatterns[pattern.type.ToString()].timestamp > 300f)
                {
                    behaviorPatterns[pattern.type.ToString()] = pattern;
                    OnBehaviorPatternDetected?.Invoke(pattern);
                }
            }
        }
        
        private float CalculateVariance(List<float> values)
        {
            if (values.Count < 2) return 0f;
            
            float mean = values.Average();
            float sumSquaredDiff = values.Sum(v => Mathf.Pow(v - mean, 2));
            return sumSquaredDiff / (values.Count - 1);
        }
        
        private void GenerateInsights()
        {
            // Generate insights based on current state
            if (currentFrustrationLevel > frustrationThreshold)
            {
                GenerateInsight(InsightType.HighFrustration, "Player frustration level is high", currentFrustrationLevel);
            }
            
            if (currentEngagementLevel < engagementThreshold)
            {
                GenerateInsight(InsightType.LowEngagement, "Player engagement level is low", currentEngagementLevel);
            }
            
            if (currentMasteryLevel > masteryThreshold)
            {
                GenerateInsight(InsightType.HighMastery, "Player has achieved high mastery", currentMasteryLevel);
            }
        }
        
        private void GenerateInsight(InsightType type, string message, float value)
        {
            var insight = new PerformanceInsight
            {
                type = type,
                message = message,
                value = value,
                timestamp = Time.time,
                priority = GetInsightPriority(type)
            };
            
            // Check if similar insight already exists
            var existingInsight = activeInsights.FirstOrDefault(i => i.type == type);
            if (existingInsight != null)
            {
                // Update existing insight
                existingInsight.value = value;
                existingInsight.timestamp = Time.time;
            }
            else
            {
                // Add new insight
                activeInsights.Add(insight);
                OnNewInsight?.Invoke(insight);
            }
            
            // Clean up old insights
            activeInsights.RemoveAll(i => Time.time - i.timestamp > 600f); // 10 min lifetime
        }
        
        private InsightPriority GetInsightPriority(InsightType type)
        {
            switch (type)
            {
                case InsightType.HighFrustration:
                case InsightType.LowEngagement:
                    return InsightPriority.High;
                case InsightType.EngagementDecline:
                case InsightType.FrustrationIncrease:
                    return InsightPriority.Medium;
                default:
                    return InsightPriority.Low;
            }
        }
        
        // Public API
        public void RecordPuzzleAttempt(PuzzleType puzzleType, bool success, float completionTime, float difficulty)
        {
            if (!puzzleTypeAnalytics.ContainsKey(puzzleType)) return;
            
            var analytics = puzzleTypeAnalytics[puzzleType];
            analytics.totalAttempts++;
            
            if (success)
            {
                analytics.successfulCompletions++;
                if (completionTime < analytics.bestCompletionTime)
                {
                    analytics.bestCompletionTime = completionTime;
                }
            }
            
            // Update average completion time
            float totalTime = analytics.averageCompletionTime * (analytics.totalAttempts - 1) + completionTime;
            analytics.averageCompletionTime = totalTime / analytics.totalAttempts;
            
            analytics.difficultyProgression.Add(difficulty);
            
            // Keep only recent difficulty data
            if (analytics.difficultyProgression.Count > 20)
            {
                analytics.difficultyProgression.RemoveAt(0);
            }
        }
        
        public void RecordPlayerSatisfaction(PuzzleType puzzleType, float satisfaction)
        {
            if (!puzzleTypeAnalytics.ContainsKey(puzzleType)) return;
            
            var analytics = puzzleTypeAnalytics[puzzleType];
            analytics.playerSatisfaction.Add(satisfaction);
            
            if (analytics.playerSatisfaction.Count > 10)
            {
                analytics.playerSatisfaction.RemoveAt(0);
            }
        }
        
        public float GetEngagementLevel() => currentEngagementLevel;
        public float GetFrustrationLevel() => currentFrustrationLevel;
        public float GetMasteryLevel() => currentMasteryLevel;
        public List<PerformanceInsight> GetActiveInsights() => new List<PerformanceInsight>(activeInsights);
        public Dictionary<PuzzleType, PuzzleTypeAnalytics> GetPuzzleAnalytics() => new Dictionary<PuzzleType, PuzzleTypeAnalytics>(puzzleTypeAnalytics);
        
        public PerformancePrediction PredictPerformance(PuzzleType puzzleType, float difficulty)
        {
            if (!enablePerformancePrediction || !puzzleTypeAnalytics.ContainsKey(puzzleType))
            {
                return new PerformancePrediction { confidence = 0f };
            }
            
            var analytics = puzzleTypeAnalytics[puzzleType];
            if (analytics.totalAttempts < 5) return new PerformancePrediction { confidence = 0f };
            
            // Simple prediction based on historical data
            float predictedCompletionRate = (float)analytics.successfulCompletions / analytics.totalAttempts;
            float predictedTime = analytics.averageCompletionTime * (1f + (difficulty - 5f) * 0.2f);
            
            return new PerformancePrediction
            {
                expectedCompletionRate = predictedCompletionRate,
                expectedCompletionTime = predictedTime,
                confidence = Mathf.Clamp01(analytics.totalAttempts / 20f)
            };
        }
    }
    
    [System.Serializable]
    public class PerformanceDataPoint
    {
        public float timestamp;
        public float engagementLevel;
        public float frustrationLevel;
        public float masteryLevel;
        public int activePuzzleCount;
        public PlayerMovementMetrics playerMovement;
        public float interactionFrequency;
        public float puzzleProgress;
    }
    
    [System.Serializable]
    public class PlayerMovementMetrics
    {
        public float intensity;
        public Vector3 direction;
        public float speed;
    }
    
    [System.Serializable]
    public class PuzzleTypeAnalytics
    {
        public PuzzleType puzzleType;
        public int totalAttempts;
        public int successfulCompletions;
        public float averageCompletionTime;
        public float bestCompletionTime;
        public List<float> difficultyProgression;
        public List<float> playerSatisfaction;
    }
    
    [System.Serializable]
    public class PerformanceInsight
    {
        public InsightType type;
        public string message;
        public float value;
        public float timestamp;
        public InsightPriority priority;
    }
    
    [System.Serializable]
    public class PlayerBehaviorPattern
    {
        public BehaviorPatternType type;
        public float confidence;
        public string description;
        public float timestamp;
    }
    
    [System.Serializable]
    public class PerformancePrediction
    {
        public float expectedCompletionRate;
        public float expectedCompletionTime;
        public float confidence;
    }
    
    public enum InsightType
    {
        HighFrustration,
        LowEngagement,
        EngagementDecline,
        FrustrationIncrease,
        OptimalExperience,
        HighMastery
    }
    
    public enum InsightPriority
    {
        Low,
        Medium,
        High
    }
    
    public enum BehaviorPatternType
    {
        ConsistentHighActivity,
        LowInteraction,
        StuckOnPuzzle,
        RapidProgress,
        HesitantMovement
    }
}
