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
				UnityEngine.Gizmos.DrawLine(trans.position + v, trans.position + t);
				v = t;
			}
		}
		static Vector3 otherOne = new Vector3(0.5f, -2.0f, 3.5f);
		public static void DrawWireCircle(Vector3 center, Vector3 axis, float radius, int prec = 100) {
			float fac = 360f / (float)prec;
			Vector3 v = Vector3.Cross(axis, (Vector3.Dot(axis, Vector3.one) == 0.0f ? otherOne : Vector3.one)).normalized * radius;
			for (int i = 0; i < prec; ++i) {
				Vector3 t = Math.Rotate(v, axis, fac);
				UnityEngine.Gizmos.DrawLine(center + v, center + t);
				v = t;
			}
		}
		
		public static void DrawWireCapsule(Vector3 center, Vector3 axis, float radius, float height, int prec = 10) {
			var innerHeight = height - radius * 2f;

			float fac = 360f / (float)prec;
			Vector3 v = Vector3.Cross(axis, (Vector3.Dot(axis, Vector3.one) == 0.0f ? otherOne : Vector3.one)).normalized * radius;
			var top = center + axis * (innerHeight * 0.5f);
			var bottom = center - axis * (innerHeight * 0.5f);
			for (int i = 0; i < prec; ++i) {
				Vector3 t = Math.Rotate(v, axis, fac);
				UnityEngine.Gizmos.DrawLine(bottom + v, bottom + t);
				UnityEngine.Gizmos.DrawLine(top + v, top + t);
				UnityEngine.Gizmos.DrawLine(top + v, bottom + v);
				DrawWireCircle(top, v, radius, prec);
				DrawWireCircle(bottom, v, radius, prec);
				v = t;
			}
		}
	}
	
}