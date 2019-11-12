using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleAnimations {
	[RequireComponent(typeof(Image))]
	public class FillAmountSimpleAnimation : SimpleAnimation {

		Tween tween;
		Image img;

		float playTargetFillAmount = 1;
		float playBackTargetFillAmount = 0;

		public override void Init() {
			img = GetComponent<Image>();
		}

		protected override void OnPlayForwards() {
			PlayTween(playTargetFillAmount, delay, Ease.OutCirc);
		}

		protected override void OnPlayBackwards() {
			PlayTween(playBackTargetFillAmount, backwardsDelay, Ease.InCirc);
		}

		public override void Stop() {
			if (tween != null) {
				tween.Kill();
			}
		}

		void PlayTween(float target, float delay, Ease ease) {
			Stop();
			tween = img.DOFillAmount(target, duration).SetDelay(delay).SetEase(ease);
		}

		public override void ResetStateForwards() {
			img.fillAmount = playBackTargetFillAmount;
		}

		public override void ResetStateBackwards() {
			img.fillAmount = playTargetFillAmount;
		}
	}
}