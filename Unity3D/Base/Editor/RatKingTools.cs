using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using RatKing.Base;

namespace RatKing {

	public class RatKingTools : EditorWindow {
		public static float snapSetting = 0.25f;
		public static Vector3 rotateAxisSetting = Vector3.up;
		public static int noiseTexSize = 128;
		public static bool noiseMono;
		public static bool copyMesh = true;
		public static float meshScaleFactor = 1f;
		public static Vector3 meshRotate = Vector3.zero;
		public static float meshSmoothFactor = 0.5f;
		//
		static Vector2 scrollPos;

		//

		[MenuItem("Tools/Rat King")]
		static void Init() {
			EditorWindow.GetWindow(typeof(RatKingTools));
		}

		//

		void OnGUI() {
			EditorGUIUtility.labelWidth = 80f;
			var btnW = GUILayout.Width(100f);
			var btnH = GUILayout.Height(40f);
			scrollPos = GUILayout.BeginScrollView(scrollPos);

			GUILayout.BeginHorizontal();
			snapSetting = EditorGUILayout.FloatField("Snap", snapSetting);
			if (GUILayout.Button("Snap!", btnW, btnH)) { Snap(); }
			GUILayout.EndHorizontal();

			GUILayout.Space(6);

			GUILayout.BeginHorizontal();
			rotateAxisSetting = EditorGUILayout.Vector3Field("Rotate Axis", rotateAxisSetting);
			if (GUILayout.Button("Random\nrotate!", btnW, btnH)) { RandomRotate(); }
			GUILayout.EndHorizontal();

			GUILayout.Space(6);

			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
			noiseTexSize = EditorGUILayout.IntField("Tex Size", noiseTexSize);
			noiseMono = EditorGUILayout.Toggle("Monochrome", noiseMono);
			GUILayout.EndVertical();
			if (GUILayout.Button("Generate\nNoise Texture", btnW, btnH)) { NoiseTexture(noiseTexSize, noiseMono); }
			GUILayout.EndHorizontal();
			
			GUILayout.Space(10);
			
			GUILayout.BeginVertical(GUI.skin.GetStyle("box"));

			copyMesh = EditorGUILayout.Toggle("COPY MESH", copyMesh);

			var guiCol = GUI.color;
			if (!copyMesh) { GUI.color = new Color(1f, 0.5f, 0.5f, 1f); }
			
			if (GUILayout.Button("Set vertex colors from texture", btnH)) { VertexColorizer(copyMesh); }
			
			GUILayout.Space(4);

			GUILayout.BeginHorizontal();
			meshScaleFactor = EditorGUILayout.FloatField("Mesh Scale", meshScaleFactor);
			if (GUILayout.Button("Scale Mesh", btnW, btnH)) {
				MeshRescaler(meshScaleFactor, copyMesh);
			}
			GUILayout.EndHorizontal();
			
			GUILayout.Space(4);

			GUILayout.BeginHorizontal();
			meshRotate = EditorGUILayout.Vector3Field("Euler angles", meshRotate);
			if (GUILayout.Button("Rotate Mesh", btnW, btnH)) {
				MeshRotator(meshRotate, copyMesh);
			}
			GUILayout.EndHorizontal();
			
			GUILayout.Space(4);

			GUILayout.BeginHorizontal();
			meshSmoothFactor = EditorGUILayout.Slider("Mesh Smooth", meshSmoothFactor, 0f, 1f);
			if (GUILayout.Button("Smooth Mesh", btnW, btnH)) {
				MeshSmoother(meshSmoothFactor, copyMesh);
			}
			GUILayout.EndHorizontal();
			
			GUILayout.Space(4);

			GUI.color = guiCol;

			if (GUILayout.Button("Export to OBJ", btnH)) {
				OBJExporter();
			}

			GUILayout.EndVertical();

			GUILayout.EndScrollView();
			EditorGUIUtility.labelWidth = 0f;
		}

		//

