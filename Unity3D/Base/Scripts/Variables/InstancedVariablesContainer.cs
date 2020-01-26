using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base {

	public interface IInstanceVar {
		IInstanceVar GetCopy();
		void SendOnChangeMessage(Component c, string name);
		string ToString();
	}

	public class InstanceVar<T> : IInstanceVar {
		public T value;
		public InstanceVar(T value) { this.value = value; }
		public void Set(T value) { this.value = value; }
		public virtual IInstanceVar GetCopy() { return new InstanceVar<T>(value); }
		public override string ToString() { return value.ToString(); }
		//
		public void SendOnChangeMessage(Component c, string name) { c.SendMessage("OnChangeVar_" + name, value, SendMessageOptions.DontRequireReceiver); }
	}
		
	public class InstanceVarInt : InstanceVar<int> {
		public InstanceVarInt(int value) : base(value) { }
		public override IInstanceVar GetCopy() { return new InstanceVarInt(value); }
		public override string ToString() { return value.ToString(); }
	}
	public class InstanceVarFloat : InstanceVar<float> {
		public InstanceVarFloat(float value) : base(value) { }
		public override IInstanceVar GetCopy() { return new InstanceVarFloat(value); }
		public override string ToString() { return value.ToString("0.###"); }
	}
	public class InstanceVarString : InstanceVar<string> {
		public InstanceVarString(string value) : base(value) { }
		public override IInstanceVar GetCopy() { return new InstanceVarString(value); }
		public override string ToString() { return '"' + StringExtras.Escape(value) + '"'; }
	}
	public class InstanceVarBool : InstanceVar<bool> {
		public InstanceVarBool(bool value) : base(value) { }
		public override IInstanceVar GetCopy() { return new InstanceVarBool(value); }
		public override string ToString() { return value ? "1" : "0"; }
	}
	public class InstanceVarList : InstanceVar<ListType> {
		public InstanceVarList(ListType value) : base(value) { }
		public override IInstanceVar GetCopy() { return new InstanceVarList(value.GetCopy()); }
		public override string ToString() { return value.ToString(); }
	}
		
	//
		
	public class InstancedVariablesContainer {
		readonly int varCount = 0;
		readonly List<TemplateVar> templateVars = null;
		IInstanceVar[] instanceVars;

		//

		public InstancedVariablesContainer(TemplateVariables templateVars) : this(templateVars.List) {
		}

		public InstancedVariablesContainer(List<TemplateVar> templateVars) {
			if (templateVars == null) { return; }
			this.templateVars = templateVars;
			varCount = templateVars.Count;
			if (instanceVars == null || instanceVars.Length != varCount) { instanceVars = new IInstanceVar[varCount]; }
			for (int i = 0, j = 0; i < varCount; ++i) {
				var vt = templateVars[i];
				if (vt == null) { Debug.LogError("Missing variable"); break; }
				instanceVars[j] = vt.GetNewVar();
				++j;
			}
		}
			
		public bool Set<T>(string id, T value) {
			for (int i = 0; i < varCount; ++i) {
				if (templateVars[i].name == id && instanceVars[i] is InstanceVar<T>) {
					(instanceVars[i] as InstanceVar<T>).Set(value);
					return true;
				}
			}
			return false;
		}

		public T Get<T>(string id, T standard = default) {
			for (int i = 0; i < varCount; ++i) {
				if (templateVars[i].name == id && instanceVars[i] is InstanceVar<T>) {
					return (instanceVars[i] as InstanceVar<T>).value;
				}
			}
			return standard;
		}

		public bool Has<T>(string id) {
			for (int i = 0; i < varCount; ++i) {
				if (templateVars[i].name == id && instanceVars[i] is InstanceVar<T>) {
					return true;
				}
			}
			return false;
		}

		public IInstanceVar[] GetCopyOfVars() {
			if (instanceVars == null) { return null; }
			var copy = new IInstanceVar[varCount];
			for (int i = 0; i < varCount; ++i) {
				copy[i] = instanceVars[i].GetCopy();
			}
			return copy;
		}
	}

}