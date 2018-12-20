using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base {
	
	[CreateAssetMenu(fileName = "SND", menuName = "Rat King/New Sound Type")]
	public class SoundType : ScriptableObject {
		[SerializeField] AudioClip[] clips = null;
		[SerializeField] RangeFloat volume = new RangeFloat(0.5f, 0.6f);
		[SerializeField] RangeFloat pitch = new RangeFloat(1f, 1f);
		[SerializeField, Range(0f, 1f)] float priority = 0.5f;
		[SerializeField] RangeFloat waitSeconds = new RangeFloat(0f, 0f);
		[SerializeField] bool loop = false;
		[SerializeField, Range(0f, 1f)] float spatialBlend = 0f;
		[Header("Pooling")]
		[SerializeField] int startCount = 3;
		[SerializeField] int addCount = 1;

		public AudioClip[] Clips { get { return clips; } }
		public RangeFloat Volume { get { return volume; } }
		public RangeFloat Pitch { get { return pitch; } }
		public float Priority { get { return priority; } }
		public RangeFloat WaitSeconds { get { return waitSeconds; } }
		public bool Loop { get { return loop; } }
		public float SpatialBlend { get { return spatialBlend; } }
		public int PoolStartCount { get { return startCount; } }
		public int PoolAddCount { get { return addCount; } }
		
		//

		public Sound Play(Transform start, string tag = null) {
			var sound = Sounds.GetOrCreateInstance().Play(this);
			if (sound == null) { return null; }
			sound.transform.position = start.position;
			sound.transform.rotation = start.rotation;
			sound.Tag = tag;
			return sound;
		}

		public Sound Play(Vector3 position, string tag = null) {
			var sound = Sounds.GetOrCreateInstance().Play(this);
			if (sound == null) { return null; }
			sound.transform.position = position;
			sound.Tag = tag;
			return sound;
		}

		public Sound Play(Vector3 position, Quaternion rotation, string tag = null) {
			var sound = Sounds.GetOrCreateInstance().Play(this);
			if (sound == null) { return null; }
			sound.transform.position = position;
			sound.transform.rotation = rotation;
			sound.Tag = tag;
			return sound;
		}

		public Sound Play(string tag = null) {
			var sound = Sounds.GetOrCreateInstance().Play(this);
			if (sound == null) { return null; }
			sound.Tag = tag;
			return sound;
		}
	}

}