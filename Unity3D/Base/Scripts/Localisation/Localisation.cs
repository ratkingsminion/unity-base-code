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
		[System.NonSerialized] public string fileContent;
	}

	public class Localisation : MonoBehaviour {

		public static Base.Event LANGUAGE_CHANGES = new Base.Event("language_changes");

		public static Localisation Inst { get; private set; }
		//
		[SerializeField] int keyPartCount = 16; // 16 should be more than enough anyway
		[SerializeField, Tooltip("Optional: enter a definition file (JSON) that will define what languages exist")] string definitionFileName = "";
		[SerializeField, Tooltip("If you don't have a definition file you need to add all languages here.")] List<LocalisationLanguage> languages = null;
		//
		public static List<LocalisationLanguage> Languages { get { return Inst.languages; } }
		//
		static int curLangIndex = 0;
		static Dictionary<string, string> texts = new Dictionary<string, string>();
		static Dictionary<string, string[]> textsAll = new Dictionary<string, string[]>();
		static List<ILocaliseMe> locas = new List<ILocaliseMe>(32);
		//
		readonly char[] keyTrimmer = new[] { '\\', '/', '\n', '\r', '\t', '"', ' ' };
		bool addedDefinitions = false;

		//

		void Awake() {
			Inst = this;
			InitTranslations(0);
			TranslateAll();

			// for testing - create a file with all keys and texts
			//var test = "";
			//foreach (var k in texts) { test += k.Value + "\n"; }
			//foreach (var k in textsAll) { foreach (var v in k.Value) { test += v + "\n"; } }
			//var filepath = Application.dataPath + "/ALL_TEXTS.txt";
			//System.IO.File.WriteAllText(filepath, test);
		}

		void OnDestroy() {
			curLangIndex = 0;
			texts.Clear();
			textsAll.Clear();
		}

		void InitTranslations(int index) {
			var hasDefinitionFile = !string.IsNullOrWhiteSpace(definitionFileName);
			if (!hasDefinitionFile && (languages == null || languages.Count == 0)) {
				Debug.LogError("No translations present");
				return;
			}
			//
			SimpleJSON.JSONNode json;
			if (hasDefinitionFile && !addedDefinitions) {
				if (languages == null) { languages = new List<LocalisationLanguage>(); }
				var definitionString = System.IO.File.ReadAllText(Application.dataPath + "/" + definitionFileName);
				json = SimpleJSON.JSONNode.Parse(definitionString);
				if (!json.IsObject) { Debug.LogError("Malformed translations definition file"); return; }
				foreach (var key in json.Keys) {
					var jsonLang = json[key];
					if (!jsonLang.IsObject) { Debug.LogWarning("Translation object " + key + " is malformed (" + jsonLang.ToString() + ")"); continue; }
					var fileName = jsonLang["file"].Value;
					var filePath = Application.dataPath + "/" + fileName;
					if (!System.IO.File.Exists(filePath)) { Debug.LogWarning("Translation file for " + key + " does not exist"); continue; }
					var fileContent = System.IO.File.ReadAllText(filePath);
					if (string.IsNullOrWhiteSpace(fileContent)) { Debug.LogWarning("Translation file for " + key + " is malformed"); continue; }
					languages.Add(new LocalisationLanguage() {
						language = (SystemLanguage)System.Enum.Parse(typeof(SystemLanguage), key, true),
						name = jsonLang["name"].Value,
						code = jsonLang["code"].Value,
						file = null,
						fileContent = fileContent
					});
				}
				addedDefinitions = true;
			}
			//
			var curKeyParts = new string[keyPartCount];
			var language = languages[index];
			json = SimpleJSON.JSONNode.Parse(language.file == null ? language.fileContent : language.file.text);
			if (json.IsObject) {
				GetTranslationKeys(curKeyParts, 0, json);
			}
		}

		void GetTranslationKeys(string[] curKeyParts, int keyPartsCount, SimpleJSON.JSONNode field) {
			if (field.IsObject) {
				foreach (var key in field.Keys) {
					curKeyParts[keyPartsCount] = key;
					GetTranslationKeys(curKeyParts, keyPartsCount + 1, field[key]);
				}
			}
			else if (field.IsArray) {
				var curKey = string.Join("/", curKeyParts, 0, keyPartsCount).Trim(keyTrimmer);
				var list = new List<string>(field.AsArray.Count);
				foreach (var sub in field.AsArray) {
					if (sub.Value.IsString) { list.Add(sub.Value); }
				}
				if (list.Count > 0) {
					textsAll[curKey] = list.ToArray();
					texts[curKey] = list[0];
				}
			}
			else if (field.IsString) {
				var curKey = string.Join("/", curKeyParts, 0, keyPartsCount).Trim(keyTrimmer);
				texts[curKey] = field.Value;
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
			var index = Inst.languages.FindLastIndex(l => l.code == code);
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