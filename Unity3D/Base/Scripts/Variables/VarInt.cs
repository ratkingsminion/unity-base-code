using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RatKing.Base {

	// source: https://unity3d.com/how-to/architect-with-scriptable-objects
	
	[CreateAssetMenu(fileName="New Int", menuName="Variables/Int")]
	public class VarInt : ScriptableObject, ISerializationCallbackReceiver {
		[SerializeField] int startValue = 0;
		public int StartValue { get { return startValue; } }
		[System.NonSerialized] public int value;

		public void Reset() {
			value = startValue;
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize() { value = startValue; }
		void ISerializationCallbackReceiver.OnBeforeSerialize() { }
	}

}