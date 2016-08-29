using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base.Helpers {
	
	public static class Gizmos {
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
	}
	
}