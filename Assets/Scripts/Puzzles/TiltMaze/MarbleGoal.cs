using UnityEngine;
using Run4theRelic.Puzzles;

namespace Run4theRelic.Puzzles.TiltMaze
{
    public class MarbleGoal : MonoBehaviour
    {
        [SerializeField] private Transform marble;

        private void OnTriggerEnter(Collider other)
        {
            if (marble != null && other.transform != marble) return;

            var active = PuzzleControllerBase.Active;
            if (active != null)
            {
                // Complete the current puzzle
                var completeMethod = active.GetType().GetMethod("Complete", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (completeMethod != null)
                {
                    completeMethod.Invoke(active, null);
                }
            }
        }
    }
}

