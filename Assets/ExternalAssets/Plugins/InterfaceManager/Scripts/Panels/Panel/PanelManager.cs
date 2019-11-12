using System;
using System.Collections.Generic;
using UnityEngine;

namespace ItSilesiaPlugins.UI {
	public class PanelManager : ParentBehaviour {

		public Action<Panel> onPanelChanged;

		readonly Stack<Panel> history = new Stack<Panel>();

		public override void OnShow(ChildBehaviour child) {
			if(activeChild == child) {
				return;
			}
			HideCurrentPanel();
			Panel panel = (Panel)child;
			RollBackHistoryIfRequired(panel);
			history.Push(panel);
			if(onPanelChanged != null) {
				onPanelChanged(panel);
			}
			base.OnShow(child);
		}

		public Panel GetCurrentPanel() {
			return history.Count > 0 ? history.Peek() : null;
		}

		public Panel GetPreviousPanel() {
			if(history.Count > 1) {
				Panel current = history.Pop();
				Panel prev = history.Peek();
				history.Push(current);
				return prev;
			}
			else {
				return null;
			}
		}

		public Panel GetPanel(System.Type type) {
			return (Panel)GetChildByType(type);
		}

		public Panel GetPanel<T>() where T : Panel {
			return GetChild<T>();
		}

		public void ShowPanel<T>() where T : Panel {
			Panel panel = GetChild<T>();
			ShowPanel(panel);
		}

		public void ShowPanel(Panel panel) {
			if (panel != null) {
				panel.Show();
			} else {
				Debug.LogError("[PANEL MANAGER] Panel to show is null!");
			}
		}

		public void GoBack() {
			if (history.Count > 1) {
				history.Pop().Close();
				history.Pop().Show();
			}
		}

		void HideCurrentPanel() {
			Panel prevPanel = GetCurrentPanel();
			if (prevPanel != null) {
				prevPanel.Hide();
			}
		}

		void RollBackHistoryIfRequired(Panel panel) {
			if (history.Contains(panel)) {
				Panel view = history.Pop();
				while (view != panel) {
					view.Close();
					view = history.Pop();
				}
			}
		}
	}
}