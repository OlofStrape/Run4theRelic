using UnityEngine;
using Run4theRelic.Core;
using System.Collections.Generic;

namespace Run4theRelic.Core
{
    /// <summary>
    /// Generates 3D models programmatically for different room types in Run4theRelic.
    /// Creates walls, floors, ceilings, and decorations with procedurally generated geometry.
    /// </summary>
    public class VRModelGenerator : MonoBehaviour
    {
        [Header("Generation Settings")]
        [SerializeField] private bool enableProceduralGeneration = true;
        [SerializeField] private bool enableLODGeneration = true;
        [SerializeField] private bool enableCollisionGeneration = true;
        
        [Header("Geometry Settings")]
        [SerializeField] private int wallSegments = 8;
        [SerializeField] private int floorSegments = 16;
        [SerializeField] private float wallHeight = 3f;
        [SerializeField] private float roomRadius = 5f;
        
        [Header("Detail Settings")]
        [SerializeField] private bool enableDetailGeometry = true;
        [SerializeField] private bool enableNormalMapping = true;
        [SerializeField] private bool enableUVMapping = true;
        
        // VR References
        private VRModelTextureManager _modelTextureManager;
        private VREnvironmentSystem _environmentSystem;
        
        // Events
        public static event System.Action<string> OnModelGenerated;
        public static event System.Action<RoomType> OnRoomModelsGenerated;
        
        protected override void Start()
        {
            // Initialize model generator
            InitializeModelGenerator();
            
            // Subscribe to VR events
            SubscribeToVREvents();
        }
        
        /// <summary>
        /// Initialize the model generator.
        /// </summary>
        private void InitializeModelGenerator()
        {
            // Find VR components
            _modelTextureManager = FindObjectOfType<VRModelTextureManager>();
            _environmentSystem = FindObjectOfType<VREnvironmentSystem>();
            
            if (_modelTextureManager == null)
            {
                Debug.LogWarning("VRModelGenerator: No VRModelTextureManager found!");
            }
            
            if (_environmentSystem == null)
            {
                Debug.LogWarning("VRModelGenerator: No VREnvironmentSystem found!");
            }
            
            Debug.Log("VRModelGenerator: Initialized successfully");
        }
        
        /// <summary>
        /// Subscribe to VR system events.
        /// </summary>
        private void SubscribeToVREvents()
        {
            if (_modelTextureManager != null)
            {
                VRModelTextureManager.OnModelsLoaded += OnModelsLoaded;
            }
        }
        
        /// <summary>
        /// Handle models loaded event.
        /// </summary>
        /// <param name="roomType">Room type that had models loaded.</param>
        private void OnModelsLoaded(RoomType roomType)
        {
            if (enableProceduralGeneration)
            {
                // Generate procedural models for this room type
                GenerateRoomModels(roomType);
            }
        }
        
        /// <summary>
        /// Generate all models for a specific room type.
        /// </summary>
        /// <param name="roomType">Room type to generate models for.</param>
        public void GenerateRoomModels(RoomType roomType)
        {
            if (_modelTextureManager == null) return;
            
            var modelSet = _modelTextureManager.GetModelSet(roomType);
            if (modelSet == null) return;
            
            Debug.Log($"VRModelGenerator: Generating models for {modelSet.roomName}");
            
            // Generate wall models
            GenerateWallModels(modelSet);
            
            // Generate floor models
            GenerateFloorModels(modelSet);
            
            // Generate ceiling models
            GenerateCeilingModels(modelSet);
            
            // Generate decoration models
            GenerateDecorationModels(modelSet);
            
            // Generate furniture models
            GenerateFurnitureModels(modelSet);
            
            // Notify room models generated
            OnRoomModelsGenerated?.Invoke(roomType);
            
            Debug.Log($"VRModelGenerator: Generated all models for {modelSet.roomName}");
        }
        
        /// <summary>
        /// Generate wall models for a room.
        /// </summary>
        /// <param name="modelSet">Room model set to generate walls for.</param>
        private void GenerateWallModels(RoomModelSet modelSet)
        {
            if (modelSet.wallModels == null) return;
            
            foreach (var wallModel in modelSet.wallModels)
            {
                GameObject wallObject = GenerateWallModel(wallModel, modelSet.roomType);
                
                // Apply materials
                ApplyMaterialsToModel(wallObject, wallModel, modelSet);
                
                // Notify model generated
                OnModelGenerated?.Invoke(wallModel.name);
            }
        }
        
