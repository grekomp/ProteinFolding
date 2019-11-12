using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItSilesiaPlugins.UI {
	public class MenuManager : ParentBehaviour {

		public override void OnShow(ChildBehaviour child) {
			if (activeChild != null) {
				activeChild.Hide();
			}
			base.OnShow(child);
		}
	}
}
