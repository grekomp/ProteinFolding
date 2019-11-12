using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ItSilesiaPlugins.UI {
	[RequireComponent(typeof(Canvas), typeof(GraphicRaycaster))]
	public class AnimationsEventHandler : EventHandler {

		public float onShowDelay;
		public float onHideDelay;

		bool isShowInProgress;
		List<AnimationBase> animations;
		int showToComplete;
		int hideToComplete;
		Canvas canvas;

		protected override void OnInit() {
			canvas = GetComponent<Canvas>();
			OnReset();
			InitAnimations();
		}

		protected override void OnReset() {
			hideToComplete = 0;
			OnPlayBackwardsComplete();
		}

		protected override void OnShow() {
			canvas.enabled = true;
			showToComplete = animations.Count;
			foreach (AnimationBase anim in animations) {
				anim.Stop();
				anim.Play();
			}
		}

		protected override void OnHide() {
			StopAnimations();
			hideToComplete = animations.Count;
			if (hideToComplete == 0) {
				OnPlayBackwardsComplete();
			}
			else {
				foreach (AnimationBase anim in animations) {
					anim.Stop();
					anim.PlayBackwards();
				}
			}
		}

		void InitAnimations() {
			animations = new List<AnimationBase>(GetComponentsInChildren<AnimationBase>(true));
			foreach (AnimationBase anim in animations) {
				anim.Init();
				anim.ResetState();
				anim.onPlayCompleted += OnPlayAnimationComplete;
				anim.delay += onShowDelay;
				anim.onPlayBackwardsCompleted += OnPlayBackwardsComplete;
				anim.backwardsDelay += onHideDelay;
			}
		}

		void StopAnimations() {
			foreach (AnimationBase anim in animations) {
				anim.Stop();
			}
		}

		void OnPlayAnimationComplete() {
			if (showToComplete > 0) {
				showToComplete = showToComplete - 1;
			}
		}

		void OnPlayBackwardsComplete() {
			if (hideToComplete > 0) {
				hideToComplete = hideToComplete - 1;
			}
			if (hideToComplete <= 0) {
				canvas.enabled = false;
			}
		}
	}
}