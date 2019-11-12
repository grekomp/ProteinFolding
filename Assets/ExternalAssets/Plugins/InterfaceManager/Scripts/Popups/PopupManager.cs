using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItSilesiaPlugins.UI {
	public class PopupManager : ParentBehaviour {

		public void ShowPopup<T>() where T : Popup {
			Popup p = GetChild<T>();
			ShowPopup(p);
		}

		public void ShowPopup(Popup p) {
			if (p != null) {
				p.Show();
			}
		}

		public void ShowPopup(string popupName) {
			foreach(ChildBehaviour child in children) {
				if(child.name == popupName) {
					child.Show();
				}
			}
		}

		public void HideAll() {
			HideAllChildreen();
		}

		public void ResetAll() {
			ResetAllChildreen();
		}
	}
}
