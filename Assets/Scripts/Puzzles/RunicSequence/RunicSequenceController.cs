using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Run4theRelic.Puzzles;

namespace Run4theRelic.Puzzles.RunicSequence
{
    /// <summary>
    /// Controls the runic sequence puzzle (Simon says style).
    /// Generates random sequences and validates player input.
    /// </summary>
    public class RunicSequenceController : PuzzleControllerBase
    {
        [Header("Sequence Settings")]
        [SerializeField] private List<RunicPad> pads = new List<RunicPad>();
        [SerializeField] private int sequenceLength = 4;
        [SerializeField] private float stepDelay = 1f;
        [SerializeField] private float inputTimeout = 5f;
        
        [Header("Debug")]
        [SerializeField] private bool showSequenceDebug = true;
        
        private List<int> _currentSequence = new List<int>();
        private List<int> _playerInput = new List<int>();
        private int _currentStep;
        private bool _isShowingSequence;
        private bool _isWaitingForInput;
        private float _inputTimer;
        
        protected override void OnFailed()
        {
            base.OnFailed();
            ResetPuzzle();
            // If you want to allow immediate retry upon timeout, uncomment to auto-restart timing
            // _timer = timeLimit; _isFailed = false; _isCompleted = false; StartCoroutine(ShowSequence());
        }

        protected override void OnPuzzleStart()
        {
            // Reset state
            _currentSequence.Clear();
            _playerInput.Clear();
            _currentStep = 0;
            _isShowingSequence = false;
            _isWaitingForInput = false;
            
            // Generate new sequence
            GenerateSequence();
            
            // Start showing sequence
            StartCoroutine(ShowSequence());
            
            Debug.Log($"Runic Sequence puzzle started with {sequenceLength} steps");
        }
        
        protected override void OnPuzzleComplete()
        {
            // Puzzle logic handled in base class
            Debug.Log("Runic Sequence puzzle completed!");
        }
        
        protected override void OnPuzzleFailed()
        {
            // Puzzle logic handled in base class
            Debug.Log("Runic Sequence puzzle failed - time expired!");
        }
        
        protected override void OnPuzzleReset()
        {
            ResetPuzzle();
        }
        
        private void Start()
        {
            // Subscribe to pad press events
            foreach (RunicPad pad in pads)
            {
                pad.OnPadPressed.AddListener(OnPadPressed);
            }
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from pad press events
            foreach (RunicPad pad in pads)
            {
                if (pad != null)
                {
                    pad.OnPadPressed.RemoveListener(OnPadPressed);
                }
            }
        }
        
        private void Update()
        {
            // Handle input timeout
            if (_isWaitingForInput)
            {
                _inputTimer -= Time.deltaTime;
                if (_inputTimer <= 0f)
                {
                    OnInputTimeout();
                }
            }
        }
        
        /// <summary>
        /// Generate a new random sequence for the puzzle.
        /// </summary>
        private void GenerateSequence()
        {
            _currentSequence.Clear();
            
            for (int i = 0; i < sequenceLength; i++)
            {
                int randomPadId = Random.Range(0, pads.Count);
                _currentSequence.Add(randomPadId);
            }
            
            if (showSequenceDebug)
            {
                string sequenceStr = string.Join(", ", _currentSequence);
                Debug.Log($"Generated sequence: {sequenceStr}");
            }
        }
        
        /// <summary>
        /// Show the sequence to the player step by step.
        /// </summary>
        private IEnumerator ShowSequence()
        {
            _isShowingSequence = true;
            
            // Wait a moment before starting
            yield return new WaitForSeconds(0.5f);
            
            // Show each step
            for (int i = 0; i < _currentSequence.Count; i++)
            {
                int padId = _currentSequence[i];
                RunicPad pad = GetPadById(padId);
                
                if (pad != null)
                {
                    // Highlight the pad
                    pad.Highlight();
                    
                    // Wait for step delay
                    yield return new WaitForSeconds(stepDelay);
                    
                    // Remove highlight
                    pad.Unhighlight();
                    
                    // Small pause between steps
                    yield return new WaitForSeconds(0.2f);
                }
            }
            
            _isShowingSequence = false;
            
            // Start waiting for player input
            StartWaitingForInput();
        }
        
        /// <summary>
        /// Start waiting for player input.
        /// </summary>
        private void StartWaitingForInput()
        {
            _isWaitingForInput = true;
            _inputTimer = inputTimeout;
            _currentStep = 0;
            _playerInput.Clear();
            
            Debug.Log("Sequence shown! Now waiting for player input...");
        }
        
