using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RatKing.Base {

	// source: https://unity3d.com/how-to/architect-with-scriptable-objects
	// source: https://jacksondunstan.com/articles/3547
	
	[CreateAssetMenu(fileName="New Event Int", menuName="Variables/Event Int")]
	public class EventVarInt : ScriptableObject, ISerializationCallbackReceiver {
		[SerializeField] int startValue = 0;
		public int StartValue { get { return startValue; } }
		//
		int value;
		System.Action<EventVarInt, int, int> OnChanged;
		//
		public int Value {
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

		public void AddListener(System.Action<EventVarInt, int, int> listener, bool callAtOnce = false) {
			OnChanged += listener;
			if (callAtOnce) { listener(this, value, value); }
		}

		public void RemoveListener(System.Action<EventVarInt, int, int> listener) {
			OnChanged -= listener;
		}

		public override string ToString() {
			return value.ToString();
		}
	 
		public bool Equals(EventVarInt other) {
			return other.value.Equals(value);
		}
	 
		public override bool Equals(object other) {
			return other != null
				&& other is EventVarInt
				&& ((EventVarInt)other).value.Equals(value);
		}
	 
		public override int GetHashCode() {
			return value.GetHashCode();
		}

		//

		public static implicit operator int(EventVarInt ev) {
			return ev.value;
		}

		public static EventVarInt operator ++(EventVarInt ev) { ev.Value++; return ev; }
		public static EventVarInt operator --(EventVarInt ev) { ev.Value--; return ev; }
	}

}