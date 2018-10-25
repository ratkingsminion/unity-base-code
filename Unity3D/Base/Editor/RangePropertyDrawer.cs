using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RatKing.Base {

	[CustomPropertyDrawer(typeof(RangeInt))]
	[CustomPropertyDrawer(typeof(RangeFloat))]
	public class RangePropertyDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginProperty(position, label, property);
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			
			var width = position.width * 0.5f;
			var minRect	= new Rect(position.x, position.y, width, position.height);
			var maxRect	= new Rect(position.x + width, position.y, width, position.height);
			
			EditorGUIUtility.labelWidth = 32f;
			EditorGUI.PropertyField(minRect, property.FindPropertyRelative("min"), new GUIContent("min"));
			EditorGUI.PropertyField(maxRect, property.FindPropertyRelative("max"), new GUIContent("max"));

			EditorGUI.indentLevel = indent;

			EditorGUI.EndProperty();
		}
	}

}