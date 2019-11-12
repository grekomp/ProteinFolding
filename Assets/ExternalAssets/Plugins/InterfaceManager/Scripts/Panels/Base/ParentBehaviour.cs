using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItSilesiaPlugins.UI {
	public abstract class ParentBehaviour : MonoBehaviour {

		[HideInInspector] public  ChildBehaviour activeChild;

		protected List<ChildBehaviour> children;
		protected RectTransform rectTransform;

		bool isInitialized;

		protected void Awake() {
			InitializeIfNeeded();
		}

		protected void Start() {
			HideAllChildreen();
		}

		public virtual void OnShow(ChildBehaviour child) {
			activeChild = child;
		}

        public virtual void OnHide(ChildBehaviour child) {
			activeChild = null;
        }

		public virtual void OnReset(ChildBehaviour child) {
			activeChild = null;
		}

		public bool IsOnTop(ChildBehaviour child) {
			return child == activeChild;
		}

		protected T GetChild<T>() where T : ChildBehaviour {
			return (T)GetChildByType(typeof(T));
		}

		protected ChildBehaviour GetChildByType(System.Type type) {
			InitializeIfNeeded();
			for (int i = 0; i < children.Count; i++) {
				if (children[i].GetType() == type) {
					return children[i];
				}
			}
			return null;
		}

		void Initialize() {
			isInitialized = true;
			rectTransform = GetComponent<RectTransform>();
			children = new List<ChildBehaviour>(GetComponentsInChildren<ChildBehaviour>(true));
			foreach (ChildBehaviour child in children) {
				child.SetParent(this);
				child.SetState(false);
				child.Init();
			}
		}

		protected void HideAllChildreen() {
			foreach (ChildBehaviour child in children) {
				child.Hide();
			}
		}

		protected void ResetAllChildreen() {
			foreach (ChildBehaviour child in children) {
				child.Reset();
			}
		}

		void InitializeIfNeeded() {
			if (isInitialized == false) {
				Initialize();
			}
		}
	}
}
