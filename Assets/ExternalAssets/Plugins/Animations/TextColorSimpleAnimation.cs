using UnityEngine;
using System.Collections;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

namespace SimpleAnimations {
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class TextColorSimpleAnimation : SimpleAnimation {
		[Space]
		public Ease playEase = Ease.OutSine;
		public Ease playBackwardsEase = Ease.InSine;
		[Space]
		public ColorReference playTargetColor;
		public ColorReference playBackTargetColor;

		TextMeshProUGUI text;
		Tween tween;

		public override void Init() {
			text = GetComponent<TextMeshProUGUI>();
		}

		public override void ResetStateBackwards() {
			text.color = playTargetColor;
		}

		public override void ResetStateForwards() {
			text.color = playBackTargetColor;
		}

		public override void Stop() {
			if (tween != null) {
				tween.Kill();
			}
		}

		protected override void OnPlayBackwards() {
			Stop();
			tween = text.DOColor(playBackTargetColor, duration).SetDelay(backwardsDelay).SetEase(playBackwardsEase);
			tween.onComplete += () => PlayCompleted(false);
		}

		protected override void OnPlayForwards() {
			Stop();
			tween = text.DOColor(playTargetColor, duration).SetDelay(delay).SetEase(playEase);
			tween.onComplete += () => PlayCompleted(true);
		}
	}
}