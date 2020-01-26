#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace RatKing.Base {

	[System.Serializable]
	public class TemplateVarList : TemplateVar<int> {
		public int standardIndex;
		[SerializeField] string[] names = null;
		public TemplateVarList source = null;
		public override IInstanceVar GetNewVar() { return new InstanceVarList(new ListType(this, standardIndex)); }
		public override int GetValue() { return standardIndex; }
		public override string ToString() { return standardIndex.ToString(); }
		public string[] GetNames() { if (source != null) { return source.names; } return names; }
		public string GetName(int index) { if (source != null) { return source.names[index % source.names.Length]; } return names[index % names.Length]; }
#if UNITY_EDITOR
		protected override string Unity3DGetButtonName() { return "LST"; }
		protected override void Unity3DSetStandardValue(Rect r) { standardIndex = EditorGUI.IntField(r, standardIndex); }
#endif
	}

}