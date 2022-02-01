using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base {

	public class LocaliseUIText : MonoBehaviour, ILocaliseMe {
		[SerializeField] string key = "";
		//
		UnityEngine.UI.Text[] ui;
		TMPro.TextMeshProUGUI[] tmp;
		TextWithBack[] twb;
		bool registered;

		//

		void Awake() {
			ui = GetComponentsInChildren<UnityEngine.UI.Text>();
			twb = GetComponentsInChildren<TextWithBack>();
			tmp = GetComponentsInChildren<TMPro.TextMeshProUGUI>();
			var l = new List<TMPro.TextMeshProUGUI>(tmp);
			if (twb.Length > 0) {
				for (int i = l.Count - 1; i >= 0; --i) { foreach (var t in twb) { if (l[i] == t.Text) { l.RemoveAt(i); break; } } }
				tmp = l.ToArray();
			}
			if (ui.Length == 0 && tmp.Length == 0 && twb.Length == 0) { Debug.LogWarning("trying to localise empty ui [" + key + "]"); Destroy(this); return; }
			Localisation.Register(this);
			registered = true;
		}

		void OnDestroy() {
			if (registered) { Localisation.Unregister(this); }
		}

		// implement ILocaliseMe

		public void TranslateMe() {
			// todo: check if already correct language?
			var text = Localisation.Do(key);
			text = text.Replace("\\n", "\n");
			foreach (var t in twb) { t.SetText(text); }
			foreach (var u in ui) { u.text = text; }
			foreach (var t in tmp) { t.text = text; }
		}
	}

}