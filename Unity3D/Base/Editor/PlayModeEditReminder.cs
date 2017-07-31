using UnityEngine;
using UnityEditor;
[InitializeOnLoad, ExecuteInEditMode] 
public static class PlayModeEditReminder {
	static GUIStyle style;
	static PlayModeEditReminder() {
		SceneView.onSceneGUIDelegate += OnSceneGUI;
	}
	static void OnSceneGUI(SceneView view) {
		if (Application.isPlaying && Selection.activeGameObject != null && Selection.activeGameObject.scene.name != null) {
			Handles.BeginGUI();		
			if (style == null) {
				style = new GUIStyle(GUI.skin.GetStyle("label")) {
					fontSize = 20,
					contentOffset = Vector2.zero,
					overflow = new RectOffset(),
					margin = new RectOffset(0,0,0,40),
					alignment = TextAnchor.LowerCenter,
					fontStyle = FontStyle.Bold,
					wordWrap = true,
					stretchHeight = true
				};
			}
			var c = Mathf.Sin(Time.realtimeSinceStartup * 15f) * 0.5f + 0.5f;
			style.normal.textColor = new Color(1f, c, c, 1f);
			GUILayout.Label("PLAY MODE IS ACTIVE, CHANGES WON'T GET SAVED!", style);
			Handles.EndGUI();
		}
	}
}