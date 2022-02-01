using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base {

	public class GameObjects {
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

		public static bool IsNullOrInactive(Transform transform) { return transform == null || !transform.gameObject.activeInHierarchy; }
		public static bool IsNullOrInactive(GameObject gameObject) { return gameObject == null || !gameObject.activeInHierarchy; }

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
	}

}