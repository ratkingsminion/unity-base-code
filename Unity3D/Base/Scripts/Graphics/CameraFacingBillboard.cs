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
					var targetPos = cur.transform.position - camT.forward * cur.rotator;
					if (cur.upOnly) { targetPos.y = cur.transform.position.y; }
					cur.transform.LookAt(targetPos, camT.up);
				}
			}
		}

		//
		[SerializeField] bool upOnly = true;
		[SerializeField] bool rotate180 = true;
		//
		float rotator = 1f;

		//

		void Awake() {
			if (mgr == null) { CamFaceBillboards.Create(); }
			mgr.Add(this);
		}

		void Start() {
			if (rotate180) { rotator = -1f; }
		}

		void OnDestroy() {
			mgr.Remove(this);
		}
	}

}