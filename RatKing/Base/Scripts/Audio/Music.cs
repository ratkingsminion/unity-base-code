using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base {

	public class Music : MonoBehaviour {

		class MusicSource {
			public AudioSource source;
			public float targetVolume;
			public float fadeSpeed;
			public float curVolume;
			public bool unused;
			public int priority;
		}

		public static float globalVolume = 1f;

		static GameObject go;
		static readonly Dictionary<string, List<MusicSource>> sources = new Dictionary<string, List<MusicSource>>();
		static Music Inst;

		//

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		static void OnRuntimeInitializeOnLoad() {
			Inst = null;
			globalVolume = 1f;
			sources.Clear();
		}

		//

		static void Init() {
			if (Inst != null) { return; }
			go = new GameObject("<MUSIC>");
			DontDestroyOnLoad(go);
			Inst = go.AddComponent<Music>();
		}

		void Update() {
			for (var iter = sources.GetEnumerator(); iter.MoveNext();) {
				var curSources = iter.Current.Value;

				// check priority first, remove sources
				var priority = -1;
				var mainIndex = -1;
				for (int i = curSources.Count - 1; i >= 0; --i) {
					var s = curSources[i];
					if (s.source == null) { curSources.RemoveAt(i); continue; }
					if (s.unused || s.targetVolume <= 0f) { continue; }
					if (s.priority <= priority) { continue; }
					priority = s.priority;
					mainIndex = i; 
				}

				for (int i = curSources.Count - 1; i >= 0; --i) {
					var s = curSources[i];
					if (s.unused) { continue; }
					var targetVolume = (i == mainIndex) ? s.targetVolume : 0f;
					if (!Mathf.Approximately(s.curVolume, targetVolume)) {
						s.curVolume = Mathf.MoveTowards(s.curVolume, targetVolume, s.fadeSpeed * Time.unscaledDeltaTime);
						s.source.volume = s.curVolume * globalVolume;
						if (s.curVolume <= 0f && i == mainIndex) { s.unused = true; s.source.Stop(); }
					}
					else {
						s.source.volume = targetVolume * globalVolume;
					}
				}
			}
		}

		static MusicSource CreateSource(string layer = "") {
			if (Inst == null) { Init(); }
			var s = new MusicSource();
			var ss = s.source = go.AddComponent<AudioSource>();
			ss.spatialBlend = 0f;
			ss.volume = s.targetVolume = s.curVolume = 0f;
			ss.playOnAwake = false;
			ss.loop = true;
			s.unused = false;
			if (!sources.ContainsKey(layer)) { sources[layer] = new List<MusicSource>() { s }; }
			else { sources[layer].Add(s); }
			return s;
		}

		static void SetTargetVolume(MusicSource source, float targetVolume, float fadeTime) {
			source.targetVolume = targetVolume;
			var t = Mathf.Abs(source.curVolume - targetVolume);
            source.fadeSpeed = t / fadeTime;
			//Debug.Log("--> " + source.source.clip + " to " + targetVolume + " over " + fadeTime + " seconds -> speed = " + source.fadeSpeed);
			Inst.enabled = true;
		}

		public static void Stop(float fadeTime = 5f, int priority = -1, string layer = null) {
			//Debug.Log("STOP MUSIC");
			if (Inst == null) { return; }
			if (layer == null) {
				for (var iter = sources.GetEnumerator(); iter.MoveNext();) {
					var curSources = iter.Current.Value;
					if (fadeTime > 0f) { foreach (var s in curSources) { if (!s.unused && (priority == -1 || s.priority == priority)) { SetTargetVolume(s, 0f, fadeTime); } } }
					else { foreach (var s in curSources) { if (priority == -1 || s.priority == priority) { s.unused = true; s.source.volume = 0f; s.source.Stop(); } } }
				}
			}
			else {
				if (!sources.ContainsKey(layer)) { return; }
				var curSources = sources[layer];
				if (fadeTime > 0f) { foreach (var s in curSources) { if (!s.unused && (priority == -1 || s.priority == priority)) { SetTargetVolume(s, 0f, fadeTime); } } }
				else { foreach (var s in curSources) { if (priority == -1 || s.priority == priority) { s.unused = true; s.source.volume = 0f; s.source.Stop(); } } }
			}
		}

		public static void Play(AudioClip clip, float volume = 0.3f, float fadeTime = 5f, int priority = 0, string layer = "") {
			if (layer == null) { return; }
			if (Inst == null) { if (clip == null || volume <= 0f) { return; } Init(); }
			if (clip == null) { Stop(fadeTime, priority, layer); return; }

			// already playing?
			List<MusicSource> curSources = null;
			if (sources.ContainsKey(layer)) { curSources = sources[layer]; }
			if (curSources != null) {
				foreach (var s in curSources) {
					if (!s.unused && s.source.clip == clip) {
						Stop(fadeTime, priority, layer); // stop all others
						SetTargetVolume(s, volume, fadeTime);
						s.priority = priority;
						return;
					}
				}
			}

			// new playing
			MusicSource useSource = null;
			if (curSources != null) {
				foreach (var s in curSources) {
					if (s.unused) { if (useSource == null) { useSource = s; } }
					// else { SetTargetVolume(s, 0f, fadeTime); }
				}
			}
			if (useSource == null) { useSource = CreateSource(layer); }
			useSource.source.clip = clip;
			SetTargetVolume(useSource, volume, fadeTime);
			useSource.unused = false;
			useSource.priority = priority;
			useSource.source.Play();
		}
	}

}