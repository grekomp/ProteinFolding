using System;
using UnityEngine;

namespace ItSilesiaPlugins.UI {
	public class InterfaceManager : MonoBehaviour {

		public Panel startPanel;

		PanelManager _panelManager;
		PanelManager panelManager {
			get {
				if (_panelManager != null) {
					return _panelManager;
				}
				_panelManager = GetComponentInChildren<PanelManager>();
				if (_panelManager == null) {
					_panelManager = gameObject.AddComponent<PanelManager>();
				}
				return _panelManager;
			}
			set {
				_panelManager = value;
			}
		}

		static Type nextStartPanelType;

		void Start() {
			this.InvokeAtNextFrame(LateStart);
		}

		void LateStart() {
			InitStartPanelFromPreviousScene();
			if (startPanel != null) {
				startPanel.Show();
			}
		}

		public static void SetNextStartPanel<T>() where T : Panel{
			nextStartPanelType = typeof(T);
		}

		public void CloseCurrentPanel() {
			panelManager.GoBack();
		}

		public Panel GetCurrentPanel() {
			return (Panel)panelManager.GetCurrentPanel();
		}

		public T GetPanel<T>() where T : Panel {
			return (T)panelManager.GetPanel<T>();
		}

		public void ShowPanel<T>() where T : Panel{
			panelManager.ShowPanel<T>();
		}

		public void ShowPanel(Panel panel) {
			panelManager.ShowPanel(panel);
		}

		public bool IsPanelOnScene(string panelName) {
			foreach (Panel panel in GetComponentsInChildren<Panel>(true)) {
				if (panel.name == panelName) {
					return true;
				}
			}
			return false;
		}

		void InitStartPanelFromPreviousScene() {
			if (nextStartPanelType != null) {
				Panel p = panelManager.GetPanel(nextStartPanelType);
				if (p != null) {
					startPanel = p;
					nextStartPanelType = null;
				}
			}
		}

		static InterfaceManager _instance;
		public static InterfaceManager instance {
			get {
				if (_instance != null) {
					return _instance;
				} else {
					_instance = FindObjectOfType<InterfaceManager>();
					if (_instance != null) {
						return _instance;
					} else {
						GameObject go = new GameObject("InterfaceManager");
						_instance = go.AddComponent<InterfaceManager>();
						Debug.LogError("Interface Manager not found on scene! Creating default one to avoid null exception.");
						return _instance;
					}
				}
			}
		}
	}
}