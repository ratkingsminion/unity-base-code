using System;
using UnityEngine;

namespace RatKing.ImageEffects.Modified
{
	[ExecuteInEditMode]
	[RequireComponent (typeof(Camera))]
	[AddComponentMenu("Image Effects/A Rat King/Global Fog")]
	class GlobalFog : MonoBehaviour
	{
		[Tooltip("Color of the fog")]
		public Color color = Color.red;
		[Tooltip("Apply distance-based fog?")]
		public bool  distanceFog = true;
		[Tooltip("Distance fog is based on radial distance from camera when checked")]
		public bool  useRadialDistance = false;
		[Tooltip("Apply height-based fog?")]
		public bool  heightFog = true;
		[Tooltip("Fog top Y coordinate")]
		public float height = 1.0f;
		[Range(0.001f,10.0f)]
		public float heightDensity = 2.0f;
		[Tooltip("Push fog away from the camera by this amount")]
		public float startDistance = 0.0f;

		public FogMode sceneMode = FogMode.Linear;
		public float sceneDensity = 0.01f; // = RenderSettings.fogDensity;
		public float sceneStart = 5f; //= RenderSettings.fogStartDistance;
		public float sceneEnd = 10f; // = RenderSettings.fogEndDistance;

		public Shader fogShader = null;
		private Material fogMaterial = null;


		public bool CheckResources ()
		{
			CheckSupport (true);

			fogMaterial = CheckShaderAndCreateMaterial (fogShader, fogMaterial);

			if (!isSupported)
				ReportAutoDisable ();
			return isSupported;
		}

		[ImageEffectOpaque]
		void OnRenderImage (RenderTexture source, RenderTexture destination)
		{
			if (CheckResources()==false || (!distanceFog && !heightFog))
			{
				Graphics.Blit (source, destination);
				return;
			}

			Camera cam = GetComponent<Camera>();
			Transform camtr = cam.transform;
			float camNear = cam.nearClipPlane;
			float camFar = cam.farClipPlane;
			float camFov = cam.fieldOfView;
			float camAspect = cam.aspect;

			Matrix4x4 frustumCorners = Matrix4x4.identity;

			float fovWHalf = camFov * 0.5f;

			Vector3 toRight = camtr.right * camNear * Mathf.Tan (fovWHalf * Mathf.Deg2Rad) * camAspect;
			Vector3 toTop = camtr.up * camNear * Mathf.Tan (fovWHalf * Mathf.Deg2Rad);

			Vector3 topLeft = (camtr.forward * camNear - toRight + toTop);
			float camScale = topLeft.magnitude * camFar/camNear;

			topLeft.Normalize();
			topLeft *= camScale;

			Vector3 topRight = (camtr.forward * camNear + toRight + toTop);
			topRight.Normalize();
			topRight *= camScale;

			Vector3 bottomRight = (camtr.forward * camNear + toRight - toTop);
			bottomRight.Normalize();
			bottomRight *= camScale;

			Vector3 bottomLeft = (camtr.forward * camNear - toRight - toTop);
			bottomLeft.Normalize();
			bottomLeft *= camScale;

			frustumCorners.SetRow (0, topLeft);
			frustumCorners.SetRow (1, topRight);
			frustumCorners.SetRow (2, bottomRight);
			frustumCorners.SetRow (3, bottomLeft);

			var camPos= camtr.position;
			float FdotC = camPos.y-height;
			float paramK = (FdotC <= 0.0f ? 1.0f : 0.0f);
			fogMaterial.SetMatrix ("_FrustumCornersWS", frustumCorners);
			fogMaterial.SetColor  ("_Color", color);
			fogMaterial.SetVector ("_CameraWS", camPos);
			fogMaterial.SetVector ("_HeightParams", new Vector4 (height, FdotC, paramK, heightDensity*0.5f));
			fogMaterial.SetVector ("_DistanceParams", new Vector4 (-Mathf.Max(startDistance,0.0f), 0, 0, 0));

			Vector4 sceneParams;
			bool  linear = (sceneMode == FogMode.Linear);
			float diff = linear ? sceneEnd - sceneStart : 0.0f;
			float invDiff = Mathf.Abs(diff) > 0.0001f ? 1.0f / diff : 0.0f;
			sceneParams.x = sceneDensity * 1.2011224087f; // density / sqrt(ln(2)), used by Exp2 fog mode
			sceneParams.y = sceneDensity * 1.4426950408f; // density / ln(2), used by Exp fog mode
			sceneParams.z = linear ? -invDiff : 0.0f;
			sceneParams.w = linear ? sceneEnd * invDiff : 0.0f;
			fogMaterial.SetVector ("_SceneFogParams", sceneParams);
			fogMaterial.SetVector ("_SceneFogMode", new Vector4((int)sceneMode, useRadialDistance ? 1 : 0, 0, 0));

			int pass = 0;
			if (distanceFog && heightFog)
				pass = 0; // distance + height
			else if (distanceFog)
				pass = 1; // distance only
			else
				pass = 2; // height only
			CustomGraphicsBlit (source, destination, fogMaterial, pass);
		}

