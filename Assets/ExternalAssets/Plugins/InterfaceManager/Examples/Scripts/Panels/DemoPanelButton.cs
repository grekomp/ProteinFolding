using UnityEngine;

namespace ItSilesiaPlugins.UI {
	public class DemoPanelButton : MonoBehaviour {

		public void OnClick() {
			Debug.Log("Showed Main Panel from script");
			InterfaceManager.instance.ShowPanel<DemoMainPanel>();
		}
	}
}
