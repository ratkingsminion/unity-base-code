using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RatKing {

	public class FloatyFloat {

		class FloatyFloats : MonoBehaviour {
			static FloatyFloats instance;
			static readonly List<FloatyFloat> floats = new List<FloatyFloat>();

			//
			
			[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
			static void OnRuntimeInitializeOnLoad() {
				instance = null;
				floats.Clear();
			}

			//

			static void Init() {
				instance = new GameObject("<FloatyFloats>").AddComponent<FloatyFloats>();
			}

			public static void Register(FloatyFloat floatyFloat) {
				if (instance == null) { Init(); }
				floats.Add(floatyFloat);
			}

			public static void Unregister(FloatyFloat floatyFloat) {
				floats.Remove(floatyFloat);
			}

			//

			void Update() {
				foreach (var f in floats) {
					if (f.curWait > Time.time) { continue; }
					f.CurValue = Mathf.SmoothDamp(f.CurValue, f.target, ref f.velocity, f.smoothTime);
				}
			}
		}

		//

		public float CurValue { get; private set; } = 0f;
		float target = 0f;
		public float Target {
			get { return target; }
			set { target = value; curWait = Time.time + delay; }
		}
		readonly float smoothTime = 0.5f;
		readonly float delay = 0f;
		float curWait = 0f;
		float velocity = 0f;


		public FloatyFloat() {
			FloatyFloats.Register(this);
		}
		public FloatyFloat(float value) {
			CurValue = target = value;
			FloatyFloats.Register(this);
		}
		public FloatyFloat(float value, float delay) {
			CurValue = target = value;
			this.delay = delay;
			FloatyFloats.Register(this);
		}
		public FloatyFloat(float value, float delay, float smoothTime) {
			CurValue = target = value;
			this.delay = delay;
			this.smoothTime = smoothTime;
			FloatyFloats.Register(this);
		}

		public void Destroy() {
			FloatyFloats.Unregister(this);
		}
	}

}
