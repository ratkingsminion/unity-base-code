using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RatKing.Base {
	
	public class Updater {

		public enum Type {
			Normal,
			Late,
			Fixed
		}

		static GameObject gameObject = null;
		static UpdaterLate updaterLate = null;
		static UpdaterNormal updaterNormal = null;
		static UpdaterFixed updaterFixed = null;
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

		[DefaultExecutionOrder(-10000)]
		public class UpdaterFixed : UpdaterBehaviour {
			void FixedUpdate() {
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
		/// <param name="type">The Updater Type to add</param>
		public static void Add(GameObject target, System.Func<bool> function, bool callThisFrame = true, Type type = Type.Normal) {
			if (target == null || function == null) { return; }
			if (type == Type.Normal) {
				if (updaterNormal == null) {
					if (gameObject == null) { gameObject = new GameObject("<UPDATER NORMAL>"); }
					updaterNormal = gameObject.AddComponent<UpdaterNormal>();
				}
				updaterNormal.Add(target, function, callThisFrame);
			}
			else if (type == Type.Late) {
				if (updaterLate == null) {
					if (gameObject == null) { gameObject = new GameObject("<UPDATER LATE>"); }
					updaterLate = gameObject.AddComponent<UpdaterLate>();
				}
				updaterLate.Add(target, function, callThisFrame);
			}
			else { // if (type == Type.Fixed) {
				if (updaterFixed == null) {
					if (gameObject == null) { gameObject = new GameObject("<UPDATER FIXED>"); }
					updaterFixed = gameObject.AddComponent<UpdaterFixed>();
				}
				updaterFixed.Add(target, function, callThisFrame);
			}
		}
		
		/// <summary>
		/// Call a function every frame in Update() or LateUpdate().
		/// </summary>
		/// <param name="function">The function to call every frame; return false to stop</param>
		/// <param name="callThisFrame">Is this function already executed in the current frame?</param>
		/// <param name="type">The Updater Type to add</param>
		public static void Add(System.Func<bool> function, bool callThisFrame = true, Type type = Type.Normal) {
			if (function == null) { return; }
			if (type == Type.Normal) {
				if (updaterNormal == null) {
					if (gameObject == null) { gameObject = new GameObject("<UPDATER NORMAL>"); }
					updaterNormal = gameObject.AddComponent<UpdaterNormal>();
				}
				updaterNormal.Add(gameObject, function, callThisFrame);
			}
			else if (type == Type.Late) {
				if (updaterLate == null) {
					if (gameObject == null) { gameObject = new GameObject("<UPDATER LATE>"); }
					updaterLate = gameObject.AddComponent<UpdaterLate>();
				}
				updaterLate.Add(gameObject, function, callThisFrame);
			}
			else { // if (type == Type.Fixed) {
				if (updaterFixed == null) {
					if (gameObject == null) { gameObject = new GameObject("<UPDATER FIXED>"); }
					updaterFixed = gameObject.AddComponent<UpdaterFixed>();
				}
				updaterFixed.Add(gameObject, function, callThisFrame);
			}
		}

		/// <summary>
		/// Helper method to call something in UpdateLate() our from anywhere
		/// </summary>
		/// <param name="target">If this gameObject ceases to exist, the function will not be called.</param>
		/// <param name="function">The function to call every frame; return false to stop</param>
		/// <param name="callThisFrame">Is this function already executed in the current frame?</param>
		public static void CallLateOnce(GameObject target, System.Action function, bool callThisFrame = true) {
			if (target == null || function == null) { return; }
			if (updaterLate == null) {
				if (gameObject == null) { gameObject = new GameObject("<UPDATER LATE>"); }
				updaterLate = gameObject.AddComponent<UpdaterLate>();
			}
			updaterLate.Add(target, () => { function(); return false; }, callThisFrame);
		}
		
		/// <summary>
		/// Helper method to call something in UpdateLate() our from anywhere
		/// </summary>
		/// <param name="function">The function to call every frame; return false to stop</param>
		/// <param name="callThisFrame">Is this function already executed in the current frame?</param>
		public static void CallLateOnce(System.Action function, bool callThisFrame = true) {
			if (function == null) { return; }
			if (updaterLate == null) {
				if (gameObject == null) { gameObject = new GameObject("<UPDATER LATE>"); }
				updaterLate = gameObject.AddComponent<UpdaterLate>();
			}
			updaterLate.Add(gameObject, () => { function(); return false; }, callThisFrame);
		}

		/// <summary>
		/// Stop calling functions associated with this target
		/// </summary>
		/// <param name="target">The gameObject that was set as target when Add() was called</param>
		/// <param name="type">The Updater Type to remove</param>
		public static void Remove(GameObject target, Type type) {
			switch (type) {
				case Type.Normal: if (updaterNormal != null) { updaterNormal.Remove(target); } break;
				case Type.Late: if (updaterLate != null) { updaterLate.Remove(target); } break;
				case Type.Fixed: if (updaterFixed != null) { updaterFixed.Remove(target); } break;
			}
		}
		
		/// <summary>
		/// Stop calling functions associated with this target in both Update() and LateUpdate()
		/// </summary>
		/// <param name="target">The gameObject that was set as target when Add() was called</param>
		public static void Remove(GameObject target) {
			if (updaterNormal != null) { updaterNormal.Remove(target); }
			if (updaterLate != null) { updaterLate.Remove(target); }
			if (updaterFixed != null) { updaterFixed.Remove(target); }
		}
		
		/// <summary>
		/// Stop calling a certain function
		/// </summary>
		/// <param name="target">The gameObject that was set as target when Add() was called</param>
		/// <param name="function">The function to be removed</param>
		/// <param name="type">The Updater Type to remove</param>
		public static void Remove(System.Func<bool> function, Type type) {
			switch (type) {
				case Type.Normal: if (updaterNormal != null) { updaterNormal.Remove(function); } break;
				case Type.Late: if (updaterLate != null) { updaterLate.Remove(function); } break;
				case Type.Fixed: if (updaterFixed != null) { updaterFixed.Remove(function); } break;
			}
		}
		
		/// <summary>
		/// Stop calling a certain function in both Update() and LateUpdate()
		/// </summary>
		/// <param name="function">The function to be removed</param>
		public static void Remove(System.Func<bool> function) {
			if (updaterNormal != null) { updaterNormal.Remove(function); }
			if (updaterLate != null) { updaterLate.Remove(function); }
			if (updaterFixed != null) { updaterFixed.Remove(function); }
		}
		
		/// <summary>
		/// Stop calling a certain function
		/// </summary>
		/// <param name="function">The function to be removed</param>
		/// <param name="target">The gameObject that was set as target when Add() was called</param>
		/// <param name="type">The Updater Type to remove</param>
		public static void Remove(GameObject target, System.Func<bool> function, Type type) {
			switch (type) {
				case Type.Normal: if (updaterNormal != null) { updaterNormal.Remove(target, function); } break;
				case Type.Late: if (updaterLate != null) { updaterLate.Remove(target, function); } break;
				case Type.Fixed: if (updaterFixed != null) { updaterFixed.Remove(target, function); } break;
			}
		}
		
		/// <summary>
		/// Stop calling a certain function in both Update() and LateUpdate()
		/// </summary>
		/// <param name="target">The gameObject that was set as target when Add() was called</param>
		/// <param name="function">The function to be removed</param>
		public static void Remove(GameObject target, System.Func<bool> function) {
				if (updaterNormal != null) { updaterNormal.Remove(target, function); }
				if (updaterLate != null) { updaterLate.Remove(target, function); }
				if (updaterFixed != null) { updaterFixed.Remove(target, function); }
		}
	}

}