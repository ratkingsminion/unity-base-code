using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RatKing.Base {
	
	[CustomEditor(typeof(EventVarFloat))]
	[CanEditMultipleObjects]
	public class EventVarFloatEditor : Editor {
		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			if (!serializedObject.isEditingMultipleObjects) {
				var evf = serializedObject.isEditingMultipleObjects ? null : (EventVarFloat)serializedObject.targetObject;
				EditorGUILayout.Space();
				EditorGUILayout.LabelField("Current Value: " + evf.Value);
				EditorUtility.SetDirty(target);
			}
		}
	}

}