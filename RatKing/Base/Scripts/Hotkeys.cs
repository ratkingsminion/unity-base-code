using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RatKing.Base {

	public class Hotkeys {

		public class Entry {
			Hotkeys parent;
			public string keyCode; // shortcut
			public bool shift, ctrl, alt; // shortcut modifiers
			public System.Action Action { get; private set; } = null;
			public int Priority => (shift ? 1 : 0) + (ctrl ? 2 : 0) + (alt ? 4 : 0);

			//

			public Entry(string keyCode, System.Action action, Hotkeys parent = null) {
				this.parent = parent;
				Set(keyCode, action);
			}

			public void Enable() {
				if (parent == null) { return; }
				if (!parent.entries.Contains(this)) { parent.entries.Add(this); parent.entries.Sort((a, b) => b.Priority - a.Priority); }
			}

			public void Disable() => parent?.entries.Remove(this);

			public string GetKeyCodeText() {
				return (shift ? "shift " : "") + (ctrl ? "ctrl " : "") + (alt ? "alt " : "") + keyCode;
			}

			public void Set(string keyCode, System.Action action) {
				this.Action = action;
				GetCode(ref keyCode, out this.keyCode, out this.shift, out this.ctrl, out this.alt);
				parent?.entries.Sort((a, b) => b.Priority - a.Priority);
			}

			public void Set(string keyCode) {
				GetCode(ref keyCode, out this.keyCode, out shift, out ctrl, out alt);
				parent?.entries.Sort((a, b) => b.Priority - a.Priority);
			}

			//

			public bool CheckInputRigid() {
				if (string.IsNullOrWhiteSpace(keyCode)) { return false; }
				if (shift != (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))) { return false; }
				if (ctrl != (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))) { return false; }
				if (alt != (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))) { return false; }
				return Input.GetKeyDown(keyCode);
			}

			public bool CheckInput() {
				if (string.IsNullOrWhiteSpace(keyCode)) { return false; }
				if (shift && !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))) { return false; }
				if (ctrl && !(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))) { return false; }
				if (alt && !(Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))) { return false; }
				return Input.GetKeyDown(keyCode);
			}

			//

			void GetCode(ref string whole, out string code, out bool shift, out bool ctrl, out bool alt) {
				whole = (whole ?? "").Trim().ToLower();
				shift = ctrl = alt = false;
				var parts = whole.Split(Base.StringExtras.spaceSplitter, System.StringSplitOptions.RemoveEmptyEntries);
				code = "";
				foreach (var p in parts) {
					var c = p.Trim();
					if (c == "shift") { shift = true; }
					else if (c == "ctrl") { ctrl = true; }
					else if (c == "alt") { alt = true; }
					else if (code == "" && !c.Contains("shift") && !c.Contains("ctrl") && !c.Contains("alt")) {
						code = c;
						try { Input.GetKey(c); }
						catch { code = ""; }
					}
				}
				if (string.IsNullOrWhiteSpace(code)) { shift = ctrl = alt = false; }
			}
		}

		//

		readonly List<Entry> entries = new();

		//

		public void CheckAndExecute() {
			for (int i = 0, c = entries.Count; i < c; ++i) {
				if (entries[i].CheckInput()) { entries[i].Action?.Invoke(); break; }
			}
		}

		public Entry Add(string keyCode, System.Action action) {
			var entry = new Entry(keyCode, action, this);
			entries.Add(entry);
			return entry;
		}
	}

}