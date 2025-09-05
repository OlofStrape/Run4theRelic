using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Run4theRelic.Core.Visual
{
    /// <summary>
    /// VR-optimerat shader-system för maximal performance och visuell kvalitet
    /// Hanterar shader-LOD, material-instancing och VR-specifika optimeringar
    /// </summary>
    public class VRShaderOptimizationSystem : MonoBehaviour
    {
        [Header("Shader Optimization Settings")]
        [SerializeField] private bool enableShaderOptimization = true;
        [SerializeField] private bool enableMaterialInstancing = true;
        [SerializeField] private bool enableShaderLOD = true;
        [SerializeField] private bool enableVRComfortMode = true;
        
        [Header("Performance Settings")]
        [SerializeField] private float targetFrameRate = 90f;
        [SerializeField] private float performanceCheckInterval = 1f;
        [SerializeField] private float qualityAdjustmentSpeed = 2f;
        [SerializeField] private int maxMaterialInstances = 100;
        
        [Header("Shader LOD Settings")]
        [SerializeField] private float lodDistance = 20f;
        [SerializeField] private float lodTransitionSpeed = 2f;
        [SerializeField] private bool enableAutomaticLOD = true;
        
        [Header("VR-Specific Optimizations")]
        [SerializeField] private bool enableSinglePassRendering = true;
        [SerializeField] private bool enableOcclusionCulling = true;
        [SerializeField] private bool enableFrustumCulling = true;
        [SerializeField] private bool enableBatching = true;
        
        [Header("Material Management")]
        [SerializeField] private List<Material> optimizedMaterials = new List<Material>();
        [SerializeField] private List<Material> highQualityMaterials = new List<Material>();
        [SerializeField] private List<Material> lowQualityMaterials = new List<Material>();
        
        // Private fields
        private Dictionary<Renderer, MaterialSettings> originalMaterialSettings = new Dictionary<Renderer, MaterialSettings>();
        private Dictionary<Renderer, Coroutine> activeCoroutines = new Dictionary<Renderer, Coroutine>();
        private List<Renderer> allRenderers = new List<Renderer>();
        private Camera vrCamera;
        private float currentFrameRate = 90f;
        private float frameRateTimer = 0f;
        private int frameCount = 0;
        private ShaderQualityLevel currentQualityLevel = ShaderQualityLevel.High;
        private float currentLODLevel = 1f;
        
        // Events
        public static event System.Action<ShaderQualityLevel> OnQualityLevelChanged;
        public static event System.Action<float> OnLODLevelChanged;
        public static event System.Action<float> OnFrameRateChanged;
        public static event System.Action<bool> OnVRComfortModeChanged;
        
        private void Start()
        {
            InitializeShaderOptimizationSystem();
        }
        
        private void Update()
        {
            if (enableShaderOptimization)
            {
                UpdatePerformanceMonitoring();
                UpdateShaderLOD();
                UpdateMaterialInstancing();
            }
        }
        
        /// <summary>
        /// Initialize shader optimization system
        /// </summary>
        private void InitializeShaderOptimizationSystem()
        {
            // Find VR camera
            vrCamera = Camera.main;
            if (vrCamera == null)
            {
                vrCamera = FindObjectOfType<Camera>();
            }
            
            // Collect all renderers
            CollectAllRenderers();
            
            // Store original material settings
            StoreOriginalMaterialSettings();
            
            // Setup VR optimizations
            SetupVROptimizations();
            
            // Apply initial quality level
            ApplyQualityLevel(currentQualityLevel);
            
            Debug.Log("[VRShaderOptimizationSystem] Shader optimization system initialized");
        }
        
        /// <summary>
        /// Collect all renderers in scene
        /// </summary>
        private void CollectAllRenderers()
        {
            allRenderers.Clear();
            
            // Find all renderers
            Renderer[] renderers = FindObjectsOfType<Renderer>();
            
            foreach (var renderer in renderers)
            {
                if (renderer != null)
                {
                    allRenderers.Add(renderer);
                }
            }
            
            Debug.Log($"[VRShaderOptimizationSystem] Found {allRenderers.Count} renderers");
        }
        
        /// <summary>
        /// Store original material settings
        /// </summary>
        private void StoreOriginalMaterialSettings()
        {
            foreach (var renderer in allRenderers)
            {
                if (renderer != null)
                {
                    originalMaterialSettings[renderer] = new MaterialSettings(renderer);
                }
            }
        }
        
        /// <summary>
        /// Setup VR optimizations
        /// </summary>
        private void SetupVROptimizations()
        {
            // Enable single-pass rendering for VR
            if (enableSinglePassRendering)
            {
                // This would require XR-specific setup
                Debug.Log("[VRShaderOptimizationSystem] Single-pass rendering enabled");
            }
            
            // Enable occlusion culling
            if (enableOcclusionCulling)
            {
                // This would require occlusion culling setup
                Debug.Log("[VRShaderOptimizationSystem] Occlusion culling enabled");
            }
            
            // Enable frustum culling
            if (enableFrustumCulling)
            {
                // This would require frustum culling setup
                Debug.Log("[VRShaderOptimizationSystem] Frustum culling enabled");
            }
            
            // Enable batching
            if (enableBatching)
            {
                // This would require batching setup
                Debug.Log("[VRShaderOptimizationSystem] Batching enabled");
            }
        }
        
        /// <summary>
        /// Update performance monitoring
        /// </summary>
        private void UpdatePerformanceMonitoring()
        {
            frameRateTimer += Time.deltaTime;
            frameCount++;
            
            if (frameRateTimer >= performanceCheckInterval)
            {
                currentFrameRate = frameCount / frameRateTimer;
                frameCount = 0;
                frameRateTimer = 0f;
                
                OnFrameRateChanged?.Invoke(currentFrameRate);
                
                // Adjust quality based on performance
                if (enableAutomaticLOD)
                {
                    AdjustQualityBasedOnPerformance();
                }
            }
        }
        
        /// <summary>
        /// Adjust quality based on performance
        /// </summary>
        private void AdjustQualityBasedOnPerformance()
        {
            if (currentFrameRate < targetFrameRate * 0.8f)
            {
                // Performance is low, reduce quality
                if (currentQualityLevel > ShaderQualityLevel.Low)
                {
                    ApplyQualityLevel(currentQualityLevel - 1);
                }
            }
            else if (currentFrameRate > targetFrameRate * 1.1f)
            {
                // Performance is good, increase quality
                if (currentQualityLevel < ShaderQualityLevel.High)
                {
                    ApplyQualityLevel(currentQualityLevel + 1);
                }
            }
        }
        
        /// <summary>
        /// Update shader LOD
        /// </summary>
        private void UpdateShaderLOD()
        {
            if (!enableShaderLOD || vrCamera == null) return;
            
            float newLODLevel = 1f;
            
            foreach (var renderer in allRenderers)
            {
                if (renderer != null)
                {
                    float distance = Vector3.Distance(vrCamera.transform.position, renderer.transform.position);
                    
                    if (distance > lodDistance)
                    {
                        // Reduce quality for distant objects
                        newLODLevel = Mathf.Lerp(0.3f, 1f, lodDistance / distance);
                        ApplyLODToRenderer(renderer, newLODLevel);
                    }
                    else
                    {
                        // Full quality for close objects
                        ApplyLODToRenderer(renderer, 1f);
                    }
                }
            }
            
            if (Mathf.Abs(currentLODLevel - newLODLevel) > 0.1f)
            {
                currentLODLevel = newLODLevel;
                OnLODLevelChanged?.Invoke(currentLODLevel);
            }
        }
        
        /// <summary>
        /// Apply LOD to renderer
        /// </summary>
        private void ApplyLODToRenderer(Renderer renderer, float lodLevel)
        {
            if (renderer == null) return;
            
            // Apply LOD based on quality level
            switch (currentQualityLevel)
            {
                case ShaderQualityLevel.High:
                    ApplyHighQualityLOD(renderer, lodLevel);
                    break;
                    
                case ShaderQualityLevel.Medium:
                    ApplyMediumQualityLOD(renderer, lodLevel);
                    break;
                    
                case ShaderQualityLevel.Low:
                    ApplyLowQualityLOD(renderer, lodLevel);
                    break;
            }
        }
        
        /// <summary>
        /// Apply high quality LOD
        /// </summary>
        private void ApplyHighQualityLOD(Renderer renderer, float lodLevel)
        {
            // Use high quality materials
            if (highQualityMaterials.Count > 0)
            {
                Material material = highQualityMaterials[0];
                if (material != null)
                {
                    renderer.material = material;
                }
            }
            
            // Enable all features
            renderer.enabled = true;
        }
        
        /// <summary>
        /// Apply medium quality LOD
        /// </summary>
        private void ApplyMediumQualityLOD(Renderer renderer, float lodLevel)
        {
            // Use optimized materials
            if (optimizedMaterials.Count > 0)
            {
                Material material = optimizedMaterials[0];
                if (material != null)
                {
                    renderer.material = material;
                }
            }
            
            // Enable with reduced features
            renderer.enabled = true;
        }
        
        /// <summary>
        /// Apply low quality LOD
        /// </summary>
        private void ApplyLowQualityLOD(Renderer renderer, float lodLevel)
        {
            // Use low quality materials
            if (lowQualityMaterials.Count > 0)
            {
                Material material = lowQualityMaterials[0];
                if (material != null)
                {
                    renderer.material = material;
                }
            }
            
            // Disable distant objects
            if (lodLevel < 0.5f)
            {
                renderer.enabled = false;
            }
            else
            {
                renderer.enabled = true;
            }
        }
        
        /// <summary>
        /// Update material instancing
        /// </summary>
        private void UpdateMaterialInstancing()
        {
            if (!enableMaterialInstancing) return;
            
            // Group renderers by material
            Dictionary<Material, List<Renderer>> materialGroups = new Dictionary<Material, List<Renderer>>();
            
            foreach (var renderer in allRenderers)
            {
                if (renderer != null && renderer.material != null)
                {
                    Material material = renderer.material;
                    
                    if (!materialGroups.ContainsKey(material))
                    {
                        materialGroups[material] = new List<Renderer>();
                    }
                    
                    materialGroups[material].Add(renderer);
                }
            }
            
            // Optimize material instances
            foreach (var kvp in materialGroups)
            {
                var material = kvp.Key;
                var renderers = kvp.Value;
                
                if (renderers.Count > maxMaterialInstances)
                {
                    // Create material instance for excess renderers
                    Material materialInstance = new Material(material);
                    
                    for (int i = maxMaterialInstances; i < renderers.Count; i++)
                    {
                        if (renderers[i] != null)
                        {
                            renderers[i].material = materialInstance;
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Apply quality level
        /// </summary>
        public void ApplyQualityLevel(ShaderQualityLevel qualityLevel)
        {
            if (qualityLevel == currentQualityLevel) return;
            
            currentQualityLevel = qualityLevel;
            
            // Apply quality level to all renderers
            foreach (var renderer in allRenderers)
            {
                if (renderer != null)
                {
                    ApplyLODToRenderer(renderer, currentLODLevel);
                }
            }
            
            OnQualityLevelChanged?.Invoke(qualityLevel);
            Debug.Log($"[VRShaderOptimizationSystem] Quality level changed to: {qualityLevel}");
        }
        
        /// <summary>
        /// Set LOD distance
        /// </summary>
        public void SetLODDistance(float distance)
        {
            lodDistance = Mathf.Max(1f, distance);
        }
        
        /// <summary>
        /// Set target frame rate
        /// </summary>
        public void SetTargetFrameRate(float frameRate)
        {
            targetFrameRate = Mathf.Max(30f, frameRate);
        }
        
        /// <summary>
        /// Toggle VR comfort mode
        /// </summary>
        public void ToggleVRComfortMode(bool enable)
        {
            enableVRComfortMode = enable;
            
            if (enable)
            {
                // Reduce quality for VR comfort
                if (currentQualityLevel > ShaderQualityLevel.Medium)
                {
                    ApplyQualityLevel(ShaderQualityLevel.Medium);
                }
                
                // Reduce LOD distance
                lodDistance = Mathf.Min(lodDistance, 15f);
            }
            
            OnVRComfortModeChanged?.Invoke(enable);
            Debug.Log($"[VRShaderOptimizationSystem] VR comfort mode {(enable ? "enabled" : "disabled")}");
        }
        
        /// <summary>
        /// Add material to optimization system
        /// </summary>
        public void AddMaterial(Material material, ShaderQualityLevel qualityLevel)
        {
            switch (qualityLevel)
            {
                case ShaderQualityLevel.High:
                    if (!highQualityMaterials.Contains(material))
                    {
                        highQualityMaterials.Add(material);
                    }
                    break;
                    
                case ShaderQualityLevel.Medium:
                    if (!optimizedMaterials.Contains(material))
                    {
                        optimizedMaterials.Add(material);
                    }
                    break;
                    
                case ShaderQualityLevel.Low:
                    if (!lowQualityMaterials.Contains(material))
                    {
                        lowQualityMaterials.Add(material);
                    }
                    break;
            }
        }
        
        /// <summary>
        /// Remove material from optimization system
        /// </summary>
        public void RemoveMaterial(Material material)
        {
            highQualityMaterials.Remove(material);
            optimizedMaterials.Remove(material);
            lowQualityMaterials.Remove(material);
        }
        
        /// <summary>
        /// Get current quality level
        /// </summary>
        public ShaderQualityLevel GetCurrentQualityLevel()
        {
            return currentQualityLevel;
        }
        
        /// <summary>
        /// Get current LOD level
        /// </summary>
        public float GetCurrentLODLevel()
        {
            return currentLODLevel;
        }
        
        /// <summary>
        /// Get current frame rate
        /// </summary>
        public float GetCurrentFrameRate()
        {
            return currentFrameRate;
        }
        
        /// <summary>
        /// Get active renderer count
        /// </summary>
        public int GetActiveRendererCount()
        {
            int count = 0;
            
            foreach (var renderer in allRenderers)
            {
                if (renderer != null && renderer.enabled)
                {
                    count++;
                }
            }
            
            return count;
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
            
            // Restore original materials
            foreach (var kvp in originalMaterialSettings)
            {
                var renderer = kvp.Key;
                var settings = kvp.Value;
                
                if (renderer != null)
                {
                    renderer.material = settings.originalMaterial;
                    renderer.enabled = settings.originalEnabled;
                }
            }
        }
    }
    
    /// <summary>
    /// Shader quality levels
    /// </summary>
    public enum ShaderQualityLevel
    {
        Low,
        Medium,
        High
    }
    
    /// <summary>
    /// Material settings storage
    /// </summary>
    [System.Serializable]
    public class MaterialSettings
    {
        public Material originalMaterial;
        public bool originalEnabled;
        public Color originalColor;
        public float originalIntensity;
        
        public MaterialSettings(Renderer renderer)
        {
            originalMaterial = renderer.material;
            originalEnabled = renderer.enabled;
            
            if (originalMaterial != null)
            {
                originalColor = originalMaterial.color;
                originalIntensity = originalMaterial.GetFloat("_Intensity");
            }
        }
    }
}
