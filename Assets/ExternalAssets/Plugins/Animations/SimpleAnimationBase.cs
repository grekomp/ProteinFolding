using ItSilesiaPlugins.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleAnimations {
	public abstract class SimpleAnimation : TriggerableAnimation {
		public float duration = 0.5f;
		public float delay = 0;
		public float backwardsDelay = 0;

	}
}
