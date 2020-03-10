using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RatKing.Base {

	public class Root : MonoBehaviour {
		[SerializeField] Transform root = null;
		public Transform Root { get { return root; } }

		//

		public static Transform Get(Transform t) {
			if (t == null) { return null; }
			if (t.TryGetComponent(out Root r)) { return r.root; }
			return t;
		}

		public static GameObject Get(GameObject go) {
			if (go == null) { return null; }
			if (go.TryGetComponent(out Root r)) { return r.root.gameObject; }
			return go;
		}

		public static T GetComponent<T>(Transform t) where T : Component {
			if (t == null) { return null; }
			if (t.TryGetComponent(out Root r)) { return r.root.GetComponent<T>(); }
			return t.GetComponent<T>();
		}

		public static T GetComponent<T>(GameObject go) where T : Component {
			if (go == null) { return null; }
			if (go.TryGetComponent(out Root r)) { return r.root.GetComponent<T>(); }
			return go.GetComponent<T>();
		}

		public static T[] GetComponents<T>(Transform t) where T : Component {
			if (t == null) { return null; }
			if (t.TryGetComponent(out Root r)) { return r.root.GetComponents<T>(); }
			return t.GetComponents<T>();
		}

		public static T[] GetComponents<T>(GameObject go) where T : Component {
			if (go == null) { return null; }
			if (go.TryGetComponent(out Root r)) { return r.root.GetComponents<T>(); }
			return go.GetComponents<T>();
		}

		public static T GetComponentInChildren<T>(Transform t, bool includeInactive = false) where T : Component {
			if (t == null) { return null; }
			if (t.TryGetComponent(out Root r)) { return r.root.GetComponentInChildren<T>(includeInactive); }
			return t.GetComponentInChildren<T>(includeInactive);
		}

		public static T GetComponentInChildren<T>(GameObject go, bool includeInactive = false) where T : Component {
			if (go == null) { return null; }
			if (go.TryGetComponent(out Root r)) { return r.root.GetComponentInChildren<T>(includeInactive); }
			return go.GetComponentInChildren<T>(includeInactive);
		}

		public static T[] GetComponentsInChildren<T>(Transform t, bool includeInactive = false) where T : Component {
			if (t == null) { return null; }
			if (t.TryGetComponent(out Root r)) { return r.root.GetComponentsInChildren<T>(includeInactive); }
			return t.GetComponentInChildren<T>();
		}

		public static T[] GetComponentsInChildren<T>(GameObject go, bool includeInactive = false) where T : Component {
			if (go == null) { return null; }
			if (go.TryGetComponent(out Root r)) { return r.root.GetComponentsInChildren<T>(includeInactive); }
			return go.GetComponentsInChildren<T>();
		}
	}

}