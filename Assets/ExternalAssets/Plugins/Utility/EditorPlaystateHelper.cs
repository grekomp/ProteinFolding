using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public static class EditorPlaystateHelper {
	public static bool initialized = false;
	public static bool cachedIsPlaying = false;
#if UNITY_EDITOR
	public static PlayModeStateChange lastState = PlayModeStateChange.EnteredEditMode;
#endif
	public static bool IsPlaying {
		get {
#if UNITY_EDITOR
			return EditorApplication.isPlaying;
#endif
			return true;
		}
	}
	public static bool IsEditorQuittingPlay {
		get {
#if UNITY_EDITOR
			return EditorApplication.isPlayingOrWillChangePlaymode == true && EditorApplication.isPlaying == true && Time.frameCount == 0;
#endif
			return false;
		}
	}
	public static bool IsEditorEnteringPlay {
		get {
#if UNITY_EDITOR
			return EditorApplication.isPlayingOrWillChangePlaymode == true && EditorApplication.isPlaying == false;
#endif
			return false;
		}
	}

	#region Runtime scene change events
	public static event UnityAction<Scene, LoadSceneMode> sceneLoaded;
	public static event UnityAction<Scene> sceneUnloaded;
	public static event UnityAction<Scene, Scene> activeSceneChanged;
	#endregion
	#region Editor-only scene change events
	public static event Action<Scene, string> sceneSaving;
	public static event Action<Scene> sceneSaved;
	public static event Action<Scene, bool> sceneClosing;
	public static event Action<Scene> sceneClosed;
#if UNITY_EDITOR
	public static event Action<string, OpenSceneMode> sceneOpening;
	public static event Action<Scene, OpenSceneMode> sceneOpened;
#endif
	#endregion

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void Initialize() {
		initialized = true;
#if UNITY_EDITOR
		cachedIsPlaying = EditorApplication.isPlaying;
		EditorApplication.playModeStateChanged -= HandlePlayModeStateChange;
		EditorApplication.playModeStateChanged += HandlePlayModeStateChange;

		EditorSceneManager.sceneLoaded += HandleSceneLoaded;
		EditorSceneManager.sceneUnloaded += HandleSceneUnloaded;
		EditorSceneManager.activeSceneChanged += HandleActiveSceneChanged;
		EditorSceneManager.activeSceneChangedInEditMode += HandleActiveSceneChanged;

		EditorSceneManager.sceneSaving += HandleSceneSaving;
		EditorSceneManager.sceneSaved += HandleSceneSaved;
		EditorSceneManager.sceneClosing += HandleSceneClosing;
		EditorSceneManager.sceneClosed += HandleSceneClosed;
		EditorSceneManager.sceneOpening += HandleSceneOpening;
		EditorSceneManager.sceneOpened += HandleSceneOpened;
#else
		cachedIsPlaying = Application.isPlaying;

		SceneManager.sceneLoaded += HandleSceneLoaded;
		SceneManager.sceneUnloaded += HandleSceneUnloaded;
		SceneManager.activeSceneChanged += HandleActiveSceneChanged;
#endif
	}

	#region Event Handlers
#if UNITY_EDITOR
	private static void HandleSceneOpened(Scene scene, OpenSceneMode mode) => sceneOpened?.Invoke(scene, mode);
	private static void HandleSceneOpening(string path, OpenSceneMode mode) => sceneOpening?.Invoke(path, mode);
	private static void HandleSceneClosed(Scene scene) => sceneClosed?.Invoke(scene);
	private static void HandleSceneClosing(Scene scene, bool removingScene) => sceneClosing?.Invoke(scene, removingScene);
	private static void HandleSceneSaved(Scene scene) => sceneSaved?.Invoke(scene);
	private static void HandleSceneSaving(Scene scene, string path) => sceneSaving?.Invoke(scene, path);
#endif

	private static void HandleSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) => sceneLoaded?.Invoke(scene, loadSceneMode);
	private static void HandleSceneUnloaded(Scene scene) => sceneUnloaded?.Invoke(scene);
	private static void HandleActiveSceneChanged(Scene scene1, Scene scene2) => activeSceneChanged?.Invoke(scene1, scene2);

#if UNITY_EDITOR
	static void HandlePlayModeStateChange(PlayModeStateChange stateChange) {
		switch (stateChange) {
			case PlayModeStateChange.EnteredEditMode:
			case PlayModeStateChange.ExitingPlayMode:
				cachedIsPlaying = false;
				break;
			case PlayModeStateChange.EnteredPlayMode:
			case PlayModeStateChange.ExitingEditMode:
				cachedIsPlaying = true;
				break;
		}
		lastState = stateChange;
		initialized = true;
	}
#endif
	#endregion
}
