using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base {

	public static class GameObjects {
		public static bool CheckLayerMask(GameObject go, int layerMask) {
			return ((1 << go.layer) & layerMask) != 0;
		}

		public static GameObject Create(string name, Vector3 pos, Quaternion rot, Transform parent = null) {
			var go = new GameObject(name);
			var t = go.transform;
			if (parent != null) { t.SetParent(parent); }
			t.position = pos;
			t.rotation = rot;
			return go;
		}

		public static Transform CreateTransform(string name, Transform parent = null) {
			var t = new GameObject(name).transform;
			if (parent != null) {
				t.SetParent(parent);
				t.localPosition = Vector3.zero;
				t.localRotation = Quaternion.identity;
			}
			return t;
		}

		public static bool IsNullOrInactive(this Transform transform) { return transform == null || !transform.gameObject.activeInHierarchy; }
		public static bool IsNullOrInactive(this GameObject gameObject) { return gameObject == null || !gameObject.activeInHierarchy; }

		/// <summary>
		/// Checks if there is a Poolable and uses this one if possible
		/// </summary>
		/// <param name="go"></param>
		/// <returns></returns>
		public static GameObject Clone(GameObject go) {
			if (go == null) { return null; }
			var poolable = go.GetComponent<Poolable>();
			if (poolable != null) { return poolable.PoolPop().gameObject; }
			return GameObject.Instantiate(go);
		}

		/// <summary>
		/// Checks if there is a Poolable and uses this one if possible
		/// </summary>
		/// <param name="go"></param>
		/// <param name="pos"></param>
		/// <returns></returns>
		public static GameObject Clone(GameObject go, Vector3 pos) {
			if (go == null) { return null; }
			var poolable = go.GetComponent<Poolable>();
			if (poolable != null) { return poolable.PoolPop(pos).gameObject; }
			return GameObject.Instantiate(go, pos, Quaternion.identity);
		}

		/// <summary>
		/// Checks if there is a Poolable and uses this one if possible
		/// </summary>
		/// <param name="go"></param>
		/// <param name="pos"></param>
		/// <param name="rot"></param>
		/// <returns></returns>
		public static GameObject Clone(GameObject go, Vector3 pos, Quaternion rot) {
			if (go == null) { return null; }
			var poolable = go.GetComponent<Poolable>();
			if (poolable != null) { return poolable.PoolPop(pos, rot).gameObject; }
			return GameObject.Instantiate(go, pos, rot);
		}

		/// <summary>
		/// Checks if there is a Poolable and uses this one if possible
		/// </summary>
		/// <param name="go"></param>
		public static void Remove(GameObject go) {
			if (go == null) { return; }
			var poolable = go.GetComponent<Poolable>();
			if (poolable != null && poolable.IsPushable()) { poolable.PoolPush(); }
			else { Object.Destroy(go); }
		}
		
		/// <summary>
		/// Checks if A is before B in the hierarchy
		/// </summary>
		/// <param name="A">The transform to check</param>
		/// <param name="B">The transform to compare with</param>
		public static bool IsAfterInHierarchy(this Transform A, Transform B) {
			if (A == null || A == B) { return false; }
			if (B == null) { return true; }
			var hierarchyA = new List<Transform>(); // TODO: cache, or remove
			while (true) {
				hierarchyA.Add(A);
				if (A.parent == null) { break; }
				A = A.parent;
				if (A == B) { return true; } // B is a (grand)parent of A
			}
			while (true) {
				var idx = hierarchyA.IndexOf(B.parent);
				if (idx == 0) { return false; } // A is a (grand)parent of B
				if (idx > 0) { return hierarchyA[idx - 1].GetSiblingIndex() > B.GetSiblingIndex(); }
				if (B.parent == null) { break; }
				B = B.parent;
			}
			return A.GetSiblingIndex() > B.GetSiblingIndex(); // A and B don't share any parent
		}
	}

}