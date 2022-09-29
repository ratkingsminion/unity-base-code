using System.Collections.Generic;

namespace RatKing.Base {

	public interface ISignal {
		void UnregisterAll();
	}

	// // Normal Signals are simple wrappers for System.Action, nothing more.
	// // Example usage:
	// public static class Signals {
	//     public readonly static Signal<float> GameHasLoaded = new Signal<float>();
	// }
	// Signals.GameHasLoaded.Register(OnGameHasLoaded); // -> void OnGameHasLoaded(float timeNeededToLoad) { ... }
	// Signals.GameHasLoaded.Broadcast(60f);
	// // Don't forget to unregister when destroying the game object!

	// zero parameters
	public class Signal : ISignal {
		protected System.Action actions;

		//

		public virtual void Register(System.Action action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to register null"); return; }
			actions += action;
		}

		public virtual void Unregister(System.Action action) {
			if (action == null) {UnityEngine.Debug.LogError("trying to unregister null");  return; }
			actions -= action;
		}

		public virtual void UnregisterAll() {
			actions = null;
		}

		public virtual void Broadcast() {
			if (actions != null) {
				actions();
			}
		}
	}

	// one parameter
	public class Signal<T> : ISignal {
		protected System.Action<T> actions;

		//

		public virtual void Register(System.Action<T> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to register null"); return; }
			actions += action;
		}

		public virtual void Unregister(System.Action<T> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			actions -= action;
		}

		public virtual void UnregisterAll() {
			actions = null;
		}

		public virtual void Broadcast(T value) {
			if (actions != null) {
				actions(value);
			}
		}
	}

	// two parameters
	public class Signal<T1, T2> : ISignal {
		protected System.Action<T1, T2> actions;

		//

		public virtual void Register(System.Action<T1, T2> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to register null"); return; }
			actions += action;
		}

		public virtual void Unregister(System.Action<T1, T2> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			actions -= action;
		}

		public virtual void UnregisterAll() {
			actions = null;
		}

		public virtual void Broadcast(T1 value1, T2 value2) {
			if (actions != null) {
				actions(value1, value2);
			}
		}
	}

	// three parameters
	public class Signal<T1, T2, T3> : ISignal {
		protected System.Action<T1, T2, T3> actions;

		//

		public virtual void Register(System.Action<T1, T2, T3> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to register null"); return; }
			actions += action;
		}

		public virtual void Unregister(System.Action<T1, T2, T3> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			actions -= action;
		}

		public virtual void UnregisterAll() {
			actions = null;
		}

		public virtual void Broadcast(T1 value1, T2 value2, T3 value3) {
			if (actions != null) {
				actions(value1, value2, value3);
			}
		}
	}

	// four parameters
	public class Signal<T1, T2, T3, T4> : ISignal {
		protected System.Action<T1, T2, T3, T4> actions;

		//

		public virtual void Register(System.Action<T1, T2, T3, T4> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to register null"); return; }
			actions += action;
		}

		public virtual void Unregister(System.Action<T1, T2, T3, T4> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			actions -= action;
		}

		public virtual void UnregisterAll() {
			actions = null;
		}

		public virtual void Broadcast(T1 value1, T2 value2, T3 value3, T4 value4) {
			if (actions != null) {
				actions(value1, value2, value3, value4);
			}
		}
	}

	//

	// // Targeted Signals are wrappers for System.Action, but always with a target parameter.
	// // Example usage:
	// public static class Signals {
	//     public readonly static TargetedSignal<Collider, Vector3> PlayerClicks = new TargetedSignal<Collider, Vector3>();
	// }
	// Signals.PlayerClicks.Register(myCollider, OnPlayerClicks); // -> void OnPlayerClicks(Vector3 point) { ... }
	// Signals.PlayerClicks.Register(OnPlayerClicks); // -> void OnPlayerClicks(Collider collider, Vector3 point) { ... }
	// Signals.PlayerClicks.Broadcast(raycastHit.collider, raycastHit.point);
	// // Don't forget to unregister when destroying the game object!

	// no parameters
	public class TargetedSignal<TTarget> : ISignal {
		protected readonly HashSet<TTarget> targets = new HashSet<TTarget>();
		protected readonly Dictionary<TTarget, System.Action<TTarget>> targetedActions = new Dictionary<TTarget, System.Action<TTarget>>();
		protected readonly HashSet<TTarget> freeTargets = new HashSet<TTarget>();
		protected readonly Dictionary<TTarget, System.Action> freeTargetedActions = new Dictionary<TTarget, System.Action>();
		protected System.Action<TTarget> generalActions;

		//

