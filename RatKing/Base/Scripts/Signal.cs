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
		protected HashSet<TTarget> targets = new HashSet<TTarget>();
		protected Dictionary<TTarget, System.Action<TTarget>> targetedActions = new Dictionary<TTarget, System.Action<TTarget>>();
		protected HashSet<TTarget> freeTargets = new HashSet<TTarget>();
		protected Dictionary<TTarget, System.Action> freeTargetedActions = new Dictionary<TTarget, System.Action>();
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
		protected HashSet<TTarget> targets = new HashSet<TTarget>();
		protected Dictionary<TTarget, System.Action<TTarget, T>> targetedActions = new Dictionary<TTarget, System.Action<TTarget, T>>();
		protected HashSet<TTarget> freeTargets = new HashSet<TTarget>();
		protected Dictionary<TTarget, System.Action<T>> freeTargetedActions = new Dictionary<TTarget, System.Action<T>>();
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
		protected HashSet<TTarget> targets = new HashSet<TTarget>();
		protected Dictionary<TTarget, System.Action<TTarget, T1, T2>> targetedActions = new Dictionary<TTarget, System.Action<TTarget, T1, T2>>();
		protected HashSet<TTarget> freeTargets = new HashSet<TTarget>();
		protected Dictionary<TTarget, System.Action<T1, T2>> freeTargetedActions = new Dictionary<TTarget, System.Action<T1, T2>>();
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
		protected HashSet<TTarget> targets = new HashSet<TTarget>();
		protected Dictionary<TTarget, System.Action<TTarget, T1, T2, T3>> targetedActions = new Dictionary<TTarget, System.Action<TTarget, T1, T2, T3>>();
		protected HashSet<TTarget> freeTargets = new HashSet<TTarget>();
		protected Dictionary<TTarget, System.Action<T1, T2, T3>> freeTargetedActions = new Dictionary<TTarget, System.Action<T1, T2, T3>>();
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
		protected HashSet<TTarget> targets = new HashSet<TTarget>();
		protected Dictionary<TTarget, System.Action<TTarget, T1, T2, T3, T4>> targetedActions = new Dictionary<TTarget, System.Action<TTarget, T1, T2, T3, T4>>();
		protected HashSet<TTarget> freeTargets = new HashSet<TTarget>();
		protected Dictionary<TTarget, System.Action<T1, T2, T3, T4>> freeTargetedActions = new Dictionary<TTarget, System.Action<T1, T2, T3, T4>>();
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
		protected HashSet<TTarget> targetsWereBroadcast = new HashSet<TTarget>();

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
			base.Broadcast(target);
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
		protected Dictionary<TTarget, T> targetsWereBroadcast = new Dictionary<TTarget, T>();
		protected static List<TTarget> targetsToRemove = new List<TTarget>();

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
			base.Broadcast(target, value);
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
		protected Dictionary<TTarget, (T1, T2)> targetsWereBroadcast = new Dictionary<TTarget, (T1, T2)>();
		protected static List<TTarget> targetsToRemove = new List<TTarget>();

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
			base.Broadcast(target, value1, value2);
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
		protected Dictionary<TTarget, (T1, T2, T3)> targetsWereBroadcast = new Dictionary<TTarget, (T1, T2, T3)>();
		protected static List<TTarget> targetsToRemove = new List<TTarget>();

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
			base.Broadcast(target, value1, value2, value3);
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
		protected Dictionary<TTarget, (T1, T2, T3, T4)> targetsWereBroadcast = new Dictionary<TTarget, (T1, T2, T3, T4)>();
		protected static List<TTarget> targetsToRemove = new List<TTarget>();

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
			base.Broadcast(target, value1, value2, value3, value4);
			targetsWereBroadcast[target] = (value1, value2, value3, value4);
		}

		public void Reset(TTarget target) {
			targetsWereBroadcast.Remove(target);
		}

		public void ResetAll() {
			targetsWereBroadcast.Clear();
		}
	}
}