#if UNITY_2_6 || UNITY_2_6_1 || UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
	#define UNITY_OLD
#else
	#define UNITY_5
#endif

#if UNITY_5 && !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2
	#define HAS_SCENEMANAGER
#endif

using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;

// STONE BUILDER v0.6
// Standalone build editor for Unity
// 2015-2017 (c) ratking (www.ratking.de)

// open Stone Builder via Tools -> Stone Builder

// TODO: don't pack if build was cancelled
// TODO: don't pack if target directory was not found (relevant for "Pack" button)
// TODO: also remove "additional files" and "levels" that are missing
// TODO: enable packing on mac, and even linux
// TODO: option for autoincreasing version number?

// changes 0.6
//	 can create .bat file now
//	 can create itch.io build script now
//	 can add files/folders into their correct sub-dir now
// changes 0.5
//	 added tooltips
//	 directory gets cleaned before build (because additional files might have been removed)

namespace RatKing.Base {

	public class StoneBuilder : EditorWindow {
		class Target {
			public static int indexer;
			public int index;
			public string name;
			public string save;
			public string shortPath;
			public string itchName;
			public int itchVal;
			public BuildTarget target;
			public string suffix;
			public static void Add(string name, string save, string shortPath, string itchName, int itchVal, BuildTarget target, string suffix, bool active) {
				targets[save] = new Target() {
					index = indexer,
					name = name,
					save = save,
					shortPath = shortPath,
					itchName = itchName,
					itchVal = itchVal,
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
		[SerializeField] static StoneBuilderSettings settings;
		static SerializedObject settingsObj;

		//

		[MenuItem("Tools/Stone Builder")]
		static void Init() {
			targets.Clear();
			var w = GetWindow(typeof(StoneBuilder));
#if UNITY_5 || UNITY_2017_1_OR_NEWER
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
			var findSettings = AssetDatabase.FindAssets("StoneBuilderSavedSettings");
			if (findSettings == null || findSettings.Length == 0) {
				StoneBuilderSettings asset = CreateInstance<StoneBuilderSettings>();
				if (asset != null) {
					asset.name = "StoneBuilderSavedSettings";
					if (!Directory.Exists(Application.dataPath + "/Resources")) {
						Directory.CreateDirectory(Application.dataPath + "/Resources");
					}
					AssetDatabase.CreateAsset(asset, "Assets/Resources/StoneBuilderSavedSettings.asset");
					AssetDatabase.SaveAssets();
				}
				findSettings = AssetDatabase.FindAssets("StoneBuilderSavedSettings");
			}
			if (findSettings != null && findSettings.Length > 0) {
				var path = AssetDatabase.GUIDToAssetPath(findSettings[0]);
#if UNITY_5 || UNITY_2017_1_OR_NEWER
				settings = AssetDatabase.LoadAssetAtPath<StoneBuilderSettings>(path);
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
				settings.buildPath = PlayerPrefs.GetString("buildpath", settings.buildPath);
				settings.optionsMask = (BuildOptions)PlayerPrefs.GetInt("optionsmask", (int)settings.optionsMask);
				settings.openAfterBuild = PlayerPrefs.GetInt("openafterbuild", settings.openAfterBuild ? 1 : 0) == 1;
				settings.ignorePDB = PlayerPrefs.GetInt("ignorepdb", settings.ignorePDB ? 1 : 0) == 1;
				settings.packWinBinsAs7Zip = PlayerPrefs.GetInt("packwinbinsas7z", settings.packWinBinsAs7Zip ? 1 : 0) == 1;
			}
			//
			targets.Clear();
			Target.indexer = 0;
			Target.Add("Windows 32 bit", "build win 32", "Win32", "windows", 0, BuildTarget.StandaloneWindows, ".exe", true);
			Target.Add("Windows 64 bit", "build win 64", "Win64", "windows", 1, BuildTarget.StandaloneWindows64, ".exe", false);
#if !UNITY_2017_3_OR_NEWER
			Target.Add("Mac OSX 32 bit", "build osx 32", "OSX32", "osx", 0, BuildTarget.StandaloneOSXIntel, ".exe", false);
			Target.Add("Mac OSX 64 bit", "build osx 64", "OSX64", "osx", 1, BuildTarget.StandaloneOSXIntel64, ".app", true);
#endif
			Target.Add("Mac OSX", "build osx uni", "OSX", "osx-universal", 0, BuildTarget.StandaloneOSX, ".app", false);
#if !UNITY_2019_2_OR_NEWER
			Target.Add("Ubuntu 32 bit", "build lnx 32", "Ubuntu32", "linux", 0, BuildTarget.StandaloneLinux, "", false);
			Target.Add("Ubuntu 64 bit", "build lnx 64", "Ubuntu64", "linux", 1, BuildTarget.StandaloneLinux64, "", true);
			Target.Add("Ubuntu Universal", "build lnx uni", "Ubuntu32+64", "linux-universal", 0, BuildTarget.StandaloneLinuxUniversal, "", false);
#else
			Target.Add("Linux 64 bit", "build lnx 64", "Linux", "linux", 1, BuildTarget.StandaloneLinux64, "", true);
#endif
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
			//GUILayout.Label(label, GUILayout.Width(90), GUILayout.ExpandWidth(false));
			//var newValue = GUILayout.TextField(string.IsNullOrEmpty(value) ? "" : value, GUILayout.Width(10), GUILayout.ExpandWidth(true));
			var newValue = EditorGUILayout.TextField(label, string.IsNullOrEmpty(value) ? "" : value);
			GUILayout.EndHorizontal();
			if (newValue.Trim() != value) {
				value = newValue.Trim();
				if (saveInEditorPrevs != "") {
					EditorPrefs.SetString(saveInEditorPrevs, value);
				}
				if (saveInPlayerPrevs != "") {
					PlayerPrefs.SetString(saveInPlayerPrevs, value);
				}
				else {
					EditorUtility.SetDirty(settings);
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
			
			EditorGUI.BeginChangeCheck();
			//TextArea("App Name", ref settings.gameShortName);
			//TextArea("Folder Name", ref settings.subfolderName);
			//TextArea("Version", ref settings.version);
			EditorGUILayout.PropertyField(settingsObj.FindProperty("gameShortName"), new GUIContent("App Name"));
			EditorGUILayout.PropertyField(settingsObj.FindProperty("subfolderName"), new GUIContent("Folder Name"));
			EditorGUILayout.PropertyField(settingsObj.FindProperty("version"), new GUIContent("Version"));
			if (EditorGUI.EndChangeCheck()) {
				settingsObj.ApplyModifiedProperties();
			}
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
					//GUILayout.Label("", GUILayout.Width(20), GUILayout.ExpandWidth(false));
					var active = t.GetActive();
					//if (GUILayout.Toggle(active, " " + t.name, GUILayout.Width(90), GUILayout.ExpandWidth(true)) != active) {
					if (EditorGUILayout.Toggle("     " + t.name, active) != active) {
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
#if UNITY_2017_1_OR_NEWER
			var newOptionsMask = (BuildOptions)EditorGUILayout.EnumFlagsField(GUIContent.none, settings.optionsMask);
#elif UNITY_5
			var newOptionsMask = (BuildOptions)EditorGUILayout.EnumMaskPopup(GUIContent.none, settings.optionsMask);
#else
			var newOptionsMask = (BuildOptions)EditorGUILayout.EnumMaskField(settings.optionsMask);
#endif
			if (settings.optionsMask != newOptionsMask) {
				settings.optionsMask = newOptionsMask;
				PlayerPrefs.SetInt("optionsmask", (int)newOptionsMask);
			}

			EditorGUILayout.Space();

			//var newOpenAfterBuild = GUILayout.Toggle(settings.openAfterBuild, " Open folder afterwards");
			var newOpenAfterBuild = EditorGUILayout.Toggle("Open folder afterwards", settings.openAfterBuild);
			if (settings.openAfterBuild != newOpenAfterBuild) {
				settings.openAfterBuild = newOpenAfterBuild;
				PlayerPrefs.SetInt("openafterbuild", newOpenAfterBuild ? 1 : 0);
			}
			
			EditorGUILayout.Space();

			// choose files to add

			SerializedProperty filesProp = settingsObj.FindProperty("includedFiles");
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(filesProp, new GUIContent("Additional files to root (" + (settings.includedFiles != null ? settings.includedFiles.Length : 0) + "):", "Files or dirs that will be added to the root folder after build, e.g. a readme.txt. To add a file or dir, drag it onto this label!"), true, null);
			if (EditorGUI.EndChangeCheck()) {
				settingsObj.ApplyModifiedProperties();
				var newFiles = new List<Object>(settings.includedFiles);
				if (newFiles.RemoveAll(o => o == null) > 0) {
					//UnityEngine.Debug.Log("Add files by dragging them onto the label!");
				}
				settings.includedFiles = new List<Object>(new HashSet<Object>(newFiles)).ToArray();
				
				//var assetPath = AssetDatabase.GetAssetPath(settings.includedFiles[1]);
				//UnityEngine.Debug.Log(assetPath);
				//UnityEngine.Debug.Log(Application.dataPath);
			}

			SerializedProperty filesWDSProp = settingsObj.FindProperty("includedFilesWithDirStruct");
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(filesWDSProp, new GUIContent("Additional files to original folder (" + (settings.includedFilesWithDirStruct != null ? settings.includedFilesWithDirStruct.Length : 0) + "):", "Files or dirs that will be added to their original folder after build. To add a file or dir, drag it onto this label!"), true, null);
			if (EditorGUI.EndChangeCheck()) {
				settingsObj.ApplyModifiedProperties();
				var newFiles = new List<Object>(settings.includedFilesWithDirStruct);
				newFiles.RemoveAll(o => o == null);
				settings.includedFilesWithDirStruct = new List<Object>(new HashSet<Object>(newFiles)).ToArray();
			}
			
			EditorGUILayout.Space();
			
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.LabelField("Batch Script");
			//settings.script = EditorGUILayout.TextArea(settings.script);
			EditorGUILayout.PropertyField(settingsObj.FindProperty("script"), GUIContent.none);
			if (EditorGUI.EndChangeCheck()) {
				settingsObj.ApplyModifiedProperties();
			}

			// itch io

			EditorGUILayout.Space();

			EditorGUI.BeginChangeCheck();
			//settings.createItchBat = GUILayout.Toggle(settings.createItchBat, "itch.io build script");
			EditorGUILayout.PropertyField(settingsObj.FindProperty("createItchBat"), new GUIContent("itch.io build script"));
			if (settings.createItchBat) {
				//TextArea("   User Name", ref settings.itchUsername);
				//TextArea("   Game Name", ref settings.itchGamename);
				//TextArea("   Add Tags", ref settings.itchAdditionalTags);
				EditorGUILayout.PropertyField(settingsObj.FindProperty("itchUsername"), new GUIContent("   User Name"));
				EditorGUILayout.PropertyField(settingsObj.FindProperty("itchGamename"), new GUIContent("   Game Name"));
				EditorGUILayout.PropertyField(settingsObj.FindProperty("itchAdditionalTags"), new GUIContent("   Add Tags"));
			}
			if (EditorGUI.EndChangeCheck()) {
				settingsObj.ApplyModifiedProperties();
			}
			
			EditorGUILayout.Space();

			if (string.IsNullOrEmpty(settings.buildPath) || string.IsNullOrEmpty(settings.gameShortName) || string.IsNullOrEmpty(settings.subfolderName) || string.IsNullOrEmpty(settings.version)) {
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
								CreateBatchScript(res, settings.script);
								Process proc = new Process();
								proc.StartInfo.FileName = res + settings.gameShortName + t.suffix;
								proc.Start();
							}
							CreateItchScript(settings);
							OpenAfter(false, res);
							EditorGUILayout.EndScrollView();
							EditorGUIUtility.ExitGUI();
							return; // <- only one!
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
							CreateBatchScript(res, settings.script);
						}
					}
					CreateItchScript(settings);
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

				//var newIgnorePDB = GUILayout.Toggle(settings.ignorePDB, new GUIContent(" Don't include PDB files", "PDB files have symbol tables, ie. are useful for debugging purposes. Normally not needed when distributing builds to clients."));
				var newIgnorePDB = EditorGUILayout.Toggle(new GUIContent("Don't include PDB files", "PDB files have symbol tables, ie. are useful for debugging purposes. Normally not needed when distributing builds to clients."), settings.ignorePDB);
				if (settings.ignorePDB != newIgnorePDB) {
					settings.ignorePDB = newIgnorePDB;
					PlayerPrefs.SetInt("ignorepdb", newIgnorePDB ? 1 : 0);
				}

				//var newPackAs7Zip = GUILayout.Toggle(settings.packWinBinsAs7Zip, new GUIContent(" Pack Windows binaries as .7z", ".7z packs a lot better than .zip, but not everybody owns a program being able to unpack it."));
				var newPackAs7Zip = EditorGUILayout.Toggle(new GUIContent("Pack Win binaries as .7z", ".7z packs a lot better than .zip, but not everybody owns a program being able to unpack it."), settings.packWinBinsAs7Zip);
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
								var path = settings.buildPath + "/" + settings.gameShortName + "-" + settings.version + "/" + t.shortPath + "/" + settings.subfolderName + "/";
								CopyAdditionalFiles(path);
								CreateBatchScript(path, settings.script);
								PackGame(t.shortPath, t.target);
							}
						}
						CreateItchScript(settings);
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
								CreateBatchScript(res, settings.script);
								PackGame(t.shortPath, t.target);
							}
						}
						CreateItchScript(settings);
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

