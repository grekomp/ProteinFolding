using DG.Tweening;
using UnityEngine;
namespace ItSilesiaPlugins.UI {
	public class MoveAnchorOffsetAnimationOnPanel : AnimationBase {
        [Space]
		public Vector2 moveInOffset;
		public Vector2 moveOutOffset;

		Vector2 initAnchorMin;
		Vector2 initAnchorMax;

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

		Tweener tweenMin;
		Tweener tweenMax;

		public override void Init() {
			rectTransform = GetComponent<RectTransform>();
			initAnchorMin = rectTransform.anchorMin;
			initAnchorMax = rectTransform.anchorMax;
		}

		public override void Play() {
			SetAnchors(moveInOffset);
            tweenMin = rectTransform.DOAnchorMin(initAnchorMin, duration)
                .SetDelay(delay);
			tweenMax = rectTransform.DOAnchorMax(initAnchorMax, duration)
				.SetDelay(delay)
				.OnComplete(CallOnPlayCompleted);
		}

		public override void PlayBackwards() {
			SetAnchors(Vector2.zero);
            tweenMin = rectTransform.DOAnchorMin(initAnchorMin + moveOutOffset, duration)
                .SetDelay(backwardsDelay);
            tweenMax = rectTransform.DOAnchorMax(initAnchorMax + moveOutOffset, duration)
				.SetDelay(backwardsDelay)
				.OnComplete(CallOnPlayBackwardsCompleted);
		}

		public override void Stop() {
			tweenMin.Kill();
			tweenMax.Kill();
		}

		public override void ResetState() {
			rectTransform.anchorMin = initAnchorMin;
			rectTransform.anchorMax = initAnchorMax;
		}

		void SetAnchors(Vector2 offset) {
			rectTransform.anchorMin = initAnchorMin + offset;
			rectTransform.anchorMax = initAnchorMax + offset;
		}
	}
}