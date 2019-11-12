using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace SimpleAnimations {
	public class RotationSimpleAnimation : SimpleAnimation {
		public Ease easeForwards = Ease.OutBack;
		public Ease easeBackwards = Ease.InBack;
		[Space]
		public Vector3 playTargetRotation = Vector3.zero;
		public Vector3 playBackTargetRotation = new Vector3(0, 90, 0);

		Tweener currentTween;

		public override void Init() { }

		public override void ResetStateBackwards() {
			transform.rotation = Quaternion.Euler(playTargetRotation);
		}

		public override void ResetStateForwards() {
			transform.rotation = Quaternion.Euler(playBackTargetRotation);
		}

		public override void Stop() {
			currentTween?.Pause();
		}

		protected override void OnPlayBackwards() {
			currentTween?.Pause();
			currentTween?.Kill();
			currentTween = transform.DORotate(playBackTargetRotation, duration).SetDelay(delay).SetEase(easeBackwards);
		}

		protected override void OnPlayForwards() {
			currentTween?.Pause();
			currentTween?.Kill();
			currentTween = transform.DORotate(playTargetRotation, duration).SetDelay(delay).SetEase(easeForwards);
		}
	}
}