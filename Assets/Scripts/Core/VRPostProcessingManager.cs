using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

namespace Run4theRelic.Core.Visual
{
    /// <summary>
    /// Avancerad post-processing manager för VR-visuella effekter
    /// Hanterar real-time lighting, particles, och VR-optimerade effekter
    /// </summary>
    public class VRPostProcessingManager : MonoBehaviour
    {
        [Header("Post-Processing Settings")]
        [SerializeField] private Volume postProcessVolume;
        [SerializeField] private bool enableDynamicQuality = true;
        [SerializeField] private float targetFrameRate = 90f;
        [SerializeField] private float qualityAdjustmentSpeed = 2f;
        
        [Header("Visual Effects")]
        [SerializeField] private bool enableBloom = true;
        [SerializeField] private bool enableVignette = true;
        [SerializeField] private bool enableChromaticAberration = true;
        [SerializeField] private bool enableMotionBlur = false; // Disabled for VR
        
        [Header("VR-Specific Settings")]
        [SerializeField] private bool enableVRComfortMode = true;
        [SerializeField] private float maxBloomIntensity = 1.5f;
        [SerializeField] private float maxVignetteIntensity = 0.4f;
        
        // Private fields
        private VolumeProfile volumeProfile;
        private Bloom bloomEffect;
        private Vignette vignetteEffect;
        private ChromaticAberration chromaticEffect;
        private MotionBlur motionBlurEffect;
        
        private float currentFrameRate = 90f;
        private float frameRateTimer = 0f;
        private int frameCount = 0;
        
        // Events
        public static event System.Action<float> OnQualityChanged;
        public static event System.Action<bool> OnVRComfortModeChanged;
        
        private void Start()
        {
            InitializePostProcessing();
        }
        
        private void Update()
        {
            if (enableDynamicQuality)
            {
                UpdateFrameRate();
                AdjustQualityDynamically();
            }
        }
        
        /// <summary>
        /// Initialize post-processing system
        /// </summary>
        private void InitializePostProcessing()
        {
            // Find post-process volume if not assigned
            if (postProcessVolume == null)
            {
                postProcessVolume = FindObjectOfType<Volume>();
            }
            
            if (postProcessVolume != null)
            {
                volumeProfile = postProcessVolume.profile;
                SetupPostProcessingEffects();
            }
            
            // Setup VR-specific optimizations
            SetupVROptimizations();
            
            Debug.Log("[VRPostProcessingManager] Post-processing system initialized");
        }
        
        /// <summary>
        /// Setup post-processing effects
        /// </summary>
        private void SetupPostProcessingEffects()
        {
            if (volumeProfile == null) return;
            
            // Setup Bloom
            if (enableBloom && volumeProfile.TryGet(out bloomEffect))
            {
                bloomEffect.intensity.value = 0.8f;
                bloomEffect.threshold.value = 0.6f;
                bloomEffect.scatter.value = 0.7f;
            }
            
            // Setup Vignette
            if (enableVignette && volumeProfile.TryGet(out vignetteEffect))
            {
                vignetteEffect.intensity.value = 0.2f;
                vignetteEffect.smoothness.value = 0.4f;
                vignetteEffect.rounded.value = true;
            }
            
            // Setup Chromatic Aberration
            if (enableChromaticAberration && volumeProfile.TryGet(out chromaticEffect))
            {
                chromaticEffect.intensity.value = 0.1f;
            }
            
            // Setup Motion Blur (disabled for VR by default)
            if (volumeProfile.TryGet(out motionBlurEffect))
            {
                motionBlurEffect.intensity.value = 0f;
                motionBlurEffect.enabled.value = false;
            }
        }
        
