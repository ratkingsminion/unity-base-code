using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base {

	public class PhysicsExtras {
		public static void WakeUpEverythingAround(GameObject go, float padding, bool disableMyColliders, int layerMaskPlayer) {
			var colliders = go.GetComponentsInChildren<Collider>(false);
			if (colliders.Length > 0) {
				var bounds = colliders[0].bounds;
				for (int i = colliders.Length - 1; i >= 1; --i) {
					var c = colliders[i];
					bounds.Encapsulate(c.bounds);
				}
				bounds.size += Vector3.one * padding * 2f;
				foreach (var c in Physics.OverlapBox(bounds.center, bounds.extents, Quaternion.identity, ~layerMaskPlayer, QueryTriggerInteraction.Collide)) {
					var r = c.attachedRigidbody;
					if (r != null) { r.WakeUp(); }
				}
				if (disableMyColliders) {
					foreach (var c in go.GetComponentsInChildren<Collider>(false)) { c.enabled = false; }
				}
			}
		}
		
		public static void WakeUpEverythingAround(Vector3 pos, Vector3 size, GameObject go, int layerMaskPlayer) {
			foreach (var c in Physics.OverlapBox(pos, size * 0.5f, Quaternion.identity, ~layerMaskPlayer, QueryTriggerInteraction.Collide)) {
				//Base.QuickGizmos.DrawBox(pos, Quaternion.identity, size * 0.5f, Color.yellow, 1f);
				var r = c.attachedRigidbody;
				if (r != null) { r.WakeUp(); }
			}
		}
	}

}