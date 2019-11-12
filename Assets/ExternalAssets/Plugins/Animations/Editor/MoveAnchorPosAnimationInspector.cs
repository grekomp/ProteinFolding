using SimpleAnimations;
using UnityEditor;
using UnityEngine;

namespace ItSilesiaPlugins.UI {
	[CustomEditor(typeof(AnchorSimpleAnimation))]
	public class MoveAnchorPosAnimationInspector : Editor {

		public override void OnInspectorGUI() {
			AnchorSimpleAnimation animTarget = (AnchorSimpleAnimation)target;
			RectTransform rect = animTarget.GetComponent<RectTransform>();
			serializedObject.Update();
			DrawDefaultInspector();
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Copy as Start")) {
				animTarget.minAnchorTargetOnBackwards = rect.anchorMin;
				animTarget.maxAnchorTargetOnBackwards = rect.anchorMax;
			}
			if (GUILayout.Button("Copy as End")) {
				animTarget.minAnchorTargetOnForward = rect.anchorMin;
				animTarget.maxAnchorTargetOnForward = rect.anchorMax;
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Load Start")) {
				rect.anchorMin = animTarget.minAnchorTargetOnBackwards;
				rect.anchorMax = animTarget.maxAnchorTargetOnBackwards;
			}
			if (GUILayout.Button("Load End")) {
				rect.anchorMin = animTarget.minAnchorTargetOnForward;
				rect.anchorMax = animTarget.maxAnchorTargetOnForward;
			}
			EditorGUILayout.EndHorizontal();
			serializedObject.ApplyModifiedProperties();
		}

	}
}