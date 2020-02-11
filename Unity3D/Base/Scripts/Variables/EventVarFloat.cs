using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RatKing.Base {

	// source: https://unity3d.com/how-to/architect-with-scriptable-objects
	// source: https://jacksondunstan.com/articles/3547
	
	[CreateAssetMenu(fileName="New Event Float", menuName="Variables/Event Float")]
	public class EventVarFloat : ScriptableObject, ISerializationCallbackReceiver {
		//
		[SerializeField] float startValue = 0f;
		public float StartValue { get { return startValue; } }
		//
		float value;
		System.Action<EventVarFloat, float, float> OnChanged;
		//
		public float Value {
			get { return value; }
			set {
				var oldValue = this.value;
				this.value = value;
				if (OnChanged != null) {
					OnChanged(this, oldValue, value);
				}
			}
		}

		public void Reset(bool callEvents = true) {
			if (callEvents) { Value = startValue; }
			else { value = startValue; }
		}

		//

		void ISerializationCallbackReceiver.OnAfterDeserialize() { value = startValue; }
		void ISerializationCallbackReceiver.OnBeforeSerialize() { }

		//

		public void AddListener(System.Action<EventVarFloat, float, float> listener, bool callAtOnce = false) {
			OnChanged += listener;
			listener(this, value, value);
		}

		public void RemoveListener(System.Action<EventVarFloat, float, float> listener) {
			OnChanged -= listener;
		}

		public override string ToString() {
			return value.ToString();
		}
	 
		public bool Equals(EventVarFloat other) {
			return other.value.Equals(value);
		}
	 
		public override bool Equals(object other) {
			return other != null
				&& other is EventVarFloat
				&& ((EventVarFloat)other).value.Equals(value);
		}
	 
		public override int GetHashCode() {
			return value.GetHashCode();
		}

		//

		public static implicit operator float(EventVarFloat ev) {
			return ev.value;
		}

		public static EventVarFloat operator ++(EventVarFloat ev) { ev.Value++; return ev; }
		public static EventVarFloat operator --(EventVarFloat ev) { ev.Value--; return ev; }
	}

}