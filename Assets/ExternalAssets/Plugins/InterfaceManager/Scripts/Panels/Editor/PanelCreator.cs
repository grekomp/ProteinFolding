using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ItSilesiaPlugins.UI {
	public class PanelCreator : EditorWindow {

		public string panelName = "";

		string errorString = "";
		string infoString = "";
		bool isValid;
		bool isInitialized = false;

		GameObject targetGameObject;
		string componentName;
		bool isComponentToAdd;

		static string templateRelativePath = "ItSilesiaPlugins/InterfaceManager/Resources/PanelScript.cs.txt";
		static string newScriptName = "NewPanel.cs";


		[MenuItem("Assets/Create/C# Panel Script", priority = 81)]
		static void CreateScript() {
			EditorUtilityExtensions.CreateScriptAsset("Assets/" + templateRelativePath, newScriptName);
		}

		public static void DisplayWindow(Rect buttonRect) {
			PanelCreator window = ScriptableObject.CreateInstance(typeof(PanelCreator)) as PanelCreator;
			Vector2 windowSize = new Vector2(buttonRect.width, 200);
			Vector2 screenPosition = GUIUtility.GUIToScreenPoint(buttonRect.position);
			window.ShowAsDropDown(new Rect(screenPosition, buttonRect.size), windowSize);
		}

		void OnGUI() {
			if(!isInitialized) {
				Init();
			}
			Repaint();
			UpdateOnGUI();
			GUILayout.BeginVertical(EditorStyles.helpBox);
			panelName = EditorGUILayout.TextField("Panel Name", panelName);
			GUILayout.FlexibleSpace();
			if (errorString.Length > 0) {
				EditorGUILayout.HelpBox(errorString, MessageType.Error, true);
			}
			if(infoString.Length > 0) {
				EditorGUILayout.HelpBox(infoString, MessageType.Info, true);
			}
			EditorGUI.BeginDisabledGroup(isValid == false);
			if (GUILayout.Button("Create")) {
				OnCreateButtonClicked();
			}
			EditorGUI.EndDisabledGroup();
			GUILayout.EndHorizontal();
		}

		void UpdateOnGUI() {
			if (isComponentToAdd && !EditorApplication.isCompiling) {
				isComponentToAdd = false;
				AddComponentToTargetObject();
				Selection.activeObject = targetGameObject;
				this.Close();
			}
			if(!isComponentToAdd) {
				infoString = "Script will be saved at: " + ProjectWindowListener.lastSelectedFolderPath;
			}
		}

		void Update() {
			if (!isComponentToAdd) {
				if (panelName.Length <= 3) {
					isValid = false;
					errorString = "Panel name should be longer than 3 characters!";
				}
				else if (!IsUnique(panelName)) {
					isValid = true;
					errorString = "Panel name must be unique!";
				}
				else {
					isValid = true;
					errorString = "";
				}
			}
		}

		void Init() {
			isInitialized = true;
			isComponentToAdd = false;
			targetGameObject = null;
			isValid = false;
			infoString = "";
			Update();
		}

		void OnCreateButtonClicked() {
			targetGameObject = CreatePanelGameObject();
			if(targetGameObject != null) {
				componentName = CreatePanelScriptFile();
				isComponentToAdd = true;
				infoString = "Please wait. Scripts compilation is in progress...";
				isValid = false;
				AssetDatabase.Refresh();
			}
		}

		bool IsUnique(string panelName) {
			string[] scripts = AssetDatabase.FindAssets("t:script " + panelName);
			foreach (string name in scripts) {
				string path = AssetDatabase.GUIDToAssetPath(name);
				if (Path.GetFileNameWithoutExtension(path) == panelName) {
					return false;
				}
			}
			return true;
		}

		GameObject CreatePanelGameObject() {
			PanelManager panelManager = FindObjectOfType<PanelManager>();
			if (panelManager == null) {
				Debug.LogError("Panel Manager not found on scene!");
			} else {
				GameObject go = new GameObject(panelName);
				go.transform.parent = panelManager.transform;
				go.transform.localPosition = Vector3.zero;
				go.transform.localScale = Vector3.one;
				RectTransform rt = go.AddComponent<RectTransform>();
				rt.anchorMin = Vector2.zero;
				rt.anchorMax = Vector2.one;
				rt.sizeDelta = Vector2.zero;
				return go;
			}
			return null;
		}

		string CreatePanelScriptFile() {
			string template = GetTemplate();
			string path = ProjectWindowListener.lastSelectedFolderPath;
			if(path.Length > 0) {
				int separatorPosition = path.IndexOf("/");
				if(separatorPosition >= 0) {
					path = path.Substring(separatorPosition + 1);
				} else {
					path = "";
				}
				
			}
			path = Path.Combine(Application.dataPath, path);
			path = Path.Combine(path, panelName + ".cs");
			path = path.ToStandarizedPathSeparators();
			using (StreamWriter writter = new StreamWriter(path)) {
				writter.Write(template);
			}
			return panelName;
		}

		bool AddComponentToTargetObject() {
			if (targetGameObject != null) {
				Type panelScript = typeof(PanelManager).Assembly.GetType(componentName);
				targetGameObject.AddComponent(panelScript);
				return true;
			}
			return false;
		}

		string GetTemplate() {
			string path = Path.Combine(Application.dataPath, templateRelativePath);
			path = path.ToStandarizedPathSeparators();
			string template;
			using (StreamReader reader = new StreamReader(path)) {
				template = reader.ReadToEnd();
			}
			template = template.Replace("#SCRIPTNAME#", panelName);
			return template;
		}
	}
}
