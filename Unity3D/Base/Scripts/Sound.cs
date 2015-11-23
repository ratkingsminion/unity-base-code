using UnityEngine;
using System.Collections;

namespace RatKing.Base {

	[RequireComponent(typeof(AudioSource))]
	[RequireComponent(typeof(Poolable))]
	public class Sound : MonoBehaviour {
		public static float mainVolume = 1f; // TODO should be in a manager
		//
		public AudioClip[] clips;
		public float randomVolume = 0.1f;
		public float randomPitch = 0.1f;
		//
		float originalVolume;
		float originalPitch;
		bool fresh = true;
		
		//

		public void Prepare(int poolCount = -1) {
			GetComponent<Poolable>().PoolPrepare(poolCount);
		}

		public Sound Play(Transform start) {
			var p = GetComponent<Poolable>().PoolPop(start.position, start.rotation);
			var s = p.GetComponent<Sound>();
			s.Playing();
			return s;
		}

		public Sound Play(Vector3 position, Quaternion rotation) {
			var p = GetComponent<Poolable>().PoolPop(position, rotation);
			var s = p.GetComponent<Sound>();
			s.Playing();
			return s;
		}

		public Sound Play(Vector3 position) {
			var p = GetComponent<Poolable>().PoolPop(position);
			var s = p.GetComponent<Sound>();
			s.Playing();
			return s;
		}

		public Sound Play() {
			var p = GetComponent<Poolable>().PoolPop();
			var s = p.GetComponent<Sound>();
			s.Playing();
			return s;
		}

		public void Stop() {
			var source = GetComponent<AudioSource>();
			if (source.isPlaying) {
				GetComponent<Poolable>().PoolPush();
			}
		}

		//

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
	}

}