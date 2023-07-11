using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base {

	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	public class Creature : MonoBehaviour {
		[Header("Movement")]
		[SerializeField] float speedWalk = 3.5f;
		[SerializeField] float speedWalkSideways = 2.5f;
		[SerializeField] float speedWalkBackwards = 1.5f;
		[SerializeField] float jumpForce = 300f;
		[SerializeField] float moveInAirForceFactor = 1.0f;
		[SerializeField] float slopeMax = 60f;
		[SerializeField] Transform rotateTransform = null;
		[Header("RayCasts")]
		[SerializeField] int numRays = 64;
		[SerializeField] float legHeight = 0.08f;
		[SerializeField] float floorDepth = 0.08f;
		[SerializeField] float innerCircleFactor = 1f;
		[SerializeField] string floorLayerName = "Default";
		[SerializeField] ParticleSystem floorDust;
		[Header("Sounds")]
		[SerializeField] GameObject soundJump;
		[SerializeField] GameObject soundWalk;
		[SerializeField] float walkTime = 0.6f;
		
		public Transform RotateTransform { get { return rotateTransform; } }
		
		public float FactorX { get; set; }
		public float FactorZ { get; set; }
		[System.NonSerialized] public float globalFactor = 1f;
		
		public bool OnFloor { get; private set; }
		public Rigidbody Rbody { get; private set; }
		public CapsuleCollider Capsule { get; private set; }
		
		float jumping = 0f;
		bool mayJump;
		float walkTimer = 0f;
		
		static int floorLayerMask = 1;

#if UNITY_EDITOR
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
#endif

		//

		void Awake() {
			floorLayerMask = 1 << LayerMask.NameToLayer(floorLayerName);
			Capsule = GetComponentInChildren<CapsuleCollider>();
			Rbody = GetComponent<Rigidbody>();
		}

		public void Jump(float factor = 1f) {
			if (!OnFloor || mayJump || factor <= 0.1f) {
				return;
			}
			if (Physics.SphereCast(new Ray(transform.position + Vector3.up * (Capsule.center.y + Capsule.height * 0.5f - Capsule.radius * 2f), Vector3.up), Capsule.radius, Capsule.radius + 0.35f)) {
				return;
			}
			jumping = 0.08f; // time to jump, TODO should be calculated
			Rbody.AddForce(transform.up * jumpForce * factor, ForceMode.Acceleration);
			if (soundJump != null) {
				Instantiate(soundJump, transform.position, transform.rotation);
			}
		}

		void FixedUpdate() {
			// standing on floor?
			var h = Capsule.height * 0.5f; // - capsule.radius;
			var d = Capsule.radius + legHeight;
			var r = (Capsule.radius - 0.01f);
			var c = Rbody.position + Vector3.up * (Capsule.center.y - h + r);
			float dist = 1000f;
			r *= innerCircleFactor;
			var normal = Vector3.zero;
			//var smallestDist = 1000f;
			// check if the creature hits the floor by casting rays
			for (int i = 0; i <= numRays; ++i) {
				Vector3 p = c;
				if (i != numRays) {
					float f = 2f * Mathf.PI * i / numRays;
					p += new Vector3(Mathf.Sin(f) * r, 0f, Mathf.Cos(f) * r);
				}
				if (Physics.Raycast(p, Vector3.down, out var hit, d + floorDepth, floorLayerMask, QueryTriggerInteraction.Ignore)) {
					/*if (hit.distance < d)*/ { normal += hit.normal; }
					var a = Vector3.Angle(hit.normal, Vector3.up);
					//if (hit.distance < smallestDist) { smallestDist = hit.distance; }
					if (a < slopeMax && hit.distance < dist) { dist = hit.distance; }
				}
			}
#if UNITY_EDITOR
			Debug.DrawRay(c, normal.normalized * 2f, Color.white);
#endif
			float diff = dist - d;
			OnFloor = diff <= legHeight;
			mayJump = jumping > 0f;
			if (!OnFloor && normal.sqrMagnitude > 0f) {
				if (Vector3.Angle(normal, Vector3.up) < slopeMax) { // failsave when stuck between two very steep slopes
					//dist = smallestDist;
					diff = legHeight * 0.5f; // legHeight; // dist - d;
					OnFloor = true;
					//mayJump = false;
				}
			}
			if (mayJump) {
				// in order to prevent sliding on slopes (upwards), give jumping a bit of time
				jumping -= Time.deltaTime;
				OnFloor = false;
			}

			if (floorDust != null) {
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
				var pe = floorDust.emission;
				pe.enabled = OnFloor && (new Vector2(Rbody.velocity.x, Rbody.velocity.z).sqrMagnitude > 10f);
#else
				floorDust.enableEmission = onFloor && (new Vector2(rb.velocity.x, rb.velocity.z).sqrMagnitude > 10f);
#endif
			}
			
			if (OnFloor) {
				var input = new Vector2(FactorX, FactorZ);
				var inputDir = input.normalized;
				var inputLength = Mathf.Min(input.magnitude, 1f);
				var inputAngle = Vector2.Angle(Vector2.up, inputDir);
				var speed = inputAngle > 90f ? Mathf.Lerp(speedWalkSideways, speedWalkBackwards, (inputAngle - 90f) / 90f) : Mathf.Lerp(speedWalk, speedWalkSideways, inputAngle / 90f);
				inputAngle *= Mathf.Deg2Rad;
				var xSpeed = inputLength * Mathf.Sin(inputAngle) * speed * Mathf.Sign(FactorX);
				var zSpeed = inputLength * Mathf.Cos(inputAngle) * speed;

				var newVel = new Vector3(
					xSpeed * globalFactor,
					-diff * (diff > 0f ? 1f : 0.25f) / Time.fixedDeltaTime,
					zSpeed * globalFactor);

				Rbody.velocity = Quaternion.Euler(0f, rotateTransform.eulerAngles.y, 0f) * newVel;

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
					var speedDir = new Vector2(FactorX, FactorZ).normalized;
					var velocityChange = new Vector3(
						FactorX * Mathf.Abs(speedDir.x) * speedWalkSideways * globalFactor * moveInAirForceFactor,
						0f,
						FactorZ * Mathf.Abs(speedDir.y) * (FactorZ > 0f ? speedWalk : speedWalkBackwards) * globalFactor * moveInAirForceFactor);
					Rbody.AddForce(Quaternion.Euler(0f, rotateTransform.eulerAngles.y, 0f) * velocityChange, ForceMode.Force);
				}
				Rbody.velocity *= 0.98f;
			}

		}

		//

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