			/*if (EditorGUI.EndChangeCheck()) {
				settingsObj.ApplyModifiedProperties();
				EditorUtility.SetDirty(settings);
			}*/
		}

		//

		static void CopyAdditionalFiles(string path) {
			// those without dir struct
			for (int i = 0; i < settings.includedFiles.Length; ++i) {
				var assetPath = AssetDatabase.GetAssetPath(settings.includedFiles[i]);
				var fileName = Path.GetFileName(assetPath);
				if (File.Exists(path + fileName)) { FileUtil.DeleteFileOrDirectory(path + fileName); }
				FileUtil.CopyFileOrDirectory(Application.dataPath + "/../" + assetPath, path + fileName);
				RemoveMetaFiles(path + fileName);
			}
			// those with dir struct
			for (int i = 0; i < settings.includedFilesWithDirStruct.Length; ++i) {
				var assetPath = AssetDatabase.GetAssetPath(settings.includedFilesWithDirStruct[i]);
				var unassetPath = assetPath.Substring("Assets".Length);
				var parentPath = Directory.GetParent(path + unassetPath).FullName;
				if (!Directory.Exists(parentPath)) { Directory.CreateDirectory(parentPath); }
				if (File.Exists(path + unassetPath) || Directory.Exists(path + unassetPath)) { FileUtil.DeleteFileOrDirectory(path + unassetPath); }
				FileUtil.CopyFileOrDirectory(Application.dataPath + unassetPath, path + unassetPath);
				RemoveMetaFiles(path + unassetPath);
			}
		}

