using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RatKing.Base {

	[CustomPropertyDrawer(typeof(Position3))]
	public class Position3PropertyDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginProperty(position, label, property);
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			
			var width = position.width / 3f - 1f;
			var xRect = new Rect(position.x, position.y, width, position.height);
			var yRect = new Rect(position.x + width + 1.5f, position.y, width, position.height);
			var zRect = new Rect(position.x + 2f * width + 3f, position.y, width, position.height);
			
			EditorGUIUtility.labelWidth = 10f;
			EditorGUI.PropertyField(xRect, property.FindPropertyRelative("x"), new GUIContent("X"));
			EditorGUI.PropertyField(yRect, property.FindPropertyRelative("y"), new GUIContent("Y"));
			EditorGUI.PropertyField(zRect, property.FindPropertyRelative("z"), new GUIContent("Z"));

			EditorGUI.indentLevel = indent;

			EditorGUI.EndProperty();
		}
	}

}