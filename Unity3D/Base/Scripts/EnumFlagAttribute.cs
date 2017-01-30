using System;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// source: https://gist.github.com/ikriz/b0f9d96205629e19859e

public class EnumFlagAttribute : PropertyAttribute {
	public string enumName;

	public EnumFlagAttribute() { }

	public EnumFlagAttribute(string name) {
		enumName = name;
	}
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(EnumFlagAttribute))]
public class EnumFlagDrawer : PropertyDrawer {
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		EnumFlagAttribute flagSettings = (EnumFlagAttribute)attribute;
		Enum targetEnum = (Enum)Enum.ToObject(fieldInfo.FieldType, property.intValue);

		GUIContent propName = new GUIContent(flagSettings.enumName);
		if (string.IsNullOrEmpty(flagSettings.enumName))
			propName = label;

		EditorGUI.BeginProperty(position, label, property);
		Enum enumNew = EditorGUI.EnumMaskField(position, propName, targetEnum);
		property.intValue = (int)Convert.ChangeType(enumNew, fieldInfo.FieldType);
		EditorGUI.EndProperty();
	}
}
#endif