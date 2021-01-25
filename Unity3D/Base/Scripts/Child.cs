using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RatKing.Base {

	public class Child : MonoBehaviour {
		[SerializeField] Transform root = null;
		public Transform Root { get { return root; } }

		//

		public T GetRootComponent<T>() where T : Component {
			return (root != null ? root : transform).GetComponent<T>();
		}

		public T[] GetRootComponents<T>() where T : Component {
			return (root != null ? root : transform).GetComponents<T>();
		}

		public T GetRootComponentInChildren<T>(bool includeInactive = false) where T : Component {
			return (root != null ? root : transform).GetComponentInChildren<T>(includeInactive);
		}

		public T[] GetRootComponentsInChildren<T>( bool includeInactive = false) where T : Component {
			return (root != null ? root : transform).GetComponentsInChildren<T>(includeInactive);
		}
	}

	public static class Root {
		public static Transform Get(Component t) {
			if (t == null) { return null; }
			if (t.TryGetComponent(out Child c)) { return c.Root; }
			return t.transform;
		}

		public static Transform Get(Transform t) {
			if (t == null) { return null; }
			if (t.TryGetComponent(out Child c)) { return c.Root; }
			return t;
		}

		public static GameObject Get(GameObject go) {
			if (go == null) { return null; }
			if (go.TryGetComponent(out Child c)) { return c.Root.gameObject; }
			return go;
		}

		public static T GetComponent<T>(Transform t) where T : Component {
			if (t == null) { return null; }
			if (t.TryGetComponent(out Child c)) { return c.Root.GetComponent<T>(); }
			return t.GetComponent<T>();
		}

		public static T GetComponent<T>(GameObject go) where T : Component {
			if (go == null) { return null; }
			if (go.TryGetComponent(out Child c)) { return c.Root.GetComponent<T>(); }
			return go.GetComponent<T>();
		}

		public static bool TryGetComponent<T>(Transform t, out T result) where T : Component {
			if (t != null && t.TryGetComponent(out Child c)) { return c.Root.TryGetComponent(out result); }
			else { result = null; return false; }
		}

		public static bool TryGetComponent<T>(GameObject go, out T result) where T : Component {
			if (go != null && go.TryGetComponent(out Child c)) { return c.Root.TryGetComponent(out result); }
			else { result = null; return false; }
		}

		public static T[] GetComponents<T>(Transform t) where T : Component {
			if (t == null) { return null; }
			if (t.TryGetComponent(out Child c)) { return c.Root.GetComponents<T>(); }
			return t.GetComponents<T>();
		}

		public static T[] GetComponents<T>(GameObject go) where T : Component {
			if (go == null) { return null; }
			if (go.TryGetComponent(out Child c)) { return c.Root.GetComponents<T>(); }
			return go.GetComponents<T>();
		}

		public static T GetComponentInChildren<T>(Transform t, bool includeInactive = false) where T : Component {
			if (t == null) { return null; }
			if (t.TryGetComponent(out Child c)) { return c.Root.GetComponentInChildren<T>(includeInactive); }
			return t.GetComponentInChildren<T>(includeInactive);
		}

		public static T GetComponentInChildren<T>(GameObject go, bool includeInactive = false) where T : Component {
			if (go == null) { return null; }
			if (go.TryGetComponent(out Child c)) { return c.Root.GetComponentInChildren<T>(includeInactive); }
			return go.GetComponentInChildren<T>(includeInactive);
		}

		public static bool TryGetComponentInChildren<T>(Transform t, out T result, bool includeInactive = false) where T : Component {
			if (t != null && t.TryGetComponent(out Child c)) { result = c.Root.GetComponentInChildren<T>(includeInactive); return result != null; }
			else { result = null; return false; }
		}

		public static bool TryGetComponentInChildren<T>(GameObject go, out T result, bool includeInactive = false) where T : Component {
			if (go != null && go.TryGetComponent(out Child c)) { result = c.Root.GetComponentInChildren<T>(includeInactive); return result != null; }
			else { result = null; return false; }
		}

		public static T[] GetComponentsInChildren<T>(Transform t, bool includeInactive = false) where T : Component {
			if (t == null) { return null; }
			if (t.TryGetComponent(out Child c)) { return c.Root.GetComponentsInChildren<T>(includeInactive); }
			return t.GetComponentsInChildren<T>(includeInactive);
		}

		public static T[] GetComponentsInChildren<T>(GameObject go, bool includeInactive = false) where T : Component {
			if (go == null) { return null; }
			if (go.TryGetComponent(out Child c)) { return c.Root.GetComponentsInChildren<T>(includeInactive); }
			return go.GetComponentsInChildren<T>(includeInactive);
		}
	}

}