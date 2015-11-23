using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace RatKing.Base {

	public class FPSCounter : MonoBehaviour {
		public float updateTime = 0.5f;
		//
		int i;
		float ms;
		float fps;
		
		void Update() {
			ms += Time.deltaTime;
			fps += (1f / Time.deltaTime);
			++i;
			if (ms > updateTime) {
				ms /= (float)i;
				fps /= (float)i;
				GetComponent<Text>().text = (Mathf.Round(ms * 10000f) / 10000f) + "ms\n" + (Mathf.Round(fps * 10f) / 10f);
				i = 0;
				fps = ms = 0f;
			}
		}
	}

}