        /// <summary>
        /// Generate a single wall model.
        /// </summary>
        /// <param name="wallModel">Wall model info.</param>
        /// <param name="roomType">Room type.</param>
        /// <returns>Generated wall GameObject.</returns>
        private GameObject GenerateWallModel(ModelInfo wallModel, RoomType roomType)
        {
            GameObject wallObject = new GameObject(wallModel.name);
            
            // Add components
            var meshFilter = wallObject.AddComponent<MeshFilter>();
            var meshRenderer = wallObject.AddComponent<MeshRenderer>();
            
            if (enableCollisionGeneration)
            {
                wallObject.AddComponent<MeshCollider>();
            }
            
            // Generate mesh based on room type
            Mesh wallMesh = GenerateWallMesh(roomType, wallModel.complexity);
            meshFilter.mesh = wallMesh;
            
            // Set position and rotation
            SetWallTransform(wallObject, wallModel, roomType);
            
            return wallObject;
        }
        
        /// <summary>
        /// Generate wall mesh based on room type and complexity.
        /// </summary>
        /// <param name="roomType">Room type.</param>
        /// <param name="complexity">Model complexity.</param>
        /// <returns>Generated wall mesh.</returns>
        private Mesh GenerateWallMesh(RoomType roomType, ModelComplexity complexity)
        {
            Mesh mesh = new Mesh();
            
            switch (roomType)
            {
                case RoomType.Entrance:
                    mesh = GenerateSimpleWallMesh(complexity);
                    break;
                    
                case RoomType.Corridor:
                    mesh = GenerateLibraryWallMesh(complexity);
                    break;
                    
                case RoomType.PuzzleRoom:
                    mesh = GenerateCavernWallMesh(complexity);
                    break;
                    
                case RoomType.RelicChamber:
                    mesh = GenerateSanctumWallMesh(complexity);
                    break;
                    
                case RoomType.BossRoom:
                    mesh = GenerateNexusWallMesh(complexity);
                    break;
                    
                case RoomType.Exit:
                    mesh = GenerateAbyssWallMesh(complexity);
                    break;
                    
                default:
                    mesh = GenerateSimpleWallMesh(complexity);
                    break;
            }
            
            return mesh;
        }
        
        /// <summary>
        /// Generate simple wall mesh.
        /// </summary>
        /// <param name="complexity">Model complexity.</param>
        /// <returns>Simple wall mesh.</returns>
        private Mesh GenerateSimpleWallMesh(ModelComplexity complexity)
        {
            Mesh mesh = new Mesh();
            
            // Basic wall geometry
            Vector3[] vertices = new Vector3[]
            {
                new Vector3(-1f, 0f, 0f),    // Bottom left
                new Vector3(1f, 0f, 0f),     // Bottom right
                new Vector3(1f, wallHeight, 0f),  // Top right
                new Vector3(-1f, wallHeight, 0f)  // Top left
            };
            
            int[] triangles = new int[]
            {
                0, 2, 1,  // Front face
                0, 3, 2   // Back face
            };
            
            Vector2[] uvs = new Vector2[]
            {
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(1f, 1f),
                new Vector2(0f, 1f)
            };
            
            // Add complexity based on complexity level
            if (complexity >= ModelComplexity.Medium)
            {
                // Add more segments
                vertices = GenerateSegmentedWallVertices(complexity);
                triangles = GenerateSegmentedWallTriangles(complexity);
                uvs = GenerateSegmentedWallUVs(complexity);
            }
            
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            
            return mesh;
        }
        
        /// <summary>
        /// Generate library wall mesh.
        /// </summary>
        /// <param name="complexity">Model complexity.</param>
        /// <returns>Library wall mesh.</returns>
        private Mesh GenerateLibraryWallMesh(ModelComplexity complexity)
        {
            Mesh mesh = new Mesh();
            
            // Library walls have more architectural details
            Vector3[] vertices = new Vector3[]
            {
                // Base wall
                new Vector3(-1f, 0f, 0f),    // Bottom left
                new Vector3(1f, 0f, 0f),     // Bottom right
                new Vector3(1f, wallHeight * 0.8f, 0f),  // Middle right
                new Vector3(-1f, wallHeight * 0.8f, 0f), // Middle left
                
                // Arch top
                new Vector3(-0.8f, wallHeight * 0.8f, 0f),  // Arch left
                new Vector3(0.8f, wallHeight * 0.8f, 0f),   // Arch right
                new Vector3(0f, wallHeight, 0f)              // Arch top
            };
            
            int[] triangles = new int[]
            {
                // Base wall
                0, 2, 1,
                0, 3, 2,
                
                // Arch
                4, 6, 5,
                4, 5, 6
            };
            
            Vector2[] uvs = new Vector2[]
            {
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(1f, 0.8f),
                new Vector2(0f, 0.8f),
                new Vector2(0.1f, 0.8f),
                new Vector2(0.9f, 0.8f),
                new Vector2(0.5f, 1f)
            };
            
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            
            return mesh;
        }
        
