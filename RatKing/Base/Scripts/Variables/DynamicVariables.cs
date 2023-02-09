using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RatKing.Base {

	[System.Serializable]
	public class DynamicVariables : ISerializationCallbackReceiver {
		[SerializeField] List<Object> objects = default;
		[SerializeField] List<string> serialized = default;
		public readonly List<IDynamicVar> Variables = new List<IDynamicVar>();
		public int Count => Variables.Count;

		//

		public void Clear() {
			objects.Clear();
			serialized.Clear();
			Variables.Clear();
		}
		
		public void Set<T>(string id, T value = default, bool removeIfDefault = false) {
			if (removeIfDefault && EqualityComparer<T>.Default.Equals(value, default)) {
				// https://stackoverflow.com/questions/65351/null-or-default-comparison-of-generic-argument-in-c-sharp
				Remove(id);
				return;
			}
			foreach (var v in Variables) {
				if (v.ID == id && v is DynamicVar<T> dv) {
					// Debug.Log("over write " + dv.Value + " to " + value);
					dv.Value = value;
					return;
				}
			}
				 if (typeof(T) == typeof(float) && value is float f) { Variables.Add(new DynamicVarFloat(id, f)); }
			else if (typeof(T) == typeof(int) && value is int i) { Variables.Add(new DynamicVarInt(id, i)); }
			else if (typeof(T) == typeof(string)) { Variables.Add(new DynamicVarString(id, value is string s ? s : "")); }
			else if (typeof(T) == typeof(bool) && value is bool b) { Variables.Add(new DynamicVarBool(id, b)); }
			else if (value is null) { Variables.Add(new DynamicVarObject(id, null)); }
			else if (value is Object) { Variables.Add(new DynamicVarObject(id, value as Object)); }
			else { Debug.LogWarning("Dynamic variable " + id + " could not be set to type " + typeof(T)); }
		}
		
		public void SetObject<T>(string id, T value = default, bool removeIfNull = false) where T : Object {
			if (removeIfNull && value == null) { Remove(id); return; }
			foreach (var v in Variables) {
				if (v.ID == id && v is DynamicVar<Object> od) { od.Value = value; return; }
			}
			Variables.Add(new DynamicVarObject(id, value));
		}

		public T Get<T>(string id, T standard = default) {
			foreach (var v in Variables) {
				if (v.ID == id && v is DynamicVar<T> dv) {
					return dv.Value;
				}
			}
			return standard;
		}

		public T GetObject<T>(string id, T standard = default) where T : Object {
			foreach (var v in Variables) {
				if (v.ID == id && v is DynamicVar<Object> dv && dv.Value is T value) {
					return value;
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

		public bool TryGetObject<T>(string id, out T result, bool mustNotBeNull = false) where T : Object {
			foreach (var v in Variables) {
				if (v.ID == id && v is DynamicVar<Object> dv && dv.Value is T value) {
					if (mustNotBeNull && dv.Value == null) { continue; }
					result = value;
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

		public bool HasObject(string id, bool mustNotBeNull = false) {
			foreach (var v in Variables) {
				if (v.ID == id && v is DynamicVar<Object> dv) {
					if (mustNotBeNull && dv.Value == null) { continue; }
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

		public void RemoveObject(string id) {
			for (int i = Variables.Count - 1; i >= 0; --i) {
				var v = Variables[i];
				if (v.ID == id && v is DynamicVar<Object>) {
					Variables.RemoveAt(i);
				}
			}
		}

		public void Remove(string id) {
			for (int i = Variables.Count - 1; i >= 0; --i) {
				var v = Variables[i];
				if (v.ID == id) {
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
				serialized.Add(v.TypeIdentifier + v.ID);
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
					default: Debug.LogWarning("Dynamic variable type for " + id + " could not be serialized"); break;
				}
			}
		}
	}

}
