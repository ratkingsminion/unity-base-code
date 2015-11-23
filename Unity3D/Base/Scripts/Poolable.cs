using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base {

	public class Poolable : MonoBehaviour {
		static Transform poolParent;
		static Dictionary<Poolable, Stack<Poolable>> stackByPrefab = new Dictionary<Poolable, Stack<Poolable>>();
		//
		public int startCount = 10;
		public int addCount = 1;
		public enum Parenting {
			UsePoolParent,
			UseOriginalParent,
			DontChangeParent
		}
		public Parenting parenting;
		//
		Poolable original;
		bool isOriginal = true;
		//
		ParticleSystem ps;

		//

		void CreateInstanceInPool(Stack<Poolable> stack) {
			var go = (GameObject)Instantiate(original.gameObject);
			switch (parenting) {
				case Parenting.UsePoolParent:
					go.transform.SetParent(poolParent);
					break;
				case Parenting.UseOriginalParent:
					go.transform.SetParent(original.transform.parent);
					go.SetActive(false);
					break;
				case Parenting.DontChangeParent:
					go.SetActive(false);
					break;
			}
			go.name = original.name;
			var p = go.GetComponent<Poolable>();
			p.original = original;
			p.isOriginal = false;
			p.ps = go.GetComponent<ParticleSystem>();
			stack.Push(p);
		}

		/// <summary>
		/// Prepare the pool.
		/// Doesn't have to be called, but most often it's good practice. But call it ONLY ONCE!
		/// </summary>
		public void PoolPrepare(int startCount = -1) {
			if (!isOriginal)
				return;

			if (stackByPrefab.ContainsKey(this)) {
				return;
				// Debug.LogWarning("Trying to manually prepare the Poolable " + name + " more than once! You should fix that!");
			}

			if (poolParent == null) {
				poolParent = new GameObject("<Pool Parent>").transform;
				GameObject.DontDestroyOnLoad(poolParent.gameObject);
				poolParent.gameObject.SetActive(false);
			}
			original = this;

			if (startCount < 0)
				startCount = this.startCount;

			var stack = stackByPrefab[original] = new Stack<Poolable>(startCount + 1);
			for (int i = 0; i < startCount; ++i) {
				CreateInstanceInPool(stack);
			}
		}

		//

		public Poolable PoolPop() {
			if (original == null)
				return PoolPop(transform.position, transform.rotation);
			return PoolPop(original.transform.position, original.transform.rotation);
		}

		public Poolable PoolPop(Vector3 pos) {
			if (original == null)
				return PoolPop(pos, transform.rotation);
			return PoolPop(pos, original.transform.rotation);
		}

		public Poolable PoolPop(Vector3 pos, Quaternion rot) {
			if (original == null || !stackByPrefab.ContainsKey(original)) {
				PoolPrepare();
			}

			var stack = stackByPrefab[original];

			if (stack.Count == 0)
				for (int i = 0; i < addCount; ++i)
					CreateInstanceInPool(stack);
			var p = stack.Pop();
			var t = p.transform;
			switch (parenting) {
				case Parenting.UsePoolParent:
					t.SetParent(null);
					break;
				case Parenting.UseOriginalParent:
					t.SetParent(original.transform.parent);
					p.gameObject.SetActive(true);
					break;
				case Parenting.DontChangeParent:
					p.gameObject.SetActive(true);
					break;
			}
			t.position = pos;
			t.rotation = rot;
			t.localScale = original.transform.localScale;

			if (p.ps != null)
				p.ps.Play();

			return p;
		}

		public void PoolPush() {
			if (isOriginal) {
#if UNITY_EDITOR
				Debug.LogWarning("Trying to push the original prefab into the pool! You should fix that!");
#endif
				return;
			}
			switch (parenting) {
				case Parenting.UsePoolParent:
					transform.SetParent(poolParent);
					break;
				case Parenting.UseOriginalParent:
				case Parenting.DontChangeParent:
					gameObject.SetActive(false);
					break;
			}
			var stack = stackByPrefab[original];
			stack.Push(this);
		}

		//

		void OnValidate() {
			if (addCount < 1) addCount = 1;
			if (startCount < 0) startCount = 0;
		}
	}

}