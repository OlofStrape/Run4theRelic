using UnityEngine;
using Run4theRelic.Core;
using System.Collections.Generic;
using System.Collections;

namespace Run4theRelic.Core
{
    /// <summary>
    /// Manages 3D models, textures, and materials for different room types in Run4theRelic.
    /// Handles model loading, texture management, and material creation for immersive VR environments.
    /// </summary>
    public class VRModelTextureManager : MonoBehaviour
    {
        [Header("Model Management")]
        [SerializeField] private bool autoLoadModels = true;
        [SerializeField] private bool enableModelPooling = true;
        [SerializeField] private int maxPooledModels = 50;
        
        [Header("Texture Management")]
        [SerializeField] private bool autoLoadTextures = true;
        [SerializeField] private bool enableTextureCompression = true;
        [SerializeField] private int maxTextureSize = 2048;
        
        [Header("Material Management")]
        [SerializeField] private bool autoCreateMaterials = true;
        [SerializeField] private bool enableMaterialInstancing = true;
        
        [Header("Room Model Sets")]
        [SerializeField] private RoomModelSet[] roomModelSets;
        
        // Model Data
        private Dictionary<RoomType, RoomModelSet> _modelSets = new Dictionary<RoomType, RoomModelSet>();
        private Dictionary<string, GameObject> _modelPool = new Dictionary<string, GameObject>();
        private Dictionary<string, Material> _materialCache = new Dictionary<string, Material>();
        
        // Texture Data
        private Dictionary<string, Texture2D> _textureCache = new Dictionary<string, Texture2D>();
        private Dictionary<string, Material> _textureMaterialCache = new Dictionary<string, Material>();
        
        // VR References
        private VRManager _vrManager;
        private VREnvironmentSystem _environmentSystem;
        
        // Events
        public static event System.Action<RoomType> OnModelsLoaded;
        public static event System.Action<string> OnModelLoaded;
        public static event System.Action<string> OnTextureLoaded;
        public static event System.Action<string> OnMaterialCreated;
        
        protected override void Start()
        {
            // Initialize model texture manager
            InitializeModelTextureManager();
            
            // Subscribe to VR events
            SubscribeToVREvents();
        }
        
        /// <summary>
        /// Initialize the model texture manager.
        /// </summary>
        private void InitializeModelTextureManager()
        {
            // Find VR components
            _vrManager = FindObjectOfType<VRManager>();
            _environmentSystem = FindObjectOfType<VREnvironmentSystem>();
            
            if (_vrManager == null)
            {
                Debug.LogWarning("VRModelTextureManager: No VRManager found!");
            }
            
            if (_environmentSystem == null)
            {
                Debug.LogWarning("VRModelTextureManager: No VREnvironmentSystem found!");
            }
            
            // Setup default room model sets if none provided
            if (roomModelSets == null || roomModelSets.Length == 0)
            {
                CreateDefaultRoomModelSets();
            }
            
            // Register model sets
            RegisterModelSets();
            
            // Load models if auto-load is enabled
            if (autoLoadModels)
            {
                StartCoroutine(LoadAllModelsAsync());
            }
            
            Debug.Log("VRModelTextureManager: Initialized successfully");
        }
        
        /// <summary>
        /// Subscribe to VR system events.
        /// </summary>
        private void SubscribeToVREvents()
        {
            if (_environmentSystem != null)
            {
                VREnvironmentSystem.OnRoomChanged += OnRoomChanged;
            }
        }
        
        /// <summary>
        /// Create default room model sets.
        /// </summary>
        private void CreateDefaultRoomModelSets()
        {
            roomModelSets = new RoomModelSet[]
            {
                CreateTutorialModelSet(),
                CreateBeginnerModelSet(),
                CreateIntermediateModelSet(),
                CreateAdvancedModelSet(),
                CreateExpertModelSet(),
                CreateMasterModelSet()
            };
        }
        
