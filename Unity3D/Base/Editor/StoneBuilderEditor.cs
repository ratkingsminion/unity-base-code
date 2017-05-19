using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;

// STONE BUILDER v0.5
// Standalone build editor for Unity
// 2015 (c) ratking (www.ratking.de)

// open Stone Builder via Tools -> Stone Builder

// TODO: don't pack if build was cancelled
// TODO: don't pack if target directory was not found (relevant for "Pack" button)
// TODO: clean target folder before build (because of removed additional files)
// TODO: also remove "additional files" and "levels" that are missing
// TODO: target format of packing should be choosable
// TODO: enable packing on mac, and even linux
// TODO: option for autoincreasing version number?

// changes 0.5
// added tooltips
// directory gets cleaned before build (because additional files might have been removed)

namespace RatKing.Base {

	[System.Serializable]
	public class StoneBuilderEditor : ScriptableObject {
		// saved in file (ie. gets versioned):
		public string gameShortName = "AppName";
		public string subfolderName = "GameFolder";
		[SerializeField,]
		public List<Object> levels = new List<Object>();
		[SerializeField]
		public Object[] includedFiles = new Object[0];
		// saved in playerprefs (ie. individual for each workstation):
		[System.NonSerialized]
		public string version = "0.0.1";
		[System.NonSerialized]
		public string buildPath = "C:/BUILDS";
		[System.NonSerialized]
		public List<bool> targetsActive = new List<bool>();
		[System.NonSerialized]
		public BuildOptions optionsMask;
		[System.NonSerialized]
		public bool openAfterBuild;
		[System.NonSerialized]
		public bool ignorePDB = true;
		[System.NonSerialized]
		public bool packWinBinsAs7Zip = true;
	}

	//
	//

	public class StoneBuilder : EditorWindow {
		class Target {
			public static int indexer;
			public int index;
			public string name;
			public string save;
			public string shortPath;
			public BuildTarget target;
			public string suffix;
			public static void Add(string name, string save, string shortPath, BuildTarget target, string suffix, bool active) {
				targets[save] = new Target() {
					index = indexer,
					name = name,
					save = save,
					shortPath = shortPath,
					target = target,
					suffix = suffix
				};
				if (settings != null && settings.targetsActive.Count < indexer + 1) {
					settings.targetsActive.Add(PlayerPrefs.GetInt(shortPath, active ? 1 : 0) == 1);
				}
				++indexer;
			}
			public bool GetActive() {
				return settings == null ? false : settings.targetsActive[index];
			}
			public void SetActive(bool active) {
				if (settings != null) {
					settings.targetsActive[index] = active;
					PlayerPrefs.SetInt(shortPath, active ? 1 : 0);
				}
			}
		}
		//
		static Dictionary<string, Target> targets = new Dictionary<string, Target>(8);
		static string[] targetNames;
		static readonly int optionsCountMax = System.Enum.GetNames(typeof(BuildOptions)).Length;
		static Vector2 scrollPosition;
		static string sevenZipPath = "C:\\Program Files\\7-Zip\\7z.exe";
		//
		static bool showTargets;
		[SerializeField] static StoneBuilderEditor settings;
		static SerializedObject settingsObj;

		//

		[MenuItem("Tools/Stone Builder")]
		static void Init() {
			targets.Clear();
			var w = GetWindow(typeof(StoneBuilder));
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
			w.titleContent = new GUIContent("Stone Builder");
#else
			w.title = "Stone Builder";
#endif
		}

		/// <summary>
		/// Get the saves settings
		/// </summary>
		static void GetSettings() {
			if (settingsObj != null) {
				return;
			}
			var findSettings = AssetDatabase.FindAssets("StoneBuilderSettings");
			if (findSettings == null || findSettings.Length == 0) {
				StoneBuilderEditor asset = ScriptableObject.CreateInstance<StoneBuilderEditor>();
				if (asset != null) {
					asset.name = "StoneBuilderSettings";
					if (!Directory.Exists(Application.dataPath + "/Resources")) {
						Directory.CreateDirectory(Application.dataPath + "/Resources");
					}
					AssetDatabase.CreateAsset(asset, "Assets/Resources/StoneBuilderSettings.asset");
					AssetDatabase.SaveAssets();
				}
				findSettings = AssetDatabase.FindAssets("StoneBuilderSettings");
			}
			if (findSettings != null && findSettings.Length > 0) {
				var path = AssetDatabase.GUIDToAssetPath(findSettings[0]);
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3  || UNITY_5_3_OR_NEWER
				settings = AssetDatabase.LoadAssetAtPath<StoneBuilderEditor>(path);
#else
				settings = (StoneBuilderEditor)AssetDatabase.LoadAssetAtPath(path, typeof(StoneBuilderEditor));
#endif
				if (settings != null) {
					settingsObj = new SerializedObject(settings);
					settingsObj.Update();
				}
			}
		}

