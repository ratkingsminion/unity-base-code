using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RatKing.Base {

	// source: https://unity3d.com/how-to/architect-with-scriptable-objects

	[CreateAssetMenu(fileName="New Float", menuName="Variables/Float")]
	public class VarFloat : ScriptableObject, ISerializationCallbackReceiver {
		[SerializeField] float startValue = 0f;
		public float StartValue { get { return startValue; } }
		[System.NonSerialized] public float value;

		public void Reset() {
			value = startValue;
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize() { value = startValue; }
		void ISerializationCallbackReceiver.OnBeforeSerialize() { }
	}

}