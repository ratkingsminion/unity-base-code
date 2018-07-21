using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RatKing.Base {

	[System.Serializable]
	public struct RangeInt {
		public int min;
		public int max;

		public RangeInt(int min, int max) {
			this.min = min;
			this.max = max;
		}

		public int Random(System.Random generator = null) {
			int a = min, b = max;
			if (a > b) { a = b; b = min; }
			if (generator == null) { return UnityEngine.Random.Range(a, b); }
			return generator.Next(a, b);
		}

		public int Difference { get { return Mathf.Abs(max - min); } }
	}

	[System.Serializable]
	public struct RangeFloat {
		public float min;
		public float max;

		public RangeFloat(float min, float max) {
			this.min = min;
			this.max = max;
		}

		public float Random(System.Random generator = null) {
			float a = min, b = max;
			if (a > b) { a = b; b = min; }
			if (generator == null) { return UnityEngine.Random.Range(a, b); }
			return (float)generator.NextDouble() * (b - a) + a;
		}

		public float Lerp(float factor) {
			return min + (max - min) * factor;
		}

		public float Difference { get { return Mathf.Abs(max - min); } }
	}

}