		void Snap() {
			foreach (var go in Selection.gameObjects) {
				var p = go.transform.position;
				p.x = (Mathf.Round(p.x / snapSetting)) * snapSetting;
				p.y = (Mathf.Round(p.y / snapSetting)) * snapSetting;
				p.z = (Mathf.Round(p.z / snapSetting)) * snapSetting;
				go.transform.position = p;
			}
			EditorGUIUtility.ExitGUI();
			return;
		}

		void RandomRotate() {
			foreach (var go in Selection.gameObjects) {
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
#if !UNITY_5_5_OR_NEWER
			textureImporter.textureFormat = TextureImporterFormat.ARGB32;
#else
				textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
#endif
				textureImporter.anisoLevel = 0;
				AssetDatabase.ImportAsset(path);

				// EditorUtility.CompressTexture(tex, TextureFormat.ARGB32, 0);
				//tex.format = TextureFormat.ARGB32;
			}
		}

		void VertexColorizer(bool copyMesh) {
			foreach (var go in Selection.gameObjects) {
				var render = go.GetComponent<Renderer>();
				if (render == null) { continue; }
				var mf = go.GetComponent<MeshFilter>();
				Mesh mesh = null;
				if (mf != null) { mesh = mf.sharedMesh; }
				else if (render is SkinnedMeshRenderer) { mesh = (render as SkinnedMeshRenderer).sharedMesh; }
				else { continue; }

				// assign vertex colors

				var materials = render.sharedMaterials;
				var colors = (mesh.colors != null && mesh.colors.Length == mesh.vertexCount) ? mesh.colors : new Color[mesh.vertexCount];
				var uv = mesh.uv;
				var allTriangles = new List<int>(mesh.vertexCount);

				for (int i = 0; i < mesh.subMeshCount; ++i) {
					var c = materials[i].GetColor("_Color"); // TODO?
					var t = materials[i].mainTexture as Texture2D;
					if (t != null) {
						var path = AssetDatabase.GetAssetPath(t);
						var ti = (TextureImporter)TextureImporter.GetAtPath(path);
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
						if (t != null) { colors[index] = t.GetPixelBilinear(uv[index].x, uv[index].y); }
						else { colors[index] = c; }
					}
				}

				// merge submeshes
				if (copyMesh) { mesh = Instantiate(mesh); }
				mesh.triangles = allTriangles.ToArray();
				mesh.colors = colors;
				mesh.subMeshCount = 1;
				render.materials = new Material[1] { materials[0] };

				if (mf != null) { mf.mesh = mesh; }
				else if (render is SkinnedMeshRenderer) { (render as SkinnedMeshRenderer).sharedMesh = mesh; }

				UnityEngine.Debug.Log("Done. Assign VertexColored material if needed.");
			}
		}

		void MeshRescaler(float factor, bool copyMesh) {
			foreach (var go in Selection.gameObjects) {
				go.transform.localScale = Vector3.one;
				go.transform.localRotation = Quaternion.identity;

				var render = go.GetComponent<Renderer>();
				if (render == null) { continue; }
				var mf = go.GetComponent<MeshFilter>();
				Mesh mesh = null;
				if (mf != null) { mesh = mf.sharedMesh; }
				else if (render is SkinnedMeshRenderer) { mesh = (render as SkinnedMeshRenderer).sharedMesh; }
				else { continue; }

				//var normals = mesh.normals;
				var vertices = mesh.vertices;

				for (int i = 0; i < mesh.vertexCount; ++i) {
					vertices[i] = vertices[i] * factor;
				}

				// merge submeshes
				if (copyMesh) { mesh = Instantiate(mesh); }
				mesh.vertices = vertices;
				mesh.RecalculateBounds();

				if (mf != null) { mf.mesh = mesh; }
				else if (render is SkinnedMeshRenderer) { (render as SkinnedMeshRenderer).sharedMesh = mesh; }

				UnityEngine.Debug.Log("Done.");
			}
		}

