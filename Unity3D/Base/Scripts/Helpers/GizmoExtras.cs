using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base {
	
	public static class GizmoExtras {
		public static void DrawWireCircle(this Transform trans, float radius, int prec = 100) {
			float fac = 360f / (float)prec;
			Vector3 v = trans.forward * radius;
			for (int i = 0; i < prec; ++i) {
				Vector3 t = Math.Rotate(v, trans.up, fac);
				Gizmos.DrawLine(trans.position + v, trans.position + t);
				v = t;
			}
		}
		static Vector3 otherOne = new Vector3(0.5f, -2.0f, 3.5f);
		public static void DrawWireCircle(Vector3 center, Vector3 axis, float radius, int prec = 100) {
			float fac = 360f / (float)prec;
			Vector3 v = Vector3.Cross(axis, (Vector3.Dot(axis, Vector3.one) == 0.0f ? otherOne : Vector3.one)).normalized * radius;
			for (int i = 0; i < prec; ++i) {
				Vector3 t = Math.Rotate(v, axis, fac);
				Gizmos.DrawLine(center + v, center + t);
				v = t;
			}
		}

		public static void DrawWireCapsule(Transform transform, CapsuleCollider capsule, int prec = 6) {
			var axis = capsule.direction == 0 ? transform.right : capsule.direction == 2 ? transform.forward : transform.up;
			DrawWireCapsule(transform.position + capsule.center, axis, capsule.radius, capsule.height, prec);
		}
		
		public static void DrawWireCapsule(Vector3 center, Vector3 axis, float radius, float height, int prec = 6) {
			var innerHeight = height - radius * 2f;
			var dprec = prec * 2;
			float fac = 360f / (float)dprec;
			Vector3 v = Vector3.Cross(axis, (Vector3.Dot(axis, Vector3.one) == 0.0f ? otherOne : Vector3.one)).normalized * radius;
			var top = center + axis * (innerHeight * 0.5f);
			var bottom = center - axis * (innerHeight * 0.5f);
			for (int i = 0; i < dprec; ++i) {
				Vector3 t = Math.Rotate(v, axis, fac);
				Gizmos.DrawLine(bottom + v, bottom + t);
				Gizmos.DrawLine(top + v, top + t);
				Gizmos.DrawLine(top + v, bottom + v);
				if (i <= prec) {
					// spheres
					Vector3 st = top + t, sb = bottom + t;
					Vector3 a = Vector3.Cross(t, axis);
					for (int j = 0; j < prec; ++j) {
						var stn = Math.RotateAround(st, top, a, fac);
						var sbn = Math.RotateAround(sb, bottom, -a, fac);
						Gizmos.DrawLine(st, stn);
						Gizmos.DrawLine(sb, sbn);
						st = stn;
						sb = sbn;
					}
				}
				v = t;
			}
		}
	}
	
}