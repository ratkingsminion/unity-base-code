using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base {

	public class CameraFacingBillboard : MonoBehaviour {

		static CamFaceBillboards mgr;

		public class CamFaceBillboards : MonoBehaviour {
			List<CameraFacingBillboard> bbs;
			Transform camT;
			public static void Create() {
				var go = new GameObject("<Camera Facing Billboards>");
				mgr = go.AddComponent<CamFaceBillboards>();
				mgr.bbs = new List<CameraFacingBillboard>();
			}
			public void Add(CameraFacingBillboard bb) {
				bbs.Add(bb);
			}
			public void Remove(CameraFacingBillboard bb) {
				bbs.Remove(bb);
			}
			void Update() {
				if (camT == null || !camT.gameObject.activeInHierarchy) { var cam = Camera.main; if (cam == null) { return; } camT = cam.transform; }
				for (var iter = bbs.GetEnumerator(); iter.MoveNext(); ) {
					var cur = iter.Current;
					var targetPos = cur.transform.position - camT.forward;
					if (cur.upOnly) { targetPos.y = cur.transform.position.y; } // TODO
					cur.transform.LookAt(targetPos, camT.up);
					cur.transform.Rotate(cur.additionalRotation); // TODO
				}
			}
		}

		//

		[SerializeField] bool upOnly = true;
		[SerializeField] Vector3 additionalRotation = Vector3.zero;

		//

		public bool UpOnly { get { return upOnly; } set { upOnly = value; } }
		public Vector3 AdditionalRotation { get { return additionalRotation; } set { additionalRotation = value; } }

		void OnEnable() {
			if (mgr == null) { CamFaceBillboards.Create(); }
			mgr.Add(this);
		}

		void OnDisable() {
			mgr.Remove(this);
		}
	}

}