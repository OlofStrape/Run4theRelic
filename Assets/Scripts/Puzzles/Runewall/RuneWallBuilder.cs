using UnityEngine;

namespace Run4TheRelic.Puzzles.Runewall
{
\tpublic sealed class RuneWallBuilder : MonoBehaviour
\t{
\t\t[SerializeField] private int width = 4;
\t\t[SerializeField] private int height = 3;
\t\tpublic Vector2Int Size => new Vector2Int(width, height);
\t}
}
