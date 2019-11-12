using UnityEngine;
using UnityEngine.UI;
using System;

[ExecuteInEditMode]
public class RowLayoutGroup : MonoBehaviour {

	[Tooltip("In case that the element has layout element on it, the row height will be taken from its minimum size.")]
	public float rowHeight = 100;
	public float spacing = 0;

	public Action OnUpdated;

	bool isDirty;
	int activeChildCount;
	int calculatedChildCount;

	RectTransform _rectTransform;
	RectTransform rectTransform {
		get {
			if(_rectTransform == null) {
				_rectTransform = GetComponent<RectTransform>();
			}
			return _rectTransform;
		}
	}

	void LateUpdate() {
		if (isDirty) {
			UpdateRows();
		}
		if (activeChildCount != transform.ChildCountActive()) {
			activeChildCount = transform.ChildCountActive();
			SetDirty();
		}
	}

	void OnEnable() {
		SetDirty();
	}

	public void SetDirty() {
		isDirty = true;
	}

	void UpdateRows() {
		int notIgnoredCount = 0;
		float totalHeight = 0;
		for (int i = 0; i < transform.childCount; i++) {
			Transform child = transform.GetChild(i);
			float height = 0;
			if(!IsIgnored(child, out height)) {
				RectTransform rt = child.GetComponent<RectTransform>();
				float anchoredPositionY = ((rt.pivot.y - 1) * height) + totalHeight; 
				totalHeight -= height + spacing;
				rt.sizeDelta = new Vector2(rt.sizeDelta.x, height);
				rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, anchoredPositionY);
				notIgnoredCount++;
			}
		}
		rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x,
			Mathf.Abs(totalHeight) - spacing);
		ResetDirty();
		calculatedChildCount = notIgnoredCount;
		if (OnUpdated != null) {
			OnUpdated();
		}
	}

	bool IsIgnored(Transform t, out float rowHeight) {
		if (t.gameObject.activeSelf == false) {
			rowHeight = -1;
			return true;
		}
		LayoutElement layoutElement = t.GetComponent<LayoutElement>();
		if (layoutElement != null) {
			if (layoutElement.ignoreLayout) {
				rowHeight = -1;
				return true;
			}
			else {
				rowHeight = layoutElement.minHeight;
				return false;
			}
		}
		rowHeight = this.rowHeight;
		return false;
	}

	void ResetDirty() {
		isDirty = false;
	}

	#if UNITY_EDITOR
	void OnValidate() {
		SetDirty();
	}
	#endif
}
