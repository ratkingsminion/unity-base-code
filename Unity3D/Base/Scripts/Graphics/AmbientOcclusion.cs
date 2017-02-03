// #define DEBUG_AO

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// based on the code from Adrian Myers, presented at https://www.youtube.com/watch?v=eeKQAXg-Qo8

namespace RatKing.Base {

	public class AmbientOcclusion {
		public enum NormalAveragation {
			None,
			Standard,
			VisibleOnly
		}
		public static int samples = 64;
		public static float maxRange = 1.5f;
		public static float minRange = 1e-10f;
		public static float intensity = 1.5f;
		public static bool resetExistingAlpha = true;
		public static NormalAveragation averageNormals = NormalAveragation.VisibleOnly;
		public static bool averageColors;
		public static int layerMask = ~0;
		public static Color color = Color.black;
		//
		static RaycastHit hit = new RaycastHit();

		//

		struct MidPoint {
			public List<Vector3> points;
			public Vector3 point;
		}

		struct Vertex {
			public int index;
			public TempInfo info;
			public Vertex(int index, TempInfo info) { this.index = index; this.info = info; }
			//
			public static bool operator ==(Vertex a, Vertex b) { return a.index == b.index && a.info == b.info; }
			public static bool operator !=(Vertex a, Vertex b) { return a.index != b.index || a.info != b.info; }
			public override bool Equals(object o) { return (o is Vertex) && (Vertex)o == this; }
			public override int GetHashCode() { return base.GetHashCode(); }
			public override string ToString() { return index + ", " + info.trans.name; }
		}

		class TempInfo {
			public Mesh mesh;
			public Transform trans;
			public TempInfo(Mesh mesh, Transform trans) { this.mesh = mesh; this.trans = trans; }
			public Vector3[] verts;
			public Vector3[] origWorldNrms;
			public Vector3[] avgWorldNrms;
			public MidPoint[] midPoints;
			public Dictionary<Base.Position3, List<int>> pointsByPos;
			public List<TempInfo> neighbours = new List<TempInfo>();
		}

		//

		/// <summary>
		/// Gather the temporary info (mid points) of a mesh
		/// </summary>
		/// <param name="mesh"></param>
		/// <param name="trans"></param>
		static TempInfo GatherTempInfo(Mesh mesh, Transform trans) {
			var ti = new TempInfo(mesh, trans);

			int i, j, l = mesh.vertexCount;

			if (mesh.normals.Length == 0) {
				// generate normals if there are none
				mesh.RecalculateNormals();
				ti.origWorldNrms = mesh.normals;
			}
			else if (averageNormals == NormalAveragation.Standard) {
				// If we're using averaged normals, a proxy mesh is used to calculate these so the original normals stay unchanged
				Mesh clonemesh = new Mesh();
				clonemesh.vertices = mesh.vertices;
				clonemesh.normals = mesh.normals;
				clonemesh.tangents = mesh.tangents;
				clonemesh.triangles = mesh.triangles;
				clonemesh.RecalculateBounds();
				clonemesh.RecalculateNormals();
				ti.origWorldNrms = clonemesh.normals;
				Object.DestroyImmediate(clonemesh);
			}
			else {
				// Otherwise, just use the originals
				ti.origWorldNrms = mesh.normals;
			}

			ti.verts = mesh.vertices;
			for (i = 0; i < l; ++i) {
				ti.verts[i] = ti.trans.TransformPoint(ti.verts[i]);
				ti.origWorldNrms[i] = ti.trans.TransformDirection(ti.origWorldNrms[i]);
			}

			// Get and points by pos
			ti.midPoints = new MidPoint[l];
			ti.pointsByPos = new Dictionary<Base.Position3, List<int>>();
			for (i = 0; i < l; ++i) {
				ti.midPoints[i].points = new List<Vector3>();
				var pos3 = Base.Position3.RoundedVector(ti.verts[i] * 100f);
				List<int> pbp;
				if (!ti.pointsByPos.TryGetValue(pos3, out pbp)) { ti.pointsByPos.Add(pos3, pbp = new List<int>(2)); }
				pbp.Add(i);
			}
			// get midpoints (triangles)
			var tris = mesh.triangles;
			var trisNum = tris.Length; // TODO: maybe one triangle per point is enough!
			for (i = 0; i < trisNum; i += 3) {
				var v0 = tris[i + 0];
				var v1 = tris[i + 1];
				var v2 = tris[i + 2];
				var middle = (ti.verts[v0] + ti.verts[v1] + ti.verts[v2]) * 0.3333333333f;
				ti.midPoints[v0].points.Add(middle);
				ti.midPoints[v1].points.Add(middle);
				ti.midPoints[v2].points.Add(middle);
			}
			for (i = 0; i < l; ++i) {
				for (j = ti.midPoints[i].points.Count - 1; j >= 0; --j) {
					ti.midPoints[i].point += ti.midPoints[i].points[j];
				}
				ti.midPoints[i].point *= 1f / ti.midPoints[i].points.Count;
#if UNITY_EDITOR && DEBUG_AO
				/// Debug.DrawRay(ti.midPoints[i].point, ti.worldNormals[i], Color.white, 2f);
#endif
			}
			return ti;
		}

