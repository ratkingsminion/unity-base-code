#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace RatKing.Base {

	[System.Serializable]
	public class TemplateVarBool : TemplateVar<bool> {
		public bool standardValue;
		public override IInstanceVar GetNewVar() { return new InstanceVarBool(standardValue); }
		public override bool GetValue() { return standardValue; }
		public override string ToString() { return standardValue ? "1" : "0"; }
#if UNITY_EDITOR
		protected override string Unity3DGetButtonName() { return "Y/N"; }
		protected override void Unity3DSetStandardValue(Rect r) { standardValue = EditorGUI.Toggle(r, standardValue); }
#endif
	}

}