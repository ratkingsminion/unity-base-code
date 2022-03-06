using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base {

	[DefaultExecutionOrder(-1000)]
	public class FileHandling : MonoBehaviour {

#if UNITY_EDITOR
		public static string DataPath => Application.dataPath;
#elif UNITY_STANDALONE_OSX
		public static string DataPath => Application.dataPath + "/../";
#else
		public static string DataPath => Application.dataPath + "/../";
#endif

		[System.Serializable]
		public class FolderInfo {
			public string typeID = "";
			public string subfolder = "";
			public string suffix = ".txt";
			//
			[System.NonSerialized] public string fullPath;
		}

		public static FileHandling Inst { get; private set; }

		[SerializeField] string folderDataFiles = "Data_Files";
		[SerializeField] List<FolderInfo> infos = null;
		//
		Dictionary<string, FolderInfo> infosByType = new Dictionary<string, FolderInfo>();

		//

		void Awake() {
			if (Inst != null) { Debug.LogError("There should be only one FileHandling instance!"); Destroy(this); return; }
			Inst = this;
			//
			foreach (var fi in infos) {
				fi.fullPath = DataPath + "/" + folderDataFiles + "/" + fi.subfolder;
				infosByType[fi.typeID.ToLower()] = fi;
			}
			//
			//mainPath = new[] {
			//	Main.DataPath + "/" + folderDataFiles,
			//	Main.DataPath + "/" + folderDataFiles + "/" + subFolderLocalisationFiles,
			//	Main.DataPath + "/" + folderDataFiles + "/" + subFolderTemporary
			//};
		}

		public string FullPath(string typeID) {
			typeID = typeID.ToLower();
#if UNITY_EDITOR
			if (!infosByType.ContainsKey(typeID)) {
				Debug.LogWarning("Info for type " + typeID + " does not exist");
				return DataPath + "/" + Inst.folderDataFiles;
			}
#endif
			return infosByType[typeID].fullPath; // mainPath[(int)type];
		}

		public string GetSuffix(string typeID) {
			typeID = typeID.ToLower();
#if UNITY_EDITOR
			if (!infosByType.ContainsKey(typeID)) {
				Debug.LogWarning("Info for type " + typeID + " does not exist");
				return "";
			}
#endif
			return infosByType[typeID].suffix;
		}

		//

		public bool TryGetFiles(string typeID, string subfolder, ref List<string> list) {
			typeID = typeID.ToLower();
			if (!infosByType.TryGetValue(typeID, out var info)) { return false; }
			var path = info.fullPath + "/" + subfolder;
			if (list == null) { list = new List<string>(); }
			else { list.Clear(); }
			if (System.IO.Directory.Exists(path)) {
				foreach (var file in System.IO.Directory.GetFiles(path)) {
					if (file.EndsWith(info.suffix)) { // System.IO.Path.GetExtension(file) == info.suffix) {
						list.Add(System.IO.Path.GetFileNameWithoutExtension(file));
					}
				}
			}
			return true;
		}

		public System.IO.StreamReader GetTextFile(string typeID, string subfolder, string filename) {
			typeID = typeID.ToLower();
			if (!infosByType.TryGetValue(typeID, out var info)) { return null; }
			var path = info.fullPath + "/" + subfolder;
			if (!System.IO.Directory.Exists(path)) { return null; }
			//
			foreach (var file in System.IO.Directory.GetFiles(path)) {
				if (file.EndsWith(info.suffix)) { // System.IO.Path.GetExtension(file) == info.suffix) {
					var name = System.IO.Path.GetFileNameWithoutExtension(file);
					if (name == filename) {
						return System.IO.File.OpenText(file);
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Get file contents
		/// </summary>
		public string GetFile(string typeID, string subfolder, string filename) {
			var f = GetTextFile(typeID, subfolder, filename);
			if (f == null) { Debug.LogWarning("Could not get script for " + typeID); return null; }
			var content = f.ReadToEnd();
			f.Close();
			return content;
		}

		/// <summary>
		/// Get file contents from subfolder, with fallback standard folder
		/// </summary>
		public string GetFile(string typeID, string subfolder, string standard, string filename) {
			var f = GetTextFile(typeID, subfolder, filename);
			if (f == null) {
				f = GetTextFile(typeID, standard, filename);
				if (f == null) {
					Debug.LogWarning("Could not get script for " + typeID);
					return null;
				}
			}
			var content = f.ReadToEnd();
			f.Close();
			return content;
		}

		/// <summary>
		/// Back up and remove a file
		/// </summary>
		public bool BackupAndRemoveFile(string typeID, string subfolder, string filename, int maxBackups = 3) {
			if (filename.Contains("\\") || filename.Contains("/")) {
				Debug.LogError("No subfolders in filename!");
				return false;
			}
			typeID = typeID.ToLower();
			if (!infosByType.TryGetValue(typeID, out var info)) { return false; }
			var path = info.fullPath + "/" + subfolder;
			var filepath = path + "/" + filename + info.suffix;
			if (System.IO.File.Exists(filepath)) {
				// do a backup
				for (int i = maxBackups; i >= 1; --i) {
					var backuppathN = path + "/" + filename + ".bak" + i;
					if (System.IO.File.Exists(backuppathN)) { System.IO.File.Delete(backuppathN); }
					if (i > 1) {
						var backuppathO = path + "/" + filename + ".bak" + (i - 1);
						if (System.IO.File.Exists(backuppathO)) { System.IO.File.Move(backuppathO, backuppathN); }
					}
				}
				if (maxBackups > 0) {
					var backuppath1 = path + "/" + filename + ".bak1";
					System.IO.File.Move(filepath, backuppath1);
				}
				// remove the file
				System.IO.File.Delete(filepath);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Remove Script
		/// </summary>
		/// <param name="subfolder"></param>
		/// <param name="filename"></param>
		public bool RemoveFile(string typeID, string subfolder, string filename) {
			typeID = typeID.ToLower();
			if (!infosByType.TryGetValue(typeID, out var info)) { return false; }
			var filepath = info.fullPath + "/" + subfolder + "/" + filename + info.suffix;
			var deleted = false;
			if (System.IO.File.Exists(filepath)) {
				System.IO.File.Delete(filepath);
				deleted = true;
			}
#if UNITY_EDITOR
			var filepathMeta = info.fullPath + "/" + subfolder + "/" + filename + info.suffix + ".meta";
			if (deleted && System.IO.File.Exists(filepathMeta)) {
				System.IO.File.Delete(filepathMeta);
			}
#endif
			return deleted;
		}

		public bool WriteFile(string typeID, string subfolder, string filename, ref string contents) {
			if (filename.Contains("\\") || filename.Contains("/")) {
				Debug.LogError("No subfolders in filename!");
				return false;
			}
			typeID = typeID.ToLower();
			if (!infosByType.TryGetValue(typeID, out var info)) { return false; }
			var path = info.fullPath + "/" + subfolder;
			var filepath = path + "/" + filename + info.suffix;
			if (!System.IO.Directory.Exists(path)) {
				System.IO.Directory.CreateDirectory(path);
			}
			else if (System.IO.File.Exists(filepath)) {
				return false;
			}
			System.IO.File.WriteAllText(filepath, contents, System.Text.Encoding.UTF8);
			return true;
		}

		public bool CopyFile(string typeID, string subfolderA, string subfolderB, string filename) {
			if (filename.Contains("\\") || filename.Contains("/")) {
				Debug.LogError("No subfolders in filename!");
				return false;
			}
			typeID = typeID.ToLower();
			if (!infosByType.TryGetValue(typeID, out var info)) { return false; }
			var filepathA = info.fullPath + "/" + subfolderA + "/" + filename + info.suffix;
			var filepathB = info.fullPath + "/" + subfolderB + "/" + filename + info.suffix;
			if (!System.IO.File.Exists(filepathA)) { return false; }
			System.IO.File.Copy(filepathA, filepathB, true);
			return true;
		}
	}
}