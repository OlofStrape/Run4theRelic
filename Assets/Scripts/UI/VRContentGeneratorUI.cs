using UnityEngine;
using UnityEngine.UI;
using Run4theRelic.Core;
using TMPro;

namespace Run4theRelic.UI
{
    /// <summary>
    /// UI-kontrollpanel för VR Content Generator som visar status och kontroller.
    /// Ger användaren full kontroll över innehållsgenereringen.
    /// </summary>
    public class VRContentGeneratorUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject uiPanel;
        [SerializeField] private Button startGenerationButton;
        [SerializeField] private Button stopGenerationButton;
        [SerializeField] private Button forceGenerateButton;
        [SerializeField] private Button clearRoomsButton;
        
        [Header("Status Display")]
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private TextMeshProUGUI statsText;
        [SerializeField] private TextMeshProUGUI qualityText;
        [SerializeField] private Slider progressSlider;
        
        [Header("Settings Panel")]
        [SerializeField] private Toggle continuousGenerationToggle;
        [SerializeField] private Toggle backgroundGenerationToggle;
        [SerializeField] private Slider generationIntervalSlider;
        [SerializeField] private TextMeshProUGUI intervalValueText;
        [SerializeField] private Slider maxRoomsSlider;
        [SerializeField] private TextMeshProUGUI maxRoomsValueText;
        
        [Header("Performance Panel")]
        [SerializeField] private Toggle performanceOptimizationToggle;
        [SerializeField] private Toggle lodGenerationToggle;
        [SerializeField] private Toggle textureAtlasToggle;
        [SerializeField] private Slider targetFPSSlider;
        [SerializeField] private TextMeshProUGUI fpsValueText;
        
        [Header("AI Features Panel")]
        [SerializeField] private Toggle adaptiveDifficultyToggle;
        [SerializeField] private Toggle playerBehaviorToggle;
        [SerializeField] private Toggle dynamicBalancingToggle;
        [SerializeField] private Slider learningRateSlider;
        [SerializeField] private TextMeshProUGUI learningRateValueText;
        
        // Private fields
        private VRContentGenerator _contentGenerator;
        private bool _isInitialized = false;
        private float _updateInterval = 1f;
        private float _lastUpdateTime = 0f;
        
        protected override void Start()
        {
            InitializeUI();
            SetupEventListeners();
        }
        
        /// <summary>
        /// Initialize the UI components
        /// </summary>
        private void InitializeUI()
        {
            // Find VR Content Generator
            _contentGenerator = FindObjectOfType<VRContentGenerator>();
            
            if (_contentGenerator == null)
            {
                Debug.LogWarning("[VRContentGeneratorUI] No VRContentGenerator found in scene!");
                return;
            }
            
            // Initialize UI state
            UpdateUIState();
            _isInitialized = true;
            
            Debug.Log("[VRContentGeneratorUI] UI initialized successfully");
        }
        
        /// <summary>
        /// Setup event listeners for buttons and toggles
        /// </summary>
        private void SetupEventListeners()
        {
            if (!_isInitialized) return;
            
            // Button listeners
            if (startGenerationButton != null)
                startGenerationButton.onClick.AddListener(StartGeneration);
            
            if (stopGenerationButton != null)
                stopGenerationButton.onClick.AddListener(StopGeneration);
            
            if (forceGenerateButton != null)
                forceGenerateButton.onClick.AddListener(ForceGenerateRoom);
            
            if (clearRoomsButton != null)
                clearRoomsButton.onClick.AddListener(ClearAllRooms);
            
            // Toggle listeners
            if (continuousGenerationToggle != null)
                continuousGenerationToggle.onValueChanged.AddListener(OnContinuousGenerationChanged);
            
            if (backgroundGenerationToggle != null)
                backgroundGenerationToggle.onValueChanged.AddListener(OnBackgroundGenerationChanged);
            
            if (performanceOptimizationToggle != null)
                performanceOptimizationToggle.onValueChanged.AddListener(OnPerformanceOptimizationChanged);
            
            if (lodGenerationToggle != null)
                lodGenerationToggle.onValueChanged.AddListener(OnLODGenerationChanged);
            
            if (textureAtlasToggle != null)
                textureAtlasToggle.onValueChanged.AddListener(OnTextureAtlasChanged);
            
            if (adaptiveDifficultyToggle != null)
                adaptiveDifficultyToggle.onValueChanged.AddListener(OnAdaptiveDifficultyChanged);
            
            if (playerBehaviorToggle != null)
                playerBehaviorToggle.onValueChanged.AddListener(OnPlayerBehaviorChanged);
            
            if (dynamicBalancingToggle != null)
                dynamicBalancingToggle.onValueChanged.AddListener(OnDynamicBalancingChanged);
            
            // Slider listeners
            if (generationIntervalSlider != null)
                generationIntervalSlider.onValueChanged.AddListener(OnGenerationIntervalChanged);
            
            if (maxRoomsSlider != null)
                maxRoomsSlider.onValueChanged.AddListener(OnMaxRoomsChanged);
            
            if (targetFPSSlider != null)
                targetFPSSlider.onValueChanged.AddListener(OnTargetFPSChanged);
            
            if (learningRateSlider != null)
                learningRateSlider.onValueChanged.AddListener(OnLearningRateChanged);
            
            // Subscribe to content generator events
            VRContentGenerator.OnRoomGenerated += OnRoomGenerated;
            VRContentGenerator.OnPuzzleGenerated += OnPuzzleGenerated;
            VRContentGenerator.OnGenerationProgress += OnGenerationProgress;
            VRContentGenerator.OnGenerationComplete += OnGenerationComplete;
        }
        
