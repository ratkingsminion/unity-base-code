using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base {

	public class QuickIMGUI : MonoBehaviour {

		static QuickIMGUI inst;
		static event System.Action OnDraw;
		//
		public static Texture2D texWhite;
		public static Texture2D texWhiteTransparent;

		//

		static void CreateInstance() {
			if (inst != null) { return; }
			var go = new GameObject("<Quick IM GUI>");
			inst = go.AddComponent<QuickIMGUI>();
			//
			texWhite = new Texture2D(1, 1); texWhite.SetPixel(0, 0, Color.white);
			texWhite.Apply();
			texWhiteTransparent = new Texture2D(1, 1); texWhiteTransparent.SetPixel(0, 0, new Color(1f, 1f, 1f, 0.5f));
			texWhiteTransparent.Apply();
		}

		public static void Add(System.Action onDraw) {
			if (onDraw == null) { return; }
			if (inst == null) { CreateInstance(); }
			if (OnDraw == null) { inst.gameObject.SetActive(true); }
			OnDraw += onDraw;
		}

		public static void Remove(System.Action onDraw) {
			if (onDraw == null) { return; }
			OnDraw -= onDraw;
			if (OnDraw == null) { inst.gameObject.SetActive(false); }
		}

		//

		void OnGUI() {
			OnDraw();
		}
	}

}