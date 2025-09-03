using UnityEngine;

namespace Run4TheRelic.Puzzles
{
\tpublic sealed class PuzzleGate : MonoBehaviour
\t{
\t\t[SerializeField] private bool isOpen;

\t\tpublic void Open() { isOpen = true; }
\t\tpublic void Close() { isOpen = false; }
\t\tpublic bool IsOpen => isOpen;
\t}
}
