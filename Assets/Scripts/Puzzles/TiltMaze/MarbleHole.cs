using UnityEngine;
using Run4theRelic.Puzzles;

namespace Run4theRelic.Puzzles.TiltMaze
{
    public class MarbleHole : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform marble;
        [SerializeField] private Transform marbleSpawnPoint;
        [SerializeField] private Rigidbody marbleRigidbody;
        [Header("Options")]
        [SerializeField] private bool applyTimeDrain = true;
        [SerializeField] private float timeDrainSeconds = 2f;

        private void ResetMarble()
        {
            if (marble == null || marbleSpawnPoint == null) return;
            if (marbleRigidbody == null) marbleRigidbody = marble.GetComponent<Rigidbody>();

            if (marbleRigidbody != null)
            {
                marbleRigidbody.velocity = Vector3.zero;
                marbleRigidbody.angularVelocity = Vector3.zero;
            }

            marble.SetPositionAndRotation(marbleSpawnPoint.position, marbleSpawnPoint.rotation);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (marble == null) return;
            if (other.transform != marble) return;

            ResetMarble();

            if (applyTimeDrain)
            {
                var active = PuzzleControllerBase.Active;
                if (active != null)
                {
                    active.ApplyTimeDrain(Mathf.Max(0f, timeDrainSeconds));
                }
            }
        }
    }
}

