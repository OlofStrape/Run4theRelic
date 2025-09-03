using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
            _sequence = new int[sequenceLength];
            for (int i = 0; i < sequenceLength; i++)
                _sequence[i] = Random.Range(0, Mathf.Min(symbolDomain, pads.Count));
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

