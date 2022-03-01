using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace RatKing.Base {
	
	[CustomPropertyDrawer(typeof(DynamicVariable))]
	public class DynamicVariableDrawer : PropertyDrawer {
		// from https://github.com/lordofduct/spacepuppy-unity-framework/blob/master/SpacepuppyBaseEditor/EditorHelper.cs
        private static object GetValue_Imp(object source, string name) {
            if (source == null)  return null;
            var type = source.GetType();
			while (type != null) {
                var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null) return f.GetValue(source);
				var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null) return p.GetValue(source, null);
				type = type.BaseType;
            }
            return null;
        }
        private static object GetValue_Imp(object source, string name, int index) {
            var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
            if (enumerable == null) return null;
            var enm = enumerable.GetEnumerator();
			for (int i = 0; i <= index; i++) { if (!enm.MoveNext()) return null; }
            return enm.Current;
        }
		public static object GetTargetObjectOfProperty(SerializedProperty prop) {
            if (prop == null) return null;
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements) {
                if (element.Contains("[")) {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else {
                    obj = GetValue_Imp(obj, element);
                }
            }
            return obj;
        }

		//

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			return 0f;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			var isDirty = false;
			var serializedObject = property.serializedObject;
			var dVariable = GetTargetObjectOfProperty(property) as DynamicVariable;
			var variable = dVariable.Variable;

			GUILayout.BeginHorizontal();

			GUILayout.Label(label.text, GUILayout.Width(EditorGUIUtility.labelWidth));

			if (!string.IsNullOrEmpty(variable?.ID)) {
				GUILayout.Label(variable.Unity3DGetButtonName().ToUpper(), GUILayout.Width(30f));
				if (variable.Unity3DSetValue()) { isDirty = true; }
				if (GUILayout.Button("Clear", GUILayout.Width(45f))) { dVariable.Clear(); isDirty = true; }
			}
			else {
				if (GUILayout.Button(DynamicVarString.Unity3DButtonName)) { dVariable.Set<string>(); isDirty = true; }
				if (GUILayout.Button(DynamicVarInt.Unity3DButtonName)) { dVariable.Set<int>(); isDirty = true; }
				if (GUILayout.Button(DynamicVarFloat.Unity3DButtonName)) { dVariable.Set<float>(); isDirty = true; }
				if (GUILayout.Button(DynamicVarBool.Unity3DButtonName)) { dVariable.Set<bool>(); isDirty = true; }
				if (GUILayout.Button(DynamicVarObject.Unity3DButtonName)) { dVariable.Set<Object>(); isDirty = true; }
				// can add more types here
			}

			GUILayout.EndHorizontal();

			if (isDirty) { SetDirty(serializedObject); }
			serializedObject.ApplyModifiedProperties();
		}

		void SetDirty(SerializedObject so) {
			if (so == null || so.targetObject == null) { return; }
			so.ApplyModifiedProperties();
			EditorUtility.SetDirty(so.targetObject);
			if (AssetDatabase.IsMainAsset(so.targetObject)) { // TODO?
			//	AssetDatabase.SaveAssets();
			}
			else {
				var c = so.targetObject as Component;
				if (c != null) { UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(c.gameObject.scene); }
			}
		}
	}

}