using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base.Helpers {

	public static class Sound {
		// source: https://twitter.com/i/web/status/785798323038937088
		public static float VolumeToDB(float volume) {
			if (volume <= 0f) { return -80f; }
			return 20f * Mathf.Log10(volume);
		}
		public static float DBToVolume(float dB) {
			return Mathf.Pow(10f, dB * 0.05f);
		}
	}

}