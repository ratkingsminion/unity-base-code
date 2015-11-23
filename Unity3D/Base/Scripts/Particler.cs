using UnityEngine;
using System.Collections;

namespace RatKing.Base {

	public class Particler : MonoBehaviour {
		[Tooltip("Can be null, will get the first ParticleSystem component.")]
		public ParticleSystem particles;
		[Tooltip("Changing this after creation will not result in anything.")]
		public Transform follow;
		public bool followWithoutRotation;
		//
		IEnumerator following;

		//

		public Particler Instantiate(Transform start, bool follow) {
			var go = (GameObject)Instantiate(gameObject, start.position, start.rotation);
			var bp = go.GetComponent<Particler>();
			if (follow)
				bp.follow = start;
			return bp;
		}

		public Particler Instantiate(Vector3 position, Quaternion rotation) {
			var go = (GameObject)Instantiate(gameObject, position, rotation);
			return go.GetComponent<Particler>();
		}

		public Particler Instantiate(Vector3 position) {
			var go = (GameObject)Instantiate(gameObject, position, Quaternion.identity);
			return go.GetComponent<Particler>();
		}

		public Particler Instantiate() {
			var go = (GameObject)Instantiate(gameObject);
			return go.GetComponent<Particler>();
		}

		//

		public void Follow(Transform follow) {
			if (follow != null) {
				if (this.follow == null) {
					StopAllCoroutines();
				}
				else {
					StopCoroutine(following);
				}
				StartCoroutine(following = FollowCR());
			}
		}

		public void StopParticles() {
			if (particles != null)
				particles.enableEmission = false;
		}

		//

		IEnumerator Start() {
			if (particles == null)
				particles = GetComponent<ParticleSystem>();
			if (follow != null) {
				StartCoroutine(FollowCR());
			}

			yield return new WaitForSeconds(particles.startDelay);
			var wait = new WaitForSeconds(0.25f);

			for (;;) {
				yield return wait;
				if (!particles.IsAlive() || particles.particleCount == 0)
					Destroy(gameObject);
			}
		}

		IEnumerator FollowCR() {
			while (follow != null) {
				transform.position = follow.position;
				if (!followWithoutRotation)
					transform.rotation = follow.rotation;

				yield return null;
			}
		}
	}

}