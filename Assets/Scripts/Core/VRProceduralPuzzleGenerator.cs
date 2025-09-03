using UnityEngine;
using Run4theRelic.Core;
using Run4theRelic.Puzzles;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Run4theRelic.Core
{
    /// <summary>
    /// Avancerad procedural puzzle generator som skapar olika typer av VR-pussel automatiskt.
    /// Genererar pussel med varierande svårighetsgrader och mekaniker.
    /// </summary>
    public class VRProceduralPuzzleGenerator : MonoBehaviour
    {
        [Header("Generation Settings")]
        [SerializeField] private bool enableProceduralGeneration = true;
        [SerializeField] private bool enableComplexityScaling = true;
        [SerializeField] private bool enableThemeVariation = true;
        [SerializeField] private int maxPuzzleTypes = 10;
        
        [Header("Puzzle Type Weights")]
        [SerializeField] private float relicPlacementWeight = 0.3f;
        [SerializeField] private float handGestureWeight = 0.25f;
        [SerializeField] private float patternMatchingWeight = 0.2f;
        [SerializeField] private float sequenceWeight = 0.15f;
        [SerializeField] private float logicWeight = 0.1f;
        
        [Header("Complexity Settings")]
        [SerializeField] private int minComplexity = 1;
        [SerializeField] private int maxComplexity = 10;
        [SerializeField] private bool enableProgressiveDifficulty = true;
        [SerializeField] private float difficultyCurve = 1.5f;
        
        [Header("Theme Integration")]
        [SerializeField] private bool enableThemeBasedPuzzles = true;
        [SerializeField] private bool enableAtmosphericPuzzles = true;
        [SerializeField] private bool enableStoryBasedPuzzles = true;
        
        [Header("Performance Settings")]
        [SerializeField] private bool enablePuzzlePooling = true;
        [SerializeField] private int maxPooledPuzzles = 50;
        [SerializeField] private bool enableLODForPuzzles = true;
        
        // Private fields
        private List<GeneratedPuzzle> generatedPuzzles = new List<GeneratedPuzzle>();
        private Dictionary<PuzzleType, int> puzzleTypeCounts = new Dictionary<PuzzleType, int>();
        private Dictionary<int, List<GeneratedPuzzle>> complexityGroups = new Dictionary<int, List<GeneratedPuzzle>>();
        private Queue<GeneratedPuzzle> puzzlePool = new Queue<GeneratedPuzzle>();
        
        // Events
        public static event System.Action<GeneratedPuzzle> OnPuzzleGenerated;
        public static event System.Action<PuzzleType, int> OnPuzzleTypeGenerated;
        public static event System.Action<int> OnComplexityGroupGenerated;
        
        protected override void Start()
        {
            InitializePuzzleGenerator();
        }
        
        /// <summary>
        /// Initialize the puzzle generator
        /// </summary>
        private void InitializePuzzleGenerator()
        {
            Debug.Log("[VRProceduralPuzzleGenerator] Initializing advanced puzzle generation system...");
            
            // Initialize puzzle type counts
            foreach (PuzzleType type in System.Enum.GetValues(typeof(PuzzleType)))
            {
                puzzleTypeCounts[type] = 0;
            }
            
            // Initialize complexity groups
            for (int i = minComplexity; i <= maxComplexity; i++)
            {
                complexityGroups[i] = new List<GeneratedPuzzle>();
            }
            
            Debug.Log("[VRProceduralPuzzleGenerator] Puzzle generator initialized successfully!");
        }
        
        /// <summary>
        /// Generate a puzzle based on specified parameters
        /// </summary>
        public GeneratedPuzzle GeneratePuzzle(int complexity, RoomTheme roomTheme, Vector3 position, string roomId)
        {
            if (!enableProceduralGeneration)
            {
                Debug.LogWarning("[VRProceduralPuzzleGenerator] Procedural generation is disabled!");
                return null;
            }
            
            // Select puzzle type based on weights and complexity
            PuzzleType selectedType = SelectPuzzleType(complexity, roomTheme);
            
            // Generate puzzle with selected type
            var puzzle = GeneratePuzzleByType(selectedType, complexity, roomTheme, position, roomId);
            
            // Add to tracking systems
            AddPuzzleToTracking(puzzle);
            
            // Trigger events
            OnPuzzleGenerated?.Invoke(puzzle);
            OnPuzzleTypeGenerated?.Invoke(selectedType, puzzleTypeCounts[selectedType]);
            
            Debug.Log($"[VRProceduralPuzzleGenerator] Generated {selectedType} puzzle with complexity {complexity}");
            
            return puzzle;
        }
        
        /// <summary>
        /// Generate multiple puzzles for a room
        /// </summary>
        public List<GeneratedPuzzle> GeneratePuzzlesForRoom(int roomComplexity, RoomTheme roomTheme, Vector3[] positions, string roomId)
        {
            var puzzles = new List<GeneratedPuzzle>();
            int puzzleCount = CalculateOptimalPuzzleCount(roomComplexity);
            
            for (int i = 0; i < puzzleCount && i < positions.Length; i++)
            {
                // Calculate progressive difficulty
                int puzzleComplexity = CalculateProgressiveDifficulty(roomComplexity, i, puzzleCount);
                
                // Generate puzzle
                var puzzle = GeneratePuzzle(puzzleComplexity, roomTheme, positions[i], roomId);
                if (puzzle != null)
                {
                    puzzles.Add(puzzle);
                }
            }
            
            return puzzles;
        }
        
        /// <summary>
        /// Select puzzle type based on complexity and theme
        /// </summary>
        private PuzzleType SelectPuzzleType(int complexity, RoomTheme roomTheme)
        {
            // Create weighted selection based on complexity
            var weightedTypes = new List<(PuzzleType type, float weight)>();
            
            // Base weights
            weightedTypes.Add((PuzzleType.RelicPlacement, relicPlacementWeight));
            weightedTypes.Add((PuzzleType.HandGesture, handGestureWeight));
            weightedTypes.Add((PuzzleType.PatternMatching, patternMatchingWeight));
            weightedTypes.Add((PuzzleType.Sequence, sequenceWeight));
            weightedTypes.Add((PuzzleType.Logic, logicWeight));
            
            // Adjust weights based on complexity
            if (enableComplexityScaling)
            {
                for (int i = 0; i < weightedTypes.Count; i++)
                {
                    var (type, baseWeight) = weightedTypes[i];
                    
                    // Adjust weight based on complexity
                    float complexityMultiplier = CalculateComplexityMultiplier(type, complexity);
                    weightedTypes[i] = (type, baseWeight * complexityMultiplier);
                }
            }
            
            // Adjust weights based on theme
            if (enableThemeBasedPuzzles)
            {
                for (int i = 0; i < weightedTypes.Count; i++)
                {
                    var (type, baseWeight) = weightedTypes[i];
                    
                    // Adjust weight based on theme compatibility
                    float themeMultiplier = CalculateThemeMultiplier(type, roomTheme);
                    weightedTypes[i] = (type, baseWeight * themeMultiplier);
                }
            }
            
            // Normalize weights
            float totalWeight = weightedTypes.Sum(wt => wt.weight);
            for (int i = 0; i < weightedTypes.Count; i++)
            {
                weightedTypes[i] = (weightedTypes[i].type, weightedTypes[i].weight / totalWeight);
            }
            
            // Select puzzle type using weighted random
            float randomValue = Random.Range(0f, 1f);
            float cumulativeWeight = 0f;
            
            foreach (var (type, weight) in weightedTypes)
            {
                cumulativeWeight += weight;
                if (randomValue <= cumulativeWeight)
                {
                    return type;
                }
            }
            
            // Fallback to first type
            return weightedTypes[0].type;
        }
        
        /// <summary>
        /// Calculate complexity multiplier for puzzle type
        /// </summary>
        private float CalculateComplexityMultiplier(PuzzleType puzzleType, int complexity)
        {
            switch (puzzleType)
            {
                case PuzzleType.RelicPlacement:
                    // Relic placement scales well with complexity
                    return 1f + (complexity - 1) * 0.1f;
                    
                case PuzzleType.HandGesture:
                    // Hand gestures scale moderately
                    return 1f + (complexity - 1) * 0.05f;
                    
                case PuzzleType.PatternMatching:
                    // Pattern matching scales well
                    return 1f + (complexity - 1) * 0.15f;
                    
                case PuzzleType.Sequence:
                    // Sequences scale very well
                    return 1f + (complexity - 1) * 0.2f;
                    
                case PuzzleType.Logic:
                    // Logic puzzles scale extremely well
                    return 1f + (complexity - 1) * 0.25f;
                    
                default:
                    return 1f;
            }
        }
        
        /// <summary>
        /// Calculate theme multiplier for puzzle type
        /// </summary>
        private float CalculateThemeMultiplier(PuzzleType puzzleType, RoomTheme roomTheme)
        {
            // Some puzzle types work better with certain themes
            switch (roomTheme)
            {
                case RoomTheme.Ancient:
                    if (puzzleType == PuzzleType.RelicPlacement) return 1.5f;
                    if (puzzleType == PuzzleType.PatternMatching) return 1.3f;
                    break;
                    
                case RoomTheme.Mystical:
                    if (puzzleType == PuzzleType.HandGesture) return 1.4f;
                    if (puzzleType == PuzzleType.Logic) return 1.2f;
                    break;
                    
                case RoomTheme.Technological:
                    if (puzzleType == PuzzleType.Sequence) return 1.5f;
                    if (puzzleType == PuzzleType.PatternMatching) return 1.3f;
                    break;
                    
                case RoomTheme.Natural:
                    if (puzzleType == PuzzleType.HandGesture) return 1.2f;
                    if (puzzleType == PuzzleType.RelicPlacement) return 1.1f;
                    break;
                    
                case RoomTheme.Sacred:
                    if (puzzleType == PuzzleType.RelicPlacement) return 1.6f;
                    if (puzzleType == PuzzleType.HandGesture) return 1.4f;
                    break;
            }
            
            return 1f; // Default multiplier
        }
        
        /// <summary>
        /// Generate puzzle by specific type
        /// </summary>
        private GeneratedPuzzle GeneratePuzzleByType(PuzzleType puzzleType, int complexity, RoomTheme roomTheme, Vector3 position, string roomId)
        {
            var puzzle = new GeneratedPuzzle();
            puzzle.puzzleId = System.Guid.NewGuid().ToString();
            puzzle.roomId = roomId;
            puzzle.puzzleType = puzzleType;
            puzzle.difficulty = complexity;
            puzzle.position = position;
            puzzle.generationTime = System.DateTime.Now;
            
            // Generate type-specific parameters
            puzzle.parameters = GenerateTypeSpecificParameters(puzzleType, complexity, roomTheme);
            
            // Generate requirements
            puzzle.requirements = GeneratePuzzleRequirements(puzzleType, complexity);
            
            return puzzle;
        }
        
        /// <summary>
        /// Generate type-specific parameters for puzzle
        /// </summary>
        private Dictionary<string, object> GenerateTypeSpecificParameters(PuzzleType puzzleType, int complexity, RoomTheme roomTheme)
        {
            var parameters = new Dictionary<string, object>();
            
            switch (puzzleType)
            {
                case PuzzleType.RelicPlacement:
                    parameters["requiredRelics"] = Mathf.Clamp(complexity, 1, 8);
                    parameters["slotCount"] = Mathf.Clamp(complexity + 1, 2, 10);
                    parameters["snapDistance"] = Mathf.Max(0.05f, 0.2f - complexity * 0.01f);
                    parameters["timeLimit"] = Mathf.Clamp(120f - complexity * 8f, 30f, 180f);
                    parameters["requireSpecificOrder"] = complexity > 5;
                    parameters["enableHapticFeedback"] = true;
                    break;
                    
                case PuzzleType.HandGesture:
                    parameters["requiredGestures"] = Mathf.Clamp(complexity / 2, 1, 6);
                    parameters["gestureHoldTime"] = Mathf.Clamp(0.5f + complexity * 0.2f, 0.5f, 3f);
                    parameters["gestureTolerance"] = Mathf.Max(0.05f, 0.2f - complexity * 0.015f);
                    parameters["requireSequential"] = complexity > 4;
                    parameters["requireBothHands"] = complexity > 6;
                    parameters["enableGestureVisualization"] = true;
                    break;
                    
                case PuzzleType.PatternMatching:
                    parameters["patternSize"] = Mathf.Clamp(complexity, 2, 8);
                    parameters["patternComplexity"] = Mathf.Clamp(complexity / 2, 1, 5);
                    parameters["timeLimit"] = Mathf.Clamp(90f - complexity * 5f, 30f, 120f);
                    parameters["allowHints"] = complexity <= 3;
                    parameters["enablePatternHighlighting"] = true;
                    break;
                    
                case PuzzleType.Sequence:
                    parameters["sequenceLength"] = Mathf.Clamp(complexity + 2, 3, 12);
                    parameters["sequenceComplexity"] = Mathf.Clamp(complexity / 2, 1, 6);
                    parameters["timeLimit"] = Mathf.Clamp(60f - complexity * 3f, 20f, 90f);
                    parameters["allowBacktracking"] = complexity <= 4;
                    parameters["enableSequenceVisualization"] = true;
                    break;
                    
                case PuzzleType.Logic:
                    parameters["logicSteps"] = Mathf.Clamp(complexity, 2, 8);
                    parameters["logicComplexity"] = Mathf.Clamp(complexity / 2, 1, 5);
                    parameters["timeLimit"] = Mathf.Clamp(180f - complexity * 10f, 60f, 300f);
                    parameters["allowHints"] = complexity <= 4;
                    parameters["enableLogicVisualization"] = true;
                    break;
                    
                case PuzzleType.Physics:
                    parameters["physicsObjects"] = Mathf.Clamp(complexity, 2, 10);
                    parameters["physicsComplexity"] = Mathf.Clamp(complexity / 2, 1, 6);
                    parameters["timeLimit"] = Mathf.Clamp(150f - complexity * 8f, 45f, 240f);
                    parameters["enablePhysicsDebug"] = complexity <= 3;
                    parameters["requirePrecision"] = complexity > 5;
                    break;
                    
                case PuzzleType.Combination:
                    parameters["combinationLength"] = Mathf.Clamp(complexity, 3, 10);
                    parameters["combinationComplexity"] = Mathf.Clamp(complexity / 2, 1, 6);
                    parameters["timeLimit"] = Mathf.Clamp(120f - complexity * 6f, 40f, 180f);
                    parameters["allowHints"] = complexity <= 3;
                    parameters["enableCombinationVisualization"] = true;
                    break;
            }
            
            // Add theme-specific parameters
            if (enableThemeBasedPuzzles)
            {
                AddThemeSpecificParameters(parameters, roomTheme, complexity);
            }
            
            return parameters;
        }
        
        /// <summary>
        /// Add theme-specific parameters to puzzle
        /// </summary>
        private void AddThemeSpecificParameters(Dictionary<string, object> parameters, RoomTheme roomTheme, int complexity)
        {
            switch (roomTheme)
            {
                case RoomTheme.Ancient:
                    parameters["ancientSymbols"] = true;
                    parameters["symbolComplexity"] = Mathf.Clamp(complexity / 2, 1, 4);
                    parameters["requireArchaeologicalKnowledge"] = complexity > 6;
                    break;
                    
                case RoomTheme.Mystical:
                    parameters["mysticalElements"] = true;
                    parameters["magicLevel"] = Mathf.Clamp(complexity / 2, 1, 5);
                    parameters["requireSpiritualAlignment"] = complexity > 5;
                    break;
                    
                case RoomTheme.Technological:
                    parameters["techLevel"] = Mathf.Clamp(complexity / 2, 1, 5);
                    parameters["requireTechnicalKnowledge"] = complexity > 4;
                    parameters["enableTechHUD"] = true;
                    break;
                    
                case RoomTheme.Natural:
                    parameters["naturalElements"] = true;
                    parameters["environmentalFactors"] = Mathf.Clamp(complexity / 2, 1, 4);
                    parameters["requireEnvironmentalAwareness"] = complexity > 5;
                    break;
                    
                case RoomTheme.Sacred:
                    parameters["sacredElements"] = true;
                    parameters["divineComplexity"] = Mathf.Clamp(complexity / 2, 1, 5);
                    parameters["requireSpiritualPurity"] = complexity > 6;
                    break;
            }
        }
        
        /// <summary>
        /// Generate puzzle requirements
        /// </summary>
        private PuzzleRequirements GeneratePuzzleRequirements(PuzzleType puzzleType, int complexity)
        {
            var requirements = new PuzzleRequirements();
            
            // Base requirements
            requirements.minTime = Mathf.Clamp(20f - complexity * 1.5f, 10f, 60f);
            requirements.maxTime = Mathf.Clamp(300f - complexity * 15f, 120f, 600f);
            requirements.requiredActions = Mathf.Clamp(complexity, 1, 15);
            requirements.successThreshold = Mathf.Clamp(0.9f - complexity * 0.03f, 0.6f, 0.95f);
            
            // Type-specific adjustments
            switch (puzzleType)
            {
                case PuzzleType.RelicPlacement:
                    requirements.successThreshold = Mathf.Clamp(0.95f - complexity * 0.02f, 0.8f, 0.98f);
                    break;
                    
                case PuzzleType.HandGesture:
                    requirements.successThreshold = Mathf.Clamp(0.85f - complexity * 0.025f, 0.7f, 0.9f);
                    break;
                    
                case PuzzleType.Logic:
                    requirements.maxTime = Mathf.Clamp(600f - complexity * 30f, 300f, 900f);
                    break;
                    
                case PuzzleType.Physics:
                    requirements.successThreshold = Mathf.Clamp(0.8f - complexity * 0.02f, 0.65f, 0.85f);
                    break;
            }
            
            return requirements;
        }
        
        /// <summary>
        /// Calculate optimal puzzle count for room
        /// </summary>
        private int CalculateOptimalPuzzleCount(int roomComplexity)
        {
            // Base puzzle count scales with room complexity
            int baseCount = Mathf.Clamp(roomComplexity / 2, 1, 5);
            
            // Add variation
            int variation = Random.Range(-1, 2);
            
            return Mathf.Clamp(baseCount + variation, 1, 8);
        }
        
        /// <summary>
        /// Calculate progressive difficulty within room
        /// </summary>
        private int CalculateProgressiveDifficulty(int roomComplexity, int puzzleIndex, int totalPuzzles)
        {
            if (!enableProgressiveDifficulty)
                return roomComplexity;
            
            // Progressive difficulty curve
            float progression = (float)puzzleIndex / Mathf.Max(1, totalPuzzles - 1);
            float difficultyMultiplier = 1f + (progression * difficultyCurve);
            
            int difficulty = Mathf.RoundToInt(roomComplexity * difficultyMultiplier);
            return Mathf.Clamp(difficulty, minComplexity, maxComplexity);
        }
        
        /// <summary>
        /// Add puzzle to tracking systems
        /// </summary>
        private void AddPuzzleToTracking(GeneratedPuzzle puzzle)
        {
            // Add to main list
            generatedPuzzles.Add(puzzle);
            
            // Update type counts
            puzzleTypeCounts[puzzle.puzzleType]++;
            
            // Add to complexity group
            if (complexityGroups.ContainsKey(puzzle.difficulty))
            {
                complexityGroups[puzzle.difficulty].Add(puzzle);
            }
            
            // Add to pool if enabled
            if (enablePuzzlePooling && puzzlePool.Count < maxPooledPuzzles)
            {
                puzzlePool.Enqueue(puzzle);
            }
            
            // Trigger complexity group event
            OnComplexityGroupGenerated?.Invoke(puzzle.difficulty);
        }
        
        /// <summary>
        /// Get puzzle statistics
        /// </summary>
        public PuzzleGenerationStats GetPuzzleStats()
        {
            var stats = new PuzzleGenerationStats();
            
            stats.totalPuzzlesGenerated = generatedPuzzles.Count;
            stats.puzzleTypeDistribution = new Dictionary<PuzzleType, int>(puzzleTypeCounts);
            stats.complexityDistribution = new Dictionary<int, int>();
            
            foreach (var kvp in complexityGroups)
            {
                stats.complexityDistribution[kvp.Key] = kvp.Value.Count;
            }
            
            stats.averageComplexity = generatedPuzzles.Count > 0 ? 
                generatedPuzzles.Average(p => p.difficulty) : 0f;
            
            stats.averageQuality = generatedPuzzles.Count > 0 ? 
                generatedPuzzles.Average(p => p.requirements.successThreshold) : 0f;
            
            return stats;
        }
        
        /// <summary>
        /// Get puzzles by type
        /// </summary>
        public List<GeneratedPuzzle> GetPuzzlesByType(PuzzleType puzzleType)
        {
            return generatedPuzzles.Where(p => p.puzzleType == puzzleType).ToList();
        }
        
        /// <summary>
        /// Get puzzles by complexity
        /// </summary>
        public List<GeneratedPuzzle> GetPuzzlesByComplexity(int complexity)
        {
            return generatedPuzzles.Where(p => p.difficulty == complexity).ToList();
        }
        
        /// <summary>
        /// Get puzzles by theme
        /// </summary>
        public List<GeneratedPuzzle> GetPuzzlesByTheme(RoomTheme theme)
        {
            // This would require adding theme to GeneratedPuzzle
            // For now, return all puzzles
            return new List<GeneratedPuzzle>(generatedPuzzles);
        }
        
        /// <summary>
        /// Clear all generated puzzles
        /// </summary>
        public void ClearAllPuzzles()
        {
            generatedPuzzles.Clear();
            puzzleTypeCounts.Clear();
            complexityGroups.Clear();
            puzzlePool.Clear();
            
            // Reinitialize
            InitializePuzzleGenerator();
            
            Debug.Log("[VRProceduralPuzzleGenerator] All puzzles cleared");
        }
        
        /// <summary>
        /// Get puzzle from pool
        /// </summary>
        public GeneratedPuzzle GetPuzzleFromPool()
        {
            if (puzzlePool.Count > 0)
            {
                return puzzlePool.Dequeue();
            }
            return null;
        }
        
        /// <summary>
        /// Return puzzle to pool
        /// </summary>
        public void ReturnPuzzleToPool(GeneratedPuzzle puzzle)
        {
            if (enablePuzzlePooling && puzzlePool.Count < maxPooledPuzzles)
            {
                puzzlePool.Enqueue(puzzle);
            }
        }
    }
    
    /// <summary>
    /// Statistics for puzzle generation
    /// </summary>
    [System.Serializable]
    public class PuzzleGenerationStats
    {
        public int totalPuzzlesGenerated;
        public Dictionary<PuzzleType, int> puzzleTypeDistribution;
        public Dictionary<int, int> complexityDistribution;
        public float averageComplexity;
        public float averageQuality;
    }
}
