using UnityEngine;
using System.Collections;
using RatKing.Base;

namespace RatKing.Base {

	public class Swoosh : MonoBehaviour {
		public enum Normal {
			Forward,
			Right,
			Up,
			Follow2D,
		}
		public Transform follow;
		public Normal normal = Normal.Forward;
		public bool normalInverted;
		public float startWidth = 1f;
		public float endWidth = 0f;
		public Color startColor = Color.white;
		public Color endColor = Color.black;
		public float secondsPerLine = 0.1f;
		public int maxLines = 10;
		public Material material;
		public bool autoDestroyOnStop;
		public bool autoDestroyOnFollowNull;
		public bool hasShadow;
		public bool distanceBasedGradient;
		//
		float counter;
		Mesh mesh;
		Vector3[] pos;
		Vector3[] norm;
		Vector3[] vertices;
		Vector2[] uv;
		Color[] colors;

		Vector3 savePos;
		Vector3 saveNorm;
		float autoDestroyCounter;
		float curLengthPart, curLength;

		delegate Vector3 GetFollowNormal();
		GetFollowNormal GetNormal;
		Vector3 GetNormalForward() { return normalInverted ? -transform.forward : transform.forward; }
		Vector3 GetNormalRight() { return normalInverted ? -transform.right : transform.right; }
		Vector3 GetNormalUp() { return normalInverted ? -transform.up : transform.up; }
		Vector3 GetNormalFollow2D() {
			var d = (savePos - transform.position);
			if (d.sqrMagnitude > 0.000001f) {
				lastNorm2D = d.normalized;
				// lastNorm2D = Vector3.RotateTowards(lastNorm2D, d, 15f * Mathf.Deg2Rad, 0f); // todo parametrize?
				// lastNorm2D = Vector3.RotateTowards(lastNorm2D, d.normalized, 45f * Mathf.Deg2Rad, 1f); // todo parametrize?
			}
			return normalInverted ? new Vector3(lastNorm2D.y, -lastNorm2D.x, -lastNorm2D.z) : new Vector3(-lastNorm2D.y, lastNorm2D.x, lastNorm2D.z);
		}
		Vector3 lastNorm2D = Vector3.up;

		//
		float saveAlphaStart;
		float saveAlphaEnd;

		void OnValidate() {
			if (maxLines < 1) maxLines = 1;
			if (secondsPerLine < 0.0f) secondsPerLine = 0.0f;
			switch (normal) {
				case Normal.Forward: GetNormal = GetNormalForward; break;
				case Normal.Right: GetNormal = GetNormalRight; break;
				case Normal.Up: GetNormal = GetNormalUp; break;
				case Normal.Follow2D: GetNormal = GetNormalFollow2D; break;
			}
		}

		//

		void Awake() {
			switch (normal) {
				case Normal.Forward: GetNormal = GetNormalForward; break;
				case Normal.Right: GetNormal = GetNormalRight; break;
				case Normal.Up: GetNormal = GetNormalUp; break;
				case Normal.Follow2D: GetNormal = GetNormalFollow2D; break;
			}

			savePos = transform.position;
			saveNorm = GetNormal();
			autoDestroyCounter = Time.time + (maxLines * secondsPerLine) + 0.1f;
			CreateMesh();
		}

		void OnEnable() {
			ResetPosition();
		}

		void CreateMesh() {
			saveAlphaStart = startColor.a;
			saveAlphaEnd = endColor.a;

			var curPos = transform.position;
			var curNormal = GetNormal();

			pos = new Vector3[maxLines + 1];
			norm = new Vector3[maxLines + 1];

			mesh = new Mesh();
			mesh.MarkDynamic();
			var mr = gameObject.AddComponent<MeshRenderer>();
			mr.shadowCastingMode = hasShadow ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.Off;
			mr.receiveShadows = hasShadow;
			mr.sortingLayerID = SortingLayer.NameToID("Actors");
			mr.sortingOrder = -10;
			var mf = gameObject.AddComponent<MeshFilter>();
			vertices = new Vector3[maxLines * 2 + 2];
			colors = new Color[maxLines * 2 + 2];
			uv = new Vector2[maxLines * 2 + 2];
			var triangles = new int[maxLines * 6];

			int i = 0, i2 = 0, i6 = 0;
			while (i <= maxLines) {
				pos[i] = vertices[i2 + 0] = vertices[i2 + 1] = curPos; // Vector3.zero;
				norm[i] = curNormal;
				colors[i2 + 0] = colors[i2 + 1] = startColor;
				uv[i2 + 0] = Vector2.right;
				uv[i2 + 1] = Vector2.one;
				if (i < maxLines) {
					triangles[i6 + 0] = i2 + 0;
					triangles[i6 + 1] = i2 + 1;
					triangles[i6 + 2] = i2 + 3;
					triangles[i6 + 3] = i2 + 3;
					triangles[i6 + 4] = i2 + 2;
					triangles[i6 + 5] = i2 + 0;
				}
				++i; i2 += 2; i6 += 6;
			}
			uv[0] = Vector2.zero;
			uv[1] = Vector2.up;
			colors[0] = colors[1] = endColor;

			mesh.vertices = vertices;
			mesh.colors = colors;
			mesh.uv = uv;
			mesh.triangles = triangles;
			mf.mesh = mesh;
			mr.material = material;
		}

