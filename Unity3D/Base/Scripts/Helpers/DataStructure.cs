using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base.Helpers {
	
	public static class DataStructure {
		static System.Random randomGenerator;
		public static void Shuffle<T>(this IList<T> ls) {
			if (randomGenerator == null)
				randomGenerator = new System.Random();
			int n = ls.Count;
			while (n > 1) {
				n--;
				int k = randomGenerator.Next(n + 1);
				T value = ls[k];
				ls[k] = ls[n];
				ls[n] = value;
			}
		}
		public static T GetRandomElement<T>(this HashSet<T> hs) {
			int r = Random.Range(0, hs.Count);
			foreach (T elem in hs)
				if (r-- <= 0)
					return elem;
			return default(T);
		}

		public static T GetRandomElement<T>(this List<T> ls) {
			return ls[Random.Range(0, ls.Count)];
		}

		public static KeyValuePair<U, T> GetRandomElement<U, T>(this Dictionary<U, T> ds) {
			int r = Random.Range(0, ds.Count);
			foreach (var elem in ds)
				if (r-- <= 0)
					return elem;
			return default(KeyValuePair<U, T>);
		}
		public static T GetFirstElement<T>(this HashSet<T> hs) {
			foreach (T elem in hs)
				return elem;
			return default(T);
		}
	}

}