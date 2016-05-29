using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class RatKingTools : EditorWindow {
	public static float snapSetting = 0.25f;
	public static Vector3 rotateAxisSetting = Vector3.up;
	public static int noiseTexSize = 128;
	public static bool noiseMono;

	//

	[MenuItem("Tools/Rat King")]
	static void Init() {
		EditorWindow.GetWindow(typeof(RatKingTools));
	}

	//

	void OnGUI() {
		snapSetting = EditorGUILayout.FloatField("Snap", snapSetting);
		if (GUILayout.Button("Snap objects")) {
			Snap();
		}
		
		GUILayout.Space(6);

		rotateAxisSetting = EditorGUILayout.Vector3Field("Rotate Axis", rotateAxisSetting);
		if (GUILayout.Button("Random rotate objects")) {
			RandomRotate();
		}

		GUILayout.Space(6);
		
		noiseTexSize = EditorGUILayout.IntField("Tex Size", noiseTexSize);
		noiseMono = EditorGUILayout.Toggle("Monochrome", noiseMono);
		if (GUILayout.Button("Generate Noise Texture")) {
			NoiseTexture(noiseTexSize, noiseMono);
		}

		GUILayout.Space(6);

		if (GUILayout.Button("Set vertex colors\nDANGER: Changes the mesh"))
			VertexColorizer();
	}

	//

	void Snap() {
		foreach (GameObject go in Selection.gameObjects) {
			Vector3 p = go.transform.position;
			p.x = (Mathf.Round(p.x / snapSetting)) * snapSetting;
			p.y = (Mathf.Round(p.y / snapSetting)) * snapSetting;
			p.z = (Mathf.Round(p.z / snapSetting)) * snapSetting;
			go.transform.position = p;
		}
		EditorGUIUtility.ExitGUI();
		return;
	}

	void RandomRotate() {
		foreach (GameObject go in Selection.gameObjects) {
			go.transform.Rotate(rotateAxisSetting, Random.Range(0f, 360f));
		}
		EditorGUIUtility.ExitGUI();
		return;
	}

	void NoiseTexture(int size, bool mono) {
		var path = EditorUtility.SaveFilePanel("Save Noise Texture", "Assets", "noise" + size, "png");
		if (path != "") {
			var tex = new Texture2D(size, size, TextureFormat.ARGB32, false);
			var s2 = size * size;
			var cols = new Color32[s2];
			for (int i = 0; i < s2; ++i) {
				if (mono) {
					var r = (byte)Random.Range(0, 256);
					cols[i] = new Color32(r, r, r, 255);
				}
				else {
					cols[i] = new Color32((byte)Random.Range(0, 256), (byte)Random.Range(0, 256), (byte)Random.Range(0, 256), 255);
				}
			}
			tex.SetPixels32(cols);
			tex.Apply();
			System.IO.File.WriteAllBytes(path, tex.EncodeToPNG());
			AssetDatabase.Refresh();
			Object.DestroyImmediate(tex);
			path = "Assets" + path.Remove(0, Application.dataPath.Length);
			// tex = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));

			TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
			textureImporter.textureFormat = TextureImporterFormat.ARGB32;
			textureImporter.anisoLevel = 0;
			AssetDatabase.ImportAsset(path);

			// EditorUtility.CompressTexture(tex, TextureFormat.ARGB32, 0);
			//tex.format = TextureFormat.ARGB32;
		}
	}

	void VertexColorizer() {
		foreach (GameObject go in Selection.gameObjects) {
			Renderer r = go.GetComponent<Renderer>();
			if (r == null)
				continue;

			Mesh mesh = null;
			if (go.GetComponent<MeshFilter>() != null)
				mesh = go.GetComponent<MeshFilter>().sharedMesh;
			else if (r is SkinnedMeshRenderer)
				mesh = (r as SkinnedMeshRenderer).sharedMesh;
			else
				continue;

			// assign vertex colors

			Material[] materials = r.sharedMaterials;
			Color[] colors = (mesh.colors != null && mesh.colors.Length == mesh.vertexCount) ? mesh.colors : new Color[mesh.vertexCount];
			Vector2[] uv = mesh.uv;
			List<int> allTriangles = new List<int>(mesh.vertexCount);

			for (int i = 0; i < mesh.subMeshCount; ++i) {
				Color c = materials[i].GetColor("_Color"); // TODO?
				Texture2D t = materials[i].mainTexture as Texture2D;
				if (t != null) {
					string path = AssetDatabase.GetAssetPath(t);
					TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(path);
					if (!ti.isReadable) {
						UnityEngine.Debug.Log("texture " + t.name + " was not readable");
						ti.isReadable = true;
						AssetDatabase.ImportAsset(path);
					}
				}
				int[] triangles = mesh.GetTriangles(i);
				allTriangles.AddRange(triangles);
				int count = triangles.Length;
				for (int j = 0; j < count; ++j) {
					int index = triangles[j];
					if (t != null)
						colors[index] = t.GetPixelBilinear(uv[index].x, uv[index].y);
					else
						colors[index] = c;
				}
			}
			
			// merge submeshes
			mesh.triangles = allTriangles.ToArray();
			mesh.subMeshCount = 1;
			r.materials = new Material[1] { materials[0] };
			
			mesh.colors = colors;
			if (go.GetComponent<MeshFilter>() != null)
				go.GetComponent<MeshFilter>().mesh = mesh;
			else if (r is SkinnedMeshRenderer)
				(r as SkinnedMeshRenderer).sharedMesh = mesh;

			UnityEngine.Debug.Log("Done. Assign VertexColored material if needed.");
		}
	}
}