		static void CustomGraphicsBlit (RenderTexture source, RenderTexture dest, Material fxMaterial, int passNr)
		{
			RenderTexture.active = dest;

			fxMaterial.SetTexture ("_MainTex", source);

			GL.PushMatrix ();
			GL.LoadOrtho ();

			fxMaterial.SetPass (passNr);

			GL.Begin (GL.QUADS);

			GL.MultiTexCoord2 (0, 0.0f, 0.0f);
			GL.Vertex3 (0.0f, 0.0f, 3.0f); // BL

			GL.MultiTexCoord2 (0, 1.0f, 0.0f);
			GL.Vertex3 (1.0f, 0.0f, 2.0f); // BR

			GL.MultiTexCoord2 (0, 1.0f, 1.0f);
			GL.Vertex3 (1.0f, 1.0f, 1.0f); // TR

			GL.MultiTexCoord2 (0, 0.0f, 1.0f);
			GL.Vertex3 (0.0f, 1.0f, 0.0f); // TL

			GL.End ();
			GL.PopMatrix ();
		}

		// from UnityStandardAssets.ImageEffects.PostEffectsBase

		
		protected bool  supportHDRTextures = true;
		protected bool  supportDX11 = false;
		protected bool  isSupported = true;

		protected Material CheckShaderAndCreateMaterial ( Shader s, Material m2Create)
		{
			if (!s)
			{
				Debug.Log("Missing shader in " + ToString ());
				enabled = false;
				return null;
			}

			if (s.isSupported && m2Create && m2Create.shader == s)
				return m2Create;

			if (!s.isSupported)
			{
				NotSupported ();
				Debug.Log("The shader " + s.ToString() + " on effect "+ToString()+" is not supported on this platform!");
				return null;
			}
			else
			{
				m2Create = new Material (s);
				m2Create.hideFlags = HideFlags.DontSave;
				if (m2Create)
					return m2Create;
				else return null;
			}
		}


		protected Material CreateMaterial (Shader s, Material m2Create)
		{
			if (!s)
			{
				Debug.Log ("Missing shader in " + ToString ());
				return null;
			}

			if (m2Create && (m2Create.shader == s) && (s.isSupported))
				return m2Create;

			if (!s.isSupported)
			{
				return null;
			}
			else
			{
				m2Create = new Material (s);
				m2Create.hideFlags = HideFlags.DontSave;
				if (m2Create)
					return m2Create;
				else return null;
			}
		}

		void OnEnable ()
		{
			isSupported = true;
		}

		protected bool CheckSupport ()
		{
			return CheckSupport (false);
		}


		protected void Start ()
		{
			CheckResources();
		}

		protected bool CheckSupport (bool needDepth)
		{
			isSupported = true;
			supportHDRTextures = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf);
			supportDX11 = SystemInfo.graphicsShaderLevel >= 50 && SystemInfo.supportsComputeShaders;