        /// <summary>
        /// Create tutorial room model set.
        /// </summary>
        /// <returns>Tutorial room model set.</returns>
        private RoomModelSet CreateTutorialModelSet()
        {
            return new RoomModelSet
            {
                roomType = RoomType.Entrance,
                roomName = "Tutorial Chamber",
                description = "Simple, welcoming models for tutorial room",
                
                // Wall models
                wallModels = new ModelInfo[]
                {
                    new ModelInfo { name = "Tutorial_Wall_Simple", category = ModelCategory.Wall, complexity = ModelComplexity.Simple },
                    new ModelInfo { name = "Tutorial_Wall_Plain", category = ModelCategory.Wall, complexity = ModelComplexity.Simple }
                },
                
                // Floor models
                floorModels = new ModelInfo[]
                {
                    new ModelInfo { name = "Tutorial_Floor_Stone", category = ModelCategory.Floor, complexity = ModelComplexity.Simple },
                    new ModelInfo { name = "Tutorial_Floor_Wood", category = ModelCategory.Floor, complexity = ModelComplexity.Simple }
                },
                
                // Ceiling models
                ceilingModels = new ModelInfo[]
                {
                    new ModelInfo { name = "Tutorial_Ceiling_Flat", category = ModelCategory.Ceiling, complexity = ModelComplexity.Simple }
                },
                
                // Decoration models
                decorationModels = new ModelInfo[]
                {
                    new ModelInfo { name = "Tutorial_Torch", category = ModelCategory.Decoration, complexity = ModelComplexity.Simple },
                    new ModelInfo { name = "Tutorial_Banner", category = ModelCategory.Decoration, complexity = ModelComplexity.Simple }
                },
                
                // Furniture models
                furnitureModels = new ModelInfo[]
                {
                    new ModelInfo { name = "Tutorial_Table", category = ModelCategory.Furniture, complexity = ModelComplexity.Simple },
                    new ModelInfo { name = "Tutorial_Chair", category = ModelCategory.Furniture, complexity = ModelComplexity.Simple }
                },
                
                // Texture sets
                textureSets = new TextureSet[]
                {
                    new TextureSet
                    {
                        name = "Tutorial_Stone",
                        category = TextureCategory.Wall,
                        baseColor = new Color(0.8f, 0.8f, 0.8f),
                        roughness = 0.7f,
                        metallic = 0.0f
                    },
                    new TextureSet
                    {
                        name = "Tutorial_Wood",
                        category = TextureCategory.Floor,
                        baseColor = new Color(0.6f, 0.4f, 0.2f),
                        roughness = 0.8f,
                        metallic = 0.0f
                    }
                }
            };
        }
        
        /// <summary>
        /// Create beginner room model set.
        /// </summary>
        /// <returns>Beginner room model set.</returns>
        private RoomModelSet CreateBeginnerModelSet()
        {
            return new RoomModelSet
            {
                roomType = RoomType.Corridor,
                roomName = "Ancient Library",
                description = "Warm, inviting models for library atmosphere",
                
                // Wall models
                wallModels = new ModelInfo[]
                {
                    new ModelInfo { name = "Library_Wall_Stone", category = ModelCategory.Wall, complexity = ModelComplexity.Medium },
                    new ModelInfo { name = "Library_Wall_Bookcase", category = ModelCategory.Wall, complexity = ModelComplexity.Medium },
                    new ModelInfo { name = "Library_Wall_Arch", category = ModelCategory.Wall, complexity = ModelComplexity.Medium }
                },
                
                // Floor models
                floorModels = new ModelInfo[]
                {
                    new ModelInfo { name = "Library_Floor_Marble", category = ModelCategory.Floor, complexity = ModelComplexity.Medium },
                    new ModelInfo { name = "Library_Floor_Carpet", category = ModelCategory.Floor, complexity = ModelComplexity.Medium }
                },
                
                // Ceiling models
                ceilingModels = new ModelInfo[]
                {
                    new ModelInfo { name = "Library_Ceiling_Vaulted", category = ModelCategory.Ceiling, complexity = ModelComplexity.Medium },
                    new ModelInfo { name = "Library_Ceiling_Beam", category = ModelCategory.Ceiling, complexity = ModelComplexity.Medium }
                },
                
                // Decoration models
                decorationModels = new ModelInfo[]
                {
                    new ModelInfo { name = "Library_Candle", category = ModelCategory.Decoration, complexity = ModelComplexity.Medium },
                    new ModelInfo { name = "Library_Scroll", category = ModelCategory.Decoration, complexity = ModelComplexity.Medium },
                    new ModelInfo { name = "Library_Plant", category = ModelCategory.Decoration, complexity = ModelComplexity.Medium }
                },
                
                // Furniture models
                furnitureModels = new ModelInfo[]
                {
                    new ModelInfo { name = "Library_Desk", category = ModelCategory.Furniture, complexity = ModelComplexity.Medium },
                    new ModelInfo { name = "Library_Bookshelf", category = ModelCategory.Furniture, complexity = ModelComplexity.Medium },
                    new ModelInfo { name = "Library_Chair_Leather", category = ModelCategory.Furniture, complexity = ModelComplexity.Medium }
                },
                
                // Texture sets
                textureSets = new TextureSet[]
                {
                    new TextureSet
                    {
                        name = "Library_Stone",
                        category = TextureCategory.Wall,
                        baseColor = new Color(0.7f, 0.6f, 0.5f),
                        roughness = 0.6f,
                        metallic = 0.0f
                    },
                    new TextureSet
                    {
                        name = "Library_Wood_Dark",
                        category = TextureCategory.Floor,
                        baseColor = new Color(0.3f, 0.2f, 0.1f),
                        roughness = 0.7f,
                        metallic = 0.0f
                    },
                    new TextureSet
                    {
                        name = "Library_Leather",
                        category = TextureCategory.Furniture,
                        baseColor = new Color(0.4f, 0.3f, 0.2f),
                        roughness = 0.9f,
                        metallic = 0.0f
                    }
                }
            };
        }
        
