using UnityEngine;
using UnityEngine.SceneManagement;
using Core;

namespace UI.PostMatch
{
	/// <summary>Enkel resultatskärm. Visar text + restart-knapp.</summary>
	public class PostMatchPanel : MonoBehaviour
	{
		[Tooltip("Root-objektet som visas vid slutet. Döljs i Start().")]
		public GameObject panelRoot;

		[Tooltip("Text-komponent (TextMeshProUGUI eller TextMesh). Om null, skapas en enkel TextMesh i world-space.")]
		public Component textTarget; // assign TMP or TextMesh in Inspector

		[Tooltip("Valfritt: StatsTracker i scenen; om null söks upp.")]
		public StatsTracker stats;

		void Awake()
		{
			if (!stats) stats = FindFirstObjectByType<StatsTracker>();
		}

		void Start()
		{
			if (panelRoot) panelRoot.SetActive(false);
			Core.GameEvents.OnRelicExtracted += OnWin;
		}

		void OnDestroy()
		{
			Core.GameEvents.OnRelicExtracted -= OnWin;
		}

		void OnWin(int _)
		{
			if (panelRoot) panelRoot.SetActive(true);
			string msg = stats ? stats.GetSummaryText() : "RESULT\n(no stats)";
			ApplyText(msg);
		}

		void ApplyText(string msg)
		{
			if (!textTarget)
			{
				var go = new GameObject("ResultText");
				go.transform.SetParent(panelRoot ? panelRoot.transform : transform, false);
				var tm = go.AddComponent<TextMesh>();
				tm.characterSize = 0.08f;
				tm.anchor = TextAnchor.UpperLeft;
				tm.text = msg;
				return;
			}

			// Support TMP or TextMesh
			var tmu = textTarget as TMPro.TextMeshProUGUI;
			if (tmu) { tmu.text = msg; return; }
			var tm = textTarget as TextMesh;
			if (tm) { tm.text = msg; return; }
			Debug.LogWarning("[PostMatchPanel] Unknown text component.");
		}

		public void PlayAgain() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		public void QuitGame() { #if UNITY_EDITOR UnityEditor.EditorApplication.isPlaying = false; #else Application.Quit(); #endif }
	}
}

