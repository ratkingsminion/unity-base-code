#if UNITY_2_6 || UNITY_2_6_1 || UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
#define UNITY_OLD
#else
#define UNITY_5
#endif

#if UNITY_5 && !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2
#define PARTICLE_SYSTEM_UPDATE_5_3
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base {

	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	public class Creature : MonoBehaviour {
		[Header("Movement")]
		public float speedWalk = 3.5f;
		public float speedWalkSideways = 2.5f;
		public float speedWalkBackwards = 1.5f;
		public float jumpForce = 300f;
		public float moveInAirForceFactor = 1.0f;
		public float slopeMax = 60f;
		[Header("RayCasts")]
		public int numRays = 12;
		public float legHeight = 0.08f;
		public float floorDepth = 0.08f;
		public float innerCircleFactor = 1f;
		[SerializeField]
		string floorLayerName = "Default";
		public ParticleSystem floorDust;
		[Header("Sounds")]
		public GameObject soundJump;
		public GameObject soundWalk;
		public float walkTime = 0.6f;
		//
		public float xFactor { get; set; }
		public float zFactor { get; set; }
		[System.NonSerialized]
		public float globalFactor = 1f;
		//
		public bool onFloor { get; private set; }
		public Rigidbody rb { get; private set; }
		public CapsuleCollider capsule { get; private set; }
		//
		float jumping = 0f;
		float walkTimer = 0f;
		//
		static int floorLayerMask = 1;

		//

		void OnValidate() {
			if (speedWalk < 0f) speedWalk = 0f;
			if (speedWalkSideways < 0f) speedWalkSideways = 0f;
			if (speedWalkBackwards < 0f) speedWalkBackwards = 0f;
			if (jumpForce < 0f) jumpForce = 0f;
			if (slopeMax < 0f) slopeMax = 0f;
			if (moveInAirForceFactor < 0f) moveInAirForceFactor = 0f;
			if (numRays < 0) numRays = 0;
			if (legHeight < 0f) legHeight = 0f;
			if (floorDepth < 0f) floorDepth = 0f;
			if (innerCircleFactor < 0f) innerCircleFactor = 0f;
			if (walkTime < 0f) walkTime = 0f;
		}

		//

		void Awake() {
			floorLayerMask = 1 << LayerMask.NameToLayer(floorLayerName);
			capsule = GetComponentInChildren<CapsuleCollider>();
			rb = GetComponent<Rigidbody>();
		}

		public void Jump(float factor = 1f) {
			if (!onFloor || jumping > 0f || factor <= 0.1f) {
				return;
			}
			if (Physics.SphereCast(new Ray(transform.position + Vector3.up * (capsule.center.y + capsule.height * 0.5f - capsule.radius * 2f), Vector3.up), capsule.radius, capsule.radius + 0.35f)) {
				return;
			}
			jumping = 0.08f; // time to jump, TODO should be calculated
			rb.AddForce(transform.up * jumpForce * factor, ForceMode.Acceleration);
			if (soundJump != null) {
				Instantiate(soundJump, transform.position, transform.rotation);
			}
		}

		void FixedUpdate() {
			// standing on floor?
			var pos = transform.position;
			var h = capsule.height * 0.5f; // - capsule.radius;
			var d = capsule.radius + legHeight;
			var r = (capsule.radius - 0.01f);
			var c = pos + Vector3.up * (capsule.center.y - h + r);
			var dist = 1000f;
			RaycastHit hit;
			onFloor = false;
			for (int i = 0; i <= numRays; ++i) {
				float f = 2f * Mathf.PI * (float)i / (float)numRays;
				r *= innerCircleFactor;
				Vector3 p = c + ((i == numRays) ? Vector3.zero : new Vector3(Mathf.Sin(f) * r, 0f, Mathf.Cos(f) * r));
				float d2 = d + floorDepth;
				if (!Physics.Raycast(p, Vector3.down, out hit, d2, floorLayerMask)) {
					continue;
				}
				if (Vector3.Angle(hit.normal, Vector3.up) < slopeMax) {
					onFloor = true;
					if (hit.distance < dist) dist = hit.distance;
				}
			}

			float diff = dist - d;
			if (onFloor && diff > legHeight) { // 0f)
				onFloor = false;
			}

			//if (jumping && onFloor) onFloor = false;
			//else if (jumping && !onFloor) jumping = false;
			if (jumping > 0f) {
				jumping -= Time.deltaTime;
				onFloor = false;
			}

			if (floorDust != null) {
#if PARTICLE_SYSTEM_UPDATE_5_3
				var pe = floorDust.emission;
				pe.enabled = onFloor && (new Vector2(rb.velocity.x, rb.velocity.z).sqrMagnitude > 10f);
#else
				floorDust.enableEmission = onFloor && (new Vector2(rb.velocity.x, rb.velocity.z).sqrMagnitude > 10f);
#endif
			}
			if (onFloor) {
				pos.y -= diff * (diff > 0f ? 1f : 0.25f);
				transform.position = pos;

				rb.velocity = Vector3.zero;
				Vector2 input = new Vector2(xFactor, zFactor);
				Vector2 inputDir = input.normalized;
				float inputLength = Mathf.Min(input.magnitude, 1f);
				float inputAngle = Vector2.Angle(Vector2.up, inputDir);
				float speed = inputAngle > 90f ? Mathf.Lerp(speedWalkSideways, speedWalkBackwards, (inputAngle - 90f) / 90f) : Mathf.Lerp(speedWalk, speedWalkSideways, inputAngle / 90f);
				inputAngle *= Mathf.Deg2Rad;
				float xSpeed = inputLength * Mathf.Sin(inputAngle) * speed * Mathf.Sign(xFactor);
				float zSpeed = inputLength * Mathf.Cos(inputAngle) * speed;

				Vector3 velocityChange = new Vector3(
					xSpeed * globalFactor,
					0f,
					zSpeed * globalFactor);

				//rb.AddForce(transform.TransformDirection(velocityChange), ForceMode.VelocityChange);
				rb.velocity = transform.TransformDirection(velocityChange);

				if (xSpeed != 0f || zSpeed != 0f) {
					if (walkTimer < Time.time) {
						if (soundWalk != null)
							Instantiate(soundWalk, transform.position, transform.rotation);
						walkTimer = Time.time + walkTime;
					}
				}
			}
			else {
				if (moveInAirForceFactor > 0f) {
					//rb.velocity = Vector3.zero;
					Vector2 speedDir = new Vector2(xFactor, zFactor).normalized;
					Vector3 velocityChange = new Vector3(
						xFactor * Mathf.Abs(speedDir.x) * speedWalkSideways * globalFactor * moveInAirForceFactor,
						0f,
						zFactor * Mathf.Abs(speedDir.y) * (zFactor > 0f ? speedWalk : speedWalkBackwards) * globalFactor * moveInAirForceFactor);
					rb.AddForce(transform.TransformDirection(velocityChange), ForceMode.Force);
				}
				rb.velocity *= 0.98f;
			}

		}

#if UNITY_EDITOR
		void OnDrawGizmos() {
			if (GetComponentInChildren<CapsuleCollider>() != null) {
				var capsule = GetComponentInChildren<CapsuleCollider>();
				var h = capsule.height * 0.5f;
				var d = capsule.radius + legHeight;
				var r = (capsule.radius - 0.01f);
				Vector3 c = transform.position + Vector3.up * (capsule.center.y - h + r);
				for (int i = 0; i <= numRays; ++i) {
					float f = 2f * Mathf.PI * (float)i / (float)numRays;
					Vector3 p = c + ((i == numRays) ? Vector3.zero : new Vector3(Mathf.Sin(f) * r * innerCircleFactor, 0f, Mathf.Cos(f) * r * innerCircleFactor));
					Gizmos.color = Color.yellow;
					Gizmos.DrawRay(p, Vector3.down * d);
					Gizmos.color = Color.red;
					Gizmos.DrawRay(p + Vector3.down * d, Vector3.down * floorDepth);
				}
			}
		}
#endif
	}

}