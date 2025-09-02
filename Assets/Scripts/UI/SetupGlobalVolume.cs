using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Run4theRelic.UI
{
	/// <summary>
	/// Ensures a global URP Volume with Bloom is present in the scene.
	/// On Awake, finds or creates a "Global Volume" with a VolumeProfile that has Bloom override.
	/// På Quest, håll bloom låg för perf.
	/// </summary>
	public class SetupGlobalVolume : MonoBehaviour
	{
		[Header("Quality")]
		public bool enableOnLowQuality = true;

		[Header("Bloom Defaults")]
		public float bloomIntensity = 0.3f;
		public float bloomThreshold = 0.9f;

		private void Awake()
		{
			if (!enableOnLowQuality && QualitySettings.GetQualityLevel() <= 0)
			{
				return;
			}

			var existing = FindObjectOfType<Volume>();
			if (existing != null)
			{
				EnsureBloom(existing);
				return;
			}

			var go = new GameObject("Global Volume");
			var volume = go.AddComponent<Volume>();
			volume.isGlobal = true;
			volume.priority = 0f;
			volume.weight = 1f;

			var profile = ScriptableObject.CreateInstance<VolumeProfile>();
			volume.sharedProfile = profile;

			AddOrUpdateBloom(profile);
		}

		private static void EnsureBloom(Volume volume)
		{
			var profile = volume.sharedProfile;
			if (profile == null)
			{
				profile = ScriptableObject.CreateInstance<VolumeProfile>();
				volume.sharedProfile = profile;
			}
			AddOrUpdateBloom(profile);
		}

		private void AddOrUpdateBloom(VolumeProfile profile)
		{
			if (!profile.TryGet(out Bloom bloom))
			{
				bloom = profile.Add<Bloom>(true);
			}
			bloom.active = true;
			bloom.intensity.overrideState = true;
			bloom.intensity.value = bloomIntensity;
			bloom.threshold.overrideState = true;
			bloom.threshold.value = bloomThreshold;
		}
	}
}

