using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RatKing.Base {

	[CustomPropertyDrawer(typeof(Position2))]
	public class Position2PropertyDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginProperty(position, label, property);
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			
			var width = position.width * 0.5f - 2f;
			var xRect = new Rect(position.x, position.y, width, position.height);
			var yRect = new Rect(position.x + width + 4f, position.y, width, position.height);
			
			EditorGUIUtility.labelWidth = 12f;
			EditorGUI.PropertyField(xRect, property.FindPropertyRelative("x"), new GUIContent("X"));
			EditorGUI.PropertyField(yRect, property.FindPropertyRelative("y"), new GUIContent("Y"));

			EditorGUI.indentLevel = indent;

			EditorGUI.EndProperty();
		}
	}

}