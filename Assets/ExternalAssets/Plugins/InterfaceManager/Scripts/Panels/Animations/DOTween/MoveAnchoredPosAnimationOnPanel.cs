using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ItSilesiaPlugins.UI {
	public class MoveAnchoredPosAnimationOnPanel : AnimationBase {
		[Space]
		public Vector2 startPosition;
		public Vector2 endPosition;

		RectTransform _rectTransform;
		public RectTransform rectTransform {
			get {
				if(_rectTransform == null) {
					_rectTransform = GetComponent<RectTransform>();
				}
				return _rectTransform;
			}

			set {
				_rectTransform = value;
			}
		}

		Sequence sequence;
	
		public override void Init() {
			rectTransform = GetComponent<RectTransform>();
		}

		public override void Play() {
			rectTransform.anchoredPosition = startPosition;
			sequence = DOTween.Sequence()
				.SetDelay(delay)
				.Append(rectTransform.DOAnchorPos(endPosition, duration))
				.AppendCallback(CallOnPlayCompleted);
		}

		public override void PlayBackwards() {
			rectTransform.anchoredPosition = endPosition;
			sequence = DOTween.Sequence()
				.SetDelay(backwardsDelay)
				.Append(rectTransform.DOAnchorPos(startPosition, duration))
				.AppendCallback(CallOnPlayBackwardsCompleted);
		}

		public override void Stop() {
			sequence.Kill();
		}

		public override void ResetState() {
			rectTransform.anchoredPosition = startPosition;
		}
	}
}