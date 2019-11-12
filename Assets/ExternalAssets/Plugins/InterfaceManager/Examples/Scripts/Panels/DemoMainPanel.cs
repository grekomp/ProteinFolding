using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ItSilesiaPlugins.UI {
	public class DemoMainPanel : Panel {

		public Image colorizedImage;

		List<Color> colors;
		int colorIndex;

		protected override void OnInit() {
			colorIndex = 0;
			GenerateRandomColors(10);
		}

		protected override void OnShow() {
			colorizedImage.color = GetNextColor();
		}

		protected override void OnHide() {
			colorizedImage.color = Color.black;
		}

		void GenerateRandomColors(int count) {
			colors = new List<Color>();
			for (int i = 0; i < count; i++) {
				colors.Add(Random.ColorHSV(0.1f, 0.25f, 0.75f, 1f, 0.75f, 1f));
			}
		}

		Color GetNextColor() {
			colorIndex++;
			colorIndex = colorIndex % colors.Count;
			return colors[colorIndex];
		}
	}
}