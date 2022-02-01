using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RatKing.Base {
	
	[CustomEditor(typeof(EventVarInt))]
	[CanEditMultipleObjects]
	public class EventVarIntEditor : Editor {
		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			if (!serializedObject.isEditingMultipleObjects) {
				var evi = serializedObject.isEditingMultipleObjects ? null : (EventVarInt)serializedObject.targetObject;
				EditorGUILayout.Space();
				EditorGUILayout.LabelField("Current Value: " + evi.Value);
				EditorUtility.SetDirty(target);
			}
		}
	}

}