		void MeshRotator(Vector3 rotation, bool copyMesh) {
			foreach (var go in Selection.gameObjects) {
				go.transform.localScale = Vector3.one;
				go.transform.localRotation = Quaternion.identity;

				var render = go.GetComponent<Renderer>();
				if (render == null) { continue; }
				var mf = go.GetComponent<MeshFilter>();
				Mesh mesh = null;
				if (mf != null) { mesh = mf.sharedMesh; }
				else if (render is SkinnedMeshRenderer) { mesh = (render as SkinnedMeshRenderer).sharedMesh; }
				else { continue; }

				var normals = mesh.normals;
				var vertices = mesh.vertices;

				for (int i = 0; i < mesh.vertexCount; ++i) {
					vertices[i] = Math.Rotate(vertices[i], rotation.x, rotation.y, rotation.z);
					normals[i] = Math.Rotate(normals[i], rotation.x, rotation.y, rotation.z);
				}

				// merge submeshes
				if (copyMesh) { mesh = Instantiate(mesh); }
				mesh.vertices = vertices;
				mesh.normals = normals;
				mesh.RecalculateBounds();

				if (mf != null) { mf.mesh = mesh; }
				else if (render is SkinnedMeshRenderer) { (render as SkinnedMeshRenderer).sharedMesh = mesh; }

				UnityEngine.Debug.Log("Done.");
			}
		}

		void MeshSmoother(float factor, bool copyMesh) {
			if (factor == 0f) { return; }
			foreach (var go in Selection.gameObjects) {
				go.transform.localScale = Vector3.one;
				go.transform.localRotation = Quaternion.identity;

				var render = go.GetComponent<Renderer>();
				if (render == null) { continue; }
				var mf = go.GetComponent<MeshFilter>();
				Mesh mesh = null;
				if (mf != null) { mesh = mf.sharedMesh; }
				else if (render is SkinnedMeshRenderer) { mesh = (render as SkinnedMeshRenderer).sharedMesh; }
				else { continue; }

				if (copyMesh) { mesh = Instantiate(mesh); }

				var copy = Instantiate(mesh);
				copy.RecalculateNormals(180f);

				if (factor == 1f) {
					mesh.triangles = copy.triangles;
					mesh.vertices = copy.vertices;
					mesh.colors = copy.colors;
					mesh.normals = copy.normals;
					mesh.uv = copy.uv;
				}
				else {
					var vertices = mesh.vertices;
					var normals = mesh.normals;
					var verticesCopy = copy.vertices;
					var normalsCopy = copy.normals;

					Dictionary<Vector3Int, int> quantsCopy = new Dictionary<Vector3Int, int>();
					for (int i = 0; i < copy.vertexCount; ++i) {
						var v3i = (verticesCopy[i] * 1000f).ToVec3i();
						quantsCopy[v3i] = i;
					}

					var fail = 0;
					for (int i = 0; i < mesh.vertexCount; ++i) {
						var v3i = (vertices[i] * 1000f).ToVec3i();
						int ci;
						if (!quantsCopy.TryGetValue(v3i, out ci)) { fail++; continue; }
						//Debug.DrawRay(vertices[i], normals[i], Color.red, 5f);
						normals[i] = Vector3.Slerp(normals[i], normalsCopy[ci], factor);
						//Debug.DrawRay(vertices[i], normals[i], Color.yellow, 6f);
						//Debug.DrawRay(vertices[i], normalsCopy[ci], Color.green, 5f);
					}

					mesh.normals = normals;
				}

				if (mf != null) { mf.mesh = mesh; }
				else if (render is SkinnedMeshRenderer) { (render as SkinnedMeshRenderer).sharedMesh = mesh; }

				UnityEngine.Debug.Log("Done.");
			}
		}

		void OBJExporter() {
			foreach (var go in Selection.gameObjects) {
				OBJ.Export(go.GetComponentsInChildren<MeshFilter>(), Application.dataPath + "/__OBJ_Export", "OBJ " + go.name);
			}
		}
	}

}