        /// <summary>
        /// Setup VR-specific optimizations
        /// </summary>
        private void SetupVROptimizations()
        {
            // Disable motion blur for VR comfort
            if (motionBlurEffect != null)
            {
                motionBlurEffect.enabled.value = false;
            }
            
            // Adjust bloom intensity for VR
            if (bloomEffect != null && enableVRComfortMode)
            {
                bloomEffect.intensity.value = Mathf.Min(bloomEffect.intensity.value, maxBloomIntensity);
            }
            
            // Adjust vignette for VR comfort
            if (vignetteEffect != null && enableVRComfortMode)
            {
                vignetteEffect.intensity.value = Mathf.Min(vignetteEffect.intensity.value, maxVignetteIntensity);
            }
        }
        
        /// <summary>
        /// Update frame rate monitoring
        /// </summary>
        private void UpdateFrameRate()
        {
            frameRateTimer += Time.deltaTime;
            frameCount++;
            
            if (frameRateTimer >= 1f)
            {
                currentFrameRate = frameCount / frameRateTimer;
                frameCount = 0;
                frameRateTimer = 0f;
            }
        }
        
        /// <summary>
        /// Adjust quality dynamically based on performance
        /// </summary>
        private void AdjustQualityDynamically()
        {
            if (currentFrameRate < targetFrameRate * 0.8f)
            {
                // Performance is low, reduce quality
                ReduceVisualQuality();
            }
            else if (currentFrameRate > targetFrameRate * 1.1f)
            {
                // Performance is good, increase quality
                IncreaseVisualQuality();
            }
        }
        
        /// <summary>
        /// Reduce visual quality for better performance
        /// </summary>
        private void ReduceVisualQuality()
        {
            if (bloomEffect != null)
            {
                bloomEffect.intensity.value = Mathf.Max(bloomEffect.intensity.value - Time.deltaTime * qualityAdjustmentSpeed, 0.3f);
            }
            
            if (vignetteEffect != null)
            {
                vignetteEffect.intensity.value = Mathf.Max(vignetteEffect.intensity.value - Time.deltaTime * qualityAdjustmentSpeed, 0.1f);
            }
            
            OnQualityChanged?.Invoke(currentFrameRate);
        }
        
        /// <summary>
        /// Increase visual quality when performance allows
        /// </summary>
        private void IncreaseVisualQuality()
        {
            if (bloomEffect != null)
            {
                bloomEffect.intensity.value = Mathf.Min(bloomEffect.intensity.value + Time.deltaTime * qualityAdjustmentSpeed, 1.2f);
            }
            
            if (vignetteEffect != null)
            {
                vignetteEffect.intensity.value = Mathf.Min(vignetteEffect.intensity.value + Time.deltaTime * qualityAdjustmentSpeed, 0.3f);
            }
            
            OnQualityChanged?.Invoke(currentFrameRate);
        }
        
        /// <summary>
        /// Toggle VR comfort mode
        /// </summary>
        public void ToggleVRComfortMode(bool enable)
        {
            enableVRComfortMode = enable;
            
            if (enable)
            {
                SetupVROptimizations();
            }
            
            OnVRComfortModeChanged?.Invoke(enable);
            Debug.Log($"[VRPostProcessingManager] VR comfort mode {(enable ? "enabled" : "disabled")}");
        }
        
        /// <summary>
        /// Set bloom intensity
        /// </summary>
        public void SetBloomIntensity(float intensity)
        {
            if (bloomEffect != null)
            {
                bloomEffect.intensity.value = Mathf.Clamp(intensity, 0f, maxBloomIntensity);
            }
        }
        
        /// <summary>
        /// Set vignette intensity
        /// </summary>
        public void SetVignetteIntensity(float intensity)
        {
            if (vignetteEffect != null)
            {
                vignetteEffect.intensity.value = Mathf.Clamp(intensity, 0f, maxVignetteIntensity);
            }
        }
        
        /// <summary>
        /// Get current frame rate
        /// </summary>
        public float GetCurrentFrameRate()
        {
            return currentFrameRate;
        }
        
        /// <summary>
        /// Get current quality level
        /// </summary>
        public float GetCurrentQualityLevel()
        {
            if (bloomEffect != null)
            {
                return bloomEffect.intensity.value / maxBloomIntensity;
            }
            return 1f;
        }
    }
}
