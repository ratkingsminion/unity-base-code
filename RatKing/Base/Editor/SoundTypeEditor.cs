using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace RatKing.Base {

	[CustomEditor(typeof(SoundType))]
	[CanEditMultipleObjects]
	public class SoundTypeEditor : Editor {
		SerializedProperty clips;
		SerializedProperty volume;
		SerializedProperty pitch;
		SerializedProperty priority;
		SerializedProperty waitSeconds;
		SerializedProperty loop;
		SerializedProperty pan;
		SerializedProperty spatialBlend;
		SerializedProperty spread3D;
		SerializedProperty distance3D;
		SerializedProperty startCount;
		SerializedProperty addCount;

		//

		void OnEnable() {
			clips = serializedObject.FindProperty("clips");
			volume = serializedObject.FindProperty("volume");
			pitch = serializedObject.FindProperty("pitch");
			priority = serializedObject.FindProperty("priority");
			waitSeconds = serializedObject.FindProperty("waitSeconds");
			loop = serializedObject.FindProperty("loop");
			pan = serializedObject.FindProperty("pan");
			spatialBlend = serializedObject.FindProperty("spatialBlend");
			spread3D = serializedObject.FindProperty("spread3D");
			distance3D = serializedObject.FindProperty("distance3D");
			startCount = serializedObject.FindProperty("startCount");
			addCount = serializedObject.FindProperty("addCount");
			StopAllClips();
		}

		public override void OnInspectorGUI() {
			//base.OnInspectorGUI();
			serializedObject.Update();

			// AUDIO STUFF

			EditorGUILayout.PropertyField(clips, true);
			if (GUILayout.Button("Test") && clips != null && clips.arraySize > 0) {
				var idx = UnityEngine.Random.Range(0, clips.arraySize);
				var clip = clips.GetArrayElementAtIndex(idx).objectReferenceValue as AudioClip;
				if (Camera.main != null && clip != null) {
					PlayClip(clip);
					//StopTest();

					//testSource = new GameObject("__TEMP__DELETE_ME__").AddComponent<AudioSource>();
					//testSource.gameObject.hideFlags = HideFlags.DontSave;
					//testSource.PlayOneShot(clip);

					//float timer = (float)EditorApplication.timeSinceStartup + clip.length;
					//testUpdateCall = () => { if (timer < EditorApplication.timeSinceStartup) { StopTest(); } };
					//testStateChangeCall = (PlayModeStateChange state) => { StopTest(); };
					//EditorApplication.playModeStateChanged += testStateChangeCall;
					//EditorApplication.update += testUpdateCall;
				}
			}
			EditorGUILayout.PropertyField(volume);
			EditorGUILayout.PropertyField(pitch);
			EditorGUILayout.PropertyField(priority);
			EditorGUILayout.PropertyField(waitSeconds);
			EditorGUILayout.PropertyField(loop);
			EditorGUILayout.PropertyField(pan);
			EditorGUILayout.PropertyField(spatialBlend);
			if (spatialBlend.floatValue > 0f) {
				EditorGUILayout.PropertyField(spread3D);
				EditorGUILayout.PropertyField(distance3D);
			}

			EditorGUILayout.PropertyField(startCount);
			EditorGUILayout.PropertyField(addCount);

			serializedObject.ApplyModifiedProperties();
		}

#if UNITY_2019_2_OR_NEWER
		// https://forum.unity.com/threads/way-to-play-audio-in-editor-using-an-editor-script.132042/#post-4767824

		public static void PlayClip(AudioClip clip, int startSample = 0, bool loop = false) {
			System.Reflection.Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
			System.Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
			System.Reflection.MethodInfo method = audioUtilClass.GetMethod(
#if UNITY_2020_2_OR_NEWER
				"PlayPreviewClip",
#else
				"PlayClip",
#endif
				System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public,
				null,
				new System.Type[] { typeof(AudioClip), typeof(int), typeof(bool) },
				null
			);
			method.Invoke(
				null,
				new object[] { clip, startSample, loop }
			);
		}

#else
		// https://answers.unity.com/questions/36388/how-to-play-audioclip-in-edit-mode.html

		public static void PlayClip(AudioClip clip) {
			var unityEditorAssembly = typeof(AudioImporter).Assembly;
			var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
			var method = audioUtilClass.GetMethod(
				"PlayClip",
				BindingFlags.Static | BindingFlags.Public,
				null,
				new System.Type[] {
					typeof(AudioClip)
				},
				null
			);
			method.Invoke(
				null,
				new object[] {
					clip
				}
			);
		}
#endif

		public static void StopAllClips() {
			var unityEditorAssembly = typeof(AudioImporter).Assembly;
			var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
			var method = audioUtilClass.GetMethod(
#if UNITY_2020_2_OR_NEWER
				"StopAllPreviewClips",
#else
				"StopAllClips",
#endif
				BindingFlags.Static | BindingFlags.Public,
				null,
				new System.Type[] { },
				null
			);
			method.Invoke(
				null,
				new object[] { }
			);
		}
	}

}