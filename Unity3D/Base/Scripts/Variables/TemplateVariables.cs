#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;

namespace RatKing.Base {
	
	[System.Serializable]
	public class TemplateVariables {
		[SerializeField] List<TemplateVar> list = null;
		public List<TemplateVar> List { get => list; }

		//

		public T Get<T>(string id, T standardValue = default) where T:TemplateVar {
			if (list == null) { return standardValue; }
			foreach (var v in list) {
				if (v is T && v.name == id) { return v as T; }
			}
			return standardValue;
		}

		public T GetByBaseType<T>(string id, T standardValue = default) {
			if (list == null) { return standardValue; }
			foreach (var v in list) {
				if (v.name == id && v is TemplateVar<T> vt) {
					return vt.GetValue();
				}
			}
			return standardValue;
		}
	}

}