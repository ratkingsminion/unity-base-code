// #define DEBUG_EVENTS

using System.Collections.Generic;
using UnityEngine;

namespace RatKing.Base {
	
	public class Event : System.IEquatable<Event> {
		protected readonly string name;
		protected readonly int hash;
		public Event(string name) {
			this.name = name;
			hash = name.GetHashCode();
		}
		public bool Equals(Event other) {
			return name == other.name;
		}
		public override int GetHashCode() {
			return hash;
		}
		public override string ToString() {
			return name;
		}
	}

	public class Event<T> : Event, System.IEquatable<Event<T>> {
		public Event(string name) : base(name) { }
		public bool Equals(Event<T> other) {
			return name == other.name;
		}
	}

	public class Event<T1, T2> : Event, System.IEquatable<Event<T1, T2>> {
		public Event(string name) : base(name) { }
		public bool Equals(Event<T1, T2> other) {
			return name == other.name;
		}
	}

	public class Event<T1, T2, T3> : Event, System.IEquatable<Event<T1, T2, T3>> {
		public Event(string name) : base(name) { }
		public bool Equals(Event<T1, T2, T3> other) {
			return name == other.name;
		}
	}

	//

	public class Events : MonoBehaviour {
		static Stack<List<object>> objectListPool;
		static List<object> PooledObjectListPop(object first = null) {
			var list = objectListPool.Count == 0 ? new List<object>() : objectListPool.Pop();
			if (first != null) { list.Add(first); }
			return list;
		}
		static void PooledObjectListPush(List<object> list) {
			if (list != null) { list.Clear(); objectListPool.Push(list); }
		}

		static Stack<Dictionary<string, List<object>>> stringDictPool;
		static Dictionary<string, List<object>> PooledStringDictPop() {
			return stringDictPool.Count == 0 ? new Dictionary<string, List<object>>() : stringDictPool.Pop();
		}
		static void PooledStringDictPush(Dictionary<string, List<object>> dict) {
			if (dict != null) { dict.Clear(); stringDictPool.Push(dict); }
		}

		static Stack<Dictionary<Event, List<object>>> eventDictPool;
		static Dictionary<Event, List<object>> PooledEventDictPop() {
			return eventDictPool.Count == 0 ? new Dictionary<Event, List<object>>() : eventDictPool.Pop();
		}
		static void PooledEventDictPush(Dictionary<Event, List<object>> dict) {
			if (dict != null) { dict.Clear(); eventDictPool.Push(dict); }
		}

		//

		static Events instance;
		Dictionary<object, Dictionary<string, List<object>>> channelsStringsActions;
		Dictionary<string, List<object>> allStringsActions;
		Dictionary<object, Dictionary<Event, List<object>>> channelsEventsActions;
		Dictionary<Event, List<object>> allEventsActions;

		//

		public static void Init() {
			if (instance != null) { return; }
			objectListPool = new Stack<List<object>>();
			stringDictPool = new Stack<Dictionary<string, List<object>>>();
			eventDictPool = new Stack<Dictionary<Event, List<object>>>();
			instance = new GameObject("<EVENTS>").AddComponent<Events>();
			DontDestroyOnLoad(instance.gameObject);
			instance.channelsStringsActions = new Dictionary<object, Dictionary<string, List<object>>>();
			instance.allStringsActions = new Dictionary<string, List<object>>();
			instance.channelsEventsActions = new Dictionary<object, Dictionary<Event, List<object>>>();
			instance.allEventsActions = new Dictionary<Event, List<object>>();
		}

#if DEBUG_EVENTS && UNITY_EDITOR
		void OnGUI() {
			string msg =
				"objectListPool:" + objectListPool.Count + " " + "stringDictPool:" + stringDictPool.Count + " " + "eventDictPool:" + eventDictPool.Count + "\n" +
				"stringChannels:" + channelsStringsActions.Count + "\n" +
				"allStrings:" + allStringsActions.Count;
			foreach (var ea in allStringsActions) {
				msg += "\n\t" + ea.Key + ":" + ea.Value.Count;
			}				
			msg += "\n" +
				"eventChannels:" + channelsEventsActions.Count + "\n" +
				"allEvents:" + allEventsActions.Count;
			foreach (var ea in allEventsActions) {
				msg += "\n\t" + ea.Key.ToString() + ":" + ea.Value.Count;
			}				
			GUI.Label(new Rect(10, 10, 500, 500), msg);
		}
#endif

