using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleAnimations {
	public class PingSimpleAnimation : SimpleAnimation {

		RectTransform rectTransform;
		Tween tween;

		public override void Init() {
			rectTransform = GetComponent<RectTransform>();
		}

		protected override void OnPlayForwards() {
			PlayTween(0.25f, delay, false);
		}

		protected override void OnPlayBackwards() {
			PlayTween(0.25f, backwardsDelay, true);
		}

		public override void Stop() {
			if (tween != null) {
				tween.Kill();
			}
		}

		void PlayTween(float punch, float delay, bool isBackwards) {
			Stop();
			ResetState(isBackwards);
			tween = rectTransform.DOPunchScale(punch * Vector3.one, duration, vibrato: 5).SetDelay(delay);
			tween.onComplete += () => PlayCompleted(!isBackwards);

			if (isBackwards) {
				tween.onComplete += PlayBackwardsCompleted;
			}
			else {
				tween.onComplete += PlayForwardsCompleted;
			}
		}

		public override void ResetStateForwards() {
			Stop();
			rectTransform.localScale = Vector3.one;
		}

		public override void ResetStateBackwards() {
			Stop();
			ResetStateForwards();
		}
	}
}