        protected override void Update()
        {
            if (!_isInitialized || _contentGenerator == null) return;
            
            // Update UI at regular intervals
            if (Time.time - _lastUpdateTime >= _updateInterval)
            {
                UpdateUIState();
                _lastUpdateTime = Time.time;
            }
        }
        
        /// <summary>
        /// Update all UI elements with current state
        /// </summary>
        private void UpdateUIState()
        {
            if (_contentGenerator == null) return;
            
            var stats = _contentGenerator.GetGenerationStats();
            
            // Update status text
            if (statusText != null)
            {
                string status = stats.isCurrentlyGenerating ? "Generating..." : "Idle";
                statusText.text = $"Status: {status}";
            }
            
            // Update stats text
            if (statsText != null)
            {
                statsText.text = $"Rooms: {stats.totalRoomsGenerated}/{stats.maxRooms}\n" +
                                $"Interval: {stats.generationInterval:F1}s\n" +
                                $"Avg Quality: {stats.averageQuality:F1}/100\n" +
                                $"Avg Complexity: {stats.averageComplexity:F1}";
            }
            
            // Update quality text
            if (qualityText != null)
            {
                qualityText.text = $"Quality Score: {stats.averageQuality:F1}/100";
            }
            
            // Update progress slider
            if (progressSlider != null)
            {
                progressSlider.value = (float)stats.totalRoomsGenerated / stats.maxRooms;
            }
            
            // Update button states
            UpdateButtonStates(stats);
        }
        
        /// <summary>
        /// Update button states based on current generation status
        /// </summary>
        private void UpdateButtonStates(GenerationStats stats)
        {
            if (startGenerationButton != null)
                startGenerationButton.interactable = !stats.isCurrentlyGenerating;
            
            if (stopGenerationButton != null)
                stopGenerationButton.interactable = stats.isCurrentlyGenerating;
            
            if (forceGenerateButton != null)
                forceGenerateButton.interactable = !stats.isCurrentlyGenerating && stats.totalRoomsGenerated < stats.maxRooms;
            
            if (clearRoomsButton != null)
                clearRoomsButton.interactable = stats.totalRoomsGenerated > 0;
        }
        
        // Button event handlers
        private void StartGeneration()
        {
            if (_contentGenerator != null)
            {
                // Start generation by enabling continuous generation
                // This will be handled by the toggle
                if (continuousGenerationToggle != null)
                    continuousGenerationToggle.isOn = true;
                
                Debug.Log("[VRContentGeneratorUI] Starting content generation...");
            }
        }
        
        private void StopGeneration()
        {
            if (_contentGenerator != null)
            {
                _contentGenerator.StopGeneration();
                
                if (continuousGenerationToggle != null)
                    continuousGenerationToggle.isOn = false;
                
                Debug.Log("[VRContentGeneratorUI] Stopped content generation");
            }
        }
        
        private void ForceGenerateRoom()
        {
            if (_contentGenerator != null)
            {
                _contentGenerator.ForceGenerateRoom();
                Debug.Log("[VRContentGeneratorUI] Forcing room generation...");
            }
        }
        
        private void ClearAllRooms()
        {
            if (_contentGenerator != null)
            {
                // This would require a method in VRContentGenerator
                Debug.Log("[VRContentGeneratorUI] Clear rooms functionality not yet implemented");
            }
        }
        
