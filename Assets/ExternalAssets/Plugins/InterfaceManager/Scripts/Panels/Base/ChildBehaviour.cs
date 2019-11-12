using System;
using UnityEngine;

namespace ItSilesiaPlugins.UI {
	public abstract class ChildBehaviour : MonoBehaviour {

		public event Action onInit;
		public event Action onShow;
		public event Action onHide;
		public event Action onReset;

		protected ParentBehaviour parent;
		protected bool isVisible;

		public void Init() {
			gameObject.SetActive(true);
			InitEventHandlers();
			OnInit();
			if (onInit != null) {
				onInit.Invoke();
			}
		}

		public void Show() {
			if (!parent.IsOnTop(this)) {
				isVisible = true;
				parent.OnShow(this);
				OnShow();
				if (onShow != null) {
					onShow.Invoke();
				}
			}
		}

		public void Hide() {
			isVisible = false;
            parent.OnHide(this);
            OnHide();
			if (onHide != null) {
				onHide.Invoke();
			}
		}

		public void Close() {
			if (isVisible) {
				Hide();
			}
		}

		public void Reset() {
			isVisible = false;
			OnReset();
			parent.OnReset(this);
			if (onReset != null) {
				onReset.Invoke();
			}
		}

		void Update() {
			if (isVisible) {
				OnUpdate();
			}
		}

		public void SetParent(ParentBehaviour parent) {
			this.parent = parent;
		}

		public void SetState(bool state) {
			isVisible = state;
		}

		protected virtual void OnInit() { }
		protected virtual void OnShow() { }
		protected virtual void OnHide() { }
		protected virtual void OnReset() { }
		protected virtual void OnUpdate() { }

		void InitEventHandlers() {
			EventHandler[] eventHandlers = GetComponentsInChildren<EventHandler>(true);
			for (int i = 0; i < eventHandlers.Length; i++) {
				eventHandlers[i].Init(this);
			}
		}
	}
}
