using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RatKing.Base {

	public class TextWithBack : MonoBehaviour {
		[SerializeField] bool initOnAwake = false;
		[SerializeField] TMPro.TextMeshProUGUI text = null;
		[SerializeField] RectTransform back = null;
		[SerializeField] List<Image> additionalFades = null;
		[SerializeField] bool changeBackHorizontal = true;
		[SerializeField] bool changeBackVertical = true;
		[SerializeField] Vector2 padding = new Vector2(0f, 0f);
		[SerializeField] Vector2 minSize = new Vector2(0f, 0f);
		[SerializeField] Vector2 maxSize = new Vector2(0f, 0f);
		[SerializeField] float wobbleAnimStrength = 1f;
		//
		RectTransform rectTrans;
		public RectTransform RectTrans { get { return rectTrans != null ? rectTrans : rectTrans = GetComponent<RectTransform>(); } }
		//
		public RectTransform Back { get { return back; } }
		public TMPro.TextMeshProUGUI Text { get { return text; } }
		public Vector2 MinSize { get { return minSize; } set { minSize = value; } }
		public Vector2 MaxSize { get { return maxSize; } set { maxSize = value; } }
		//
		public Image BackImage { get; private set; }
		//
		bool awoken, fadingIn, fadingOut;
		Color maxColorText, minColorText;
		Color[] maxColorFades, minColorFades, tempColorFades;
		int tweenID;

		//

		void Awake() {
			if (awoken) { return; }
			awoken = true;
			BackImage = back.GetComponent<Image>();
			if (BackImage != null && !additionalFades.Contains(BackImage)) { additionalFades.Add(BackImage); }
			maxColorText = text.color; minColorText = maxColorText.WithNoAlpha();
			minColorFades = new Color[additionalFades.Count];
			maxColorFades = new Color[additionalFades.Count];
			tempColorFades = new Color[additionalFades.Count];
			for (int i = additionalFades.Count - 1; i >= 0; --i) {
				maxColorFades[i] = additionalFades[i].color;
				minColorFades[i] = maxColorFades[i].WithNoAlpha();
			}
			if (initOnAwake) { SetText(text.text, false, true); }
		}

		//

		public void SetText(string message, bool withAnim = false, bool changeBack = true) {
			//if (text.text == message) { return; }
			var textSize = text.rectTransform.sizeDelta;
			var newSize = text.rectTransform.sizeDelta;
			newSize.x = Mathf.Max(newSize.x, minSize.x);
			newSize.y = Mathf.Max(newSize.y, minSize.y);
			if (maxSize.x > 0f) { newSize.x = Mathf.Min(newSize.x, maxSize.x); }
			if (maxSize.y > 0f) { newSize.y = Mathf.Min(newSize.y, maxSize.y); }
			if (changeBackHorizontal) { textSize.x = newSize.x; }
			if (changeBackVertical) { textSize.y = newSize.y; }
			//
			if (changeBackHorizontal && changeBackVertical) { text.rectTransform.sizeDelta = new Vector2(100000f, 100000f); }
			else if (changeBackHorizontal) { text.rectTransform.SetSizeDeltaX(10000f); }
			else if (changeBackVertical) { text.rectTransform.SetSizeDeltaY(10000f); }
			text.text = message;
			//
			if (changeBack) {
				text.ForceMeshUpdate(true);
				UpdateBack();
			}
			else {
				text.rectTransform.sizeDelta = textSize;
			}
			//
			if (withAnim) { Base.WobbleAnimations.StartFor(back, 0.7f, wobbleAnimStrength); }
		}

		public void UpdateBack() {
			var backSize = back.sizeDelta;
			var newSize = text.textBounds.size.ToVec2() + padding;
			newSize.x = Mathf.Max(newSize.x, minSize.x);
			newSize.y = Mathf.Max(newSize.y, minSize.y);
			if (maxSize.x > 0f) { newSize.x = Mathf.Min(newSize.x, maxSize.x); }
			if (maxSize.y > 0f) { newSize.y = Mathf.Min(newSize.y, maxSize.y); }
			if (changeBackHorizontal) { backSize.x = newSize.x; }
			if (changeBackVertical) { backSize.y = newSize.y; }
			back.sizeDelta = backSize;
			//
			if (changeBackHorizontal && changeBackVertical) { text.rectTransform.sizeDelta = backSize; }
			else if (changeBackHorizontal) { text.rectTransform.SetSizeDeltaX(backSize.x); }
			else if (changeBackVertical) { text.rectTransform.SetSizeDeltaY(backSize.y); }
			text.ForceMeshUpdate(true);
		}

		public void FadeIn(float seconds) {
			if (fadingIn) { return; }
			if (!awoken) { Awake(); }
			if (fadingOut) { fadingOut = false; Tweens.Stop(tweenID); }
			for (int i = additionalFades.Count - 1; i >= 0; --i) {
				tempColorFades[i] = additionalFades[i].color;
				if (!gameObject.activeSelf) { additionalFades[i].color = minColorFades[i]; }
			}
			var startColorText = gameObject.activeSelf ? text.color : minColorText;
			if (!gameObject.activeSelf) { text.color = minColorText; }
			gameObject.SetActive(true);
			if (seconds > 0f) {
				fadingIn = true;
				tweenID = Tweens.Do(0f, 1f, seconds)
					.IgnoreTimeScale(true)
					.Ease(Tweens.Ease.SmoothStep)
					.OnUpdate((float f) => {
						for (int i = additionalFades.Count - 1; i >= 0; --i) { additionalFades[i].color = Color.Lerp(tempColorFades[i], maxColorFades[i], f); }
						text.color = Color.Lerp(startColorText, maxColorText, f);
					})
					.OnComplete(() => {
						fadingIn = false;
					})
					.id;
			}
			else {
				for (int i = additionalFades.Count - 1; i >= 0; --i) { additionalFades[i].color = maxColorFades[i]; }
				text.color = maxColorText;
			}
		}

		public void FadeOut(float seconds) {
			if (fadingOut || !gameObject.activeSelf) { return; }
			if (!awoken) { Awake(); }
			if (fadingIn) { fadingIn = false; Tweens.Stop(tweenID); }
			for (int i = additionalFades.Count - 1; i >= 0; --i) { tempColorFades[i] = additionalFades[i].color; }
			var startColorText = text.color;
			if (seconds > 0f) {
				fadingOut = true;
				tweenID = Tweens.Do(0f, 1f, seconds)
					.IgnoreTimeScale(true)
					.Ease(Tweens.Ease.SmoothStep)
					.OnUpdate((float f) => {
						for (int i = additionalFades.Count - 1; i >= 0; --i) { additionalFades[i].color = Color.Lerp(tempColorFades[i], minColorFades[i], f); }
						text.color = Color.Lerp(startColorText, minColorText, f);
					})
					.OnComplete(() => {
						gameObject.SetActive(false);
						fadingOut = false;
					})
					.id;
			}
			else {
				for (int i = additionalFades.Count - 1; i >= 0; --i) { additionalFades[i].color = minColorFades[i]; }
				text.color = minColorText;
				gameObject.SetActive(false);
			}
		}
	}

}
