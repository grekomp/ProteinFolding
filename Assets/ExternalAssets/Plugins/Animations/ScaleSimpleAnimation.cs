using DG.Tweening;
using UnityEngine;

namespace SimpleAnimations {
	public class ScaleSimpleAnimation : SimpleAnimation {
		public Ease easeForwards = Ease.OutBack;
		public Ease easeBackwards = Ease.InBack;
		public float easeOvershootOrAmplitude = 1.70158f;

		public float playTargetScale = 1;
		public float playBackTargetScale = 0;

		Tween tween;

		public override void Init() { }

		protected override void OnPlayForwards() {
			PlayTween(playTargetScale, delay, easeForwards, true);
		}

		protected override void OnPlayBackwards() {
			PlayTween(playBackTargetScale, backwardsDelay, easeBackwards, false);
		}

		public override void Stop() {
			if (tween != null) {
				tween.Kill();
			}
		}

		void PlayTween(float targetScale, float delay, Ease ease, bool isForwards) {
			Stop();
			tween = transform.DOScale(targetScale, duration).SetDelay(delay).SetEase(ease);
			tween.onComplete += () => PlayCompleted(isForwards);
			tween.easeOvershootOrAmplitude = easeOvershootOrAmplitude;
		}

		public override void ResetStateForwards() {
			Stop();
			transform.localScale = Vector3.one * playBackTargetScale;
		}
		public override void ResetStateBackwards() {
			Stop();
			transform.localScale = Vector3.one * playTargetScale;
		}
	}
}