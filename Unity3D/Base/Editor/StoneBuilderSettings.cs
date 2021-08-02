using UnityEngine;
using System.Collections.Generic;

namespace RatKing.Base {

	[System.Serializable]
	public class StoneBuilderSettings : ScriptableObject {
		// saved in file (ie. gets versioned):
		public string gameShortName = "AppName";
		public string subfolderName = "GameFolder";
		public string version = "0.0.1";
		public bool useBundleVersion = false;
		public List<Object> levels = new List<Object>();
		public Object[] includedFiles = new Object[0];
		public Object[] includedFilesWithDirStruct = new Object[0];
		[Multiline] public string script = "";
		public bool createItchBat = false;
		public string itchUsername;
		public string itchGamename;
		public string itchAdditionalTags;
		[Multiline] public string itchAddScript = "";
		// saved in playerprefs (ie. individual for each workstation):
		[System.NonSerialized] public string buildPath = "C:/BUILDS";
		[System.NonSerialized] public List<bool> targetsActive = new List<bool>();
#if UNITY_EDITOR
		[System.NonSerialized] public UnityEditor.BuildOptions optionsMask;
#endif
		[System.NonSerialized] public bool openAfterBuild;
		[System.NonSerialized] public bool ignorePDB = true;
		[System.NonSerialized] public bool packWinBinsAs7Zip = true;
	}

}