using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ItSilesiaPlugins.UI;

[CustomEditor(typeof(ChildBehaviour), true)]
public class ChildInspector : Editor {

	public override void OnInspectorGUI() {
		ChildBehaviour targetChild = (ChildBehaviour)target;
		DrawDefaultInspector();
		DrawWarningIfNecessary(targetChild);
		DrawHelperButtons(targetChild);
	}

	void DrawHelperButtons(ChildBehaviour targetChild) {
		EditorGUILayout.Space();
		EditorGUILayout.BeginHorizontal();
		if (Application.isPlaying) {
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Show", GUILayout.Width(150))) {
				targetChild.Show();
			}
			GUILayout.Space(20);
			if (GUILayout.Button("Hide", GUILayout.Width(150))) {
				targetChild.Hide();
			}
			GUILayout.FlexibleSpace();
		} else {
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Show", GUILayout.Width(150))) {
				ShowThisChildOnly(targetChild);
			}
			GUILayout.FlexibleSpace();
		}
		EditorGUILayout.EndHorizontal();
	}

	void DrawWarningIfNecessary(ChildBehaviour targetChild) {
		EventHandler eh = targetChild.GetComponent<EventHandler>();
		if(eh == null) {
			EditorGUILayout.Space();
			EditorGUILayout.HelpBox("You should attach EventHandler component", MessageType.Warning);
		}
	}

	void ShowThisChildOnly(ChildBehaviour child) {
		ParentBehaviour manager = child.GetComponentInParent<ParentBehaviour>();
		foreach(ChildBehaviour c in manager.GetComponentsInChildren<ChildBehaviour>()) {
			c.gameObject.SetActive(false);
		}
		child.gameObject.SetActive(true);
	}
}
