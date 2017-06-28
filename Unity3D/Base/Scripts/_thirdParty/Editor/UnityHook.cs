using UnityEditor;
using System.Reflection;

// source: https://twitter.com/PsyKola/status/874623012141768704

[InitializeOnLoad]
public class UnityHook {
	static UnityHook() {
		typeof(EditorGUI).GetField("kFloatFieldFormatString", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, "0.#######");
		typeof(EditorGUI).GetField("kDoubleFieldFormatString", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, "0.##############");
	}
}