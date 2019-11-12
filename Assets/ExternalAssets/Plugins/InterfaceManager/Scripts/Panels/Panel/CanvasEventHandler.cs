using UnityEngine;
using UnityEngine.UI;

namespace ItSilesiaPlugins.UI {
	[RequireComponent(typeof(Canvas), typeof(GraphicRaycaster))]
	public class CanvasEventHandler : EventHandler {

		Canvas canvas;
		GraphicRaycaster graphicRaycaster;
		
		protected override void OnInit() {
			canvas = GetComponent<Canvas>();
			graphicRaycaster = GetComponent<GraphicRaycaster>();
		}

		protected override void OnShow() {
			canvas.enabled = true;
			graphicRaycaster.enabled = true;
		}

		protected override void OnHide() {
			canvas.enabled = false;
			graphicRaycaster.enabled = false;
		}

		protected override void OnReset() {
			OnHide();
		}
	}
}
