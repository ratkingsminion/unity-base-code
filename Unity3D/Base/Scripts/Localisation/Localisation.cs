using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base {

	[System.Serializable]
	public struct LocalisationLanguage {
		public SystemLanguage language;
		public string name;
		public string code;
		public TextAsset file;
	}

	public class Localisation : MonoBehaviour {

		public static Base.Event LANGUAGE_CHANGES = new Base.Event("language_changes");

		public static Localisation Inst { get; private set; }
		//
		[SerializeField] List<LocalisationLanguage> languages = null;
		//
		static int curLangIndex = 0;
		static Dictionary<string, string> texts = new Dictionary<string, string>();
		static Dictionary<string, string[]> textsAll = new Dictionary<string, string[]>();
		static List<ILocaliseMe> locas = new List<ILocaliseMe>(32);
		//
		readonly char[] keyTrimmer = new[] { '\\', '/', '\n', '\r', '\t', '"', ' ' };

		//

		void Awake() {
			Inst = this;
			InitTranslations(0);
			TranslateAll();

			//var test = "";
			//foreach (var k in texts) {
			//	test += k.Value + "\n";
			//}
			//foreach (var k in textsAll) {
			//	foreach (var v in k.Value) {
			//		test += v + "\n";
			//	}
			//}
			//var filepath = Application.dataPath + "/ALL_TEXTS.txt";
			//System.IO.File.WriteAllText(filepath, test);
		}

		void OnDestroy() {
			curLangIndex = 0;
			texts.Clear();
			textsAll.Clear();
		}

		void InitTranslations(int index) {
			var curKeyParts = new string[16]; // 16 should be more than enough...
			var json = SimpleJSON.JSONNode.Parse(languages[index].file.text);
			if (json.IsObject && json["translations"] != null) { GetTranslationKeys(curKeyParts, 0, json["translations"]); }
		}

		void GetTranslationKeys(string[] curKeyParts, int keyPartsCount, SimpleJSON.JSONNode curField) {
			if (curField.IsObject) {
				foreach (var key in curField.Keys) {
					curKeyParts[keyPartsCount] = key;
					GetTranslationKeys(curKeyParts, keyPartsCount + 1, curField[key]);
				}
			}
			else if (curField.IsArray) {
				var curKey = string.Join("/", curKeyParts, 0, keyPartsCount).Trim(keyTrimmer);
				var list = new List<string>(curField.AsArray.Count);
				foreach (var sub in curField.AsArray) {
					if (sub.Value.IsString) { list.Add(sub.Value); }
				}
				if (list.Count > 0) {
					textsAll[curKey] = list.ToArray();
					texts[curKey] = list[0];
				}
			}
			else if (curField.IsString) {
				var curKey = string.Join("/", curKeyParts, 0, keyPartsCount).Trim(keyTrimmer);
				texts[curKey] = curField.Value;
			}
		}

		//

		public static int GetCurLanguageIndex() {
			return curLangIndex;
		}

		public static SystemLanguage GetCurLanguage() {
			return Inst.languages[curLangIndex].language;
		}

		public static void ChangeLanguage(string code) {
			var index = Inst.languages.FindIndex(l => l.code == code);
			if (index < 0) { Debug.LogWarning("Language " + code + " not found!"); }
			else { ChangeLanguage(index); }
		}

		public static void ChangeLanguage(int index) {
			if (Inst == null || curLangIndex == index) { return; }
			Inst.InitTranslations(index);
			curLangIndex = index;
			Base.Events.BroadcastAll(LANGUAGE_CHANGES);
			Inst.TranslateAll();
		}

		//

		public static void Register(ILocaliseMe loca) {
			if (locas.Contains(loca)) { Debug.LogWarning("trying to add ui to localisation more than once!"); return; }
			locas.Add(loca);
			if (Inst == null) { return; } // wait for start, do translate later
			loca.TranslateMe();
		}

		public static void Unregister(ILocaliseMe loca) {
#if UNITY_EDITOR
			if (!locas.Contains(loca)) { Debug.LogWarning("trying to unregister ui from localisation without it being registered!"); return; }
#endif
			locas.Remove(loca);
		}

		public static string Do(string key, bool convertSpecial = true) {
#if UNITY_EDITOR
			if (texts.Count == 0) {  Debug.LogWarning("trying to get key " + key + " before localisation inited!"); }
#endif
			string text;
			//Debug.Log("try to get <" + key + "> " + texts.Count);
			if (texts.TryGetValue(key, out text)) {
				if (convertSpecial) { return text.Replace("\\n", "\n").Replace("\\t", "\t"); }
				return text;
			}
			return "error '" + key + "' not found!";
		}

		public static string[] DoAll(string key) {
#if UNITY_EDITOR
			if (textsAll.Count == 0) {  Debug.LogWarning("trying to get key " + key + " before localisation inited!"); }
#endif
			string[] texts;
			//Debug.Log("try to get <" + key + "> " + texts.Count);
			if (textsAll.TryGetValue(key, out texts)) {
				return texts;
			}
			return null;
		}

		public static string ConvertSpecial(string text) {
			return text.Replace("\\n", "\n").Replace("\\t", "\t");
		}

		public static void Set(string key, string value) {
			texts[key] = value;
			if (textsAll.ContainsKey(key)) { textsAll[key][0] = value; }
		}

		//

		void TranslateAll() {
			foreach (var loca in locas) {
				loca.TranslateMe();
			}
		}
	}

}