using UnityEditor;
using UnityEngine;

namespace Run4theRelic.EditorTools
{
	/// <summary>
	/// Material factory for creating and updating URP/Lit materials used by Run for the Relic.
	/// Adds a menu item: Relic → Build Materials.
	/// 
	/// Creates/updates the following materials in Assets/Materials/:
	/// - M_Stone (base #8A8A8A, Metallic 0, Smoothness 0.2)
	/// - M_Moss (base #2A7F61, Metallic 0, Smoothness 0.35)
	/// - M_Gold (base #C8A33A, Metallic 1.0, Smoothness 0.8)
	/// - M_RuneGlow (dark base, Emission ON #00E5FF at ~1.5x)
	/// - M_RelicCore (dark base, Emission ON #FF7A00 at ~2.0x)
	/// </summary>
	public static class MaterialFactory
	{
		private const string MaterialsFolder = "Assets/Materials";
		private const string ShaderName = "Universal Render Pipeline/Lit";

		[MenuItem("Relic/Build Materials")]
		private static void BuildMaterials()
		{
			EnsureFolderExists(MaterialsFolder);

			CreateOrUpdateLit(
				"M_Stone",
				new Color32(0x8A, 0x8A, 0x8A, 0xFF),
				metallic: 0f,
				smoothness: 0.2f
			);

			CreateOrUpdateLit(
				"M_Moss",
				new Color32(0x2A, 0x7F, 0x61, 0xFF),
				metallic: 0f,
				smoothness: 0.35f
			);

			CreateOrUpdateLit(
				"M_Gold",
				new Color32(0xC8, 0xA3, 0x3A, 0xFF),
				metallic: 1.0f,
				smoothness: 0.8f
			);

			CreateOrUpdateLit(
				"M_RuneGlow",
				new Color32(10, 10, 10, 0xFF),
				metallic: 0f,
				smoothness: 0.2f,
				emissionColor: new Color32(0x00, 0xE5, 0xFF, 0xFF),
				emissionIntensity: 1.5f
			);

			CreateOrUpdateLit(
				"M_RelicCore",
				new Color32(10, 10, 10, 0xFF),
				metallic: 0f,
				smoothness: 0.2f,
				emissionColor: new Color32(0xFF, 0x7A, 0x00, 0xFF),
				emissionIntensity: 2.0f
			);
		}

		private static void EnsureFolderExists(string assetFolder)
		{
			if (!AssetDatabase.IsValidFolder(assetFolder))
			{
				var parent = System.IO.Path.GetDirectoryName(assetFolder).Replace('\\', '/');
				var name = System.IO.Path.GetFileName(assetFolder);
				AssetDatabase.CreateFolder(parent, name);
				AssetDatabase.SaveAssets();
			}
		}

		private static void CreateOrUpdateLit(
			string materialName,
			Color baseColor,
			float metallic,
			float smoothness,
			Color? emissionColor = null,
			float emissionIntensity = 1f)
		{
			var shader = Shader.Find(ShaderName);
			if (shader == null)
			{
				Debug.LogError($"Shader not found: {ShaderName}. Ensure URP is installed.");
				return;
			}

			var path = $"{MaterialsFolder}/{materialName}.mat";
			var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
			bool isNew = false;
			if (mat == null)
			{
				mat = new Material(shader);
				AssetDatabase.CreateAsset(mat, path);
				isNew = true;
			}

			mat.shader = shader;
			mat.SetColor("_BaseColor", baseColor);
			mat.SetFloat("_Metallic", metallic);
			mat.SetFloat("_Smoothness", smoothness);

			if (emissionColor.HasValue)
			{
				mat.EnableKeyword("_EMISSION");
				var emissive = (Color)emissionColor * emissionIntensity;
				mat.SetColor("_EmissionColor", emissive);
			}
			else
			{
				mat.DisableKeyword("_EMISSION");
			}

			EditorUtility.SetDirty(mat);
			AssetDatabase.SaveAssets();

			Debug.Log($"{(isNew ? "Created" : "Updated")} {materialName}");
		}
	}
}

