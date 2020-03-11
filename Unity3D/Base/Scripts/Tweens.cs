using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RatKing.Base {
	
	// very simple tweening class, just to be independent from plugins like LeanTween
	public class Tweens : MonoBehaviour {

		public static class Ease {

			public static float Linear(float t) {
				return t;
			}

			public static float SmoothStep(float t) {
				return Mathf.SmoothStep(0f, 1f, t);
			}

			// easings from: https://github.com/ai/eaMathf.Sings.net/blob/master/src/eaMathf.Sings/eaMathf.SingsFunctions.ts

			static readonly float PI = Mathf.PI;
			static readonly float c1 = 1.70158f;
			static readonly float c2 = c1 * 1.525f;
			static readonly float c3 = c1 + 1f;
			static readonly float c4 = (2f * PI) / 3f;
			static readonly float c5 = (2f * PI) / 4.5f;

			static float BounceOut(float t) {
				float n1 = 7.5625f;
				float d1 = 2.75f;
				if (t < 1f / d1) { return n1 * t * t; }
				else if (t < 2f / d1) { return n1 * (t -= 1.5f / d1) * t + 0.75f; }
				else if (t < 2.5f / d1) { return n1 * (t -= 2.25f / d1) * t + 0.9375f; }
				return n1 * (t -= 2.625f / d1) * t + 0.984375f;
			}

			public static float InQuad(float t) {
				return t * t;
			}
			public static float OutQuad(float t) {
				return 1f - (1f - t) * (1f - t);
			}
			public static float InOutQuad(float t) {
				return t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) * 0.5f;
			}
			public static float InCubic(float t) {
				return t * t * t;
			}
			public static float OutCubic(float t) {
				return 1f - Mathf.Pow(1f - t, 3f);
			}
			public static float InOutCubic(float t) {
				return t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) * 0.5f;
			}
			public static float InQuart(float t) {
				return t * t * t * t;
			}
			public static float OutQuart(float t) {
				return 1f - Mathf.Pow(1f - t, 4f);
			}
			public static float InOutQuart(float t) {
				return t < 0.5f ? 8f * t * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 4f) * 0.5f;
			}
			public static float InQuint(float t) {
				return t * t * t * t * t;
			}
			public static float OutQuint(float t) {
				return 1f - Mathf.Pow(1f - t, 5f);
			}
			public static float InOutQuint(float t) {
				return t < 0.5f ? 16f * t * t * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 5f) * 0.5f;
			}
			public static float InSin(float t) {
				return 1f - Mathf.Cos((t * PI) * 0.5f);
			}
			public static float OutSin(float t) {
				return Mathf.Sin((t * PI) * 0.5f);
			}
			public static float InOutSin(float t) {
				return -(Mathf.Cos(PI * t) - 1f) * 0.5f;
			}
			public static float InExpo(float t) {
				return t == 0f ? 0f : Mathf.Pow(2f, 10f * t - 10f);
			}
			public static float OutExpo(float t) {
				return t == 1f ? 1f : 1f - Mathf.Pow(2f, -10f * t);
			}
			public static float InOutExpo(float t) {
				return t == 0f ? 0f
					: t == 1f ? 1f
					: t < 0.5f ? Mathf.Pow(2f, 20f * t - 10f) * 0.5f
					: (2f - Mathf.Pow(2f, -20f * t + 10f)) * 0.5f;
			}
			public static float InCirc(float t) {
				return 1f - Mathf.Sqrt(1f - Mathf.Pow(t, 2f));
			}
			public static float OutCirc(float t) {
				return Mathf.Sqrt(1f - Mathf.Pow(t - 1f, 2f));
			}
			public static float InOutCirc(float t) {
				return t < 0.5f
					? (1f - Mathf.Sqrt(1f - Mathf.Pow(2f * t, 2f))) * 0.5f
					: (Mathf.Sqrt(1f - Mathf.Pow(-2f * t + 2f, 2f)) + 1f) * 0.5f;
			}
			public static float InBack(float t) {
				return c3 * t * t * t - c1 * t * t;
			}
			public static float OutBack(float t) {
				return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
			}
			public static float InOutBack(float t) {
				return t < 0.5f
					? (Mathf.Pow(2f * t, 2f) * ((c2 + 1f) * 2f * t - c2)) * 0.5f
					: (Mathf.Pow(2f * t - 2f, 2f) * ((c2 + 1f) * (t * 2f - 2f) + c2) + 2f) * 0.5f;
			}
			public static float InElastic(float t) {
				return t == 0f ? 0f
					: t == 1f ? 1f
					: -Mathf.Pow(2f, 10f * t - 10f) * Mathf.Sin((t * 10f - 10.75f) * c4);
			}
			public static float OutElastic(float t) {
				return t == 0f ? 0f
					: t == 1f ? 1f
					: Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * c4) + 1f;
			}
			public static float InOutElastic(float t) {
				return t == 0f ? 0f
					: t == 1f ? 1f
					: t < 0.5f ? -(Mathf.Pow(2f, 20f * t - 10f) * Mathf.Sin((20f * t - 11.125f) * c5)) * 0.5f
					: (Mathf.Pow(2f, -20f * t + 10f) * Mathf.Sin((20f * t - 11.125f) * c5)) * 0.5f + 1f;
			}
			public static float InBounce(float t) {
				return 1f - BounceOut(1f - t);
			}
			public static float OutBounce(float t) {
				return BounceOut(t);
			}
			public static float InOutBounce(float t) {
				return t < 0.5f ? (1f - BounceOut(1f - 2f * t)) * 0.5f
					: (1f + BounceOut(2f * t - 1f)) * 0.5f;
			}

		}

		//

		public class Tween {
			public string name = "";
			public float factor = 0f;
			public float speed = 0f;
			public float start;
			public float end;
			public System.Func<float, float> easeFunc;
			public System.Action<float> updateFunc;
			public System.Action completeFunc;
			public float delay;
			public int id;
			public bool ignoreTimeScale = false;

			public Tween Reset(float start, float end, float speed) {
				name = "";
				factor = 0f;
				this.start = start;
				this.end = end;
				this.speed = speed;
				easeFunc = Tweens.Ease.Linear;
				updateFunc = null;
				completeFunc = null;
				id = Random.Range(int.MinValue, int.MaxValue);
				ignoreTimeScale = false;
				return this;
			}

			public Tween(float start, float end, float speed) {
				this.start = start;
				this.end = end;
				this.speed = speed;
				easeFunc = Tweens.Ease.Linear;
				id = Random.Range(int.MinValue, int.MaxValue);
			}

			public Tween Ease(System.Func<float, float> func) {
				easeFunc = func;
				return this;
			}

			public Tween OnUpdate(System.Action<float> func) {
				updateFunc = func;
				return this;
			}

			public Tween OnComplete(System.Action func) {
				completeFunc = func;
				return this;
			}

			public Tween Name(string name) {
				this.name = name;
				return this;
			}

			public Tween Delay(float delay) {
				this.delay = delay;
				return this;
			}

			public Tween ID(int id) {
				this.id = id;
				return this;
			}

			public Tween IgnoreTimeScale(bool ignoreTimeScale) {
				this.ignoreTimeScale = ignoreTimeScale;
				return this;
			}
		}

		//

		static Stack<Tween> poolTweens = new Stack<Tween>();
		static Tween PoolPopTween(float start, float end, float speed) {
			var t = (poolTweens.Count == 0) ? new Tween(start, end, speed) : poolTweens.Pop().Reset(start, end, speed);
			curTweens.Add(t);
			return t;
		}
		static void PoolPushTween(Tween t) {
			poolTweens.Push(t);
			curTweens.Remove(t);
		}
		static List<Tween> curTweens = new List<Tween>();
		static Tweens inst = null;

		//

		void Update() {
			var count = curTweens.Count;
			for (int i = count - 1; i >= 0; --i) {
				var t = curTweens[i];
				var dt = t.ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
				if (t.delay > 0f) { t.delay -= dt; continue; }
				if (t.factor >= 1f) { // done
					if (t.completeFunc != null) { t.completeFunc(); }
					PoolPushTween(t);
					continue;
				}
				t.factor += dt * t.speed;
				if (t.factor >= 1f) { t.factor = 1f; } // done
				var value = Mathf.Lerp(t.start, t.end, t.easeFunc(t.factor));
				if (t.updateFunc != null) { t.updateFunc(value); }
			}
		}

		//

		public static Tween Timer(float seconds, System.Action completeFunc) {
			if (inst == null) { var go = new GameObject("<TWEENS>"); DontDestroyOnLoad(go); inst = go.AddComponent<Tweens>(); }
			var tween = PoolPopTween(0f, 1f, 1f / seconds);
			tween.completeFunc = completeFunc;
			return tween;
		}

		public static Tween Do(float seconds, System.Action<float> updateFunc = null) {
			if (inst == null) { var go = new GameObject("<TWEENS>"); DontDestroyOnLoad(go); inst = go.AddComponent<Tweens>(); }
			var tween = PoolPopTween(0f, 1f, 1f / seconds);
			tween.updateFunc = updateFunc;
			return tween;
		}

		public static Tween Do(float start, float end, float seconds, System.Action<float> updateFunc = null) {
			if (inst == null) { var go = new GameObject("<TWEENS>"); DontDestroyOnLoad(go); inst = go.AddComponent<Tweens>(); }
			var tween = PoolPopTween(start, end, 1f / seconds);
			tween.updateFunc = updateFunc;
			return tween;
		}

		public static bool Stop(string name, bool withComplete = false) {
			if (inst == null) { return false; }
			var removed = false;
			for (int i = curTweens.Count - 1; i >= 0; --i) {
				var t = curTweens[i];
				if (t.name == name) {
					if (withComplete && t.completeFunc != null) { t.completeFunc(); }
					PoolPushTween(t);
					curTweens.RemoveAt(i);
					removed = true;
				}
			}
			return removed;
		}

		public static bool Stop(int id, bool withComplete = false) {
			if (inst == null) { return false; }
			var removed = false;
			for (int i = curTweens.Count - 1; i >= 0; --i) {
				var t = curTweens[i];
				if (t.id == id) {
					if (withComplete && t.completeFunc != null) { t.completeFunc(); }
					PoolPushTween(t);
					curTweens.RemoveAt(i);
					removed = true;
				}
			}
			return removed;
		}
	}

}