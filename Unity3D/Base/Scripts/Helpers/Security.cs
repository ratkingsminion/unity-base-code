using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base.Helpers {
	
	public static class Security {
		public static string MD5Sum(string strToEncrypt) {
			var ue = new System.Text.UTF8Encoding();
			byte[] bytes = ue.GetBytes(strToEncrypt);

			// encrypt bytes
			var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
			byte[] hashBytes = md5.ComputeHash(bytes);

			// Convert the encrypted bytes back to a string (base 16)
			string hashString = "";

			for (int i = 0; i < hashBytes.Length; i++) {
				hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
			}

			return hashString.PadLeft(32, '0');
		}
	}

}