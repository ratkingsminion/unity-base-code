using UnityEngine;

namespace RatKing.Base {
	
	public class Sound : MonoBehaviour {
		public SoundType Type { get; private set; }
		public bool IsPaused { get; private set; }
		public bool InUse { get; private set; }
		public string Tag { get; set; }
		public AudioSource Source { get { return source; } }
		
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

		public static bool TryPlay(out Sound sound, SoundType type, Transform start, string tag = null) {
			sound = type != null ? type.Play(start, tag) : null;
			return sound != null;
		}

		public static bool TryPlay(out Sound sound, SoundType type, Vector3 position, string tag = null) {
			sound = type != null ? type.Play(position, tag) : null;
			return sound != null;
		}

		public static bool TryPlay(out Sound sound, SoundType type, string tag = null) {
			sound = type != null ? type.Play(tag) : null;
			return sound != null;
		}

		//

		public void PlayType(SoundType type, int clipIndex = -1, Vector3 pos = default) {
			Tag = null;
			InUse = true;
			IsPaused = false;
			Type = type;
			var src = source = GetComponent<AudioSource>();
			src.transform.position = pos;
			if (clipIndex >= 0) { type.CurPlayIndex = clipIndex % Type.Clips.Length; }
			else if (type.Clips.Length == 1) { type.CurPlayIndex = 0; }
			else if (type.RandomOrder) { type.CurPlayIndex = (type.CurPlayIndex < 0) ? Random.Range(0, type.Clips.Length) : (type.CurPlayIndex + Random.Range(0, type.Clips.Length - 1)) % type.Clips.Length; }
			else { type.CurPlayIndex = (type.CurPlayIndex + 1) % type.Clips.Length; }
			src.clip = type.Clips[type.CurPlayIndex];
			src.volume = (volume = type.Volume.Random()) * Sounds.GlobalVolume;
			src.pitch = type.Pitch.Random();
			src.outputAudioMixerGroup = type.MixerGroup;
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