using UnityEngine;
using Run4theRelic.Core;

namespace Run4theRelic.Relic
{
    /// <summary>
    /// Represents the extraction zone where players can win by bringing the Relic.
    /// Triggers victory condition when a player with the Relic enters.
    /// </summary>
    public class ExtractionZone : MonoBehaviour
    {
        [Header("Extraction Settings")]
        [SerializeField] private float extractionTime = 2f;
        [SerializeField] private LayerMask playerLayerMask = -1;
        [SerializeField] private bool requireRelic = true;
        
        [Header("Visual")]
        [SerializeField] private Renderer zoneRenderer;
        [SerializeField] private Material activeMaterial;
        [SerializeField] private Material inactiveMaterial;
        [SerializeField] private Color extractionColor = Color.green;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        private bool _isExtracting;
        private float _extractionTimer;
        private Transform _extractingPlayer;
        private RelicController _extractingRelic;
        
        /// <summary>
        /// Is extraction currently in progress.
        /// </summary>
        public bool IsExtracting => _isExtracting;
        
        /// <summary>
        /// The player currently extracting, or null if none.
        /// </summary>
        public Transform ExtractingPlayer => _extractingPlayer;
        
        /// <summary>
        /// The Relic being extracted, or null if none.
        /// </summary>
        public RelicController ExtractingRelic => _extractingRelic;
        
        /// <summary>
        /// Current extraction progress (0.0 to 1.0).
        /// </summary>
        public float ExtractionProgress => _isExtracting ? _extractionTimer / extractionTime : 0f;
        
        private void Start()
        {
            UpdateVisual();
        }
        
        private void Update()
        {
            if (_isExtracting)
            {
                HandleExtraction();
            }
        }
        
        /// <summary>
        /// Handle the extraction process.
        /// </summary>
        private void HandleExtraction()
        {
            if (_extractingPlayer == null || _extractingRelic == null)
            {
                // Player or Relic was lost during extraction
                CancelExtraction();
                return;
            }
            
            // Check if player still has the Relic
            if (!_extractingRelic.IsCarried || _extractingRelic.Carrier != _extractingPlayer)
            {
                CancelExtraction();
                return;
            }
            
            // Update extraction timer
            _extractionTimer += Time.deltaTime;
            
            if (showDebugInfo)
            {
                Debug.Log($"Extracting Relic: {ExtractionProgress:P0} complete");
            }
            
            // Check if extraction is complete
            if (_extractionTimer >= extractionTime)
            {
                CompleteExtraction();
            }
        }
        
        /// <summary>
        /// Start extraction process for a player.
        /// </summary>
        /// <param name="player">The player attempting extraction.</param>
        /// <returns>True if extraction can start, false otherwise.</returns>
        public bool StartExtraction(Transform player)
        {
            if (_isExtracting)
            {
                Debug.LogWarning("Extraction already in progress!");
                return false;
            }
            
            if (player == null)
            {
                Debug.LogWarning("Cannot start extraction: player is null");
                return false;
            }
            
            // Check if player has a Relic
            RelicController relic = FindRelicInPlayer(player);
            if (requireRelic && relic == null)
            {
                Debug.LogWarning($"Player {player.name} has no Relic to extract!");
                return false;
            }
            
            // Start extraction
            _isExtracting = true;
            _extractionTimer = 0f;
            _extractingPlayer = player;
            _extractingRelic = relic;
            
            UpdateVisual();
            
            if (showDebugInfo)
            {
                string relicInfo = relic != null ? $"with Relic" : "without Relic";
                Debug.Log($"Extraction started by {player.name} {relicInfo}");
            }
            
            return true;
        }
        
        /// <summary>
        /// Cancel the current extraction process.
        /// </summary>
        public void CancelExtraction()
        {
            if (!_isExtracting) return;
            
            if (showDebugInfo)
            {
                Debug.Log($"Extraction cancelled by {_extractingPlayer?.name ?? "unknown"}");
            }
            
            // Reset extraction state
            _isExtracting = false;
            _extractionTimer = 0f;
            _extractingPlayer = null;
            _extractingRelic = null;
            
            UpdateVisual();
        }
        
