using UnityEngine;
using System.Collections;

namespace Run4theRelic.Puzzles.RelicPlacement
{
    /// <summary>
    /// Slot that accepts exactly one matching TriadRelic. Provides auto-snap and wrong-placement flash.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class TriadSlot : MonoBehaviour
    {
        [Header("Slot Matching")]
        [SerializeField] private TriadRelicKey acceptedKey = TriadRelicKey.A;
        [SerializeField] private Transform snapPoint;

        [Header("Visual Feedback (optional)")]
        [SerializeField] private Renderer feedbackRenderer;
        [SerializeField] private Color wrongColor = Color.red;
        [SerializeField] private Color correctColor = Color.green;

        public System.Action<TriadSlot, bool> OnSlotResolved;

        float snapDistance = 0.2f;
        float wrongFlashDuration = 0.5f;
        TriadSlotsController controller;
        Color originalColor;
        bool hasOriginalColor;
        TriadRelic currentRelic;

        public bool IsCorrectlyOccupied => currentRelic != null && currentRelic.Key == acceptedKey;

        void Reset()
        {
            var col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        void Awake()
        {
            if (!snapPoint) snapPoint = transform;
            if (feedbackRenderer && feedbackRenderer.material != null)
            {
                originalColor = feedbackRenderer.material.color;
                hasOriginalColor = true;
            }
        }

        public void Configure(TriadSlotsController owner, float snapDist, float flashDuration)
        {
            controller = owner;
            snapDistance = Mathf.Max(0.01f, snapDist);
            wrongFlashDuration = Mathf.Max(0.05f, flashDuration);
        }

        void OnTriggerStay(Collider other)
        {
            if (currentRelic != null) return;
            var relic = other.GetComponentInParent<TriadRelic>();
            if (relic == null || relic.IsLocked) return;

            float distance = Vector3.Distance(relic.transform.position, snapPoint.position);
            if (distance <= snapDistance)
            {
                controller?.ValidatePlacement(this, relic);
            }
        }

        public bool AcceptsRelic(TriadRelic relic)
        {
            return relic != null && relic.Key == acceptedKey;
        }

        public void ResolvePlacement(TriadRelic relic, bool correct)
        {
            if (correct)
            {
                SnapRelic(relic);
                SetColor(correctColor, 0.2f);
                OnSlotResolved?.Invoke(this, true);
            }
            else
            {
                StartCoroutine(FlashWrong());
                OnSlotResolved?.Invoke(this, false);
            }
        }

        void SnapRelic(TriadRelic relic)
        {
            currentRelic = relic;
            var rb = relic.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            relic.transform.SetPositionAndRotation(snapPoint.position, snapPoint.rotation);
            relic.transform.SetParent(transform, true);
            relic.Lock();
        }

        IEnumerator FlashWrong()
        {
            SetColor(wrongColor, 1f);
            yield return new WaitForSeconds(wrongFlashDuration);
            RestoreColor();
        }

        void SetColor(Color color, float intensity)
        {
            if (!feedbackRenderer) return;
            if (!hasOriginalColor)
            {
                originalColor = feedbackRenderer.material.color;
                hasOriginalColor = true;
            }
            var c = color;
            c.a = Mathf.Clamp01(intensity);
            feedbackRenderer.material.color = c;
        }

        void RestoreColor()
        {
            if (feedbackRenderer && hasOriginalColor)
            {
                feedbackRenderer.material.color = originalColor;
            }
        }

        void OnDrawGizmosSelected()
        {
            if (snapPoint == null) snapPoint = transform;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(snapPoint.position, snapDistance > 0f ? snapDistance : 0.2f);
        }
    }
}