        /// <summary>
        /// Generate cavern wall mesh.
        /// </summary>
        /// <param name="complexity">Model complexity.</param>
        /// <returns>Cavern wall mesh.</returns>
        private Mesh GenerateCavernWallMesh(ModelComplexity complexity)
        {
            Mesh mesh = new Mesh();
            
            // Cavern walls have organic, irregular shapes
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uvs = new List<Vector2>();
            
            // Generate irregular wall surface
            int segments = complexity >= ModelComplexity.High ? 12 : 8;
            float segmentWidth = 2f / segments;
            
            for (int i = 0; i <= segments; i++)
            {
                float x = -1f + i * segmentWidth;
                float heightVariation = Mathf.Sin(i * 0.5f) * 0.2f;
                
                // Add vertices for this segment
                vertices.Add(new Vector3(x, 0f, 0f));
                vertices.Add(new Vector3(x, wallHeight + heightVariation, 0f));
                
                // Add UVs
                uvs.Add(new Vector2((float)i / segments, 0f));
                uvs.Add(new Vector2((float)i / segments, 1f));
            }
            
            // Generate triangles
            for (int i = 0; i < segments; i++)
            {
                int baseIndex = i * 2;
                
                // First triangle
                triangles.Add(baseIndex);
                triangles.Add(baseIndex + 3);
                triangles.Add(baseIndex + 1);
                
                // Second triangle
                triangles.Add(baseIndex);
                triangles.Add(baseIndex + 2);
                triangles.Add(baseIndex + 3);
            }
            
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();
            
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            
            return mesh;
        }
        
        /// <summary>
        /// Generate sanctum wall mesh.
        /// </summary>
        /// <param name="complexity">Model complexity.</param>
        /// <returns>Sanctum wall mesh.</returns>
        private Mesh GenerateSanctumWallMesh(ModelComplexity complexity)
        {
            Mesh mesh = new Mesh();
            
            // Sanctum walls have dark, foreboding geometry
            Vector3[] vertices = new Vector3[]
            {
                // Base wall with shadow effects
                new Vector3(-1f, 0f, 0f),    // Bottom left
                new Vector3(1f, 0f, 0f),     // Bottom right
                new Vector3(1f, wallHeight, 0f),  // Top right
                new Vector3(-1f, wallHeight, 0f), // Top left
                
                // Shadow overlay
                new Vector3(-0.9f, 0.1f, 0.01f),  // Shadow left
                new Vector3(0.9f, 0.1f, 0.01f),   // Shadow right
                new Vector3(0.9f, wallHeight - 0.1f, 0.01f),  // Shadow top right
                new Vector3(-0.9f, wallHeight - 0.1f, 0.01f)  // Shadow top left
            };
            
            int[] triangles = new int[]
            {
                // Base wall
                0, 2, 1,
                0, 3, 2,
                
                // Shadow overlay
                4, 6, 5,
                4, 7, 6
            };
            
            Vector2[] uvs = new Vector2[]
            {
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(1f, 1f),
                new Vector2(0f, 1f),
                new Vector2(0.05f, 0.05f),
                new Vector2(0.95f, 0.05f),
                new Vector2(0.95f, 0.95f),
                new Vector2(0.05f, 0.95f)
            };
            
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            
            return mesh;
        }
        