        /// <summary>
        /// Complete the extraction and trigger victory.
        /// </summary>
        private void CompleteExtraction()
        {
            if (!_isExtracting) return;
            
            if (showDebugInfo)
            {
                Debug.Log($"Extraction completed by {_extractingPlayer?.name ?? "unknown"}!");
            }
            
            // Trigger victory event
            GameEvents.TriggerRelicExtracted(-1); // -1 = singleplayer
            Debug.Log("WIN: Relic extracted! You win!");
            
            // Reset extraction state
            _isExtracting = false;
            _extractionTimer = 0f;
            _extractingPlayer = null;
            _extractingRelic = null;
            
            UpdateVisual();
        }
        
        /// <summary>
        /// Check if a player can extract (has Relic if required).
        /// </summary>
        /// <param name="player">The player to check.</param>
        /// <returns>True if player can extract, false otherwise.</returns>
        public bool CanExtract(Transform player)
        {
            if (player == null) return false;
            
            if (!requireRelic) return true;
            
            RelicController relic = FindRelicInPlayer(player);
            return relic != null && relic.IsCarried && relic.Carrier == player;
        }
        
        /// <summary>
        /// Find a Relic being carried by a player.
        /// </summary>
        /// <param name="player">The player to search.</param>
        /// <returns>The Relic being carried, or null if none.</returns>
        private RelicController FindRelicInPlayer(Transform player)
        {
            // Search for Relic in player's children or nearby
            RelicController[] relics = FindObjectsOfType<RelicController>();
            
            foreach (RelicController relic in relics)
            {
                if (relic.IsCarried && relic.Carrier == player)
                {
                    return relic;
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// Update the visual appearance of the extraction zone.
        /// </summary>
        private void UpdateVisual()
        {
            if (zoneRenderer == null) return;
            
            Material targetMaterial;
            
            if (_isExtracting)
            {
                targetMaterial = activeMaterial;
                if (targetMaterial != null)
                {
                    targetMaterial.color = extractionColor;
                }
            }
            else
            {
                targetMaterial = inactiveMaterial;
            }
            
            if (targetMaterial != null)
            {
                zoneRenderer.material = targetMaterial;
            }
        }
        
        // Trigger detection for player entry
        private void OnTriggerEnter(Collider other)
        {
            if (_isExtracting) return;
            
            // Check if this is a player
            if (((1 << other.gameObject.layer) & playerLayerMask) != 0)
            {
                Transform player = other.transform;
                
                // Check if player can extract
                if (CanExtract(player))
                {
                    StartExtraction(player);
                }
            }
        }
        
        // Trigger detection for player exit
        private void OnTriggerExit(Collider other)
        {
            if (!_isExtracting) return;
            
            // Check if this is the extracting player
            if (((1 << other.gameObject.layer) & playerLayerMask) != 0)
            {
                Transform player = other.transform;
                
                if (player == _extractingPlayer)
                {
                    CancelExtraction();
                }
            }
        }
        
        // Unity Editor support
        private void OnValidate()
        {
            // Ensure extraction time is positive
            extractionTime = Mathf.Max(0.1f, extractionTime);
        }
        
        // Gizmos for easier setup in editor
        private void OnDrawGizmos()
        {
            // Draw extraction zone
            Gizmos.color = _isExtracting ? Color.green : Color.yellow;
            Gizmos.DrawWireCube(transform.position, transform.localScale);
            
            // Draw extraction progress
            if (_isExtracting)
            {
                float progress = ExtractionProgress;
                Gizmos.color = Color.Lerp(Color.yellow, Color.green, progress);
                Gizmos.DrawWireSphere(transform.position, progress * 0.5f);
            }
        }
    }
} 