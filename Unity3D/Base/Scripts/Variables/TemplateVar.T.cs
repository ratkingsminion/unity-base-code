#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;

namespace RatKing.Base {
	
	[System.Serializable]
	public class TemplateVar<T> : TemplateVar {
		public virtual T GetValue() { return default; }
	}

}