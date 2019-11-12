using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItSilesiaPlugins.UI {
	public class BackgroundManager : ParentBehaviour {

		[SerializeField] PanelManager panelManager;

		public void Start() {
			panelManager.onPanelChanged += CheckBackground;
		}

		public override void OnShow(ChildBehaviour child) {
			if (activeChild != null) {
				activeChild.Hide();
			}
			base.OnShow(child);
		}

		 void CheckBackground(Panel panel) {
			if (panel.gameObject.GetComponent<BackgroundGroupPanel>() == null) {
				if (activeChild != null) {
					activeChild.Hide();
					activeChild = null;
				}
			}
		}
	}
}