        /// <summary>
        /// Create intermediate room model set.
        /// </summary>
        /// <returns>Intermediate room model set.</returns>
        private RoomModelSet CreateIntermediateModelSet()
        {
            return new RoomModelSet
            {
                roomType = RoomType.PuzzleRoom,
                roomName = "Crystal Cavern",
                description = "Mystical crystal models for cavern atmosphere",
                
                // Wall models
                wallModels = new ModelInfo[]
                {
                    new ModelInfo { name = "Cavern_Wall_Crystal", category = ModelCategory.Wall, complexity = ModelComplexity.High },
                    new ModelInfo { name = "Cavern_Wall_Stalactite", category = ModelCategory.Wall, complexity = ModelComplexity.High },
                    new ModelInfo { name = "Cavern_Wall_Geode", category = ModelCategory.Wall, complexity = ModelComplexity.High }
                },
                
                // Floor models
                floorModels = new ModelInfo[]
                {
                    new ModelInfo { name = "Cavern_Floor_Crystal", category = ModelCategory.Floor, complexity = ModelComplexity.High },
                    new ModelInfo { name = "Cavern_Floor_Stalagmite", category = ModelCategory.Floor, complexity = ModelComplexity.High }
                },
                
                // Ceiling models
                ceilingModels = new ModelInfo[]
                {
                    new ModelInfo { name = "Cavern_Ceiling_Crystal", category = ModelCategory.Ceiling, complexity = ModelComplexity.High },
                    new ModelInfo { name = "Cavern_Ceiling_Stalactite", category = ModelCategory.Ceiling, complexity = ModelComplexity.High }
                },
                
                // Decoration models
                decorationModels = new ModelInfo[]
                {
                    new ModelInfo { name = "Cavern_Crystal_Cluster", category = ModelCategory.Decoration, complexity = ModelComplexity.High },
                    new ModelInfo { name = "Cavern_Crystal_Shard", category = ModelCategory.Decoration, complexity = ModelComplexity.High },
                    new ModelInfo { name = "Cavern_Crystal_Growth", category = ModelCategory.Decoration, complexity = ModelComplexity.High }
                },
                
                // Furniture models
                furnitureModels = new ModelInfo[]
                {
                    new ModelInfo { name = "Cavern_Altar", category = ModelCategory.Furniture, complexity = ModelComplexity.High },
                    new ModelInfo { name = "Cavern_Pedestal", category = ModelCategory.Furniture, complexity = ModelComplexity.High }
                },
                
                // Texture sets
                textureSets = new TextureSet[]
                {
                    new TextureSet
                    {
                        name = "Cavern_Crystal",
                        category = TextureCategory.Wall,
                        baseColor = new Color(0.6f, 0.8f, 1.0f),
                        roughness = 0.1f,
                        metallic = 0.0f
                    },
                    new TextureSet
                    {
                        name = "Cavern_Stone",
                        category = TextureCategory.Floor,
                        baseColor = new Color(0.4f, 0.4f, 0.5f),
                        roughness = 0.8f,
                        metallic = 0.0f
                    }
                }
            };
        }
        
