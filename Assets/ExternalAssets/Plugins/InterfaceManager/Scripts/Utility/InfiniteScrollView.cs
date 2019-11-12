using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

[RequireComponent(typeof(ScrollRect))]
public class InfiniteScrollView : MonoBehaviour, IEndDragHandler {

	public bool isLoading {
		get {
			return loadingAnimObject != null;
		}
	}
	[HideInInspector]
	public GameObject loadingAnimObject;

	public Action<bool> LoadRequest;

	[SerializeField] RowLayoutGroup layout;
	[SerializeField] float pxPullThreshold = 200;
	[SerializeField] GameObject releaseToRefreshPrefab;

	int contentChildCount;
	ScrollRect scrollRect;
	RectTransform scrollRectContent;
	Transform firstChild;
	RectTransform releaseToRefreshObject;

	void Start() {
		scrollRect = GetComponent<ScrollRect>();
		scrollRect.onValueChanged.AddListener(ReleaseToRefresh);
		scrollRectContent = scrollRect.content;
		UpdateChildCount();

		SetupReleaseToRefresh(true);
		DisableReleaseToRefresh();
		InitFeed();
	}

	void InitFeed() {
		scrollRect.content.DestroyChildren();
	}

	public void OnEndDrag(PointerEventData pointerEventData) {
		if (releaseToRefreshObject != null) {
			DisableReleaseToRefresh();
		}	
		if (!isLoading && LoadRequest != null) {
			CheckPxThreshold(() => LoadRequest(true), ()=> LoadRequest(false));
		}
	}

	void ReleaseToRefresh(Vector2 value) {
		if (!isLoading && !releaseToRefreshObject.gameObject.activeSelf) {
			CheckPxThreshold(
				()=> SetupReleaseToRefresh(true),
				()=> SetupReleaseToRefresh(false)
			);
		}
		else if(releaseToRefreshObject.gameObject.activeSelf && !CheckPxThreshold(null,null)) {
			DisableReleaseToRefresh();
		}
	}

	void SetupReleaseToRefresh(bool onTop) {
		scrollRect.StopMovement();
		if (releaseToRefreshObject == null) {
			releaseToRefreshObject = Instantiate(releaseToRefreshPrefab, scrollRect.viewport).GetComponent<RectTransform>();
		}
		else {
			releaseToRefreshObject.gameObject.SetActive(true);
		}
		RectTransform rt = releaseToRefreshObject;
		if (!onTop) {	
			rt.anchorMax = rt.anchorMin = rt.pivot = new Vector2(0.5f, 0);
			rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 0);
		}
		else {
			rt.anchorMax = rt.anchorMin = rt.pivot = new Vector2(0.5f, 1);
			rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 0);
		}
	}

	void DisableReleaseToRefresh() {
		releaseToRefreshObject.gameObject.SetActive(false);
	}

	bool CheckPxThreshold(Action pullUp, Action pullDown) {
		if (PixelPullUp() > pxPullThreshold) {
			if(pullUp != null) {
				pullUp();
			}
		}
		else if (PixelPullDown() > pxPullThreshold) {
			if(pullDown != null) {
				pullDown();
			}
		}
		else {
			return false;
		}
		return true;
	}

	float PixelPullDown() {
		return -scrollRect.verticalNormalizedPosition * scrollRectContent.rect.height;
	}

	float PixelPullUp() {
		return (scrollRectContent.pivot.y - 1) * scrollRectContent.sizeDelta.y - scrollRectContent.anchoredPosition.y;
	}

	public IEnumerator RecalculateOffset() {
		UpdateScrollRect();
		yield return null;
		UpdateScrollRect();		//this is important for Unity Layout Groups
		UpdateChildCount();
	}

	void UpdateScrollRect() {
		scrollRect.StopMovement();
		scrollRectContent.anchoredPosition = new Vector2(scrollRectContent.anchoredPosition.x, CalculateOffset());
	}

	float CalculateOffset() {
		int newChildCount = scrollRectContent.childCount;
		int difference = newChildCount - contentChildCount;
		float offset = 0;
		if (difference > 0) {
			offset = (layout.rowHeight + layout.spacing) * (difference - 1);
		}
		return offset;
	}

	public void UpdateChildCount() {
		contentChildCount = scrollRectContent.childCount;
		if (contentChildCount > 0) {
			firstChild = scrollRectContent.GetChild(0);
		}
		if(loadingAnimObject!=null) {
			contentChildCount--;
		}
	}
}
