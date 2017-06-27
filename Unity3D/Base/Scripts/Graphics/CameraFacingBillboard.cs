using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base {

	public class CameraFacingBillboard : MonoBehaviour {

		static CamFaceBillboards mgr;

		public class CamFaceBillboards : MonoBehaviour {
			List<CameraFacingBillboard> bbs;
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
				if (Camera.main == null) { return; }
				var camT = Camera.main.transform;
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

		void Awake() {
			if (mgr == null) { CamFaceBillboards.Create(); }
			mgr.Add(this);
		}

		void OnDestroy() {
			mgr.Remove(this);
		}
	}

}