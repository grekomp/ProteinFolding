using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleAnimations {
	public class SimpleAnimationsGroup : TriggerableAnimation {

		public bool ignoreParentGroup = true;
		public bool isOnAtStart = false;
		[SerializeField] bool manualInit = false;

		List<SimpleAnimation> animations = new List<SimpleAnimation>();
		int playForwardsToComplete;
		int playBackwardsToComplete;

		void GetAnimationsComponents(GameObject obj) {
			if (obj == null) {
				return;
			}

			if (animations == null) {
				animations = new List<SimpleAnimation>();
			}
			if (obj == gameObject) {
				animations.Clear();
			}

			foreach (Transform child in obj.transform) {
				if (child == null) {
					continue;
				}

				SimpleAnimation[] anims = child.GetComponents<SimpleAnimation>();
				foreach (SimpleAnimation a in anims) {
					animations.Add(a);
				}

				SimpleAnimationsGroup parent = child.GetComponent<SimpleAnimationsGroup>();
				if (parent != null) {
					if (parent.ignoreParentGroup) {
						continue;
					}
				}
				GetAnimationsComponents(child.gameObject);
			}
		}

		public override void Init() {
			GetAnimationsComponents(gameObject);
			InitAnimationsState();
		}

		protected override void OnPlayForwards() {
			foreach (SimpleAnimation anim in animations) {
				anim?.PlayForwards();
			}
			playForwardsToComplete = animations.Count;
		}

		protected override void OnPlayBackwards() {
			foreach (var anim in animations) {
				anim?.PlayBackwards();
			}
			playBackwardsToComplete = animations.Count;
		}

		void InitAnimationsState() {
			foreach (var anim in animations) {
				anim?.Init();
				anim?.ResetState(isOnAtStart);
				anim?.onPlayForwardsCompleted.AddListener((x) => OnSingleForwardsAnimCompleted());
				anim?.onPlayBackwardsCompleted.AddListener((x) => OnSingleBackwardsAnimCompleted());
			}
		}

		public override void Stop() {
			foreach (var anim in animations) {
				anim?.Stop();
			}
		}

		public override void ResetState(bool isForwards = true) {
			foreach (var anim in animations) {
				anim?.ResetState(isForwards);
			}
		}

		public override void ResetStateForwards() {
			foreach (var anim in animations) {
				anim?.ResetStateForwards();
			}
		}

		public override void ResetStateBackwards() {
			foreach (var anim in animations) {
				anim?.ResetStateBackwards();
			}
		}

		void OnSingleForwardsAnimCompleted() {
			if (playForwardsToComplete > 0) {
				playForwardsToComplete = playForwardsToComplete - 1;
			}
			if (playForwardsToComplete <= 0) {
				PlayForwardsCompleted();
			}
		}

		void OnSingleBackwardsAnimCompleted() {
			if (playBackwardsToComplete > 0) {
				playBackwardsToComplete = playBackwardsToComplete - 1;
			}
			if (playBackwardsToComplete <= 0) {
				PlayBackwardsCompleted();
			}
		}
	}
}