		public virtual void Register(TTarget target, System.Action<TTarget> action) {
			if (target == null || action == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to register null"); return; }
			if (!targetedActions.TryGetValue(target, out var actions)) { targets.Add(target); targetedActions.Add(target, action); }
			else { targetedActions[target] = actions + action; }
		}

		public virtual void Register(TTarget target, System.Action action) {
			if (target == null || action == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to register null"); return; }
			if (!freeTargetedActions.TryGetValue(target, out var actions)) { freeTargets.Add(target); freeTargetedActions.Add(target, action); }
			else { freeTargetedActions[target] = actions + action; }
		}

		public virtual void Register(System.Action<TTarget> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to register null"); return; }
			generalActions += action;
		}

		public virtual void Unregister(TTarget target, System.Action<TTarget> action) {
			if (target == null || action == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			if (!targetedActions.TryGetValue(target, out var actions)) { return; }
			actions -= action;
			if (actions != null) { targetedActions[target] = actions; }
			else { targets.Remove(target); targetedActions.Remove(target); }
		}

		public virtual void Unregister(TTarget target, System.Action action) {
			if (target == null || action == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			if (!freeTargetedActions.TryGetValue(target, out var actions)) { return; }
			actions -= action;
			if (actions != null) { freeTargetedActions[target] = actions; }
			else { freeTargets.Remove(target); freeTargetedActions.Remove(target); }
		}

		public virtual void Unregister(System.Action action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			foreach (var target in freeTargets) {
				var actions = freeTargetedActions[target];
				actions -= action;
				if (actions != null) { freeTargetedActions[target] = actions; }
				else { freeTargetedActions.Remove(target); }
			}
			freeTargets.RemoveWhere(t => !freeTargetedActions.ContainsKey(t));
		}

		public virtual void Unregister(System.Action<TTarget> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			foreach (var target in targets) {
				var actions = targetedActions[target];
				actions -= action;
				if (actions != null) { targetedActions[target] = actions; }
				else { targetedActions.Remove(target); }
			}
			targets.RemoveWhere(t => !targetedActions.ContainsKey(t));
			generalActions -= action;
		}

		public virtual void Unregister(TTarget target) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			targets.Remove(target);
			targetedActions.Remove(target);
			freeTargets.Remove(target);
			freeTargetedActions.Remove(target);
		}
		
		public virtual void UnregisterAll() {
			targets.Clear();
			targetedActions.Clear();
			freeTargets.Clear();
			freeTargetedActions.Clear();
			generalActions = null;
		}

		public virtual void Broadcast(TTarget target) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to broadcast null"); return; }
			if (freeTargetedActions.TryGetValue(target, out var freeActions)) { freeActions(); }
			if (targetedActions.TryGetValue(target, out var actions)) { actions(target); }
			if (generalActions != null) { generalActions(target); }
		}
	}

	// one parameter
	public class TargetedSignal<TTarget, T> : ISignal {
		protected readonly HashSet<TTarget> targets = new HashSet<TTarget>();
		protected readonly Dictionary<TTarget, System.Action<TTarget, T>> targetedActions = new Dictionary<TTarget, System.Action<TTarget, T>>();
		protected readonly HashSet<TTarget> freeTargets = new HashSet<TTarget>();
		protected readonly Dictionary<TTarget, System.Action<T>> freeTargetedActions = new Dictionary<TTarget, System.Action<T>>();
		protected System.Action<TTarget, T> generalActions;

		//

		public virtual void Register(TTarget target, System.Action<TTarget, T> action) {
			if (target == null || action == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to register null"); return; }
			if (!targetedActions.TryGetValue(target, out var actions)) { targets.Add(target); targetedActions.Add(target, action); }
			else { targetedActions[target] = actions + action; }
		}

		public virtual void Register(TTarget target, System.Action<T> action) {
			if (target == null || action == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to register null"); return; }
			if (!freeTargetedActions.TryGetValue(target, out var actions)) { freeTargets.Add(target); freeTargetedActions.Add(target, action); }
			else { freeTargetedActions[target] = actions + action; }
		}

		public virtual void Register(System.Action<TTarget, T> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to register null"); return; }
			generalActions += action;
		}

		public virtual void Unregister(TTarget target, System.Action<TTarget, T> action) {
			if (target == null || action == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			if (!targetedActions.TryGetValue(target, out var actions)) { return; }
			actions -= action;
			if (actions != null) { targetedActions[target] = actions; }
			else { targets.Remove(target); targetedActions.Remove(target); }
		}

		public virtual void Unregister(TTarget target, System.Action<T> action) {
			if (target == null || action == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			if (!freeTargetedActions.TryGetValue(target, out var actions)) { return; }
			actions -= action;
			if (actions != null) { freeTargetedActions[target] = actions; }
			else { freeTargets.Remove(target); freeTargetedActions.Remove(target); }
		}

		public virtual void Unregister(System.Action<T> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			foreach (var target in freeTargets) {
				var actions = freeTargetedActions[target];
				actions -= action;
				if (actions != null) { freeTargetedActions[target] = actions; }
				else { freeTargetedActions.Remove(target); }
			}
			freeTargets.RemoveWhere(t => !freeTargetedActions.ContainsKey(t));
		}

		public virtual void Unregister(System.Action<TTarget, T> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			foreach (var target in targets) {
				var actions = targetedActions[target];
				actions -= action;
				if (actions != null) { targetedActions[target] = actions; }
				else { targetedActions.Remove(target); }
			}
			targets.RemoveWhere(t => !targetedActions.ContainsKey(t));
			generalActions -= action;
		}

		public virtual void Unregister(TTarget target) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			targets.Remove(target);
			targetedActions.Remove(target);
			freeTargets.Remove(target);
			freeTargetedActions.Remove(target);
		}
		
		public virtual void UnregisterAll() {
			targets.Clear();
			targetedActions.Clear();
			freeTargets.Clear();
			freeTargetedActions.Clear();
			generalActions = null;
		}

		public virtual void Broadcast(TTarget target, T value) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to broadcast null"); return; }
			if (freeTargetedActions.TryGetValue(target, out var freeActions)) { freeActions(value); }
			if (targetedActions.TryGetValue(target, out var actions)) { actions(target, value); }
			if (generalActions != null) { generalActions(target, value); }
		}
	}

	// two parameters
	public class TargetedSignal<TTarget, T1, T2> : ISignal {
		protected readonly HashSet<TTarget> targets = new HashSet<TTarget>();
		protected readonly Dictionary<TTarget, System.Action<TTarget, T1, T2>> targetedActions = new Dictionary<TTarget, System.Action<TTarget, T1, T2>>();
		protected readonly HashSet<TTarget> freeTargets = new HashSet<TTarget>();
		protected readonly Dictionary<TTarget, System.Action<T1, T2>> freeTargetedActions = new Dictionary<TTarget, System.Action<T1, T2>>();
		protected System.Action<TTarget, T1, T2> generalActions;

		//

		public virtual void Register(TTarget target, System.Action<TTarget, T1, T2> action) {
			if (target == null || action == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to register null"); return; }
			if (!targetedActions.TryGetValue(target, out var actions)) { targets.Add(target); targetedActions.Add(target, action); }
			else { targetedActions[target] = actions + action; }
		}

		public virtual void Register(TTarget target, System.Action<T1, T2> action) {
			if (target == null || action == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to register null"); return; }
			if (!freeTargetedActions.TryGetValue(target, out var actions)) { freeTargets.Add(target); freeTargetedActions.Add(target, action); }
			else { freeTargetedActions[target] = actions + action; }
		}

		public virtual void Register(System.Action<TTarget, T1, T2> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to register null"); return; }
			generalActions += action;
		}

		public virtual void Unregister(TTarget target, System.Action<TTarget, T1, T2> action) {
			if (target == null || action == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			if (!targetedActions.TryGetValue(target, out var actions)) { return; }
			actions -= action;
			if (actions != null) { targetedActions[target] = actions; }
			else { targets.Remove(target); targetedActions.Remove(target); }
		}

		public virtual void Unregister(TTarget target, System.Action<T1, T2> action) {
			if (target == null || action == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			if (!freeTargetedActions.TryGetValue(target, out var actions)) { return; }
			actions -= action;
			if (actions != null) { freeTargetedActions[target] = actions; }
			else { freeTargets.Remove(target); freeTargetedActions.Remove(target); }
		}

		public virtual void Unregister(System.Action<T1, T2> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			foreach (var target in freeTargets) {
				var actions = freeTargetedActions[target];
				actions -= action;
				if (actions != null) { freeTargetedActions[target] = actions; }
				else { freeTargetedActions.Remove(target); }
			}
			freeTargets.RemoveWhere(t => !freeTargetedActions.ContainsKey(t));
		}

		public virtual void Unregister(System.Action<TTarget, T1, T2> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			foreach (var target in targets) {
				var actions = targetedActions[target];
				actions -= action;
				if (actions != null) { targetedActions[target] = actions; }
				else { targetedActions.Remove(target); }
			}
			targets.RemoveWhere(t => !targetedActions.ContainsKey(t));
			generalActions -= action;
		}

		public virtual void Unregister(TTarget target) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			targets.Remove(target);
			targetedActions.Remove(target);
			freeTargets.Remove(target);
			freeTargetedActions.Remove(target);
		}
		
		public virtual void UnregisterAll() {
			targets.Clear();
			targetedActions.Clear();
			freeTargets.Clear();
			freeTargetedActions.Clear();
			generalActions = null;
		}

		public virtual void Broadcast(TTarget target, T1 value1, T2 value2) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to broadcast null"); return; }
			if (freeTargetedActions.TryGetValue(target, out var freeActions)) { freeActions(value1, value2); }
			if (targetedActions.TryGetValue(target, out var actions)) { actions(target, value1, value2); }
			if (generalActions != null) { generalActions(target, value1, value2); }
		}
	}

	// three parameters
	public class TargetedSignal<TTarget, T1, T2, T3> : ISignal {
		protected readonly HashSet<TTarget> targets = new HashSet<TTarget>();
		protected readonly Dictionary<TTarget, System.Action<TTarget, T1, T2, T3>> targetedActions = new Dictionary<TTarget, System.Action<TTarget, T1, T2, T3>>();
		protected readonly HashSet<TTarget> freeTargets = new HashSet<TTarget>();
		protected readonly Dictionary<TTarget, System.Action<T1, T2, T3>> freeTargetedActions = new Dictionary<TTarget, System.Action<T1, T2, T3>>();
		protected System.Action<TTarget, T1, T2, T3> generalActions;

		//

		public virtual void Register(TTarget target, System.Action<TTarget, T1, T2, T3> action) {
			if (target == null || action == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to register null"); return; }
			if (!targetedActions.TryGetValue(target, out var actions)) { targets.Add(target); targetedActions.Add(target, action); }
			else { targetedActions[target] = actions + action; }
		}

		public virtual void Register(TTarget target, System.Action<T1, T2, T3> action) {
			if (target == null || action == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to register null"); return; }
			if (!freeTargetedActions.TryGetValue(target, out var actions)) { freeTargets.Add(target); freeTargetedActions.Add(target, action); }
			else { freeTargetedActions[target] = actions + action; }
		}

		public virtual void Register(System.Action<TTarget, T1, T2, T3> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to register null"); return; }
			generalActions += action;
		}

		public virtual void Unregister(TTarget target, System.Action<TTarget, T1, T2, T3> action) {
			if (target == null || action == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			if (!targetedActions.TryGetValue(target, out var actions)) { return; }
			actions -= action;
			if (actions != null) { targetedActions[target] = actions; }
			else { targets.Remove(target); targetedActions.Remove(target); }
		}

		public virtual void Unregister(TTarget target, System.Action<T1, T2, T3> action) {
			if (target == null || action == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			if (!freeTargetedActions.TryGetValue(target, out var actions)) { return; }
			actions -= action;
			if (actions != null) { freeTargetedActions[target] = actions; }
			else { freeTargets.Remove(target); freeTargetedActions.Remove(target); }
		}

		public virtual void Unregister(System.Action<T1, T2, T3> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			foreach (var target in freeTargets) {
				var actions = freeTargetedActions[target];
				actions -= action;
				if (actions != null) { freeTargetedActions[target] = actions; }
				else { freeTargetedActions.Remove(target); }
			}
			freeTargets.RemoveWhere(t => !freeTargetedActions.ContainsKey(t));
		}

		public virtual void Unregister(System.Action<TTarget, T1, T2, T3> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			foreach (var target in targets) {
				var actions = targetedActions[target];
				actions -= action;
				if (actions != null) { targetedActions[target] = actions; }
				else { targetedActions.Remove(target); }
			}
			targets.RemoveWhere(t => !targetedActions.ContainsKey(t));
			generalActions -= action;
		}

		public virtual void Unregister(TTarget target) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			targets.Remove(target);
			targetedActions.Remove(target);
			freeTargets.Remove(target);
			freeTargetedActions.Remove(target);
		}
		
		public virtual void UnregisterAll() {
			targets.Clear();
			targetedActions.Clear();
			freeTargets.Clear();
			freeTargetedActions.Clear();
			generalActions = null;
		}

		public virtual void Broadcast(TTarget target, T1 value1, T2 value2, T3 value3) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to broadcast null"); return; }
			if (freeTargetedActions.TryGetValue(target, out var freeActions)) { freeActions(value1, value2, value3); }
			if (targetedActions.TryGetValue(target, out var actions)) { actions(target, value1, value2, value3); }
			if (generalActions != null) { generalActions(target, value1, value2, value3); }
		}
	}

	// four parameters
	public class TargetedSignal<TTarget, T1, T2, T3, T4> : ISignal {
		protected readonly HashSet<TTarget> targets = new HashSet<TTarget>();
		protected readonly Dictionary<TTarget, System.Action<TTarget, T1, T2, T3, T4>> targetedActions = new Dictionary<TTarget, System.Action<TTarget, T1, T2, T3, T4>>();
		protected readonly HashSet<TTarget> freeTargets = new HashSet<TTarget>();
		protected readonly Dictionary<TTarget, System.Action<T1, T2, T3, T4>> freeTargetedActions = new Dictionary<TTarget, System.Action<T1, T2, T3, T4>>();
		protected System.Action<TTarget, T1, T2, T3, T4> generalActions;

		//

		public virtual void Register(TTarget target, System.Action<TTarget, T1, T2, T3, T4> action) {
			if (target == null || action == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to register null"); return; }
			if (!targetedActions.TryGetValue(target, out var actions)) { targets.Add(target); targetedActions.Add(target, action); }
			else { targetedActions[target] = actions + action; }
		}

		public virtual void Register(TTarget target, System.Action<T1, T2, T3, T4> action) {
			if (target == null || action == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to register null"); return; }
			if (!freeTargetedActions.TryGetValue(target, out var actions)) { freeTargets.Add(target); freeTargetedActions.Add(target, action); }
			else { freeTargetedActions[target] = actions + action; }
		}

		public virtual void Register(System.Action<TTarget, T1, T2, T3, T4> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to register null"); return; }
			generalActions += action;
		}

		public virtual void Unregister(TTarget target, System.Action<TTarget, T1, T2, T3, T4> action) {
			if (target == null || action == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			if (!targetedActions.TryGetValue(target, out var actions)) { return; }
			actions -= action;
			if (actions != null) { targetedActions[target] = actions; }
			else { targets.Remove(target); targetedActions.Remove(target); }
		}

		public virtual void Unregister(TTarget target, System.Action<T1, T2, T3, T4> action) {
			if (target == null || action == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			if (!freeTargetedActions.TryGetValue(target, out var actions)) { return; }
			actions -= action;
			if (actions != null) { freeTargetedActions[target] = actions; }
			else { freeTargets.Remove(target); freeTargetedActions.Remove(target); }
		}

		public virtual void Unregister(System.Action<T1, T2, T3, T4> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			foreach (var target in freeTargets) {
				var actions = freeTargetedActions[target];
				actions -= action;
				if (actions != null) { freeTargetedActions[target] = actions; }
				else { freeTargetedActions.Remove(target); }
			}
			freeTargets.RemoveWhere(t => !freeTargetedActions.ContainsKey(t));
		}

		public virtual void Unregister(System.Action<TTarget, T1, T2, T3, T4> action) {
			if (action == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			foreach (var target in targets) {
				var actions = targetedActions[target];
				actions -= action;
				if (actions != null) { targetedActions[target] = actions; }
				else { targetedActions.Remove(target); }
			}
			targets.RemoveWhere(t => !targetedActions.ContainsKey(t));
			generalActions -= action;
		}

		public virtual void Unregister(TTarget target) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			targets.Remove(target);
			targetedActions.Remove(target);
			freeTargets.Remove(target);
			freeTargetedActions.Remove(target);
		}
		
		public virtual void UnregisterAll() {
			targets.Clear();
			targetedActions.Clear();
			freeTargets.Clear();
			freeTargetedActions.Clear();
			generalActions = null;
		}

		public virtual void Broadcast(TTarget target, T1 value1, T2 value2, T3 value3, T4 value4) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to broadcast null"); return; }
			if (freeTargetedActions.TryGetValue(target, out var freeActions)) { freeActions(value1, value2, value3, value4); }
			if (targetedActions.TryGetValue(target, out var actions)) { actions(target, value1, value2, value3, value4); }
			if (generalActions != null) { generalActions(target, value1, value2, value3, value4); }
		}
	}

	//

	// // State Signals are like Signals, but they will "catch up" with a listener if they were already broadcast.
	// // ATTENTION: Use with care! If you reset your game/scene, don't forget to also reset these signals by calling Reset()!

	public class StateSignal : Signal {
		public bool WasBroadcast { get; private set; }

		public override void Register(System.Action action) {
			base.Register(action);
			if (WasBroadcast) { action(); }
		}

		public override void Broadcast() {
			base.Broadcast();
			WasBroadcast = true;
		}

		public void Reset() {
			WasBroadcast = false;
		}
	}

	public class StateSignal<T> : Signal<T> {
		public bool WasBroadcast { get; private set; }
		public T LastBroadcastValue { get; private set; }

		public override void Register(System.Action<T> action) {
			base.Register(action);
			if (WasBroadcast) { action(LastBroadcastValue); }
		}

		public override void Broadcast(T value) {
			base.Broadcast(value);
			WasBroadcast = true;
			LastBroadcastValue = value;
		}

		public void Reset() {
			WasBroadcast = false;
			LastBroadcastValue = default(T);
		}
	}

	public class StateSignal<T1, T2> : Signal<T1, T2> {
		public bool WasBroadcast { get; private set; }
		public T1 LastBroadcastValue1 { get; private set; }
		public T2 LastBroadcastValue2 { get; private set; }

		public override void Register(System.Action<T1, T2> action) {
			base.Register(action);
			if (WasBroadcast) { action(LastBroadcastValue1, LastBroadcastValue2); }
		}

		public override void Broadcast(T1 value1, T2 value2) {
			base.Broadcast(value1, value2);
			WasBroadcast = true;
			LastBroadcastValue1 = value1;
			LastBroadcastValue2 = value2;
		}

		public void Reset() {
			WasBroadcast = false;
			LastBroadcastValue1 = default(T1);
			LastBroadcastValue2 = default(T2);
		}
	}

	public class StateSignal<T1, T2, T3> : Signal<T1, T2, T3> {
		public bool WasBroadcast { get; private set; }
		public T1 LastBroadcastValue1 { get; private set; }
		public T2 LastBroadcastValue2 { get; private set; }
		public T3 LastBroadcastValue3 { get; private set; }

		public override void Register(System.Action<T1, T2, T3> action) {
			base.Register(action);
			if (WasBroadcast) { action(LastBroadcastValue1, LastBroadcastValue2, LastBroadcastValue3); }
		}

		public override void Broadcast(T1 value1, T2 value2, T3 value3) {
			base.Broadcast(value1, value2, value3);
			WasBroadcast = true;
			LastBroadcastValue1 = value1;
			LastBroadcastValue2 = value2;
			LastBroadcastValue3 = value3;
		}

		public void Reset() {
			WasBroadcast = false;
			LastBroadcastValue1 = default(T1);
			LastBroadcastValue2 = default(T2);
			LastBroadcastValue3 = default(T3);
		}
	}

	public class StateSignal<T1, T2, T3, T4> : Signal<T1, T2, T3, T4> {
		public bool WasBroadcast { get; private set; }
		public T1 LastBroadcastValue1 { get; private set; }
		public T2 LastBroadcastValue2 { get; private set; }
		public T3 LastBroadcastValue3 { get; private set; }
		public T4 LastBroadcastValue4 { get; private set; }

		public override void Register(System.Action<T1, T2, T3, T4> action) {
			base.Register(action);
			if (WasBroadcast) { action(LastBroadcastValue1, LastBroadcastValue2, LastBroadcastValue3, LastBroadcastValue4); }
		}

		public override void Broadcast(T1 value1, T2 value2, T3 value3, T4 value4) {
			base.Broadcast(value1, value2, value3, value4);
			WasBroadcast = true;
			LastBroadcastValue1 = value1;
			LastBroadcastValue2 = value2;
			LastBroadcastValue3 = value3;
			LastBroadcastValue4 = value4;
		}

		public void Reset() {
			WasBroadcast = false;
			LastBroadcastValue1 = default(T1);
			LastBroadcastValue2 = default(T2);
			LastBroadcastValue3 = default(T3);
			LastBroadcastValue4 = default(T4);
		}
	}

	//

	public class TargetedStateSignal<TTarget> : TargetedSignal<TTarget> {
		protected readonly HashSet<TTarget> targetsWereBroadcast = new HashSet<TTarget>();

		public override void Register(TTarget target, System.Action<TTarget> action) {
			base.Register(target, action);
			if (targetsWereBroadcast.Contains(target)) { action(target); }
		}

		public override void Register(TTarget target, System.Action action) {
			base.Register(target, action);
			if (targetsWereBroadcast.Contains(target)) { action(); }
		}

		public override void Register(System.Action<TTarget> action) {
			base.Register(action);
			foreach (var t in targetsWereBroadcast) { action(t); }
		}

		public override void Unregister(TTarget target, System.Action<TTarget> action) {
			base.Unregister(target, action);
			if (!targets.Contains(target) && !freeTargets.Contains(target)) { targetsWereBroadcast.Remove(target); }
		}

		public override void Unregister(TTarget target, System.Action action) {
			base.Unregister(target, action);
			if (!targets.Contains(target) && !freeTargets.Contains(target)) { targetsWereBroadcast.Remove(target); }
		}

		public override void Unregister(System.Action action) {
			base.Unregister(action);
			targetsWereBroadcast.RemoveWhere(t => !targets.Contains(t) && !freeTargets.Contains(t));
		}

		public override void Unregister(System.Action<TTarget> action) {
			base.Unregister(action);
			targetsWereBroadcast.RemoveWhere(t => !targets.Contains(t) && !freeTargets.Contains(t));
		}

		public override void Unregister(TTarget target) {
			base.Unregister(target);
			targetsWereBroadcast.Remove(target);
		}
		
		public override void UnregisterAll() {
			base.UnregisterAll();
			targetsWereBroadcast.Clear();
		}

		public override void Broadcast(TTarget target) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to broadcast null"); return; }
			if (freeTargetedActions.TryGetValue(target, out var freeActions)) { freeActions(); }
			if (targetedActions.TryGetValue(target, out var actions)) { actions(target); }
			if (generalActions != null) { generalActions(target); }
			targetsWereBroadcast.Add(target);
		}

		public void Reset(TTarget target) {
			targetsWereBroadcast.Remove(target);
		}

		public void ResetAll() {
			targetsWereBroadcast.Clear();
		}
	}

	public class TargetedStateSignal<TTarget, T> : TargetedSignal<TTarget, T> {
		protected readonly Dictionary<TTarget, T> targetsWereBroadcast = new Dictionary<TTarget, T>();
		protected readonly static List<TTarget> targetsToRemove = new List<TTarget>();

		public override void Register(TTarget target, System.Action<TTarget, T> action) {
			base.Register(target, action);
			if (targetsWereBroadcast.TryGetValue(target, out T value)) { action(target, value); }
		}

		public override void Register(TTarget target, System.Action<T> action) {
			base.Register(target, action);
			if (targetsWereBroadcast.TryGetValue(target, out T value)) { action(value); }
		}

		public override void Register(System.Action<TTarget, T> action) {
			base.Register(action);
			foreach (var t in targetsWereBroadcast) { action(t.Key, t.Value); }
		}

		public override void Unregister(TTarget target, System.Action<TTarget, T> action) {
			base.Unregister(target, action);
			if (!targets.Contains(target) && !freeTargets.Contains(target)) { targetsWereBroadcast.Remove(target); }
		}

		public override void Unregister(TTarget target, System.Action<T> action) {
			base.Unregister(target, action);
			if (!targets.Contains(target) && !freeTargets.Contains(target)) { targetsWereBroadcast.Remove(target); }
		}

		public override void Unregister(System.Action<T> action) {
			base.Unregister(action);
			targetsToRemove.Clear();
			foreach (var t in targetsWereBroadcast.Keys) { if (!targets.Contains(t) && !freeTargets.Contains(t)) { targetsToRemove.Add(t); } }
			foreach (var t in targetsToRemove) { targetsWereBroadcast.Remove(t); }
		}

		public override void Unregister(System.Action<TTarget, T> action) {
			base.Unregister(action);
			targetsToRemove.Clear();
			foreach (var t in targetsWereBroadcast.Keys) { if (!targets.Contains(t) && !freeTargets.Contains(t)) { targetsToRemove.Add(t); } }
			foreach (var t in targetsToRemove) { targetsWereBroadcast.Remove(t); }
		}

		public override void Unregister(TTarget target) {
			base.Unregister(target);
			targetsWereBroadcast.Remove(target);
		}
		
		public override void UnregisterAll() {
			base.UnregisterAll();
			targetsWereBroadcast.Clear();
		}

		public override void Broadcast(TTarget target, T value) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to broadcast null"); return; }
			if (freeTargetedActions.TryGetValue(target, out var freeActions)) { freeActions(value); }
			if (targetedActions.TryGetValue(target, out var actions)) { actions(target, value); }
			if (generalActions != null) { generalActions(target, value); }
			targetsWereBroadcast[target] = value;
		}

		public void Reset(TTarget target) {
			targetsWereBroadcast.Remove(target);
		}

		public void ResetAll() {
			targetsWereBroadcast.Clear();
		}
	}

	public class TargetedStateSignal<TTarget, T1, T2> : TargetedSignal<TTarget, T1, T2> {
		protected readonly Dictionary<TTarget, (T1, T2)> targetsWereBroadcast = new Dictionary<TTarget, (T1, T2)>();
		protected readonly static List<TTarget> targetsToRemove = new List<TTarget>();

		public override void Register(TTarget target, System.Action<TTarget, T1, T2> action) {
			base.Register(target, action);
			if (targetsWereBroadcast.TryGetValue(target, out (T1, T2) values)) { action(target, values.Item1, values.Item2); }
		}

		public override void Register(TTarget target, System.Action<T1, T2> action) {
			base.Register(target, action);
			if (targetsWereBroadcast.TryGetValue(target, out (T1, T2) values)) { action(values.Item1, values.Item2); }
		}

		public override void Register(System.Action<TTarget, T1, T2> action) {
			base.Register(action);
			foreach (var t in targetsWereBroadcast) { action(t.Key, t.Value.Item1, t.Value.Item2); }
		}

		public override void Unregister(TTarget target, System.Action<TTarget, T1, T2> action) {
			base.Unregister(target, action);
			if (!targets.Contains(target) && !freeTargets.Contains(target)) { targetsWereBroadcast.Remove(target); }
		}

		public override void Unregister(TTarget target, System.Action<T1, T2> action) {
			base.Unregister(target, action);
			if (!targets.Contains(target) && !freeTargets.Contains(target)) { targetsWereBroadcast.Remove(target); }
		}

		public override void Unregister(System.Action<T1, T2> action) {
			base.Unregister(action);
			targetsToRemove.Clear();
			foreach (var t in targetsWereBroadcast.Keys) { if (!targets.Contains(t) && !freeTargets.Contains(t)) { targetsToRemove.Add(t); } }
			foreach (var t in targetsToRemove) { targetsWereBroadcast.Remove(t); }
		}

		public override void Unregister(System.Action<TTarget, T1, T2> action) {
			base.Unregister(action);
			targetsToRemove.Clear();
			foreach (var t in targetsWereBroadcast.Keys) { if (!targets.Contains(t) && !freeTargets.Contains(t)) { targetsToRemove.Add(t); } }
			foreach (var t in targetsToRemove) { targetsWereBroadcast.Remove(t); }
		}

		public override void Unregister(TTarget target) {
			base.Unregister(target);
			targetsWereBroadcast.Remove(target);
		}
		
		public override void UnregisterAll() {
			base.UnregisterAll();
			targetsWereBroadcast.Clear();
		}

		public override void Broadcast(TTarget target, T1 value1, T2 value2) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to broadcast null"); return; }
			if (freeTargetedActions.TryGetValue(target, out var freeActions)) { freeActions(value1, value2); }
			if (targetedActions.TryGetValue(target, out var actions)) { actions(target, value1, value2); }
			if (generalActions != null) { generalActions(target, value1, value2); }
			targetsWereBroadcast[target] = (value1, value2);
		}

		public void Reset(TTarget target) {
			targetsWereBroadcast.Remove(target);
		}

		public void ResetAll() {
			targetsWereBroadcast.Clear();
		}
	}

	public class TargetedStateSignal<TTarget, T1, T2, T3> : TargetedSignal<TTarget, T1, T2, T3> {
		protected readonly Dictionary<TTarget, (T1, T2, T3)> targetsWereBroadcast = new Dictionary<TTarget, (T1, T2, T3)>();
		protected readonly static List<TTarget> targetsToRemove = new List<TTarget>();

		public override void Register(TTarget target, System.Action<TTarget, T1, T2, T3> action) {
			base.Register(target, action);
			if (targetsWereBroadcast.TryGetValue(target, out (T1, T2, T3) values)) { action(target, values.Item1, values.Item2, values.Item3); }
		}

		public override void Register(TTarget target, System.Action<T1, T2, T3> action) {
			base.Register(target, action);
			if (targetsWereBroadcast.TryGetValue(target, out (T1, T2, T3) values)) { action(values.Item1, values.Item2, values.Item3); }
		}

		public override void Register(System.Action<TTarget, T1, T2, T3> action) {
			base.Register(action);
			foreach (var t in targetsWereBroadcast) { action(t.Key, t.Value.Item1, t.Value.Item2, t.Value.Item3); }
		}

		public override void Unregister(TTarget target, System.Action<TTarget, T1, T2, T3> action) {
			base.Unregister(target, action);
			if (!targets.Contains(target) && !freeTargets.Contains(target)) { targetsWereBroadcast.Remove(target); }
		}

		public override void Unregister(TTarget target, System.Action<T1, T2, T3> action) {
			base.Unregister(target, action);
			if (!targets.Contains(target) && !freeTargets.Contains(target)) { targetsWereBroadcast.Remove(target); }
		}

		public override void Unregister(System.Action<T1, T2, T3> action) {
			base.Unregister(action);
			targetsToRemove.Clear();
			foreach (var t in targetsWereBroadcast.Keys) { if (!targets.Contains(t) && !freeTargets.Contains(t)) { targetsToRemove.Add(t); } }
			foreach (var t in targetsToRemove) { targetsWereBroadcast.Remove(t); }
		}

		public override void Unregister(System.Action<TTarget, T1, T2, T3> action) {
			base.Unregister(action);
			targetsToRemove.Clear();
			foreach (var t in targetsWereBroadcast.Keys) { if (!targets.Contains(t) && !freeTargets.Contains(t)) { targetsToRemove.Add(t); } }
			foreach (var t in targetsToRemove) { targetsWereBroadcast.Remove(t); }
		}

		public override void Unregister(TTarget target) {
			base.Unregister(target);
			targetsWereBroadcast.Remove(target);
		}
		
		public override void UnregisterAll() {
			base.UnregisterAll();
			targetsWereBroadcast.Clear();
		}

		public override void Broadcast(TTarget target, T1 value1, T2 value2, T3 value3) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to broadcast null"); return; }
			if (freeTargetedActions.TryGetValue(target, out var freeActions)) { freeActions(value1, value2, value3); }
			if (targetedActions.TryGetValue(target, out var actions)) { actions(target, value1, value2, value3); }
			if (generalActions != null) { generalActions(target, value1, value2, value3); }
			targetsWereBroadcast[target] = (value1, value2, value3);
		}

		public void Reset(TTarget target) {
			targetsWereBroadcast.Remove(target);
		}

		public void ResetAll() {
			targetsWereBroadcast.Clear();
		}
	}

	public class TargetedStateSignal<TTarget, T1, T2, T3, T4> : TargetedSignal<TTarget, T1, T2, T3, T4> {
		protected readonly Dictionary<TTarget, (T1, T2, T3, T4)> targetsWereBroadcast = new Dictionary<TTarget, (T1, T2, T3, T4)>();
		protected readonly static List<TTarget> targetsToRemove = new List<TTarget>();

		public override void Register(TTarget target, System.Action<TTarget, T1, T2, T3, T4> action) {
			base.Register(target, action);
			if (targetsWereBroadcast.TryGetValue(target, out (T1, T2, T3, T4) values)) { action(target, values.Item1, values.Item2, values.Item3, values.Item4); }
		}

		public override void Register(TTarget target, System.Action<T1, T2, T3, T4> action) {
			base.Register(target, action);
			if (targetsWereBroadcast.TryGetValue(target, out (T1, T2, T3, T4) values)) { action(values.Item1, values.Item2, values.Item3, values.Item4); }
		}

		public override void Register(System.Action<TTarget, T1, T2, T3, T4> action) {
			base.Register(action);
			foreach (var t in targetsWereBroadcast) { action(t.Key, t.Value.Item1, t.Value.Item2, t.Value.Item3, t.Value.Item4); }
		}

		public override void Unregister(TTarget target, System.Action<TTarget, T1, T2, T3, T4> action) {
			base.Unregister(target, action);
			if (!targets.Contains(target) && !freeTargets.Contains(target)) { targetsWereBroadcast.Remove(target); }
		}

		public override void Unregister(TTarget target, System.Action<T1, T2, T3, T4> action) {
			base.Unregister(target, action);
			if (!targets.Contains(target) && !freeTargets.Contains(target)) { targetsWereBroadcast.Remove(target); }
		}

		public override void Unregister(System.Action<T1, T2, T3, T4> action) {
			base.Unregister(action);
			targetsToRemove.Clear();
			foreach (var t in targetsWereBroadcast.Keys) { if (!targets.Contains(t) && !freeTargets.Contains(t)) { targetsToRemove.Add(t); } }
			foreach (var t in targetsToRemove) { targetsWereBroadcast.Remove(t); }
		}

		public override void Unregister(System.Action<TTarget, T1, T2, T3, T4> action) {
			base.Unregister(action);
			targetsToRemove.Clear();
			foreach (var t in targetsWereBroadcast.Keys) { if (!targets.Contains(t) && !freeTargets.Contains(t)) { targetsToRemove.Add(t); } }
			foreach (var t in targetsToRemove) { targetsWereBroadcast.Remove(t); }
		}

		public override void Unregister(TTarget target) {
			base.Unregister(target);
			targetsWereBroadcast.Remove(target);
		}
		
		public override void UnregisterAll() {
			base.UnregisterAll();
			targetsWereBroadcast.Clear();
		}

		public override void Broadcast(TTarget target, T1 value1, T2 value2, T3 value3, T4 value4) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to broadcast null"); return; }
			if (freeTargetedActions.TryGetValue(target, out var freeActions)) { freeActions(value1, value2, value3, value4); }
			if (targetedActions.TryGetValue(target, out var actions)) { actions(target, value1, value2, value3, value4); }
			if (generalActions != null) { generalActions(target, value1, value2, value3, value4); }
			targetsWereBroadcast[target] = (value1, value2, value3, value4);
		}

		public void Reset(TTarget target) {
			targetsWereBroadcast.Remove(target);
		}

		public void ResetAll() {
			targetsWereBroadcast.Clear();
		}
	}

	//

	// // Reply Signals are like Signals, but return a value (only the last one registered, of course)
	// // Example usage:
	// public static class Signals {
	//     public readonly static ReplySignal<int> TimesKilled = new ReplySignal<int>();
	// }
	// Signals.TimesKilled.Register(CheckTimesKilled); // -> void CheckTimesKilled() { return 16; }
	// Signals.TimesKilled.Broadcast(value => { Debug.Log("Killed: " + value); });
	// for (int i = Signals.TimesKilled.Broadcast(out var results) - 1; i >= 0; --i) { Debug.Log(i + ") Killed: " + results[i]); }

	// zero parameters
	public class ReplySignal<TResult> : ISignal {
		protected readonly List<System.Func<TResult>> functions = new List<System.Func<TResult>>();

		//

		public virtual void Register(System.Func<TResult> function) {
			if (function == null) { UnityEngine.Debug.LogError("trying to register null"); return; }
			functions.Add(function);
		}

		public virtual bool Unregister(System.Func<TResult> function) {
			if (function == null) { UnityEngine.Debug.LogError("trying to unregister null"); return false; }
			return functions.Remove(function);
		}

		public virtual void UnregisterAll() {
			functions.Clear();
		}

		// returns default if there are no listeners!
		// result is from the last listener
		public virtual TResult Broadcast() {
			if (functions.Count == 0) { return default; }
			TResult lastResult = default;
			foreach (var func in functions) { lastResult = func(); }
			return lastResult;
		}

		public virtual int Broadcast(out TResult[] results) {
			var count = functions.Count;
			if (count == 0) { results = null; return 0; }
			results = new TResult[count];
			for (int i = 0; i < count; ++i) { results[i] = functions[i](); }
			return count;
		}

		public virtual int Broadcast(ref List<TResult> results) {
			var count = functions.Count;
			if (count > 0) {
				if (results == null) { results = new List<TResult>(count); }
				else { results.Clear(); }
				foreach (var func in functions) { results.Add(func()); }
			}
			return count;
		}

		public virtual bool Broadcast(System.Action<TResult> callback) {
			if (callback == null || functions.Count == 0) { return false; }
			foreach (var func in functions) { callback(func()); }
			return true;
		}

		// result is from the last listener
		public virtual bool TryBroadcast(out TResult result) {
			result = default;
			if (functions.Count == 0) { return false; }
			foreach (var func in functions) { result = func(); }
			return true;
		}
	}
	
	// one parameter
	public class ReplySignal<T, TResult> : ISignal {
		protected readonly List<System.Func<T, TResult>> functions = new List<System.Func<T, TResult>>();

		//

		public virtual void Register(System.Func<T, TResult> function) {
			if (function == null) { UnityEngine.Debug.LogError("trying to register null"); return; }
			functions.Add(function);
		}

		public virtual bool Unregister(System.Func<T, TResult> function) {
			if (function == null) {UnityEngine.Debug.LogError("trying to unregister null"); return false; }
			return functions.Remove(function);
		}

		public virtual void UnregisterAll() {
			functions.Clear();
		}

		// returns default if there are no listeners!
		// result is from the last listener
		public virtual TResult Broadcast(T value) {
			if (functions.Count == 0) { return default; }
			TResult lastResult = default;
			foreach (var func in functions) { lastResult = func(value); }
			return lastResult;
		}

		public virtual int Broadcast(T value, out TResult[] results) {
			var count = functions.Count;
			if (count == 0) { results = null; return 0; }
			results = new TResult[count];
			for (int i = 0; i < count; ++i) { results[i] = functions[i](value); }
			return count;
		}

		public virtual int Broadcast(T value, ref List<TResult> results) {
			var count = functions.Count;
			if (count > 0) {
				if (results == null) { results = new List<TResult>(count); }
				else { results.Clear(); }
				foreach (var func in functions) { results.Add(func(value)); }
			}
			return count;
		}

		public virtual bool Broadcast(T value, System.Action<TResult> callback) {
			if (callback == null || functions.Count == 0) { return false; }
			foreach (var func in functions) { callback(func(value)); }
			return true;
		}

		// result is from the last listener
		public virtual bool TryBroadcast(T value, out TResult result) {
			result = default;
			if (functions.Count == 0) { return false; }
			foreach (var func in functions) { result = func(value); }
			return true;
		}
	}

	// two parameters
	public class ReplySignal<T1, T2, TResult> : ISignal {
		protected readonly List<System.Func<T1, T2, TResult>> functions = new List<System.Func<T1, T2, TResult>>();

		//

		public virtual void Register(System.Func<T1, T2, TResult> function) {
			if (function == null) { UnityEngine.Debug.LogError("trying to register null"); return; }
			functions.Add(function);
		}

		public virtual bool Unregister(System.Func<T1, T2, TResult> function) {
			if (function == null) { UnityEngine.Debug.LogError("trying to unregister null"); return false; }
			return functions.Remove(function);
		}

		public virtual void UnregisterAll() {
			functions.Clear();
		}

		// returns default if there are no listeners!
		// result is from the last listener
		public virtual TResult Broadcast(T1 value1, T2 value2) {
			if (functions.Count == 0) { return default; }
			TResult lastResult = default;
			foreach (var func in functions) { lastResult = func(value1, value2); }
			return lastResult;
		}

		public virtual int Broadcast(T1 value1, T2 value2, out TResult[] results) {
			var count = functions.Count;
			if (count == 0) { results = null; return 0; }
			results = new TResult[count];
			for (int i = 0; i < count; ++i) { results[i] = functions[i](value1, value2); }
			return count;
		}

		public virtual int Broadcast(T1 value1, T2 value2, ref List<TResult> results) {
			var count = functions.Count;
			if (count > 0) {
				if (results == null) { results = new List<TResult>(count); }
				else { results.Clear(); }
				foreach (var func in functions) { results.Add(func(value1, value2)); }
			}
			return count;
		}

		public virtual bool Broadcast(T1 value1, T2 value2, System.Action<TResult> callback) {
			if (callback == null || functions.Count == 0) { return false; }
			foreach (var func in functions) { callback(func(value1, value2)); }
			return true;
		}

		// result is from the last listener
		public virtual bool TryBroadcast(T1 value1, T2 value2, out TResult result) {
			result = default;
			if (functions.Count == 0) { return false; }
			foreach (var func in functions) { result = func(value1, value2); }
			return true;
		}
	}
	
	// three parameters
	public class ReplySignal<T1, T2, T3, TResult> : ISignal {
		protected readonly List<System.Func<T1, T2, T3, TResult>> functions = new List<System.Func<T1, T2, T3, TResult>>();

		//

		public virtual void Register(System.Func<T1, T2, T3, TResult> function) {
			if (function == null) { UnityEngine.Debug.LogError("trying to register null"); return; }
			functions.Add(function);
		}

		public virtual bool Unregister(System.Func<T1, T2, T3, TResult> function) {
			if (function == null) {UnityEngine.Debug.LogError("trying to unregister null"); return false; }
			return functions.Remove(function);
		}

		public virtual void UnregisterAll() {
			functions.Clear();
		}

		// returns default if there are no listeners!
		// result is from the last listener
		public virtual TResult Broadcast(T1 value1, T2 value2, T3 value3) {
			if (functions.Count == 0) { return default; }
			TResult lastResult = default;
			foreach (var func in functions) { lastResult = func(value1, value2, value3); }
			return lastResult;
		}

		public virtual int Broadcast(T1 value1, T2 value2, T3 value3, out TResult[] results) {
			var count = functions.Count;
			if (count == 0) { results = null; return 0; }
			results = new TResult[count];
			for (int i = 0; i < count; ++i) { results[i] = functions[i](value1, value2, value3); }
			return count;
		}

		public virtual int Broadcast(T1 value1, T2 value2, T3 value3, ref List<TResult> results) {
			var count = functions.Count;
			if (count > 0) {
				if (results == null) { results = new List<TResult>(count); }
				else { results.Clear(); }
				foreach (var func in functions) { results.Add(func(value1, value2, value3)); }
			}
			return count;
		}

		public virtual bool Broadcast(T1 value1, T2 value2, T3 value3, System.Action<TResult> callback) {
			if (callback == null || functions.Count == 0) { return false; }
			foreach (var func in functions) { callback(func(value1, value2, value3)); }
			return true;
		}

		// result is from the last listener
		public virtual bool TryBroadcast(T1 value1, T2 value2, T3 value3, out TResult result) {
			result = default;
			if (functions.Count == 0) { return false; }
			foreach (var func in functions) { result = func(value1, value2, value3); }
			return true;
		}
	}
	
	// four parameters
	public class ReplySignal<T1, T2, T3, T4, TResult> : ISignal {
		protected readonly List<System.Func<T1, T2, T3, T4, TResult>> functions = new List<System.Func<T1, T2, T3, T4, TResult>>();

		//

		public virtual void Register(System.Func<T1, T2, T3, T4, TResult> function) {
			if (function == null) { UnityEngine.Debug.LogError("trying to register null"); return; }
			functions.Add(function);
		}

		public virtual bool Unregister(System.Func<T1, T2, T3, T4, TResult> function) {
			if (function == null) { UnityEngine.Debug.LogError("trying to unregister null"); return false; }
			return functions.Remove(function);
		}

		public virtual void UnregisterAll() {
			functions.Clear();
		}

		// returns default if there are no listeners!
		// result is from the last listener
		public virtual TResult Broadcast(T1 value1, T2 value2, T3 value3, T4 value4) {
			if (functions.Count == 0) { return default; }
			TResult lastResult = default;
			foreach (var func in functions) { lastResult = func(value1, value2, value3, value4); }
			return lastResult;
		}

		public virtual int Broadcast(T1 value1, T2 value2, T3 value3, T4 value4, out TResult[] results) {
			var count = functions.Count;
			if (count == 0) { results = null; return 0; }
			results = new TResult[count];
			for (int i = 0; i < count; ++i) { results[i] = functions[i](value1, value2, value3, value4); }
			return count;
		}

		public virtual int Broadcast(T1 value1, T2 value2, T3 value3, T4 value4, ref List<TResult> results) {
			var count = functions.Count;
			if (count > 0) {
				if (results == null) { results = new List<TResult>(count); }
				else { results.Clear(); }
				foreach (var func in functions) { results.Add(func(value1, value2, value3, value4)); }
			}
			return count;
		}

		public virtual bool Broadcast(T1 value1, T2 value2, T3 value3, T4 value4, System.Action<TResult> callback) {
			if (callback == null || functions.Count == 0) { return false; }
			foreach (var func in functions) { callback(func(value1, value2, value3, value4)); }
			return true;
		}

		// result is from the last listener
		public virtual bool TryBroadcast(T1 value1, T2 value2, T3 value3, T4 value4, out TResult result) {
			result = default;
			if (functions.Count == 0) { return false; }
			foreach (var func in functions) { result = func(value1, value2, value3, value4); }
			return true;
		}
	}

	//

	// no parameters
	public class TargetedReplySignal<TTarget, TResult> : ISignal {
		protected readonly HashSet<TTarget> targets = new HashSet<TTarget>();
		protected readonly Dictionary<TTarget, List<System.Func<TTarget, TResult>>> targetedFunctions = new Dictionary<TTarget, List<System.Func<TTarget, TResult>>>();
		protected readonly HashSet<TTarget> freeTargets = new HashSet<TTarget>();
		protected readonly Dictionary<TTarget, List<System.Func<TResult>>> freeTargetedFunctions = new Dictionary<TTarget, List<System.Func<TResult>>>();
		protected readonly List<System.Func<TTarget, TResult>> generalFunctions = new List<System.Func<TTarget, TResult>>();

		//

		public virtual void Register(TTarget target, System.Func<TTarget, TResult> function) {
			if (target == null || function == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to register null"); return; }
			if (!targetedFunctions.TryGetValue(target, out var functions)) { targets.Add(target); targetedFunctions.Add(target, new List<System.Func<TTarget, TResult>>() { function }); }
			else { targetedFunctions[target].Add(function); }
		}

		public virtual void Register(TTarget target, System.Func<TResult> function) {
			if (target == null || function == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to register null"); return; }
			if (!freeTargetedFunctions.TryGetValue(target, out var functions)) { freeTargets.Add(target); freeTargetedFunctions.Add(target, new List<System.Func<TResult>>() { function }); }
			else { freeTargetedFunctions[target].Add(function); }
		}

		public virtual void Register(System.Func<TTarget, TResult> function) {
			if (function == null) { UnityEngine.Debug.LogError("trying to register null"); return; }
			generalFunctions.Add(function);
		}

		public virtual void Unregister(TTarget target, System.Func<TTarget, TResult> function) {
			if (target == null || function == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			if (!targetedFunctions.TryGetValue(target, out var functions)) { return; }
			if (functions.Remove(function) && functions.Count == 0) { targets.Remove(target); targetedFunctions.Remove(target); }
		}

		public virtual void Unregister(TTarget target, System.Func<TResult> function) {
			if (target == null || function == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			if (!freeTargetedFunctions.TryGetValue(target, out var functions)) { return; }
			if (functions.Remove(function) && functions.Count == 0) { freeTargets.Remove(target); freeTargetedFunctions.Remove(target); }
		}

		public virtual void Unregister(System.Func<TResult> function) {
			if (function == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			foreach (var target in freeTargets) {
				var functions = freeTargetedFunctions[target];
				if (functions.Remove(function) && functions.Count == 0) { freeTargetedFunctions.Remove(target); }
			}
			freeTargets.RemoveWhere(t => !freeTargetedFunctions.ContainsKey(t));
		}

		public virtual void Unregister(System.Func<TTarget, TResult> function) {
			if (function == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			foreach (var target in targets) {
				var functions = targetedFunctions[target];
				if (functions.Remove(function) && functions.Count == 0) { targetedFunctions.Remove(target); }
			}
			targets.RemoveWhere(t => !targetedFunctions.ContainsKey(t));
			generalFunctions.Remove(function);
		}

		public virtual void Unregister(TTarget target) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			targets.Remove(target);
			targetedFunctions.Remove(target);
			freeTargets.Remove(target);
			freeTargetedFunctions.Remove(target);
		}

		public virtual void UnregisterAll() {
			targets.Clear();
			targetedFunctions.Clear();
			freeTargets.Clear();
			freeTargetedFunctions.Clear();
			generalFunctions.Clear();
		}

		// returns default if there are no listeners!
		// result is from the last listener
		public virtual TResult Broadcast(TTarget target) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to broadcast null"); return default; }
			TResult lastResult = default;
			if (freeTargetedFunctions.TryGetValue(target, out var freeFunctions)) { foreach (var func in freeFunctions) { lastResult = func(); } }
			if (targetedFunctions.TryGetValue(target, out var functions)) { foreach (var func in functions) { lastResult = func(target); } }
			foreach (var func in generalFunctions) { lastResult = func(target); }
			return lastResult;
		}

		public virtual int Broadcast(TTarget target, out TResult[] results) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { results = null; UnityEngine.Debug.LogError("trying to broadcast null"); return 0; }
			var freeFunctionsCount = freeTargetedFunctions.TryGetValue(target, out var freeFunctions) ? freeFunctions.Count : 0;
			var targetedFunctionsCount = targetedFunctions.TryGetValue(target, out var functions) ? functions.Count : 0;
			var generalFunctionsCount = generalFunctions.Count;
			var count = freeFunctionsCount + targetedFunctionsCount + generalFunctionsCount;
			if (count == 0) { results = null; return 0; }
			results = new TResult[count];
			int r = 0;
			for (int i = 0; i < freeFunctionsCount; ++i, ++r) { results[r] = freeFunctions[i](); }
			for (int i = 0; i < targetedFunctionsCount; ++i, ++r) { results[r] = functions[i](target); }
			for (int i = 0; i < generalFunctionsCount; ++i, ++r) { results[r] = generalFunctions[i](target); }
			return count;
		}

		public virtual int Broadcast(TTarget target, ref List<TResult> results) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to broadcast null"); return 0; }
			var freeFunctionsCount = freeTargetedFunctions.TryGetValue(target, out var freeFunctions) ? freeFunctions.Count : 0;
			var targetedFunctionsCount = targetedFunctions.TryGetValue(target, out var functions) ? functions.Count : 0;
			var generalFunctionsCount = generalFunctions.Count;
			var count = freeFunctionsCount + targetedFunctionsCount + generalFunctionsCount;
			if (count == 0) { results = null; return 0; }
			if (results == null) { results = new List<TResult>(count); }
			else { results.Clear(); }
			int r = 0;
			for (int i = 0; i < freeFunctionsCount; ++i, ++r) { results.Add(freeFunctions[i]()); }
			for (int i = 0; i < targetedFunctionsCount; ++i, ++r) { results.Add(functions[i](target)); }
			for (int i = 0; i < generalFunctionsCount; ++i, ++r) { results.Add(generalFunctions[i](target)); }
			return count;
		}

		public virtual bool Broadcast(TTarget target, System.Action<TResult> callback) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to broadcast null"); return false; }
			if (callback == null) { return false; }
			var freeFunctionsCount = freeTargetedFunctions.TryGetValue(target, out var freeFunctions) ? freeFunctions.Count : 0;
			var targetedFunctionsCount = targetedFunctions.TryGetValue(target, out var functions) ? functions.Count : 0;
			if (freeFunctionsCount + targetedFunctionsCount + generalFunctions.Count == 0) { return false; }
			if (freeFunctionsCount != 0) { foreach(var func in freeFunctions) { callback(func()); } }
			if (targetedFunctionsCount != 0) { foreach(var func in functions) { callback(func(target)); } }
			foreach (var func in generalFunctions) { callback(func(target)); }
			return true;
		}

		public virtual void Broadcast(System.Action<TTarget, TResult> callback) {
			if (callback == null) { return; }
			foreach (var kvp in freeTargetedFunctions) { foreach (var func in kvp.Value) { callback(kvp.Key, func()); } }
			foreach (var kvp in targetedFunctions) { foreach (var func in kvp.Value) { callback(kvp.Key, func(kvp.Key)); } }
		}

		// result is from the last listener
		public virtual bool TryBroadcast(TTarget target, out TResult result) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { result = default; UnityEngine.Debug.LogError("trying to broadcast null"); return false; }
			var freeFunctionsCount = freeTargetedFunctions.TryGetValue(target, out var freeFunctions) ? freeFunctions.Count : 0;
			var targetedFunctionsCount = targetedFunctions.TryGetValue(target, out var functions) ? functions.Count : 0;
			var count = freeFunctionsCount + targetedFunctionsCount + generalFunctions.Count;
			result = default;
			if (count == 0) { return false; }
			if (freeFunctionsCount != 0) { foreach (var func in freeFunctions) { result = func(); } }
			if (targetedFunctionsCount != 0) { foreach (var func in functions) { result = func(target); } }
			foreach (var func in generalFunctions) { result = func(target); }
			return true;
		}
	}

	// one parameter
	public class TargetedReplySignal<TTarget, T, TResult> : ISignal {
		protected readonly HashSet<TTarget> targets = new HashSet<TTarget>();
		protected readonly Dictionary<TTarget, List<System.Func<TTarget, T, TResult>>> targetedFunctions = new Dictionary<TTarget, List<System.Func<TTarget, T, TResult>>>();
		protected readonly HashSet<TTarget> freeTargets = new HashSet<TTarget>();
		protected readonly Dictionary<TTarget, List<System.Func<T, TResult>>> freeTargetedFunctions = new Dictionary<TTarget, List<System.Func<T, TResult>>>();
		protected readonly List<System.Func<TTarget, T, TResult>> generalFunctions = new List<System.Func<TTarget, T, TResult>>();

		//

		public virtual void Register(TTarget target, System.Func<TTarget, T, TResult> function) {
			if (target == null || function == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to register null"); return; }
			if (!targetedFunctions.TryGetValue(target, out var functions)) { targets.Add(target); targetedFunctions.Add(target, new List<System.Func<TTarget, T, TResult>>() { function }); }
			else { targetedFunctions[target].Add(function); }
		}

		public virtual void Register(TTarget target, System.Func<T, TResult> function) {
			if (target == null || function == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to register null"); return; }
			if (!freeTargetedFunctions.TryGetValue(target, out var functions)) { freeTargets.Add(target); freeTargetedFunctions.Add(target, new List<System.Func<T, TResult>>() { function }); }
			else { freeTargetedFunctions[target].Add(function); }
		}

		public virtual void Register(System.Func<TTarget, T, TResult> function) {
			if (function == null) { UnityEngine.Debug.LogError("trying to register null"); return; }
			generalFunctions.Add(function);
		}

		public virtual void Unregister(TTarget target, System.Func<TTarget, T, TResult> function) {
			if (target == null || function == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			if (!targetedFunctions.TryGetValue(target, out var functions)) { return; }
			if (functions.Remove(function) && functions.Count == 0) { targets.Remove(target); targetedFunctions.Remove(target); }
		}

		public virtual void Unregister(TTarget target, System.Func<T, TResult> function) {
			if (target == null || function == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			if (!freeTargetedFunctions.TryGetValue(target, out var functions)) { return; }
			if (functions.Remove(function) && functions.Count == 0) { freeTargets.Remove(target); freeTargetedFunctions.Remove(target); }
		}

		public virtual void Unregister(System.Func<T, TResult> function) {
			if (function == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			foreach (var target in freeTargets) {
				var functions = freeTargetedFunctions[target];
				if (functions.Remove(function) && functions.Count == 0) { freeTargetedFunctions.Remove(target); }
			}
			freeTargets.RemoveWhere(t => !freeTargetedFunctions.ContainsKey(t));
		}

		public virtual void Unregister(System.Func<TTarget, T, TResult> function) {
			if (function == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			foreach (var target in targets) {
				var functions = targetedFunctions[target];
				if (functions.Remove(function) && functions.Count == 0) { targetedFunctions.Remove(target); }
			}
			targets.RemoveWhere(t => !targetedFunctions.ContainsKey(t));
			generalFunctions.Remove(function);
		}

		public virtual void Unregister(TTarget target) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			targets.Remove(target);
			targetedFunctions.Remove(target);
			freeTargets.Remove(target);
			freeTargetedFunctions.Remove(target);
		}
		
		public virtual void UnregisterAll() {
			targets.Clear();
			targetedFunctions.Clear();
			freeTargets.Clear();
			freeTargetedFunctions.Clear();
			generalFunctions.Clear();
		}

		// returns default if there are no listeners!
		// result is from the last listener
		public virtual TResult Broadcast(TTarget target, T value) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to broadcast null"); return default; }
			TResult lastResult = default;
			if (freeTargetedFunctions.TryGetValue(target, out var freeFunctions)) { foreach (var func in freeFunctions) { lastResult = func(value); } }
			if (targetedFunctions.TryGetValue(target, out var functions)) { foreach (var func in functions) { lastResult = func(target, value); } }
			foreach (var func in generalFunctions) { lastResult = func(target, value); }
			return lastResult;
		}

		public virtual int Broadcast(TTarget target, T value, out TResult[] results) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { results = null; UnityEngine.Debug.LogError("trying to broadcast null"); return 0; }
			var freeFunctionsCount = freeTargetedFunctions.TryGetValue(target, out var freeFunctions) ? freeFunctions.Count : 0;
			var targetedFunctionsCount = targetedFunctions.TryGetValue(target, out var functions) ? functions.Count : 0;
			var generalFunctionsCount = generalFunctions.Count;
			var count = freeFunctionsCount + targetedFunctionsCount + generalFunctionsCount;
			if (count == 0) { results = null; return 0; }
			results = new TResult[count];
			int r = 0;
			for (int i = 0; i < freeFunctionsCount; ++i, ++r) { results[r] = freeFunctions[i](value); }
			for (int i = 0; i < targetedFunctionsCount; ++i, ++r) { results[r] = functions[i](target, value); }
			for (int i = 0; i < generalFunctionsCount; ++i, ++r) { results[r] = generalFunctions[i](target, value); }
			return count;
		}

		public virtual int Broadcast(TTarget target, T value, ref List<TResult> results) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to broadcast null"); return 0; }
			var freeFunctionsCount = freeTargetedFunctions.TryGetValue(target, out var freeFunctions) ? freeFunctions.Count : 0;
			var targetedFunctionsCount = targetedFunctions.TryGetValue(target, out var functions) ? functions.Count : 0;
			var generalFunctionsCount = generalFunctions.Count;
			var count = freeFunctionsCount + targetedFunctionsCount + generalFunctionsCount;
			if (count == 0) { results = null; return 0; }
			if (results == null) { results = new List<TResult>(count); }
			else { results.Clear(); }
			int r = 0;
			for (int i = 0; i < freeFunctionsCount; ++i, ++r) { results.Add(freeFunctions[i](value)); }
			for (int i = 0; i < targetedFunctionsCount; ++i, ++r) { results.Add(functions[i](target, value)); }
			for (int i = 0; i < generalFunctionsCount; ++i, ++r) { results.Add(generalFunctions[i](target, value)); }
			return count;
		}

		public virtual bool Broadcast(TTarget target, T value, System.Action<TResult> callback) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to broadcast null"); return false; }
			if (callback == null) { return false; }
			var freeFunctionsCount = freeTargetedFunctions.TryGetValue(target, out var freeFunctions) ? freeFunctions.Count : 0;
			var targetedFunctionsCount = targetedFunctions.TryGetValue(target, out var functions) ? functions.Count : 0;
			if (freeFunctionsCount + targetedFunctionsCount + generalFunctions.Count == 0) { return false; }
			if (freeFunctionsCount != 0) { foreach (var func in freeFunctions) { callback(func(value)); } }
			if (targetedFunctionsCount != 0) { foreach (var func in functions) { callback(func(target, value)); } }
			foreach (var func in generalFunctions) { callback(func(target, value)); }
			return true;
		}

		public virtual void Broadcast(T value, System.Action<TTarget, TResult> callback) {
			if (callback == null) { return; }
			foreach (var kvp in freeTargetedFunctions) { foreach (var func in kvp.Value) { callback(kvp.Key, func(value)); } }
			foreach (var kvp in targetedFunctions) { foreach (var func in kvp.Value) { callback(kvp.Key, func(kvp.Key, value)); } }
		}

		// result is from the last listener
		public virtual bool TryBroadcast(TTarget target, T value, out TResult result) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { result = default; UnityEngine.Debug.LogError("trying to broadcast null"); return false; }
			var freeFunctionsCount = freeTargetedFunctions.TryGetValue(target, out var freeFunctions) ? freeFunctions.Count : 0;
			var targetedFunctionsCount = targetedFunctions.TryGetValue(target, out var functions) ? functions.Count : 0;
			var count = freeFunctionsCount + targetedFunctionsCount + generalFunctions.Count;
			result = default;
			if (count == 0) { return false; }
			if (freeFunctionsCount != 0) { foreach (var func in freeFunctions) { result = func(value); } }
			if (targetedFunctionsCount != 0) { foreach (var func in functions) { result = func(target, value); } }
			foreach (var func in generalFunctions) { result = func(target, value); }
			return true;
		}
	}

	// two parameters
	public class TargetedReplySignal<TTarget, T1, T2, TResult> : ISignal {
		protected readonly HashSet<TTarget> targets = new HashSet<TTarget>();
		protected readonly Dictionary<TTarget, List<System.Func<TTarget, T1, T2, TResult>>> targetedFunctions = new Dictionary<TTarget, List<System.Func<TTarget, T1, T2, TResult>>>();
		protected readonly HashSet<TTarget> freeTargets = new HashSet<TTarget>();
		protected readonly Dictionary<TTarget, List<System.Func<T1, T2, TResult>>> freeTargetedFunctions = new Dictionary<TTarget, List<System.Func<T1, T2, TResult>>>();
		protected readonly List<System.Func<TTarget, T1, T2, TResult>> generalFunctions = new List<System.Func<TTarget, T1, T2, TResult>>();

		//

		public virtual void Register(TTarget target, System.Func<TTarget, T1, T2, TResult> function) {
			if (target == null || function == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to register null"); return; }
			if (!targetedFunctions.TryGetValue(target, out var functions)) { targets.Add(target); targetedFunctions.Add(target, new List<System.Func<TTarget, T1, T2, TResult>>() { function }); }
			else { targetedFunctions[target].Add(function); }
		}

		public virtual void Register(TTarget target, System.Func<T1, T2, TResult> function) {
			if (target == null || function == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to register null"); return; }
			if (!freeTargetedFunctions.TryGetValue(target, out var functions)) { freeTargets.Add(target); freeTargetedFunctions.Add(target, new List<System.Func<T1, T2, TResult>>() { function }); }
			else { freeTargetedFunctions[target].Add(function); }
		}

		public virtual void Register(System.Func<TTarget, T1, T2, TResult> function) {
			if (function == null) { UnityEngine.Debug.LogError("trying to register null"); return; }
			generalFunctions.Add(function);
		}

		public virtual void Unregister(TTarget target, System.Func<TTarget, T1, T2, TResult> function) {
			if (target == null || function == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			if (!targetedFunctions.TryGetValue(target, out var functions)) { return; }
			if (functions.Remove(function) && functions.Count == 0) { targets.Remove(target); targetedFunctions.Remove(target); }
		}

		public virtual void Unregister(TTarget target, System.Func<T1, T2, TResult> function) {
			if (target == null || function == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			if (!freeTargetedFunctions.TryGetValue(target, out var functions)) { return; }
			if (functions.Remove(function) && functions.Count == 0) { freeTargets.Remove(target); freeTargetedFunctions.Remove(target); }
		}

		public virtual void Unregister(System.Func<T1, T2, TResult> function) {
			if (function == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			foreach (var target in freeTargets) {
				var functions = freeTargetedFunctions[target];
				if (functions.Remove(function) && functions.Count == 0) { freeTargetedFunctions.Remove(target); }
			}
			freeTargets.RemoveWhere(t => !freeTargetedFunctions.ContainsKey(t));
		}

		public virtual void Unregister(System.Func<TTarget, T1, T2, TResult> function) {
			if (function == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			foreach (var target in targets) {
				var functions = targetedFunctions[target];
				if (functions.Remove(function) && functions.Count == 0) { targetedFunctions.Remove(target); }
			}
			targets.RemoveWhere(t => !targetedFunctions.ContainsKey(t));
			generalFunctions.Remove(function);
		}

		public virtual void Unregister(TTarget target) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			targets.Remove(target);
			targetedFunctions.Remove(target);
			freeTargets.Remove(target);
			freeTargetedFunctions.Remove(target);
		}
		
		public virtual void UnregisterAll() {
			targets.Clear();
			targetedFunctions.Clear();
			freeTargets.Clear();
			freeTargetedFunctions.Clear();
			generalFunctions.Clear();
		}

		// returns default if there are no listeners!
		// result is from the last listener
		public virtual TResult Broadcast(TTarget target, T1 value1, T2 value2) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to broadcast null"); return default; }
			TResult lastResult = default;
			if (freeTargetedFunctions.TryGetValue(target, out var freeFunctions)) { foreach (var func in freeFunctions) { lastResult = func(value1, value2); } }
			if (targetedFunctions.TryGetValue(target, out var functions)) { foreach (var func in functions) { lastResult = func(target, value1, value2); } }
			foreach (var func in generalFunctions) { lastResult = func(target, value1, value2); }
			return lastResult;
		}

		public virtual int Broadcast(TTarget target, T1 value1, T2 value2, out TResult[] results) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { results = null; UnityEngine.Debug.LogError("trying to broadcast null"); return 0; }
			var freeFunctionsCount = freeTargetedFunctions.TryGetValue(target, out var freeFunctions) ? freeFunctions.Count : 0;
			var targetedFunctionsCount = targetedFunctions.TryGetValue(target, out var functions) ? functions.Count : 0;
			var generalFunctionsCount = generalFunctions.Count;
			var count = freeFunctionsCount + targetedFunctionsCount + generalFunctionsCount;
			if (count == 0) { results = null; return 0; }
			results = new TResult[count];
			int r = 0;
			for (int i = 0; i < freeFunctionsCount; ++i, ++r) { results[r] = freeFunctions[i](value1, value2); }
			for (int i = 0; i < targetedFunctionsCount; ++i, ++r) { results[r] = functions[i](target, value1, value2); }
			for (int i = 0; i < generalFunctionsCount; ++i, ++r) { results[r] = generalFunctions[i](target, value1, value2); }
			return count;
		}

		public virtual int Broadcast(TTarget target, T1 value1, T2 value2, ref List<TResult> results) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to broadcast null"); return 0; }
			var freeFunctionsCount = freeTargetedFunctions.TryGetValue(target, out var freeFunctions) ? freeFunctions.Count : 0;
			var targetedFunctionsCount = targetedFunctions.TryGetValue(target, out var functions) ? functions.Count : 0;
			var generalFunctionsCount = generalFunctions.Count;
			var count = freeFunctionsCount + targetedFunctionsCount + generalFunctionsCount;
			if (count == 0) { results = null; return 0; }
			if (results == null) { results = new List<TResult>(count); }
			else { results.Clear(); }
			int r = 0;
			for (int i = 0; i < freeFunctionsCount; ++i, ++r) { results.Add(freeFunctions[i](value1, value2)); }
			for (int i = 0; i < targetedFunctionsCount; ++i, ++r) { results.Add(functions[i](target, value1, value2)); }
			for (int i = 0; i < generalFunctionsCount; ++i, ++r) { results.Add(generalFunctions[i](target, value1, value2)); }
			return count;
		}

		public virtual bool Broadcast(TTarget target, T1 value1, T2 value2, System.Action<TResult> callback) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to broadcast null"); return false; }
			if (callback == null) { return false; }
			var freeFunctionsCount = freeTargetedFunctions.TryGetValue(target, out var freeFunctions) ? freeFunctions.Count : 0;
			var targetedFunctionsCount = targetedFunctions.TryGetValue(target, out var functions) ? functions.Count : 0;
			if (freeFunctionsCount + targetedFunctionsCount + generalFunctions.Count == 0) { return false; }
			if (freeFunctionsCount != 0) { foreach (var func in freeFunctions) { callback(func(value1, value2)); } }
			if (targetedFunctionsCount != 0) { foreach (var func in functions) { callback(func(target, value1, value2)); } }
			foreach (var func in generalFunctions) { callback(func(target, value1, value2)); }
			return true;
		}

		public virtual void Broadcast(T1 value1, T2 value2, System.Action<TTarget, TResult> callback) {
			if (callback == null) { return; }
			foreach (var kvp in freeTargetedFunctions) { foreach (var func in kvp.Value) { callback(kvp.Key, func(value1, value2)); } }
			foreach (var kvp in targetedFunctions) { foreach (var func in kvp.Value) { callback(kvp.Key, func(kvp.Key, value1, value2)); } }
		}

		// result is from the last listener
		public virtual bool TryBroadcast(TTarget target, T1 value1, T2 value2, out TResult result) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { result = default; UnityEngine.Debug.LogError("trying to broadcast null"); return false; }
			var freeFunctionsCount = freeTargetedFunctions.TryGetValue(target, out var freeFunctions) ? freeFunctions.Count : 0;
			var targetedFunctionsCount = targetedFunctions.TryGetValue(target, out var functions) ? functions.Count : 0;
			var count = freeFunctionsCount + targetedFunctionsCount + generalFunctions.Count;
			result = default;
			if (count == 0) { return false; }
			if (freeFunctionsCount != 0) { foreach (var func in freeFunctions) { result = func(value1, value2); } }
			if (targetedFunctionsCount != 0) { foreach (var func in functions) { result = func(target, value1, value2); } }
			foreach (var func in generalFunctions) { result = func(target, value1, value2); }
			return true;
		}
	}
	
	// three parameters
	public class TargetedReplySignal<TTarget, T1, T2, T3, TResult> : ISignal {
		protected readonly HashSet<TTarget> targets = new HashSet<TTarget>();
		protected readonly Dictionary<TTarget, List<System.Func<TTarget, T1, T2, T3, TResult>>> targetedFunctions = new Dictionary<TTarget, List<System.Func<TTarget, T1, T2, T3, TResult>>>();
		protected readonly HashSet<TTarget> freeTargets = new HashSet<TTarget>();
		protected readonly Dictionary<TTarget, List<System.Func<T1, T2, T3, TResult>>> freeTargetedFunctions = new Dictionary<TTarget, List<System.Func<T1, T2, T3, TResult>>>();
		protected readonly List<System.Func<TTarget, T1, T2, T3, TResult>> generalFunctions = new List<System.Func<TTarget, T1, T2, T3, TResult>>();

		//

		public virtual void Register(TTarget target, System.Func<TTarget, T1, T2, T3, TResult> function) {
			if (target == null || function == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to register null"); return; }
			if (!targetedFunctions.TryGetValue(target, out var functions)) { targets.Add(target); targetedFunctions.Add(target, new List<System.Func<TTarget, T1, T2, T3, TResult>>() { function }); }
			else { targetedFunctions[target].Add(function); }
		}

		public virtual void Register(TTarget target, System.Func<T1, T2, T3, TResult> function) {
			if (target == null || function == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to register null"); return; }
			if (!freeTargetedFunctions.TryGetValue(target, out var functions)) { freeTargets.Add(target); freeTargetedFunctions.Add(target, new List<System.Func<T1, T2, T3, TResult>>() { function }); }
			else { freeTargetedFunctions[target].Add(function); }
		}

		public virtual void Register(System.Func<TTarget, T1, T2, T3, TResult> function) {
			if (function == null) { UnityEngine.Debug.LogError("trying to register null"); return; }
			generalFunctions.Add(function);
		}

		public virtual void Unregister(TTarget target, System.Func<TTarget, T1, T2, T3, TResult> function) {
			if (target == null || function == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			if (!targetedFunctions.TryGetValue(target, out var functions)) { return; }
			if (functions.Remove(function) && functions.Count == 0) { targets.Remove(target); targetedFunctions.Remove(target); }
		}

		public virtual void Unregister(TTarget target, System.Func<T1, T2, T3, TResult> function) {
			if (target == null || function == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			if (!freeTargetedFunctions.TryGetValue(target, out var functions)) { return; }
			if (functions.Remove(function) && functions.Count == 0) { freeTargets.Remove(target); freeTargetedFunctions.Remove(target); }
		}

		public virtual void Unregister(System.Func<T1, T2, T3, TResult> function) {
			if (function == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			foreach (var target in freeTargets) {
				var functions = freeTargetedFunctions[target];
				if (functions.Remove(function) && functions.Count == 0) { freeTargetedFunctions.Remove(target); }
			}
			freeTargets.RemoveWhere(t => !freeTargetedFunctions.ContainsKey(t));
		}

		public virtual void Unregister(System.Func<TTarget, T1, T2, T3, TResult> function) {
			if (function == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			foreach (var target in targets) {
				var functions = targetedFunctions[target];
				if (functions.Remove(function) && functions.Count == 0) { targetedFunctions.Remove(target); }
			}
			targets.RemoveWhere(t => !targetedFunctions.ContainsKey(t));
			generalFunctions.Remove(function);
		}

		public virtual void Unregister(TTarget target) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			targets.Remove(target);
			targetedFunctions.Remove(target);
			freeTargets.Remove(target);
			freeTargetedFunctions.Remove(target);
		}
		
		public virtual void UnregisterAll() {
			targets.Clear();
			targetedFunctions.Clear();
			freeTargets.Clear();
			freeTargetedFunctions.Clear();
			generalFunctions.Clear();
		}

		// returns default if there are no listeners!
		// result is from the last listener
		public virtual TResult Broadcast(TTarget target, T1 value1, T2 value2, T3 value3) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to broadcast null"); return default; }
			TResult lastResult = default;
			if (freeTargetedFunctions.TryGetValue(target, out var freeFunctions)) { foreach (var func in freeFunctions) { lastResult = func(value1, value2, value3); } }
			if (targetedFunctions.TryGetValue(target, out var functions)) { foreach (var func in functions) { lastResult = func(target, value1, value2, value3); } }
			foreach (var func in generalFunctions) { lastResult = func(target, value1, value2, value3); }
			return lastResult;
		}

		public virtual int Broadcast(TTarget target, T1 value1, T2 value2, T3 value3, out TResult[] results) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { results = null; UnityEngine.Debug.LogError("trying to broadcast null"); return 0; }
			var freeFunctionsCount = freeTargetedFunctions.TryGetValue(target, out var freeFunctions) ? freeFunctions.Count : 0;
			var targetedFunctionsCount = targetedFunctions.TryGetValue(target, out var functions) ? functions.Count : 0;
			var generalFunctionsCount = generalFunctions.Count;
			var count = freeFunctionsCount + targetedFunctionsCount + generalFunctionsCount;
			if (count == 0) { results = null; return 0; }
			results = new TResult[count];
			int r = 0;
			for (int i = 0; i < freeFunctionsCount; ++i, ++r) { results[r] = freeFunctions[i](value1, value2, value3); }
			for (int i = 0; i < targetedFunctionsCount; ++i, ++r) { results[r] = functions[i](target, value1, value2, value3); }
			for (int i = 0; i < generalFunctionsCount; ++i, ++r) { results[r] = generalFunctions[i](target, value1, value2, value3); }
			return count;
		}

		public virtual int Broadcast(TTarget target, T1 value1, T2 value2, T3 value3, ref List<TResult> results) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to broadcast null"); return 0; }
			var freeFunctionsCount = freeTargetedFunctions.TryGetValue(target, out var freeFunctions) ? freeFunctions.Count : 0;
			var targetedFunctionsCount = targetedFunctions.TryGetValue(target, out var functions) ? functions.Count : 0;
			var generalFunctionsCount = generalFunctions.Count;
			var count = freeFunctionsCount + targetedFunctionsCount + generalFunctionsCount;
			if (count == 0) { results = null; return 0; }
			if (results == null) { results = new List<TResult>(count); }
			else { results.Clear(); }
			int r = 0;
			for (int i = 0; i < freeFunctionsCount; ++i, ++r) { results.Add(freeFunctions[i](value1, value2, value3)); }
			for (int i = 0; i < targetedFunctionsCount; ++i, ++r) { results.Add(functions[i](target, value1, value2, value3)); }
			for (int i = 0; i < generalFunctionsCount; ++i, ++r) { results.Add(generalFunctions[i](target, value1, value2, value3)); }
			return count;
		}

		public virtual bool Broadcast(TTarget target, T1 value1, T2 value2, T3 value3, System.Action<TResult> callback) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to broadcast null"); return false; }
			if (callback == null) { return false; }
			var freeFunctionsCount = freeTargetedFunctions.TryGetValue(target, out var freeFunctions) ? freeFunctions.Count : 0;
			var targetedFunctionsCount = targetedFunctions.TryGetValue(target, out var functions) ? functions.Count : 0;
			var generalFunctionsCount = generalFunctions.Count;
			if (freeFunctionsCount + targetedFunctionsCount + generalFunctionsCount == 0) { return false; }
			if (freeFunctionsCount != 0) { foreach (var func in freeFunctions) { callback(func(value1, value2, value3)); } }
			if (targetedFunctionsCount != 0) { foreach (var func in functions) { callback(func(target, value1, value2, value3)); } }
			foreach (var func in generalFunctions) { callback(func(target, value1, value2, value3)); }
			return true;
		}

		public virtual void Broadcast(T1 value1, T2 value2, T3 value3, System.Action<TTarget, TResult> callback) {
			if (callback == null) { return; }
			foreach (var kvp in freeTargetedFunctions) { foreach (var func in kvp.Value) { callback(kvp.Key, func(value1, value2, value3)); } }
			foreach (var kvp in targetedFunctions) { foreach (var func in kvp.Value) { callback(kvp.Key, func(kvp.Key, value1, value2, value3)); } }
		}

		// result is from the last listener
		public virtual bool TryBroadcast(TTarget target, T1 value1, T2 value2, T3 value3, out TResult result) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { result = default; UnityEngine.Debug.LogError("trying to broadcast null"); return false; }
			var freeFunctionsCount = freeTargetedFunctions.TryGetValue(target, out var freeFunctions) ? freeFunctions.Count : 0;
			var targetedFunctionsCount = targetedFunctions.TryGetValue(target, out var functions) ? functions.Count : 0;
			var generalFunctionsCount = generalFunctions.Count;
			var count = freeFunctionsCount + targetedFunctionsCount + generalFunctionsCount;
			result = default;
			if (count == 0) { return false; }
			if (freeFunctionsCount != 0) { foreach (var func in freeFunctions) { result = func(value1, value2, value3); } }
			if (targetedFunctionsCount != 0) { foreach (var func in functions) { result = func(target, value1, value2, value3); } }
			foreach (var func in generalFunctions) { result = func(target, value1, value2, value3); }
			return true;
		}
	}
	
	// four parameters
	public class TargetedReplySignal<TTarget, T1, T2, T3, T4, TResult> : ISignal {
		protected readonly HashSet<TTarget> targets = new HashSet<TTarget>();
		protected readonly Dictionary<TTarget, List<System.Func<TTarget, T1, T2, T3, T4, TResult>>> targetedFunctions = new Dictionary<TTarget, List<System.Func<TTarget, T1, T2, T3, T4, TResult>>>();
		protected readonly HashSet<TTarget> freeTargets = new HashSet<TTarget>();
		protected readonly Dictionary<TTarget, List<System.Func<T1, T2, T3, T4, TResult>>> freeTargetedFunctions = new Dictionary<TTarget, List<System.Func<T1, T2, T3, T4, TResult>>>();
		protected readonly List<System.Func<TTarget, T1, T2, T3, T4, TResult>> generalFunctions = new List<System.Func<TTarget, T1, T2, T3, T4, TResult>>();

		//

		public virtual void Register(TTarget target, System.Func<TTarget, T1, T2, T3, T4, TResult> function) {
			if (target == null || function == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to register null"); return; }
			if (!targetedFunctions.TryGetValue(target, out var functions)) { targets.Add(target); targetedFunctions.Add(target, new List<System.Func<TTarget, T1, T2, T3, T4, TResult>>() { function }); }
			else { targetedFunctions[target].Add(function); }
		}

		public virtual void Register(TTarget target, System.Func<T1, T2, T3, T4, TResult> function) {
			if (target == null || function == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to register null"); return; }
			if (!freeTargetedFunctions.TryGetValue(target, out var functions)) { freeTargets.Add(target); freeTargetedFunctions.Add(target, new List<System.Func<T1, T2, T3, T4, TResult>>() { function }); }
			else { freeTargetedFunctions[target].Add(function); }
		}

		public virtual void Register(System.Func<TTarget, T1, T2, T3, T4, TResult> function) {
			if (function == null) { UnityEngine.Debug.LogError("trying to register null"); return; }
			generalFunctions.Add(function);
		}

		public virtual void Unregister(TTarget target, System.Func<TTarget, T1, T2, T3, T4, TResult> function) {
			if (target == null || function == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			if (!targetedFunctions.TryGetValue(target, out var functions)) { return; }
			if (functions.Remove(function) && functions.Count == 0) { targets.Remove(target); targetedFunctions.Remove(target); }
		}

		public virtual void Unregister(TTarget target, System.Func<T1, T2, T3, T4, TResult> function) {
			if (target == null || function == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			if (!freeTargetedFunctions.TryGetValue(target, out var functions)) { return; }
			if (functions.Remove(function) && functions.Count == 0) { freeTargets.Remove(target); freeTargetedFunctions.Remove(target); }
		}

		public virtual void Unregister(System.Func<T1, T2, T3, T4, TResult> function) {
			if (function == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			foreach (var target in freeTargets) {
				var functions = freeTargetedFunctions[target];
				if (functions.Remove(function) && functions.Count == 0) { freeTargetedFunctions.Remove(target); }
			}
			freeTargets.RemoveWhere(t => !freeTargetedFunctions.ContainsKey(t));
		}

		public virtual void Unregister(System.Func<TTarget, T1, T2, T3, T4, TResult> function) {
			if (function == null) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			foreach (var target in targets) {
				var functions = targetedFunctions[target];
				if (functions.Remove(function) && functions.Count == 0) { targetedFunctions.Remove(target); }
			}
			targets.RemoveWhere(t => !targetedFunctions.ContainsKey(t));
			generalFunctions.Remove(function);
		}

		public virtual void Unregister(TTarget target) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to unregister null"); return; }
			targets.Remove(target);
			targetedFunctions.Remove(target);
			freeTargets.Remove(target);
			freeTargetedFunctions.Remove(target);
		}
		
		public virtual void UnregisterAll() {
			targets.Clear();
			targetedFunctions.Clear();
			freeTargets.Clear();
			freeTargetedFunctions.Clear();
			generalFunctions.Clear();
		}

		// returns default if there are no listeners!
		// result is from the last listener
		public virtual TResult Broadcast(TTarget target, T1 value1, T2 value2, T3 value3, T4 value4) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to broadcast null"); return default; }
			TResult lastResult = default;
			if (freeTargetedFunctions.TryGetValue(target, out var freeFunctions)) { foreach (var func in freeFunctions) { lastResult = func(value1, value2, value3, value4); } }
			if (targetedFunctions.TryGetValue(target, out var functions)) { foreach (var func in functions) { lastResult = func(target, value1, value2, value3, value4); } }
			foreach (var func in generalFunctions) { lastResult = func(target, value1, value2, value3, value4); }
			return lastResult;
		}

		public virtual int Broadcast(TTarget target, T1 value1, T2 value2, T3 value3, T4 value4, out TResult[] results) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { results = null; UnityEngine.Debug.LogError("trying to broadcast null"); return 0; }
			var freeFunctionsCount = freeTargetedFunctions.TryGetValue(target, out var freeFunctions) ? freeFunctions.Count : 0;
			var targetedFunctionsCount = targetedFunctions.TryGetValue(target, out var functions) ? functions.Count : 0;
			var generalFunctionsCount = generalFunctions.Count;
			var count = freeFunctionsCount + targetedFunctionsCount + generalFunctionsCount;
			if (count == 0) { results = null; return 0; }
			results = new TResult[count];
			int r = 0;
			for (int i = 0; i < freeFunctionsCount; ++i, ++r) { results[r] = freeFunctions[i](value1, value2, value3, value4); }
			for (int i = 0; i < targetedFunctionsCount; ++i, ++r) { results[r] = functions[i](target, value1, value2, value3, value4); }
			for (int i = 0; i < generalFunctionsCount; ++i, ++r) { results[r] = generalFunctions[i](target, value1, value2, value3, value4); }
			return count;
		}

		public virtual int Broadcast(TTarget target, T1 value1, T2 value2, T3 value3, T4 value4, ref List<TResult> results) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to broadcast null"); return 0; }
			var freeFunctionsCount = freeTargetedFunctions.TryGetValue(target, out var freeFunctions) ? freeFunctions.Count : 0;
			var targetedFunctionsCount = targetedFunctions.TryGetValue(target, out var functions) ? functions.Count : 0;
			var generalFunctionsCount = generalFunctions.Count;
			var count = freeFunctionsCount + targetedFunctionsCount + generalFunctionsCount;
			if (count == 0) { results = null; return 0; }
			if (results == null) { results = new List<TResult>(count); }
			else { results.Clear(); }
			int r = 0;
			for (int i = 0; i < freeFunctionsCount; ++i, ++r) { results.Add(freeFunctions[i](value1, value2, value3, value4)); }
			for (int i = 0; i < targetedFunctionsCount; ++i, ++r) { results.Add(functions[i](target, value1, value2, value3, value4)); }
			for (int i = 0; i < generalFunctionsCount; ++i, ++r) { results.Add(generalFunctions[i](target, value1, value2, value3, value4)); }
			return count;
		}

		public virtual bool Broadcast(TTarget target, T1 value1, T2 value2, T3 value3, T4 value4, System.Action<TResult> callback) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to broadcast null"); return false; }
			if (callback == null) { return false; }
			var freeFunctionsCount = freeTargetedFunctions.TryGetValue(target, out var freeFunctions) ? freeFunctions.Count : 0;
			var targetedFunctionsCount = targetedFunctions.TryGetValue(target, out var functions) ? functions.Count : 0;
			if (freeFunctionsCount + targetedFunctionsCount + generalFunctions.Count == 0) { return false; }
			if (freeFunctionsCount != 0) { foreach (var func in freeFunctions) { callback(func(value1, value2, value3, value4)); } }
			if (targetedFunctionsCount != 0) { foreach (var func in functions) { callback(func(target, value1, value2, value3, value4)); } }
			foreach (var func in generalFunctions) { callback(func(target, value1, value2, value3, value4)); }
			return true;
		}

		public virtual void Broadcast(T1 value1, T2 value2, T3 value3, T4 value4, System.Action<TTarget, TResult> callback) {
			if (callback == null) { return; }
			foreach (var kvp in freeTargetedFunctions) { foreach (var func in kvp.Value) { callback(kvp.Key, func(value1, value2, value3, value4)); } }
			foreach (var kvp in targetedFunctions) { foreach (var func in kvp.Value) { callback(kvp.Key, func(kvp.Key, value1, value2, value3, value4)); } }
		}

		// result is from the last listener
		public virtual bool TryBroadcast(TTarget target, T1 value1, T2 value2, T3 value3, T4 value4, out TResult result) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { result = default; UnityEngine.Debug.LogError("trying to broadcast null"); return false; }
			var freeFunctionsCount = freeTargetedFunctions.TryGetValue(target, out var freeFunctions) ? freeFunctions.Count : 0;
			var targetedFunctionsCount = targetedFunctions.TryGetValue(target, out var functions) ? functions.Count : 0;
			var count = freeFunctionsCount + targetedFunctionsCount + generalFunctions.Count;
			result = default;
			if (count == 0) { return false; }
			if (freeFunctionsCount != 0) { foreach (var func in freeFunctions) { result = func(value1, value2, value3, value4); } }
			if (targetedFunctionsCount != 0) { foreach (var func in functions) { result = func(target, value1, value2, value3, value4); } }
			foreach (var func in generalFunctions) { result = func(target, value1, value2, value3, value4); }
			return true;
		}
	}

	//

	// // Confirmation Signals are just Reply Signals with bool as TResult
	// // Example usage:
	// public static class Signals {
	//     public readonly static ConfirmationSignal WantToPlay = new ConfirmationSignal();
	// }
	// Signals.WantToPlay.Register(CheckWantToPlay); // -> void CheckWantToPlay() { return Random.value < 0.5f; }
	// if (Signals.WantToPlay.Broadcast()) { Debug.Log("Get played!"); }

	// zero parameters
	public class ConfirmationSignal : ReplySignal<bool> {
		public virtual void Broadcast(System.Action callback) {
			if (callback == null) { return; }
			foreach (var func in functions) { if (func()) { callback(); } }
		}
	}

	// one parameter
	public class ConfirmationSignal<T> : ReplySignal<T, bool> {
		public virtual void Broadcast(T value, System.Action callback) {
			if (callback == null) { return; }
			foreach (var func in functions) { if (func(value)) { callback(); } }
		}
	}

	// two parameters
	public class ConfirmationSignal<T1, T2> : ReplySignal<T1, T2, bool> {
		public virtual void Broadcast(T1 value1, T2 value2, System.Action callback) {
			if (callback == null) { return; }
			foreach (var func in functions) { if (func(value1, value2)) { callback(); } }
		}
	}

	// three parameters
	public class ConfirmationSignal<T1, T2, T3> : ReplySignal<T1, T2, T3, bool> {
		public virtual void Broadcast(T1 value1, T2 value2, T3 value3, System.Action callback) {
			if (callback == null) { return; }
			foreach (var func in functions) { if (func(value1, value2, value3)) { callback(); } }
		}
	}

	// four parameters
	public class ConfirmationSignal<T1, T2, T3, T4> : ReplySignal<T1, T2, T3, T4, bool> {
		public virtual void Broadcast(T1 value1, T2 value2, T3 value3, T4 value4, System.Action callback) {
			if (callback == null) { return; }
			foreach (var func in functions) { if (func(value1, value2, value3, value4)) { callback(); } }
		}
	}

	//

	// zero parameters
	public class TargetedConfirmationSignal<TTarget> : TargetedReplySignal<TTarget, bool> {
		public virtual void Broadcast(TTarget target, System.Action callback) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to broadcast null"); return; }
			if (callback == null) { return; }
			if (freeTargetedFunctions.TryGetValue(target, out var freeFunctions)) { foreach (var func in freeFunctions) { if (func()) { callback(); } } }
			if (targetedFunctions.TryGetValue(target, out var functions)) { foreach (var func in functions) { if (func(target)) { callback(); } } }
			foreach (var func in generalFunctions) { if (func(target)) { callback(); }; }
		}

		public virtual void Broadcast(System.Action<TTarget> callback) {
			if (callback == null) { return; }
			foreach (var kvp in freeTargetedFunctions) { foreach (var func in kvp.Value) { if (func()) { callback(kvp.Key); } } }
			foreach (var kvp in targetedFunctions) { foreach (var func in kvp.Value) { if (func(kvp.Key)) { callback(kvp.Key); } } }
		}
	}

	// one parameter
	public class TargetedConfirmationSignal<TTarget, T> : TargetedReplySignal<TTarget, T, bool> {
		public virtual void Broadcast(TTarget target, T value, System.Action callback) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to broadcast null"); return; }
			if (callback == null) { return; }
			if (freeTargetedFunctions.TryGetValue(target, out var freeFunctions)) { foreach (var func in freeFunctions) { if (func(value)) { callback(); } } }
			if (targetedFunctions.TryGetValue(target, out var functions)) { foreach (var func in functions) { if (func(target, value)) { callback(); } } }
			foreach (var func in generalFunctions) { if (func(target, value)) { callback(); }; }
		}

		public virtual void Broadcast(T value, System.Action<TTarget> callback) {
			if (callback == null) { return; }
			foreach (var kvp in freeTargetedFunctions) { foreach (var func in kvp.Value) { if (func(value)) { callback(kvp.Key); } } }
			foreach (var kvp in targetedFunctions) { foreach (var func in kvp.Value) { if (func(kvp.Key, value)) { callback(kvp.Key); } } }
		}
	}
	
	// two parameters
	public class TargetedConfirmationSignal<TTarget, T1, T2> : TargetedReplySignal<TTarget, T1, T2, bool> {
		public virtual void Broadcast(TTarget target, T1 value1, T2 value2, System.Action callback) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to broadcast null"); return; }
			if (callback == null) { return; }
			if (freeTargetedFunctions.TryGetValue(target, out var freeFunctions)) { foreach (var func in freeFunctions) { if (func(value1, value2)) { callback(); } } }
			if (targetedFunctions.TryGetValue(target, out var functions)) { foreach (var func in functions) { if (func(target, value1, value2)) { callback(); } } }
			foreach (var func in generalFunctions) { if (func(target, value1, value2)) { callback(); }; }
		}

		public virtual void Broadcast(T1 value1, T2 value2, System.Action<TTarget> callback) {
			if (callback == null) { return; }
			foreach (var kvp in freeTargetedFunctions) { foreach (var func in kvp.Value) { if (func(value1, value2)) { callback(kvp.Key); } } }
			foreach (var kvp in targetedFunctions) { foreach (var func in kvp.Value) { if (func(kvp.Key, value1, value2)) { callback(kvp.Key); } } }
		}
	}

	// three parameters
	public class TargetedConfirmationSignal<TTarget, T1, T2, T3> : TargetedReplySignal<TTarget, T1, T2, T3, bool> {
		public virtual void Broadcast(TTarget target, T1 value1, T2 value2, T3 value3, System.Action callback) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to broadcast null"); return; }
			if (callback == null) { return; }
			if (freeTargetedFunctions.TryGetValue(target, out var freeFunctions)) { foreach (var func in freeFunctions) { if (func(value1, value2, value3)) { callback(); } } }
			if (targetedFunctions.TryGetValue(target, out var functions)) { foreach (var func in functions) { if (func(target, value1, value2, value3)) { callback(); } } }
			foreach (var func in generalFunctions) { if (func(target, value1, value2, value3)) { callback(); }; }
		}

		public virtual void Broadcast(T1 value1, T2 value2, T3 value3, System.Action<TTarget> callback) {
			if (callback == null) { return; }
			foreach (var kvp in freeTargetedFunctions) { foreach (var func in kvp.Value) { if (func(value1, value2, value3)) { callback(kvp.Key); } } }
			foreach (var kvp in targetedFunctions) { foreach (var func in kvp.Value) { if (func(kvp.Key, value1, value2, value3)) { callback(kvp.Key); } } }
		}
	}
	
