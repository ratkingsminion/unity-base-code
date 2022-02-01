using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RatKing.Base {
	
	[CustomEditor(typeof(VarFloat))]
	[CanEditMultipleObjects]
	public class VarFloatEditor : Editor {
		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			if (!serializedObject.isEditingMultipleObjects) {
				var vf = serializedObject.isEditingMultipleObjects ? null : (VarFloat)serializedObject.targetObject;
				EditorGUILayout.Space();
				EditorGUILayout.LabelField("Current Value: " + vf.value);
				EditorUtility.SetDirty(target);
			}
		}
	}

}