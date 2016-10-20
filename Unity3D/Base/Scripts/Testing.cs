using UnityEngine;
using System.Collections;

namespace RatKing.Base {

	public class Testing : MonoBehaviour {
		public string screenshotPrefix = "screen";
		public float screenshotTakeRate = 0.25f;
		bool takingScreenshots;

		//

		void Update() {
			if (screenshotTakeRate > 0f && Input.GetKeyDown(KeyCode.F11)) {
				if (!takingScreenshots) {
					StartCoroutine("ScreenshotTakerCR");
				}
				else {
					StopCoroutine("ScreenshotTakerCR");
				}
				takingScreenshots = !takingScreenshots;
			}
			if (Input.GetKeyDown(KeyCode.F12)) {
				Base.Helpers.Debug.CreateScreenshot(screenshotPrefix);
			}
			if (Input.GetKeyDown(KeyCode.R)) {
				 UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
			}
		}

		IEnumerator ScreenshotTakerCR() {
			var wait = new WaitForSeconds(screenshotTakeRate);
			while (true) {
				Helpers.Debug.CreateScreenshot(screenshotPrefix, "screenshots/series");
				yield return wait;
			}
		}
	}
	
}