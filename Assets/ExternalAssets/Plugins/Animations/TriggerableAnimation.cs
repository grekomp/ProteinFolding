using UnityEngine;
using System.Collections;
using System;
using ItSilesiaPlugins.UI;
using UnityEngine.Events;

namespace SimpleAnimations {
	[ExecuteAlways]
	public abstract class TriggerableAnimation : DisposableMonoBehaviour {
		public BoolUnityEvent onPlayCompleted;
		public BoolUnityEvent onPlayForwardsCompleted;
		public BoolUnityEvent onPlayBackwardsCompleted;

		public bool playOnAwake = false;
		public bool resetStateOnAwake = true;
		public bool resetStateForwards = true;

		public abstract void Init();
		public virtual void Play(bool isForward) {
			if (isForward) {
				PlayForwards();
			}
			else {
				PlayBackwards();
			}
		}

		[ContextMenu("Play Forwards")]
		public virtual void PlayForwards() {
#if UNITY_EDITOR
			if (Application.isPlaying) {
				OnPlayForwards();
			}
			else {
				ResetStateBackwards();
			}
#else
			OnPlayForwards();
#endif
		}
		protected abstract void OnPlayForwards();

		[ContextMenu("Play Backwards")]
		public virtual void PlayBackwards() {
#if UNITY_EDITOR
			if (Application.isPlaying) {
				OnPlayBackwards();
			}
			else {
				ResetStateForwards();
			}
#else
			OnPlayBackwards();
#endif
		}
		protected abstract void OnPlayBackwards();

		[ContextMenu("Stop")]
		public abstract void Stop();
		[ContextMenu("Reset State Forwards")]
		public abstract void ResetStateForwards();
		[ContextMenu("Reset State Backwards")]
		public abstract void ResetStateBackwards();
		public virtual void ResetState(bool isForwards = true) {
			if (isForwards) {
				ResetStateForwards();
			}
			else {
				ResetStateBackwards();
			}
		}

		protected virtual void PlayCompleted(bool isForwards = true) {
			if (isForwards) {
				PlayForwardsCompleted();
			}
			else {
				PlayBackwardsCompleted();
			}
		}

		protected virtual void PlayForwardsCompleted() {
			OnPlayForwardsCompleted();
		}

		private void OnPlayForwardsCompleted() {
			onPlayCompleted.Invoke(true);
			onPlayForwardsCompleted.Invoke(true);
		}

		protected virtual void PlayBackwardsCompleted() {
			OnPlayBackwardsCompleted();
		}

		private void OnPlayBackwardsCompleted() {
			onPlayCompleted.Invoke(false);
			onPlayBackwardsCompleted.Invoke(false);
		}

		protected virtual void Awake() {
			Init();

			if (resetStateOnAwake) ResetState(resetStateForwards);
			if (playOnAwake) PlayForwards();
		}

		#region Disposable
		protected override IEnumerator HandleDispose(Action onDisposed = null) {
			if (onDisposed != null) {
				UnityAction<bool> callback = null;
				onPlayBackwardsCompleted.AddListener(callback = (b) => {
					onDisposed.Invoke();
					onPlayBackwardsCompleted.RemoveListener(callback);
				});
			}
			PlayBackwards();
			yield return 0;
		}
		#endregion
	}
}