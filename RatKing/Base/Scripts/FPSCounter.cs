using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace RatKing.Base {

	public class FPSCounter : MonoBehaviour {
		[SerializeField] TMPro.TextMeshProUGUI uiText = null;
		[SerializeField] float updateTime = 0.5f;
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
				uiText.text = $"{(ms*1000):0.0}ms\n{fps:0.0}";
				i = 0;
				fps = ms = 0f;
			}
		}
	}

}