		/// <summary>
		/// Only direct neighbours (or overlapping meshes) can share vertices
		/// thus we find out whose bounds are intersecting
		/// </summary>
		/// <param name="infos"></param>
		static void FindNeighbours(IEnumerable<TempInfo> infos) {
			for (var ti = infos.GetEnumerator(); ti.MoveNext();) {
				var info_i = ti.Current;
				var coll_i = info_i.trans.GetComponent<Collider>();
				for (var tj = infos.GetEnumerator(); tj.MoveNext();) {
					var info_j = tj.Current;
					if (info_i == info_j) { continue; }
					var coll_j = info_j.trans.GetComponent<Collider>();
					if (coll_i.bounds.Intersects(coll_j.bounds)) {
						info_i.neighbours.Add(info_j);
					}
				}
			}
		}

		/// <summary>
		/// Average the normals of the mesh, but check with with all neighbour meshes
		/// </summary>
		/// <param name="ti"></param>
		static void AverageNormals(TempInfo ti) {
			int i, j, l = ti.verts.Length;
			ti.avgWorldNrms = new Vector3[l];
			// create the sets of points which "see" each other
			var psets = new List<HashSet<Vertex>>();
			for (var pbp_iter = ti.pointsByPos.GetEnumerator(); pbp_iter.MoveNext();) {
				var pos3 = pbp_iter.Current.Key;
				var list = pbp_iter.Current.Value;
				var count = list.Count - 1;
				for (i = count; i >= 0; --i) {
					int li = list[i];
					var mni = ti.midPoints[li].point + ti.origWorldNrms[li] * 0.01f; // moved point
					var mni_i = ti.midPoints[li].point - ti.origWorldNrms[li] * 0.01f; // inverse moved point
					var vi = new Vertex(li, ti);
					var curSet = psets.Find(v => v.Contains(vi));
					if (curSet == null) {
						psets.Add(curSet = new HashSet<Vertex>());
					}
					curSet.Add(new Vertex(li, ti));
					// check point with own points
					for (j = count; j >= 0; --j) {
						int lj = list[j];
						if (li != lj) {
							var mnj = ti.midPoints[lj].point + ti.origWorldNrms[lj] * 0.01f; // moved point
							var mnj_i = ti.midPoints[lj].point - ti.origWorldNrms[lj] * 0.01f; // inverse moved point
							if ((mni - mnj).sqrMagnitude <= (mni_i - mnj_i).sqrMagnitude && !Physics.Linecast(mni, mnj, layerMask)) {
								// same set if seeing.
								curSet.Add(new Vertex(lj, ti));
#if UNITY_EDITOR && DEBUG_AO
								Color c = new Color(Random.value, Random.value, Random.value);
								Debug.DrawLine(ti.verts[li], mnj + (mni - mnj) * 0.5f, c, 5f);
								Debug.DrawLine(ti.midPoints[li].point, mni, c, 5f);
								Debug.DrawLine(ti.midPoints[lj].point, mnj, c, 5f);
								Debug.DrawLine(mni, mnj, c, 5f);
#endif
							}
						}
					}
					// check point with neighbours
					for (var nbr_iter = ti.neighbours.GetEnumerator(); nbr_iter.MoveNext();) {
						var nbr = nbr_iter.Current;
						List<int> list_n;
						if (nbr.pointsByPos.TryGetValue(pos3, out list_n)) {
							for (j = list_n.Count - 1; j >= 0; --j) {
								int lj = list_n[j];
								var mnj = nbr.midPoints[lj].point + nbr.origWorldNrms[lj] * 0.01f; // moved point
								var mnj_i = nbr.midPoints[lj].point - nbr.origWorldNrms[lj] * 0.01f; // inverse moved point
								if ((mni - mnj).sqrMagnitude <= (mni_i - mnj_i).sqrMagnitude && !Physics.Linecast(mni, mnj, layerMask)) {
									// same set if seeing.
									curSet.Add(new Vertex(lj, nbr));
#if UNITY_EDITOR && DEBUG_AO
									Color c = new Color(Random.value, Random.value, Random.value);
									Debug.DrawLine(ti.verts[li], mnj + (mni - mnj) * 0.5f, c, 5f);
									Debug.DrawLine(ti.midPoints[li].point, mni, c, 5f);
									Debug.DrawLine(nbr.midPoints[lj].point, mnj, c, 5f);
									Debug.DrawLine(mni, mnj, c, 5f);
#endif
								}
							}
						}
					}
				}
			}
			// join sets that share (somehow) vertices
			l = psets.Count;
			for (i = l - 1; i >= 0; --i) {
				if (psets[i] == null) { continue; }
				for (j = l - 1; j >= 0; --j) {
					if (i != j && psets[j] != null && psets[i].Overlaps(psets[j])) {
						psets[i].UnionWith(psets[j]);
						psets[j] = null;
					}
				}
			}
			psets.RemoveAll(v => v == null);
#if UNITY_EDITOR && DEBUG_AO
			Debug.Log("Points by Pos: " + ti.pointsByPos.Count + " ---  Sets before join: " + l + " - after: " + psets.Count + " --- Overall: " + ti.verts.Length);
#endif
			// the sets now do the normal sharing -> just add them together and normalize them afterwards
			for (var sets_iter = psets.GetEnumerator(); sets_iter.MoveNext();) {
				for (var outer_iter = sets_iter.Current.GetEnumerator(); outer_iter.MoveNext();) {
					var vi = outer_iter.Current;
					if (vi.info == ti) {
						for (var inner_iter = sets_iter.Current.GetEnumerator(); inner_iter.MoveNext();) {
							var vj = inner_iter.Current;
							ti.avgWorldNrms[vi.index] += vj.info.origWorldNrms[vj.index];
						}
					}
				}
			}
			l = ti.verts.Length;
			for (i = 0; i < l; ++i) {
				// ti.avgWorldNrms[i] = ti.avgWorldNrms[i].sqrMagnitude > 0.0001f ? ti.avgWorldNrms[i].normalized : Random.onUnitSphere;
				ti.avgWorldNrms[i] = ti.avgWorldNrms[i].normalized;
#if UNITY_EDITOR && DEBUG_AO
				Debug.DrawRay(ti.verts[i], ti.avgWorldNrms[i], Color.white, 3f);
#endif
			}
		}

