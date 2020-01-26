#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;

namespace RatKing.Base {
	
	[System.Serializable]
	public class TemplateVar : ScriptableObject {
		
		public enum MinMax { None = 0, HasMinValue = 1, HasMaxValue = 2, HasMinAndMaxValue = 3, HasMinAndMaxSuggestion = 4 }

		public virtual IInstanceVar GetNewVar() { return null; }
		public override string ToString() { return "<missing>"; }
#if UNITY_EDITOR
		protected virtual string Unity3DGetButtonName() { return "NON"; }
		protected virtual void Unity3DSetStandardValue(Rect r) { /*nothing*/ }
		public bool Unity3DDrawAndCheck(Rect r) {
			var w = r.width; r.width = 38f;		var res = GUI.Button(r, Unity3DGetButtonName());
			r.x += r.width; r.width = 62f;		name = GUI.TextField(r, name ?? "");
			r.x += r.width; r.width = w - 100f;	Unity3DSetStandardValue(r);
			return res;
		}
#endif
	}
	
	[System.Serializable]
	public class TemplateVar<T> : TemplateVar {
		//public System.Type GetBaseType() { return typeof(T); }
		public virtual T GetValue() { return default; }
	}

}