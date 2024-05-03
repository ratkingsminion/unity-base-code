using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//#if ODIN_INSPECTOR
//using Sirenix.OdinInspector.Editor;
//using Sirenix.Utilities.Editor;
//#endif

namespace RatKing.Base {

	// from https://www.youtube.com/watch?v=uZmWgQ7cLNI

//#if ODIN_INSPECTOR
//	public class OptionalPropertyDrawer<T> : OdinValueDrawer<Optional<T>> {
//		protected override void DrawPropertyLayout(GUIContent label) {
//			var optional = this.ValueEntry.SmartValue;
//
//			var enabled = this.Property.Children["<Enabled>k__BackingField"];
//			var value = this.Property.Children["<Value>k__BackingField"];
//
//			enabled.Draw(label);
//			EditorGUI.BeginDisabledGroup(!optional.Enabled);
//			value.Draw(GUIContent.none);
//			EditorGUI.EndDisabledGroup();
//
//			this.ValueEntry.SmartValue = optional;
//		}
//#else

	[CustomPropertyDrawer(typeof(Optional<>))]
	public class OptionalPropertyDrawer : PropertyDrawer {
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			var valProp = property.FindPropertyRelative("value");
			return EditorGUI.GetPropertyHeight(valProp);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			//base.OnGUI(position, property, label);
			var valProp = property.FindPropertyRelative("value");
			var enableProp = property.FindPropertyRelative("enabled");
		
			position.width -= 24f;
			EditorGUI.BeginDisabledGroup(!enableProp.boolValue);
			EditorGUI.PropertyField(position, valProp, label, true);
			EditorGUI.EndDisabledGroup();
		
			position.x += position.width + 34f;
			position.width = position.height = EditorGUI.GetPropertyHeight(enableProp);
			position.x -= position.width + 12f;
			EditorGUI.PropertyField(position, enableProp, GUIContent.none);
		}
//#endif
	}

}
