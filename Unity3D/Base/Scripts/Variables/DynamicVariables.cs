using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RatKing.Base {

	public interface IDynamicVar {
		string ID { get; set; }
		IDynamicVar GetCopy();
		string TypeIdentifier { get; }
		string ToString();
#if UNITY_EDITOR
		string Unity3DGetButtonName();
		bool Unity3DSetValue(Rect r);
#endif
	}

	[System.Serializable]
	public abstract class DynamicVar<T> : IDynamicVar {
		[SerializeField] protected T value = default;
		[SerializeField] protected string id = "";
		public DynamicVar() { }
		public DynamicVar(string id, T value) { this.id = id; }
		public DynamicVar(string id, string value) { this.id = id; }
		public string ID { get => id; set => id = value; }
		public abstract T Value { get; set; }
		public abstract IDynamicVar GetCopy();
		public abstract string TypeIdentifier { get; }
		public override string ToString() { return ""; }
#if UNITY_EDITOR
		public string Unity3DGetButtonName() => TypeIdentifier.ToUpper();
		public abstract bool Unity3DSetValue(Rect r);
#endif
	}
	
	[System.Serializable]
	public class DynamicVarInt : DynamicVar<int> {
		public DynamicVarInt() { }
		public DynamicVarInt(string id, int value) : base(id, value) { this.value = value; }
		public DynamicVarInt(string id, string value) : base(id, value) { this.value = int.Parse(value); }
		public override int Value { get => value; set => this.value = value; }
		public override IDynamicVar GetCopy() { return new DynamicVarInt(id, value); }
		public override string TypeIdentifier => "int";
		public override string ToString() => value.ToString();
#if UNITY_EDITOR
		public override bool Unity3DSetValue(Rect r) { EditorGUI.BeginChangeCheck(); value = EditorGUI.IntField(r, value); return EditorGUI.EndChangeCheck(); }
#endif
	}
	
	[System.Serializable]
	public class DynamicVarFloat : DynamicVar<float> {
		public DynamicVarFloat() { }
		public DynamicVarFloat(string id, float value) : base(id, value) { this.value = value; }
		public DynamicVarFloat(string id, string value) : base(id, value) { this.value = float.Parse(value, System.Globalization.CultureInfo.InvariantCulture); }
		public override float Value { get => value; set => this.value = value; }
		public override IDynamicVar GetCopy() { return new DynamicVarFloat(id, value); }
		public override string TypeIdentifier => "flt";
		public override string ToString() => value.ToString(System.Globalization.CultureInfo.InvariantCulture);
#if UNITY_EDITOR
		public override bool Unity3DSetValue(Rect r) { EditorGUI.BeginChangeCheck(); value = EditorGUI.FloatField(r, value); return EditorGUI.EndChangeCheck(); }
#endif
	}
	
	[System.Serializable]
	public class DynamicVarString : DynamicVar<string> {
		public DynamicVarString() { }
		public DynamicVarString(string id, string value) : base(id, value) { this.value = value; }
		public override string Value { get => value; set => this.value = value; }
		public override IDynamicVar GetCopy() { return new DynamicVarString(id, value); }
		public override string TypeIdentifier => "str";
		public override string ToString() => (value ?? "").ToString();
#if UNITY_EDITOR
		public override bool Unity3DSetValue(Rect r) { EditorGUI.BeginChangeCheck(); value = EditorGUI.TextField(r, value); return EditorGUI.EndChangeCheck(); }
#endif
	}
	
	[System.Serializable]
	public class DynamicVarBool : DynamicVar<bool> {
		public DynamicVarBool() { }
		public DynamicVarBool(string id, bool value) : base(id, value) { this.value = value; }
		public DynamicVarBool(string id, string value) : base(id, value) { this.value = bool.Parse(value); }
		public override bool Value { get => value; set => this.value = value; }
		public override IDynamicVar GetCopy() { return new DynamicVarBool(id, value); }
		public override string TypeIdentifier => "y/n";
		public override string ToString() => value.ToString();
#if UNITY_EDITOR
		public override bool Unity3DSetValue(Rect r) { EditorGUI.BeginChangeCheck(); value = EditorGUI.Toggle(r, value); return EditorGUI.EndChangeCheck(); }
#endif
	}
	
	[System.Serializable]
	public class DynamicVarObject : DynamicVar<Object> {
		public DynamicVarObject() { }
		public DynamicVarObject(string id, Object value) : base(id, value) { this.value = value; }
		public override Object Value { get => value; set => this.value = value; }
		public override IDynamicVar GetCopy() { return new DynamicVarObject(id, value); }
		public override string TypeIdentifier => "obj";
		public override string ToString() => value != null ? value.ToString() : "(null)";
#if UNITY_EDITOR
		public override bool Unity3DSetValue(Rect r) { var oldVal = value; value = EditorGUI.ObjectField(r, value, typeof(Object), true); return oldVal != value; }
#endif
	}

	[System.Serializable]
	public class DynamicVariables : ISerializationCallbackReceiver {
		[SerializeField] List<Object> objects = default;
		[SerializeField] List<string> serialized = default;
		public List<IDynamicVar> Variables { get; private set; } = new List<IDynamicVar>();
		public int Count => Variables.Count;

		//

		public void Clear() {
			objects.Clear();
			serialized.Clear();
			Variables.Clear();
		}
			
		public void Set<T>(string id, T value) {
			foreach (var v in Variables) {
				if (v.ID == id && v is DynamicVar<T> dv) {
					// Debug.Log("over write " + dv.Value + " to " + value);
					dv.Value = value;
					return;
				}
			}
			switch(value) {
				case int i: Variables.Add(new DynamicVarInt(id, i)); break;
				case float f: Variables.Add(new DynamicVarFloat(id, f)); break;
				case string s: Variables.Add(new DynamicVarString(id, s)); break;
				case bool b: Variables.Add(new DynamicVarBool(id, b)); break;
				case Object o: Variables.Add(new DynamicVarObject(id, o)); break;
			}
		}

		public T Get<T>(string id, T standard = default) {
			foreach (var v in Variables) {
				if (v.ID == id && v is DynamicVar<T> dv) {
					return dv.Value;
				}
			}
			return standard;
		}

		public float GetNumber(string id, float standard = default) {
			foreach (var v in Variables) {
				if (v.ID == id) {
					if (v is DynamicVar<float> df) { return df.Value; }
					else if (v is DynamicVar<int> di) { return di.Value; }
				}
			}
			return standard;
		}

		public bool TryGet<T>(string id, out T result) {
			foreach (var v in Variables) {
				if (v.ID == id && v is DynamicVar<T> dv) {
					result = dv.Value;
					return true;
				}
			}
			result = default;
			return false;
		}

		public bool TryGetNumber(string id, out float result) {
			foreach (var v in Variables) {
				if (v.ID == id) {
					if (v is DynamicVar<float> df) { result = df.Value; return true; }
					else if (v is DynamicVar<int> di) { result = di.Value; return true; }
				}
			}
			result = -1f;
			return false;
		}

		public bool Has(string id) {
			foreach (var v in Variables) {
				if (v.ID == id) {
					return true;
				}
			}
			return false;
		}

		public bool Has<T>(string id) {
			foreach (var v in Variables) {
				if (v.ID == id && v is DynamicVar<T>) {
					return true;
				}
			}
			return false;
		}

		public bool HasNumber(string id) {
			foreach (var v in Variables) {
				if (v.ID == id && (v is DynamicVar<float> || v is DynamicVar<int>)) {
					return true;
				}
			}
			return false;
		}

		public void Remove<T>(string id) {
			for (int i = Variables.Count - 1; i >= 0; --i) {
				var v = Variables[i];
				if (v.ID == id && v is DynamicVar<T>) {
					Variables.RemoveAt(i);
				}
			}
		}

		public DynamicVariables GetCopy() {
			if (Variables == null) { return null; }
			var dv = new DynamicVariables();
			foreach (var v in Variables) {
				dv.Variables.Add(v.GetCopy());
			}
			return dv;
		}

		public void Merge(DynamicVariables other) {
			if (other == null || other.Count == 0) { return; }
			foreach (var v in other.Variables) {
				switch(v) {
					case DynamicVarInt i: Set(v.ID, i.Value); break;
					case DynamicVarFloat f: Set(v.ID, f.Value); break;
					case DynamicVarString s: Set(v.ID, s.Value); break;
					case DynamicVarBool b: Set(v.ID, b.Value); break;
					case DynamicVarObject o: Set(v.ID, o.Value); break;
				}
			}
		}

		/// if exactly is false then it suffices that A's vars are in B
		/// if exactly is true then A needs to be completely like B
		public bool Is(DynamicVariables other, bool exactly = false) {
			if (other == null) { return false; }
			if (exactly) { if (other.Count != Count) { return false; } }
			else { if (other.Count < Count) { return false; } }
			foreach (var v in Variables) {
				switch (v) {
					case DynamicVarInt i: if (other.Get<int>(v.ID) != i.Value) { return false; } break;
					case DynamicVarFloat f: if (other.Get<float>(v.ID) != f.Value) { return false; } break;
					case DynamicVarString s: if (other.Get<string>(v.ID) != s.Value) { return false; } break;
					case DynamicVarBool b: if (other.Get<bool>(v.ID) != b.Value) { return false; } break;
					case DynamicVarObject o: if (other.Get<Object>(v.ID) != o.Value) { return false; } break;
				}
			}
			return true;
		}

		public static DynamicVariables Merged(DynamicVariables A, DynamicVariables B) {
			DynamicVariables merged = null;
			if (A != null && A.Count > 0) {
				merged = A.GetCopy();
			}
			if (B != null && B.Count > 0) {
				if (merged != null) { merged.Merge(B); }
				else { merged = B.GetCopy();  }
			}
			return merged;
		}

		/// if exactly is false then it suffices that A's vars are in B
		/// if exactly is true then A needs to be completely like B
		public static bool Is(DynamicVariables A, DynamicVariables B, bool exactly = false) {
			if (A == null || A.Count == 0) {
				if (B == null || B.Count == 0) { return true; }
				return false;
			}
			return A.Is(B, exactly);
		}

		//

        void ISerializationCallbackReceiver.OnBeforeSerialize() {
			if (objects == null) { objects = new List<Object>(); }
			else { objects.Clear(); }
			if (serialized == null) { serialized = new List<string>(Variables.Count * 2); }
			else { serialized.Clear(); }
			foreach (var v in Variables) {
				serialized.Add(v.TypeIdentifier[0] + v.ID);
				if (v is DynamicVarObject o) {
					var idx = objects.IndexOf(o.Value);
					if (idx < 0) { idx = objects.Count; objects.Add(o.Value); }
					serialized.Add(idx.ToString());
				}
				else {
					serialized.Add(v.ToString());
				}
			}
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()  {
			Variables.Clear();
			for (int i = 0, c = serialized.Count; i < c; i += 2) {
				var type = serialized[i][0];
				var id = serialized[i].Substring(1);
				var value = serialized[i + 1];
				switch (type) {
					case 'f': Variables.Add(new DynamicVarFloat(id, value)); break;
					case 'i': Variables.Add(new DynamicVarInt(id, value)); break;
					case 's': Variables.Add(new DynamicVarString(id, value)); break;
					case 'b': Variables.Add(new DynamicVarBool(id, value)); break;
					case 'o': Variables.Add(new DynamicVarObject(id, objects[int.Parse(value)])); break;
				}
			}
		}
	}

}