		/// <summary>
		/// Get the build targets, and more
		/// </summary>
		static void GetTargets() {
			GetSettings();
			sevenZipPath = EditorPrefs.GetString("sevenzippath", sevenZipPath);
			if (settings != null) {
				settings.version = PlayerPrefs.GetString("version", settings.version);
				settings.buildPath = PlayerPrefs.GetString("buildpath", settings.buildPath);
				settings.optionsMask = (BuildOptions)PlayerPrefs.GetInt("optionsmask", (int)settings.optionsMask);
				settings.openAfterBuild = PlayerPrefs.GetInt("openafterbuild", settings.openAfterBuild ? 1 : 0) == 1;
				settings.ignorePDB = PlayerPrefs.GetInt("ignorepdb", settings.ignorePDB ? 1 : 0) == 1;
				settings.packWinBinsAs7Zip = PlayerPrefs.GetInt("packwinbinsas7z", settings.packWinBinsAs7Zip ? 1 : 0) == 1;
			}
			//
			targets.Clear();
			Target.indexer = 0;
			Target.Add("Windows 32 bit", "build win 32", "Win32", BuildTarget.StandaloneWindows, ".exe", true);
			Target.Add("Windows 64 bit", "build win 64", "Win64", BuildTarget.StandaloneWindows64, ".exe", false);
			Target.Add("Mac OSX 32 bit", "build osx 32", "OSX32", BuildTarget.StandaloneOSXIntel, ".exe", false);
			Target.Add("Mac OSX 64 bit", "build osx 64", "OSX64", BuildTarget.StandaloneOSXIntel64, ".app", true);
			Target.Add("Mac OSX Universal", "build osx uni", "OSX32+64", BuildTarget.StandaloneOSXUniversal, ".app", false);
			Target.Add("Ubuntu 32 bit", "build lnx 32", "Ubuntu32", BuildTarget.StandaloneLinux, "", false);
			Target.Add("Ubuntu 64 bit", "build lnx 64", "Ubuntu64", BuildTarget.StandaloneLinux64, "", true);
			Target.Add("Ubuntu Universal", "build lnx uni", "Ubuntu32+64", BuildTarget.StandaloneLinuxUniversal, "", false);
			//
			targetNames = new string[targets.Count];
			int i = 0;
			for (var iter = targets.GetEnumerator(); iter.MoveNext(); ++i) {
				targetNames[i] = iter.Current.Value.name;
			}
		}

		//

		/// <summary>
		/// Text area where you can write stuff. It saves the value
		/// </summary>
		void TextArea(string label, ref string value, string saveInEditorPrevs = "", string saveInPlayerPrevs = "") {
			GUILayout.BeginHorizontal();
			GUILayout.Label(label, GUILayout.Width(90), GUILayout.ExpandWidth(false));
			var newValue = GUILayout.TextField(value, GUILayout.Width(10), GUILayout.ExpandWidth(true));
			GUILayout.EndHorizontal();
			if (newValue.Trim() != value) {
				value = newValue.Trim();
				if (saveInEditorPrevs != "") {
					EditorPrefs.SetString(saveInEditorPrevs, value);
				}
				if (saveInPlayerPrevs != "") {
					PlayerPrefs.SetString(saveInPlayerPrevs, value);
				}
			}
		}

