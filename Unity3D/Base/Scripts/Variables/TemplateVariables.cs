#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;

namespace RatKing.Base {
	
	[System.Serializable]
	public class TemplateVariables : List<TemplateVar> {
		public T Get<T>(string id, T standardValue = default) where T:TemplateVar {
			foreach (var v in this) {
				if (v is T && v.name == id) { return v as T; }
			}
			return standardValue;
		}

		public T GetByBaseType<T>(string id, T standardValue = default) {
			foreach (var v in this) {
				if (v.name == id && v is TemplateVar<T> vt) {
					return vt.GetValue();
				}
			}
			return standardValue;
		}
	}

}