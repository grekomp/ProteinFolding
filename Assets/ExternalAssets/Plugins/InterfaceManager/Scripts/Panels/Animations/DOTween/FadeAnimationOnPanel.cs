using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ItSilesiaPlugins.UI {
	[RequireComponent(typeof(CanvasGroup))]
	public class FadeAnimationOnPanel : AnimationBase {

		CanvasGroup canvasGroup;
		Sequence sequence;

		public override void Init() {
			canvasGroup = GetComponent<CanvasGroup>();
		}

		public override void Play() {
			canvasGroup.alpha = 0;
			sequence = DOTween.Sequence()
				.SetDelay(delay)
				.Append(DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1, duration))
				.AppendCallback(CallOnPlayCompleted);
		}

		public override void PlayBackwards() {
			canvasGroup.alpha = 1;
			sequence = DOTween.Sequence()
				.SetDelay(backwardsDelay)
				.Append(DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0, duration))
				.AppendCallback(CallOnPlayBackwardsCompleted);
		}

		public override void ResetState() {
			canvasGroup.alpha = 0;
		}

		public override void Stop() {
			sequence.Kill();
		}
	}
}