		/// <summary>
		/// user generated script file
		/// </summary>
		/// <param name="path"></param>
		/// <param name="script"></param>
		static void CreateBatchScript(string path, string script) {
			script = script.Trim();
			if (string.IsNullOrEmpty(script)) { return; }
			if (script == "") { return; }
			var filename = "____script.bat";
			File.WriteAllText(path + "/" + filename, script);
			var startInfo = new ProcessStartInfo(filename) {
				WorkingDirectory = (path.Replace("/", "\\").TrimEnd(new[] { '\\' }) + "\\")
			};
			var proc = Process.Start(startInfo);
			proc.WaitForExit();
			File.Delete(path + "/" + filename);
		}

		/// <summary>
		/// ITCH IO
		/// </summary>
		/// <param name="settings"></param>
		static void CreateItchScript(StoneBuilderSettings settings) {
			var s = settings;
			if (!s.createItchBat) { return; }
			string path = settings.buildPath + "/" + settings.gameShortName + "-" + settings.version + "/publish_to_itch.bat";
			string script = "";
			List<Target> activeTargets = new List<Target>();
			for (var iter = targets.GetEnumerator(); iter.MoveNext();) {
				var t = iter.Current.Value;
				if (t.GetActive()) {
					activeTargets.RemoveAll(o => o.itchName == t.itchName && o.itchVal <= t.itchVal);
					if (!activeTargets.Exists(o => o.itchName == t.itchName)) { activeTargets.Add(t); }
				}
			}
			for (var iter = activeTargets.GetEnumerator(); iter.MoveNext(); ) {
				var t = iter.Current;
				if (script != "") { script += "\r\n"; }
				var pat = settings.buildPath + "/" + settings.gameShortName + "-" + settings.version + "/" + t.shortPath + "/" + settings.subfolderName;
				var tag = !string.IsNullOrEmpty(settings.itchAdditionalTags) ? ("-" + settings.itchAdditionalTags) : "";
				script += "butler push " + pat + " " + settings.itchUsername + "/" + settings.itchGamename + ":" + t.itchName + tag + " --userversion " + settings.version;
			}
			File.WriteAllText(path, script);
		}

