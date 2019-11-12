using System.Collections.Generic;
using UnityEngine;

namespace ItSilesiaPlugins.UI {
	public class RenderersEventHandler : EventHandler {

		List<MeshRenderer> renderers;
		List<Collider> colliders;

		protected override void OnInit() {
			renderers = new List<MeshRenderer>(GetComponentsInChildren<MeshRenderer>());
			colliders = new List<Collider>(GetComponentsInChildren<Collider>());
		}

		protected override void OnShow() {
			SetChildrenActive(true);
		}

		protected override void OnHide() {
			SetChildrenActive(false);
		}

		protected override void OnReset() {
			SetChildrenActive(false);
		}

		void SetChildrenActive(bool isActive) {
			foreach(MeshRenderer renderer in renderers) {
				renderer.enabled = isActive;
			}
			foreach (Collider collider in colliders) {
				collider.enabled = isActive;
			}
		}
	}
}
