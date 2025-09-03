using UnityEngine;
using Run4theRelic.Core;

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

            // Seed:a mål slumpmässigt vid start (om allt 0). Använd CSPRNG och undvik triviala mönster som alla lika.
            if (outerTarget == 0 && midTarget == 0 && innerTarget == 0)
            {
                int oSteps = outerRing ? Mathf.Max(1, outerRing.steps) : 8;
                int mSteps = midRing ? Mathf.Max(1, midRing.steps) : 8;
                int iSteps = innerRing ? Mathf.Max(1, innerRing.steps) : 8;

                outerTarget = SecureRandom.NextInt(oSteps);
                // Undvik att alla tre blir identiska genom enkla exclusions när möjligt
                midTarget = oSteps == mSteps ? SecureRandom.NextIndexExcluding(mSteps, outerTarget) : SecureRandom.NextInt(mSteps);
                if (midTarget < 0) midTarget = SecureRandom.NextInt(mSteps);

                if (iSteps >= 3 && oSteps == iSteps && mSteps == iSteps)
                {
                    // Försök göra inner unik från både outer och mid
                    int r = SecureRandom.NextInt(iSteps - 2);
                    int idx = 0, k = 0, choice = 0;
                    while (true)
                    {
                        if (idx != outerTarget && idx != midTarget)
                        {
                            if (k == r) { choice = idx; break; }
                            k++;
                        }
                        idx++;
                    }
                    innerTarget = choice;
                }
                else
                {
                    // Olika stegstorlekar: välj fritt
                    innerTarget = SecureRandom.NextInt(iSteps);
                }
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