        /// <summary>
        /// Create advanced room model set.
        /// </summary>
        /// <returns>Advanced room model set.</returns>
        private RoomModelSet CreateAdvancedModelSet()
        {
            return new RoomModelSet
            {
                roomType = RoomType.RelicChamber,
                roomName = "Shadow Sanctum",
                description = "Dark, foreboding models for shadow sanctum",
                
                // Wall models
                wallModels = new ModelInfo[]
                {
                    new ModelInfo { name = "Sanctum_Wall_Shadow", category = ModelCategory.Wall, complexity = ModelComplexity.High },
                    new ModelInfo { name = "Sanctum_Wall_Dark", category = ModelCategory.Wall, complexity = ModelComplexity.High },
                    new ModelInfo { name = "Sanctum_Wall_Ancient", category = ModelCategory.Wall, complexity = ModelComplexity.High }
                },
                
                // Floor models
                floorModels = new ModelInfo[]
                {
                    new ModelInfo { name = "Sanctum_Floor_Shadow", category = ModelCategory.Floor, complexity = ModelComplexity.High },
                    new ModelInfo { name = "Sanctum_Floor_Dark", category = ModelCategory.Floor, complexity = ModelComplexity.High }
                },
                
                // Ceiling models
                ceilingModels = new ModelInfo[]
                {
                    new ModelInfo { name = "Sanctum_Ceiling_Shadow", category = ModelCategory.Ceiling, complexity = ModelComplexity.High },
                    new ModelInfo { name = "Sanctum_Ceiling_Dark", category = ModelCategory.Ceiling, complexity = ModelComplexity.High }
                },
                
                // Decoration models
                decorationModels = new ModelInfo[]
                {
                    new ModelInfo { name = "Sanctum_Shadow_Effect", category = ModelCategory.Decoration, complexity = ModelComplexity.High },
                    new ModelInfo { name = "Sanctum_Dark_Orb", category = ModelCategory.Decoration, complexity = ModelComplexity.High },
                    new ModelInfo { name = "Sanctum_Ancient_Rune", category = ModelCategory.Decoration, complexity = ModelComplexity.High }
                },
                
                // Furniture models
                furnitureModels = new ModelInfo[]
                {
                    new ModelInfo { name = "Sanctum_Throne", category = ModelCategory.Furniture, complexity = ModelComplexity.High },
                    new ModelInfo { name = "Sanctum_Altar_Dark", category = ModelCategory.Furniture, complexity = ModelComplexity.High }
                },
                
                // Texture sets
                textureSets = new TextureSet[]
                {
                    new TextureSet
                    {
                        name = "Sanctum_Shadow",
                        category = TextureCategory.Wall,
                        baseColor = new Color(0.2f, 0.1f, 0.3f),
                        roughness = 0.9f,
                        metallic = 0.0f
                    },
                    new TextureSet
                    {
                        name = "Sanctum_Dark",
                        category = TextureCategory.Floor,
                        baseColor = new Color(0.1f, 0.05f, 0.15f),
                        roughness = 0.8f,
                        metallic = 0.0f
                    }
                }
            };
        }
        
        /// <summary>
        /// Create expert room model set.
        /// </summary>
        /// <returns>Expert room model set.</returns>
        private RoomModelSet CreateExpertModelSet()
        {
            return new RoomModelSet
            {
                roomType = RoomType.BossRoom,
                roomName = "Void Nexus",
                description = "Otherworldly models for void nexus",
                
                // Wall models
                wallModels = new ModelInfo[]
                {
                    new ModelInfo { name = "Nexus_Wall_Void", category = ModelCategory.Wall, complexity = ModelComplexity.VeryHigh },
                    new ModelInfo { name = "Nexus_Wall_Energy", category = ModelCategory.Wall, complexity = ModelComplexity.VeryHigh },
                    new ModelInfo { name = "Nexus_Wall_Reality", category = ModelCategory.Wall, complexity = ModelComplexity.VeryHigh }
                },
                
                // Floor models
                floorModels = new ModelInfo[]
                {
                    new ModelInfo { name = "Nexus_Floor_Void", category = ModelCategory.Floor, complexity = ModelComplexity.VeryHigh },
                    new ModelInfo { name = "Nexus_Floor_Energy", category = ModelCategory.Floor, complexity = ModelComplexity.VeryHigh }
                },
                
                // Ceiling models
                ceilingModels = new ModelInfo[]
                {
                    new ModelInfo { name = "Nexus_Ceiling_Void", category = ModelCategory.Ceiling, complexity = ModelComplexity.VeryHigh },
                    new ModelInfo { name = "Nexus_Ceiling_Energy", category = ModelCategory.Ceiling, complexity = ModelComplexity.VeryHigh }
                },
                
                // Decoration models
                decorationModels = new ModelInfo[]
                {
                    new ModelInfo { name = "Nexus_Energy_Field", category = ModelCategory.Decoration, complexity = ModelComplexity.VeryHigh },
                    new ModelInfo { name = "Nexus_Reality_Shard", category = ModelCategory.Decoration, complexity = ModelComplexity.VeryHigh },
                    new ModelInfo { name = "Nexus_Void_Portal", category = ModelCategory.Decoration, complexity = ModelComplexity.VeryHigh }
                },
                
                // Furniture models
                furnitureModels = new ModelInfo[]
                {
                    new ModelInfo { name = "Nexus_Energy_Throne", category = ModelCategory.Furniture, complexity = ModelComplexity.VeryHigh },
                    new ModelInfo { name = "Nexus_Reality_Altar", category = ModelCategory.Furniture, complexity = ModelComplexity.VeryHigh }
                },
                
                // Texture sets
                textureSets = new TextureSet[]
                {
                    new TextureSet
                    {
                        name = "Nexus_Energy",
                        category = TextureCategory.Wall,
                        baseColor = new Color(0.1f, 0.6f, 0.7f),
                        roughness = 0.2f,
                        metallic = 0.8f
                    },
                    new TextureSet
                    {
                        name = "Nexus_Void",
                        category = TextureCategory.Floor,
                        baseColor = new Color(0.05f, 0.1f, 0.15f),
                        roughness = 0.9f,
                        metallic = 0.0f
                    }
                }
            };
        }
        
