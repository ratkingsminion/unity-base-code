using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base {

	public class Pause : MonoBehaviour {

		public static readonly Event<bool> EVENT = new Event<bool>("pause_event");
		static Pause inst = null;

		class Pausing {
			public float timeScale;
			public float fadeTo = -1f;
			public float fadeSpeed = -1f;
			public System.Action onFadeDone = null;
			public Pausing(float factor) { this.timeScale = factor; }
		}

		List<string> layers = new List<string>();
		List<Pausing> pausings = new List<Pausing>();

		//
			
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		static void OnRuntimeInitializeOnLoad() {
			inst = null;
			Base.Events.UnregisterAll(EVENT);
		}

		//

		void Update() {
			var target = 1f;

			// find lowest one
			for (int i = pausings.Count - 1; i>= 0; --i) {
				var pausing = pausings[i];
				if (pausing.fadeTo >= 0f) {
					pausing.timeScale = Mathf.MoveTowards(pausing.timeScale, pausing.fadeTo, pausing.fadeSpeed * Time.unscaledDeltaTime);
					if (pausing.timeScale == pausing.fadeTo) {
						if (pausing.onFadeDone != null) { pausing.onFadeDone(); pausing.onFadeDone = null; }
						if (pausing.fadeTo == 1f) { layers.RemoveAt(i); pausings.RemoveAt(i); continue; }
						pausing.fadeTo = pausing.fadeSpeed = -1f;
					}
				}
				if (pausing.timeScale < target) { target = pausing.timeScale; }
			}

			var nextTimeScale = Mathf.MoveTowards(Time.timeScale, target, 1f);
			if (Time.timeScale > 0f && nextTimeScale <= 0f) {
				Base.Events.BroadcastAll(EVENT, true);
			}
			else if (Time.timeScale <= 0f && nextTimeScale > 0f) {
				Base.Events.BroadcastAll(EVENT, false);
			}
			Time.timeScale = nextTimeScale;
		}

		void OnDestroy() {
			Time.timeScale = 1f;
		}

		//

		static void CreateInstance() {
			if (inst != null) { return; }
			inst = new GameObject("<PAUSE>").AddComponent<Pause>();
			DontDestroyOnLoad(inst.gameObject);
		}

		//

		public static bool Is() {
			return Time.timeScale <= 0f;
		}

		public static void Do(string layer, float seconds = 0f, System.Action onDone = null) {
			CreateInstance();
			//
			var idx = inst.layers.IndexOf(layer);
			if (idx < 0) {
				idx = inst.layers.Count;
				inst.layers.Add(layer);
				inst.pausings.Add(new Pausing(1f));
			}

			var pausing = inst.pausings[idx];

			if (seconds > 0f) {
				pausing.fadeTo = 0f;
				pausing.fadeSpeed = pausing.timeScale / seconds;
				pausing.onFadeDone = onDone;
			}
			else {
				if (onDone != null) { onDone(); }
				pausing.timeScale = 0f;
				pausing.fadeTo = pausing.fadeSpeed = -1f;
				pausing.onFadeDone = null;
			}
		}

		public static void Undo(string layer, float seconds = 0f, System.Action onUndone = null) {
			if (inst == null) { return; }

			var idx = inst.layers.IndexOf(layer);
			if (idx < 0) {
				if (onUndone != null) { onUndone(); }
				return;
			}

			var pausing = inst.pausings[idx];

			if (seconds > 0f) {
				pausing.fadeTo = 1f;
				pausing.fadeSpeed = (1f - pausing.timeScale) / seconds;
				pausing.onFadeDone = onUndone;
			}
			else {
				if (onUndone != null) { onUndone(); }
				inst.layers.RemoveAt(idx);
				inst.pausings.RemoveAt(idx);
			}
		}
	}

}