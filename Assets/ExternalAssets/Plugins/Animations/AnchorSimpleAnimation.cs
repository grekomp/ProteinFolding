using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleAnimations {
	[RequireComponent(typeof(RectTransform))]
	public class AnchorSimpleAnimation : SimpleAnimation {

		public InitAnchor initAnchor;

		public Vector2 minAnchorTargetOnForward;
		public Vector2 maxAnchorTargetOnForward;

		public Vector2 minAnchorTargetOnBackwards;
		public Vector2 maxAnchorTargetOnBackwards;
		public Ease tweenEase = Ease.InOutSine;

		RectTransform rectTransform;
		Vector2 initAnchorMin;
		Vector2 initAnchorMax;
		Tweener tweenMin;
		Tweener tweenMax;

		public override void Init() {
			rectTransform = GetComponent<RectTransform>();
			SetInitAnchor();
		}

		protected override void OnPlayForwards() {
			StartTweenAnchors(minAnchorTargetOnForward, maxAnchorTargetOnForward, delay, true);
		}

		protected override void OnPlayBackwards() {
			StartTweenAnchors(minAnchorTargetOnBackwards, maxAnchorTargetOnBackwards, backwardsDelay, false);
		}

		public override void Stop() {
			tweenMin.Kill();
			tweenMax.Kill();
		}

		void StartTweenAnchors(Vector2 minTarget, Vector2 maxTarget, float delay, bool isForwards) {
			Stop();
			tweenMin = rectTransform.DOAnchorMin(minTarget, duration).SetDelay(delay).SetEase(tweenEase);
			tweenMax = rectTransform.DOAnchorMax(maxTarget, duration).SetDelay(delay).SetEase(tweenEase);
			tweenMax.onComplete += () => PlayCompleted(isForwards);
		}

		void SetInitAnchor() {
			switch (initAnchor) {
				case InitAnchor.Current:
					initAnchorMin = rectTransform.anchorMin;
					initAnchorMax = rectTransform.anchorMax;
					break;
				case InitAnchor.Forward:
					initAnchorMin = minAnchorTargetOnForward;
					initAnchorMax = maxAnchorTargetOnForward;
					break;
				case InitAnchor.Backwards:
					initAnchorMin = minAnchorTargetOnBackwards;
					initAnchorMax = maxAnchorTargetOnBackwards;
					break;
			}

		}

		public override void ResetStateForwards() {
			rectTransform.anchorMin = initAnchorMin;
			rectTransform.anchorMax = initAnchorMax;
		}

		public override void ResetStateBackwards() {
			ResetStateForwards();
		}

		public enum InitAnchor { Current, Forward, Backwards }
	}
}