#define ACTIVATE_ONLY_WHEN_GAME_WINDOW_FOCUSED
//#define REMOVE_TOOLBAR_DURING_PLAY

// original: https://forum.unity.com/threads/unwanted-editor-hotkeys-in-game-mode.182073/#post-6964244

#if REMOVE_TOOLBAR_DURING_PLAY && UNITY_EDITOR_WIN
using System;
using System.Runtime.InteropServices;
using System.Text;
#endif
using System.Linq;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace RatKing.Base {

    [InitializeOnLoad]
    public static class DisableShortcutsDuringPlay {
        static bool activated = false;

        static DisableShortcutsDuringPlay() {
            EditorApplication.playModeStateChanged += PlayModeStateChanged;
            EditorApplication.quitting += Quitting;
        }

        private static void PlayModeStateChanged(PlayModeStateChange playModeStateChange) {
            switch (playModeStateChange) {
                case PlayModeStateChange.EnteredPlayMode:
#if !ACTIVATE_ONLY_WHEN_GAME_WINDOW_FOCUSED
                    Activate();
#endif

                    new GameObject("DisableShortcutsDuringPlay", typeof(DisableShortcutsDuringPlayBehaviour));
                    break;

                case PlayModeStateChange.ExitingPlayMode:
                    ShortcutManager.instance.activeProfileId = "Default";
                    break;
            }
        }

        public static void Activate() {
            if (activated) { return; }

            if (ShortcutManager.instance.GetAvailableProfileIds().Contains("Play")) {
                ShortcutManager.instance.activeProfileId = "Play";
            }
            else {
                ShortcutManager.instance.CreateProfile("Play");
                ShortcutManager.instance.activeProfileId = "Play";

                foreach (string shortcutId in ShortcutManager.instance.GetAvailableShortcutIds()) {
                    if (shortcutId != "Main Menu/Edit/Play") {
                        ShortcutManager.instance.RebindShortcut(shortcutId, ShortcutBinding.empty);
                    }
                }
            }

            activated = true;
		}
        public static void Deactivate() {
            if (!activated) { return; }
            ShortcutManager.instance.activeProfileId = "Default";
            activated = false;
        }

        private static void Quitting() {
            Deactivate();
        }
            }

    public class DisableShortcutsDuringPlayBehaviour : MonoBehaviour {

#if REMOVE_TOOLBAR_DURING_PLAY && UNITY_EDITOR_WIN
        private delegate int EnumThreadWndProc(IntPtr hWnd, IntPtr lParam);
        
        [DllImport("user32.dll")] static extern int EnumThreadWindows(uint dwThreadId, EnumThreadWndProc lpfn, IntPtr lParam);
        [DllImport("kernel32.dll")] static extern uint GetCurrentThreadId();
        [DllImport("user32.dll")] static extern int GetClassNameA(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
        
        [DllImport("user32.dll")] private static extern IntPtr GetMenu(IntPtr hWnd);
        [DllImport("user32.dll")] private static extern int SetMenu(IntPtr hWnd, IntPtr hMenu);
        [DllImport("user32.dll")] private static extern int DrawMenuBar(IntPtr hWnd);
        
        [DllImport("user32.dll")] private static extern int GetWindowLongA(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")] private static extern int SetWindowLongA(IntPtr hWnd, int nIndex, int dwNewLong);
        
        private const int GWL_STYLE = -16;
        
        private const int WS_CAPTION = 0x00C00000;
        private const int WS_SYSMENU = 0x00080000;
        
        private IntPtr hFrameWnd;
        private IntPtr hFrameMenu;

        bool inactiveToolbar;
#endif

        private void Awake() {
#if REMOVE_TOOLBAR_DURING_PLAY && UNITY_EDITOR_WIN
            int EnumThreadWndProc(IntPtr hWnd, IntPtr lParam) {
                StringBuilder className = new StringBuilder(64);
                GetClassNameA(hWnd, className, 64);
                if (className.ToString() == "UnityContainerWndClass") {
                    hFrameWnd = hWnd;
                    hFrameMenu = GetMenu(hWnd);
                    return 0;
                }
                return 1;
            }
            EnumThreadWindows(GetCurrentThreadId(), EnumThreadWndProc, IntPtr.Zero);
#endif
            gameObject.hideFlags = HideFlags.HideInHierarchy;

            GameObject.DontDestroyOnLoad(gameObject);
        }

#if REMOVE_TOOLBAR_DURING_PLAY && UNITY_EDITOR_WIN
        void ActivateToolbar() {
            if (!inactiveToolbar) { return; }
            SetWindowLongA(hFrameWnd, GWL_STYLE, GetWindowLongA(hFrameWnd, GWL_STYLE) | (WS_CAPTION | WS_SYSMENU));
            SetMenu(hFrameWnd, hFrameMenu);
            DrawMenuBar(hFrameWnd);
            inactiveToolbar = false;
        }

        void DeactivateToolbar() {
            if (inactiveToolbar) { return; }
            SetWindowLongA(hFrameWnd, GWL_STYLE, GetWindowLongA(hFrameWnd, GWL_STYLE) & ~(WS_CAPTION | WS_SYSMENU));
            SetMenu(hFrameWnd, IntPtr.Zero);
            DrawMenuBar(hFrameWnd);
            inactiveToolbar = true;
        }
#endif

        private void Update() {
#if ACTIVATE_ONLY_WHEN_GAME_WINDOW_FOCUSED
            if (EditorWindow.focusedWindow != null && EditorWindow.focusedWindow.titleContent.text == "Game") {
                DisableShortcutsDuringPlay.Activate();
#if REMOVE_TOOLBAR_DURING_PLAY && UNITY_EDITOR_WIN
                 DeactivateToolbar();
#endif
			}
            else {
                DisableShortcutsDuringPlay.Deactivate();
#if REMOVE_TOOLBAR_DURING_PLAY && UNITY_EDITOR_WIN
                 ActivateToolbar();
#endif
			}
#else

#if REMOVE_TOOLBAR_DURING_PLAY && UNITY_EDITOR_WIN
            if (Time.frameCount == 3) {
                DeactivateToolbar();
            }
#endif

#endif
        }

        private void OnDestroy() {
#if REMOVE_TOOLBAR_DURING_PLAY && UNITY_EDITOR_WIN
            ActivateToolbar();
#endif
            DisableShortcutsDuringPlay.Deactivate();
        }
    }

}