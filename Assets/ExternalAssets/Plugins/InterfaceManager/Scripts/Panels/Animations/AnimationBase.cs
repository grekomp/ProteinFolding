using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ItSilesiaPlugins.UI {
	public abstract class AnimationBase : MonoBehaviour, IAnimation {

		public float duration = 0.5f;
		public float delay = 0;
		public float backwardsDelay = 0;

		public Action onPlayCompleted;
		public Action onPlayBackwardsCompleted;

		public abstract void Init();
		public abstract void Play();
		public abstract void PlayBackwards();
		public abstract void Stop();
		public abstract void ResetState();

		public void CallOnPlayCompleted() {
			if (onPlayCompleted != null) {
				onPlayCompleted.Invoke();
			}
		}

		public void CallOnPlayBackwardsCompleted() {
			if (onPlayBackwardsCompleted != null) {
				onPlayBackwardsCompleted.Invoke();
			}
		}
	}
}