using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ItSilesiaPlugins.UI {
	public class ScaleAnimationOnPanel : AnimationBase {

		public Ease ease;
		public Ease backwardsEase;
		public float overshootOrAmplitude = 1.70158f;
		Sequence sequence;	

		public override void Init() { }

		public override void Play() {
			transform.localScale = Vector3.zero;
			Tween tween = transform.DOScale(new Vector3(1, 1, 1), duration)
					.SetEase(ease);
			tween.easeOvershootOrAmplitude = overshootOrAmplitude;
			sequence = DOTween.Sequence()
				.SetDelay(delay)
				.Append(tween)
				.AppendCallback(CallOnPlayCompleted);
		}

		public override void PlayBackwards() {
			transform.localScale = Vector3.one;
			Tween tween = transform.DOScale(new Vector3(0, 0, 0), duration)
				.SetEase(backwardsEase);
			tween.easeOvershootOrAmplitude = overshootOrAmplitude;
			sequence = DOTween.Sequence()
				.SetDelay(backwardsDelay)
				.Append(tween)
				.AppendCallback(CallOnPlayBackwardsCompleted);
		}

		public override void Stop() {
			sequence.Kill();
		}

		public override void ResetState() {
			transform.localScale = Vector3.zero;
		}
	}
}