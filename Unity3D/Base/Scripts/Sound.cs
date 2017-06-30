using UnityEngine;
using System.Collections;

namespace RatKing.Base {

	[RequireComponent(typeof(AudioSource))]
	[RequireComponent(typeof(Poolable))]
	public class Sound : MonoBehaviour {
		public static float mainVolume = 1f; // TODO should be in a manager
#if UNITY_EDITOR
		public static Transform parent;
#endif
		//
		public AudioClip[] clips;
		public float randomVolume = 0.1f;
		public float randomPitch = 0.1f;
		public float waitSeconds = 0f;
		//
		float originalVolume;
		float originalPitch;
		bool fresh = true;
		float time = 0f;
		//
		Sound original;

		// prefab

		public void Prepare(int poolCount = -1) {
			GetComponent<Poolable>().PoolPrepare(poolCount);
			time = 0f;
		}

		public Sound Play(Transform start) {
			if (original != null) { Debug.Log("Warning: Use Prefab to Play()!"); }
#if UNITY_EDITOR
			if (Time.time < time && Time.time >= (time - waitSeconds)) { return null; }
#else
			if (Time.time < time) { return null; }
#endif
			var p = GetComponent<Poolable>().PoolPop(start.position, start.rotation);
#if UNITY_EDITOR
			if (parent == null) { parent = new GameObject("<SOUNDS>").transform; } p.transform.SetParent(parent, true);
#endif
			var s = p.GetComponent<Sound>();
			s.original = this;
			s.Playing();
			time = Time.time + waitSeconds;
			return s;
		}

		public Sound Play(Vector3 position, Quaternion rotation) {
			if (original != null) { Debug.Log("Warning: Use Prefab to Play()!"); }
#if UNITY_EDITOR
			if (Time.time < time && Time.time >= (time - waitSeconds)) { return null; }
#else
			if (Time.time < time) { return null; }
#endif
			var p = GetComponent<Poolable>().PoolPop(position, rotation);
#if UNITY_EDITOR
			if (parent == null) { parent = new GameObject("<SOUNDS>").transform; } p.transform.SetParent(parent, true);
#endif
			var s = p.GetComponent<Sound>();
			s.original = this;
			s.Playing();
			time = Time.time + waitSeconds;
			return s;
		}

		public Sound Play(Vector3 position) {
			if (original != null) { Debug.Log("Warning: Use Prefab to Play()!"); }
#if UNITY_EDITOR
			if (Time.time < time && Time.time >= (time - waitSeconds)) { return null; }
#else
			if (Time.time < time) { return null; }
#endif
			var p = GetComponent<Poolable>().PoolPop(position);
#if UNITY_EDITOR
			if (parent == null) { parent = new GameObject("<SOUNDS>").transform; } p.transform.SetParent(parent, true);
#endif
			var s = p.GetComponent<Sound>();
			s.original = this;
			s.Playing();
			time = Time.time + waitSeconds;
			return s;
		}

		public Sound Play() {
			if (original != null) { Debug.Log("Warning: Use Prefab to Play()!"); }
#if UNITY_EDITOR
			if (Time.time < time && Time.time >= (time - waitSeconds)) { return null; }
#else
			if (Time.time < time) { return null; }
#endif
			var p = GetComponent<Poolable>().PoolPop();
#if UNITY_EDITOR
			if (parent == null) { parent = new GameObject("<SOUNDS>").transform; } p.transform.SetParent(parent, true);
#endif
			var s = p.GetComponent<Sound>();
			s.original = this;
			s.Playing();
			time = Time.time + waitSeconds;
			return s;
		}

		public void Stop() {
			var source = GetComponent<AudioSource>();
			if (source.isPlaying) {
				GetComponent<Poolable>().PoolPush();
			}
		}

		// internal, non-prefab

		void Playing() {
			var source = GetComponent<AudioSource>();
			source.playOnAwake = false;
			if (fresh) {
				originalVolume = source.volume;
				originalPitch = source.pitch;
				fresh = false;
			}
			StartCoroutine(PlayingCR(source));
		}

		IEnumerator PlayingCR(AudioSource source) {
			AudioClip clip = clips[Random.Range(0, clips.Length)];
			source.volume = originalVolume * (1.0f + Random.Range(-randomVolume, randomVolume) * 0.5f) * mainVolume;
			source.pitch = originalPitch * (1.0f + Random.Range(-randomPitch, randomPitch) * 0.5f);
			if (source.loop) {
				source.clip = clip;
				source.Play();
			}
			else {
				source.PlayOneShot(clip);
				yield return new WaitForSeconds(clip.length * source.pitch);
				GetComponent<Poolable>().PoolPush();
			}
		}

		//

		public bool Looping() {
			return GetComponent<AudioSource>().loop;
		}

		public void MultiplyVolume(float v) {
			if (fresh) { Debug.Log("Warning: Don't manipulate volume of original!"); }
			var source = GetComponent<AudioSource>();
			source.volume = v * originalVolume * (1.0f + Random.Range(-randomVolume, randomVolume) * 0.5f) * mainVolume;
		}
	}

}