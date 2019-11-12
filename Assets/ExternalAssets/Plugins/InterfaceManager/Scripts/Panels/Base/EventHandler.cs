using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItSilesiaPlugins.UI {
	public abstract class EventHandler : MonoBehaviour {

		protected ChildBehaviour child;

		protected virtual void OnInit() { }
		protected virtual void OnShow() { }
		protected virtual void OnHide() { }
		protected virtual void OnReset() { }

		public void Init(ChildBehaviour child) {
			this.child = child;
			child.onInit += OnInit;
			child.onShow += OnShow;
			child.onHide += OnHide;
			child.onReset += OnReset;
		}
	}
}