			if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
			{
				NotSupported ();
				return false;
			}

			if (needDepth && !SystemInfo.SupportsRenderTextureFormat (RenderTextureFormat.Depth))
			{
				NotSupported ();
				return false;
			}

			if (needDepth)
				GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;

			return true;
		}

		protected bool CheckSupport (bool needDepth,  bool needHdr)
		{
			if (!CheckSupport(needDepth))
				return false;

			if (needHdr && !supportHDRTextures)
			{
				NotSupported ();
				return false;
			}

			return true;
		}


		public bool Dx11Support ()
		{
			return supportDX11;
		}


		protected void ReportAutoDisable ()
		{
			Debug.LogWarning ("The image effect " + ToString() + " has been disabled as it's not supported on the current platform.");
		}

		protected void NotSupported ()
		{
			enabled = false;
			isSupported = false;
			return;
		}


		protected void DrawBorder (RenderTexture dest, Material material)
		{
			float x1;
			float x2;
			float y1;
			float y2;

			RenderTexture.active = dest;
			bool  invertY = true; // source.texelSize.y < 0.0ff;
			// Set up the simple Matrix
			GL.PushMatrix();
			GL.LoadOrtho();

			for (int i = 0; i < material.passCount; i++)
			{
				material.SetPass(i);

				float y1_; float y2_;
				if (invertY)
				{
					y1_ = 1.0f; y2_ = 0.0f;
				}
				else
				{
					y1_ = 0.0f; y2_ = 1.0f;
				}

				// left
				x1 = 0.0f;
				x2 = 0.0f + 1.0f/(dest.width*1.0f);
				y1 = 0.0f;
				y2 = 1.0f;
				GL.Begin(GL.QUADS);

				GL.TexCoord2(0.0f, y1_); GL.Vertex3(x1, y1, 0.1f);
				GL.TexCoord2(1.0f, y1_); GL.Vertex3(x2, y1, 0.1f);
				GL.TexCoord2(1.0f, y2_); GL.Vertex3(x2, y2, 0.1f);
				GL.TexCoord2(0.0f, y2_); GL.Vertex3(x1, y2, 0.1f);

				// right
				x1 = 1.0f - 1.0f/(dest.width*1.0f);
				x2 = 1.0f;
				y1 = 0.0f;
				y2 = 1.0f;

				GL.TexCoord2(0.0f, y1_); GL.Vertex3(x1, y1, 0.1f);
				GL.TexCoord2(1.0f, y1_); GL.Vertex3(x2, y1, 0.1f);
				GL.TexCoord2(1.0f, y2_); GL.Vertex3(x2, y2, 0.1f);
				GL.TexCoord2(0.0f, y2_); GL.Vertex3(x1, y2, 0.1f);

				// top
				x1 = 0.0f;
				x2 = 1.0f;
				y1 = 0.0f;
				y2 = 0.0f + 1.0f/(dest.height*1.0f);

				GL.TexCoord2(0.0f, y1_); GL.Vertex3(x1, y1, 0.1f);
				GL.TexCoord2(1.0f, y1_); GL.Vertex3(x2, y1, 0.1f);
				GL.TexCoord2(1.0f, y2_); GL.Vertex3(x2, y2, 0.1f);
				GL.TexCoord2(0.0f, y2_); GL.Vertex3(x1, y2, 0.1f);

				// bottom
				x1 = 0.0f;
				x2 = 1.0f;
				y1 = 1.0f - 1.0f/(dest.height*1.0f);
				y2 = 1.0f;

				GL.TexCoord2(0.0f, y1_); GL.Vertex3(x1, y1, 0.1f);
				GL.TexCoord2(1.0f, y1_); GL.Vertex3(x2, y1, 0.1f);
				GL.TexCoord2(1.0f, y2_); GL.Vertex3(x2, y2, 0.1f);
				GL.TexCoord2(0.0f, y2_); GL.Vertex3(x1, y2, 0.1f);

				GL.End();
			}

			GL.PopMatrix();
		}
	}
}


