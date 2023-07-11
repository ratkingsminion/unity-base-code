using UnityEngine;
using System.Collections;

namespace RatKing.Base {

	[SelectionBase]
	[RequireComponent(typeof(Creature))]
	public class PlayerInput : MonoBehaviour {
		[Header("Rotating")]
		[SerializeField] float yawRotSpeed = 10f;
		[SerializeField] float pitchRotSpeed = 10f;
		[SerializeField] float pitchMax = 80f;
		[SerializeField] float pitchMin = -80f;
		[SerializeField] float mouseSensitivity = 1f;
		[SerializeField] float smoothness = 1f;
		[Header("Components")]
		[SerializeField] Creature creature = null;
		
		float macMouseFactor = 1f;
		float pitch = 0f, smoothedPitch = 0f, smoothPitchVel = 0f;
		float yaw = 0f, smoothedYaw = 0f, smoothYawVel = 0f;
		bool catchMouse = true;
		Transform camDummy;

		public Creature Creature { get { return creature; } }
		public Transform RotTrans { get { return creature.RotateTransform; } }
		bool moveAllowed = true;

		//

		void Start() {
			creature = creature == null ? GetComponentInChildren<Creature>() : creature;
#if UNITY_WEBPLAYER
			if (Application.platform == RuntimePlatform.OSXWebPlayer) {
#elif UNITY_2017_1_OR_NEWER
			if (Application.platform == RuntimePlatform.OSXPlayer)
#else
			if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXDashboardPlayer)
#endif
			macMouseFactor = 0.18f;

			camDummy = new GameObject("CamDummy").transform;
			camDummy.SetPositionAndRotation(RotTrans.position, RotTrans.rotation);
			camDummy.SetParent(RotTrans.parent);
			RotTrans.SetParent(null);
		}

		void Update() {
			// move

			if (moveAllowed) {
				creature.FactorX = Input.GetAxis("Horizontal");
				creature.FactorZ = Input.GetAxis("Vertical");
			}
			else {
				creature.FactorX = creature.FactorZ = 0f;
			}

			// jump

			if (moveAllowed && Input.GetButtonDown("Jump"))
				creature.Jump();

			// look

			if (moveAllowed && Input.GetKeyDown(KeyCode.Tab)) {
				if (catchMouse) {
					Cursor.visible = true;
					Cursor.lockState = CursorLockMode.None;
				}
				catchMouse = !catchMouse;
			}

			if (catchMouse) {
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
			}
		}

		void LateUpdate() {
			if (moveAllowed && catchMouse) {
				yaw += Input.GetAxis("Mouse X") * yawRotSpeed * macMouseFactor * mouseSensitivity;
				smoothedYaw = Mathf.SmoothDamp(smoothedYaw, yaw, ref smoothYawVel, smoothness);

				pitch += Input.GetAxis("Mouse Y") * -pitchRotSpeed * macMouseFactor * mouseSensitivity * 1.1f;
				pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
				smoothedPitch = Mathf.SmoothDamp(smoothedPitch, pitch, ref smoothPitchVel, smoothness);

				RotTrans.localEulerAngles = new Vector3(smoothedPitch, Mathf.Repeat(smoothedYaw + 180f, 360f) - 180f, 0f);
			}

			RotTrans.position = camDummy.position;
		}

		void OnDisable() {
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}

		//

		public void AllowMove(bool allow) {
			if (!allow) {
				creature.FactorX = creature.FactorZ = 0f;
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
				catchMouse = false;
			}
			else {
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
				catchMouse = true;
			}
			moveAllowed = allow;
		}

		public void Warped(float yaw) {
			smoothedYaw = this.yaw = yaw;
			RotTrans.localEulerAngles = new Vector3(smoothedPitch, Mathf.Repeat(smoothedYaw + 180f, 360f) - 180f, 0f);
		}
	}

}