using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace RatKing.Base {

	public class Poolable : MonoBehaviour {
		static Transform _poolParent;
		static readonly Dictionary<Poolable, Stack<Poolable>> _stackByPrefab = new Dictionary<Poolable, Stack<Poolable>>();

		protected virtual string PoolParentName { get { return "Standard"; } }
		protected virtual Transform PoolParent { get { return _poolParent; } set { _poolParent = value; } }
		protected virtual Dictionary<Poolable, Stack<Poolable>> StackByPrefab { get { return _stackByPrefab; } set { } }

		[SerializeField] int startCount = 10;
		[SerializeField] int addCount = 1;
		public enum Parenting {
			UsePoolParent, // a generated parent will be used for the pooled object
			UseOriginalParent, // the parent of the original will be used for the pooled object, and it will be set inactive
			DontChangeParent // no parent change will happen, the pooled object will be set inactive
		}
		[SerializeField] Parenting parenting = Parenting.UsePoolParent;
		[SerializeField] UnityEvent onPop = null;
		[SerializeField] UnityEvent onPush = null;

		Poolable original;
		bool isOriginal = true;

		ParticleSystem ps;

		//

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		static void OnRuntimeInitializeOnLoad() {
			_stackByPrefab.Clear();
		}

		//

		void Awake() {
			if (isOriginal && parenting == Parenting.UseOriginalParent) { gameObject.SetActive(false); }
		}

		void CreateInstanceInPool(Stack<Poolable> stack) {
			var go = (GameObject)Instantiate(original.gameObject);
			switch (parenting) {
				case Parenting.UsePoolParent:
					go.transform.SetParent(PoolParent);
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

			if (StackByPrefab.ContainsKey(this)) {
				return;
				// Debug.LogWarning("Trying to manually prepare the Poolable " + name + " more than once! You should fix that!");
			}

			if (PoolParent == null) {
				PoolParent = new GameObject("<" + PoolParentName + " Pool>").transform;
				GameObject.DontDestroyOnLoad(PoolParent.gameObject);
				PoolParent.gameObject.SetActive(false);
			}
			original = this;

			if (startCount < 0)
				startCount = this.startCount;

			var stack = StackByPrefab[original] = new Stack<Poolable>(startCount + 1);
			for (int i = 0; i < startCount; ++i) {
				CreateInstanceInPool(stack);
			}
		}

		//

		public T PoolPop<T>() where T : Component {
			return PoolPop().GetComponent<T>();
		}

		public T PoolPop<T>(Vector3 pos) where T : Component {
			return PoolPop(pos).GetComponent<T>();
		}

		public T PoolPop<T>(Vector3 pos, Quaternion rot) where T : Component {
			return PoolPop(pos, rot).GetComponent<T>();
		}

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
			if (original == null || !StackByPrefab.ContainsKey(original)) {
				PoolPrepare();
			}

			var stack = StackByPrefab[original];

			if (stack.Count == 0)
				for (int i = 0; i < addCount; ++i)
					CreateInstanceInPool(stack);
			var p = stack.Pop();
			var t = p.transform;
			switch (parenting) {
				case Parenting.UsePoolParent:
					t.SetParent(null);
#if UNITY_EDITOR
					PoolParent.name = "<" + PoolParentName + " Pool " + PoolParent.childCount + ">";
#endif
					break;
				case Parenting.UseOriginalParent:
					t.SetParent(original.transform.parent);
					p.gameObject.SetActive(true);
					break;
				case Parenting.DontChangeParent:
					p.gameObject.SetActive(true);
					break;
			}
			t.SetPositionAndRotation(pos, rot);
			t.localScale = original.transform.localScale;

			if (p.ps != null)
				p.ps.Play();

			if (p.onPop != null)
				p.onPop.Invoke();

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
					transform.SetParent(PoolParent);
#if UNITY_EDITOR
					PoolParent.name = "<" + PoolParentName + " Pool " + PoolParent.childCount + ">";
#endif
					break;
				case Parenting.UseOriginalParent:
				case Parenting.DontChangeParent:
					gameObject.SetActive(false);
					break;
			}
			var stack = StackByPrefab[original];
			stack.Push(this);

			if (onPush != null)
				onPush.Invoke();
		}

		public bool IsPushable() {
			return original != null;
		}

		//

		void OnValidate() {
			if (addCount < 1) addCount = 1;
			if (startCount < 0) startCount = 0;
		}
	}

}