        /// <summary>
        /// Generate nexus wall mesh.
        /// </summary>
        /// <param name="complexity">Model complexity.</param>
        /// <returns>Nexus wall mesh.</returns>
        private Mesh GenerateNexusWallMesh(ModelComplexity complexity)
        {
            Mesh mesh = new Mesh();
            
            // Nexus walls have energy field effects
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uvs = new List<Vector2>();
            
            // Generate energy field pattern
            int segments = complexity >= ModelComplexity.VeryHigh ? 16 : 12;
            float segmentWidth = 2f / segments;
            
            for (int i = 0; i <= segments; i++)
            {
                float x = -1f + i * segmentWidth;
                float energyVariation = Mathf.Sin(i * 0.8f) * 0.3f;
                
                // Add vertices for energy field
                vertices.Add(new Vector3(x, 0f, 0f));
                vertices.Add(new Vector3(x, wallHeight + energyVariation, 0f));
                vertices.Add(new Vector3(x, 0f, 0.1f));  // Energy depth
                vertices.Add(new Vector3(x, wallHeight + energyVariation, 0.1f));
                
                // Add UVs
                uvs.Add(new Vector2((float)i / segments, 0f));
                uvs.Add(new Vector2((float)i / segments, 1f));
                uvs.Add(new Vector2((float)i / segments, 0f));
                uvs.Add(new Vector2((float)i / segments, 1f));
            }
            
            // Generate triangles for energy field
            for (int i = 0; i < segments; i++)
            {
                int baseIndex = i * 4;
                
                // Front face
                triangles.Add(baseIndex);
                triangles.Add(baseIndex + 5);
                triangles.Add(baseIndex + 1);
                triangles.Add(baseIndex);
                triangles.Add(baseIndex + 4);
                triangles.Add(baseIndex + 5);
                
                // Side faces
                triangles.Add(baseIndex);
                triangles.Add(baseIndex + 1);
                triangles.Add(baseIndex + 2);
                triangles.Add(baseIndex + 1);
                triangles.Add(baseIndex + 3);
                triangles.Add(baseIndex + 2);
            }
            
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();
            
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            
            return mesh;
        }
        
        /// <summary>
        /// Generate abyss wall mesh.
        /// </summary>
        /// <param name="complexity">Model complexity.</param>
        /// <returns>Abyss wall mesh.</returns>
        private Mesh GenerateAbyssWallMesh(ModelComplexity complexity)
        {
            Mesh mesh = new Mesh();
            
            // Abyss walls have apocalyptic, chaotic geometry
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uvs = new List<Vector2>();
            
            // Generate chaotic wall surface
            int segments = complexity >= ModelComplexity.VeryHigh ? 20 : 16;
            float segmentWidth = 2f / segments;
            
            for (int i = 0; i <= segments; i++)
            {
                float x = -1f + i * segmentWidth;
                float chaosVariation = Mathf.PerlinNoise(i * 0.3f, 0f) * 0.5f;
                float heightVariation = Mathf.Sin(i * 1.2f) * 0.4f;
                
                // Add vertices for chaotic surface
                vertices.Add(new Vector3(x, 0f, 0f));
                vertices.Add(new Vector3(x, wallHeight + heightVariation + chaosVariation, 0f));
                vertices.Add(new Vector3(x + chaosVariation * 0.1f, 0f, chaosVariation * 0.2f));
                vertices.Add(new Vector3(x + chaosVariation * 0.1f, wallHeight + heightVariation + chaosVariation, chaosVariation * 0.2f));
                
                // Add UVs
                uvs.Add(new Vector2((float)i / segments, 0f));
                uvs.Add(new Vector2((float)i / segments, 1f));
                uvs.Add(new Vector2((float)i / segments, 0f));
                uvs.Add(new Vector2((float)i / segments, 1f));
            }
            
            // Generate triangles for chaotic surface
            for (int i = 0; i < segments; i++)
            {
                int baseIndex = i * 4;
                
                // Main surface
                triangles.Add(baseIndex);
                triangles.Add(baseIndex + 5);
                triangles.Add(baseIndex + 1);
                triangles.Add(baseIndex);
                triangles.Add(baseIndex + 4);
                triangles.Add(baseIndex + 5);
                
                // Chaos variations
                triangles.Add(baseIndex);
                triangles.Add(baseIndex + 1);
                triangles.Add(baseIndex + 2);
                triangles.Add(baseIndex + 1);
                triangles.Add(baseIndex + 3);
                triangles.Add(baseIndex + 2);
            }
            
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();
            
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            
            return mesh;
        }
        
        /// <summary>
        /// Generate segmented wall vertices.
        /// </summary>
        /// <param name="complexity">Model complexity.</param>
        /// <returns>Array of segmented wall vertices.</returns>
        private Vector3[] GenerateSegmentedWallVertices(ModelComplexity complexity)
        {
            int segments = GetSegmentCount(complexity);
            List<Vector3> vertices = new List<Vector3>();
            
            float segmentWidth = 2f / segments;
            
            for (int i = 0; i <= segments; i++)
            {
                float x = -1f + i * segmentWidth;
                vertices.Add(new Vector3(x, 0f, 0f));
                vertices.Add(new Vector3(x, wallHeight, 0f));
            }
            
            return vertices.ToArray();
        }
        
