using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base {

	public class Debugging : MonoBehaviour {

		static void CreateInstance() {
			if (inst != null)
				return;
			var go = new GameObject("<BaseHelpersDebug>");
		}

		public static void CreateScreenshot(string prefix, string folder = "screenshots") {
			CreateInstance();
			System.DateTime t = System.DateTime.Now;

			string time = t.Year + ""
				+ (t.Month < 10 ? "0" : "") + t.Month + ""
				+ (t.Day < 10 ? "0" : "") + t.Day + "_"
				+ (t.Hour < 10 ? "0" : "") + t.Hour + ""
				+ (t.Minute < 10 ? "0" : "") + t.Minute + ""
				+ (t.Second < 10 ? "0" : "") + t.Second + "_"
				+ (t.Millisecond < 100 ? "0" : "") + (t.Millisecond < 10 ? "0" : "") + t.Millisecond;

#if UNITY_WEBPLAYER
#if UNITY_2017_1_OR_NEWER
			ScreenCapture.CaptureScreenshot(folder + "/" + prefix + "_" + time + ".png");
#else
			Application.CaptureScreenshot(folder + "/" + prefix + "_" + time + ".png");
#endif
#else

#if UNITY_EDITOR
			if (!System.IO.Directory.Exists(Application.dataPath + "/../../" + folder)) {
				System.IO.Directory.CreateDirectory(Application.dataPath + "/../../" + folder);
			}
			var path = "Assets/../../" + folder + "/" + prefix + "_" + time;
#if UNITY_2017_1_OR_NEWER
			ScreenCapture.CaptureScreenshot(path + ".png");
#else
			Application.CaptureScreenshot(path + ".png");
#endif
			inst.StartCoroutine(inst.CreateScreenshotCR(path));
#else
			string p = Application.platform == RuntimePlatform.OSXPlayer ? "/../../" : "/../";
			if (!System.IO.Directory.Exists(Application.dataPath + p + folder))
				System.IO.Directory.CreateDirectory(Application.dataPath + "/../" + folder);
				
			var path = Application.dataPath + p + folder + "/" + prefix + "_" + time;
#if UNITY_2017_1_OR_NEWER
			ScreenCapture.CaptureScreenshot(path + ".png");
#else
			Application.CaptureScreenshot(path + ".png");
#endif
			inst.StartCoroutine(inst.CreateScreenshotCR(path));
#endif
		}
		IEnumerator CreateScreenshotCR(string path, int quality = 99) {
			bool error = true;
			float wait = Time.time + 0.5f;
			while (wait > Time.time) {
				yield return null; // wait for png actually existing
				if (System.IO.File.Exists(path + ".png")) {
					error = false;
					break;
				}
			}
			if (!error) {
				var texture = new Texture2D(2, 2, TextureFormat.RGB24, false);
				var data = System.IO.File.ReadAllBytes(path + ".png");
				texture.LoadImage(data);
				System.IO.File.WriteAllBytes(path + ".jpg", texture.EncodeToJPG(quality));
				System.IO.File.Delete(path + ".png");
				UnityEngine.Debug.Log("Screenshot: " + path + ".jpg");
			}
			else {
				UnityEngine.Debug.Log("Error creating screenshot! " + path);
				yield break;
			}
		}
#endif
	}

}
