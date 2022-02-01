using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base {

	public class ListType {
		public TemplateVarList names;
		public int index;
		//
		public ListType(TemplateVarList names) { this.names = names; this.index = 0; }
		public ListType(TemplateVarList names, int index) { this.names = names; this.index = index; }
		public ListType GetCopy() { return new ListType(names, index); }
		//
		public override string ToString() { return index.ToString() + "/" + names.GetNames().Length; }
	}

}