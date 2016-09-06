using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base.Helpers {
	
	public static class OBJ {

		struct VertexPosAndCol : System.IEquatable<VertexPosAndCol> {
			public int x, y, z;
			public int r, g, b;
			public VertexPosAndCol(double x, double y, double z, double r, double g, double b) {
				this.x = (int)System.Math.Round(x * 10000.0);
				this.y = (int)System.Math.Round(y * 10000.0);
				this.z = (int)System.Math.Round(z * 10000.0);
				this.r = (int)System.Math.Round(r * 1000.0);
				this.g = (int)System.Math.Round(g * 1000.0);
				this.b = (int)System.Math.Round(b * 1000.0);
			}
			public VertexPosAndCol(double x, double y, double z) {
				this.x = (int)System.Math.Round(x * 10000.0);
				this.y = (int)System.Math.Round(y * 10000.0);
				this.z = (int)System.Math.Round(z * 10000.0);
				r = g = b = 0;
			}
			public VertexPosAndCol(Vector3 v) {
				x = (int)System.Math.Round(v.x * 10000.0);
				y = (int)System.Math.Round(v.y * 10000.0);
				z = (int)System.Math.Round(v.z * 10000.0);
				r = g = b = 0;
			}
			public void SetColor(double r, double g, double b) {
				this.r = (int)System.Math.Round(r * 1000.0);
				this.g = (int)System.Math.Round(g * 1000.0);
				this.b = (int)System.Math.Round(b * 1000.0);
			}
			public void SetColor(Color c) {
				r = (int)System.Math.Round(c.r * 1000.0);
				g = (int)System.Math.Round(c.g * 1000.0);
				b = (int)System.Math.Round(c.b * 1000.0);
			}
			public static bool operator ==(VertexPosAndCol a, VertexPosAndCol b) { return a.x == b.x && a.y == b.y && a.z == b.z && a.r == b.r && a.g == b.g && a.b == b.b; }
			public static bool operator !=(VertexPosAndCol a, VertexPosAndCol b) { return a.x != b.x || a.y != b.y || a.z != b.z || a.r != b.r || a.g != b.g || a.b != b.b; }
			public override bool Equals(object o) { var v = (VertexPosAndCol)o; return v == this; }
			public override int GetHashCode() { return base.GetHashCode(); } // TODO change
			public override string ToString() { return x + "," + y + "," + z + " - " + r + "," + g + "," + b; }
			public bool Equals(VertexPosAndCol other) { return x == other.x && y == other.y && z == other.z && r == other.r && g == other.g && b == other.b; }
		}

		struct VertexNormal : System.IEquatable<VertexNormal> {
			public int x, y, z;
			public VertexNormal(double x, double y, double z) {
				this.x = (int)System.Math.Round(x * 10000.0);
				this.y = (int)System.Math.Round(y * 10000.0);
				this.z = (int)System.Math.Round(z * 10000.0);
			}
			public VertexNormal(Vector3 v) {
				x = (int)System.Math.Round(v.x * 10000.0);
				y = (int)System.Math.Round(v.y * 10000.0);
				z = (int)System.Math.Round(v.z * 10000.0);
			}
			public static bool operator ==(VertexNormal a, VertexNormal b) { return a.x == b.x && a.y == b.y && a.z == b.z; }
			public static bool operator !=(VertexNormal a, VertexNormal b) { return a.x != b.x || a.y != b.y || a.z != b.z; }
			public override bool Equals(object o) { var v = (VertexNormal)o; return v == this; }
			public override int GetHashCode() { return base.GetHashCode(); } // TODO change
			public override string ToString() { return x + "," + y + "," + z; }
			public bool Equals(VertexNormal other) { return x == other.x && y == other.y && z == other.z; }
		}

		struct VertexTexCoord : System.IEquatable<VertexTexCoord> {
			public int x, y;
			public VertexTexCoord(double x, double y) {
				this.x = (int)System.Math.Round(x * 10000.0);
				this.y = (int)System.Math.Round(y * 10000.0);
			}
			public VertexTexCoord(Vector2 v) {
				x = (int)System.Math.Round(v.x * 10000.0);
				y = (int)System.Math.Round(v.y * 10000.0);
			}
			public static bool operator ==(VertexTexCoord a, VertexTexCoord b) { return a.x == b.x && a.y == b.y; }
			public static bool operator !=(VertexTexCoord a, VertexTexCoord b) { return a.x != b.x || a.y != b.y; }
			public override bool Equals(object o) { var v = (VertexTexCoord)o; return v == this; }
			public override int GetHashCode() { return base.GetHashCode(); } // TODO change
			public override string ToString() { return x + "," + y; }
			public bool Equals(VertexTexCoord other) { return x == other.x && y == other.y; }
		}

		//

		public static void Export(MeshFilter[] filters, string path, string filename) {
			// TODO: allow merging for objects with shared materials

			var mtls = new List<Material>();

			var objStr = new System.Text.StringBuilder();
			objStr.Append("# Exported from Morited\n");
			objStr.Append("s off\n");
			objStr.Append("mtllib ").Append(filename).Append(".mtl\n");

			// the meshes

			int sharedVertsCount = 0;
			int sharedUVsCount = 0;
			int sharedNormsCount = 0;
			for (int i = 0; i < filters.Length; ++i) {
				var f = filters[i];
				var r = f.GetComponent<Renderer>();
				var m = f.mesh;

				if (r == null || m == null || m.vertexCount == 0) { continue; }

				// NAME
				objStr.Append("g ").Append(f.name).Append('\n');
				objStr.Append("o ").Append(f.name).Append('\n'); // blender and max treat those differently somehow, just add both ...

				if (r.sharedMaterials.Length > 1 && r.sharedMaterials[0] != null) {
					objStr.Append("usemtl ").Append(r.sharedMaterials[0].name).Append('\n');
					if (!mtls.Contains(r.sharedMaterials[0])) { mtls.Add(r.sharedMaterials[0]); }
				}
				else if (r.sharedMaterial != null) {
					objStr.Append("usemtl ").Append(r.sharedMaterial.name).Append('\n');
					if (!mtls.Contains(r.sharedMaterial)) { mtls.Add(r.sharedMaterial); }
				}

				// VERTICES AND COLORS
				var vertices = m.vertices;
				var colors = m.colors;
				var hasColors = colors != null && colors.Length == m.vertexCount;
				// find shared vertex positions and colors, write them
				var sharedVerts = new List<VertexPosAndCol>(m.vertexCount);
				var sharedVertsIndices = new int[m.vertexCount];
				for (int j = 0; j < m.vertexCount; ++j) {
					var v = f.transform.TransformPoint(vertices[j]);
					var p = new VertexPosAndCol(v);
					if (hasColors) { p.SetColor(colors[j]); }
					var index = sharedVerts.IndexOf(p);
					if (index < 0) {
						index = sharedVerts.Count;
						sharedVerts.Add(p);
						objStr.Append("v ").Append(v.x).Append(' ').Append(v.y).Append(' ').Append(v.z);
						if (hasColors) {
							objStr.Append(" 1.0 ").Append(colors[j].r).Append(' ').Append(colors[j].g).Append(' ').Append(colors[j].b);
						}
						objStr.Append('\n');
					}
					sharedVertsIndices[j] = index;
				}

				// TEXTURE COORDS
				var uv = m.uv;
				// find shared tex coords
				var sharedUVs = new List<VertexTexCoord>(m.vertexCount);
				var sharedUVsIndices = new int[m.vertexCount];
				for (int j = 0; j < m.vertexCount; ++j) {
					var u = uv[j];
					var p = new VertexTexCoord(u);
					var index = sharedUVs.IndexOf(p);
					if (index < 0) {
						index = sharedUVs.Count;
						sharedUVs.Add(p);
						objStr.Append("vt ").Append(u.x).Append(' ').Append(u.y).Append('\n');
					}
					sharedUVsIndices[j] = index;
				}

				// NORMALS
				var normals = m.normals;
				// find shared normals
				var sharedNorms = new List<VertexNormal>(m.vertexCount);
				var sharedNormsIndices = new int[m.vertexCount];
				for (int j = 0; j < m.vertexCount; ++j) {
					var n = f.transform.TransformDirection(normals[j]);
					var p = new VertexNormal(n);
					var index = sharedNorms.IndexOf(p);
					if (index < 0) {
						index = sharedNorms.Count;
						sharedNorms.Add(p);
						objStr.Append("vn ").Append(n.x).Append(' ').Append(n.y).Append(' ').Append(n.z).Append('\n');
					}
					sharedNormsIndices[j] = index;
				}

				// FACES
				var triangles = m.triangles;
				var triCount = triangles.Length;
				for (int j = 0; j < triCount; ) {
					objStr.Append("f ");
					for (int k = 0; k < 3; ++k, ++j) {
						var t = triangles[j];
						objStr.Append(sharedVertsCount + sharedVertsIndices[t] + 1).Append('/')
							  .Append(sharedUVsCount + sharedUVsIndices[t] + 1).Append('/')
							  .Append(sharedNormsCount + sharedNormsIndices[t] + 1);
						objStr.Append(k == 2 ? '\n' : ' ');
					}
				}

				// Debug.Log(filters[i].name + " vertices: " + m.vertexCount + " -> " + sharedVerts.Count + " --- normals -> " + sharedNorms.Count);

				sharedVertsCount += sharedVerts.Count;
				sharedUVsCount += sharedUVs.Count;
				sharedNormsCount += sharedNorms.Count;
			}
			
			if (!System.IO.Directory.Exists(path)) {
				System.IO.Directory.CreateDirectory(path);
			}

			System.IO.File.WriteAllText(path + "/" + filename + ".obj", objStr.ToString());
			// Debug.Log(objStr.ToString());

			//

			if (!System.IO.File.Exists(path + "/" + filename + ".mtl")) {
				System.IO.File.Delete(path + "/" + filename + ".mtl");
			}

			var invalidChars = System.IO.Path.GetInvalidFileNameChars();
			var textures = new List<Texture2D>();

			if (mtls.Count > 0) {
				var mtlStr = new System.Text.StringBuilder();
				mtlStr.Append("# Exported from Morited\n");
				for (int i = 0; i < mtls.Count; ++i) {
					var m = mtls[i];
					mtlStr.Append("newmtl ").Append(m.name).Append('\n'); // TODO: same names?
					mtlStr.Append("Ns 0\nd 1\nTr 0\n");
					mtlStr.Append("Ks 0.0 0.0 0.0\nNs 10.0\n");
					try {
						var c = m.color;
						mtlStr.Append("Ka ").Append(c.r).Append(' ').Append(c.g).Append(' ').Append(c.b).Append('\n');
						mtlStr.Append("Kd ").Append(c.r).Append(' ').Append(c.g).Append(' ').Append(c.b).Append('\n');
					}
					catch { UnityEngine.Debug.Log("Material has no color."); }
					try {
						var t = m.mainTexture as Texture2D;
						var tName = t.name;
						for (int c = 0; c < invalidChars.Length; ++c) { tName = tName.Replace(invalidChars[c].ToString(), "_"); }
						if (t != null) {
							
							mtlStr.Append("map_Ka ").Append(tName).Append(".png\n");
							mtlStr.Append("map_Kd ").Append(tName).Append(".png\n");
							if (!textures.Contains(t)) { textures.Add(t); }
						}
					}
					catch { UnityEngine.Debug.Log("Material has no texture."); }
				}

				System.IO.File.WriteAllText(path + "/" + filename + ".mtl", mtlStr.ToString());
			}
			
			for (int i = 0; i < textures.Count; ++i) {
				var t = textures[i];
				var tName = t.name;
				for (int c = 0; c < invalidChars.Length; ++c) { tName = tName.Replace(invalidChars[c].ToString(), "_"); }
				var bytes = t.EncodeToPNG();
				System.IO.File.WriteAllBytes(path + "/" + tName + ".png", bytes); // TODO: same names?
			}

			UnityEngine.Debug.Log("Export of '" + filename + "' (OBJ) done.");
		}
	}

}