	// four parameters
	public class TargetedConfirmationSignal<TTarget, T1, T2, T3, T4> : TargetedReplySignal<TTarget, T1, T2, T3, T4, bool> {
		public virtual void Broadcast(TTarget target, T1 value1, T2 value2, T3 value3, T4 value4, System.Action callback) {
			if (target == null || (target is UnityEngine.Object obj && obj == null)) { UnityEngine.Debug.LogError("trying to broadcast null"); return; }
			if (callback == null) { return; }
			if (freeTargetedFunctions.TryGetValue(target, out var freeFunctions)) { foreach (var func in freeFunctions) { if (func(value1, value2, value3, value4)) { callback(); } } }
			if (targetedFunctions.TryGetValue(target, out var functions)) { foreach (var func in functions) { if (func(target, value1, value2, value3, value4)) { callback(); } } }
			foreach (var func in generalFunctions) { if (func(target, value1, value2, value3, value4)) { callback(); }; }
		}

		public virtual void Broadcast(T1 value1, T2 value2, T3 value3, T4 value4, System.Action<TTarget> callback) {
			if (callback == null) { return; }
			foreach (var kvp in freeTargetedFunctions) { foreach (var func in kvp.Value) { if (func(value1, value2, value3, value4)) { callback(kvp.Key); } } }
			foreach (var kvp in targetedFunctions) { foreach (var func in kvp.Value) { if (func(kvp.Key, value1, value2, value3, value4)) { callback(kvp.Key); } } }
		}
	}

}