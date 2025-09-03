using System.Collections;
using UnityEngine;
using Run4theRelic.Core;
using Run4theRelic.Puzzles;

namespace Run4theRelic.Sabotage
{
    /// <summary>
    /// Central sabotage controller. Provides Fog, TimeDrain and FakeClues effects.
    /// Uses PuzzleControllerBase.Active to target the currently running puzzle.
    /// </summary>
    public class SabotageManager : MonoBehaviour
    {
        public static SabotageManager Instance { get; private set; }

        [Header("Fog")]
        [Tooltip("Rot för dimma/FX. Om null skapas ett enkelt ParticleSystem i runtime.")]
        public GameObject fogRoot;
        [Min(0.5f)] public float fogDuration = 5f;

        Coroutine _fogRoutine;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        // === FOG ===
        /// <summary>Apply fog for the default <see cref="fogDuration"/>.</summary>
        public void ApplyFog() => ApplyFog(fogDuration);

        /// <summary>Apply fog effect for a specific duration in seconds.</summary>
        public void ApplyFog(float duration)
        {
            if (_fogRoutine != null) StopCoroutine(_fogRoutine);
            _fogRoutine = StartCoroutine(FogRoutine(duration));
            GameEvents.TriggerSabotaged("fog", duration);
        }

        IEnumerator FogRoutine(float duration)
        {
            if (!fogRoot)
            {
                // Skapa enkel dimma som fallback
                fogRoot = CreateFallbackFog();
            }

            // Aktivera rot och starta alla ParticleSystems under den
            fogRoot.SetActive(true);
            var systems = fogRoot.GetComponentsInChildren<ParticleSystem>(true);
            for (int i = 0; i < systems.Length; i++)
            {
                systems[i].Play(true);
            }
            yield return new WaitForSeconds(duration);
            // Stoppa emission och stäng av rot
            for (int i = 0; i < systems.Length; i++)
            {
                systems[i].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
            fogRoot.SetActive(false);
            _fogRoutine = null;
        }

        GameObject CreateFallbackFog()
        {
            var go = new GameObject("FallbackFog");
            go.transform.SetParent(transform);
            var ps = go.AddComponent<ParticleSystem>();
            var main = ps.main; main.startLifetime = 3f; main.startSpeed = 0.1f; main.startSize = 4f; main.loop = true;
            var emission = ps.emission; emission.rateOverTime = 8f;
            var shape = ps.shape; shape.shapeType = ParticleSystemShapeType.Sphere; shape.radius = 2.5f;
            var c = ps.colorOverLifetime; c.enabled = true;
            var grad = new Gradient();
            grad.SetKeys(
                new[] { new GradientColorKey(new Color(0.8f, 0.8f, 0.8f), 0f), new GradientColorKey(new Color(0.75f, 0.75f, 0.75f), 1f) },
                new[] { new GradientAlphaKey(0.0f, 0f), new GradientAlphaKey(0.3f, 0.2f), new GradientAlphaKey(0.3f, 0.8f), new GradientAlphaKey(0.0f, 1f) }
            );
            c.color = new ParticleSystem.MinMaxGradient(grad);
            // Startläge: inaktiv rot, system startas på ApplyFog
            go.SetActive(false);
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            return go;
        }

        // === TIME DRAIN ===
        /// <summary>Drain time from the active puzzle.</summary>
        public void ApplyTimeDrain(float seconds)
        {
            if (seconds <= 0f) return;
            var active = PuzzleControllerBase.Active;
            if (active != null)
            {
                active.ApplyTimeDrain(seconds);
                GameEvents.TriggerSabotaged("timedrain", seconds);
            }
            else
            {
                Debug.LogWarning("[Sabotage] No active puzzle for TimeDrain.");
            }
        }

        // === FAKE CLUES ===
        /// <summary>Trigger fake clues on the active puzzle (delegated to puzzle implementation).</summary>
        public void ApplyFakeClues(float duration)
        {
            var active = PuzzleControllerBase.Active;
            if (active != null)
            {
                active.SpawnFakeClues(duration);
                GameEvents.TriggerSabotaged("fakeclues", duration);
            }
            else
            {
                Debug.LogWarning("[Sabotage] No active puzzle for FakeClues.");
            }
        }

        // Overload to be compatible with existing UI wheel signature
        /// <summary>Overload kept for UI compatibility; count is ignored by default.</summary>
        public void ApplyFakeClues(float duration, int count)
        {
            ApplyFakeClues(duration);
        }
    }
}