		/// <summary>
		/// remove ".meta" stuff from unity
		/// </summary>
		/// <param name="dirName"></param>
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
#if !UNITY_2017_3_OR_NEWER
				case BuildTarget.StandaloneOSXIntel:
				case BuildTarget.StandaloneOSXIntel64:
#endif
				case BuildTarget.StandaloneOSX:
					format = "";
					suffix = ".zip";
					break;
#if !UNITY_2019_2_OR_NEWER
				case BuildTarget.StandaloneLinux:
				case BuildTarget.StandaloneLinuxUniversal:
#endif
				case BuildTarget.StandaloneLinux64:
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
			ProcessStartInfo info = new ProcessStartInfo("\"" + sevenZipPath + "\"") {
				Arguments = "a " + format + " " + path + packedFile + " " + path + settings.subfolderName + "/" + ignorePDB
			};
			var proc = Process.Start(info);
			proc.WaitForExit();

			if (isLinux) {
				var newPackedFile = packedFile + ".gz";
				info = new ProcessStartInfo("\"" + sevenZipPath + "\"") {
					Arguments = "a -tgzip " + path + newPackedFile + " " + path + packedFile + ignorePDB
				};
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
#if HAS_SCENEMANAGER
			if (!UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
#else
			if (!EditorApplication.SaveCurrentSceneIfUserWantsTo()) {
#endif
				return "";
			}

#if HAS_SCENEMANAGER
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
			Directory.CreateDirectory(path);
			var levelPaths = new string[settings.levels.Count];
			for (int i = 0; i < levelPaths.Length; ++i) {
				levelPaths[i] = AssetDatabase.GetAssetPath(settings.levels[i]);
			}
#if UNITY_2018_1_OR_NEWER
			BuildPipeline.BuildPlayer(levelPaths, path + settings.gameShortName + suffix, buildTarget, options);
#else
			var res = BuildPipeline.BuildPlayer(levelPaths, path + settings.gameShortName + suffix, buildTarget, options);
			if (res != "") {
				UnityEngine.Debug.Log(res);
				return "";
			}
#endif

#if HAS_SCENEMANAGER
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