using UnityEngine;
using UnityEditor;

namespace RatKing.Base {
	
	public class EditorHelpers {

		public static GUIStyle headerStyle;

		public static GUIStyle GetHeaderStyle() {
			if (headerStyle == null) {
				headerStyle = new GUIStyle(GUI.skin.GetStyle("label")) { richText = true };
			}
			return headerStyle;
		}

		/// <summary>
		/// Text area where you can write stuff. It saves the value
		/// </summary>
		public static void TextArea(string label, ref string value, string saveInEditorPrevs = null, string saveInPlayerPrevs = null) {
			GUILayout.BeginHorizontal();
			GUILayout.Label(label, GUILayout.Width(90), GUILayout.ExpandWidth(false));
			var newValue = EditorGUILayout.TextField(string.IsNullOrEmpty(value) ? "" : value, GUILayout.Width(10), GUILayout.ExpandWidth(true));
			GUILayout.EndHorizontal();
			if (newValue.Trim() != value) {
				value = newValue.Trim();
				if (!string.IsNullOrEmpty(saveInEditorPrevs)) { EditorPrefs.SetString(saveInEditorPrevs, value); }
				if (!string.IsNullOrEmpty(saveInPlayerPrevs)) { PlayerPrefs.SetString(saveInPlayerPrevs, value); }
			}
		}
	}

}