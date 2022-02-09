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
			protected int lastFrameCalled = -1;

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
				lastFrameCalled = Time.frameCount;
				if (functions.Count == 0) { enabled = false; return; }
			}

			//

			public void Add(GameObject target, System.Func<bool> function, bool callThisFrame) {
				functions.Add(new Function() { target = target, func = function });
				if (functions.Count == 1) { enabled = true; }
				if (callThisFrame && lastFrameCalled == Time.frameCount) { function(); }
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

			public void Remove(GameObject target, System.Func<bool> function) {
#if !UNITY_EDITOR
				functions.RemoveAll(f => f.target == target && f.func == function);
#else
				Updater.functionsCount -= functions.RemoveAll(f => f.target == target && f.func == function);
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

		/// <summary>
		/// Call a function every frame in Update() or LateUpdate().
		/// </summary>
		/// <param name="target">If this gameObject ceases to exist, the function stops getting called.</param>
		/// <param name="function">The function to call every frame; return false to stop</param>
		/// <param name="callThisFrame">Is this function already executed in the current frame?</param>
		/// <param name="lateUpdate">Use LateUpdate()?</param>
		public static void Add(GameObject target, System.Func<bool> function, bool callThisFrame = true, bool lateUpdate = false) {
			if (target == null || function == null) { return; }
			if (lateUpdate) {
				if (updaterLate == null) {
					if (gameObject == null) { gameObject = new GameObject("<UPDATER>"); }
					updaterLate = gameObject.AddComponent<UpdaterLate>();
				}
				updaterLate.Add(target, function, callThisFrame);
			}
			else {
				if (updaterNormal == null) {
					if (gameObject == null) { gameObject = new GameObject("<UPDATER>"); }
					updaterNormal = gameObject.AddComponent<UpdaterNormal>();
				}
				updaterNormal.Add(target, function, callThisFrame);
			}
		}
		
		/// <summary>
		/// Call a function every frame in Update() or LateUpdate().
		/// </summary>
		/// <param name="function">The function to call every frame; return false to stop</param>
		/// <param name="callThisFrame">Is this function already executed in the current frame?</param>
		/// <param name="lateUpdate">Use LateUpdate()?</param>
		public static void Add(System.Func<bool> function, bool callThisFrame = true, bool lateUpdate = false) {
			if (function == null) { return; }
			if (lateUpdate) {
				if (updaterLate == null) {
					if (gameObject == null) { gameObject = new GameObject("<UPDATER>"); }
					updaterLate = gameObject.AddComponent<UpdaterLate>();
				}
				updaterLate.Add(gameObject, function, callThisFrame);
			}
			else {
				if (updaterNormal == null) {
					if (gameObject == null) { gameObject = new GameObject("<UPDATER>"); }
					updaterNormal = gameObject.AddComponent<UpdaterNormal>();
				}
				updaterNormal.Add(gameObject, function, callThisFrame);
			}
		}

		/// <summary>
		/// Stop calling functions associated with this target
		/// </summary>
		/// <param name="target">The gameObject that was set as target when Add() was called</param>
		/// <param name="lateUpdate">If true only removes functions from LateUpdate(), if false only functions from Update() are removed</param>
		public static void Remove(GameObject target, bool lateUpdate) {
			if (lateUpdate && updaterLate != null) { updaterLate.Remove(target); }
			else if (!lateUpdate && updaterNormal == null) { updaterNormal.Remove(target); }
		}
		
		/// <summary>
		/// Stop calling functions associated with this target in both Update() and LateUpdate()
		/// </summary>
		/// <param name="target">The gameObject that was set as target when Add() was called</param>
		public static void Remove(GameObject target) {
			if (updaterLate != null) { updaterLate.Remove(target); }
			if (updaterNormal != null) { updaterNormal.Remove(target); }
		}
		
		/// <summary>
		/// Stop calling a certain function
		/// </summary>
		/// <param name="target">The gameObject that was set as target when Add() was called</param>
		/// <param name="function">The function to be removed</param>
		/// <param name="lateUpdate">If true only removes the function from LateUpdate(), if false only from Update()</param>
		public static void Remove(System.Func<bool> function, bool lateUpdate) {
			if (lateUpdate && updaterLate != null) { updaterLate.Remove(function); }
			else if (!lateUpdate && updaterNormal == null) { updaterNormal.Remove(function); }
		}
		
		/// <summary>
		/// Stop calling a certain function in both Update() and LateUpdate()
		/// </summary>
		/// <param name="function">The function to be removed</param>
		public static void Remove(System.Func<bool> function) {
			if (updaterLate != null) { updaterLate.Remove(function); }
			if (updaterNormal != null) { updaterNormal.Remove(function); }
		}
		
		/// <summary>
		/// Stop calling a certain function
		/// </summary>
		/// <param name="function">The function to be removed</param>
		/// <param name="target">The gameObject that was set as target when Add() was called</param>
		/// <param name="lateUpdate">If true only removes the function from LateUpdate(), if false only from Update()</param>
		public static void Remove(GameObject target, System.Func<bool> function, bool lateUpdate) {
			if (lateUpdate && updaterLate != null) { updaterLate.Remove(target, function); }
			else if (!lateUpdate && updaterNormal == null) { updaterNormal.Remove(target, function); }
		}
		
		/// <summary>
		/// Stop calling a certain function in both Update() and LateUpdate()
		/// </summary>
		/// <param name="target">The gameObject that was set as target when Add() was called</param>
		/// <param name="function">The function to be removed</param>
		public static void Remove(GameObject target, System.Func<bool> function) {
			if (updaterLate != null) { updaterLate.Remove(target, function); }
			if (updaterNormal != null) { updaterNormal.Remove(target, function); }
		}
	}

}