        /// <summary>
        /// Generate segmented wall triangles.
        /// </summary>
        /// <param name="complexity">Model complexity.</param>
        /// <returns>Array of segmented wall triangles.</returns>
        private int[] GenerateSegmentedWallTriangles(ModelComplexity complexity)
        {
            int segments = GetSegmentCount(complexity);
            List<int> triangles = new List<int>();
            
            for (int i = 0; i < segments; i++)
            {
                int baseIndex = i * 2;
                
                // First triangle
                triangles.Add(baseIndex);
                triangles.Add(baseIndex + 3);
                triangles.Add(baseIndex + 1);
                
                // Second triangle
                triangles.Add(baseIndex);
                triangles.Add(baseIndex + 2);
                triangles.Add(baseIndex + 3);
            }
            
            return triangles.ToArray();
        }
        
        /// <summary>
        /// Generate segmented wall UVs.
        /// </summary>
        /// <param name="complexity">Model complexity.</param>
        /// <returns>Array of segmented wall UVs.</returns>
        private Vector2[] GenerateSegmentedWallUVs(ModelComplexity complexity)
        {
            int segments = GetSegmentCount(complexity);
            List<Vector2> uvs = new List<Vector2>();
            
            for (int i = 0; i <= segments; i++)
            {
                float u = (float)i / segments;
                uvs.Add(new Vector2(u, 0f));
                uvs.Add(new Vector2(u, 1f));
            }
            
            return uvs.ToArray();
        }
        
        /// <summary>
        /// Get segment count for complexity level.
        /// </summary>
        /// <param name="complexity">Model complexity.</param>
        /// <returns>Number of segments.</returns>
        private int GetSegmentCount(ModelComplexity complexity)
        {
            switch (complexity)
            {
                case ModelComplexity.Simple: return 4;
                case ModelComplexity.Medium: return 8;
                case ModelComplexity.High: return 12;
                case ModelComplexity.VeryHigh: return 16;
                default: return 8;
            }
        }
        
        /// <summary>
        /// Set wall transform based on model info and room type.
        /// </summary>
        /// <param name="wallObject">Wall GameObject.</param>
        /// <param name="wallModel">Wall model info.</param>
        /// <param name="roomType">Room type.</param>
        private void SetWallTransform(GameObject wallObject, ModelInfo wallModel, RoomType roomType)
        {
            // Position walls around the room
            float angle = Random.Range(0f, 360f);
            float radius = roomRadius;
            
            Vector3 position = new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
                0f,
                Mathf.Sin(angle * Mathf.Deg2Rad) * radius
            );
            
            wallObject.transform.position = position;
            wallObject.transform.rotation = Quaternion.Euler(0f, angle + 90f, 0f);
            
            // Scale based on complexity
            float scale = GetScaleForComplexity(wallModel.complexity);
            wallObject.transform.localScale = Vector3.one * scale;
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
        /// Generate floor models for a room.
        /// </summary>
        /// <param name="modelSet">Room model set to generate floors for.</param>
        private void GenerateFloorModels(RoomModelSet modelSet)
        {
            if (modelSet.floorModels == null) return;
            
            foreach (var floorModel in modelSet.floorModels)
            {
                GameObject floorObject = GenerateFloorModel(floorModel, modelSet.roomType);
                
                // Apply materials
                ApplyMaterialsToModel(floorObject, floorModel, modelSet);
                
                // Notify model generated
                OnModelGenerated?.Invoke(floorModel.name);
            }
        }
        
        /// <summary>
        /// Generate a single floor model.
        /// </summary>
        /// <param name="floorModel">Floor model info.</param>
        /// <param name="roomType">Room type.</param>
        /// <returns>Generated floor GameObject.</returns>
        private GameObject GenerateFloorModel(ModelInfo floorModel, RoomType roomType)
        {
            GameObject floorObject = new GameObject(floorModel.name);
            
            // Add components
            var meshFilter = floorObject.AddComponent<MeshFilter>();
            var meshRenderer = floorObject.AddComponent<MeshRenderer>();
            
            if (enableCollisionGeneration)
            {
                floorObject.AddComponent<MeshCollider>();
            }
            
            // Generate floor mesh
            Mesh floorMesh = GenerateFloorMesh(roomType, floorModel.complexity);
            meshFilter.mesh = floorMesh;
            
            // Set position
            floorObject.transform.position = Vector3.zero;
            floorObject.transform.localScale = Vector3.one * roomRadius;
            
            return floorObject;
        }
        
