using UnityEngine;
using Run4theRelic.Puzzles;

namespace Run4theRelic.Puzzles.BalanceBridge
{
    /// <summary>
    /// Controls the balance bridge puzzle where player must maintain balance for a set duration.
    /// Puzzle is completed when testPoint.y stays above threshold for balanceTime seconds.
    /// </summary>
    public class BalanceBridgeController : PuzzleControllerBase
    {
        [Header("Balance Settings")]
        [SerializeField] private Transform testPoint;
        [SerializeField] private float threshold = 0.1f;
        [SerializeField] private float balanceTime = 3f;
        [SerializeField] private float checkInterval = 0.1f;
        
        [Header("Debug")]
        [SerializeField] private bool showBalanceDebug = true;
        [SerializeField] private bool drawGizmos = true;
        
        private float _balanceTimer;
        private bool _isBalanced;
        private float _lastCheckTime;
        
        protected override void OnFailed()
        {
            base.OnFailed();
            ResetPuzzle();
            // Optionally allow immediate retry by restarting the timer:
            // _timer = timeLimit; _isFailed = false; _isCompleted = false;
        }

        protected override void OnPuzzleStart()
        {
            // Reset balance state
            _balanceTimer = 0f;
            _isBalanced = false;
            _lastCheckTime = 0f;
            
            // Validate setup
            ValidateSetup();
            
            Debug.Log($"Balance Bridge puzzle started. Threshold: {threshold}, Required time: {balanceTime}s");
        }
        
        protected override void OnPuzzleComplete()
        {
            // Puzzle logic handled in base class
            Debug.Log("Balance Bridge puzzle completed!");
        }
        
        protected override void OnPuzzleFailed()
        {
            // Puzzle logic handled in base class
            Debug.Log("Balance Bridge puzzle failed - time expired!");
        }
        
        protected override void OnPuzzleReset()
        {
            ResetPuzzle();
        }
        
        private void Update()
        {
            if (!IsActive || IsCompleted || IsFailed) return;
            
            // Check balance at intervals
            if (Time.time - _lastCheckTime >= checkInterval)
            {
                CheckBalance();
                _lastCheckTime = Time.time;
            }
        }
        
        /// <summary>
        /// Check if the test point is within balance threshold.
        /// </summary>
        private void CheckBalance()
        {
            if (testPoint == null) return;
            
            // Check if Y position is above threshold
            bool currentlyBalanced = testPoint.position.y >= threshold;
            
            if (currentlyBalanced)
            {
                if (!_isBalanced)
                {
                    // Just started balancing
                    _isBalanced = true;
                    _balanceTimer = 0f;
                    
                    if (showBalanceDebug)
                    {
                        Debug.Log("Balance achieved! Timer started.");
                    }
                }
                
                // Increment balance timer
                _balanceTimer += checkInterval;
                
                if (showBalanceDebug)
                {
                    Debug.Log($"Balancing: {_balanceTimer:F1}s / {balanceTime}s");
                }
                
                // Check if balance time requirement is met
                if (_balanceTimer >= balanceTime)
                {
                    Complete();
                }
            }
            else
            {
                if (_isBalanced)
                {
                    // Lost balance - reset timer
                    _isBalanced = false;
                    _balanceTimer = 0f;
                    
                    if (showBalanceDebug)
                    {
                        Debug.Log("Balance lost! Timer reset.");
                    }
                }
            }
        }
        
        /// <summary>
        /// Validate that the puzzle setup is correct.
        /// </summary>
        private void ValidateSetup()
        {
            if (testPoint == null)
            {
                Debug.LogError("Balance Bridge: Test point is not assigned!");
            }
            
            if (threshold <= 0f)
            {
                Debug.LogWarning("Balance Bridge: Threshold should be positive!");
            }
            
            if (balanceTime <= 0f)
            {
                Debug.LogWarning("Balance Bridge: Balance time should be positive!");
            }
            
            if (checkInterval <= 0f)
            {
                Debug.LogWarning("Balance Bridge: Check interval should be positive!");
            }
        }
        
        /// <summary>
        /// Get current balance progress as a percentage.
        /// </summary>
        /// <returns>Balance progress from 0.0 to 1.0.</returns>
        public float GetBalanceProgress()
        {
            if (!_isBalanced) return 0f;
            return Mathf.Clamp01(_balanceTimer / balanceTime);
        }

        private void ResetPuzzle()
        {
            // Reset balance state
            _balanceTimer = 0f;
            _isBalanced = false;
            _lastCheckTime = 0f;
            // Notify legacy hook
            OnPuzzleReset();
        }
        
        /// <summary>
        /// Get current balance status.
        /// </summary>
        /// <returns>True if currently balanced, false otherwise.</returns>
        public bool IsCurrentlyBalanced()
        {
            return _isBalanced;
        }
        
        /// <summary>
        /// Get the current Y position of the test point.
        /// </summary>
        /// <returns>Y position, or 0 if testPoint is null.</returns>
        public float GetCurrentYPosition()
        {
            return testPoint != null ? testPoint.position.y : 0f;
        }
        
        // Unity Editor support
        private void OnValidate()
        {
            // Ensure values are positive
            threshold = Mathf.Max(0.01f, threshold);
            balanceTime = Mathf.Max(0.1f, balanceTime);
            checkInterval = Mathf.Max(0.01f, checkInterval);
            
            // Ensure check interval is not larger than balance time
            checkInterval = Mathf.Min(checkInterval, balanceTime);
        }
        
        // Gizmos for easier setup in editor
        private void OnDrawGizmos()
        {
            if (!drawGizmos) return;
            
            if (testPoint != null)
            {
                // Draw threshold line
                Gizmos.color = Color.red;
                Vector3 thresholdPos = testPoint.position;
                thresholdPos.y = threshold;
                Gizmos.DrawLine(thresholdPos + Vector3.left * 2f, thresholdPos + Vector3.right * 2f);
                
                // Draw current position
                Gizmos.color = _isBalanced ? Color.green : Color.yellow;
                Gizmos.DrawWireSphere(testPoint.position, 0.1f);
                
                // Draw threshold indicator
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(thresholdPos, Vector3.one * 0.2f);
                
                // Draw balance progress
                if (_isBalanced)
                {
                    float progress = GetBalanceProgress();
                    Gizmos.color = Color.Lerp(Color.yellow, Color.green, progress);
                    Gizmos.DrawWireSphere(testPoint.position, 0.2f + progress * 0.3f);
                }
            }
        }
        
        // Unity Editor support for easier setup
        private void OnDrawGizmosSelected()
        {
            if (testPoint == null) return;
            
            // Draw detailed balance information when selected
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
            
            // Draw connection line to test point
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, testPoint.position);
        }
    }
} 