        /// <summary>
        /// Handle pad press from player.
        /// </summary>
        /// <param name="padId">ID of the pressed pad.</param>
        private void OnPadPressed(int padId)
        {
            if (!_isWaitingForInput || _isShowingSequence) return;
            
            // Check if this is the correct next step
            if (padId == _currentSequence[_currentStep])
            {
                // Correct input
                _playerInput.Add(padId);
                _currentStep++;
                
                // Reset input timer
                _inputTimer = inputTimeout;
                
                if (showSequenceDebug)
                {
                    Debug.Log($"Correct input: {padId} (step {_currentStep}/{_currentSequence.Count})");
                }
                
                // Check if sequence is complete
                if (_currentStep >= _currentSequence.Count)
                {
                    OnSequenceCompleted();
                }
            }
            else
            {
                // Wrong input - reset puzzle
                OnWrongInput(padId);
            }
        }
        
        /// <summary>
        /// Handle correct sequence completion.
        /// </summary>
        private void OnSequenceCompleted()
        {
            _isWaitingForInput = false;
            Debug.Log("Sequence completed correctly!");
            
            // Complete the puzzle
            Complete();
        }
        
        /// <summary>
        /// Handle wrong input from player.
        /// </summary>
        /// <param name="wrongPadId">ID of the incorrectly pressed pad.</param>
        private void OnWrongInput(int wrongPadId)
        {
            _isWaitingForInput = false;
            Debug.Log($"Wrong input: {wrongPadId}. Expected: {_currentSequence[_currentStep]}. Resetting...");
            
            // Reset and try again
            StartCoroutine(ResetAndRetry());
        }
        
        /// <summary>
        /// Handle input timeout.
        /// </summary>
        private void OnInputTimeout()
        {
            _isWaitingForInput = false;
            Debug.Log("Input timeout! Resetting...");
            
            // Reset and try again
            StartCoroutine(ResetAndRetry());
        }
        
        /// <summary>
        /// Reset puzzle and retry with same sequence.
        /// </summary>
        private IEnumerator ResetAndRetry()
        {
            // Reset state
            _currentStep = 0;
            _playerInput.Clear();
            
            // Wait a moment
            yield return new WaitForSeconds(1f);
            
            // Show sequence again
            StartCoroutine(ShowSequence());
        }

        private void ResetPuzzle()
        {
            // Stop any running coroutines
            StopAllCoroutines();

            // Reset local state
            _isShowingSequence = false;
            _isWaitingForInput = false;
            _inputTimer = 0f;
            _currentStep = 0;
            _playerInput.Clear();

            // Reset pads visuals/state
            foreach (RunicPad pad in pads)
            {
                if (pad != null)
                {
                    pad.Unhighlight();
                    pad.Reset();
                }
            }

            // Notify legacy reset hook
            OnPuzzleReset();
        }
        
        /// <summary>
        /// Get a pad by its ID.
        /// </summary>
        /// <param name="padId">ID of the pad to find.</param>
        /// <returns>The pad with the specified ID, or null if not found.</returns>
        private RunicPad GetPadById(int padId)
        {
            foreach (RunicPad pad in pads)
            {
                if (pad.PadId == padId)
                {
                    return pad;
                }
            }
            return null;
        }
        
        // Unity Editor support
        private void OnValidate()
        {
            // Ensure sequenceLength is positive
            sequenceLength = Mathf.Max(1, sequenceLength);
            
            // Ensure stepDelay is positive
            stepDelay = Mathf.Max(0.1f, stepDelay);
            
            // Ensure inputTimeout is positive
            inputTimeout = Mathf.Max(1f, inputTimeout);
            
            // Remove null entries
            pads.RemoveAll(pad => pad == null);
        }
        
        // Gizmos for easier setup in editor
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            
            // Draw current sequence progress
            if (_currentSequence.Count > 0 && _currentStep < _currentSequence.Count)
            {
                Gizmos.color = Color.yellow;
                Vector3 center = Vector3.zero;
                int count = 0;
                
                foreach (RunicPad pad in pads)
                {
                    center += pad.transform.position;
                    count++;
                }
                
                if (count > 0)
                {
                    center /= count;
                    Gizmos.DrawWireSphere(center, 2f);
                    
                    // Draw progress indicator
                    float progress = (float)_currentStep / _currentSequence.Count;
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(center, 2f * progress);
                }
            }
        }
    }
} 