		/// <summary>
		/// Simulate ambient occlusion by shooting many rays (physics)
		/// </summary>
		/// <param name="ti"></param>
		/// <param name="rays"></param>
		static void ShootRays(TempInfo ti, Vector3[] rays) {
			// Loop over the verts and perform basic, slow, horrible AO based on Physics.Raycast
			int i, j, l = ti.verts.Length;

			Color[] colors = ti.mesh.colors;
			if (colors.Length == 0) {
				colors = new Color[l];
			}

			// Initialize alpha values, necessary unless going for complex effect or some kind of manual multipass use
			if (resetExistingAlpha) {
				for (i = 0; i < colors.Length; i++) {
					colors[i].a = 1f;
					}
			}

			//DRCHitInfo hitInfo = null;
			for (i = 0; i < l; i++) {
				// Store vert in world space
				Vector3 v = ti.verts[i]; // ti.trans.TransformPoint(ti.verts[i]);
				// Store normal offset in world space (displacement from vertex position)
				Vector3 n = averageNormals == NormalAveragation.VisibleOnly ? ti.avgWorldNrms[i] : ti.origWorldNrms[i];
				if (n.sqrMagnitude <= 0.000001f) {
					continue;
				}
				n += ti.verts[i];
				// Store world-space normal
				Vector3 wnrm = (n-v).normalized;
				// Get the displacement between a random, upward-facing ray, and the world-space surface normal
				// Quaternion dirq = Quaternion.FromToRotation(Vector3.up, wnrm);

				// Total occlusion at this vertex
				float occ = 0;
				int usedSamples = 0;
				// Main loop, take samples up to the limit
				for (j = 0; j < samples; j++) {

					var dot = Vector3.Dot(rays[j], wnrm);
					//if (Vector3.Angle(rays[j], wnrm) > 90f) {
					if (dot < 0f) {
						// Debug.DrawRay(v, rays[j], new Color(0f, 0f, 0f, 0.1f), 1f);
						continue;
					}
					usedSamples++;

					// Move a bit in the reflected direction from ray, to give a little bit of space against a shared wall or something
					Vector3 offset = Vector3.Reflect(rays[j], wnrm) * 0.05f;
					//Vector3 offset = rays[j] * 0.1f;

					//if (DRCS.RayCast(new DRCRay(v - offset, rays[j]), maxRange, ref hitInfo, true)) {
					if (Physics.Raycast(v - offset, rays[j], out hit, maxRange, layerMask)) {
						// You'd add a dot product test here or after the distance test, if you wanted one
						// This would only be for special effects or unusual cases
						//float dot = Vector3.Dot(nrm, hit.normal);

						// Reject any degenerate tests
						//if (hitInfo.hitDistance > minRange) {
						if (hit.distance > minRange) {
							// Occlusion Value depends on collision distance relative to max range
							// If you want a simpler AO effect, just use occ++
							//occ += Mathf.Clamp01(1f - (hitInfo.hitDistance / maxRange));
							occ += Mathf.Clamp01(1f - (hit.distance / maxRange));
							//occ++;
#if UNITY_EDITOR && DEBUG_AO
							Debug.DrawLine(v - offset, hit.point, new Color(0f, 1f, 0f, 0.5f), 1f);
#endif
						}
					}
					//else {
					//	Debug.DrawRay(v - offset, rays[j], new Color(1f, 0f, 0f, 0.15f), 1f);
					//}

					// Update the progress bar periodically
					/*if (++sample % 500 == 0) {
						EditorUtility.DisplayProgressBar(
							"VERTEX AO",
							"Calculating...",
							(float)sample / (float)numSamples
						);
					}*/
				}

				// Modulate occlusion by sample count and intensity, and flip
				occ = Mathf.Clamp01(1f - ((occ * intensity) / usedSamples));

				// Any given color entry should only be processed once.
				// If alpha values were reset, you can just copy occ (since it's just overwriting 1)
				// Otherwise, multiply the values to allow for multi-pass use, although that would take some doing on the user's part
				//if (resetExistingAlpha)
				//	colors[ i ].a = occ;
				//else
				colors[i].a = colors[i].a * occ;

				// if (i % 150 == 0) yield return null;
			} // verts

			// Optional averaging pass
			if (averageColors) {
				var tris = ti.mesh.triangles;
				var trisNum = tris.Length;
				for (i = 0; i < trisNum; i += 3) {
					int vi0 = tris[i+0], vi1 = tris[i+1], vi2 = tris[i+2];
					Color c0 = colors[vi0], c1 = colors[vi1], c2 = colors[vi2];
					float a = (c0.a + c1.a + c2.a) / 3;
					c0.a = c0.a + (a - c0.a) / 2;
					c1.a = c1.a + (a - c1.a) / 2;
					c2.a = c2.a + (a - c2.a) / 2;
					colors[vi0] = c0; colors[vi1] = c1; colors[vi2] = c2;
				}
			}

			// change alpha to rgb
			for (i = 0; i < l; ++i) {
				colors[i] = Color.Lerp(color, Color.white, colors[i].a);
				colors[i].a = 1f;
			}

			// finally apply colors
			ti.mesh.colors = colors;
		}

