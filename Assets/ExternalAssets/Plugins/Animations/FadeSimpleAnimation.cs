using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleAnimations {
	[RequireComponent(typeof(CanvasGroup))]
	public class FadeSimpleAnimation : SimpleAnimation {
		[Space]
		public Ease playEase = Ease.OutSine;
		public Ease playBackwardsEase = Ease.InSine;
		[Space]
		public float playTargetAlpha = 1;
		public bool playTargetInteractible = true;
		public bool playTargetBlocksRaycasts = true;
		[Space]
		public float playBackTargetAlpha = 0;
		public bool playBackTargetInteractible = false;
		public bool playBackTargetBlocksRaycasts = false;

		CanvasGroup canvasGroup;
		Tween tween;

		public override void Init() {
			canvasGroup = GetComponent<CanvasGroup>();
		}

		protected override void OnPlayForwards() {
			PlayTween(playTargetAlpha, delay, playEase, true);
		}

		protected override void OnPlayBackwards() {
			PlayTween(playBackTargetAlpha, backwardsDelay, playBackwardsEase, false);
		}

		public override void Stop() {
			if (tween != null) {
				tween.Kill();
			}
		}

		void PlayTween(float targetAlpha, float delay, Ease ease, bool isForwards) {
			Stop();
			tween = canvasGroup.DOFade(targetAlpha, duration).SetDelay(delay).SetEase(ease);
			tween.onComplete += () => PlayCompleted(isForwards);
		}

		public override void ResetStateForwards() {
			Stop();
			canvasGroup.alpha = playBackTargetAlpha;
			canvasGroup.interactable = playBackTargetInteractible;
			canvasGroup.blocksRaycasts = playBackTargetBlocksRaycasts;
		}

		public override void ResetStateBackwards() {
			Stop();
			canvasGroup.alpha = playTargetAlpha;
			canvasGroup.interactable = playTargetInteractible;
			canvasGroup.blocksRaycasts = playTargetBlocksRaycasts;
		}

		protected override void PlayForwardsCompleted() {
			canvasGroup.interactable = playTargetInteractible;
			canvasGroup.blocksRaycasts = playTargetBlocksRaycasts;
			base.PlayForwardsCompleted();
		}
		protected override void PlayBackwardsCompleted() {
			canvasGroup.interactable = playBackTargetInteractible;
			canvasGroup.blocksRaycasts = playBackTargetBlocksRaycasts;
			base.PlayBackwardsCompleted();
		}
	}
}