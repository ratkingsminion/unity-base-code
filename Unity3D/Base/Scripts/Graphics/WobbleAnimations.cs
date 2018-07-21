using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RatKing.Base {

	public class WobbleAnimations : MonoBehaviour {
		public enum Type {
			Scale,
			Rotation,
			Both
		}

		//

		static WobbleAnimations Inst;
		static List<WobbleAnimation> animations = new List<WobbleAnimation>();

		//

		public static WobbleAnimation StartFor(Transform target, float seconds, float strength = 1f, Type type = Type.Scale, Vector3 rotAxis = default(Vector3)) {
			if (Inst == null) { Inst = new GameObject("<WobbleAnimations>").AddComponent<WobbleAnimations>(); }

			var wa = animations.Find(w => w.target == target);
			if (wa != null) {
				wa.seconds = seconds;
				return wa;
			}
			wa = new WobbleAnimation() {
				factor = 0f,
				target = target,
				startSeconds = seconds,
				seconds = seconds,
				strength = strength,
				type = type,
				originalScale = target.localScale,
				rotationAxis = rotAxis,
				originalRotation = target.localRotation,
				startTime = Random.value * Mathf.PI,
			};
			animations.Add(wa);
			return wa;
		}

		//

		void Update() {
			for (int i = animations.Count - 1; i >= 0; --i) {
				var a = animations[i];
				if (a.target == null) {
					animations.RemoveAt(i);
					continue;
				}

				a.seconds -= Time.deltaTime;

				//var targetFactor = 1f + Mathf.Log(a.seconds / a.startSeconds) * 0.1f;
				//var targetFactor = Mathf.Sqrt(Mathf.Sqrt(a.seconds / a.startSeconds));
				//var targetFactor = Mathf.Sqrt(a.seconds / a.startSeconds);
				var targetFactor = Mathf.Clamp01(a.seconds / a.startSeconds);
				a.factor = Mathf.MoveTowards(a.factor, targetFactor, Time.deltaTime * 10f);

				if (a.factor <= 0f && a.seconds <= 0f) {
					if (a.type == Type.Scale || a.type == Type.Both) { a.target.localScale = a.originalScale; }
					if (a.type == Type.Rotation || a.type == Type.Both) { a.target.localRotation = a.originalRotation; }
					animations.RemoveAt(i);
					continue;
				}

				if (a.type == Type.Scale || a.type == Type.Both) {
					a.target.localScale = Mathf.Lerp(
							1f,
							Base.Math.Remap(Mathf.Sin(a.startTime + Time.time * 20f), -1f, 1f, Mathf.LerpUnclamped(1f, 0.75f, a.strength), Mathf.LerpUnclamped(1f, 1.5f, a.strength)),
							a.factor) * a.originalScale;
				}
				if (a.type == Type.Rotation || a.type == Type.Both) {
					a.target.localRotation = a.originalRotation;
					a.target.Rotate(a.rotationAxis, a.strength * a.factor * Mathf.Sin(a.startTime + Time.time * 15f) * 15f, Space.Self);
				}
			}
		}
	}

}