		//

		static MovementEffects.CoroutineHandle handle;
		static int subNumOverall, subNumDone;

		static void DrawBar() {
			var w = Mathf.Floor(Screen.width * 0.5f);
			var h = Mathf.Floor(Screen.height * 0.75f);
			GUI.color = Color.white;
			GUI.DrawTexture(new Rect(w - 100f, h, 200f, 16f), Base.QuickIMGUI.texWhiteTransparent);
			GUI.color = Color.green;
			GUI.DrawTexture(new Rect(w - 98f, h + 2f, 196f * (subNumDone / (float)subNumOverall), 12f), Base.QuickIMGUI.texWhite);
		}

		public static void Generate(GameObject[] subjects) {
			if (MovementEffects.Timing.KillCoroutines(handle) == 0) {
				Base.QuickIMGUI.Add(DrawBar);
			}
			handle = MovementEffects.Timing.RunCoroutine(GenerateCR(subjects), MovementEffects.Segment.Update);
		}
		static IEnumerator<float> GenerateCR(GameObject[] subjects) {
			subNumDone = 0;
			subNumOverall = subjects.Length;
			var infos = new List<TempInfo>(subNumOverall);
			for (int s = 0; s < subNumOverall; ++s) {
				var sub = subjects[s];
				if (sub == null) { continue;  }
				var filters = sub.GetComponentsInChildren<MeshFilter>();
				for (var iter = filters.GetEnumerator(); iter.MoveNext();) {
					var mf = (MeshFilter)iter.Current;
					var info = GatherTempInfo(mf.sharedMesh, mf.transform);
					infos.Add(info);
				}
			}
			FindNeighbours(infos);

			//Random.seed = (int)(p.x * 20000 + p.y * 2000 + p.z * 10); // Random.Range(0, int.MaxValue);
			var rays = new Vector3[samples];
			for (int i = 0; i < samples; ++i) {
				// TODO: actually non-random distribution?
				rays[i] = Random.onUnitSphere * maxRange;
			}

			for (var info_iter = infos.GetEnumerator(); info_iter.MoveNext();) {
				subNumDone++;
				var m = info_iter.Current.mesh;
				if (m.vertexCount > 0 && m.colors32.Length == 0) {
					if (averageNormals == NormalAveragation.VisibleOnly) {
						AverageNormals(info_iter.Current);
					}
					ShootRays(info_iter.Current, rays);
					yield return 0f;
				}
			}

			Base.QuickIMGUI.Remove(DrawBar);
		}
	}

}