        /// <summary>
        /// Create master room model set.
        /// </summary>
        /// <returns>Master room model set.</returns>
        private RoomModelSet CreateMasterModelSet()
        {
            return new RoomModelSet
            {
                roomType = RoomType.Exit,
                roomName = "Eternal Abyss",
                description = "Apocalyptic models for eternal abyss",
                
                // Wall models
                wallModels = new ModelInfo[]
                {
                    new ModelInfo { name = "Abyss_Wall_Apocalypse", category = ModelCategory.Wall, complexity = ModelComplexity.VeryHigh },
                    new ModelInfo { name = "Abyss_Wall_Destruction", category = ModelCategory.Wall, complexity = ModelComplexity.VeryHigh },
                    new ModelInfo { name = "Abyss_Wall_Chaos", category = ModelCategory.Wall, complexity = ModelComplexity.VeryHigh }
                },
                
                // Floor models
                floorModels = new ModelInfo[]
                {
                    new ModelInfo { name = "Abyss_Floor_Apocalypse", category = ModelCategory.Floor, complexity = ModelComplexity.VeryHigh },
                    new ModelInfo { name = "Abyss_Floor_Destruction", category = ModelCategory.Floor, complexity = ModelComplexity.VeryHigh }
                },
                
                // Ceiling models
                ceilingModels = new ModelInfo[]
                {
                    new ModelInfo { name = "Abyss_Ceiling_Apocalypse", category = ModelCategory.Ceiling, complexity = ModelComplexity.VeryHigh },
                    new ModelInfo { name = "Abyss_Ceiling_Destruction", category = ModelCategory.Ceiling, complexity = ModelComplexity.VeryHigh }
                },
                
                // Decoration models
                decorationModels = new ModelInfo[]
                {
                    new ModelInfo { name = "Abyss_Apocalypse_Effect", category = ModelCategory.Decoration, complexity = ModelComplexity.VeryHigh },
                    new ModelInfo { name = "Abyss_Destruction_Field", category = ModelCategory.Decoration, complexity = ModelComplexity.VeryHigh },
                    new ModelInfo { name = "Abyss_Chaos_Portal", category = ModelCategory.Decoration, complexity = ModelComplexity.VeryHigh }
                },
                
                // Furniture models
                furnitureModels = new ModelInfo[]
                {
                    new ModelInfo { name = "Abyss_Apocalypse_Throne", category = ModelCategory.Furniture, complexity = ModelComplexity.VeryHigh },
                    new ModelInfo { name = "Abyss_Destruction_Altar", category = ModelCategory.Furniture, complexity = ModelComplexity.VeryHigh }
                },
                
                // Texture sets
                textureSets = new TextureSet[]
                {
                    new TextureSet
                    {
                        name = "Abyss_Apocalypse",
                        category = TextureCategory.Wall,
                        baseColor = new Color(0.6f, 0.1f, 0.1f),
                        roughness = 0.8f,
                        metallic = 0.2f
                    },
                    new TextureSet
                    {
                        name = "Abyss_Destruction",
                        category = TextureCategory.Floor,
                        baseColor = new Color(0.4f, 0.05f, 0.05f),
                        roughness = 0.9f,
                        metallic = 0.0f
                    }
                }
            };
        }
        
        /// <summary>
        /// Register all model sets.
        /// </summary>
        private void RegisterModelSets()
        {
            foreach (var modelSet in roomModelSets)
            {
                if (modelSet != null)
                {
                    _modelSets[modelSet.roomType] = modelSet;
                    Debug.Log($"VRModelTextureManager: Registered model set for {modelSet.roomName}");
                }
            }
        }
        
        /// <summary>
        /// Load all models asynchronously.
        /// </summary>
        /// <returns>Coroutine.</returns>
        private IEnumerator LoadAllModelsAsync()
        {
            foreach (var kvp in _modelSets)
            {
                var roomType = kvp.Key;
                var modelSet = kvp.Value;
                
                Debug.Log($"VRModelTextureManager: Loading models for {modelSet.roomName}");
                
                // Load models for this room type
                yield return StartCoroutine(LoadRoomModelsAsync(modelSet));
                
                // Notify models loaded for this room type
                OnModelsLoaded?.Invoke(roomType);
            }
            
            Debug.Log("VRModelTextureManager: All models loaded successfully");
        }
        
        /// <summary>
        /// Load models for a specific room type.
        /// </summary>
        /// <param name="modelSet">Room model set to load.</param>
        /// <returns>Coroutine.</returns>
        private IEnumerator LoadRoomModelsAsync(RoomModelSet modelSet)
        {
            // Load wall models
            yield return StartCoroutine(LoadModelCategoryAsync(modelSet.wallModels));
            
            // Load floor models
            yield return StartCoroutine(LoadModelCategoryAsync(modelSet.floorModels));
            
            // Load ceiling models
            yield return StartCoroutine(LoadModelCategoryAsync(modelSet.ceilingModels));
            
            // Load decoration models
            yield return StartCoroutine(LoadModelCategoryAsync(modelSet.decorationModels));
            
            // Load furniture models
            yield return StartCoroutine(LoadModelCategoryAsync(modelSet.furnitureModels));
            
            // Load textures
            yield return StartCoroutine(LoadTexturesAsync(modelSet.textureSets));
            
            // Create materials
            yield return StartCoroutine(CreateMaterialsAsync(modelSet));
        }
        
