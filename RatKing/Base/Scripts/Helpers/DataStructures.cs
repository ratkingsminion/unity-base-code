using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base {
	
	public static class DataStructures {
		static System.Random randomGenerator;

		public static void Shuffle<T>(this IList<T> ls, System.Random generator = null) {
			if (generator == null) {
				if (randomGenerator == null) { randomGenerator = new System.Random(); }
				generator = randomGenerator;
			}
			int n = ls.Count;
			while (n > 1) {
				n--;
				int k = generator.Next(n + 1);
				T value = ls[k];
				ls[k] = ls[n];
				ls[n] = value;
			}
		}

		public static T GetRandomElement<T>(this HashSet<T> hs, System.Random generator = null)	 {
			if (hs.Count > 0) {
				if (generator == null) {
					if (randomGenerator == null) { randomGenerator = new System.Random(); }
					generator = randomGenerator;
				}
				int r = generator.Next(0, hs.Count);
				foreach (T elem in hs)
					if (r-- <= 0)
						return elem;
			}
			return default(T);
		}

		public static T GetRandomElement<T>(this List<T> ls, System.Random generator = null) {
			if (ls.Count == 0) { return default(T); }
			if (generator == null) {
				if (randomGenerator == null) { randomGenerator = new System.Random(); }
				generator = randomGenerator;
			}
			return ls[generator.Next(0, ls.Count)];
		}

		public static KeyValuePair<U, T> GetRandomElement<U, T>(this Dictionary<U, T> ds, System.Random generator = null) {
			if (ds.Count > 0) {
				if (generator == null) {
					if (randomGenerator == null) { randomGenerator = new System.Random(); }
					generator = randomGenerator;
				}
				int r = generator.Next(0, ds.Count);
				foreach (var elem in ds)
					if (r-- <= 0)
						return elem;
			}
			return default(KeyValuePair<U, T>);
		}
		
		public static T GetRandomElement<T>(this T[] array, System.Random generator = null) {
			if (array.Length == 0) { return default(T); }
			if (generator == null) {
				if (randomGenerator == null) { randomGenerator = new System.Random(); }
				generator = randomGenerator;
			}
			return array[generator.Next(0, array.Length)];
		}

		public static T GetFirstElement<T>(this HashSet<T> hs) {
			foreach (T elem in hs)
				return elem;
			return default(T);
		}
	}

}