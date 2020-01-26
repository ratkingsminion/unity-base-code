using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base.Samples {
	
	public class SampleClassWithInstancedTemplateVars {
		public SampleTypeWithTemplateVars Type { get; private set; }
		public Base.InstancedVariablesContainer Variables { get; private set; }

		//

		public SampleClassWithInstancedTemplateVars(SampleTypeWithTemplateVars type) {
			Type = type;
			Variables = new Base.InstancedVariablesContainer(type.Variables);
		}
	}

}