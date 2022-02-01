#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace RatKing.Base {

	[System.Serializable]
	public class TemplateVarFloat : TemplateVar<float> {
		public float standardValue;
		public MinMax minMax = MinMax.None;
		public float valueMin;
		public float valueMax;
		public override IInstanceVar GetNewVar() { return new InstanceVarFloat(standardValue); }
		public override float GetValue() { return standardValue; }
		public override string ToString() { return standardValue.ToString("0.###"); }
#if UNITY_EDITOR
		protected override string Unity3DGetButtonName() { return "FLT"; }
		protected override void Unity3DSetStandardValue(Rect r) { standardValue = EditorGUI.FloatField(r, standardValue); }
#endif
		public bool IsOutsideMinMax(InstanceVarFloat num) {
			return num.value < valueMin || num.value > valueMax;
		}

		//

		public static implicit operator float(TemplateVarFloat tv) {
			return tv.standardValue;
		}
	}

}