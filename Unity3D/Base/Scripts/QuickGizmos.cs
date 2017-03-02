using UnityEngine;
using System.Collections.Generic;

namespace RatKing.Base {

	public class QuickGizmos : MonoBehaviour {

		public enum GizmoType { Sphere, Box, WiredSphere, WiredBox }
		struct QuickGizmo {
			public GizmoType type;
			public Matrix4x4 matrix;
			public Color color;
			public bool renderInGame;
			public QuickGizmo(GizmoType type, Vector3 position, Quaternion rotation, Vector3 scale, Color color, bool renderInGame) {
				this.type = type;
				matrix = new Matrix4x4();
				matrix.SetTRS(position, rotation, scale);
				this.color = color;
				this.renderInGame = renderInGame;
			}
		}

		static QuickGizmos inst;
		static Mesh boxMesh, sphereMesh;
		static Material material;
		static List<QuickGizmo> gizmos = new List<QuickGizmo>();
		static List<float> gizmoTime = new List<float>();

		//

		static void CreateInstance() {
			if (inst != null) { return; }
			var go = Camera.main.gameObject;
			inst = go.AddComponent<QuickGizmos>();
			var boxGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
			boxMesh = boxGO.GetComponent<MeshFilter>().mesh;
			Destroy(boxGO);
			var sphereGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			sphereMesh = sphereGO.GetComponent<MeshFilter>().mesh;
			Destroy(sphereGO);
			material = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));
		}

		//

		public static void DrawBox(Vector3 pos, Quaternion rot, Vector3 extents, Color color, float seconds = 0f, bool renderInGame = true) {
#if UNITY_EDITOR
			if (seconds < 0f || color.a <= 0f) { return; }
			CreateInstance();
			gizmos.Add(new QuickGizmo(GizmoType.Box, pos, rot, extents * 2f, color, renderInGame));
			gizmoTime.Add(Time.time + seconds);
			if (gizmos.Count == 1) { inst.enabled = true; }
#endif
		}

		public static void DrawWiredBox(Vector3 pos, Quaternion rot, Vector3 extents, Color color, float seconds = 0f) {
#if UNITY_EDITOR
			if (seconds < 0f || color.a <= 0f) { return; }
			CreateInstance();
			gizmos.Add(new QuickGizmo(GizmoType.WiredBox, pos, rot, extents * 2f, color, false));
			gizmoTime.Add(Time.time + seconds);
			if (gizmos.Count == 1) { inst.enabled = true; }
#endif
		}

		public static void DrawSphere(Vector3 pos, float radius, Color color, float seconds = 0f, bool renderInGame = true) {
#if UNITY_EDITOR
			if (seconds < 0f || color.a <= 0f) { return; }
			CreateInstance();
			gizmos.Add(new QuickGizmo(GizmoType.Sphere, pos, Quaternion.identity, Vector3.one * radius, color, renderInGame));
			gizmoTime.Add(Time.time + seconds);
			if (gizmos.Count == 1) { inst.enabled = true; }
#endif
		}

		public static void DrawWiredSphere(Vector3 pos, float radius, Color color, float seconds = 0f) {
#if UNITY_EDITOR
			if (seconds < 0f || color.a <= 0f) { return; }
			CreateInstance();
			gizmos.Add(new QuickGizmo(GizmoType.WiredSphere, pos, Quaternion.identity, Vector3.one * radius, color, false));
			gizmoTime.Add(Time.time + seconds);
			if (gizmos.Count == 1) { inst.enabled = true; }
#endif
		}

		//

		/// <summary>
		/// check life times
		/// </summary>
		void LateUpdate() {
			for (int i = gizmos.Count - 1; i >= 0; --i) {
				if (gizmoTime[i] < Time.time) {
					gizmos.RemoveAt(i);
					gizmoTime.RemoveAt(i);
					if (gizmos.Count == 0) { inst.enabled = false; }
				}
			}
		}

		/// <summary>
		/// Render the ingame gizmos
		/// </summary>
		public void OnPostRender() {
			for (int i = gizmos.Count - 1; i >= 0; --i) {
				var g = gizmos[i];
				if (!g.renderInGame) { continue; }
				switch (g.type) {
					case GizmoType.Box:
						material.SetColor("_Color", g.color);
						material.SetPass(0);
						Graphics.DrawMeshNow(boxMesh, g.matrix);
						break;
					case GizmoType.Sphere:
						material.SetColor("_Color", g.color);
						material.SetPass(0);
						Graphics.DrawMeshNow(sphereMesh, g.matrix);
						break;
				}
			}
		}

		/// <summary>
		/// Render the ineditor gizmos
		/// </summary>
		void OnDrawGizmos() {
			if (inst == null) { return; }
			for (int i = gizmos.Count - 1; i >= 0; --i) {
				var g = gizmos[i];
				Gizmos.matrix = g.matrix;
				Gizmos.color = g.color;
				switch (g.type) {
					case GizmoType.Box: Gizmos.DrawCube(Vector3.zero, Vector3.one); break;
					case GizmoType.WiredBox: Gizmos.DrawWireCube(Vector3.zero, Vector3.one); break;
					case GizmoType.Sphere: Gizmos.DrawSphere(Vector3.zero, 1f); break;
					case GizmoType.WiredSphere: Gizmos.DrawWireSphere(Vector3.zero, 1f); break;
				}
			}
		}
	}

}