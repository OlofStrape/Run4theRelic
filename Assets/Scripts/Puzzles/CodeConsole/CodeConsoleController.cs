using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Run4theRelic.Core;

namespace Run4theRelic.Puzzles.CodeConsole
{
    /// <summary>Visar en kodsekvens som spelaren måste trycka in i rätt ordning.</summary>
    public class CodeConsoleController : Run4theRelic.Puzzles.PuzzleControllerBase
    {
        [Header("Pads & sekvens")]
        public List<PadButton> pads = new();
        [Min(1)] public int sequenceLength = 6;
        [Tooltip("Antal olika symboler som kan förekomma (0..N-1).")]
        [Min(2)] public int symbolDomain = 10;

        [Header("Feedback")]
        [Tooltip("Tid mellan blink vid uppspelning av sekvens.")]
        public float playbackStepSeconds = 0.35f;
        [Tooltip("Strafftid vid fel input (sekunder).")]
        public float failPenaltySeconds = 2f;

        int[] _sequence;
        int _inputIndex;
        bool _playingBack;

        protected override void OnStartPuzzle()
        {
            GenerateSequence();
            StartCoroutine(PlaySequence());
        }

        void GenerateSequence()
        {
            int domain = Mathf.Min(symbolDomain, pads.Count);
            if (domain < 2) domain = 2;

            _sequence = new int[sequenceLength];
            for (int i = 0; i < sequenceLength; i++)
            {
                if (i == 0)
                {
                    _sequence[i] = SecureRandom.NextInt(domain);
                }
                else if (i == 1)
                {
                    // Forbid immediate repeat
                    int prev = _sequence[i - 1];
                    int choice = SecureRandom.NextIndexExcluding(domain, prev);
                    _sequence[i] = choice < 0 ? prev : choice;
                }
                else
                {
                    int prev = _sequence[i - 1];
                    int prev2 = _sequence[i - 2];
                    if (domain >= 3)
                    {
                        // Exclude prev and prev2 to reduce ABAB patterns
                        int r = SecureRandom.NextInt(domain - 2);
                        int idx = 0; int k = 0; int choice = 0;
                        while (true)
                        {
                            if (idx != prev && idx != prev2)
                            {
                                if (k == r) { choice = idx; break; }
                                k++;
                            }
                            idx++;
                        }
                        _sequence[i] = choice;
                    }
                    else
                    {
                        // Only two symbols: alternate (cannot avoid ABAB entirely)
                        _sequence[i] = prev == 0 ? 1 : 0;
                    }
                }
            }
            _inputIndex = 0;
        }

        IEnumerator PlaySequence()
        {
            _playingBack = true;
            yield return new WaitForSeconds(0.4f);

            for (int i = 0; i < _sequence.Length; i++)
            {
                var pad = FindPad(_sequence[i]);
                if (pad) pad.BlinkOK(0.15f);
                yield return new WaitForSeconds(playbackStepSeconds);
            }

            _playingBack = false;
        }

        PadButton FindPad(int id)
        {
            for (int i = 0; i < pads.Count; i++)
                if (pads[i] && pads[i].symbolId == id) return pads[i];
            return null;
        }

        public void OnPadPressed(PadButton pad)
        {
            if (!IsActive || _playingBack || pad == null) return;

            int expected = _sequence[_inputIndex];
            if (pad.symbolId == expected)
            {
                pad.BlinkOK();
                _inputIndex++;

                if (_inputIndex >= _sequence.Length)
                {
                    // puzzle clear!
                    Complete();
                }
            }
            else
            {
                // Fel – straffa och börja om input
                pad.BlinkError();
                ApplyTimeDrain(failPenaltySeconds);
                _inputIndex = 0;
                // (Valfritt) spela upp sekvensen igen för att hjälpa spelaren:
                StartCoroutine(PlaySequence());
            }
        }
    }
}

