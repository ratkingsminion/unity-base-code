using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

namespace RatKing.Base {
	
	[CustomPropertyDrawer(typeof(DynamicVariables))]
	public class DynamicVariablesDrawer : PropertyDrawer {
		static readonly float labelsHeight = 18f;
		static readonly float buttonsHeight = 20f;
		static readonly float entriesHeight = 20f;

		static GUIStyle headerStyle;

		public static GUIStyle GetHeaderStyle() {
			if (headerStyle == null) { headerStyle = new GUIStyle(GUI.skin.GetStyle("label")) { richText = true }; }
			return headerStyle;
		}

		//

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
			var dynamicDict = GetTargetObjectOfProperty(property) as DynamicVariables;
			var variables = dynamicDict.Variables;
			var h = labelsHeight + buttonsHeight * 2;
			if (variables != null && variables.Count > 0) { h += variables.Count * entriesHeight + labelsHeight; }
			return h;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			var headerStyle = GetHeaderStyle();
			
			var serializedObject = property.serializedObject;
			var dynamicDict = GetTargetObjectOfProperty(property) as DynamicVariables;
			var variables = dynamicDict.Variables;
			var variablesCount = variables != null ? variables.Count : 0;
			
			var r = new Rect();
			position.width -= 5f;
			var w = position.width;
			GUI.Box(position, GUIContent.none);
			GUI.BeginGroup(position);

			r.Set(0f, 0f, w, labelsHeight);
			GUI.Label(r, label.text + " (" + variablesCount + "):", headerStyle);
			//GUI.changed = false;
			
			var isDirty = false;

			if (variablesCount > 0) {
				var typeWidth = 32f;
				var idWidth = Screen.width * 0.25f;
				r.Set(0f, r.y + r.height, 0f, labelsHeight);
				r.width = typeWidth; GUI.Label(r, "Typ");
				r.x += r.width; r.width = idWidth; GUI.Label(r, "ID");
				r.x += r.width; r.width = w - r.x; GUI.Label(r, "Value");
				
				int i = 0;
				foreach (var v in variables) {
					r.Set(0f, r.y + r.height, w - 50f, entriesHeight);
					if (v != null) {
						var rw = r.width; r.width = typeWidth; GUI.Label(r, v.Unity3DGetButtonName());
						r.x += r.width; r.width = idWidth; var oldID = v.ID; v.ID = GUI.TextField(r, v.ID ?? ""); isDirty = isDirty || (v.ID != oldID);
						r.x += r.width; r.width = rw - (idWidth + typeWidth); if (v.Unity3DSetValue(r)) { isDirty = true; }
					}

					r.x += r.width; r.width = 30f;
					if (GUI.Button(r, "-")) {
						// delete a variable
						variables.Remove(v);
						if (variables.Count == 0) { variables = null; }
						isDirty = true;
						return;
					}
					// sorting the variables:
					r.x += r.width; r.width = 20f;
					if (i++ != 0 && GUI.Button(r, "^")) {
						variables.Remove(v);
						variables.Insert(i - 1, v);
						isDirty = true;
						return;
					}
				}
			}

			r.Set(0f, r.y + r.height, w / 5f, buttonsHeight);
							if (GUI.Button(r, "+Str")) { AddVariable<DynamicVarString>(property, ref variables); }
			r.x += r.width;	if (GUI.Button(r, "+Int")) { AddVariable<DynamicVarInt>(property, ref variables); }
			r.x += r.width;	if (GUI.Button(r, "+Flt")) { AddVariable<DynamicVarFloat>(property, ref variables); }
			r.x += r.width;	if (GUI.Button(r, "+Y/N")) { AddVariable<DynamicVarBool>(property, ref variables); }
			r.x += r.width;	if (GUI.Button(r, "+Obj")) { AddVariable<DynamicVarObject>(property, ref variables); }
			// can add more types here

			r.Set(0f, r.y + r.height, w, buttonsHeight);
			if (GUI.Button(r, "Clear all")) {
				variables.Clear();
				variables = null;
				isDirty = true;
			}

			if (isDirty) { SetDirty(serializedObject); }
			
			GUI.EndGroup();

			serializedObject.ApplyModifiedProperties();
		}

		void AddVariable<T>(SerializedProperty property, ref List<IDynamicVar> variables) where T : IDynamicVar, new() {
			if (variables == null) { variables = new List<IDynamicVar>(); }
			variables.Add(new T());
			SetDirty(property.serializedObject);
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