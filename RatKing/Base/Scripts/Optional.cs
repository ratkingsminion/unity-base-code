#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RatKing.Base {

    // from https://www.youtube.com/watch?v=uZmWgQ7cLNI

    [System.Serializable]
#if ODIN_INSPECTOR
    [HideReferenceObjectPicker]
#endif
    //	public class Optional<T> {
    //		[field: SerializeField] public T Value { get; set; }
    //		[field: SerializeField] public bool Enabled { get; set; }
    //		public Optional(T value, bool enabled = true) { Value = value; Enabled = enabled; }
    //		public Optional<T> Set(T value, bool enabled = true) { Value = value; Enabled = enabled; return this; }
    //	}
    //
    //}

    public class Optional<T> {
        [SerializeField] private bool enabled;
        [SerializeField] private T value;

        public bool Enabled { get => enabled; set => enabled = value; }
        public T Value { get => value; set => this.value = value; }

        public Optional(T value, bool enabled = true) { this.value = value; this.enabled = enabled; }
        public Optional<T> Set(T value, bool enabled = true) { this.value = value; this.enabled = enabled; return this; }
    }

    public static class OptionalExtensions {
        public static T Get<T>(this Optional<T> opt, T std) {
            if (opt != null && opt.Enabled) { return opt.Value; }
            return std;
        }
        public static bool TryGet<T>(this Optional<T> opt, out T value) {
            if (opt != null && opt.Enabled) { value = opt.Value; return true; }
            value = default; return false;
        }
    }

}