		void OnGUI() {
			if (targets == null || targets.Count == 0) {
				GetTargets();
			}
			if (settingsObj == null || settings == null) {
				GetSettings();
			}
			if (settings == null) {
				return;
			}

			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

			TextArea("App Name", ref settings.gameShortName);
			TextArea("Folder Name", ref settings.subfolderName);
			TextArea("Version", ref settings.version, "", "version");
			TextArea("Build path", ref settings.buildPath, "", "buildpath");

			EditorGUILayout.Space();

			// toggle boxes

			var countTrue = 0;
			for (var iter = targets.GetEnumerator(); iter.MoveNext();) {
				countTrue += iter.Current.Value.GetActive() ? 1 : 0;
			}
			
			showTargets = EditorGUILayout.Foldout(showTargets, "Builds targets (" + countTrue + "/" + targets.Count + ")");

			if (showTargets) {
				for (var iter = targets.GetEnumerator(); iter.MoveNext();) {
					var t = iter.Current.Value;
					GUILayout.BeginHorizontal();
					GUILayout.Label("", GUILayout.Width(20), GUILayout.ExpandWidth(false));
					var active = t.GetActive();
					if (GUILayout.Toggle(active, t.name, GUILayout.Width(90), GUILayout.ExpandWidth(true)) != active) {
						t.SetActive(active = !active);
						countTrue += active ? -1 : 1;
					}
					GUILayout.EndHorizontal();
				}
			}

			EditorGUILayout.Space();

			// choose levels
			
			//EditorGUIUtility.LookLikeInspector();
			settingsObj.Update();
			SerializedProperty levelsProp = settingsObj.FindProperty("levels");
			EditorGUI.BeginChangeCheck();
			var levelAssets = AssetDatabase.FindAssets("t:scene");
			EditorGUILayout.PropertyField(levelsProp, new GUIContent("Included levels (" + (settings.levels != null ? settings.levels.Count : 0) + "/" + levelAssets.Length + "):", "Levels that will be included in the builds. To add a file, drag it onto this label!"), true, null);
			if (EditorGUI.EndChangeCheck()) {
				settingsObj.ApplyModifiedProperties();
				// settings.levels.RemoveAll(o => o == null || !AssetDatabase.GetAssetPath(o).EndsWith(".unity"));
				if (settings.levels.RemoveAll(o => o == null) > 0) {
					// UnityEngine.Debug.Log("Add scenes by dragging them onto the label!");
				}
				if (settings.levels.RemoveAll(o => !AssetDatabase.GetAssetPath(o).EndsWith(".unity")) > 0) {
					UnityEngine.Debug.LogWarning("Only add Unity scenes to Included levels!");
				}
				settings.levels = new List<Object>(new HashSet<Object>(settings.levels));
			}
			//EditorGUIUtility.LookLikeControls();

			EditorGUILayout.Space();

			var optionsCount = 0;
			for (int i = 0; i < optionsCountMax; ++i) {
				if (((int)settings.optionsMask & (1 << i)) > 0) {
					optionsCount++;
				}
			}
			GUILayout.Label("Build options (" + optionsCount + "/" + optionsCountMax + "):");
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3  || UNITY_5_3_OR_NEWER
			var newOptionsMask = (BuildOptions)EditorGUILayout.EnumMaskPopup(GUIContent.none, settings.optionsMask);
#else
			var newOptionsMask = (BuildOptions)EditorGUILayout.EnumMaskField(settings.optionsMask);
#endif
			if (settings.optionsMask != newOptionsMask) {
				settings.optionsMask = newOptionsMask;
				PlayerPrefs.SetInt("optionsmask", (int)newOptionsMask);
			}

			EditorGUILayout.Space();

			var newOpenAfterBuild = GUILayout.Toggle(settings.openAfterBuild, "Open folder afterwards");
			if (settings.openAfterBuild != newOpenAfterBuild) {
				settings.openAfterBuild = newOpenAfterBuild;
				PlayerPrefs.SetInt("openafterbuild", newOpenAfterBuild ? 1 : 0);
			}
			
			EditorGUILayout.Space();

			// choose files to add

			//EditorGUIUtility.LookLikeInspector();
			SerializedProperty filesProp = settingsObj.FindProperty("includedFiles");
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(filesProp, new GUIContent("Additional files (" + (settings.includedFiles != null ? settings.includedFiles.Length : 0) + "):", "Files that will be added to the root folder after build, e.g. a readme.txt. To add a file, drag it onto this label!"), true, null);
			if (EditorGUI.EndChangeCheck()) {
				settingsObj.ApplyModifiedProperties();
				var newFiles = new List<Object>(settings.includedFiles);
				if (newFiles.RemoveAll(o => o == null) > 0) {
					//UnityEngine.Debug.Log("Add files by dragging them onto the label!");
				}
				settings.includedFiles = new List<Object>(new HashSet<Object>(newFiles)).ToArray();
			}
			//EditorGUIUtility.LookLikeControls();

			EditorGUILayout.Space();

			if (settings.buildPath == "" || settings.gameShortName == "" || settings.subfolderName == "" || settings.version == "") {
				var color = GUI.color;
				GUI.color = Color.red;
				EditorGUILayout.LabelField("WARNING", EditorStyles.whiteLabel);
				GUI.color = color;
				EditorGUILayout.LabelField("All text areas above need to be filled out!", EditorStyles.wordWrappedLabel, GUILayout.Height(100));
			}
			else {

				// build and run if only one
				if (countTrue == 1 && GUILayout.Button("Build and Run", GUILayout.Height(22f))) {
					for (var iter = targets.GetEnumerator(); iter.MoveNext();) {
						var t = iter.Current.Value;
						if (t.GetActive()) {
							string res = BuildGame(t.target, t.shortPath, t.suffix, settings.optionsMask);
							if (res != "") {
								CopyAdditionalFiles(res);
								Process proc = new Process();
								proc.StartInfo.FileName = res + settings.gameShortName + t.suffix;
								proc.Start();
							}
							OpenAfter(false, res);
							EditorGUILayout.EndScrollView();
							EditorGUIUtility.ExitGUI();
							return;
						}
					}
				}

				// build everything
				if (countTrue > 0 && GUILayout.Button(countTrue == 1 ? "Build" : "Build All", GUILayout.Height(22f))) {
					var startBuildTarget = EditorUserBuildSettings.activeBuildTarget;
					string res = "";
					for (var iter = targets.GetEnumerator(); iter.MoveNext();) {
						var t = iter.Current.Value;
						if (t.GetActive()) {
							res = BuildGame(t.target, t.shortPath, t.suffix, settings.optionsMask);
							if (res == "") {
								break;
							}
							CopyAdditionalFiles(res);
						}
					}
					OpenAfter(countTrue > 1, res);
#if UNITY_5_6_OR_NEWER
					EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, startBuildTarget);
#else
					EditorUserBuildSettings.SwitchActiveBuildTarget(startBuildTarget);
#endif
					EditorGUILayout.EndScrollView();
					EditorGUIUtility.ExitGUI();
					return;
				}

#if UNITY_EDITOR_WIN

				EditorGUILayout.Space();

				TextArea("Path of 7Zip", ref sevenZipPath, "sevenzippath");

				var newIgnorePDB = GUILayout.Toggle(settings.ignorePDB, new GUIContent("Don't include PDB files", "PDB files have symbol tables, ie. are useful for debugging purposes. Normally not needed when distributing builds to clients."));
				if (settings.ignorePDB != newIgnorePDB) {
					settings.ignorePDB = newIgnorePDB;
					PlayerPrefs.SetInt("ignorepdb", newIgnorePDB ? 1 : 0);
				}

				var newPackAs7Zip = GUILayout.Toggle(settings.packWinBinsAs7Zip, new GUIContent("Pack Windows binaries as .7z", ".7z packs a lot better than .zip, but not everybody owns a program being able to unpack it."));
				if (settings.packWinBinsAs7Zip != newPackAs7Zip) {
					settings.packWinBinsAs7Zip = newPackAs7Zip;
					PlayerPrefs.SetInt("packwinbinsas7z", newPackAs7Zip ? 1 : 0);
				}

				if (File.Exists(sevenZipPath)) {

					// pack (everything)
					if (countTrue > 0 && GUILayout.Button(countTrue == 1 ? "Pack" : "Pack All", GUILayout.Height(22f))) {
						// packing
						for (var iter = targets.GetEnumerator(); iter.MoveNext();) {
							var t = iter.Current.Value;
							if (t.GetActive()) {
								CopyAdditionalFiles(settings.buildPath + "/" + settings.gameShortName + "-" + settings.version + "/" + t.shortPath + "/" + settings.subfolderName + "/");
								PackGame(t.shortPath, t.target);
							}
						}
						OpenAfter(true, "..");
						EditorGUILayout.EndScrollView();
						EditorGUIUtility.ExitGUI();
						return;
					}

					// build (everything) and pack
					if (countTrue > 0 && GUILayout.Button(countTrue == 1 ? "Build and Pack" : "Build and Pack All", GUILayout.Height(22f))) {
						var startBuildTarget = EditorUserBuildSettings.activeBuildTarget;
						// building
						string res = "";
						for (var iter = targets.GetEnumerator(); iter.MoveNext();) {
							var t = iter.Current.Value;
							if (t.GetActive()) {
								res = BuildGame(t.target, t.shortPath, t.suffix, settings.optionsMask);
								if (res == "") {
									break;
								}
								CopyAdditionalFiles(res);
								PackGame(t.shortPath, t.target);
							}
						}
						OpenAfter(true, res != "" ? ".." : "");
#if UNITY_5_6_OR_NEWER
						EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, startBuildTarget);
#else
						EditorUserBuildSettings.SwitchActiveBuildTarget(startBuildTarget);
#endif
						EditorGUILayout.EndScrollView();
						EditorGUIUtility.ExitGUI();
						return;
					}
				}
#endif
					}

