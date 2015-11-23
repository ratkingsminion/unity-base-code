using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base {

	public class Music : MonoBehaviour {
		public static float globalVolume = 1f;
		//
		static AudioSource[] sources = new AudioSource[2];
		static float[] fadeTargetVolumes = new float[2];
		static float[] fadeSpeeds = new float[2];
		static float[] curVolumes = new float[2];
		static Music myself;
		static int activeIndex = 1;
		static int otherIndex = 0;
		static bool tweening;

		//

		static void Init() {
			if (myself != null)
				return;
			var go = new GameObject("<BaseMusic>");
			DontDestroyOnLoad(go);
			myself = go.AddComponent<Music>();
			for (int i = 0; i < 2; ++i) {
				sources[i] = go.AddComponent<AudioSource>();
#if !UNITY_5
				source[i].panLevel = 0f;
#else
				sources[i].spatialBlend = 0f;
#endif
				sources[i].volume = fadeTargetVolumes[i] = curVolumes[i] = 0f;
				sources[i].playOnAwake = false;
				sources[i].loop = true;
			}
		}

		void Update() {

			for (int i = 0; i < 2; ++i) {
                if (sources[i].isPlaying && !Mathf.Approximately(curVolumes[i], fadeTargetVolumes[i])) {

					curVolumes[i] = Mathf.MoveTowards(curVolumes[i], fadeTargetVolumes[i], fadeSpeeds[i] * Time.deltaTime);
					sources[i].volume = curVolumes[i] * globalVolume;
                    if (curVolumes[i] <= 0f) {

						sources[i].Stop();
					}

				}
				else {
					sources[i].volume = fadeTargetVolumes[i] * globalVolume;
                }
			}


		}

		static void SetTargetVolume(int index, float targetVolume, float fadeTime) {
			fadeTargetVolumes[index] = targetVolume;
			var t = Mathf.Abs(curVolumes[index] - targetVolume);
            fadeSpeeds[index] = t / fadeTime;
			// print(index + ") to " + targetVolume + " over " + fadeTime + " seconds -> speed = " + fadeSpeeds[index]);
			myself.enabled = true;
		}

		public static void Play(AudioClip clip, float volume = 0.3f, float fadeTime = 10f) {
			if (myself == null) {
				Init();
			}

			if (clip == null) {
				Stop(fadeTime);

				return;
			}

			if (sources[activeIndex].clip == clip && sources[activeIndex].isPlaying) {
				if (!Mathf.Approximately(sources[activeIndex].volume, volume)) {
					SetTargetVolume(activeIndex, volume, fadeTime);
				}
				return;
			}

			bool restart = true;
			if (!Mathf.Approximately(sources[otherIndex].volume, fadeTargetVolumes[otherIndex]) && sources[otherIndex].clip == clip) {
				restart = false;
			}
			
			activeIndex = otherIndex;
			otherIndex = 1 - otherIndex;
			
			sources[activeIndex].clip = clip;
			if (restart) {
				sources[activeIndex].Play();
			}

			if (fadeTime <= 0f) {
				sources[activeIndex].volume = volume;
				sources[otherIndex].volume = 0f;
				sources[otherIndex].Stop();
			}
			else {
				SetTargetVolume(activeIndex, volume, fadeTime);
				SetTargetVolume(otherIndex, 0f, fadeTime);
			}
		}

		public static void Stop(float fadeTime = 5f) {
			if (myself == null) {
				return;
			}
			if (fadeTime > 0f) {
				SetTargetVolume(0, 0f, fadeTime);
				SetTargetVolume(1, 0f, fadeTime);
			}
			else {
				sources[0].volume = 0f;
				sources[0].Stop();
				sources[1].volume = 0f;
				sources[1].Stop();
			}
		}
	}

}