        /// <summary>
        /// Generate floor mesh based on room type and complexity.
        /// </summary>
        /// <param name="roomType">Room type.</param>
        /// <param name="complexity">Model complexity.</param>
        /// <returns>Generated floor mesh.</returns>
        private Mesh GenerateFloorMesh(RoomType roomType, ModelComplexity complexity)
        {
            Mesh mesh = new Mesh();
            
            // Generate circular floor
            int segments = GetSegmentCount(complexity);
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uvs = new List<Vector2>();
            
            // Center vertex
            vertices.Add(Vector3.zero);
            uvs.Add(new Vector2(0.5f, 0.5f));
            
            // Perimeter vertices
            for (int i = 0; i <= segments; i++)
            {
                float angle = (float)i / segments * 2f * Mathf.PI;
                float x = Mathf.Cos(angle);
                float z = Mathf.Sin(angle);
                
                vertices.Add(new Vector3(x, 0f, z));
                uvs.Add(new Vector2((x + 1f) * 0.5f, (z + 1f) * 0.5f));
            }
            
            // Generate triangles
            for (int i = 0; i < segments; i++)
            {
                triangles.Add(0);  // Center
                triangles.Add(i + 2);
                triangles.Add(i + 1);
            }
            
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();
            
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            
            return mesh;
        }
        
        /// <summary>
        /// Generate ceiling models for a room.
        /// </summary>
        /// <param name="modelSet">Room model set to generate ceilings for.</param>
        private void GenerateCeilingModels(RoomModelSet modelSet)
        {
            if (modelSet.ceilingModels == null) return;
            
            foreach (var ceilingModel in modelSet.ceilingModels)
            {
                GameObject ceilingObject = GenerateCeilingModel(ceilingModel, modelSet.roomType);
                
                // Apply materials
                ApplyMaterialsToModel(ceilingObject, ceilingModel, modelSet);
                
                // Notify model generated
                OnModelGenerated?.Invoke(ceilingModel.name);
            }
        }
        
        /// <summary>
        /// Generate a single ceiling model.
        /// </summary>
        /// <param name="ceilingModel">Ceiling model info.</param>
        /// <param name="roomType">Room type.</param>
        /// <returns>Generated ceiling GameObject.</returns>
        private GameObject GenerateCeilingModel(ModelInfo ceilingModel, RoomType roomType)
        {
            GameObject ceilingObject = new GameObject(ceilingModel.name);
            
            // Add components
            var meshFilter = ceilingObject.AddComponent<MeshFilter>();
            var meshRenderer = ceilingObject.AddComponent<MeshRenderer>();
            
            if (enableCollisionGeneration)
            {
                ceilingObject.AddComponent<MeshCollider>();
            }
            
            // Generate ceiling mesh
            Mesh ceilingMesh = GenerateCeilingMesh(roomType, ceilingModel.complexity);
            meshFilter.mesh = ceilingMesh;
            
            // Set position
            ceilingObject.transform.position = new Vector3(0f, wallHeight, 0f);
            ceilingObject.transform.localScale = Vector3.one * roomRadius;
            
            return ceilingObject;
        }
        
        /// <summary>
        /// Generate ceiling mesh based on room type and complexity.
        /// </summary>
        /// <param name="roomType">Room type.</param>
        /// <param name="complexity">Model complexity.</param>
        /// <returns>Generated ceiling mesh.</returns>
        private Mesh GenerateCeilingMesh(RoomType roomType, ModelComplexity complexity)
        {
            // Similar to floor mesh but inverted
            Mesh mesh = GenerateFloorMesh(roomType, complexity);
            
            // Invert normals for ceiling
            Vector3[] normals = mesh.normals;
            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = -normals[i];
            }
            mesh.normals = normals;
            
            return mesh;
        }
        
        /// <summary>
        /// Generate decoration models for a room.
        /// </summary>
        /// <param name="modelSet">Room model set to generate decorations for.</param>
        private void GenerateDecorationModels(RoomModelSet modelSet)
        {
            if (modelSet.decorationModels == null) return;
            
            foreach (var decorationModel in modelSet.decorationModels)
            {
                GameObject decorationObject = GenerateDecorationModel(decorationModel, modelSet.roomType);
                
                // Apply materials
                ApplyMaterialsToModel(decorationObject, decorationModel, modelSet);
                
                // Notify model generated
                OnModelGenerated?.Invoke(decorationModel.name);
            }
        }
        
