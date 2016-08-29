using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base.Helpers {

	public static class Randomness {
		public static Color GetColor(bool randomAlpha, float alpha = 1f) {
			return new Color(Random.value, Random.value, Random.value, randomAlpha ? Random.value : alpha);
		}

		public static class Probabilities {
			public static int GetRandomIndex(float[] probabilites) {
				float r = Random.value;
				int i = 0;
				for (; i < probabilites.Length; ++i) {
					if (r < probabilites[i])
						break;
				}
				return i;
			}
			public static float[] Normalize(int[] probabilities) {
				float sum = 0f;
				float[] newProbs = new float[probabilities.Length];
				for (int i = 0; i < probabilities.Length; ++i)
					sum += (float)probabilities[i];
				for (int i = 0; i < probabilities.Length; ++i)
					newProbs[i] = ((float)probabilities[i] / sum);
				for (int i = 1; i < probabilities.Length; ++i)
					newProbs[i] += newProbs[i - 1];
				newProbs[probabilities.Length - 1] = 1f; // to be sure.
				return newProbs;
			}
		}
		//
		public static class SimplexNoise {
			// from: http://stephencarmody.wikispaces.com/Simplex+Noise

			static int i, j, k;
			static int[] A = new int[] { 0, 0, 0 };
			static float u, v, w, s;
			static float onethird = 0.333333333f;
			static float onesixth = 0.166666667f;
			static int[] T = new int[] { 0x15, 0x38, 0x32, 0x2c, 0x0d, 0x13, 0x07, 0x2a };

			//

			public static float NoiseLoop(float x, float y, float t, float T) {
				return ((T - t) * Noise(x, y, t) + t * Noise(x, y, t - T)) / T;
			}

			// returns a value in the range of about [-0.347 .. 0.347]
			public static float Noise(float x, float y, float z) {
				// Skew input space to relative coordinate in simplex cell
				s = (x + y + z) * onethird;
				i = Fastfloor(x + s);
				j = Fastfloor(y + s);
				k = Fastfloor(z + s);

				// Unskew cell origin back to (x, y , z) space
				s = (i + j + k) * onesixth;
				u = x - i + s;
				v = y - j + s;
				w = z - k + s; ;

				A[0] = A[1] = A[2] = 0;

				// For 3D case, the simplex shape is a slightly irregular tetrahedron.
				// Determine which simplex we're in
				int hi = u >= w ? u >= v ? 0 : 1 : v >= w ? 1 : 2;
				int lo = u < w ? u < v ? 0 : 1 : v < w ? 1 : 2;

				return k_m(hi) + k_m(3 - hi - lo) + k_m(lo) + k_m(0);
			}

			// normalized (goes [0..1], but only circa!)
			public static float NormalizedNoise(float x, float y, float z) {
				return (Noise(x, y, z) + 0.347f) * 1.4409f;
			}

			// normalized (goes [-1..1], but only circa!)
			public static float NoiseMinusPlus1(float x, float y, float z) {
				return Noise(x, y, z) * 1.4409f;
			}

			//

			static int Fastfloor(float n) {
				return n > 0 ? (int)(n) : (int)(n - 1);
			}

			static float k_m(int a) {
				s = (A[0] + A[1] + A[2]) * onesixth;
				float x = u - A[0] + s;
				float y = v - A[1] + s;
				float z = w - A[2] + s;
				float t = 0.6f - x * x - y * y - z * z;
				int h = Shuffle(i + A[0], j + A[1], k + A[2]);
				A[a]++;
				if (t < 0) return 0;
				int b5 = h >> 5 & 1;
				int b4 = h >> 4 & 1;
				int b3 = h >> 3 & 1;
				int b2 = h >> 2 & 1;
				int b = h & 3;
				float p = b == 1 ? x : b == 2 ? y : z;
				float q = b == 1 ? y : b == 2 ? z : x;
				float r = b == 1 ? z : b == 2 ? x : y;
				p = b5 == b3 ? -p : p;
				q = b5 == b4 ? -q : q;
				r = b5 != (b4 ^ b3) ? -r : r;
				t *= t;
				return 8 * t * t * (p + (b == 0 ? q + r : b2 == 0 ? q : r));
			}

			static int Shuffle(int i, int j, int k) {
				return b_m(i, j, k, 0) + b_m(j, k, i, 1) + b_m(k, i, j, 2) + b_m(i, j, k, 3) +
						b_m(j, k, i, 4) + b_m(k, i, j, 5) + b_m(i, j, k, 6) + b_m(j, k, i, 7);
			}

			static int b_m(int i, int j, int k, int B) {
				return T[b2_m(i, B) << 2 | b2_m(j, B) << 1 | b2_m(k, B)];
			}

			static int b2_m(int N, int B) {
				return N >> B & 1;
			}
		}
	}

}