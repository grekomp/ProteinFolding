using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItSilesiaPlugins.UI {
	public class BackgroundGroupPanel : EventHandler {

		public BackgroundGroup background;

		protected override void OnShow() {
			if (background != null) {
				background.Show();
			}
		}
	}
}