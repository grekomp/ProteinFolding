using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ItSilesiaPlugins.UI {
	[CustomEditor(typeof(PanelManager))]
	public class PanelManagerInspector : Editor {

		bool isClicked = false;
		bool isPanelsListOpen = true;

		public override void OnInspectorGUI() {
			PanelManager panelManager = (PanelManager)target;
			serializedObject.Update();
			DrawDefaultInspector();
			DrawPanelsList(panelManager);
			DrawButtons();
			serializedObject.ApplyModifiedProperties();
		}


		void DrawPanelsList(PanelManager panelManager) {
			Panel[] panels = panelManager.gameObject.GetComponentsInChildren<Panel>(true);
			isPanelsListOpen = EditorGUILayout.Foldout(isPanelsListOpen, "Panels", true);
			if(isPanelsListOpen) {
				GUILayout.BeginVertical();
				foreach (Panel p in panels) {
					GUILayout.BeginHorizontal(GUILayout.MaxHeight(20));
					GUILayout.Label(p.name);
					if (IsPanelActive(p, panelManager)) {
						GUILayout.Label(EditorGUIUtility.IconContent("ViewToolOrbit"), GUILayout.MaxHeight(17));
					}
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Show", GUILayout.Width(75))) {
						ShowPanel(panels, p);
					}	
					GUILayout.EndHorizontal();
				}
				GUILayout.EndVertical();
			}
		}

		void DrawButtons() {
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUIStyle style = new GUIStyle(GUI.skin.button);
			style.fontSize = 12;
			style.fixedWidth = 230;
			style.fixedHeight = 23;
			if (GUILayout.Button("Add Panel", style)) {
				isClicked = true;
			}
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			if (isClicked && Event.current.type == EventType.Repaint) {
				isClicked = false;
				PanelCreator.DisplayWindow(GUILayoutUtility.GetLastRect());
			}
		}

		void ShowPanel(Panel[] panelList, Panel activePanel) {
			if (Application.isPlaying) {
				activePanel.Show();
			} else {
				foreach (Panel p in panelList) {
					p.gameObject.SetActive(p == activePanel);
				}
			}
		}

		bool IsPanelActive(Panel p, PanelManager manager) {
			if(Application.isPlaying) {
				return manager.activeChild == p;
			} else {
				return p.isActiveAndEnabled;
			}
		}
	}
}