        /// <summary>
        /// Load models for a specific category.
        /// </summary>
        /// <param name="models">Array of model info to load.</param>
        /// <returns>Coroutine.</returns>
        private IEnumerator LoadModelCategoryAsync(ModelInfo[] models)
        {
            if (models == null) yield break;
            
            foreach (var modelInfo in models)
            {
                if (modelInfo != null)
                {
                    // Load model (this would integrate with actual model loading system)
                    yield return StartCoroutine(LoadModelAsync(modelInfo));
                }
            }
        }
        
        /// <summary>
        /// Load a single model asynchronously.
        /// </summary>
        /// <param name="modelInfo">Model info to load.</param>
        /// <returns>Coroutine.</returns>
        private IEnumerator LoadModelAsync(ModelInfo modelInfo)
        {
            // Simulate model loading delay
            yield return new WaitForSeconds(0.1f);
            
            // Create placeholder model GameObject
            GameObject modelObject = CreatePlaceholderModel(modelInfo);
            
            // Add to model pool
            if (enableModelPooling)
            {
                _modelPool[modelInfo.name] = modelObject;
            }
            
            // Notify model loaded
            OnModelLoaded?.Invoke(modelInfo.name);
            
            Debug.Log($"VRModelTextureManager: Loaded model {modelInfo.name}");
        }
        
        /// <summary>
        /// Create a placeholder model GameObject.
        /// </summary>
        /// <param name="modelInfo">Model info to create placeholder for.</param>
        /// <returns>Placeholder GameObject.</returns>
        private GameObject CreatePlaceholderModel(ModelInfo modelInfo)
        {
            GameObject placeholder = new GameObject($"Placeholder_{modelInfo.name}");
            
            // Add basic components based on category
            switch (modelInfo.category)
            {
                case ModelCategory.Wall:
                    placeholder.AddComponent<BoxCollider>();
                    placeholder.AddComponent<MeshRenderer>();
                    break;
                    
                case ModelCategory.Floor:
                    placeholder.AddComponent<BoxCollider>();
                    placeholder.AddComponent<MeshRenderer>();
                    break;
                    
                case ModelCategory.Ceiling:
                    placeholder.AddComponent<BoxCollider>();
                    placeholder.AddComponent<MeshRenderer>();
                    break;
                    
                case ModelCategory.Decoration:
                    placeholder.AddComponent<SphereCollider>();
                    placeholder.AddComponent<MeshRenderer>();
                    break;
                    
                case ModelCategory.Furniture:
                    placeholder.AddComponent<BoxCollider>();
                    placeholder.AddComponent<MeshRenderer>();
                    break;
            }
            
            // Set scale based on complexity
            float scale = GetScaleForComplexity(modelInfo.complexity);
            placeholder.transform.localScale = Vector3.one * scale;
            
            return placeholder;
        }
        
        /// <summary>
        /// Get scale for model complexity.
        /// </summary>
        /// <param name="complexity">Model complexity.</param>
        /// <returns>Scale value.</returns>
        private float GetScaleForComplexity(ModelComplexity complexity)
        {
            switch (complexity)
            {
                case ModelComplexity.Simple: return 1.0f;
                case ModelComplexity.Medium: return 1.2f;
                case ModelComplexity.High: return 1.5f;
                case ModelComplexity.VeryHigh: return 2.0f;
                default: return 1.0f;
            }
        }
        
        /// <summary>
        /// Load textures asynchronously.
        /// </summary>
        /// <param name="textureSets">Array of texture sets to load.</param>
        /// <returns>Coroutine.</returns>
        private IEnumerator LoadTexturesAsync(TextureSet[] textureSets)
        {
            if (textureSets == null) yield break;
            
            foreach (var textureSet in textureSets)
            {
                if (textureSet != null)
                {
                    // Load texture (this would integrate with actual texture loading system)
                    yield return StartCoroutine(LoadTextureAsync(textureSet));
                }
            }
        }
        
        /// <summary>
        /// Load a single texture asynchronously.
        /// </summary>
        /// <param name="textureSet">Texture set to load.</param>
        /// <returns>Coroutine.</returns>
        private IEnumerator LoadTextureAsync(TextureSet textureSet)
        {
            // Simulate texture loading delay
            yield return new WaitForSeconds(0.05f);
            
            // Create placeholder texture
            Texture2D placeholderTexture = CreatePlaceholderTexture(textureSet);
            
            // Add to texture cache
            _textureCache[textureSet.name] = placeholderTexture;
            
            // Notify texture loaded
            OnTextureLoaded?.Invoke(textureSet.name);
            
            Debug.Log($"VRModelTextureManager: Loaded texture {textureSet.name}");
        }
        
