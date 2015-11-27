using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base {

	// all static helper classes
	public class Helpers : MonoBehaviour {
		static Helpers inst;
#if !UNITY_WEBPLAYER
		static Debug debug;
#endif

		static void CreateInstance() {
			if (inst != null)
				return;
			var go = new GameObject("<BaseHelpers>");
			inst = go.AddComponent<Helpers>();
#if !UNITY_WEBPLAYER
			debug = new Debug();
#endif
		}

		public class Debug {
			public static void CreateScreenshot(string prefix, string folder = "screenshots") {
				CreateInstance();
				System.DateTime t = System.DateTime.Now;

				string time = t.Year + ""
					+ (t.Month < 10 ? "0" : "") + t.Month + ""
					+ (t.Day < 10 ? "0" : "") + t.Day + "_"
					+ (t.Hour < 10 ? "0" : "") + t.Hour + ""
					+ (t.Minute < 10 ? "0" : "") + t.Minute + ""
					+ (t.Second < 10 ? "0" : "") + t.Second + "_"
					+ (t.Millisecond < 100 ? "0" : "") + (t.Millisecond < 10 ? "0" : "") + t.Millisecond;

#if UNITY_WEBPLAYER
				Application.CaptureScreenshot(folder + "/" + prefix + "_" + time + ".png");
#else

#if UNITY_EDITOR
				if (!System.IO.Directory.Exists(Application.dataPath + "/../../" + folder))
					System.IO.Directory.CreateDirectory(Application.dataPath + "/../../" + folder);
				inst.StartCoroutine(debug.CreateScreenshotCR("Assets/../../" + folder + "/" + prefix + "_" + time + ".jpg"));
				UnityEngine.Debug.Log("Screenshot: " + "Assets/../../" + folder + "/" + prefix + "_" + time + ".jpg");
#else
				string p = Application.platform == RuntimePlatform.OSXPlayer ? "/../../" : "/../";
				if (!System.IO.Directory.Exists(Application.dataPath + p + folder))
					System.IO.Directory.CreateDirectory(Application.dataPath + "/../" + folder);

				inst.StartCoroutine(debug.CreateScreenshotCR(folder + "/" + prefix + "_" + time + ".jpg"));
#endif
			}
			IEnumerator CreateScreenshotCR(string path) {
				yield return new WaitForEndOfFrame();
				Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
#if UNITY_EDITOR
				var offset = GetWindowOffset() - GetMainGameView().position.position;
				offset.x += 2;
				offset.y -= 16;
				texture.ReadPixels(new Rect(offset.x, offset.y, Screen.width, Screen.height), 0, 0);
#else
				texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
#endif
				texture.Apply();
				yield return null;
				JPGEncoder encoder = new JPGEncoder(texture, 95.0f);
				while (!encoder.isDone)
					yield return null;
				System.IO.File.WriteAllBytes(path, encoder.GetBytes());
#endif
			}

#if UNITY_EDITOR
			// help from http://answers.unity3d.com/questions/915069/want-to-get-the-location-of-the-gameview-window.html
			// and http://answers.unity3d.com/questions/179775/game-window-size-from-editor-window-in-editor-mode.html
			public static UnityEditor.EditorWindow GetMainGameView() {
				System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
				System.Reflection.MethodInfo GetMainGameView = T.GetMethod("GetMainGameView",System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
				System.Object Res = GetMainGameView.Invoke(null,null);
				return (UnityEditor.EditorWindow)Res;
			}
			[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
			struct POINT {
				public int X;
				public int Y;
				public static implicit operator Vector2(POINT p) {
					return new Vector2(p.X, p.Y);
				}
			}
			[System.Runtime.InteropServices.DllImport("user32.dll")]
			[return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
			static extern bool GetCursorPos(out POINT lpPoint);
			static Vector2 GetWindowOffset() {
				Vector2 inputCursor = Input.mousePosition;
				// flip input Cursor y (as the Reference "0" is the last scanline)
				inputCursor.y = Screen.height - 1 - inputCursor.y;
				POINT p;
				GetCursorPos(out p);
				return (p - inputCursor);
			}
#endif
		}

		public static class Gizmos {
			public static void DrawWireCircleAroundTransform(Transform trans, float radius, int prec = 100) {
				float fac = 360f / (float)prec;
				Vector3 v = trans.forward * radius;
				for (int i = 0; i < prec; ++i) {
					Vector3 t = Math.Rotate(v, trans.up, fac);
					UnityEngine.Gizmos.DrawLine(trans.position + v, trans.position + t);
					v = t;
				}
			}
			static Vector3 otherOne = new Vector3(0.5f, -2.0f, 3.5f);
			public static void DrawWireCircle(Vector3 center, Vector3 axis, float radius, int prec = 100) {
				float fac = 360f / (float)prec;
				Vector3 v = Vector3.Cross(axis, (Vector3.Dot(axis, Vector3.one) == 0.0f ? otherOne : Vector3.one)).normalized * radius;
				for (int i = 0; i < prec; ++i) {
					Vector3 t = Math.Rotate(v, axis, fac);
					UnityEngine.Gizmos.DrawLine(center + v, center + t);
					v = t;
				}
			}
		}

		public static class Cameras {
			public static float GetWideFOV(Camera cam) {
				return GetWideFOV(cam.fieldOfView, Screen.width * cam.rect.width, Screen.height * cam.rect.height);
			}
			public static float GetWideFOV(float fov, float w = -1f, float h = -1f) {
				if (w < 0f) w = Screen.width;
				if (h < 0f) h = Screen.height;
				return 2f * Mathf.Atan(w * Mathf.Tan(fov * Mathf.Deg2Rad * 0.5f) / h) * Mathf.Rad2Deg;
			}
		}

		public static class Materials {
			public class Pool {
				private static Dictionary<Material, Dictionary<Color, Material>> usedMaterials = new Dictionary<Material, Dictionary<Color, Material>>();
				public static void Add(Material material) {
					if (!usedMaterials.ContainsKey(material))
						usedMaterials[material] = new Dictionary<Color, Material>();
					usedMaterials[material][material.color] = material;
				}
				public static Material Get(Material material, Color color) {
					if (!usedMaterials.ContainsKey(material))
						usedMaterials[material] = new Dictionary<Color, Material>();

					if (!usedMaterials[material].ContainsKey(color)) {
						Material newMaterial = (Material)Material.Instantiate(material);
						newMaterial.color = color;
						usedMaterials[material][color] = newMaterial;
						return newMaterial;
					}

					return usedMaterials[material][color];
				}
			}
		}

		///////////////////////////////////////////////////////

		public static class Colors {
			public static Color GetFromHexa(string hexCode) {
				// 0 should be "#"
				int r = int.Parse(hexCode.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
				int g = int.Parse(hexCode.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
				int b = int.Parse(hexCode.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
				return new Color((float)r / 255f, (float)g / 255f, (float)b / 255f, 1f);
			}
			//
			public static bool Approx(Color c1, Color c2, float epsilon = 0.01f) {
				return c1.r < c2.r + epsilon && c1.r > c2.r - epsilon &&
					   c1.g < c2.g + epsilon && c1.g > c2.g - epsilon &&
					   c1.b < c2.b + epsilon && c1.b > c2.b - epsilon;
			}
			//
			// from http://www.cs.rit.edu/~ncs/color/t_convert.html
			// r,g,b values are from 0 to 1 | h = [0,360], s = [0,1], v = [0,1] | if s == 0, then h = -1 (undefined)
			public static void RGBtoHSV(float r, float g, float b, out float h, out float s, out float v) {
				float min, max, delta;

				float[] rgb = new float[] { r, g, b };
				min = Mathf.Min(rgb);
				max = Mathf.Max(rgb);
				v = max;

				delta = max - min;

				if (max != 0f) {
					s = delta / max;
				}
				else {
					s = 0f;
					h = -1f;
					return;
				}

				if (r == max)
					h = (g - b) / delta;
				else if (g == max)
					h = 2f + (b - r) / delta;
				else
					h = 4f + (r - g) / delta;

				h *= 60f;
				if (h < 0f)
					h += 360f;
			}
			public static void HSVtoRGB(out float r, out float g, out float b, float h, float s, float v) {
				int i;
				float f, p, q, t;

				if (s == 0f) {
					r = g = b = v;
					return;
				}

				h /= 60f;
				i = Mathf.FloorToInt(h);
				f = h - i;
				p = v * (1f - s);
				q = v * (1f - s * f);
				t = v * (1f - s * (1f - f));

				switch (i) {
					case 0: r = v; g = t; b = p; break;
					case 1: r = q; g = v; b = p; break;
					case 2: r = p; g = v; b = t; break;
					case 3: r = p; g = q; b = v; break;
					case 4: r = t; g = p; b = v; break;
					default: r = v; g = p; b = q; break;
				}
			}
			public static Color WithAlpha(Color c, float a) {
				return new Color(c.r, c.g, c.b, a);
			}
        }


		///////////////////////////////////////////////////////

		public static class Math {
			// vector3 stuff:
			public static Vector3 Rotate(Vector3 vector, Vector3 axis, float angle) {
				return Quaternion.AngleAxis(angle, axis) * vector;
			}
			public static Vector3 Rotate(Vector3 vector, float x, float y, float z) {
				return Quaternion.Euler(x, y, z) * vector;
			}
			public static Vector3 RotateAround(Vector3 vector, Vector3 origin, Vector3 axis, float angle) {
				return origin + Quaternion.AngleAxis(angle, axis) * (vector - origin);
			}
			public static Vector3 Rotate(Vector3 vector, Vector3 origin, float x, float y, float z) {
				return origin + Quaternion.Euler(x, y, z) * (vector - origin);
			}
			// from http://forum.unity3d.com/threads/33215-Vector-rotation
			public static float GetPitch(Vector3 v) {
				float len = Mathf.Sqrt((v.x * v.x) + (v.z * v.z));
				return -Mathf.Atan2(v.y, len);
			}
			public static float GetYaw(Vector3 v) {
				return Mathf.Atan2(v.x, v.z);
			}
			public static Vector3 QuantizeRound(Vector3 v, float e = 1f) {
				float ie = 1f / e;
				v.x = Mathf.Round(v.x * ie) * e;
				v.y = Mathf.Round(v.y * ie) * e;
				v.z = Mathf.Round(v.z * ie) * e;
				return v.normalized;
			}
			public static Vector3 QuantizeFloor(Vector3 v, float e = 1f) {
				float ie = 1f / e;
				v.x = Mathf.Floor(v.x * ie) * e;
				v.y = Mathf.Floor(v.y * ie) * e;
				v.z = Mathf.Floor(v.z * ie) * e;
				return v;
			}
			// quaternion stuff:
			public static float Length(Quaternion q) {
				return Mathf.Sqrt(q.w * q.w + q.x * q.x + q.y * q.y + q.z * q.z);
			}
			public static void Normalize(ref Quaternion q) {
				float l = 1f / Mathf.Sqrt(q.w * q.w + q.x * q.x + q.y * q.y + q.z * q.z);
				q.w *= l; q.x *= l; q.y *= l; q.z *= l;
			}
			// from http://www.euclideanspace.com/maths/algebra/realNormedAlgebra/quaternions/slerp/
			// has auto normalization
			public static Quaternion Slerp(Quaternion qa, Quaternion qb, float t) {
				// quaternion to return
				// Calculate angle between them.
				float cosHalfTheta = qa.w * qb.w + qa.x * qb.x + qa.y * qb.y + qa.z * qb.z;
				//Debug.Log (	Mathf.Sqrt(qa.w * qa.w + qa.x * qa.x + qa.y * qa.y + qa.z * qa.z) + " - " + 
				//			Mathf.Sqrt(qb.w * qb.w + qb.x * qb.x + qb.y * qb.y + qb.z * qb.z));
				// if qa=qb or qa=-qb then theta = 0 and we can return qa
				if (Mathf.Abs(cosHalfTheta) >= 1f) {
					//qm.w = qa.w;qm.x = qa.x;qm.y = qa.y;qm.z = qa.z;
					return qa;
				}
				// Calculate temporary values.
				float halfTheta = Mathf.Acos(cosHalfTheta);
				float sinHalfTheta = Mathf.Sqrt(1f - cosHalfTheta * cosHalfTheta);
				Quaternion qm = new Quaternion();
				// if theta = 180 degrees then result is not fully defined
				// we could rotate around any axis normal to qa or qb
				if (Mathf.Abs(sinHalfTheta) < 0.001f) {
					qm.w = (qa.w * 0.5f + qb.w * 0.5f);
					qm.x = (qa.x * 0.5f + qb.x * 0.5f);
					qm.y = (qa.y * 0.5f + qb.y * 0.5f);
					qm.z = (qa.z * 0.5f + qb.z * 0.5f);
				}
				else {
					float ratioA = Mathf.Sin((1f - t) * halfTheta) / sinHalfTheta;
					float ratioB = Mathf.Sin(t * halfTheta) / sinHalfTheta;
					//calculate Quaternion.
					qm.w = (qa.w * ratioA + qb.w * ratioB);
					qm.x = (qa.x * ratioA + qb.x * ratioB);
					qm.y = (qa.y * ratioA + qb.y * ratioB);
					qm.z = (qa.z * ratioA + qb.z * ratioB);
				}
				float il = 1f / Mathf.Sqrt(qm.w * qm.w + qm.x * qm.x + qm.y * qm.y + qm.z * qm.z);
				qm.w *= il; qm.x *= il; qm.y *= il; qm.z *= il;
				return qm;
			}
			// transitions:
			public static float Hermite(float start, float end, float value) {
				return Mathf.Lerp(start, end, value * value * (3f - 2f * value));
			}
			public static float Sinerp(float start, float end, float value) {
				return Mathf.Lerp(start, end, Mathf.Sin(value * Mathf.PI * 0.5f));
			}
			public static float Coserp(float start, float end, float value) {
				return Mathf.Lerp(start, end, 1f - Mathf.Cos(value * Mathf.PI * 0.5f));
			}
			public static float Berp(float start, float end, float value) {
				value = Mathf.Clamp01(value);
				value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
				return start + (end - start) * value;
			}
			public static float SmoothStep(float x, float min, float max) {
				x = Mathf.Clamp(x, min, max);
				float v1 = (x - min) / (max - min);
				float v2 = (x - min) / (max - min);
				return -2 * v1 * v1 * v1 + 3 * v2 * v2;
			}
			public static float Lerp(float start, float end, float value) {
				return ((1f - value) * start) + (value * end);
			}
			public static float NearestPointDistance(Ray ray, Vector3 point) {
				return Vector3.Dot((point - ray.origin), ray.direction) / Vector3.Dot(ray.direction, ray.direction);
			}
			public static float NearestPointDistance(Vector3 lineStart, Vector3 lineDirection, Vector3 point) {
				return Vector3.Dot((point - lineStart), lineDirection) / Vector3.Dot(lineDirection, lineDirection);
			}
			public static Vector3 NearestPoint(Ray ray, Vector3 point) {
				float closestPoint = Vector3.Dot((point - ray.origin), ray.direction);
				return ray.origin + (closestPoint * ray.direction);
			}
			public static Vector3 NearestPoint(Vector3 lineStart, Vector3 lineEnd, Vector3 point) {
				Vector3 lineDirection = Vector3.Normalize(lineEnd - lineStart);
				float closestPoint = Vector3.Dot((point - lineStart), lineDirection); // Vector3.Dot(lineDirection,lineDirection);
				return lineStart + (closestPoint * lineDirection);
			}
			public static Vector3 NearestPointStrict(Vector3 lineStart, Vector3 lineEnd, Vector3 point) {
				Vector3 fullDirection = lineEnd - lineStart;
				Vector3 lineDirection = Vector3.Normalize(fullDirection);
				float closestPoint = Vector3.Dot((point - lineStart), lineDirection) / lineDirection.sqrMagnitude; //Vector3.Dot(lineDirection,lineDirection);
				return lineStart + (Mathf.Clamp(closestPoint, 0f, Vector3.Magnitude(fullDirection)) * lineDirection);
			}
			public static float Bounce(float x) {
				return Mathf.Abs(Mathf.Sin(6.28f * (x + 1f) * (x + 1f)) * (1f - x));
			}
			/*
			* CLerp - Circular Lerp - is like lerp but handles the wraparound from 0 to 360.
			* This is useful when interpolating eulerAngles and the object
			* crosses the 0/360 boundary.  The standard Lerp function causes the object
			* to rotate in the wrong direction and looks stupid. Clerp fixes that.
			*/
			public static float Clerp(float start, float end, float value) {
				float min = 0f;
				float max = 360f;
				float half = Mathf.Abs((max - min) / 2f);//half the distance between min and max
				float diff = 0f;

				if ((end - start) < -half) {
					diff = ((max - start) + end) * value;
					return start + diff;
				}
				else if ((end - start) > half) {
					diff = -((max - end) + start) * value;
					return start + diff;
				}
				return start + (end - start) * value;
			}
			// from http://pastebin.com/NZrstYL4
			public static Vector3 Qarp(Vector3 a, Vector3 b, Vector3 c, float blend) {
				Vector3 a_b = Vector3.Lerp(a, b, blend);
				return a_b + (Vector3.Lerp(b, c, blend) - a_b) * blend;
			}
			// from http://pastebin.com/NZrstYL4
			public static Vector3 SuperLerp(Vector3 p0, Vector3 p1, Vector3 v0, Vector3 v1, float l) {
				return Qarp(p0, (p0 + v0 * 0.5f + p1 - v1 * 0.5f) / 2f, p1, l);
			}
			//
			public static bool Approx(float f1, float f2, float epsilon = 0.01f) {
				return f1 < f2 + epsilon && f1 > f2 - epsilon;
			}
			public static bool Approx(Vector3 v1, Vector3 v2, float epsilon = 0.01f) {
				return v1.x < v2.x + epsilon && v1.x > v2.x - epsilon &&
					   v1.y < v2.y + epsilon && v1.y > v2.y - epsilon &&
					   v1.z < v2.z + epsilon && v1.z > v2.z - epsilon;
			}
		}

		///////////////////////////////////////////////////////

		public static class String {
			public static char[] linesSplitter = new char[1] { '\n' };
			public static char[] spaceSplitter = new char[5] { ' ', '\n', '\t', '\r', '\0' };
			//
			public static string DisplayScore(int score, int digits = 7) {
				string text = string.Empty;
				int ts = score;
				do {
					ts /= 10;
					digits--;
				} while (ts > 0);
				for (int i = digits - 1; i >= 0; --i)
					text += "0";
				return text + score;
			}
			public static string DisplayMinutes(int seconds) {
				int s = seconds % 60;
				int m = seconds / 60;
				return m.ToString() + ":" + (s < 10 ? ("0" + s) : s.ToString());
			}
		}

		///////////////////////////////////////////////////////

		public static class DataStructure {
			static System.Random randomGenerator;
			public static void Shuffle<T>(IList<T> ls) {
				if (randomGenerator == null)
					randomGenerator = new System.Random();
				int n = ls.Count;
				while (n > 1) {
					n--;
					int k = randomGenerator.Next(n + 1);
					T value = ls[k];
					ls[k] = ls[n];
					ls[n] = value;
				}
			}
			public static T GetRandomElement<T>(HashSet<T> hs) {
				int r = Random.Range(0, hs.Count);
				foreach (T elem in hs)
					if (r-- <= 0)
						return elem;
				return default(T);
			}

			public static T GetRandomElement<T>(List<T> ls) {
				return ls[Random.Range(0, ls.Count)];
			}

			public static KeyValuePair<U, T> GetRandomElement<U, T>(Dictionary<U, T> ds) {
				int r = Random.Range(0, ds.Count);
				foreach (var elem in ds)
					if (r-- <= 0)
						return elem;
				return default(KeyValuePair<U, T>);
			}
			public static T GetFirstElement<T>(HashSet<T> hs) {
				foreach (T elem in hs)
					return elem;
				return default(T);
			}
		}

		///////////////////////////////////////////////////////

		public static class Randomness {
			public static Color GetColor(bool randomAlpha, float alpha = 1f) {
				return new Color(Random.value, Random.value, Random.value, randomAlpha ? Random.value : alpha);
			}

			public static class Probabilities {
				public static int GetRandomIndex(float[] probabilites) {
					float r = Random.value;
					int i = 0;
					for (; i < probabilites.Length; ++i) {
						if (r < probabilites[i])
							break;
					}
					return i;
				}
				public static float[] Normalize(int[] probabilities) {
					float sum = 0f;
					float[] newProbs = new float[probabilities.Length];
					for (int i = 0; i < probabilities.Length; ++i)
						sum += (float)probabilities[i];
					for (int i = 0; i < probabilities.Length; ++i)
						newProbs[i] = ((float)probabilities[i] / sum);
					for (int i = 1; i < probabilities.Length; ++i)
						newProbs[i] += newProbs[i - 1];
					newProbs[probabilities.Length - 1] = 1f; // to be sure.
					return newProbs;
				}
			}
			//
			public static class SimplexNoise {
				// from: http://stephencarmody.wikispaces.com/Simplex+Noise

				private static int i, j, k;
				private static int[] A = new int[] { 0, 0, 0 };
				private static float u, v, w, s;
				private static float onethird = 0.333333333f;
				private static float onesixth = 0.166666667f;
				private static int[] T = new int[] { 0x15, 0x38, 0x32, 0x2c, 0x0d, 0x13, 0x07, 0x2a };

				//

				public static float NoiseLoop(float x, float y, float t, float T) {
					return ((T - t) * Noise(x, y, t) + t * Noise(x, y, t - T)) / T;
				}

				// returns a value in the range of about [-0.347 .. 0.347]
				public static float Noise(float x, float y, float z) {
					// Skew input space to relative coordinate in simplex cell
					s = (x + y + z) * onethird;
					i = Fastfloor(x + s);
					j = Fastfloor(y + s);
					k = Fastfloor(z + s);

					// Unskew cell origin back to (x, y , z) space
					s = (i + j + k) * onesixth;
					u = x - i + s;
					v = y - j + s;
					w = z - k + s; ;

					A[0] = A[1] = A[2] = 0;

					// For 3D case, the simplex shape is a slightly irregular tetrahedron.
					// Determine which simplex we're in
					int hi = u >= w ? u >= v ? 0 : 1 : v >= w ? 1 : 2;
					int lo = u < w ? u < v ? 0 : 1 : v < w ? 1 : 2;

					return k_m(hi) + k_m(3 - hi - lo) + k_m(lo) + k_m(0);
				}

				// normalized (goes [0..1], but only circa!)
				public static float NormalizedNoise(float x, float y, float z) {
					return (Noise(x, y, z) + 0.347f) * 1.4409f;
				}

				// normalized (goes [-1..1], but only circa!)
				public static float NoiseMinusPlus1(float x, float y, float z) {
					return Noise(x, y, z) * 1.4409f;
				}

				//

				private static int Fastfloor(float n) {
					return n > 0 ? (int)(n) : (int)(n - 1);
				}

				private static float k_m(int a) {
					s = (A[0] + A[1] + A[2]) * onesixth;
					float x = u - A[0] + s;
					float y = v - A[1] + s;
					float z = w - A[2] + s;
					float t = 0.6f - x * x - y * y - z * z;
					int h = Shuffle(i + A[0], j + A[1], k + A[2]);
					A[a]++;
					if (t < 0) return 0;
					int b5 = h >> 5 & 1;
					int b4 = h >> 4 & 1;
					int b3 = h >> 3 & 1;
					int b2 = h >> 2 & 1;
					int b = h & 3;
					float p = b == 1 ? x : b == 2 ? y : z;
					float q = b == 1 ? y : b == 2 ? z : x;
					float r = b == 1 ? z : b == 2 ? x : y;
					p = b5 == b3 ? -p : p;
					q = b5 == b4 ? -q : q;
					r = b5 != (b4 ^ b3) ? -r : r;
					t *= t;
					return 8 * t * t * (p + (b == 0 ? q + r : b2 == 0 ? q : r));
				}

				private static int Shuffle(int i, int j, int k) {
					return b_m(i, j, k, 0) + b_m(j, k, i, 1) + b_m(k, i, j, 2) + b_m(i, j, k, 3) +
						   b_m(j, k, i, 4) + b_m(k, i, j, 5) + b_m(i, j, k, 6) + b_m(j, k, i, 7);
				}

				private static int b_m(int i, int j, int k, int B) {
					return T[b2_m(i, B) << 2 | b2_m(j, B) << 1 | b2_m(k, B)];
				}

				private static int b2_m(int N, int B) {
					return N >> B & 1;
				}
			}
		}

		///////////////////////////////////////////////////////

		public static class Geometry {
			public static void SetLayer(Transform t, int layer) {
				for (int i = 0; i < t.childCount; ++i) {
					Transform child = t.GetChild(i);
					SetLayer(child, layer);
				}
				t.gameObject.layer = layer;
			}

			// from: http://answers.unity3d.com/questions/7789/calculating-tangents-vector4.html
			public static void CalculateMeshTangents(Mesh mesh) {
				//speed up math by copying the mesh arrays
				int[] triangles = mesh.triangles;
				Vector3[] vertices = mesh.vertices;
				Vector2[] uv = mesh.uv;
				Vector3[] normals = mesh.normals;

				//variable definitions
				int triangleCount = triangles.Length;
				int vertexCount = vertices.Length;

				Vector3[] tan1 = new Vector3[vertexCount];
				Vector3[] tan2 = new Vector3[vertexCount];

				Vector4[] tangents = new Vector4[vertexCount];

				for (long a = 0; a < triangleCount; a += 3) {
					long i1 = triangles[a + 0];
					long i2 = triangles[a + 1];
					long i3 = triangles[a + 2];

					Vector3 v1 = vertices[i1];
					Vector3 v2 = vertices[i2];
					Vector3 v3 = vertices[i3];

					Vector2 w1 = uv[i1];
					Vector2 w2 = uv[i2];
					Vector2 w3 = uv[i3];

					float x1 = v2.x - v1.x;
					float x2 = v3.x - v1.x;
					float y1 = v2.y - v1.y;
					float y2 = v3.y - v1.y;
					float z1 = v2.z - v1.z;
					float z2 = v3.z - v1.z;

					float s1 = w2.x - w1.x;
					float s2 = w3.x - w1.x;
					float t1 = w2.y - w1.y;
					float t2 = w3.y - w1.y;

					float r = 1f / (s1 * t2 - s2 * t1);

					Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
					Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

					tan1[i1] += sdir;
					tan1[i2] += sdir;
					tan1[i3] += sdir;

					tan2[i1] += tdir;
					tan2[i2] += tdir;
					tan2[i3] += tdir;
				}


				for (long a = 0; a < vertexCount; ++a) {
					Vector3 n = normals[a];
					Vector3 t = tan1[a];

					//Vector3 tmp = (t - n * Vector3.Dot(n, t)).normalized;
					//tangents[a] = new Vector4(tmp.x, tmp.y, tmp.z);
					Vector3.OrthoNormalize(ref n, ref t);
					tangents[a].x = t.x;
					tangents[a].y = t.y;
					tangents[a].z = t.z;

					tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0f) ? -1f : 1f;
				}

				mesh.tangents = tangents;
			}
		}

		///////////////////////////////////////////////////////

		public static class Pathfinding {
			public class Node<T> where T : IPosition {
				public Node<T> parent = null;
				public Waypoint<T> block;
				public int F = 0;
				public int G = 0;
				public int H = 0;
				public Node(Waypoint<T> block) { this.block = block; }
				public Node(Waypoint<T> block, Node<T> parent, int F, int G, int H) { this.block = block; this.parent = parent; this.F = F; this.G = G; this.H = H; }
			};

			// Finds A Path Between Two Blocks, Returns The Path In An Array
			public static bool Find<T>(Waypoint<T> A, Waypoint<T> B, out Waypoint<T>[] path, int maxLength, bool ignoreCreatures) where T : IPosition {
				if (A == null || B == null) {
					path = null;
					return false;
				}

				path = null;
				List<Node<T>> open = new List<Node<T>>(maxLength);
				List<Node<T>> closed = new List<Node<T>>(maxLength);
				Node<T> current = null;
				open.Add(new Node<T>(A));

				// pathfinding!
				for (;;) {

					// no nodes any more, but target not found? -> end
					if (open.Count == 0)
						return false;

					current = open[0]; // list is sorted!

					// remove node from open list and move it into closed list
					open.Remove(current);
					closed.Add(current);

					// target found? -> end the search!
					if (current.block == B) {
						int pathLength = 0;
						Node<T> counter = current;

						do // go backwards, for counting only
						{

							counter = counter.parent;
							++pathLength;
						} while (counter != null);

						path = new Waypoint<T>[pathLength];

						// go forward, for building path
						for (int i = 0; i < pathLength; ++i) {
							path[pathLength - i - 1] = current.block;
							current = current.parent;
						}

						return true;
					}

					int G = current.G + 1; // Position.GetSqrDistance(current.block.absPosition, neighbour.absPosition);
					if (G > maxLength) continue; // return false;

					// iterate through connections
					int nm = current.block.neighbours.Count;
					for (int i = 0; i < nm; ++i) {
						Waypoint<T> neighBlock = current.block.neighbours[i];
						if (neighBlock == null || (!ignoreCreatures && neighBlock != B && neighBlock.creature != null))
							continue;

						// node is already in closed list?
						if (closed.Find(n => n.block == neighBlock) != null)
							continue;

						// node is already in open list?
						Node<T> oldNode = open.Find(n => n.block == neighBlock);
						if (oldNode != null) {
							if (G < oldNode.G) {
								oldNode.G = G;
								oldNode.F = G + oldNode.H;
								oldNode.parent = current;
								open.Sort((Node<T> a, Node<T> b) => a.F - b.F);
							}
						}
						else {
							int H = neighBlock.pos.GetManhattanDistanceTo(B.pos);
							// int H = Mathf.RoundToInt(Mathf.Abs(neighBlock.worldPos.x - B.worldPos.x) + Mathf.Abs(neighBlock.worldPos.y - B.worldPos.y) + Mathf.Abs(neighBlock.worldPos.z - B.worldPos.z));
							int F = G + H;
							Node<T> newNode = new Node<T>(neighBlock, current, F, G, H);
							open.Add(newNode);
							open.Sort((Node<T> a, Node<T> b) => a.F - b.F);
						}
					}
				}
			}
		}

		///////////////////////////////////////////////////////
		public static class Security {
			public static string MD5Sum(string strToEncrypt) {
				var ue = new System.Text.UTF8Encoding();
				byte[] bytes = ue.GetBytes(strToEncrypt);

				// encrypt bytes
				var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
				byte[] hashBytes = md5.ComputeHash(bytes);

				// Convert the encrypted bytes back to a string (base 16)
				string hashString = "";

				for (int i = 0; i < hashBytes.Length; i++) {
					hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
				}

				return hashString.PadLeft(32, '0');
			}
		}
	}

	///////////////////////////////////////////////////////

	// a 3d waypoint for pathfinding
	public class Waypoint<T> where T : IPosition {
		public T pos;
		public Vector3 worldPos;
		public bool solid;
		public List<Waypoint<T>> neighbours = new List<Waypoint<T>>();
		public Creature creature;
		public Creature nextCreature;
		//public int type;
		public Waypoint(T pos, bool solid) { this.pos = pos; worldPos = pos.ToVector(); this.solid = solid; }
	}

	public interface IPosition {
		Vector3 ToVector();
		int GetDistanceTo(IPosition other);
		int GetSqrDistanceTo(IPosition other);
		int GetManhattanDistanceTo(IPosition other);
		int GetSqrManhattanDistanceTo(IPosition other);
	}

	// integer 3d position
	[System.Serializable]
	public struct Position3 : IPosition {
		public int x, y, z;
		public static Position3 RoundedVector(Vector3 v) { return new Position3(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z)); }
		public static Position3 FlooredVector(Vector3 v) { return new Position3(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y), Mathf.FloorToInt(v.z)); }
		public static Position3 CeiledVector(Vector3 v) { return new Position3(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y), Mathf.CeilToInt(v.z)); }
		public Position3(int x = 0, int y = 0, int z = 0) { this.x = x; this.y = y; this.z = z; }
		public void Set(int x, int y, int z) { this.x = x; this.y = y; this.z = z; }
		public void Reset() { x = y = z = 0; }
		//
		public int GetDistanceTo(IPosition other) {
			return (int)(other.ToVector() - ToVector()).magnitude;
		}
		public int GetSqrDistanceTo(IPosition other) {
			return (int)(other.ToVector() - ToVector()).sqrMagnitude;
		}
		public int GetManhattanDistanceTo(IPosition other) {
			var v = other.ToVector();
			return (int)(Mathf.Abs(x - v.x) + Mathf.Abs(y - v.y) + Mathf.Abs(z - v.z));
		}
		public int GetSqrManhattanDistanceTo(IPosition other) {
			int md = GetManhattanDistanceTo(other);
			return md * md;
		}
		public Vector3 ToVector() { return new Vector3((float)x, (float)y, (float)z); }
		public Vector3 ToVector(float width) { return new Vector3(x * width, y * width, z * width); }
		//
		public static bool operator ==(Position3 a, Position3 b) { return a.x == b.x && a.y == b.y && a.z == b.z; }
		public static bool operator !=(Position3 a, Position3 b) { return a.x != b.x || a.y != b.y || a.z != b.z; }
		public static Position3 operator +(Position3 a, Position3 b) { return new Position3(a.x + b.x, a.y + b.y, a.z + b.z); }
		public static Position3 operator -(Position3 a, Position3 b) { return new Position3(a.x - b.x, a.y - b.y, a.z - b.z); }
		public static Position3 operator *(Position3 p, int i) { return new Position3(p.x * i, p.y * i, p.z * i); }
		public static Position3 operator /(Position3 p, int i) { return new Position3(p.x / i, p.y / i, p.z / i); }
		public static Position3 operator %(Position3 p, int i) { return new Position3(p.x % i, p.y % i, p.z % i); }
		//
		//public override bool Equals(object o) { try { return (bool)(this == (Position3)o); } catch { return false; } }
		public override bool Equals(object o) { Position3 p = (Position3)o; return p == this; }
		public override int GetHashCode() { return base.GetHashCode(); }
		public override string ToString() { return this.x + ", " + this.y + ", " + this.z; }
	}


	// integer 2d position
	[System.Serializable]
	public struct Position2 : IPosition {
		public int x, y;
		public static Position2 RoundedVector(Vector2 v) { return new Position2(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y)); }
		public static Position2 FlooredVector(Vector2 v) { return new Position2(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y)); }
		public static Position2 CeiledVector(Vector2 v) { return new Position2(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y)); }
		public Position2(int x = 0, int y = 0) { this.x = x; this.y = y; }
		public void Set(int x, int y) { this.x = x; this.y = y; }
		public void Reset() { x = y = 0; }
		//
		public int GetDistanceTo(IPosition other) {
			return (int)(other.ToVector() - ToVector()).magnitude;
		}
		public int GetSqrDistanceTo(IPosition other) {
			return (int)(other.ToVector() - ToVector()).sqrMagnitude;
		}
		public int GetManhattanDistanceTo(IPosition other) {
			var v = other.ToVector();
			return (int)(Mathf.Abs(x - v.x) + Mathf.Abs(y - v.y));
		}
		public int GetSqrManhattanDistanceTo(IPosition other) {
			int md = GetManhattanDistanceTo(other);
			return md * md;
		}
		public Vector3 ToVector() { return new Vector3((float)x, (float)y, 0f); }
		//
		public static bool operator ==(Position2 a, Position2 b) { return a.x == b.x && a.y == b.y; }
		public static bool operator !=(Position2 a, Position2 b) { return a.x != b.x || a.y != b.y; }
		public static Position2 operator +(Position2 a, Position2 b) { return new Position2(a.x + b.x, a.y + b.y); }
		public static Position2 operator -(Position2 a, Position2 b) { return new Position2(a.x - b.x, a.y - b.y); }
		public static Position2 operator *(Position2 p, int i) { return new Position2(p.x * i, p.y * i); }
		public static Position2 operator /(Position2 p, int i) { return new Position2(p.x / i, p.y / i); }
		public static Position2 operator %(Position2 p, int i) { return new Position2(p.x % i, p.y % i); }
		//
		public static Position2 zero = new Position2(0, 0);
		//
		public override bool Equals(object o) { try { return (bool)(this == (Position2)o); } catch { return false; } }
		public override int GetHashCode() { return base.GetHashCode(); }
		public override string ToString() { return this.x + ", " + this.y; }
	}

}