		//

		public static void UnregisterAll(object channel, Event @event) {
			if (instance == null) { return; }
			Dictionary<Event, List<object>> eventsActions;
			// events: channel-specific
			if (instance.channelsEventsActions.TryGetValue(channel, out eventsActions)) {
				List<object> actions;
				if (!eventsActions.TryGetValue(@event, out actions)) { return; }
				eventsActions.Remove(@event);
				// events: all
				List<object> actionsAll = instance.allEventsActions[@event];
				foreach (var a in actions) { actionsAll.Remove(a); }
				if (actionsAll.Count == 0) {
					PooledObjectListPush(actionsAll);
					instance.allEventsActions.Remove(@event);
				}
				if (eventsActions.Count == 0) {
					PooledEventDictPush(eventsActions);
					instance.channelsEventsActions.Remove(channel);
				}
				PooledObjectListPush(actions);
			}
		}

		public static void UnregisterAll(object channel, string @event) {
			if (instance == null) { return; }
			Dictionary<string, List<object>> stringsActions;
			// strings: channel-specific
			if (instance.channelsStringsActions.TryGetValue(channel, out stringsActions)) {
				List<object> actions;
				if (!stringsActions.TryGetValue(@event, out actions)) { return; }
				stringsActions.Remove(@event);
				// strings: all
				List<object> actionsAll = instance.allStringsActions[@event];
				foreach (var a in actions) { actionsAll.Remove(a); }
				if (actionsAll.Count == 0) {
					PooledObjectListPush(actionsAll);
					instance.allStringsActions.Remove(@event);
				}
				if (stringsActions.Count == 0) {
					PooledStringDictPush(stringsActions);
					instance.channelsStringsActions.Remove(channel);
				}
				PooledObjectListPush(actions);
			}
		}

		public static void UnregisterAll(object channel) {
			if (instance == null) { return; }
			Dictionary<Event, List<object>> eventsActions;
			Dictionary<string, List<object>> stringsActions;
			// events: channel-specific + all
			if (instance.channelsEventsActions.TryGetValue(channel, out eventsActions)) {
				foreach (var ea in eventsActions) {
					List<object> actions = instance.allEventsActions[ea.Key];
					foreach (var a in ea.Value) { actions.Remove(a); }
					if (actions.Count == 0) {
						PooledObjectListPush(actions);
						instance.allEventsActions.Remove(ea.Key);
					}
					PooledObjectListPush(ea.Value);
				}
				PooledEventDictPush(eventsActions);
				instance.channelsEventsActions.Remove(channel);
			}
			// strings: channel-specific + all
			if (instance.channelsStringsActions.TryGetValue(channel, out stringsActions)) {
				foreach (var ea in stringsActions) {
					List<object> actions = instance.allStringsActions[ea.Key];
					foreach (var a in ea.Value) { actions.Remove(a); }
					if (actions.Count == 0) {
						PooledObjectListPush(actions);
						instance.allStringsActions.Remove(ea.Key);
					}
					PooledObjectListPush(ea.Value);
				}
				PooledStringDictPush(stringsActions);
				instance.channelsStringsActions.Remove(channel);
			}
		}

		// no parameters / string as event identifier

