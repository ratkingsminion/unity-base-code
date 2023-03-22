using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

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

		[SerializeField] List<FolderInfo> infos = null;
		//
		readonly Dictionary<string, FolderInfo> infosByType = new Dictionary<string, FolderInfo>();
		public ConfigFile ConfigFile { get; private set; }
		string backupsPath = null;

		//

		void Awake() {
			if (Inst != null) { Debug.LogError("There should be only one FileHandling instance!"); Destroy(this); return; }
			Inst = this;
			//
			ConfigFile = new Base.ConfigFile(); // TODO:

			if (ConfigFile.HasString("backups")) {
				backupsPath = ConfigFile.GetString("backups");
				if (!Path.IsPathFullyQualified(backupsPath)) { backupsPath = DataPath + "/" + backupsPath; }
			}

			//Debug.Log(DataPath);
			foreach (var fi in infos) {
				fi.subfolder = ConfigFile.GetString(fi.typeID, fi.subfolder);
				//Debug.Log(fi.subfolder + " ... " + Path.IsPathFullyQualified(fi.subfolder));
				if (Path.IsPathFullyQualified(fi.subfolder)) { fi.fullPath = fi.subfolder; }
				else { fi.fullPath = DataPath + "/" + fi.subfolder; }
				//Debug.Log(fi.typeID + ": " + fi.fullPath);
				infosByType[fi.typeID.ToLower()] = fi;
			}
		}

		public string FullPath(string typeID) {
			typeID = typeID.ToLower();
			if (!infosByType.ContainsKey(typeID)) {
#if UNITY_EDITOR
				Debug.LogWarning("Info for type " + typeID + " does not exist");
#endif
				return DataPath;
			}
			return infosByType[typeID].fullPath; // mainPath[(int)type];
		}

		public string GetSuffix(string typeID) {
			typeID = typeID.ToLower();
			if (!infosByType.ContainsKey(typeID)) {
#if UNITY_EDITOR
				Debug.LogWarning("Info for type " + typeID + " does not exist");
#endif
				return "";
			}
			return infosByType[typeID].suffix;
		}

		//

		public bool TryGetFiles(string typeID, string subfolder, ref List<string> list) {
			typeID = typeID.ToLower();
			if (!infosByType.TryGetValue(typeID, out var info)) {
				Debug.LogError("Type ID " + typeID + " not found!");
				return false;
			}
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

		public System.IO.StreamReader GetTextFile(string typeID, string filename) {
			return GetTextFile(typeID, "", filename);
		}

		public System.IO.StreamReader GetTextFile(string typeID, string subfolder, string filename) {
			typeID = typeID.ToLower();
			if (!infosByType.TryGetValue(typeID, out var info)) {
				Debug.LogError("Type ID " + typeID + " not found!");
				return null;
			}
			var path = info.fullPath + "/" + subfolder;
			if (!System.IO.Directory.Exists(path)) { Debug.LogWarning("Directory \"" + path + "\" does not exist!"); return null; }
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
		public string GetFileText(string typeID, string filename) {
			var f = GetTextFile(typeID, filename);
			if (f == null) { Debug.LogWarning("Could not get script for " + typeID + " ... " + filename); return null; }
			var content = f.ReadToEnd();
			f.Close();
			return content;
		}
		public string GetFileText(string typeID, string subfolder, string filename) {
			var f = GetTextFile(typeID, subfolder, filename);
			if (f == null) { Debug.LogWarning("Could not get script for " + typeID + " ... " + subfolder + " ... " + filename); return null; }
			var content = f.ReadToEnd();
			f.Close();
			return content;
		}

		/// <summary>
		/// Back up and remove a file
		/// </summary>
		public bool BackupAndRemoveFile(string typeID, string filename, int maxBackups = 3) {
			return BackupAndRemoveFile(typeID, "", filename, maxBackups);
		}
		public bool BackupAndRemoveFile(string typeID, string subfolder, string filename, int maxBackups = 3) {
			if (filename.Contains("\\") || filename.Contains("/")) {
				Debug.LogError("No subfolders in filename!");
				return false;
			}
			typeID = typeID.ToLower();
			if (!infosByType.TryGetValue(typeID, out var info)) {
				Debug.LogError("Type ID " + typeID + " not found!");
				return false;
			}
			var path = info.fullPath + "/" + subfolder;
			var backupPath = !string.IsNullOrWhiteSpace(backupsPath) ? backupsPath : (info.fullPath + "/" + subfolder);
			var filepath = path + "/" + filename + info.suffix;
			if (System.IO.File.Exists(filepath)) {
				if (maxBackups > 0 && !Directory.Exists(backupPath)) {
					Directory.CreateDirectory(backupPath);
				}
				// do a backup
				for (int i = maxBackups; i >= 1; --i) {
					var backuppathN = backupPath + "/" + filename + ".bak" + i;
					if (System.IO.File.Exists(backuppathN)) { System.IO.File.Delete(backuppathN); }
					if (i > 1) {
						var backuppathO = backupPath + "/" + filename + ".bak" + (i - 1);
						if (System.IO.File.Exists(backuppathO)) { System.IO.File.Move(backuppathO, backuppathN); }
					}
				}
				if (maxBackups > 0) {
					var backuppath1 = backupPath + "/" + filename + ".bak1";
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
		public bool RemoveFile(string typeID, string filename) {
			return RemoveFile(typeID, "", filename);
		}
		public bool RemoveFile(string typeID, string subfolder, string filename) {
			typeID = typeID.ToLower();
			if (!infosByType.TryGetValue(typeID, out var info)) {
				Debug.LogError("Type ID " + typeID + " not found!");
				return false;
			}
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

		public bool WriteFile(string typeID, string filename, ref string contents) {
			return WriteFile(typeID, "", filename, ref contents);
		}
		public bool WriteFile(string typeID, string subfolder, string filename, ref string contents) {
			if (filename.Contains("\\") || filename.Contains("/")) {
				Debug.LogError("No subfolders in filename!");
				return false;
			}
			typeID = typeID.ToLower();
			if (!infosByType.TryGetValue(typeID, out var info)) {
				Debug.LogError("Type ID " + typeID + " not found!");
				return false;
			}
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
	}
}