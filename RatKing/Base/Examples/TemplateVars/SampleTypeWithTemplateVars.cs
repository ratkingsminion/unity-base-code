using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base.Samples {
	
	public class SampleTypeWithTemplateVars : ScriptableObject {
		[SerializeField] string id = "";
		public string ID { get => id; }
		[SerializeField] Base.TemplateVariables constants = null;
		public Base.TemplateVariables Constants { get => constants; }
		[SerializeField] Base.TemplateVariables variables = null;
		public Base.TemplateVariables Variables { get => variables; }

		//

		public T GetVariable<T>(string id, T standardValue = default) where T:Base.TemplateVar {
			if (variables != null) { return variables.Get<T>(id, standardValue); }
			return standardValue;
		}

		// "Constants" are just variables, but they should not be overwritten per instanced object
		public T GetConstant<T>(string id, T standardValue = default) {
			if (constants != null) { return constants.GetByBaseType<T>(id, standardValue); }
			return standardValue;
		}
	}

}