        /// <summary>
        /// Generate a single decoration model.
        /// </summary>
        /// <param name="decorationModel">Decoration model info.</param>
        /// <param name="roomType">Room type.</param>
        /// <returns>Generated decoration GameObject.</returns>
        private GameObject GenerateDecorationModel(ModelInfo decorationModel, RoomType roomType)
        {
            GameObject decorationObject = new GameObject(decorationModel.name);
            
            // Add components
            var meshFilter = decorationObject.AddComponent<MeshFilter>();
            var meshRenderer = decorationObject.AddComponent<MeshRenderer>();
            
            if (enableCollisionGeneration)
            {
                decorationObject.AddComponent<SphereCollider>();
            }
            
            // Generate decoration mesh
            Mesh decorationMesh = GenerateDecorationMesh(roomType, decorationModel.complexity);
            meshFilter.mesh = decorationMesh;
            
            // Set random position in room
            Vector3 randomPosition = new Vector3(
                Random.Range(-roomRadius * 0.8f, roomRadius * 0.8f),
                Random.Range(0.5f, wallHeight - 0.5f),
                Random.Range(-roomRadius * 0.8f, roomRadius * 0.8f)
            );
            
            decorationObject.transform.position = randomPosition;
            decorationObject.transform.localScale = Vector3.one * Random.Range(0.5f, 1.5f);
            
            return decorationObject;
        }
        
        /// <summary>
        /// Generate decoration mesh based on room type and complexity.
        /// </summary>
        /// <param name="roomType">Room type.</param>
        /// <param name="complexity">Model complexity.</param>
        /// <returns>Generated decoration mesh.</returns>
        private Mesh GenerateDecorationMesh(RoomType roomType, ModelComplexity complexity)
        {
            // Simple sphere mesh for decorations
            Mesh mesh = new Mesh();
            
            int segments = GetSegmentCount(complexity);
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uvs = new List<Vector2>();
            
            // Generate sphere vertices
            for (int lat = 0; lat <= segments; lat++)
            {
                float theta = lat * Mathf.PI / segments;
                float sinTheta = Mathf.Sin(theta);
                float cosTheta = Mathf.Cos(theta);
                
                for (int lon = 0; lon <= segments; lon++)
                {
                    float phi = lon * 2f * Mathf.PI / segments;
                    float sinPhi = Mathf.Sin(phi);
                    float cosPhi = Mathf.Cos(phi);
                    
                    float x = cosPhi * sinTheta;
                    float y = cosTheta;
                    float z = sinPhi * sinTheta;
                    
                    vertices.Add(new Vector3(x, y, z));
                    uvs.Add(new Vector2((float)lon / segments, (float)lat / segments));
                }
            }
            
            // Generate triangles
            for (int lat = 0; lat < segments; lat++)
            {
                for (int lon = 0; lon < segments; lon++)
                {
                    int current = lat * (segments + 1) + lon;
                    int next = current + segments + 1;
                    
                    triangles.Add(current);
                    triangles.Add(next);
                    triangles.Add(current + 1);
                    
                    triangles.Add(current + 1);
                    triangles.Add(next);
                    triangles.Add(next + 1);
                }
            }
            
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();
            
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            
            return mesh;
        }
        
        /// <summary>
        /// Generate furniture models for a room.
        /// </summary>
        /// <param name="modelSet">Room model set to generate furniture for.</param>
        private void GenerateFurnitureModels(RoomModelSet modelSet)
        {
            if (modelSet.furnitureModels == null) return;
            
            foreach (var furnitureModel in modelSet.furnitureModels)
            {
                GameObject furnitureObject = GenerateFurnitureModel(furnitureModel, modelSet.roomType);
                
                // Apply materials
                ApplyMaterialsToModel(furnitureObject, furnitureModel, modelSet);
                
                // Notify model generated
                OnModelGenerated?.Invoke(furnitureModel.name);
            }
        }
        
        /// <summary>
        /// Generate a single furniture model.
        /// </summary>
        /// <param name="furnitureModel">Furniture model info.</param>
        /// <param name="roomType">Room type.</param>
        /// <returns>Generated furniture GameObject.</returns>
        private GameObject GenerateFurnitureModel(ModelInfo furnitureModel, RoomType roomType)
        {
            GameObject furnitureObject = new GameObject(furnitureModel.name);
            
            // Add components
            var meshFilter = furnitureObject.AddComponent<MeshFilter>();
            var meshRenderer = furnitureObject.AddComponent<MeshRenderer>();
            
            if (enableCollisionGeneration)
            {
                furnitureObject.AddComponent<BoxCollider>();
            }
            
            // Generate furniture mesh
            Mesh furnitureMesh = GenerateFurnitureMesh(roomType, furnitureModel.complexity);
            meshFilter.mesh = furnitureMesh;
            
            // Set position
            Vector3 position = new Vector3(
                Random.Range(-roomRadius * 0.6f, roomRadius * 0.6f),
                0f,
                Random.Range(-roomRadius * 0.6f, roomRadius * 0.6f)
            );
            
            furnitureObject.transform.position = position;
            furnitureObject.transform.localScale = Vector3.one * Random.Range(0.8f, 1.2f);
            
            return furnitureObject;
        }
        