		public static void Register(object channel, string @event, System.Action action) {
			if (instance == null) { Init(); }
			Dictionary<string, List<object>> stringsActions;
			List<object> actions = null;
			// channel-specific
			if (!instance.channelsStringsActions.TryGetValue(channel, out stringsActions)) {
				instance.channelsStringsActions[channel] = stringsActions = PooledStringDictPop();
			}
			if (stringsActions.TryGetValue(@event, out actions)) { actions.Add(action); }
			else { stringsActions[@event] = PooledObjectListPop(action); }
			// all
			if (instance.allStringsActions.TryGetValue(@event, out actions)) { actions.Add(action); }
			else { instance.allStringsActions[@event] = PooledObjectListPop(action); }
		}

		public static void Unregister(object channel, string @event, System.Action action) {
			if (instance == null) { return; }
			Dictionary<string, List<object>> stringsActions;
			List<object> actions;
			// channel-specific
			if (instance.channelsStringsActions.TryGetValue(channel, out stringsActions)) {
				if (stringsActions.TryGetValue(@event, out actions)) {
					actions.Remove(action);
					if (actions.Count == 0) {
						PooledObjectListPush(actions);
						stringsActions.Remove(@event);
						if (stringsActions.Count == 0) {
							PooledStringDictPush(stringsActions);
							instance.channelsStringsActions.Remove(channel);
						}
					}
				}
			}
			// all
			if (instance.allStringsActions.TryGetValue(@event, out actions)) {
				actions.Remove(action);
				if (actions.Count == 0) {
					PooledObjectListPush(actions);
					instance.allStringsActions.Remove(@event);
				}
			}
		}

		public static void Broadcast(object channel, string @event) {
			if (instance == null) { return; }
			Dictionary<string, List<object>> stringsActions;
			List<object> actions;
			// channel-specific only
			if (instance.channelsStringsActions.TryGetValue(channel, out stringsActions)) {
				if (stringsActions.TryGetValue(@event, out actions)) {
					for (int i = actions.Count - 1; i >= 0; --i) {
						((System.Action)actions[i])();
					}
				}
			}
		}

		public static void BroadcastAll(string @event) {
			if (instance == null) { return; }
			List<object> actions;
			// all only
			if (instance.allStringsActions.TryGetValue(@event, out actions)) {
				for (int i = actions.Count - 1; i >= 0; --i) {
					((System.Action)actions[i])();
				}
			}
		}

		// no parameters / Event as event identifier

		public static void Register(object channel, Event @event, System.Action action) {
			if (instance == null) { Init(); }
			Dictionary<Event, List<object>> eventsActions;
			List<object> actions = null;
			// channel-specific
			if (!instance.channelsEventsActions.TryGetValue(channel, out eventsActions)) {
				instance.channelsEventsActions[channel] = eventsActions = PooledEventDictPop();
			}
			if (eventsActions.TryGetValue(@event, out actions)) { actions.Add(action); }
			else { eventsActions[@event] = PooledObjectListPop(action); }
			// all
			if (instance.allEventsActions.TryGetValue(@event, out actions)) { actions.Add(action); }
			else { instance.allEventsActions[@event] = PooledObjectListPop(action); }
		}

		public static void Unregister(object channel, Event @event, System.Action action) {
			if (instance == null) { return; }
			Dictionary<Event, List<object>> eventsActions;
			List<object> actions;
			// channel-specific
			if (instance.channelsEventsActions.TryGetValue(channel, out eventsActions)) {
				if (eventsActions.TryGetValue(@event, out actions)) {
					actions.Remove(action);
					if (actions.Count == 0) {
						PooledObjectListPush(actions);
						eventsActions.Remove(@event);
						if (eventsActions.Count == 0) {
							PooledEventDictPush(eventsActions);
							instance.channelsEventsActions.Remove(channel);
						}
					}
				}
			}
			// all
			if (instance.allEventsActions.TryGetValue(@event, out actions)) {
				actions.Remove(action);
				if (actions.Count == 0) {
					PooledObjectListPush(actions);
					instance.allEventsActions.Remove(@event);
				}
			}
		}

