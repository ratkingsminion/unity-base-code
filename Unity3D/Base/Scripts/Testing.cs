using UnityEngine;
using System.Collections;

namespace RatKing.Base {

	public class Testing : MonoBehaviour {
		public string screenshotPrefix = "screen";
		
		//
		
		void Update() {
			if (Input.GetKeyDown(KeyCode.F12)) {
				Base.Helpers.Debug.CreateScreenshot(screenshotPrefix);
			}
			if (Input.GetKeyDown(KeyCode.R)) {
				UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
			}
		}

	}
	
}