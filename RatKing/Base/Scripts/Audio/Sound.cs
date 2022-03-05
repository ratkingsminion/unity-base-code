using UnityEngine;

namespace RatKing.Base {
	
	public class Sound : MonoBehaviour {
		public SoundType Type { get; private set; }
		public bool IsPaused { get; private set; }
		public bool InUse { get; private set; }
		public string Tag { get; set; }
		public AudioSource Source { get { return source; } }
		//
		AudioSource source;

		//

		float volume;
		public float Volume {
			get { return volume; }
			set {
				if (!InUse || source == null) { return; }
				source.volume = (volume = value) * Sounds.GlobalVolume;
			}
		}

		public bool IsPlaying {
			get {
				return InUse && source != null && source.isPlaying;
			}
		}

		//

		public static Sound Play(SoundType type, Transform start, string tag = null) {
			return type != null ? type.Play(start, tag) : null;
		}

		public static Sound Play(SoundType type, Vector3 position, string tag = null) {
			return type != null ? type.Play(position, tag) : null;
		}

		public static Sound Play(SoundType type, string tag = null) {
			return type != null ? type.Play(tag) : null;
		}

		//

		public void PlayType(SoundType type, int clipIndex = -1, Vector3 pos = default) {
			Tag = null;
			InUse = true;
			IsPaused = false;
			Type = type;
			var src = source = GetComponent<AudioSource>();
			src.transform.position = pos;
			src.clip = clipIndex < 0 ? DataStructures.GetRandomElement(type.Clips) : type.Clips[clipIndex];
			src.volume = (volume = type.Volume.Random()) * Sounds.GlobalVolume;
			src.pitch = type.Pitch.Random();
			if (type.SpatialBlend > 0f) {
				src.minDistance = type.Distance3D.min;
				src.maxDistance = type.Distance3D.max;
			}
			src.Play();
		}

		public bool Pause() {
			if (!InUse || IsPaused) { return false; }
			IsPaused = true;
			source.Pause();
			return true;
		}

		public bool Resume() {
			if (!InUse || !IsPaused) { return false; }
			IsPaused = false;
			source.Play();
			return true;
		}

		public bool Stop() {
			if (!InUse) { return false; }
			IsPaused = false;
			source.Stop();
			InUse = false;
			return true;
		}

		public bool Rewind() {
			if (!InUse) { return false; }
			source.time = 0f;
			return true;
		}

		public bool SetNormalizedTime(float t) {
			if (!InUse || source.clip == null) { return false; }
			source.time = Mathf.Repeat(t, 1f) * source.clip.length;
			return true;
		}
	}

}