using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RatKing.Base {
	
	[CustomEditor(typeof(VarInt))]
	[CanEditMultipleObjects]
	public class VarIntEditor : Editor {
		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			if (!serializedObject.isEditingMultipleObjects) {
				var vi = serializedObject.isEditingMultipleObjects ? null : (VarInt)serializedObject.targetObject;
				EditorGUILayout.Space();
				EditorGUILayout.LabelField("Current Value: " + vi.value);
				EditorUtility.SetDirty(target);
			}
		}
	}

}