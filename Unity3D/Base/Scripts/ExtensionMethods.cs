using UnityEngine;

namespace RatKing {

	public static class ExtensionMethods {
		public static Vector2 ToVec2(this Vector3 v) { return new Vector2(v.x, v.y); }
		public static Vector3 ToVec3(this Vector2 v, float z = 0f) { return new Vector3(v.x, v.y, z); }
		
		public static Vector2 ToVec2f(this Vector2Int v, float scale = 1f) { return new Vector2(v.x * scale, v.y * scale); }
		public static Vector3 ToVec3f(this Vector3Int v, float scale = 1f) { return new Vector3(v.x * scale, v.y * scale, v.z * scale); }
		public static Vector2Int ToVec2i(this Vector2 v, bool floor = false) { return floor ? new Vector2Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y)) : new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y)); }
		public static Vector3Int ToVec3i(this Vector3 v, bool floor = false) { return floor ? new Vector3Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y), Mathf.FloorToInt(v.z)) : new Vector3Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z)); }

		public static Vector2 WithX(this Vector2 v, float x) { v.x = x; return v; } // "this" on structs is always by value, never by reference
		public static Vector2 WithY(this Vector2 v, float y) { v.y = y; return v; } // thus the methods only edit a copy and return a copy
		public static Vector3 WithX(this Vector3 v, float x) { v.x = x; return v; }
		public static Vector3 WithY(this Vector3 v, float y) { v.y = y; return v; }
		public static Vector3 WithZ(this Vector3 v, float z) { v.z = z; return v; }
		public static Vector2Int WithX(this Vector2Int v, int x) { v.x = x; return v; } 
		public static Vector2Int WithY(this Vector2Int v, int y) { v.y = y; return v; }
		public static Vector3Int WithX(this Vector3Int v, int x) { v.x = x; return v; }
		public static Vector3Int WithY(this Vector3Int v, int y) { v.y = y; return v; }
		public static Vector3Int WithZ(this Vector3Int v, int z) { v.z = z; return v; }
		
		public static Vector2 AddX(this Vector2 v, float x) { v.x += x; return v; }
		public static Vector2 AddY(this Vector2 v, float y) { v.y += y; return v; }
		public static Vector3 AddX(this Vector3 v, float x) { v.x += x; return v; }
		public static Vector3 AddY(this Vector3 v, float y) { v.y += y; return v; }
		public static Vector3 AddZ(this Vector3 v, float z) { v.z += z; return v; }
		public static Vector2Int AddX(this Vector2Int v, int x) { v.x += x; return v; } 
		public static Vector2Int AddY(this Vector2Int v, int y) { v.y += y; return v; }
		public static Vector3Int AddX(this Vector3Int v, int x) { v.x += x; return v; }
		public static Vector3Int AddY(this Vector3Int v, int y) { v.y += y; return v; }
		public static Vector3Int AddZ(this Vector3Int v, int z) { v.z += z; return v; }

		public static Color WithRed(this Color c, float r) { c.r = r; return c; }
		public static Color WithGreen(this Color c, float g) { c.g = g; return c; }
		public static Color WithBlue(this Color c, float b) { c.b = b; return c; }
		public static Color WithAlpha(this Color c, float a) { c.a = a; return c; }
		public static Color WithNoAlpha(this Color c) { c.a = 0f; return c; }
		public static Color WithFullAlpha(this Color c) { c.a = 1f; return c; }

		public static Vector3 Position(this GameObject go) { return go.transform.position; }
		public static Vector3 Position(this GameObject go, Vector3 newPosition) { return go.transform.position = newPosition; }
		public static Vector3 LocalPosition(this GameObject go) { return go.transform.localPosition; }
		public static Vector3 LocalPosition(this GameObject go, Vector3 newPosition) { return go.transform.localPosition = newPosition; }
		public static Quaternion Rotation(this GameObject go) { return go.transform.rotation; }
		public static Quaternion Rotation(this GameObject go, Quaternion newRotation) { return go.transform.rotation = newRotation; }
		public static Quaternion LocalRotation(this GameObject go) { return go.transform.localRotation; }
		public static Quaternion LocalRotation(this GameObject go, Quaternion newRotation) { return go.transform.localRotation = newRotation; }
		public static Vector3 EulerAngles(this GameObject go) { return go.transform.eulerAngles; }
		public static Vector3 EulerAngles(this GameObject go, Vector3 newAngles) { return go.transform.eulerAngles = newAngles; }
		public static Vector3 LocalEulerAngles(this GameObject go) { return go.transform.localEulerAngles; }
		public static Vector3 LocalEulerAngles(this GameObject go, Vector3 newAngles) { return go.transform.localEulerAngles = newAngles; }
		public static Vector3 LocalScale(this GameObject go) { return go.transform.localScale; }
		public static Vector3 LocalScale(this GameObject go, Vector3 newScale) { return go.transform.localScale = newScale; }

		public static Vector3 SetPosX(this Transform t, float x) { var p = t.position; p.x = x; return t.position = p; }
		public static Vector3 SetPosY(this Transform t, float y) { var p = t.position; p.y = y; return t.position = p; }
		public static Vector3 SetPosZ(this Transform t, float z) { var p = t.position; p.z = z; return t.position = p; }
		public static Vector3 SetLocalPosX(this Transform t, float x) { var p = t.localPosition; p.x = x; return t.localPosition = p; }
		public static Vector3 SetLocalPosY(this Transform t, float y) { var p = t.localPosition; p.y = y; return t.localPosition = p; }
		public static Vector3 SetLocalPosZ(this Transform t, float z) { var p = t.localPosition; p.z = z; return t.localPosition = p; }
		public static Vector3 SetRotX(this Transform t, float x) { var p = t.eulerAngles; p.x = x; return t.eulerAngles = p; }
		public static Vector3 SetRotY(this Transform t, float y) { var p = t.eulerAngles; p.y = y; return t.eulerAngles = p; }
		public static Vector3 SetRotZ(this Transform t, float z) { var p = t.eulerAngles; p.z = z; return t.eulerAngles = p; }
		public static Vector3 SetLocalRotX(this Transform t, float x) { var p = t.localEulerAngles; p.x = x; return t.localEulerAngles = p; }
		public static Vector3 SetLocalRotY(this Transform t, float y) { var p = t.localEulerAngles; p.y = y; return t.localEulerAngles = p; }
		public static Vector3 SetLocalRotZ(this Transform t, float z) { var p = t.localEulerAngles; p.z = z; return t.localEulerAngles = p; }
		public static Vector3 SetLocalScaleX(this Transform t, float x) { var p = t.localScale; p.x = x; return t.localScale = p; }
		public static Vector3 SetLocalScaleY(this Transform t, float y) { var p = t.localScale; p.y = y; return t.localScale = p; }
		public static Vector3 SetLocalScaleZ(this Transform t, float z) { var p = t.localScale; p.z = z; return t.localScale = p; }

		public static Vector3 AddPosX(this Transform t, float x) { var p = t.position; p.x += x; return t.position = p; }
		public static Vector3 AddPosY(this Transform t, float y) { var p = t.position; p.y += y; return t.position = p; }
		public static Vector3 AddPosZ(this Transform t, float z) { var p = t.position; p.z += z; return t.position = p; }
		public static Vector3 AddLocalPosX(this Transform t, float x) { var p = t.localPosition; p.x += x; return t.localPosition = p; }
		public static Vector3 AddLocalPosY(this Transform t, float y) { var p = t.localPosition; p.y += y; return t.localPosition = p; }
		public static Vector3 AddLocalPosZ(this Transform t, float z) { var p = t.localPosition; p.z += z; return t.localPosition = p; }
		public static Vector3 AddRotX(this Transform t, float x) { var p = t.eulerAngles; p.x += x; return t.eulerAngles = p; }
		public static Vector3 AddRotY(this Transform t, float y) { var p = t.eulerAngles; p.y += y; return t.eulerAngles = p; }
		public static Vector3 AddRotZ(this Transform t, float z) { var p = t.eulerAngles; p.z += z; return t.eulerAngles = p; }
		public static Vector3 AddLocalRotX(this Transform t, float x) { var p = t.localEulerAngles; p.x += x; return t.localEulerAngles = p; }
		public static Vector3 AddLocalRotY(this Transform t, float y) { var p = t.localEulerAngles; p.y += y; return t.localEulerAngles = p; }
		public static Vector3 AddLocalRotZ(this Transform t, float z) { var p = t.localEulerAngles; p.z += z; return t.localEulerAngles = p; }
		public static Vector3 AddLocalScaleX(this Transform t, float x) { var p = t.localScale; p.x += x; return t.localScale = p; }
		public static Vector3 AddLocalScaleY(this Transform t, float y) { var p = t.localScale; p.y += y; return t.localScale = p; }
		public static Vector3 AddLocalScaleZ(this Transform t, float z) { var p = t.localScale; p.z += z; return t.localScale = p; }

		public static Vector2 SetSizeDeltaX(this RectTransform rt, float x) { var sd = rt.sizeDelta; sd.x = x; return rt.sizeDelta = sd; }
		public static Vector2 SetSizeDeltaY(this RectTransform rt, float y) { var sd = rt.sizeDelta; sd.y = y; return rt.sizeDelta = sd; }
		public static Vector2 SetAnchoredPosX(this RectTransform rt, float x) { var ap = rt.anchoredPosition; ap.x = x; return rt.anchoredPosition = ap; }
		public static Vector2 SetAnchoredPosY(this RectTransform rt, float y) { var ap = rt.anchoredPosition; ap.y = y; return rt.anchoredPosition = ap; }
		public static Vector3 SetAnchoredPos3DX(this RectTransform rt, float x) { var ap = rt.anchoredPosition3D; ap.x = x; return rt.anchoredPosition3D = ap; }
		public static Vector3 SetAnchoredPos3DY(this RectTransform rt, float y) { var ap = rt.anchoredPosition3D; ap.y = y; return rt.anchoredPosition3D = ap; }
		public static Vector3 SetAnchoredPos3DZ(this RectTransform rt, float z) { var ap = rt.anchoredPosition3D; ap.z = z; return rt.anchoredPosition3D = ap; }

		public static Vector2 GetPosAsVec2(this GameObject go) { return go.transform.position; }
		public static Vector2 GetPosAsVec2(this Transform t) { return t.position; }
		public static Vector2 GetLocalPosAsVec2(this GameObject go) { return go.transform.localPosition; }
		public static Vector2 GetLocalPosAsVec2(this Transform t) { return t.localPosition; }
		
		public static Vector3 Add(this Vector3 v, Vector2 o) { return new Vector3(v.x + o.x, v.y + o.y, v.z); }
		public static Vector2 Add(this Vector2 v, Vector3 o) { return new Vector2(v.x + o.x, v.y + o.y); }
		public static Vector3 Subtract(this Vector3 v, Vector2 o) { return new Vector3(v.x - o.x, v.y - o.y, v.z); }
		public static Vector2 Subtract(this Vector2 v, Vector3 o) { return new Vector2(v.x - o.x, v.y - o.y); }
		public static Vector3 Multiply(this Vector3 v, Vector3 o) { return new Vector3(v.x * o.x, v.y * o.y, o.z * o.z); }
		public static Vector2 Multiply(this Vector2 v, Vector2 o) { return new Vector2(v.x * o.x, v.y * o.y); }
		public static Vector3Int Add(this Vector3Int v, Vector2Int o) { return new Vector3Int(v.x + o.x, v.y + o.y, v.z); }
		public static Vector2Int Add(this Vector2Int v, Vector3Int o) { return new Vector2Int(v.x + o.x, v.y + o.y); }
		public static Vector3Int Subtract(this Vector3Int v, Vector2Int o) { return new Vector3Int(v.x - o.x, v.y - o.y, v.z); }
		public static Vector2Int Subtract(this Vector2Int v, Vector3Int o) { return new Vector2Int(v.x - o.x, v.y - o.y); }
		public static Vector3Int Multiply(this Vector3Int v, Vector3Int o) { return new Vector3Int(v.x * o.x, v.y * o.y, o.z * o.z); }
		public static Vector2Int Multiply(this Vector2Int v, Vector2Int o) { return new Vector2Int(v.x * o.x, v.y * o.y); }

		public static T GetOrAddComponent<T>(this GameObject go) where T : Component { var ac = go.GetComponent<T>(); if (ac == null) { ac = go.AddComponent<T>(); } return ac; }
		public static T AddComponent<T>(this Component c) where T : Component { return c.gameObject.AddComponent<T>(); }
		public static T GetOrAddComponent<T>(this Component c) where T : Component { var ac = c.gameObject.GetComponent<T>(); if (ac == null) { ac = c.gameObject.AddComponent<T>(); } return ac; }
	}

}