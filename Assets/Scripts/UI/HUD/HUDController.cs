using System.Collections;
using UnityEngine;
using Run4theRelic.Core;

namespace UI.HUD
{
    /// <summary>
    /// World-space HUD that displays match phase, timers, and sabotage messages.
    /// Auto-billboards toward the camera and positions itself relative to it.
    /// Attempts to use TextMeshPro if present, otherwise falls back to TextMesh.
    /// To add TMP: open Package Manager and install "TextMeshPro".
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        [Header("Setup")]
        /// <summary>
        /// Target camera for positioning and billboarding. If null, uses Camera.main.
        /// </summary>
        public Camera targetCamera;

        [Header("Layout")]
        /// <summary>
        /// Distance in meters in front of the camera to place the HUD.
        /// </summary>
        public float distance = 1.2f;

        /// <summary>
        /// Local offset relative to the camera forward anchor.
        /// </summary>
        public Vector3 localOffset = new Vector3(0f, 0.25f, 0f);

        [Header("Visuals")]
        /// <summary>
        /// Gradient for timer colorization from green -> yellow -> red.
        /// Initialized in Awake if not set.
        /// </summary>
        public Gradient timerColor;

        /// <summary>
        /// Duration of the sabotage flash in seconds.
        /// </summary>
        public float sabotageFlashSeconds = 2f;

        // Internal text components (TMP or TextMesh at runtime)
        private Component _phaseText;
        private Component _timerText;
        private Component _messageText;

        // Cached for TMP detection
        private System.Type _tmpTypeText;

        private void Awake()
        {
            if (timerColor == null || timerColor.colorKeys == null || timerColor.colorKeys.Length == 0)
            {
                var g = new Gradient();
                g.SetKeys(
                    new[]
                    {
                        new GradientColorKey(Color.green, 0f),
                        new GradientColorKey(Color.yellow, 0.5f),
                        new GradientColorKey(Color.red, 1f)
                    },
                    new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
                );
                timerColor = g;
            }

            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }

            DetectTMP();
            BuildTexts();
        }

        private void OnEnable()
        {
            GameEvents.OnPhaseChanged += HandlePhaseChanged;
            GameEvents.OnPuzzleTimerTick += HandlePuzzleTimerTick;
            GameEvents.OnSabotaged += HandleSabotaged;
        }

        private void OnDisable()
        {
            GameEvents.OnPhaseChanged -= HandlePhaseChanged;
            GameEvents.OnPuzzleTimerTick -= HandlePuzzleTimerTick;
            GameEvents.OnSabotaged -= HandleSabotaged;
        }

        private void LateUpdate()
        {
            Camera cam = targetCamera != null ? targetCamera : Camera.main;
            if (cam == null) return;

            Vector3 anchor = cam.transform.position + cam.transform.forward * distance;
            transform.position = anchor + cam.transform.TransformVector(localOffset);
        }

        private void HandlePhaseChanged(MatchPhase phase, float durationSeconds)
        {
            SetText(_phaseText, PhaseToString(phase));
            if (durationSeconds <= 0.001f)
            {
                SetText(_timerText, string.Empty);
            }
        }

        private void HandlePuzzleTimerTick(int secondsRemaining, int secondsLimit)
        {
            SetText(_timerText, FormatTime(secondsRemaining));

            float t = 1f - Mathf.Clamp01(secondsLimit <= 0 ? 0f : (secondsRemaining / Mathf.Max(1f, (float)secondsLimit)));
            Color c = timerColor.Evaluate(t);
            SetColor(_timerText, c);
        }

        private void HandleSabotaged(string typeId, float durationSeconds)
        {
            StopAllCoroutines();
            StartCoroutine(SabotageFlashCoroutine(durationSeconds > 0f ? durationSeconds : sabotageFlashSeconds));
        }

        private IEnumerator SabotageFlashCoroutine(float duration)
        {
            SetText(_messageText, "SABOTAGED!");
            SetColor(_messageText, Color.red);
            SetAlpha(_messageText, 1f);

            float elapsed = 0f;
            while (elapsed < duration)
            {
                float pulse = 0.5f + 0.5f * Mathf.Sin(elapsed * 8f);
                SetScale(_messageText, Mathf.Lerp(1f, 1.15f, pulse));
                SetAlpha(_messageText, Mathf.Lerp(0.8f, 1f, pulse));

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Fade out
            float fade = 0.5f;
            float t = 0f;
            while (t < fade)
            {
                float a = Mathf.Lerp(1f, 0f, t / fade);
                SetAlpha(_messageText, a);
                t += Time.deltaTime;
                yield return null;
            }
            SetText(_messageText, string.Empty);
            SetScale(_messageText, 1f);
        }

        private void DetectTMP()
        {
            // Runtime-detect TextMeshPro by type name, avoids compile-time dependency
            _tmpTypeText = System.Type.GetType("TMPro.TextMeshPro, Unity.TextMeshPro") ?? System.Type.GetType("TMPro.TextMeshPro, TMPro");
        }

        private void BuildTexts()
        {
            // Phase
            var phaseGo = new GameObject("HUD_PhaseText");
            phaseGo.transform.SetParent(transform, false);
            _phaseText = CreateTextComponent(phaseGo, new Vector3(0f, 0.06f, 0f), 0.05f, FontStyle.Bold);
            phaseGo.AddComponent<Billboard>().targetCamera = targetCamera;

            // Timer
            var timerGo = new GameObject("HUD_TimerText");
            timerGo.transform.SetParent(transform, false);
            _timerText = CreateTextComponent(timerGo, new Vector3(0f, -0.02f, 0f), 0.06f, FontStyle.Bold);
            timerGo.AddComponent<Billboard>().targetCamera = targetCamera;

            // Message
            var msgGo = new GameObject("HUD_MessageText");
            msgGo.transform.SetParent(transform, false);
            _messageText = CreateTextComponent(msgGo, new Vector3(0f, 0.14f, 0f), 0.07f, FontStyle.Bold);
            msgGo.AddComponent<Billboard>().targetCamera = targetCamera;
            SetText(_messageText, string.Empty);
        }

        private Component CreateTextComponent(GameObject go, Vector3 localPos, float size, FontStyle style)
        {
            go.transform.localPosition = localPos;
            if (_tmpTypeText != null)
            {
                // Use TMPro if available
                var tmp = go.AddComponent(_tmpTypeText);
                // Set common properties via reflection
                SetTMPProperty(tmp, "text", "");
                SetTMPProperty(tmp, "fontSize", size * 100f);
                SetTMPAlignmentCenter(tmp);
                SetColor(tmp, Color.white);
                return tmp;
            }
            else
            {
                // Fallback to TextMesh (3D text)
                var tm = go.AddComponent<TextMesh>();
                tm.text = "";
                tm.fontSize = 64;
                tm.characterSize = size;
                tm.anchor = TextAnchor.MiddleCenter;
                tm.alignment = TextAlignment.Center;
                tm.color = Color.white;
                return tm;
            }
        }

        private void SetTMPAlignmentCenter(Component tmp)
        {
            if (tmp == null) return;
            var prop = tmp.GetType().GetProperty("alignment");
            if (prop != null)
            {
                // 514 = Midline + Center in TMPro Horizontal/Vertical options
                // But safer to use enum via string when available
                var enumType = prop.PropertyType;
                var center = System.Enum.Parse(enumType, "Center");
                prop.SetValue(tmp, center, null);
            }
        }

        private void SetTMPProperty(Component tmp, string property, object value)
        {
            if (tmp == null) return;
            var p = tmp.GetType().GetProperty(property);
            if (p != null && p.CanWrite)
            {
                p.SetValue(tmp, value, null);
            }
        }

        private void SetText(Component c, string text)
        {
            if (c == null) return;
            if (_tmpTypeText != null && _tmpTypeText.IsInstanceOfType(c))
            {
                SetTMPProperty(c, "text", text);
            }
            else if (c is TextMesh tm)
            {
                tm.text = text;
            }
        }

        private void SetColor(Component c, Color color)
        {
            if (c == null) return;
            if (_tmpTypeText != null && _tmpTypeText.IsInstanceOfType(c))
            {
                SetTMPProperty(c, "color", color);
            }
            else if (c is TextMesh tm)
            {
                tm.color = color;
            }
        }

        private void SetAlpha(Component c, float alpha)
        {
            if (c == null) return;
            if (_tmpTypeText != null && _tmpTypeText.IsInstanceOfType(c))
            {
                Color col = (Color)c.GetType().GetProperty("color").GetValue(c, null);
                col.a = alpha;
                SetTMPProperty(c, "color", col);
            }
            else if (c is TextMesh tm)
            {
                Color col = tm.color;
                col.a = alpha;
                tm.color = col;
            }
        }

        private void SetScale(Component c, float scale)
        {
            if (c == null) return;
            c.transform.localScale = Vector3.one * scale;
        }

        private static string FormatTime(int seconds)
        {
            seconds = Mathf.Max(0, seconds);
            int m = seconds / 60;
            int s = seconds % 60;
            return m.ToString("00") + ":" + s.ToString("00");
        }

        private static string PhaseToString(MatchPhase phase)
        {
            switch (phase)
            {
                case MatchPhase.Lobby: return "LOBBY";
                case MatchPhase.Countdown: return "COUNTDOWN";
                case MatchPhase.Puzzle1: return "PUZZLE 1";
                case MatchPhase.Puzzle2: return "PUZZLE 2";
                case MatchPhase.Puzzle3: return "PUZZLE 3";
                case MatchPhase.GoldTimeSabotage: return "SABOTAGE";
                case MatchPhase.Final: return "FINAL";
                case MatchPhase.PostMatch: return "POST MATCH";
                default: return phase.ToString().ToUpperInvariant();
            }
        }
    }
}