		//

		void Update() {
			if (autoDestroyOnFollowNull && follow == null) {
				autoDestroyCounter = Time.time + (maxLines * secondsPerLine) + 0.1f;
				if (autoDestroyCounter < Time.time) {
					Destroy(gameObject); // TODO
					return;
				}
			}
			if (autoDestroyOnStop) {
				if ((!Helpers.Math.Approx(savePos, transform.position) || !Helpers.Math.Approx(saveNorm, GetNormal()))) {
					autoDestroyCounter = Time.time + (maxLines * secondsPerLine) + 0.1f;
				}
				if (autoDestroyCounter < Time.time) {
					Destroy(gameObject); // TODO
					return;
				}
			}

			if (follow != null) {
				transform.position = follow.position;
			}
			transform.rotation = Quaternion.identity;
			var curPos = transform.position;

			float delta = 1f - (counter - Time.time) / secondsPerLine;
			float inverseMax = 1f / (float)(maxLines - 1);

			// current pos (== last line)
			norm[maxLines] = GetNormal(); // TODO which norm?
			Vector3 w = norm[maxLines - 1] * startWidth * 0.5f;
			pos[maxLines] = curPos; // Vector3.zero;
			vertices[maxLines * 2 + 0] = -w; // pos[maxLines] - w - curPos;
			vertices[maxLines * 2 + 1] = w; // pos[maxLines] + w - curPos;

			if (distanceBasedGradient) {
				curLength = curLengthPart + Vector3.Distance(pos[maxLines - 1], pos[maxLines]);
			}

			// last pos (== first line)
			Vector3 lNorm = Vector3.Slerp(norm[0], norm[1], delta);
			w = lNorm * endWidth * 0.5f;
			Vector3 lpos = Vector3.Lerp(pos[0], pos[1], delta);
			vertices[0] = lpos - w - curPos;
			vertices[1] = lpos + w - curPos;

			// update all the lines sans the first and the last one
			if (distanceBasedGradient) {
				var g = 0f;
				for (int i = 1, i2 = 2; i < maxLines; ++i, i2 += 2) {
					float f = Mathf.Clamp01((i - delta) * inverseMax);
					w = norm[i] * (f * startWidth + (1f - f) * endWidth) * 0.5f;
					vertices[i2 + 0] = pos[i] - w - curPos;
					vertices[i2 + 1] = pos[i] + w - curPos;

					g += Vector3.Distance(pos[i - 1], pos[i]) / curLength;
					colors[i2 + 0] = colors[i2 + 1] = Color.Lerp(endColor, startColor, g);
					uv[i2 + 0].x = uv[i2 + 1].x = g;
				}
			}
			else {
				for (int i = 1, i2 = 2; i < maxLines; ++i, i2 += 2) {
					float f = Mathf.Clamp01((i - delta) * inverseMax);
					w = norm[i] * (f * startWidth + (1f - f) * endWidth) * 0.5f;
					vertices[i2 + 0] = pos[i] - w - curPos;
					vertices[i2 + 1] = pos[i] + w - curPos;

					colors[i2 + 0] = colors[i2 + 1] = Color.Lerp(endColor, startColor, f);
					uv[i2 + 0].x = uv[i2 + 1].x = f;
				}
			}

			mesh.vertices = vertices;
			mesh.colors = colors;
			mesh.uv = uv;

			if (counter < Time.time) {
				counter = Time.time + secondsPerLine;
				for (int i = 1; i <= maxLines; ++i) {
					pos[i - 1] = pos[i];
					norm[i - 1] = norm[i];
				}

				if (distanceBasedGradient) {
					curLengthPart = 0f;
					for (int i = 1; i < maxLines; ++i) {
						curLengthPart += Vector3.Distance(pos[i - 1], pos[i]);
					}
				}

				mesh.RecalculateBounds();
			}
			mesh.RecalculateNormals();

			saveNorm = GetNormal();
			savePos = curPos;
		}

		public void SetAlpha(float alpha) {
			startColor.a = alpha * saveAlphaStart;
			endColor.a = alpha * saveAlphaEnd;
			colors[maxLines * 2 + 0] = colors[maxLines * 2 + 1] = startColor;
			colors[0] = colors[1] = endColor;
		}

		public void ResetPosition() {
			var curPos = follow != null ? follow.position : transform.position;
			savePos = curPos;
			if (vertices != null) {
				for (int i = 0; i <= maxLines; ++i) {
					pos[i] = vertices[i * 2 + 0] = vertices[i * 2 + 1] = curPos;
				}
				mesh.vertices = vertices;
			}
		}
	}

}