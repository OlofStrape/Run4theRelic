using UnityEngine;
using Run4theRelic.UI;

namespace Run4theRelic.Sabotage
{
    /// <summary>
    /// Ensures required sabotage systems exist at runtime. Place once in any scene.
    /// </summary>
    public class SabotageBootstrap : MonoBehaviour
    {
        [SerializeField] private bool createWheelIfMissing = true;
        [SerializeField] private bool createTokenBankIfMissing = true;
        [SerializeField] private bool createManagerIfMissing = true;

        private void Awake()
        {
            if (createManagerIfMissing && SabotageManager.Instance == null)
            {
                var go = new GameObject("SabotageManager");
                go.AddComponent<SabotageManager>();
            }

            if (createTokenBankIfMissing && SabotageTokenBank.Instance == null)
            {
                var go = new GameObject("SabotageTokenBank");
                go.AddComponent<SabotageTokenBank>();
            }

            if (createWheelIfMissing && FindObjectOfType<SabotageWheel>(true) == null)
            {
                var go = new GameObject("SabotageWheel");
                go.AddComponent<Canvas>();
                go.AddComponent<SabotageWheel>();
            }
        }
    }
}

