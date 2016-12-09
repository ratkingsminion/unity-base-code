using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base.Helpers {

	public static class String {
		public static char[] linesSplitter = new char[1] { '\n' };
		public static char[] spaceSplitter = new char[5] { ' ', '\n', '\t', '\r', '\0' };
		//
		public static string DisplayScore(int score, int digits = 7) {
			/*
			string text = string.Empty;
			for (int i = (int)Mathf.Pow(10, digits - 1); i > 1; i /= 10)
				text += (score < i) ? "0" : "";
			return text + score;
			*/
			string text = string.Empty;
			int ts = score;
			do {
				ts /= 10;
				digits--;
			} while (ts > 0);
			for (int i = digits - 1; i >= 0; --i)
				text += "0";
			return text + score;
		}
		public static string DisplayMinutes(int seconds) {
			int s = seconds % 60;
			int m = seconds / 60;
			return m.ToString() + ":" + (s < 10 ? ("0" + s) : s.ToString());
		}

		public static string CreateID(int numBlocks = 4, int lengthBlock = 4, string delimiter = "-", string possibleCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYabcdefghijklmnopqrstuvwxyzZ1234567890") {
			string code = "";
			for (int i = 0; i < numBlocks; ++i) {
				if (i != 0) code += delimiter;
				for (int j = 0; j < lengthBlock; ++j)
					code += possibleCharacters[UnityEngine.Random.Range(0, possibleCharacters.Length)].ToString();
			}
			return code;
		}

		public static string Escape(this string text) {
			return text.Replace("\\", "\\\\").Replace("\r\n", "\\n").Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t");
		}
	}

}