			EditorGUILayout.EndScrollView();
		}

		//

		static void CopyAdditionalFiles(string path) {
			for (int i = 0; i < settings.includedFiles.Length; ++i) {
				var assetPath = AssetDatabase.GetAssetPath(settings.includedFiles[i]);
				var fileName = Path.GetFileName(assetPath);
				var sourceName = Application.dataPath + "/../" + assetPath;
				FileUtil.CopyFileOrDirectory(sourceName, path + fileName);
				//File.Copy(sourceName, path + fileName, true);
				RemoveMetaFiles(path + fileName);
			}
		}

		static void RemoveMetaFiles(string dirName) {
			// remove meta files if necessary
			DirectoryInfo dir = new DirectoryInfo(dirName);
			if (dir.Exists) {
				FileInfo[] files = dir.GetFiles();
				foreach (FileInfo file in files) {
					if (Path.GetExtension(file.Name) == ".meta" || Path.GetExtension(file.Name) == ".gitignore") { FileUtil.DeleteFileOrDirectory(file.FullName); }
				}
				foreach (DirectoryInfo subdir in dir.GetDirectories()) {
					RemoveMetaFiles(subdir.FullName);
				}
			}
		}

		/// <summary>
		/// Open after building?
		/// </summary>
		/// <param name="openMainFolder"></param>
		/// <param name="path"></param>
		static void OpenAfter(bool openMainFolder, string path) {
			if (!settings.openAfterBuild || path == "") {
				return;
			}
			Process prc = new Process();
			if (!openMainFolder) {
				prc.StartInfo.FileName = Path.GetDirectoryName(path);
			}
			else {
				prc.StartInfo.FileName = Path.GetDirectoryName(settings.buildPath + "/" + settings.gameShortName + "-" + settings.version + "/");
			}
			prc.Start();
		}

		/// <summary>
		/// Pack the directories
		/// </summary>
		static void PackGame(string buildTargetName, BuildTarget target) {
			var format = "";
			var suffix = "";
			var isLinux = false;
			switch (target) {
				case BuildTarget.StandaloneWindows:
				case BuildTarget.StandaloneWindows64:
					if (settings.packWinBinsAs7Zip) {
						format = "-t7z";
						suffix = ".7z";
					}
					else {
						format = "";
						suffix = ".zip";
					}
					break;
				case BuildTarget.StandaloneOSXIntel:
				case BuildTarget.StandaloneOSXIntel64:
				case BuildTarget.StandaloneOSXUniversal:
					format = "";
					suffix = ".zip";
					break;
				case BuildTarget.StandaloneLinux:
				case BuildTarget.StandaloneLinux64:
				case BuildTarget.StandaloneLinuxUniversal:
					isLinux = true;
					format = "-ttar";
					suffix = ".tar";
					break;
				default:
					return;
			}

			var path = settings.buildPath + "/" + settings.gameShortName + "-" + settings.version + "/" + buildTargetName + "/";
			var packedFile = settings.gameShortName + "-" + settings.version + "-" + buildTargetName + suffix;
			var ignorePDB = settings.ignorePDB ? " -r -x!*.pdb" : "";

			// var info = new FileInfo("/Applications/TextEdit.app/Contents/MacOS/TextEdit");
			ProcessStartInfo info = new ProcessStartInfo("\"" + sevenZipPath + "\"");
			info.Arguments = "a " + format + " " + path + packedFile + " " + path + settings.subfolderName + "/" + ignorePDB;
			var proc = Process.Start(info);
			proc.WaitForExit();

			if (isLinux) {
				var newPackedFile = packedFile + ".gz";
				info = new ProcessStartInfo("\"" + sevenZipPath + "\"");
				info.Arguments = "a -tgzip " + path + newPackedFile + " " + path + packedFile + ignorePDB;
				proc = Process.Start(info);
				proc.WaitForExit();
				if (File.Exists(path + packedFile)) {
					File.Delete(path + packedFile);
				}
				packedFile = newPackedFile;
			}

			if (!File.Exists(path + "/" + packedFile)) {
				UnityEngine.Debug.Log("Could not pack " + target + ", build it first!");
			}
			var moveTo = settings.buildPath + "/" + settings.gameShortName + "-" + settings.version + "/" + packedFile;
			if (File.Exists(moveTo)) {
				File.Delete(moveTo);
			}
			File.Move(path + "/" + packedFile, moveTo);
		}

		/// <summary>
		/// Build the game
		/// </summary>
		/// <param name="buildTarget"></param>
		/// <param name="buildTargetName"></param>
		/// <param name="suffix"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static string BuildGame(BuildTarget buildTarget, string buildTargetName, string suffix, BuildOptions options) {
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
			if (!UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
#else
			if (!EditorApplication.SaveCurrentSceneIfUserWantsTo()) {
#endif
				return "";
			}

#if UNITY_5_3 || UNITY_5_3_OR_NEWER
			var openLevels = UnityEditor.SceneManagement.EditorSceneManager.GetSceneManagerSetup();
#else
			var openLevel = EditorApplication.currentScene;
#endif

			// path
			string path = settings.buildPath + "/" + settings.gameShortName + "-" + settings.version + "/" + buildTargetName + "/" + settings.subfolderName + "/";
			Directory.CreateDirectory(settings.buildPath + "/" + settings.gameShortName + "-" + settings.version);

			if (Directory.Exists(settings.buildPath + "/" + settings.gameShortName + "-" + settings.version + "/" + buildTargetName)) {
				// clean directory before building
				foreach (var d in Directory.GetDirectories(settings.buildPath + "/" + settings.gameShortName + "-" + settings.version + "/" + buildTargetName)) {
					Directory.Delete(d, true);
				}
				foreach (var f in Directory.GetFiles(settings.buildPath + "/" + settings.gameShortName + "-" + settings.version + "/" + buildTargetName)) {
					File.Delete(f);
				}
			}

			Directory.CreateDirectory(settings.buildPath + "/" + settings.gameShortName + "-" + settings.version + "/" + buildTargetName);

			// BUILD
			var res = "";
			Directory.CreateDirectory(path);
			var levelPaths = new string[settings.levels.Count];
			for (int i = 0; i < levelPaths.Length; ++i) {
				levelPaths[i] = AssetDatabase.GetAssetPath(settings.levels[i]);
			}
			res = BuildPipeline.BuildPlayer(levelPaths, path + settings.gameShortName + suffix, buildTarget, options);

			if (res != "") {
				UnityEngine.Debug.Log(res);
				return "";
			}

#if UNITY_5_3 || UNITY_5_3_OR_NEWER
			UnityEditor.SceneManagement.EditorSceneManager.RestoreSceneManagerSetup(openLevels);
#else
			if (EditorApplication.currentScene != openLevel) {
				EditorApplication.OpenScene(openLevel);
			}
#endif

			return path; // + gameShortName + suffix;
		}
	}
}