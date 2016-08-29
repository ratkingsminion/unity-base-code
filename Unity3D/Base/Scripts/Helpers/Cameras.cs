using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base.Helpers {

	public static class Cameras {
		public static float GetWideFOV(this Camera cam) {
			return GetWideFOV(cam.fieldOfView, Screen.width * cam.rect.width, Screen.height * cam.rect.height);
		}
		public static float GetWideFOV(float fov, float w = -1f, float h = -1f) {
			if (w < 0f) w = Screen.width;
			if (h < 0f) h = Screen.height;
			return 2f * Mathf.Atan(w * Mathf.Tan(fov * Mathf.Deg2Rad * 0.5f) / h) * Mathf.Rad2Deg;
		}
	}

}