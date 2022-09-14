using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RatKing.Base {

	// use:
	// the config file must be in the asset folder (unity editor), in the root folder (windows)
	// TODO mac, linux
	// every key is/becomes lowercase
	// # is comment
	// standard separator is :
	// bool is true/false and on/off

	public class ConfigFile {
		static readonly HashSet<string> valuesDefine = new HashSet<string>();
		static readonly Dictionary<string, string> valuesString = new Dictionary<string, string>();
		static readonly Dictionary<string, bool> valuesBool = new Dictionary<string, bool>();
		static readonly Dictionary<string, float> valuesNumber = new Dictionary<string, float>();

		//

		public ConfigFile(string filename = "config.txt") : this(filename, '#', ":") {
		}


		public ConfigFile(string filename, char comment, string separator) {
#if UNITY_EDITOR
			var path = Application.dataPath + "/config.txt";
#else
			var path = Application.dataPath + "/../config.txt";
#endif
			float valueNumber;

			if (System.IO.File.Exists(path)) {
				var txt = System.IO.File.OpenText(path);
				var content = txt.ReadToEnd();
				var lines = content.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
				for (int l = 0; l < lines.Length; ++l) {
					var trimmedLine = lines[l].Trim();
					if (trimmedLine.Length == 0 || trimmedLine[0] == comment) { continue; }
					var line = trimmedLine.Split(new[] { separator }, System.StringSplitOptions.RemoveEmptyEntries);
					if (line.Length == 0 || line[0].Trim().Length == 0) {
						continue;
					}
					else if (line.Length == 1) {
						valuesDefine.Add(line[0].Trim().ToLower());
					}
					else {
						var key = line[0].Trim().ToLower();
						var value = trimmedLine.Substring(line[0].Length + separator.Length).Trim().ToLower();
						if (value == "on" || value == "true") {
							valuesBool[key] = true;
						}
						else if (value == "off" || value == "false") {
							valuesBool[key] = false;
						}
						else if (float.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out valueNumber)) {
							valuesNumber[key] = valueNumber;
						}
						else { // is string
							valuesString[key] = trimmedLine.Substring(line[0].Length + separator.Length);
						}
					}
				}
			}
		}

		//

		public bool IsDefined(string key) {
			return valuesDefine.Contains(key.ToLower());
		}

		public bool HasBool(string key) {
			return valuesBool.ContainsKey(key.ToLower());
		}
		public bool GetBool(string key, bool standard = false) {
			bool value;
			if (valuesBool.TryGetValue(key.ToLower(), out value)) { return value; }
			return standard;
		}

		public bool HasNumber(string key) {
			return valuesNumber.ContainsKey(key.ToLower());
		}
		public float GetNumber(string key, float standard = 0f) {
			float value;
			if (valuesNumber.TryGetValue(key.ToLower(), out value)) { return value; }
			return standard;
		}

		public bool HasString(string key) {
			return valuesString.ContainsKey(key.ToLower());
		}
		public string GetString(string key, string standard = "") {
			string value;
			if (valuesString.TryGetValue(key.ToLower(), out value)) { return value; }
			return standard;
		}
	}

}