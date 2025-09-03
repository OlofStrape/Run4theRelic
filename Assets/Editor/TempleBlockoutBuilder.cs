using UnityEditor;
using UnityEngine;

namespace Run4theRelic.EditorTools
{
	/// <summary>
	/// Builds a simple temple blockout for quick iteration.
	/// Adds a menu item: Relic → Build Temple Blockout.
	/// 
	/// Layout:
	/// - Root: Temple_Blockout
	/// - Floor: Plane 10x10 with M_Stone
	/// - Walls: 3 sides closed, one side with door opening; height ~3.5m, thickness ~0.2m
	/// - Pillars: 8 cubes along edges with slight random scale/color variation using M_Moss
	/// - Pedestal: Center cylinder with M_Gold
	/// </summary>
	public static class TempleBlockoutBuilder
	{
		private const string MaterialsFolder = "Assets/Materials";
		private const string MStone = MaterialsFolder + "/M_Stone.mat";
		private const string MMoss = MaterialsFolder + "/M_Moss.mat";
		private const string MGold = MaterialsFolder + "/M_Gold.mat";

		[MenuItem("Relic/Build Temple Blockout")]
		private static void BuildTemple()
		{
			var stone = AssetDatabase.LoadAssetAtPath<Material>(MStone);
			var moss = AssetDatabase.LoadAssetAtPath<Material>(MMoss);
			var gold = AssetDatabase.LoadAssetAtPath<Material>(MGold);

			if (stone == null || moss == null || gold == null)
			{
				Debug.LogWarning("Some materials are missing. Run 'Relic → Build Materials' first.");
			}

			var root = new GameObject("Temple_Blockout");

			// Floor
			var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
			floor.name = "Floor";
			floor.transform.SetParent(root.transform);
			floor.transform.localScale = new Vector3(10f, 1f, 10f);
			AssignMaterialSafe(floor, stone);

			// Walls (leave an opening on +Z side for a doorway)
			float wallHeight = 3.5f;
			float wallThickness = 0.2f;
			float half = 5f * 10f; // plane scaled by 10 => 10 units per side; plane size ~10 => extents ~50
			// Adjust to ~50 world units extents from scale above
			half = 50f;

			CreateWall(root.transform, new Vector3(0f, wallHeight * 0.5f, -half), new Vector3(2f * half, wallHeight, wallThickness), stone, "Wall_-Z");
			CreateWall(root.transform, new Vector3(-half, wallHeight * 0.5f, 0f), new Vector3(wallThickness, wallHeight, 2f * half), stone, "Wall_-X");
			CreateWall(root.transform, new Vector3(half, wallHeight * 0.5f, 0f), new Vector3(wallThickness, wallHeight, 2f * half), stone, "Wall_+X");

			// +Z side with doorway opening in the center
			float doorwayWidth = 6f;
			float doorwayHeight = 2.5f;
			float segment = (2f * half - doorwayWidth) * 0.5f;
			CreateWall(root.transform, new Vector3(-((doorwayWidth * 0.5f) + (segment * 0.5f)), wallHeight * 0.5f, half), new Vector3(segment, wallHeight, wallThickness), stone, "Wall_+Z_L");
			CreateWall(root.transform, new Vector3(((doorwayWidth * 0.5f) + (segment * 0.5f)), wallHeight * 0.5f, half), new Vector3(segment, wallHeight, wallThickness), stone, "Wall_+Z_R");
			// optional header above doorway
			CreateWall(root.transform, new Vector3(0f, (doorwayHeight + wallHeight) * 0.5f, half), new Vector3(doorwayWidth, wallHeight - doorwayHeight, wallThickness), stone, "Wall_+Z_Top");

			// Pillars (8 around edges)
			int pillarCount = 8;
			for (int i = 0; i < pillarCount; i++)
			{
				float t = i / (float)pillarCount;
				float angle = t * Mathf.PI * 2f;
				float radius = half - 3f;
				var pos = new Vector3(Mathf.Cos(angle) * radius, wallHeight * 0.5f, Mathf.Sin(angle) * radius);
				var pillar = GameObject.CreatePrimitive(PrimitiveType.Cube);
				pillar.name = $"Pillar_{i + 1}";
				pillar.transform.SetParent(root.transform);
				float scaleY = wallHeight * Random.Range(0.9f, 1.1f);
				pillar.transform.localScale = new Vector3(Random.Range(0.3f, 0.5f), scaleY, Random.Range(0.3f, 0.5f));
				pillar.transform.localPosition = pos;
				AssignMaterialSafe(pillar, moss);
			}

			// Pedestal
			var pedestal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			pedestal.name = "Pedestal";
			pedestal.transform.SetParent(root.transform);
			pedestal.transform.localScale = new Vector3(2f, 1.2f, 2f);
			pedestal.transform.localPosition = Vector3.zero + Vector3.up * 1.2f;
			AssignMaterialSafe(pedestal, gold);

			Selection.activeGameObject = root;
			Debug.Log("Temple blockout created");
		}

		private static void CreateWall(Transform parent, Vector3 center, Vector3 size, Material mat, string name)
		{
			var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
			go.name = name;
			go.transform.SetParent(parent);
			go.transform.localPosition = center;
			go.transform.localScale = size;
			AssignMaterialSafe(go, mat);
		}

		private static void AssignMaterialSafe(GameObject go, Material mat)
		{
			var renderer = go.GetComponent<Renderer>();
			if (renderer != null && mat != null)
			{
				renderer.sharedMaterial = mat;
			}
		}
	}
}