        /// <summary>
        /// Generate furniture mesh based on room type and complexity.
        /// </summary>
        /// <param name="roomType">Room type.</param>
        /// <param name="complexity">Model complexity.</param>
        /// <returns>Generated furniture mesh.</returns>
        private Mesh GenerateFurnitureMesh(RoomType roomType, ModelComplexity complexity)
        {
            // Simple box mesh for furniture
            Mesh mesh = new Mesh();
            
            Vector3[] vertices = new Vector3[]
            {
                // Front face
                new Vector3(-0.5f, 0f, -0.5f),
                new Vector3(0.5f, 0f, -0.5f),
                new Vector3(0.5f, 1f, -0.5f),
                new Vector3(-0.5f, 1f, -0.5f),
                
                // Back face
                new Vector3(-0.5f, 0f, 0.5f),
                new Vector3(0.5f, 0f, 0.5f),
                new Vector3(0.5f, 1f, 0.5f),
                new Vector3(-0.5f, 1f, 0.5f)
            };
            
            int[] triangles = new int[]
            {
                // Front
                0, 2, 1, 0, 3, 2,
                // Back
                5, 7, 4, 5, 6, 7,
                // Left
                4, 7, 0, 0, 7, 3,
                // Right
                1, 6, 5, 1, 2, 6,
                // Top
                3, 6, 2, 3, 7, 6,
                // Bottom
                4, 1, 5, 4, 0, 1
            };
            
            Vector2[] uvs = new Vector2[]
            {
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(1f, 1f),
                new Vector2(0f, 1f),
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(1f, 1f),
                new Vector2(0f, 1f)
            };
            
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            
            return mesh;
        }
        
        /// <summary>
        /// Apply materials to a generated model.
        /// </summary>
        /// <param name="modelObject">Model GameObject.</param>
        /// <param name="modelInfo">Model info.</param>
        /// <param name="modelSet">Room model set.</param>
        private void ApplyMaterialsToModel(GameObject modelObject, ModelInfo modelInfo, RoomModelSet modelSet)
        {
            if (_modelTextureManager == null) return;
            
            var meshRenderer = modelObject.GetComponent<MeshRenderer>();
            if (meshRenderer == null) return;
            
            // Try to get material from cache
            Material material = _modelTextureManager.GetMaterialFromCache(modelInfo.name);
            
            if (material == null && modelSet.textureSets != null && modelSet.textureSets.Length > 0)
            {
                // Use first available texture set
                var textureSet = modelSet.textureSets[0];
                material = _modelTextureManager.GetMaterialFromCache(textureSet.name);
            }
            
            if (material != null)
            {
                meshRenderer.material = material;
            }
            else
            {
                // Create default material
                Material defaultMaterial = new Material(Shader.Find("Standard"));
                defaultMaterial.color = Color.gray;
                meshRenderer.material = defaultMaterial;
            }
        }
        
        /// <summary>
        /// Generate models for all room types.
        /// </summary>
        public void GenerateAllRoomModels()
        {
            if (_modelTextureManager == null) return;
            
            var roomTypes = _modelTextureManager.GetAvailableRoomTypes();
            
            foreach (var roomType in roomTypes)
            {
                GenerateRoomModels(roomType);
            }
        }
        
        /// <summary>
        /// Get generation progress for a room type.
        /// </summary>
        /// <param name="roomType">Room type to get progress for.</param>
        /// <returns>Generation progress as percentage (0-100).</returns>
        public float GetGenerationProgress(RoomType roomType)
        {
            if (_modelTextureManager == null) return 0f;
            
            var modelSet = _modelTextureManager.GetModelSet(roomType);
            if (modelSet == null) return 0f;
            
            int totalModels = 0;
            if (modelSet.wallModels != null) totalModels += modelSet.wallModels.Length;
            if (modelSet.floorModels != null) totalModels += modelSet.floorModels.Length;
            if (modelSet.ceilingModels != null) totalModels += modelSet.ceilingModels.Length;
            if (modelSet.decorationModels != null) totalModels += modelSet.decorationModels.Length;
            if (modelSet.furnitureModels != null) totalModels += modelSet.furnitureModels.Length;
            
            // This would track actual generation progress
            // For now, return a placeholder value
            return totalModels > 0 ? 75f : 0f;
        }
        
        protected override void OnDestroy()
        {
            // Unsubscribe from VR events
            if (_modelTextureManager != null)
            {
                VRModelTextureManager.OnModelsLoaded -= OnModelsLoaded;
            }
            
            base.OnDestroy();
        }
    }
}
