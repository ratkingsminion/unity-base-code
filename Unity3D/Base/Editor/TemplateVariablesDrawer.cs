using UnityEngine;
using UnityEditor;

namespace RatKing.Base {
	
	[CustomPropertyDrawer(typeof(TemplateVariables))]
	public class TemplateVariablesDrawer : PropertyDrawer {
		static readonly float labelsHeight = 18f;
		static readonly float buttonsHeight = 20f;
		static readonly float entriesHeight = 20f;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			var variables = fieldInfo.GetValue(property.serializedObject.targetObject) as TemplateVariables;
			var h = labelsHeight + buttonsHeight * 2;
			if (variables != null && variables.Count > 0) { h += variables.Count * entriesHeight + labelsHeight; }
			return h;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			var headerStyle = EditorHelpers.GetHeaderStyle();

			//EditorGUI.BeginProperty(position, label, property);
			
			var serializedObject = property.serializedObject;
			var variables = fieldInfo.GetValue(serializedObject.targetObject) as TemplateVariables;
			
			var r = new Rect();
			position.width -= 5f;
			var w = position.width;
			GUI.Box(position, GUIContent.none);
			GUI.BeginGroup(position);

			r.Set(0f, 0f, w, labelsHeight);
			GUI.Label(r, "<b>" + label.text + " (" + variables.Count + ")</b>:", headerStyle);
			//GUI.changed = false;

			if (variables != null && variables.Count > 0) {
				r.Set(0f, r.y + r.height, 0f, labelsHeight);
				r.width = 38f; GUI.Label(r, "TYP");
				r.x += r.width; r.width = 62f; GUI.Label(r, "ID");
				r.x += r.width; r.width = w - r.x; GUI.Label(r, "VAL");
				
				int i = 0;
				for (var iter = variables.GetEnumerator(); iter.MoveNext();) {
					var v = iter.Current;

					r.Set(0f, r.y + r.height, w - 50f, entriesHeight);
					if (v != null && v.Unity3DDrawAndCheck(r)) {
						Selection.activeObject = v;
					}

					r.x += r.width; r.width = 30f;
					if (GUI.Button(r, "-")) {
						// delete a variable
						variables.Remove(v);
						Object.DestroyImmediate(v, true);
						AssetDatabase.SaveAssets();
						if (variables.Count == 0) { variables = null; }
						return;
					}
					// sorting the variables:
					r.x += r.width; r.width = 20f;
					if (i != 0 && GUI.Button(r, "^")) {
						variables.Remove(v);
						variables.Insert(i - 1, v);
						AssetDatabase.SaveAssets();
						return;
					}
					++i;
				}
			}

			r.Set(0f, r.y + r.height, w / 5f, buttonsHeight);
							if (GUI.Button(r, "+Txt"  )) { AddVariable<TemplateVarString>(serializedObject.targetObject, ref variables); }
			r.x += r.width;	if (GUI.Button(r, "+Int"  )) { AddVariable<TemplateVarInt>(serializedObject.targetObject, ref variables); }
			r.x += r.width;	if (GUI.Button(r, "+Float")) { AddVariable<TemplateVarFloat>(serializedObject.targetObject, ref variables); }
			r.x += r.width;	if (GUI.Button(r, "+Bool" )) { AddVariable<TemplateVarBool>(serializedObject.targetObject, ref variables); }
			r.x += r.width;	if (GUI.Button(r, "+List" )) { AddVariable<TemplateVarList>(serializedObject.targetObject, ref variables); }
			// can add more types here

			r.Set(0f, r.y + r.height, w, buttonsHeight);
			if (GUI.Button(r, "Clear all")) {
				var objs = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(serializedObject.targetObject));
				for (int j = 0; j < objs.Length; ++j) {
					if (objs[j] is TemplateVar) {
						if (variables.Contains((TemplateVar)objs[j])) { // comment this check if you want to delete all sub-SOs because they got stuck
							Object.DestroyImmediate(objs[j], true);
						}
					}
				}
				variables.Clear();
				variables = null;
				AssetDatabase.SaveAssets();
				return;
			}
			
			GUI.EndGroup();

			//EditorGUI.EndProperty();
		}

		public static void AddVariable<T>(Object asset, ref TemplateVariables variables) where T : TemplateVar {
			if (variables == null) { variables = new TemplateVariables(); }
			var si = ScriptableObject.CreateInstance<T>();
			variables.Add(si);
			EditorUtility.SetDirty(si);
			AssetDatabase.AddObjectToAsset(si, asset);
			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(asset));
		}
	}

}