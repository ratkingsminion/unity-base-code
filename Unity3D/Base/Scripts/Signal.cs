using System.Collections.Generic;

namespace RatKing.Base {

	public interface ISignal {
		void UnregisterAll();
	}

	// // normal Signals are simple wrappers for System.Action, nothing more
	// // example use:
	// public static class Signals {
	//     public static Signal<float> GameHasLoaded = new Signal<float>();
	// }
	// Signals.GameHasLoaded.Register(OnGameHasLoaded); // -> void OnGameHasLoaded(float timeNeededToLoad) { ... }
	// Signals.GameHasLoaded.Broadcast(60f);
	// // Don't forget to unregister when destroying the game object!

	// zero parameters
	public class Signal : ISignal {
		System.Action actions;

		//

		public void Register(System.Action action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to register null"); return; }
			actions += action;
		}

		public void Unregister(System.Action action) {
			if (action == null) {UnityEngine.Debug.LogError("trying to unregister null");  return; }
			actions -= action;
		}

		public void UnregisterAll() {
			actions = null;
		}

		public void Broadcast() {
			if (actions != null) {
				actions();
			}
		}
	}

	// one parameter
	public class Signal<T1> : ISignal {
		System.Action<T1> actions;

		//

		public void Register(System.Action<T1> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to register null"); return; }
			actions += action;
		}

		public void Unregister(System.Action<T1> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			actions -= action;
		}

		public void UnregisterAll() {
			actions = null;
		}

		public void Broadcast(T1 value1) {
			if (actions != null) {
				actions(value1);
			}
		}
	}

	// two parameters
	public class Signal<T1, T2> : ISignal {
		System.Action<T1, T2> actions;

		//

		public void Register(System.Action<T1, T2> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to register null"); return; }
			actions += action;
		}

		public void Unregister(System.Action<T1, T2> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			actions -= action;
		}

		public void UnregisterAll() {
			actions = null;
		}

		public void Broadcast(T1 value1, T2 value2) {
			if (actions != null) {
				actions(value1, value2);
			}
		}
	}

	// three parameters
	public class Signal<T1, T2, T3> : ISignal {
		System.Action<T1, T2, T3> actions;

		//

		public void Register(System.Action<T1, T2, T3> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to register null"); return; }
			actions += action;
		}

		public void Unregister(System.Action<T1, T2, T3> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			actions -= action;
		}

		public void UnregisterAll() {
			actions = null;
		}

		public void Broadcast(T1 value1, T2 value2, T3 value3) {
			if (actions != null) {
				actions(value1, value2, value3);
			}
		}
	}

	//

	// // Targeted Signals are wrappers for System.Action, but always with a target parameter
	// // example use:
	// public static class Signals {
	//     public static TargetedSignal<Collider, Vector3> PlayerClicks = new TargetedSignal<Collider, Vector3>();
	// }
	// Signals.PlayerClicks.Register(myCollider, OnPlayerClicks); // -> void OnPlayerClicks(Vector3 point) { ... }
	// Signals.PlayerClicks.Register(OnPlayerClicks); // -> void OnPlayerClicks(Collider collider, Vector3 point) { ... }
	// Signals.PlayerClicks.Broadcast(raycastHit.collider, raycastHit.point);
	// // Don't forget to unregister when destroying the game object!

	[System.Flags]
	public enum SignalCallee {
		TargetOnly = 1,
		GeneralOnly = 2,
		Both = 3
	}

	// no parameters
	public class TargetedSignal<TTarget> : ISignal {
		HashSet<TTarget> targets = new HashSet<TTarget>();
		Dictionary<TTarget, System.Action> targetedActions = new Dictionary<TTarget, System.Action>();
		System.Action<TTarget> generalActions;

		//

