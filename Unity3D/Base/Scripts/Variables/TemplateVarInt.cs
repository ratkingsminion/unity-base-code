#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace RatKing.Base {

	[System.Serializable]
	public class TemplateVarInt : TemplateVar<int> {
		public int standardValue;
		public MinMax minMax = MinMax.None;
		public int valueMin;
		public int valueMax;
		public override IInstanceVar GetNewVar() { return new InstanceVarInt(standardValue); }
		public override int GetValue() { return standardValue; }
		public override string ToString() { return standardValue.ToString("0.###"); }
#if UNITY_EDITOR
		protected override string Unity3DGetButtonName() { return "INT"; }
		protected override void Unity3DSetStandardValue(Rect r) { standardValue = EditorGUI.IntField(r, standardValue); }
#endif
		public bool IsOutsideMinMax(InstanceVarInt num) {
			return num.value < valueMin || num.value > valueMax;
		}
	}

}