        /// <summary>
        /// Create a placeholder texture.
        /// </summary>
        /// <param name="textureSet">Texture set to create placeholder for.</param>
        /// <returns>Placeholder Texture2D.</returns>
        private Texture2D CreatePlaceholderTexture(TextureSet textureSet)
        {
            // Create simple colored texture
            Texture2D texture = new Texture2D(64, 64);
            Color[] pixels = new Color[64 * 64];
            
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = textureSet.baseColor;
            }
            
            texture.SetPixels(pixels);
            texture.Apply();
            
            return texture;
        }
        
        /// <summary>
        /// Create materials asynchronously.
        /// </summary>
        /// <param name="modelSet">Room model set to create materials for.</param>
        /// <returns>Coroutine.</returns>
        private IEnumerator CreateMaterialsAsync(RoomModelSet modelSet)
        {
            if (modelSet.textureSets == null) yield break;
            
            foreach (var textureSet in modelSet.textureSets)
            {
                if (textureSet != null)
                {
                    // Create material (this would integrate with actual material creation system)
                    yield return StartCoroutine(CreateMaterialAsync(textureSet));
                }
            }
        }
        
        /// <summary>
        /// Create a single material asynchronously.
        /// </summary>
        /// <param name="textureSet">Texture set to create material for.</param>
        /// <returns>Coroutine.</returns>
        private IEnumerator CreateMaterialAsync(TextureSet textureSet)
        {
            // Simulate material creation delay
            yield return new WaitForSeconds(0.02f);
            
            // Create placeholder material
            Material material = CreatePlaceholderMaterial(textureSet);
            
            // Add to material cache
            _materialCache[textureSet.name] = material;
            
            // Notify material created
            OnMaterialCreated?.Invoke(textureSet.name);
            
            Debug.Log($"VRModelTextureManager: Created material {textureSet.name}");
        }
        
        /// <summary>
        /// Create a placeholder material.
        /// </summary>
        /// <param name="textureSet">Texture set to create material for.</param>
        /// <returns>Placeholder Material.</returns>
        private Material CreatePlaceholderMaterial(TextureSet textureSet)
        {
            // Create material with standard shader
            Material material = new Material(Shader.Find("Standard"));
            
            // Set properties
            material.color = textureSet.baseColor;
            material.SetFloat("_Glossiness", 1f - textureSet.roughness);
            material.SetFloat("_Metallic", textureSet.metallic);
            
            // Set texture if available
            if (_textureCache.ContainsKey(textureSet.name))
            {
                material.mainTexture = _textureCache[textureSet.name];
            }
            
            return material;
        }
        
        /// <summary>
        /// Handle room changes.
        /// </summary>
        /// <param name="newRoom">New room that was entered.</param>
        private void OnRoomChanged(VREnvironmentSystem.VRRoom newRoom)
        {
            // This would integrate with room system
            Debug.Log($"VRModelTextureManager: Room changed to {newRoom.roomName}");
        }
        
        /// <summary>
        /// Get model set for a room type.
        /// </summary>
        /// <param name="roomType">Room type to get model set for.</param>
        /// <returns>Room model set, or null if not found.</returns>
        public RoomModelSet GetModelSet(RoomType roomType)
        {
            if (_modelSets.ContainsKey(roomType))
            {
                return _modelSets[roomType];
            }
            return null;
        }
        
        /// <summary>
        /// Get model from pool.
        /// </summary>
        /// <param name="modelName">Name of the model to get.</param>
        /// <returns>Model GameObject, or null if not found.</returns>
        public GameObject GetModelFromPool(string modelName)
        {
            if (_modelPool.ContainsKey(modelName))
            {
                return _modelPool[modelName];
            }
            return null;
        }
        
        /// <summary>
        /// Get material from cache.
        /// </summary>
        /// <param name="materialName">Name of the material to get.</param>
        /// <returns>Material, or null if not found.</returns>
        public Material GetMaterialFromCache(string materialName)
        {
            if (_materialCache.ContainsKey(materialName))
            {
                return _materialCache[materialName];
            }
            return null;
        }
        
        /// <summary>
        /// Get texture from cache.
        /// </summary>
        /// <param name="textureName">Name of the texture to get.</param>
        /// <returns>Texture2D, or null if not found.</returns>
        public Texture2D GetTextureFromCache(string textureName)
        {
            if (_textureCache.ContainsKey(textureName))
            {
                return _textureCache[textureName];
            }
            return null;
        }
        
