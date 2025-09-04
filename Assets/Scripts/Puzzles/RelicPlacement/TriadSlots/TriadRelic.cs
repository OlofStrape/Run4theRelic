using UnityEngine;

namespace Run4theRelic.Puzzles.RelicPlacement
{
    public enum TriadRelicKey { A, B, C }

    /// <summary>
    /// Tag component for relics used in Triad Slots. Supports locking after snap.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class TriadRelic : MonoBehaviour
    {
        [SerializeField] private TriadRelicKey key = TriadRelicKey.A;
        public TriadRelicKey Key => key;

        public bool IsLocked { get; private set; }

        public void Lock()
        {
            IsLocked = true;
            var col = GetComponent<Collider>();
            if (col) col.enabled = false;
        }
    }
}