		public void Register(TTarget target, System.Action action) {
			if (target == null || action == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to register null"); return; }
			if (!targetedActions.TryGetValue(target, out var actions)) { targets.Add(target); targetedActions.Add(target, action); }
			else { targetedActions[target] = actions + action; }
		}

		public void Register(System.Action<TTarget> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to register null"); return; }
			generalActions += action;
		}

		public void Unregister(TTarget target, System.Action action) {
			if (target == null || action == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			if (!targetedActions.TryGetValue(target, out var actions)) { return; }
			actions -= action;
			if (actions != null) { targetedActions[target] = actions; }
			else { targets.Remove(target); targetedActions.Remove(target); }
		}

		public void Unregister(System.Action action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			foreach (var target in targets) {
				var actions = targetedActions[target];
				actions -= action;
				if (actions != null) { targetedActions[target] = actions; }
				else { targetedActions.Remove(target); }
			}
			targets.RemoveWhere(t => !targetedActions.ContainsKey(t));
		}

		public void Unregister(System.Action<TTarget> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			generalActions -= action;
		}

		public void Unregister(TTarget target) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			targets.Remove(target);
			targetedActions.Remove(target);
		}
		
		public void UnregisterAll() {
			targets.Clear();
			targetedActions.Clear();
			generalActions = null;
		}

		public void Broadcast(TTarget target, SignalCallee callee = SignalCallee.Both) {
			if (target == null || (callee & SignalCallee.Both) == 0 || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to broadcast null"); return; }
			if ((callee & SignalCallee.TargetOnly) != 0 && targetedActions.TryGetValue(target, out var actions)) {
				actions();
			}
			if ((callee & SignalCallee.GeneralOnly) != 0 && generalActions != null) {
				generalActions(target);
			}
		}
	}

	// one parameter
	public class TargetedSignal<TTarget, T1> : ISignal {
		HashSet<TTarget> targets = new HashSet<TTarget>();
		Dictionary<TTarget, System.Action<T1>> targetedActions = new Dictionary<TTarget, System.Action<T1>>();
		System.Action<TTarget, T1> generalActions;

		//

		public void Register(TTarget target, System.Action<T1> action) {
			if (target == null || action == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to register null"); return; }
			if (!targetedActions.TryGetValue(target, out var actions)) { targets.Add(target); targetedActions.Add(target, action); }
			else { targetedActions[target] = actions + action; }
		}

		public void Register(System.Action<TTarget, T1> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to register null"); return; }
			generalActions += action;
		}

		public void Unregister(TTarget target, System.Action<T1> action) {
			if (target == null || action == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			if (!targetedActions.TryGetValue(target, out var actions)) { return; }
			actions -= action;
			if (actions != null) { targetedActions[target] = actions; }
			else { targets.Remove(target); targetedActions.Remove(target); }
		}

		public void Unregister(System.Action<T1> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			foreach (var target in targets) {
				var actions = targetedActions[target];
				actions -= action;
				if (actions != null) { targetedActions[target] = actions; }
				else { targetedActions.Remove(target); }
			}
			targets.RemoveWhere(t => !targetedActions.ContainsKey(t));
		}

		public void Unregister(System.Action<TTarget, T1> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			generalActions -= action;
		}

		public void Unregister(TTarget target) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			targets.Remove(target);
			targetedActions.Remove(target);
		}
		
		public void UnregisterAll() {
			targets.Clear();
			targetedActions.Clear();
			generalActions = null;
		}

		public void Broadcast(TTarget target, T1 value1, SignalCallee callee = SignalCallee.Both) {
			if (target == null || (callee & SignalCallee.Both) == 0 || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to broadcast null"); return; }
			if ((callee & SignalCallee.TargetOnly) != 0 && targetedActions.TryGetValue(target, out var actions)) {
				actions(value1);
			}
			if ((callee & SignalCallee.GeneralOnly) != 0 && generalActions != null) {
				generalActions(target, value1);
			}
		}
	}

	// two parameters
	public class TargetedSignal<TTarget, T1, T2> : ISignal {
		HashSet<TTarget> targets = new HashSet<TTarget>();
		Dictionary<TTarget, System.Action<T1, T2>> targetedActions = new Dictionary<TTarget, System.Action<T1, T2>>();
		System.Action<TTarget, T1, T2> generalActions;

		//

		public void Register(TTarget target, System.Action<T1, T2> action) {
			if (target == null || action == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to register null"); return; }
			if (!targetedActions.TryGetValue(target, out var actions)) { targets.Add(target); targetedActions.Add(target, action); }
			else { targetedActions[target] = actions + action; }
		}

		public void Register(System.Action<TTarget, T1, T2> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to register null"); return; }
			generalActions += action;
		}

		public void Unregister(TTarget target, System.Action<T1, T2> action) {
			if (target == null || action == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			if (!targetedActions.TryGetValue(target, out var actions)) { return; }
			actions -= action;
			if (actions != null) { targetedActions[target] = actions; }
			else { targets.Remove(target); targetedActions.Remove(target); }
		}

		public void Unregister(System.Action<T1, T2> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			foreach (var target in targets) {
				var actions = targetedActions[target];
				actions -= action;
				if (actions != null) { targetedActions[target] = actions; }
				else { targetedActions.Remove(target); }
			}
			targets.RemoveWhere(t => !targetedActions.ContainsKey(t));
		}

		public void Unregister(System.Action<TTarget, T1, T2> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			generalActions -= action;
		}

		public void Unregister(TTarget target) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			targets.Remove(target);
			targetedActions.Remove(target);
		}
		
		public void UnregisterAll() {
			targets.Clear();
			targetedActions.Clear();
			generalActions = null;
		}

		public void Broadcast(TTarget target, T1 value1, T2 value2, SignalCallee callee = SignalCallee.Both) {
			if (target == null || (callee & SignalCallee.Both) == 0 || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to broadcast null"); return; }
			if ((callee & SignalCallee.TargetOnly) != 0 && targetedActions.TryGetValue(target, out var actions)) {
				actions(value1, value2);
			}
			if ((callee & SignalCallee.GeneralOnly) != 0 && generalActions != null) {
				generalActions(target, value1, value2);
			}
		}
	}

	// three parameters
	public class TargetedSignal<TTarget, T1, T2, T3> : ISignal {
		HashSet<TTarget> targets = new HashSet<TTarget>();
		Dictionary<TTarget, System.Action<T1, T2, T3>> targetedActions = new Dictionary<TTarget, System.Action<T1, T2, T3>>();
		System.Action<TTarget, T1, T2, T3> generalActions;

		//

		public void Register(TTarget target, System.Action<T1, T2, T3> action) {
			if (target == null || action == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to register null"); return; }
			if (!targetedActions.TryGetValue(target, out var actions)) { targets.Add(target); targetedActions.Add(target, action); }
			else { targetedActions[target] = actions + action; }
		}

		public void Register(System.Action<TTarget, T1, T2, T3> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to register null"); return; }
			generalActions += action;
		}

		public void Unregister(TTarget target, System.Action<T1, T2, T3> action) {
			if (target == null || action == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			if (!targetedActions.TryGetValue(target, out var actions)) { return; }
			actions -= action;
			if (actions != null) { targetedActions[target] = actions; }
			else { targets.Remove(target); targetedActions.Remove(target); }
		}

		public void Unregister(System.Action<T1, T2, T3> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			foreach (var target in targets) {
				var actions = targetedActions[target];
				actions -= action;
				if (actions != null) { targetedActions[target] = actions; }
				else { targetedActions.Remove(target); }
			}
			targets.RemoveWhere(t => !targetedActions.ContainsKey(t));
		}

		public void Unregister(System.Action<TTarget, T1, T2, T3> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			generalActions -= action;
		}

		public void Unregister(TTarget target) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			targets.Remove(target);
			targetedActions.Remove(target);
		}
		
		public void UnregisterAll() {
			targets.Clear();
			targetedActions.Clear();
			generalActions = null;
		}

		public void Broadcast(TTarget target, T1 value1, T2 value2, T3 value3, SignalCallee callee = SignalCallee.Both) {
			if (target == null || (callee & SignalCallee.Both) == 0 || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to broadcast null"); return; }
			if ((callee & SignalCallee.TargetOnly) != 0 && targetedActions.TryGetValue(target, out var actions)) {
				actions(value1, value2, value3);
			}
			if ((callee & SignalCallee.GeneralOnly) != 0 && generalActions != null) {
				generalActions(target, value1, value2, value3);
			}
		}
	}

	// four parameters
	public class TargetedSignal<TTarget, T1, T2, T3, T4> : ISignal {
		HashSet<TTarget> targets = new HashSet<TTarget>();
		Dictionary<TTarget, System.Action<T1, T2, T3, T4>> targetedActions = new Dictionary<TTarget, System.Action<T1, T2, T3, T4>>();
		System.Action<TTarget, T1, T2, T3, T4> generalActions;

		//

		public void Register(TTarget target, System.Action<T1, T2, T3, T4> action) {
			if (target == null || action == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to register null"); return; }
			if (!targetedActions.TryGetValue(target, out var actions)) { targets.Add(target); targetedActions.Add(target, action); }
			else { targetedActions[target] = actions + action; }
		}

		public void Register(System.Action<TTarget, T1, T2, T3, T4> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to register null"); return; }
			generalActions += action;
		}

		public void Unregister(TTarget target, System.Action<T1, T2, T3, T4> action) {
			if (target == null || action == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			if (!targetedActions.TryGetValue(target, out var actions)) { return; }
			actions -= action;
			if (actions != null) { targetedActions[target] = actions; }
			else { targets.Remove(target); targetedActions.Remove(target); }
		}

		public void Unregister(System.Action<T1, T2, T3, T4> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			foreach (var target in targets) {
				var actions = targetedActions[target];
				actions -= action;
				if (actions != null) { targetedActions[target] = actions; }
				else { targetedActions.Remove(target); }
			}
			targets.RemoveWhere(t => !targetedActions.ContainsKey(t));
		}

		public void Unregister(System.Action<TTarget, T1, T2, T3, T4> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			generalActions -= action;
		}

		public void Unregister(TTarget target) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			targets.Remove(target);
			targetedActions.Remove(target);
		}
		
		public void UnregisterAll() {
			targets.Clear();
			targetedActions.Clear();
			generalActions = null;
		}

		public void Broadcast(TTarget target, T1 value1, T2 value2, T3 value3, T4 value4, SignalCallee callee = SignalCallee.Both) {
			if (target == null || (callee & SignalCallee.Both) == 0 || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to broadcast null"); return; }
			if ((callee & SignalCallee.TargetOnly) != 0 && targetedActions.TryGetValue(target, out var actions)) {
				actions(value1, value2, value3, value4);
			}
			if ((callee & SignalCallee.GeneralOnly) != 0 && generalActions != null) {
				generalActions(target, value1, value2, value3, value4);
			}
		}
	}
}