        // Toggle event handlers
        private void OnContinuousGenerationChanged(bool value)
        {
            // This would update the VRContentGenerator settings
            Debug.Log($"[VRContentGeneratorUI] Continuous generation: {value}");
        }
        
        private void OnBackgroundGenerationChanged(bool value)
        {
            Debug.Log($"[VRContentGeneratorUI] Background generation: {value}");
        }
        
        private void OnPerformanceOptimizationChanged(bool value)
        {
            Debug.Log($"[VRContentGeneratorUI] Performance optimization: {value}");
        }
        
        private void OnLODGenerationChanged(bool value)
        {
            Debug.Log($"[VRContentGeneratorUI] LOD generation: {value}");
        }
        
        private void OnTextureAtlasChanged(bool value)
        {
            Debug.Log($"[VRContentGeneratorUI] Texture atlas: {value}");
        }
        
        private void OnAdaptiveDifficultyChanged(bool value)
        {
            Debug.Log($"[VRContentGeneratorUI] Adaptive difficulty: {value}");
        }
        
        private void OnPlayerBehaviorChanged(bool value)
        {
            Debug.Log($"[VRContentGeneratorUI] Player behavior analysis: {value}");
        }
        
        private void OnDynamicBalancingChanged(bool value)
        {
            Debug.Log($"[VRContentGeneratorUI] Dynamic balancing: {value}");
        }
        
        // Slider event handlers
        private void OnGenerationIntervalChanged(float value)
        {
            if (intervalValueText != null)
                intervalValueText.text = $"{value:F1}s";
            
            Debug.Log($"[VRContentGeneratorUI] Generation interval: {value:F1}s");
        }
        
        private void OnMaxRoomsChanged(float value)
        {
            if (maxRoomsValueText != null)
                maxRoomsValueText.text = $"{Mathf.RoundToInt(value)}";
            
            Debug.Log($"[VRContentGeneratorUI] Max rooms: {Mathf.RoundToInt(value)}");
        }
        
        private void OnTargetFPSChanged(float value)
        {
            if (fpsValueText != null)
                fpsValueText.text = $"{Mathf.RoundToInt(value)}";
            
            Debug.Log($"[VRContentGeneratorUI] Target FPS: {Mathf.RoundToInt(value)}");
        }
        
        private void OnLearningRateChanged(float value)
        {
            if (learningRateValueText != null)
                learningRateValueText.text = $"{value:F2}";
            
            Debug.Log($"[VRContentGeneratorUI] Learning rate: {value:F2}");
        }
        
        // Content generator event handlers
        private void OnRoomGenerated(GeneratedRoom room)
        {
            Debug.Log($"[VRContentGeneratorUI] Room generated: {room.roomName} (Quality: {room.qualityScore:F1}/100)");
            
            // Update UI immediately
            UpdateUIState();
        }
        
        private void OnPuzzleGenerated(GeneratedPuzzle puzzle)
        {
            Debug.Log($"[VRContentGeneratorUI] Puzzle generated: {puzzle.puzzleType} (Difficulty: {puzzle.difficulty})");
        }
        
        private void OnGenerationProgress(int progress)
        {
            Debug.Log($"[VRContentGeneratorUI] Generation progress: {progress}%");
        }
        
        private void OnGenerationComplete(string message)
        {
            Debug.Log($"[VRContentGeneratorUI] {message}");
            
            // Update UI immediately
            UpdateUIState();
        }
        
        /// <summary>
        /// Toggle UI panel visibility
        /// </summary>
        public void ToggleUIPanel()
        {
            if (uiPanel != null)
            {
                uiPanel.SetActive(!uiPanel.activeSelf);
            }
        }
        
        /// <summary>
        /// Show UI panel
        /// </summary>
        public void ShowUIPanel()
        {
            if (uiPanel != null)
            {
                uiPanel.SetActive(true);
            }
        }
        
        /// <summary>
        /// Hide UI panel
        /// </summary>
        public void HideUIPanel()
        {
            if (uiPanel != null)
            {
                uiPanel.SetActive(false);
            }
        }
        
        protected override void OnDestroy()
        {
            // Unsubscribe from events
            VRContentGenerator.OnRoomGenerated -= OnRoomGenerated;
            VRContentGenerator.OnPuzzleGenerated -= OnPuzzleGenerated;
            VRContentGenerator.OnGenerationProgress -= OnGenerationProgress;
            VRContentGenerator.OnGenerationComplete -= OnGenerationComplete;
        }
    }
}
