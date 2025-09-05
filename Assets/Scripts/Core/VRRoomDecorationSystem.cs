using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Run4theRelic.Core.Visual
{
    /// <summary>
    /// Procedural rum-dekorering system för VR med AI-driven innehållsgenerering
    /// Skapar unika rum med atmosfäriska detaljer och interaktiva element
    /// </summary>
    public class VRRoomDecorationSystem : MonoBehaviour
    {
        [Header("Decoration Settings")]
        [SerializeField] private bool enableProceduralDecoration = true;
        [SerializeField] private bool enableInteractiveDecorations = true;
        [SerializeField] private float decorationDensity = 0.7f;
        [SerializeField] private int maxDecorationsPerRoom = 50;
        
        [Header("Room Themes")]
        [SerializeField] private RoomDecorationTheme currentTheme = RoomDecorationTheme.Mystical;
        [SerializeField] private bool enableThemeTransitions = true;
        [SerializeField] private float themeTransitionDuration = 3f;
        
        [Header("Decoration Prefabs")]
        [SerializeField] private List<GameObject> mysticalDecorations = new List<GameObject>();
        [SerializeField] private List<GameObject> technologicalDecorations = new List<GameObject>();
        [SerializeField] private List<GameObject> naturalDecorations = new List<GameObject>();
        [SerializeField] private List<GameObject> ancientDecorations = new List<GameObject>();
        [SerializeField] private List<GameObject> futuristicDecorations = new List<GameObject>();
        
        [Header("Interactive Elements")]
        [SerializeField] private List<GameObject> interactiveDecorations = new List<GameObject>();
        [SerializeField] private bool enableHapticFeedback = true;
        [SerializeField] private float interactionRange = 2f;
        
        [Header("Atmospheric Details")]
        [SerializeField] private bool enableAtmosphericDetails = true;
        [SerializeField] private List<GameObject> atmosphericElements = new List<GameObject>();
        [SerializeField] private float atmosphericDensity = 0.5f;
        
        [Header("Performance Settings")]
        [SerializeField] private bool enableLODSystem = true;
        [SerializeField] private float lodDistance = 15f;
        [SerializeField] private bool enableOcclusionCulling = true;
        
        // Private fields
        private List<GameObject> activeDecorations = new List<GameObject>();
        private Dictionary<GameObject, DecorationSettings> decorationSettings = new Dictionary<GameObject, DecorationSettings>();
        private Dictionary<GameObject, Coroutine> activeCoroutines = new Dictionary<GameObject, Coroutine>();
        private Camera vrCamera;
        private RoomDecorationTheme targetTheme;
        private bool isTransitioning = false;
        
        // Events
        public static event System.Action<RoomDecorationTheme> OnThemeChanged;
        public static event System.Action<GameObject> OnDecorationSpawned;
        public static event System.Action<GameObject> OnDecorationRemoved;
        public static event System.Action<GameObject> OnDecorationInteracted;
        
        private void Start()
        {
            InitializeDecorationSystem();
        }
        
        private void Update()
        {
            if (enableProceduralDecoration)
            {
                UpdateLODSystem();
                UpdateInteractiveElements();
            }
        }
        
        /// <summary>
        /// Initialize decoration system
        /// </summary>
        private void InitializeDecorationSystem()
        {
            // Find VR camera
            vrCamera = Camera.main;
            if (vrCamera == null)
            {
                vrCamera = FindObjectOfType<Camera>();
            }
            
            // Set initial theme
            targetTheme = currentTheme;
            
            // Generate initial decorations
            if (enableProceduralDecoration)
            {
                GenerateRoomDecorations();
            }
            
            Debug.Log("[VRRoomDecorationSystem] Room decoration system initialized");
        }
        
        /// <summary>
        /// Generate room decorations based on current theme
        /// </summary>
        public void GenerateRoomDecorations()
        {
            // Clear existing decorations
            ClearAllDecorations();
            
            // Get decoration list for current theme
            List<GameObject> themeDecorations = GetDecorationsForTheme(currentTheme);
            
            if (themeDecorations.Count == 0)
            {
                Debug.LogWarning($"[VRRoomDecorationSystem] No decorations found for theme: {currentTheme}");
                return;
            }
            
            // Calculate number of decorations to spawn
            int decorationCount = Mathf.RoundToInt(maxDecorationsPerRoom * decorationDensity);
            
            // Spawn decorations
            for (int i = 0; i < decorationCount; i++)
            {
                SpawnRandomDecoration(themeDecorations);
            }
            
            // Add atmospheric details
            if (enableAtmosphericDetails)
            {
                AddAtmosphericDetails();
            }
            
            Debug.Log($"[VRRoomDecorationSystem] Generated {activeDecorations.Count} decorations for theme: {currentTheme}");
        }
        
        /// <summary>
        /// Get decorations for specific theme
        /// </summary>
        private List<GameObject> GetDecorationsForTheme(RoomDecorationTheme theme)
        {
            switch (theme)
            {
                case RoomDecorationTheme.Mystical:
                    return mysticalDecorations;
                case RoomDecorationTheme.Technological:
                    return technologicalDecorations;
                case RoomDecorationTheme.Natural:
                    return naturalDecorations;
                case RoomDecorationTheme.Ancient:
                    return ancientDecorations;
                case RoomDecorationTheme.Futuristic:
                    return futuristicDecorations;
                default:
                    return mysticalDecorations;
            }
        }
        
        /// <summary>
        /// Spawn random decoration
        /// </summary>
        private void SpawnRandomDecoration(List<GameObject> decorations)
        {
            if (decorations.Count == 0) return;
            
            // Select random decoration
            GameObject decorationPrefab = decorations[Random.Range(0, decorations.Count)];
            
            // Calculate random position
            Vector3 position = CalculateRandomPosition();
            
            // Spawn decoration
            GameObject decoration = Instantiate(decorationPrefab, position, Quaternion.identity);
            
            // Set random rotation
            decoration.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            
            // Set random scale
            float scale = Random.Range(0.8f, 1.2f);
            decoration.transform.localScale = Vector3.one * scale;
            
            // Add to active decorations
            activeDecorations.Add(decoration);
            
            // Store decoration settings
            decorationSettings[decoration] = new DecorationSettings
            {
                originalPosition = position,
                originalRotation = decoration.transform.rotation,
                originalScale = decoration.transform.localScale,
                isInteractive = enableInteractiveDecorations && Random.value < 0.3f
            };
            
            // Setup interaction if interactive
            if (decorationSettings[decoration].isInteractive)
            {
                SetupInteractiveDecoration(decoration);
            }
            
            OnDecorationSpawned?.Invoke(decoration);
        }
        
        /// <summary>
        /// Calculate random position for decoration
        /// </summary>
        private Vector3 CalculateRandomPosition()
        {
            // This would need to be adapted based on room layout
            // For now, use a simple random position within bounds
            float x = Random.Range(-10f, 10f);
            float z = Random.Range(-10f, 10f);
            float y = 0f; // Ground level
            
            return new Vector3(x, y, z);
        }
        
        /// <summary>
        /// Setup interactive decoration
        /// </summary>
        private void SetupInteractiveDecoration(GameObject decoration)
        {
            // Add collider if not present
            if (decoration.GetComponent<Collider>() == null)
            {
                decoration.AddComponent<BoxCollider>();
            }
            
            // Add interaction script
            var interaction = decoration.GetComponent<VRDecorationInteraction>();
            if (interaction == null)
            {
                interaction = decoration.AddComponent<VRDecorationInteraction>();
            }
            
            // Setup interaction events
            interaction.OnInteracted += HandleDecorationInteraction;
        }
        
        /// <summary>
        /// Handle decoration interaction
        /// </summary>
        private void HandleDecorationInteraction(GameObject decoration)
        {
            OnDecorationInteracted?.Invoke(decoration);
            
            // Add haptic feedback
            if (enableHapticFeedback)
            {
                // This would integrate with VR haptic system
                Debug.Log($"[VRRoomDecorationSystem] Haptic feedback triggered for {decoration.name}");
            }
            
            // Start interaction effect
            StartCoroutine(PlayInteractionEffect(decoration));
        }
        
        /// <summary>
        /// Play interaction effect
        /// </summary>
        private IEnumerator PlayInteractionEffect(GameObject decoration)
        {
            // Store original scale
            Vector3 originalScale = decoration.transform.localScale;
            
            // Scale up
            float elapsed = 0f;
            float duration = 0.2f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                decoration.transform.localScale = Vector3.Lerp(originalScale, originalScale * 1.1f, t);
                
                yield return null;
            }
            
            // Scale back down
            elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                decoration.transform.localScale = Vector3.Lerp(originalScale * 1.1f, originalScale, t);
                
                yield return null;
            }
            
            // Restore original scale
            decoration.transform.localScale = originalScale;
        }
        
        /// <summary>
        /// Add atmospheric details
        /// </summary>
        private void AddAtmosphericDetails()
        {
            if (atmosphericElements.Count == 0) return;
            
            int detailCount = Mathf.RoundToInt(maxDecorationsPerRoom * atmosphericDensity);
            
            for (int i = 0; i < detailCount; i++)
            {
                GameObject detailPrefab = atmosphericElements[Random.Range(0, atmosphericElements.Count)];
                Vector3 position = CalculateRandomPosition();
                
                GameObject detail = Instantiate(detailPrefab, position, Quaternion.identity);
                detail.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                
                activeDecorations.Add(detail);
                
                decorationSettings[detail] = new DecorationSettings
                {
                    originalPosition = position,
                    originalRotation = detail.transform.rotation,
                    originalScale = detail.transform.localScale,
                    isInteractive = false
                };
            }
        }
        
        /// <summary>
        /// Change room decoration theme
        /// </summary>
        public void ChangeRoomTheme(RoomDecorationTheme newTheme)
        {
            if (newTheme == currentTheme || isTransitioning) return;
            
            targetTheme = newTheme;
            StartCoroutine(TransitionToNewTheme());
        }
        
        /// <summary>
        /// Transition to new theme
        /// </summary>
        private IEnumerator TransitionToNewTheme()
        {
            isTransitioning = true;
            
            // Fade out current decorations
            yield return StartCoroutine(FadeOutDecorations());
            
            // Change theme
            currentTheme = targetTheme;
            
            // Generate new decorations
            GenerateRoomDecorations();
            
            // Fade in new decorations
            yield return StartCoroutine(FadeInDecorations());
            
            isTransitioning = false;
            
            OnThemeChanged?.Invoke(currentTheme);
            Debug.Log($"[VRRoomDecorationSystem] Theme changed to: {currentTheme}");
        }
        
        /// <summary>
        /// Fade out decorations
        /// </summary>
        private IEnumerator FadeOutDecorations()
        {
            if (!enableThemeTransitions) yield break;
            
            float elapsed = 0f;
            float duration = themeTransitionDuration * 0.5f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                foreach (var decoration in activeDecorations)
                {
                    if (decoration != null)
                    {
                        var renderer = decoration.GetComponent<Renderer>();
                        if (renderer != null)
                        {
                            Color color = renderer.material.color;
                            color.a = Mathf.Lerp(1f, 0f, t);
                            renderer.material.color = color;
                        }
                    }
                }
                
                yield return null;
            }
        }
        
        /// <summary>
        /// Fade in decorations
        /// </summary>
        private IEnumerator FadeInDecorations()
        {
            if (!enableThemeTransitions) yield break;
            
            float elapsed = 0f;
            float duration = themeTransitionDuration * 0.5f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                foreach (var decoration in activeDecorations)
                {
                    if (decoration != null)
                    {
                        var renderer = decoration.GetComponent<Renderer>();
                        if (renderer != null)
                        {
                            Color color = renderer.material.color;
                            color.a = Mathf.Lerp(0f, 1f, t);
                            renderer.material.color = color;
                        }
                    }
                }
                
                yield return null;
            }
        }
        
        /// <summary>
        /// Update LOD system
        /// </summary>
        private void UpdateLODSystem()
        {
            if (!enableLODSystem || vrCamera == null) return;
            
            foreach (var decoration in activeDecorations)
            {
                if (decoration != null)
                {
                    float distance = Vector3.Distance(vrCamera.transform.position, decoration.transform.position);
                    
                    // Enable/disable based on distance
                    decoration.SetActive(distance <= lodDistance);
                }
            }
        }
        
        /// <summary>
        /// Update interactive elements
        /// </summary>
        private void UpdateInteractiveElements()
        {
            if (!enableInteractiveDecorations || vrCamera == null) return;
            
            foreach (var decoration in activeDecorations)
            {
                if (decoration != null && decorationSettings.ContainsKey(decoration))
                {
                    var settings = decorationSettings[decoration];
                    
                    if (settings.isInteractive)
                    {
                        float distance = Vector3.Distance(vrCamera.transform.position, decoration.transform.position);
                        
                        // Enable interaction within range
                        var interaction = decoration.GetComponent<VRDecorationInteraction>();
                        if (interaction != null)
                        {
                            interaction.SetInteractionEnabled(distance <= interactionRange);
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Clear all decorations
        /// </summary>
        public void ClearAllDecorations()
        {
            foreach (var decoration in activeDecorations)
            {
                if (decoration != null)
                {
                    OnDecorationRemoved?.Invoke(decoration);
                    DestroyImmediate(decoration);
                }
            }
            
            activeDecorations.Clear();
            decorationSettings.Clear();
        }
        
        /// <summary>
        /// Get current theme
        /// </summary>
        public RoomDecorationTheme GetCurrentTheme()
        {
            return currentTheme;
        }
        
        /// <summary>
        /// Get active decoration count
        /// </summary>
        public int GetActiveDecorationCount()
        {
            return activeDecorations.Count;
        }
        
        private void OnDestroy()
        {
            // Stop all coroutines
            foreach (var coroutine in activeCoroutines.Values)
            {
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }
            }
            
            // Clear all decorations
            ClearAllDecorations();
        }
    }
    
    /// <summary>
    /// Room decoration themes
    /// </summary>
    public enum RoomDecorationTheme
    {
        Mystical,
        Technological,
        Natural,
        Ancient,
        Futuristic,
        Minimalist,
        Ornate
    }
    
    /// <summary>
    /// Decoration settings storage
    /// </summary>
    [System.Serializable]
    public class DecorationSettings
    {
        public Vector3 originalPosition;
        public Quaternion originalRotation;
        public Vector3 originalScale;
        public bool isInteractive;
    }
    
    /// <summary>
    /// VR decoration interaction component
    /// </summary>
    public class VRDecorationInteraction : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] private bool interactionEnabled = true;
        [SerializeField] private float interactionCooldown = 1f;
        
        private float lastInteractionTime = 0f;
        
        public event System.Action<GameObject> OnInteracted;
        
        private void OnTriggerEnter(Collider other)
        {
            if (interactionEnabled && Time.time - lastInteractionTime > interactionCooldown)
            {
                // Check if it's a VR controller
                if (other.CompareTag("VRController") || other.CompareTag("Player"))
                {
                    HandleInteraction();
                }
            }
        }
        
        private void HandleInteraction()
        {
            lastInteractionTime = Time.time;
            OnInteracted?.Invoke(gameObject);
        }
        
        public void SetInteractionEnabled(bool enabled)
        {
            interactionEnabled = enabled;
        }
    }
}
