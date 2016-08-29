using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base.Helpers {
	
	public static class Colors {
		public static Color GetFromHexa(string hexCode) {
			// 0 should be "#"
			int r = int.Parse(hexCode.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
			int g = int.Parse(hexCode.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
			int b = int.Parse(hexCode.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
			return new Color((float)r / 255f, (float)g / 255f, (float)b / 255f, 1f);
		}
		//
		public static bool Approx(this Color c1, Color c2, float epsilon = 0.01f) {
			return c1.r < c2.r + epsilon && c1.r > c2.r - epsilon &&
					c1.g < c2.g + epsilon && c1.g > c2.g - epsilon &&
					c1.b < c2.b + epsilon && c1.b > c2.b - epsilon;
		}
		//
		// from http://www.cs.rit.edu/~ncs/color/t_convert.html
		// r,g,b values are from 0 to 1 | h = [0,360], s = [0,1], v = [0,1] | if s == 0, then h = -1 (undefined)
		public static void RGBtoHSV(float r, float g, float b, out float h, out float s, out float v) {
			float min, max, delta;

			float[] rgb = new float[] { r, g, b };
			min = Mathf.Min(rgb);
			max = Mathf.Max(rgb);
			v = max;

			delta = max - min;

			if (max != 0f) {
				s = delta / max;
			}
			else {
				s = 0f;
				h = -1f;
				return;
			}

			if (r == max)
				h = (g - b) / delta;
			else if (g == max)
				h = 2f + (b - r) / delta;
			else
				h = 4f + (r - g) / delta;

			h *= 60f;
			if (h < 0f)
				h += 360f;
		}
		public static void HSVtoRGB(out float r, out float g, out float b, float h, float s, float v) {
			int i;
			float f, p, q, t;

			if (s == 0f) {
				r = g = b = v;
				return;
			}

			h /= 60f;
			i = Mathf.FloorToInt(h);
			f = h - i;
			p = v * (1f - s);
			q = v * (1f - s * f);
			t = v * (1f - s * (1f - f));

			switch (i) {
				case 0: r = v; g = t; b = p; break;
				case 1: r = q; g = v; b = p; break;
				case 2: r = p; g = v; b = t; break;
				case 3: r = p; g = q; b = v; break;
				case 4: r = t; g = p; b = v; break;
				default: r = v; g = p; b = q; break;
			}
		}
		public static Color WithAlpha(this Color c, float a) {
			return new Color(c.r, c.g, c.b, a);
		}
    }

}