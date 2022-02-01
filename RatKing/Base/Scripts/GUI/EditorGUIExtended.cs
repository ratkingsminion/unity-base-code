using System;
using UnityEngine;

namespace RatKing.Base {

	public static class EditorGUIExtended {
		static Texture2D texLine;
		static Matrix4x4 saveMatrix;
		static Rect lineRect;

		//

		static EditorGUIExtended() {
			texLine = new Texture2D(1, 5, TextureFormat.ARGB32, true);
			texLine.SetPixel(0, 0, new Color(1, 1, 1, 0));
			texLine.SetPixel(0, 1, Color.white);
			texLine.SetPixel(0, 2, Color.white);
			texLine.SetPixel(0, 3, Color.white);
			texLine.SetPixel(0, 4, new Color(1, 1, 1, 0));
			texLine.Apply();
			SetLineWidth(5f);
		}

		public static void SetLineWidth(float w) {
			lineRect = new Rect(0f, -w * 0.5f, 1f, w);
		}

		public static void BeginDrawingSeveralLines() {
			saveMatrix = GUI.matrix;
		}
		public static void DrawOneLineOfSeveral(Vector2 pointA, Vector2 pointB) {
			var delta = pointB - pointA;
			var length = delta.magnitude;
			if (length < 0.01f) { return; }
			GUI.matrix = saveMatrix * Matrix4x4.TRS(pointA, Quaternion.Euler(0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg), new Vector3(length, 1f, 1f));
			GUI.DrawTexture(lineRect, texLine);
		}
		public static void DrawOneRayOfSeveral(Vector2 point, Vector2 direction) {
			var length = direction.magnitude;
			if (length < 0.01f) { return; }
			GUI.matrix = saveMatrix * Matrix4x4.TRS(point, Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg), new Vector3(length, 1f, 1f));
			GUI.DrawTexture(lineRect, texLine);
		}
		public static void EndDrawingSeveralLines() {
			GUI.matrix = saveMatrix;
		}

		public static void DrawLine(Vector2 pointA, Vector2 pointB) {
			var delta = pointB - pointA;
			var length = delta.magnitude;
			if (length < 0.01f) { return; }
			var saveMatrix = GUI.matrix;
			GUI.matrix = saveMatrix * Matrix4x4.TRS(pointA, Quaternion.Euler(0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg), new Vector3(length, 1f, 1f));
			GUI.DrawTexture(lineRect, texLine);
			GUI.matrix = saveMatrix;
		}
		public static void DrawRay(Vector2 point, Vector2 direction) {
			var length = direction.magnitude;
			if (length < 0.01f) { return; }
			var saveMatrix = GUI.matrix;
			GUI.matrix = saveMatrix * Matrix4x4.TRS(point, Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg), new Vector3(length, 1f, 1f));
			GUI.DrawTexture(lineRect, texLine);
			GUI.matrix = saveMatrix;
		}
		
		private static Vector2 GetPt(Vector2 p1, Vector2 p2, float perc) {
			return p1 + ((p2 - p1) * perc);
		}

		public static void DrawBezier(Vector2[] points, int segments = 15) {
			if (!IsBezierInsideScreen(points)) { return; }
			float granularity = 1f / segments;
			var last = points[0];
			var A = points[1] - points[0];
			var B = points[2] - points[1];
			var C = points[3] - points[2];
			BeginDrawingSeveralLines();
			for (float p = granularity; p < 1f; p += granularity) {
				var a = points[0] + A * p;
				var b = points[1] + B * p;
				var c = points[2] + C * p;
				var d = a + (b - a) * p;
				var e = b + (c - b) * p;
				var now = d + (e - d) * p;
				DrawOneLineOfSeveral(last, now);
				last = now;
			}
			DrawOneLineOfSeveral(last, points[3]);
			EndDrawingSeveralLines();
		}

		//

		public static void GetPointOnBezier(Vector2[] points, float t, out Vector2 v) {
			var mt = 1f - t;
			var mt2 = mt*mt;
			var mt3 = mt2*mt;
			var t2 = t*t;
			v = new Vector2() {
				x = points[0].x * mt3 + points[1].x * 3 * mt2 * t + points[2].x * 3 * mt * t2 + points[3].x * t * t2,
				y = points[0].y * mt3 + points[1].y * 3 * mt2 * t + points[2].y * 3 * mt * t2 + points[3].y * t * t2
			};
		}

		public static bool IsRectInsideScreen(Rect rect) {
			var min = GUIUtility.GUIToScreenPoint(new Vector2(rect.xMin, rect.yMin));
			var max = GUIUtility.GUIToScreenPoint(new Vector2(rect.xMax, rect.yMax));
			return min.x > -rect.width && max.x < (Screen.width + rect.width) && min.y > -rect.height && max.y < (Screen.height + rect.height);
		}

		public static bool IsBezierInsideScreen(Vector2[] points) {
			if (points.Length != 4) { return false; }
			var min = points[0];
			var max = points[0];
			for (int i = 1; i < 4; ++i) {
				if (points[i].x < min.x) { min.x = points[i].x; }
				if (points[i].x > max.x) { max.x = points[i].x; }
				if (points[i].y < min.y) { min.y = points[i].y; }
				if (points[i].y > max.y) { max.y = points[i].y; }
			}
			min = GUIUtility.GUIToScreenPoint(min);
			max = GUIUtility.GUIToScreenPoint(max);
			var width = max.x - min.x;
			var height = max.y - min.y;
			return min.x > -width && max.x < (Screen.width + width) && min.y > -height && max.y < (Screen.height + height);
		}
	}

}