        /// <summary>
        /// Get all available room types.
        /// </summary>
        /// <returns>Array of available room types.</returns>
        public RoomType[] GetAvailableRoomTypes()
        {
            RoomType[] types = new RoomType[_modelSets.Count];
            _modelSets.Keys.CopyTo(types, 0);
            return types;
        }
        
        /// <summary>
        /// Check if models are loaded for a room type.
        /// </summary>
        /// <param name="roomType">Room type to check.</param>
        /// <returns>True if models are loaded.</returns>
        public bool AreModelsLoaded(RoomType roomType)
        {
            if (!_modelSets.ContainsKey(roomType)) return false;
            
            var modelSet = _modelSets[roomType];
            
            // Check if all model categories have models
            return modelSet.wallModels != null && modelSet.wallModels.Length > 0 &&
                   modelSet.floorModels != null && modelSet.floorModels.Length > 0 &&
                   modelSet.ceilingModels != null && modelSet.ceilingModels.Length > 0;
        }
        
        /// <summary>
        /// Get loading progress for a room type.
        /// </summary>
        /// <param name="roomType">Room type to get progress for.</param>
        /// <returns>Loading progress as percentage (0-100).</returns>
        public float GetLoadingProgress(RoomType roomType)
        {
            if (!_modelSets.ContainsKey(roomType)) return 0f;
            
            var modelSet = _modelSets[roomType];
            int totalModels = 0;
            int loadedModels = 0;
            
            // Count total models
            if (modelSet.wallModels != null) totalModels += modelSet.wallModels.Length;
            if (modelSet.floorModels != null) totalModels += modelSet.floorModels.Length;
            if (modelSet.ceilingModels != null) totalModels += modelSet.ceilingModels.Length;
            if (modelSet.decorationModels != null) totalModels += modelSet.decorationModels.Length;
            if (modelSet.furnitureModels != null) totalModels += modelSet.furnitureModels.Length;
            
            // Count loaded models
            if (modelSet.wallModels != null)
            {
                foreach (var model in modelSet.wallModels)
                {
                    if (_modelPool.ContainsKey(model.name)) loadedModels++;
                }
            }
            
            if (totalModels == 0) return 100f;
            return (float)loadedModels / totalModels * 100f;
        }
        
        protected override void OnDestroy()
        {
            // Unsubscribe from VR events
            if (_environmentSystem != null)
            {
                VREnvironmentSystem.OnRoomChanged -= OnRoomChanged;
            }
            
            base.OnDestroy();
        }
    }
    
    /// <summary>
    /// Room model set containing all models and textures for a room type.
    /// </summary>
    [System.Serializable]
    public class RoomModelSet
    {
        [Header("Room Info")]
        public RoomType roomType;
        public string roomName;
        public string description;
        
        [Header("Wall Models")]
        public ModelInfo[] wallModels;
        
        [Header("Floor Models")]
        public ModelInfo[] floorModels;
        
        [Header("Ceiling Models")]
        public ModelInfo[] ceilingModels;
        
        [Header("Decoration Models")]
        public ModelInfo[] decorationModels;
        
        [Header("Furniture Models")]
        public ModelInfo[] furnitureModels;
        
        [Header("Texture Sets")]
        public TextureSet[] textureSets;
    }
    
    /// <summary>
    /// Information about a 3D model.
    /// </summary>
    [System.Serializable]
    public class ModelInfo
    {
        [Header("Model Info")]
        public string name;
        public ModelCategory category;
        public ModelComplexity complexity;
        
        [Header("Model Settings")]
        public bool useLOD = true;
        public bool enableCollision = true;
        public bool enableShadows = true;
        
        [Header("Performance")]
        public int triangleCount = 1000;
        public int vertexCount = 500;
        public float boundingRadius = 1f;
    }
    
    /// <summary>
    /// Information about a texture set.
    /// </summary>
    [System.Serializable]
    public class TextureSet
    {
        [Header("Texture Info")]
        public string name;
        public TextureCategory category;
        
        [Header("Material Properties")]
        public Color baseColor = Color.white;
        public float roughness = 0.5f;
        public float metallic = 0.0f;
        public float normalStrength = 1.0f;
        
        [Header("Texture Settings")]
        public int resolution = 1024;
        public bool enableCompression = true;
        public bool generateMipMaps = true;
    }
    
    /// <summary>
    /// Model categories.
    /// </summary>
    public enum ModelCategory
    {
        Wall,
        Floor,
        Ceiling,
        Decoration,
        Furniture,
        Prop,
        Effect
    }
    
    /// <summary>
    /// Model complexity levels.
    /// </summary>
    public enum ModelComplexity
    {
        Simple,
        Medium,
        High,
        VeryHigh
    }
    
    /// <summary>
    /// Texture categories.
    /// </summary>
    public enum TextureCategory
    {
        Wall,
        Floor,
        Ceiling,
        Furniture,
        Prop,
        Effect,
        UI
    }
}