		public static void Broadcast(object channel, Event @event) {
			if (instance == null) { return; }
			Dictionary<Event, List<object>> eventsActions;
			List<object> actions;
			// channel-specific only
			if (instance.channelsEventsActions.TryGetValue(channel, out eventsActions)) {
				if (eventsActions.TryGetValue(@event, out actions)) {
					for (int i = actions.Count - 1; i >= 0; --i) {
						((System.Action)actions[i])();
					}
				}
			}
		}

		public static void BroadcastAll(Event @event) {
			if (instance == null) { return; }
			List<object> actions;
			// all only
			if (instance.allEventsActions.TryGetValue(@event, out actions)) {
				for (int i = actions.Count - 1; i >= 0; --i) {
					((System.Action)actions[i])();
				}
			}
		}

		// one parameter / string as event identifier

		public static void Register<T>(object channel, string @event, System.Action<T> action) {
			if (instance == null) { Init(); }
			Dictionary<string, List<object>> eventsActions;
			List<object> actions = null;
			// channel-specific
			if (!instance.channelsStringsActions.TryGetValue(channel, out eventsActions)) {
				instance.channelsStringsActions[channel] = eventsActions = PooledStringDictPop();
			}
			if (eventsActions.TryGetValue(@event, out actions)) { actions.Add(action); }
			else { eventsActions[@event] = PooledObjectListPop(action); }
			// all
			if (instance.allStringsActions.TryGetValue(@event, out actions)) { actions.Add(action); }
			else { instance.allStringsActions[@event] = PooledObjectListPop(action); }
		}

		public static void Unregister<T>(object channel, string @event, System.Action<T> action) {
			if (instance == null) { return; }
			Dictionary<string, List<object>> eventsActions;
			List<object> actions;
			// channel-specific
			if (instance.channelsStringsActions.TryGetValue(channel, out eventsActions)) {
				if (eventsActions.TryGetValue(@event, out actions)) {
					actions.Remove(action);
					if (actions.Count == 0) {
						PooledObjectListPush(actions);
						eventsActions.Remove(@event);
						if (eventsActions.Count == 0) {
							PooledStringDictPush(eventsActions);
							instance.channelsStringsActions.Remove(channel);
						}
					}
				}
			}
			// all
			if (instance.allStringsActions.TryGetValue(@event, out actions)) {
				actions.Remove(action);
				if (actions.Count == 0) {
					PooledObjectListPush(actions);
					instance.allStringsActions.Remove(@event);
				}
			}
		}

		public static void Broadcast<T>(object channel, string @event, T value) {
			if (instance == null) { return; }
			Dictionary<string, List<object>> eventsActions;
			List<object> actions;
			// channel-specific only
			if (instance.channelsStringsActions.TryGetValue(channel, out eventsActions)) {
				if (eventsActions.TryGetValue(@event, out actions)) {
					for (int i = actions.Count - 1; i >= 0; --i) {
						((System.Action)actions[i])(value);
					}
				}
			}
		}

		public static void BroadcastAll<T>(string @event, T value) {
			if (instance == null) { return; }
			List<object> actions;
			// all only
			if (instance.allStringsActions.TryGetValue(@event, out actions)) {
				for (int i = actions.Count - 1; i >= 0; --i) {
					((System.Action)actions[i])(value);
				}
			}
		}

		// one parameter / Event as event identifier

		public static void Register<T>(object channel, Event<T> @event, System.Action<T> action) {
			if (instance == null) { Init(); }
			Dictionary<Event, List<object>> eventsActions;
			List<object> actions = null;
			// channel-specific
			if (!instance.channelsEventsActions.TryGetValue(channel, out eventsActions)) {
				instance.channelsEventsActions[channel] = eventsActions = PooledEventDictPop();
			}
			if (eventsActions.TryGetValue(@event, out actions)) { actions.Add(action); }
			else { eventsActions[@event] = PooledObjectListPop(action); }
			// all
			if (instance.allEventsActions.TryGetValue(@event, out actions)) { actions.Add(action); }
			else { instance.allEventsActions[@event] = PooledObjectListPop(action); }
		}

