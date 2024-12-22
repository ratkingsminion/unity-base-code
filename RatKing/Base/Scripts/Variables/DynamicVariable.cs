using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RatKing.Base {

	public interface IDynamicVar {
		string ID { get; set; }
		IDynamicVar GetCopy();
		string ToString();
		string TypeIdentifier { get; }
#if UNITY_EDITOR
		string Unity3DGetButtonName();
		bool Unity3DSetValue(Rect r);
#endif
#if UNITY_2022_1_OR_NEWER
		public int GetInt() { return this is DynamicVarInt v ? v.Value : default; }
		public float GetFloat() { return this is DynamicVarFloat v ? v.Value : default; }
		public float GetNumber() { return this is DynamicVarFloat f ? f.Value : this is DynamicVarInt i ? i.Value : default; }
		public string GetString() { return this is DynamicVarString v ? v.Value : default; }
		public bool GetBool() { return this is DynamicVarBool v && v.Value; }
		public Object GetObject() { return this is DynamicVarObject v ? v.Value : default; }
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
		public override string ToString() { return ""; }
		public abstract string TypeIdentifier { get; }
#if UNITY_EDITOR
		public abstract string Unity3DGetButtonName();
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
		public override string ToString() => value.ToString();
		public override string TypeIdentifier => "i";
#if UNITY_EDITOR
		public const string Unity3DButtonName = "Int";
		public override string Unity3DGetButtonName() => Unity3DButtonName;
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
		public override string ToString() => value.ToString(System.Globalization.CultureInfo.InvariantCulture);
		public override string TypeIdentifier => "f";
#if UNITY_EDITOR
		public const string Unity3DButtonName = "Flt";
		public override string Unity3DGetButtonName() => Unity3DButtonName;
		public override bool Unity3DSetValue(Rect r) { EditorGUI.BeginChangeCheck(); value = EditorGUI.FloatField(r, value); return EditorGUI.EndChangeCheck(); }
#endif
	}
	
	[System.Serializable]
	public class DynamicVarString : DynamicVar<string> {
		public DynamicVarString() { }
		public DynamicVarString(string id, string value) : base(id, value) { this.value = value; }
		public override string Value { get => value; set => this.value = value; }
		public override IDynamicVar GetCopy() { return new DynamicVarString(id, value); }
		public override string ToString() => (value ?? "").ToString();
		public override string TypeIdentifier => "s";
#if UNITY_EDITOR
		public const string Unity3DButtonName = "Str";
		public override string Unity3DGetButtonName() => Unity3DButtonName;
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
		public override string ToString() => value.ToString();
		public override string TypeIdentifier => "b";
#if UNITY_EDITOR
		public const string Unity3DButtonName = "Y/N";
		public override string Unity3DGetButtonName() => Unity3DButtonName;
		public override bool Unity3DSetValue(Rect r) { EditorGUI.BeginChangeCheck(); value = EditorGUI.Toggle(r, value); return EditorGUI.EndChangeCheck(); }
#endif
	}
	
	[System.Serializable]
	public class DynamicVarObject : DynamicVar<Object> {
		public DynamicVarObject() { }
		public DynamicVarObject(string id, Object value) : base(id, value) { this.value = value; }
		public override Object Value { get => value; set => this.value = value; }
		public override IDynamicVar GetCopy() { return new DynamicVarObject(id, value); }
		public override string ToString() => value != null ? value.ToString() : "(null)";
		public override string TypeIdentifier => "o";
#if UNITY_EDITOR
		public const string Unity3DButtonName = "Obj";
		public override string Unity3DGetButtonName() => Unity3DButtonName;
		public override bool Unity3DSetValue(Rect r) { var oldVal = value; value = EditorGUI.ObjectField(r, value, typeof(Object), true); return oldVal != value; }
#endif
	}

	[System.Serializable]
	public class DynamicVariable : ISerializationCallbackReceiver {

		[SerializeField] Object @object = default;
		[SerializeField] string serialized = default;
		public IDynamicVar Variable { get; private set; } = null;

		//

		public void Clear() {
			@object = default;
			serialized = default;
			Variable = null;
		}

		public override string ToString() {
			return Variable?.ToString();
		}

		public void Set<T>(T value = default) {
			if (Variable is DynamicVar<T> dv) { dv.Value = value; }
			else if (typeof(T) == typeof(float) && value is float f) { Variable = new DynamicVarFloat("set", f); }
			else if (typeof(T) == typeof(int) && value is int i) { Variable = new DynamicVarInt("set", i); }
			else if (typeof(T) == typeof(string)) { Variable = new DynamicVarString("set", value is string s ? s : ""); }
			else if (typeof(T) == typeof(bool) && value is bool b) { Variable = new DynamicVarBool("set", b); }
			else if (value is Object o) { Variable = new DynamicVarObject("set", o); }
#if UNITY_2020_1_OR_NEWER
			else if (value is null) { Variable = new DynamicVarObject("set", null); }
#else
			else if (value == null) { Variable = new DynamicVarObject("set", null); }
#endif
			else { Debug.LogWarning("Dynamic variable could not be set to this type"); }
		}

		public T Get<T>(T standard = default) {
			if (standard is Object && Variable is DynamicVar<Object> dv && dv.Value is T value) { return value; }
			else if (Variable is DynamicVar<T> dvt) { return dvt.Value; }
			return standard;
		}

		public float GetNumber(float standard = default) {
			if (Variable is DynamicVar<float> df) { return df.Value; }
			else if (Variable is DynamicVar<int> di) { return di.Value; }
			return standard;
		}

		public bool TryGet<T>(out T result) {
			if ((T)default is Object && Variable is DynamicVar<Object> dv && dv.Value is T value) {
				result = value;
				return true;
			}
			else if (Variable is DynamicVar<T> dvt) {
				result = dvt.Value;
				return true;
			}
			result = default;
			return false;
		}

		public bool TryGetNumber(out float result) {
			if (Variable is DynamicVar<float> df) { result = df.Value; return true; }
			else if (Variable is DynamicVar<int> di) { result = di.Value; return true; }
			result = -1f;
			return false;
		}

		public bool IsNumber() {
			return Variable is DynamicVar<float> || Variable is DynamicVar<int>;
		}

		public DynamicVariable GetCopy() {
			var dv = new DynamicVariable();
			dv.Variable = Variable.GetCopy();
			return dv;
		}

		/// if exactly is false then it suffices that A's vars are in B
		/// if exactly is true then A needs to be completely like B
		public bool Is(DynamicVariable other) {
			if (other == null) { return false; }
			switch (Variable) {
				case DynamicVarInt i: if (other.Get<int>() != i.Value) { return false; } break;
				case DynamicVarFloat f: if (other.Get<float>() != f.Value) { return false; } break;
				case DynamicVarString s: if (other.Get<string>() != s.Value) { return false; } break;
				case DynamicVarBool b: if (other.Get<bool>() != b.Value) { return false; } break;
				case DynamicVarObject o: if (other.Get<Object>() != o.Value) { return false; } break;
			}
			return true;
		}

		/// if exactly is false then it suffices that A's vars are in B
		/// if exactly is true then A needs to be completely like B
		public static bool Is(DynamicVariable A, DynamicVariable B) {
			return A.Is(B);
		}

		//

        void ISerializationCallbackReceiver.OnBeforeSerialize() {
			if (Variable == null) { serialized = null; return; }
			serialized = Variable.TypeIdentifier.ToString();
			if (Variable is DynamicVarObject o) { @object = o.Value; }
			else { @object = null;  serialized += Variable.ToString(); }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()  {
			if (string.IsNullOrEmpty(serialized)) { return; }
			switch (serialized[0]) {
				case 'f': Variable = new DynamicVarFloat("set", serialized.Substring(1)); break;
				case 'i': Variable = new DynamicVarInt("set", serialized.Substring(1)); break;
				case 's': Variable = new DynamicVarString("set", serialized.Substring(1)); break;
				case 'b': Variable = new DynamicVarBool("set", serialized.Substring(1)); break;
				case 'o': Variable = new DynamicVarObject("set", @object); break;
				default: Debug.LogWarning("Dynamic variable type could not be serialized"); break;
			}
		}
	}

}
