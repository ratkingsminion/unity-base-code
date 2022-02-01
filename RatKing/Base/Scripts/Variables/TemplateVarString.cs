#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace RatKing.Base {

	[System.Serializable]
	public class TemplateVarString : TemplateVar<string> {
		public string standardValue;
		public bool multiLine;
		public override IInstanceVar GetNewVar() { return new InstanceVarString(standardValue); }
		public override string GetValue() { return standardValue; }
		public override string ToString() { return '"' + standardValue + '"'; }
#if UNITY_EDITOR
		protected override string Unity3DGetButtonName() { return "TXT"; }
		protected override void Unity3DSetStandardValue(Rect r) { standardValue = EditorGUI.TextField(r, standardValue); }
#endif
	}

}