		public static void Unregister<T>(object channel, Event<T> @event, System.Action<T> action) {
			if (instance == null) { return; }
			Dictionary<Event, List<object>> eventsActions;
			List<object> actions;
			// channel-specific
			if (instance.channelsEventsActions.TryGetValue(channel, out eventsActions)) {
				if (eventsActions.TryGetValue(@event, out actions)) {
					actions.Remove(action);
					if (actions.Count == 0) {
						PooledObjectListPush(actions);
						eventsActions.Remove(@event);
						if (eventsActions.Count == 0) {
							PooledEventDictPush(eventsActions);
							instance.channelsEventsActions.Remove(channel);
						}
					}
				}
			}
			// all
			if (instance.allEventsActions.TryGetValue(@event, out actions)) {
				actions.Remove(action);
				if (actions.Count == 0) {
					PooledObjectListPush(actions);
					instance.allEventsActions.Remove(@event);
				}
			}
		}

		public static void Broadcast<T>(object channel, Event<T> @event, T value) {
			if (instance == null) { return; }
			Dictionary<Event, List<object>> eventsActions;
			List<object> actions;
			// channel-specific only
			if (instance.channelsEventsActions.TryGetValue(channel, out eventsActions)) {
				if (eventsActions.TryGetValue(@event, out actions)) {
					for (int i = actions.Count - 1; i >= 0; --i) {
						((System.Action<T>)actions[i])(value);
					}
				}
			}
		}

		public static void BroadcastAll<T>(Event<T> @event, T value) {
			if (instance == null) { return; }
			List<object> actions;
			// all only
			if (instance.allEventsActions.TryGetValue(@event, out actions)) {
				for (int i = actions.Count - 1; i >= 0; --i) {
					((System.Action<T>)actions[i])(value);
				}
			}
		}

		// two parameters / Event as event identifier

		public static void Register<T1, T2>(object channel, Event<T1, T2> @event, System.Action<T1, T2> action) {
			if (instance == null) { Init(); }
			Dictionary<Event, List<object>> eventsActions;
			List<object> actions = null;
			// channel-specific
			if (!instance.channelsEventsActions.TryGetValue(channel, out eventsActions)) {
				instance.channelsEventsActions[channel] = eventsActions = PooledEventDictPop();
			}
			if (eventsActions.TryGetValue(@event, out actions)) { actions.Add(action); }
			else { eventsActions[@event] = PooledObjectListPop(action); }
			// all
			if (instance.allEventsActions.TryGetValue(@event, out actions)) { actions.Add(action); }
			else { instance.allEventsActions[@event] = PooledObjectListPop(action); }
		}

		public static void Unregister<T1, T2>(object channel, Event<T1, T2> @event, System.Action<T1, T2> action) {
			if (instance == null) { return; }
			Dictionary<Event, List<object>> eventsActions;
			List<object> actions;
			// channel-specific
			if (instance.channelsEventsActions.TryGetValue(channel, out eventsActions)) {
				if (eventsActions.TryGetValue(@event, out actions)) {
					actions.Remove(action);
					if (actions.Count == 0) {
						PooledObjectListPush(actions);
						eventsActions.Remove(@event);
						if (eventsActions.Count == 0) {
							PooledEventDictPush(eventsActions);
							instance.channelsEventsActions.Remove(channel);
						}
					}
				}
			}
			// all
			if (instance.allEventsActions.TryGetValue(@event, out actions)) {
				actions.Remove(action);
				if (actions.Count == 0) {
					PooledObjectListPush(actions);
					instance.allEventsActions.Remove(@event);
				}
			}
		}

