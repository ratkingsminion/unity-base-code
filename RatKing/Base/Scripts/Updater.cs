using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RatKing.Base {
	
	public class Updater {

		static GameObject gameObject = null;
		static UpdaterLate updaterLate = null;
		static UpdaterNormal updaterNormal = null;
#if UNITY_EDITOR
		static int functionsCount = 0;
#endif

		[DefaultExecutionOrder(-10000)]
		public class UpdaterBehaviour : MonoBehaviour {
			protected struct Function {
				public GameObject target;
				public System.Func<bool> func;
			}

			protected List<Function> functions = new List<Function>();

			protected void Iteration() {
				for (int i = 0, count = functions.Count; i < count; ++i) {
					var f = functions[i];
					if (f.target == null || !f.func()) {
						functions.RemoveAt(i);
						--i;
						--count;
#if UNITY_EDITOR
						--Updater.functionsCount;
						Updater.gameObject.name = $"<UPDATER {functionsCount}>";
#endif
					}
				}
				if (functions.Count == 0) { enabled = false; return; }
			}

			//

			public void Add(GameObject target, System.Func<bool> function) {
				functions.Add(new Function() { target = target, func = function });
				if (functions.Count == 1) { enabled = true; }
#if UNITY_EDITOR
				++Updater.functionsCount;
				Updater.gameObject.name = $"<UPDATER {functionsCount}>";
#endif
			}

			public void Remove(GameObject target) {
#if !UNITY_EDITOR
				functions.RemoveAll(f => f.target == target);
#else
				Updater.functionsCount -= functions.RemoveAll(f => f.target == target);
				Updater.gameObject.name = $"<UPDATER {functionsCount}>";
#endif
			}

			public void Remove(System.Func<bool> function) {
#if !UNITY_EDITOR
				functions.RemoveAll(f => f.func == function);
#else
				Updater.functionsCount -= functions.RemoveAll(f => f.func == function);
				Updater.gameObject.name = $"<UPDATER {functionsCount}>";
#endif
			}
		}

		[DefaultExecutionOrder(-10000)]
		public class UpdaterNormal : UpdaterBehaviour {
			void Update() {
				Iteration();
			}
		}

		[DefaultExecutionOrder(-10000)]
		public class UpdaterLate : UpdaterBehaviour {
			void LateUpdate() {
				Iteration();
			}
		}

		//

		public static void Add(GameObject target, System.Func<bool> function, bool late = false) {
			if (target == null || function == null) { return; }
			if (late) {
				if (updaterLate == null) {
					if (gameObject == null) { gameObject = new GameObject("<UPDATER>"); }
					updaterLate = gameObject.AddComponent<UpdaterLate>();
				}
				updaterLate.Add(target, function);
			}
			else {
				if (updaterNormal == null) {
					if (gameObject == null) { gameObject = new GameObject("<UPDATER>"); }
					updaterNormal = gameObject.AddComponent<UpdaterNormal>();
				}
				updaterNormal.Add(target, function);
			}
		}

		public static void Remove(GameObject target, bool late = false) {
			if (late) {
				if (updaterLate == null) { return; }
				updaterLate.Remove(target);
			}
			else {
				if (updaterNormal == null) { return; }
				updaterNormal.Remove(target);
			}
		}

		public static void Remove(System.Func<bool> function, bool late = false) {
			if (late) {
				if (updaterLate == null) { return; }
				updaterLate.Remove(function);
			}
			else {
				if (updaterNormal == null) { return; }
				updaterNormal.Remove(function);
			}
		}
	}

}