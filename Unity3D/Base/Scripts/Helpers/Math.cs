using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base.Helpers {
	
	public static class Math {
		// vector3 stuff:
		public static Vector3 Rotate(Vector3 vector, Vector3 axis, float angle) {
			return Quaternion.AngleAxis(angle, axis) * vector;
		}
		public static Vector3 Rotate(Vector3 vector, float x, float y, float z) {
			return Quaternion.Euler(x, y, z) * vector;
		}
		public static Vector3 RotateAround(Vector3 vector, Vector3 origin, Vector3 axis, float angle) {
			return origin + Quaternion.AngleAxis(angle, axis) * (vector - origin);
		}
		public static Vector3 Rotate(Vector3 vector, Vector3 origin, float x, float y, float z) {
			return origin + Quaternion.Euler(x, y, z) * (vector - origin);
		}
		// from http://forum.unity3d.com/threads/33215-Vector-rotation
		public static float GetPitch(Vector3 v) {
			float len = Mathf.Sqrt((v.x * v.x) + (v.z * v.z));
			return -Mathf.Atan2(v.y, len);
		}
		public static float GetYaw(Vector3 v) {
			return Mathf.Atan2(v.x, v.z);
		}
		public static Vector3 QuantizeRound(Vector3 v, float e = 1f) {
			float ie = 1f / e;
			v.x = Mathf.Round(v.x * ie) * e;
			v.y = Mathf.Round(v.y * ie) * e;
			v.z = Mathf.Round(v.z * ie) * e;
			return v.normalized;
		}
		public static Vector3 QuantizeFloor(Vector3 v, float e = 1f) {
			float ie = 1f / e;
			v.x = Mathf.Floor(v.x * ie) * e;
			v.y = Mathf.Floor(v.y * ie) * e;
			v.z = Mathf.Floor(v.z * ie) * e;
			return v;
		}
		// quaternion stuff:
		public static float Length(this Quaternion q) {
			return Mathf.Sqrt(q.w * q.w + q.x * q.x + q.y * q.y + q.z * q.z);
		}
		public static void Normalize(ref Quaternion q) {
			float l = 1f / Mathf.Sqrt(q.w * q.w + q.x * q.x + q.y * q.y + q.z * q.z);
			q.w *= l; q.x *= l; q.y *= l; q.z *= l;
		}
		// from http://www.euclideanspace.com/maths/algebra/realNormedAlgebra/quaternions/slerp/
		// has auto normalization
		public static Quaternion Slerp(Quaternion qa, Quaternion qb, float t) {
			// quaternion to return
			// Calculate angle between them.
			float cosHalfTheta = qa.w * qb.w + qa.x * qb.x + qa.y * qb.y + qa.z * qb.z;
			//Debug.Log (	Mathf.Sqrt(qa.w * qa.w + qa.x * qa.x + qa.y * qa.y + qa.z * qa.z) + " - " + 
			//			Mathf.Sqrt(qb.w * qb.w + qb.x * qb.x + qb.y * qb.y + qb.z * qb.z));
			// if qa=qb or qa=-qb then theta = 0 and we can return qa
			if (Mathf.Abs(cosHalfTheta) >= 1f) {
				//qm.w = qa.w;qm.x = qa.x;qm.y = qa.y;qm.z = qa.z;
				return qa;
			}
			// Calculate temporary values.
			float halfTheta = Mathf.Acos(cosHalfTheta);
			float sinHalfTheta = Mathf.Sqrt(1f - cosHalfTheta * cosHalfTheta);
			Quaternion qm = new Quaternion();
			// if theta = 180 degrees then result is not fully defined
			// we could rotate around any axis normal to qa or qb
			if (Mathf.Abs(sinHalfTheta) < 0.001f) {
				qm.w = (qa.w * 0.5f + qb.w * 0.5f);
				qm.x = (qa.x * 0.5f + qb.x * 0.5f);
				qm.y = (qa.y * 0.5f + qb.y * 0.5f);
				qm.z = (qa.z * 0.5f + qb.z * 0.5f);
			}
			else {
				float ratioA = Mathf.Sin((1f - t) * halfTheta) / sinHalfTheta;
				float ratioB = Mathf.Sin(t * halfTheta) / sinHalfTheta;
				//calculate Quaternion.
				qm.w = (qa.w * ratioA + qb.w * ratioB);
				qm.x = (qa.x * ratioA + qb.x * ratioB);
				qm.y = (qa.y * ratioA + qb.y * ratioB);
				qm.z = (qa.z * ratioA + qb.z * ratioB);
			}
			float il = 1f / Mathf.Sqrt(qm.w * qm.w + qm.x * qm.x + qm.y * qm.y + qm.z * qm.z);
			qm.w *= il; qm.x *= il; qm.y *= il; qm.z *= il;
			return qm;
		}
		// transitions:
		public static float Hermite(float start, float end, float value) {
			return Mathf.Lerp(start, end, value * value * (3f - 2f * value));
		}
		public static float Sinerp(float start, float end, float value) {
			return Mathf.Lerp(start, end, Mathf.Sin(value * Mathf.PI * 0.5f));
		}
		public static float Coserp(float start, float end, float value) {
			return Mathf.Lerp(start, end, 1f - Mathf.Cos(value * Mathf.PI * 0.5f));
		}
		public static float Berp(float start, float end, float value) {
			value = Mathf.Clamp01(value);
			value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
			return start + (end - start) * value;
		}
		public static float SmoothStep(float x, float min, float max) {
			x = Mathf.Clamp(x, min, max);
			float v1 = (x - min) / (max - min);
			float v2 = (x - min) / (max - min);
			return -2 * v1 * v1 * v1 + 3 * v2 * v2;
		}
		public static float Lerp(float start, float end, float value) {
			return ((1f - value) * start) + (value * end);
		}
		public static float NearestPointDistance(Ray ray, Vector3 point) {
			return Vector3.Dot((point - ray.origin), ray.direction) / Vector3.Dot(ray.direction, ray.direction);
		}
		public static float NearestPointDistance(Vector3 lineStart, Vector3 lineDirection, Vector3 point) {
			return Vector3.Dot((point - lineStart), lineDirection) / Vector3.Dot(lineDirection, lineDirection);
		}
		public static Vector3 NearestPoint(Ray ray, Vector3 point) {
			float closestPoint = Vector3.Dot((point - ray.origin), ray.direction);
			return ray.origin + (closestPoint * ray.direction);
		}
		public static Vector3 NearestPoint(Vector3 lineStart, Vector3 lineEnd, Vector3 point) {
			Vector3 lineDirection = Vector3.Normalize(lineEnd - lineStart);
			float closestPoint = Vector3.Dot((point - lineStart), lineDirection); // Vector3.Dot(lineDirection,lineDirection);
			return lineStart + (closestPoint * lineDirection);
		}
		public static Vector3 NearestPointStrict(Vector3 lineStart, Vector3 lineEnd, Vector3 point) {
			Vector3 fullDirection = lineEnd - lineStart;
			Vector3 lineDirection = Vector3.Normalize(fullDirection);
			float closestPoint = Vector3.Dot((point - lineStart), lineDirection) / lineDirection.sqrMagnitude; //Vector3.Dot(lineDirection,lineDirection);
			return lineStart + (Mathf.Clamp(closestPoint, 0f, Vector3.Magnitude(fullDirection)) * lineDirection);
		}
		public static float Bounce(float x) {
			return Mathf.Abs(Mathf.Sin(6.28f * (x + 1f) * (x + 1f)) * (1f - x));
		}
		/*
		* CLerp - Circular Lerp - is like lerp but handles the wraparound from 0 to 360.
		* This is useful when interpolating eulerAngles and the object
		* crosses the 0/360 boundary.  The standard Lerp function causes the object
		* to rotate in the wrong direction and looks stupid. Clerp fixes that.
		*/
		public static float Clerp(float start, float end, float value) {
			float min = 0f;
			float max = 360f;
			float half = Mathf.Abs((max - min) / 2f);//half the distance between min and max
			float diff = 0f;

			if ((end - start) < -half) {
				diff = ((max - start) + end) * value;
				return start + diff;
			}
			else if ((end - start) > half) {
				diff = -((max - end) + start) * value;
				return start + diff;
			}
			return start + (end - start) * value;
		}
		// from http://pastebin.com/NZrstYL4
		public static Vector3 Qarp(Vector3 a, Vector3 b, Vector3 c, float blend) {
			Vector3 a_b = Vector3.Lerp(a, b, blend);
			return a_b + (Vector3.Lerp(b, c, blend) - a_b) * blend;
		}
		// from http://pastebin.com/NZrstYL4
		public static Vector3 SuperLerp(Vector3 p0, Vector3 p1, Vector3 v0, Vector3 v1, float l) {
			return Qarp(p0, (p0 + v0 * 0.5f + p1 - v1 * 0.5f) / 2f, p1, l);
		}
		//
		public static bool Approx(this float f1, float f2, float epsilon = 0.01f) {
			return f1 < f2 + epsilon && f1 > f2 - epsilon;
		}
		public static bool Approx(this Vector3 v1, Vector3 v2, float epsilon = 0.01f) {
			return v1.x < v2.x + epsilon && v1.x > v2.x - epsilon &&
					v1.y < v2.y + epsilon && v1.y > v2.y - epsilon &&
					v1.z < v2.z + epsilon && v1.z > v2.z - epsilon;
		}
	}

}