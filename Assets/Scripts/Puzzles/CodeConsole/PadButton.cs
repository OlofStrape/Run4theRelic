using UnityEngine;

namespace Run4theRelic.Puzzles.CodeConsole
{
    /// <summary>Tryckbar pad. Kopplas till CodeConsoleController.</summary>
    public class PadButton : MonoBehaviour
    {
        [Tooltip("Unikt ID (0..N-1) för den här knappen).")]
        public int symbolId;

        [Tooltip("Valfri renderer för blink/feedback.")]
        public Renderer targetRenderer;

        [Header("Färger")]
        public Color idleColor = new Color(0.1f, 0.1f, 0.1f);
        public Color pressColor = new Color(0.3f, 0.8f, 1f);
        public Color errorColor = new Color(1f, 0.2f, 0.2f);

        Material _matInstance;

        void Awake()
        {
            if (targetRenderer)
                _matInstance = targetRenderer.material;
            SetColor(idleColor);
        }

        public void Press()
        {
            // Hooka denna till XR Simple Interactable → Activated → PadButton.Press()
            var console = GetComponentInParent<CodeConsoleController>();
            if (console) console.OnPadPressed(this);
        }

        public void BlinkOK(float seconds = 0.1f) => StartCoroutine(Blink(pressColor, seconds));
        public void BlinkError(float seconds = 0.15f) => StartCoroutine(Blink(errorColor, seconds));

        System.Collections.IEnumerator Blink(Color c, float s)
        {
            var prev = _matInstance ? _matInstance.color : Color.white;
            SetColor(c);
            yield return new WaitForSeconds(s);
            SetColor(idleColor);
        }

        void SetColor(Color c)
        {
            if (_matInstance) _matInstance.color = c;
        }
    }
}