		public static void Broadcast<T1, T2>(object channel, Event<T1, T2> @event, T1 value1, T2 value2) {
			if (instance == null) { return; }
			Dictionary<Event, List<object>> eventsActions;
			List<object> actions;
			// channel-specific only
			if (instance.channelsEventsActions.TryGetValue(channel, out eventsActions)) {
				if (eventsActions.TryGetValue(@event, out actions)) {
					for (int i = actions.Count - 1; i >= 0; --i) {
						((System.Action<T1, T2>)actions[i])(value1, value2);
					}
				}
			}
		}

		public static void BroadcastAll<T1, T2>(Event<T1, T2> @event, T1 value1, T2 value2) {
			if (instance == null) { return; }
			List<object> actions;
			// all only
			if (instance.allEventsActions.TryGetValue(@event, out actions)) {
				for (int i = actions.Count - 1; i >= 0; --i) {
					((System.Action<T1, T2>)actions[i])(value1, value2);
				}
			}
		}

		// three parameters / Event as event identifier

		public static void Register<T1, T2, T3>(object channel, Event<T1, T2, T3> @event, System.Action<T1, T2, T3> action) {
			if (instance == null) { Init(); }
			Dictionary<Event, List<object>> eventsActions;
			List<object> actions = null;
			// channel-specific
			if (!instance.channelsEventsActions.TryGetValue(channel, out eventsActions)) {
				instance.channelsEventsActions[channel] = eventsActions = PooledEventDictPop();
			}
			if (eventsActions.TryGetValue(@event, out actions)) { actions.Add(action); }
			else { eventsActions[@event] = PooledObjectListPop(action); }
			// all
			if (instance.allEventsActions.TryGetValue(@event, out actions)) { actions.Add(action); }
			else { instance.allEventsActions[@event] = PooledObjectListPop(action); }
		}

		public static void Unregister<T1, T2, T3>(object channel, Event<T1, T2, T3> @event, System.Action<T1, T2, T3> action) {
			if (instance == null) { return; }
			Dictionary<Event, List<object>> eventsActions;
			List<object> actions;
			// channel-specific
			if (instance.channelsEventsActions.TryGetValue(channel, out eventsActions)) {
				if (eventsActions.TryGetValue(@event, out actions)) {
					actions.Remove(action);
					if (actions.Count == 0) {
						PooledObjectListPush(actions);
						eventsActions.Remove(@event);
						if (eventsActions.Count == 0) {
							PooledEventDictPush(eventsActions);
							instance.channelsEventsActions.Remove(channel);
						}
					}
				}
			}
			// all
			if (instance.allEventsActions.TryGetValue(@event, out actions)) {
				actions.Remove(action);
				if (actions.Count == 0) {
					PooledObjectListPush(actions);
					instance.allEventsActions.Remove(@event);
				}
			}
		}

		public static void Broadcast<T1, T2, T3>(object channel, Event<T1, T2, T3> @event, T1 value1, T2 value2, T3 value3) {
			if (instance == null) { return; }
			Dictionary<Event, List<object>> eventsActions;
			List<object> actions;
			// channel-specific only
			if (instance.channelsEventsActions.TryGetValue(channel, out eventsActions)) {
				if (eventsActions.TryGetValue(@event, out actions)) {
					for (int i = actions.Count - 1; i >= 0; --i) {
						((System.Action<T1, T2, T3>)actions[i])(value1, value2, value3);
					}
				}
			}
		}

		public static void BroadcastAll<T1, T2, T3>(Event<T1, T2, T3> @event, T1 value1, T2 value2, T3 value3) {
			if (instance == null) { return; }
			List<object> actions;
			// all only
			if (instance.allEventsActions.TryGetValue(@event, out actions)) {
				for (int i = actions.Count - 1; i >= 0; --i) {
					((System.Action<T1, T2, T3>)actions[i])(value1, value2, value3);
				}
			}
		}

		//

		public static bool EventExists(Event @event) {
			return instance.allEventsActions.ContainsKey(@event);
		}

		public static bool EventExists(string @event) {
			return instance.allStringsActions.ContainsKey(@event);
		}
	}

}