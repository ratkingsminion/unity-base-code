using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RatKing.Base {

	public class WobbleAnimation {
		public float factor = 0f;
		public float seconds = 1f;
		public float startSeconds = 1f;
		public float strength = 1f;
		public WobbleAnimations.Type type = WobbleAnimations.Type.Scale;
		public Transform target;
		public Vector3 originalScale;
		public Vector3 rotationAxis;
		public Quaternion originalRotation;
		public float startTime = 0f;
		public bool ignoreTimeScale;

		public void IgnoreTimeScale(bool ignore = true) { ignoreTimeScale = ignore; }
	}

}