using UnityEngine;

namespace Run4theRelic.Puzzles.RuneCipher
{
    /// <summary>Tre ringhjul som ska matcha en mål-kombination.</summary>
    public class RuneCipherController : Run4theRelic.Puzzles.PuzzleControllerBase
    {
        public DialKnob outerRing;
        public DialKnob midRing;
        public DialKnob innerRing;

        [Header("Mål (0..steps-1)")]
        public int outerTarget;
        public int midTarget;
        public int innerTarget;

        protected override void OnStartPuzzle()
        {
            if (outerRing) outerRing.OnStepChanged += _ => CheckSolved();
            if (midRing) midRing.OnStepChanged += _ => CheckSolved();
            if (innerRing) innerRing.OnStepChanged += _ => CheckSolved();

            // Seed:a mål slumpmässigt vid start (om allt 0)
            if (outerTarget == 0 && midTarget == 0 && innerTarget == 0)
            {
                outerTarget = Random.Range(0, outerRing ? outerRing.steps : 8);
                midTarget = Random.Range(0, midRing ? midRing.steps : 8);
                innerTarget = Random.Range(0, innerRing ? innerRing.steps : 8);
            }
        }

        void CheckSolved()
        {
            if (!IsActive) return;
            if (!outerRing || !midRing || !innerRing) return;

            bool ok =
                outerRing.CurrentStep == outerTarget &&
                midRing.CurrentStep == midTarget &&
                innerRing.CurrentStep == innerTarget;

            if (ok) Complete();
        }

        public override void SpawnFakeClues(float durationSeconds)
        {
            Debug.Log($"[RuneCipher] FakeClues: visar fel mål i {durationSeconds:0.0}s (MVP-stub).");
        }
    }
}

