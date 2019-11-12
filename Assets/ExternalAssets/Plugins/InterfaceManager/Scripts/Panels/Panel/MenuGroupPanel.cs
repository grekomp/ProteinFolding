using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItSilesiaPlugins.UI {
	public class MenuGroupPanel : EventHandler {

		public MenuGroup menuGroup;

		protected override void OnShow() {
			if(menuGroup != null) {
				menuGroup.Show();
			}
		}
	}
}
