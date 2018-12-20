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

		public void PlayType(SoundType type, int clipIndex = -1) {
			Tag = null;
			InUse = true;
			IsPaused = false;
			Type = type;
			var src = source = GetComponent<AudioSource>();
			src.clip = clipIndex < 0 ? DataStructures.GetRandomElement(type.Clips) : type.Clips[clipIndex];
			src.volume = (volume = type.Volume.Random()) * Sounds.GlobalVolume;
			src.pitch = type.Pitch.Random();
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
			source.time = Mathf.Repeat(t / source.clip.length, 1f);
			return true;
		}
	}

}