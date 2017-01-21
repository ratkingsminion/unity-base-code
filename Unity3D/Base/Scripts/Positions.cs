using UnityEngine;
using System.Collections.Generic;

namespace RatKing.Base {

	// a 3d waypoint for pathfinding
	public class Waypoint<T> where T : IPosition {
		public T pos;
		public Vector3 worldPos;
		public bool solid;
		public List<Waypoint<T>> neighbours = new List<Waypoint<T>>();
		public Creature creature;
		public Creature nextCreature;
		//public int type;
		public Waypoint(T pos, bool solid) { this.pos = pos; worldPos = pos.ToVector(); this.solid = solid; }
	}

	public interface IPosition {
		Vector3 ToVector();
		int GetDistanceTo(IPosition other);
		int GetSqrDistanceTo(IPosition other);
		int GetManhattanDistanceTo(IPosition other);
		int GetSqrManhattanDistanceTo(IPosition other);
	}

	// integer 3d position - immutable
	[System.Serializable]
	public struct ImmutablePosition3 : IPosition, System.IEquatable<ImmutablePosition3> {
		public readonly int x, y, z;
		public static ImmutablePosition3 RoundedVector(Vector3 v) { return new ImmutablePosition3(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z)); }
		public static ImmutablePosition3 FlooredVector(Vector3 v) { return new ImmutablePosition3(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y), Mathf.FloorToInt(v.z)); }
		public static ImmutablePosition3 CeiledVector(Vector3 v) { return new ImmutablePosition3(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y), Mathf.CeilToInt(v.z)); }
		public ImmutablePosition3(int x = 0, int y = 0, int z = 0) { this.x = x; this.y = y; this.z = z; }
		//
		public int GetDistanceTo(IPosition other) {
			return (int)(other.ToVector() - ToVector()).magnitude;
		}
		public int GetSqrDistanceTo(IPosition other) {
			return (int)(other.ToVector() - ToVector()).sqrMagnitude;
		}
		public int GetManhattanDistanceTo(IPosition other) {
			var v = other.ToVector();
			return (int)(Mathf.Abs(x - v.x) + Mathf.Abs(y - v.y) + Mathf.Abs(z - v.z));
		}
		public int GetSqrManhattanDistanceTo(IPosition other) {
			int md = GetManhattanDistanceTo(other);
			return md * md;
		}
		public Vector3 ToVector() { return new Vector3(x, y, z); }
		public Vector3 ToVector(float width) { return new Vector3(x * width, y * width, z * width); }
		//
		public static bool operator ==(ImmutablePosition3 a, ImmutablePosition3 b) { return a.x == b.x && a.y == b.y && a.z == b.z; }
		public static bool operator !=(ImmutablePosition3 a, ImmutablePosition3 b) { return a.x != b.x || a.y != b.y || a.z != b.z; }
		public static ImmutablePosition3 operator +(ImmutablePosition3 a, ImmutablePosition3 b) { return new ImmutablePosition3(a.x + b.x, a.y + b.y, a.z + b.z); }
		public static ImmutablePosition3 operator -(ImmutablePosition3 a, ImmutablePosition3 b) { return new ImmutablePosition3(a.x - b.x, a.y - b.y, a.z - b.z); }
		public static ImmutablePosition3 operator *(ImmutablePosition3 p, int i) { return new ImmutablePosition3(p.x * i, p.y * i, p.z * i); }
		public static ImmutablePosition3 operator /(ImmutablePosition3 p, int i) { return new ImmutablePosition3(p.x / i, p.y / i, p.z / i); }
		public static ImmutablePosition3 operator %(ImmutablePosition3 p, int i) { return new ImmutablePosition3(p.x % i, p.y % i, p.z % i); }
		//
		public static ImmutablePosition3 zero    = new ImmutablePosition3(0, 0, 0);
		public static ImmutablePosition3 one     = new ImmutablePosition3(1, 1, 1);
		public static ImmutablePosition3 right   = new ImmutablePosition3(1, 0, 0);
		public static ImmutablePosition3 left    = new ImmutablePosition3(-1, 0, 0);
		public static ImmutablePosition3 up      = new ImmutablePosition3(0, 1, 0);
		public static ImmutablePosition3 down    = new ImmutablePosition3(0, -1, 0);
		public static ImmutablePosition3 forward = new ImmutablePosition3(0, 0, 1);
		public static ImmutablePosition3 back    = new ImmutablePosition3(0, 0, -1);
		//
		public override bool Equals(object o) { var p = (ImmutablePosition3)o; return p == this; }
		public override int GetHashCode() { return base.GetHashCode(); }
		public override string ToString() { return x + ", " + y + ", " + z; }
		//
		public bool Equals(Position3 other) { return x == other.x && y == other.y && z == other.z; }
		public bool Equals(ImmutablePosition3 other) { return x == other.x && y == other.y && z == other.z; }
	}

	// integer 3d position
	[System.Serializable]
	public struct Position3 : IPosition, System.IEquatable<Position3> {
		public int x, y, z;
		public static Position3 RoundedVector(Vector3 v) { return new Position3(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z)); }
		public static Position3 FlooredVector(Vector3 v) { return new Position3(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y), Mathf.FloorToInt(v.z)); }
		public static Position3 CeiledVector(Vector3 v) { return new Position3(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y), Mathf.CeilToInt(v.z)); }
		public Position3(int x = 0, int y = 0, int z = 0) { this.x = x; this.y = y; this.z = z; }
		public void Set(int x, int y, int z) { this.x = x; this.y = y; this.z = z; }
		public void Reset() { x = y = z = 0; }
		//
		public int GetDistanceTo(IPosition other) {
			return (int)(other.ToVector() - ToVector()).magnitude;
		}
		public int GetSqrDistanceTo(IPosition other) {
			return (int)(other.ToVector() - ToVector()).sqrMagnitude;
		}
		public int GetManhattanDistanceTo(IPosition other) {
			var v = other.ToVector();
			return (int)(Mathf.Abs(x - v.x) + Mathf.Abs(y - v.y) + Mathf.Abs(z - v.z));
		}
		public int GetSqrManhattanDistanceTo(IPosition other) {
			int md = GetManhattanDistanceTo(other);
			return md * md;
		}
		public Vector3 ToVector() { return new Vector3(x, y, z); }
		public Vector3 ToVector(float width) { return new Vector3(x * width, y * width, z * width); }
		//
		public static bool operator ==(Position3 a, Position3 b) { return a.x == b.x && a.y == b.y && a.z == b.z; }
		public static bool operator !=(Position3 a, Position3 b) { return a.x != b.x || a.y != b.y || a.z != b.z; }
		public static Position3 operator +(Position3 a, Position3 b) { return new Position3(a.x + b.x, a.y + b.y, a.z + b.z); }
		public static Position3 operator -(Position3 a, Position3 b) { return new Position3(a.x - b.x, a.y - b.y, a.z - b.z); }
		public static Position3 operator *(Position3 p, int i) { return new Position3(p.x * i, p.y * i, p.z * i); }
		public static Position3 operator /(Position3 p, int i) { return new Position3(p.x / i, p.y / i, p.z / i); }
		public static Position3 operator %(Position3 p, int i) { return new Position3(p.x % i, p.y % i, p.z % i); }
		//
		public static Position3 zero { get { return new Position3(0, 0, 0); } }
		public static Position3 one { get { return new Position3(1, 1, 1); } }
		public static Position3 right { get { return new Position3(1, 0, 0); } }
		public static Position3 left { get { return new Position3(-1, 0, 0); } }
		public static Position3 up { get { return new Position3(0, 1, 0); } }
		public static Position3 down { get { return new Position3(0, -1, 0); } }
		public static Position3 forward { get { return new Position3(0, 0, 1); } }
		public static Position3 back { get { return new Position3(0, 0, -1); } }
		//
		public override bool Equals(object o) { Position3 p = (Position3)o; return p == this; }
		public override int GetHashCode() { return base.GetHashCode(); } // TODO change
		public override string ToString() { return x + ", " + y + ", " + z; }
		//
		public bool Equals(Position3 other) { return x == other.x && y == other.y && z == other.z; }
		public bool Equals(ImmutablePosition3 other) { return x == other.x && y == other.y && z == other.z; }
	}

	// integer 2d position - immutable
	[System.Serializable]
	public struct ImmutablePosition2 : IPosition, System.IEquatable<ImmutablePosition2> {
		public readonly int x, y;
		public static ImmutablePosition2 RoundedVector(Vector2 v) { return new ImmutablePosition2(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y)); }
		public static ImmutablePosition2 FlooredVector(Vector2 v) { return new ImmutablePosition2(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y)); }
		public static ImmutablePosition2 CeiledVector(Vector2 v) { return new ImmutablePosition2(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y)); }
		public ImmutablePosition2(int x = 0, int y = 0) { this.x = x; this.y = y; }
		//
		public int GetDistanceTo(IPosition other) {
			return (int)(other.ToVector() - ToVector()).magnitude;
		}
		public int GetSqrDistanceTo(IPosition other) {
			return (int)(other.ToVector() - ToVector()).sqrMagnitude;
		}
		public int GetManhattanDistanceTo(IPosition other) {
			var v = other.ToVector();
			return (int)(Mathf.Abs(x - v.x) + Mathf.Abs(y - v.y));
		}
		public int GetSqrManhattanDistanceTo(IPosition other) {
			int md = GetManhattanDistanceTo(other);
			return md * md;
		}
		public Vector3 ToVector() { return new Vector3(x, y, 0f); }
		//
		public static bool operator ==(ImmutablePosition2 a, ImmutablePosition2 b) { return a.x == b.x && a.y == b.y; }
		public static bool operator !=(ImmutablePosition2 a, ImmutablePosition2 b) { return a.x != b.x || a.y != b.y; }
		public static ImmutablePosition2 operator +(ImmutablePosition2 a, ImmutablePosition2 b) { return new ImmutablePosition2(a.x + b.x, a.y + b.y); }
		public static ImmutablePosition2 operator -(ImmutablePosition2 a, ImmutablePosition2 b) { return new ImmutablePosition2(a.x - b.x, a.y - b.y); }
		public static ImmutablePosition2 operator *(ImmutablePosition2 p, int i) { return new ImmutablePosition2(p.x * i, p.y * i); }
		public static ImmutablePosition2 operator /(ImmutablePosition2 p, int i) { return new ImmutablePosition2(p.x / i, p.y / i); }
		public static ImmutablePosition2 operator %(ImmutablePosition2 p, int i) { return new ImmutablePosition2(p.x % i, p.y % i); }
		//
		public static ImmutablePosition2 zero  = new ImmutablePosition2(0, 0);
		public static ImmutablePosition2 one   = new ImmutablePosition2(1, 1);
		public static ImmutablePosition2 right = new ImmutablePosition2(1, 0);
		public static ImmutablePosition2 left  = new ImmutablePosition2(-1, 0);
		public static ImmutablePosition2 up    = new ImmutablePosition2(0, 1);
		public static ImmutablePosition2 down  = new ImmutablePosition2(0, -1);
		//
		public override bool Equals(object o) { try { return (bool)(this == (ImmutablePosition2)o); } catch { return false; } }
		public override int GetHashCode() { return base.GetHashCode(); } // TODO change
		public override string ToString() { return x + ", " + y; }
		//
		public bool Equals(Position2 other) { return x == other.x && y == other.y; }
		public bool Equals(ImmutablePosition2 other) { return x == other.x && y == other.y; }
	}

	// integer 2d position
	[System.Serializable]
	public struct Position2 : IPosition, System.IEquatable<Position2> {
		public int x, y;
		public static Position2 RoundedVector(Vector2 v) { return new Position2(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y)); }
		public static Position2 FlooredVector(Vector2 v) { return new Position2(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y)); }
		public static Position2 CeiledVector(Vector2 v) { return new Position2(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y)); }
		public Position2(int x = 0, int y = 0) { this.x = x; this.y = y; }
		public void Set(int x, int y) { this.x = x; this.y = y; }
		public void Reset() { x = y = 0; }
		//
		public int GetDistanceTo(IPosition other) {
			return (int)(other.ToVector() - ToVector()).magnitude;
		}
		public int GetSqrDistanceTo(IPosition other) {
			return (int)(other.ToVector() - ToVector()).sqrMagnitude;
		}
		public int GetManhattanDistanceTo(IPosition other) {
			var v = other.ToVector();
			return (int)(Mathf.Abs(x - v.x) + Mathf.Abs(y - v.y));
		}
		public int GetSqrManhattanDistanceTo(IPosition other) {
			int md = GetManhattanDistanceTo(other);
			return md * md;
		}
		public Vector3 ToVector() { return new Vector3(x, y, 0f); }
		//
		public static bool operator ==(Position2 a, Position2 b) { return a.x == b.x && a.y == b.y; }
		public static bool operator !=(Position2 a, Position2 b) { return a.x != b.x || a.y != b.y; }
		public static Position2 operator +(Position2 a, Position2 b) { return new Position2(a.x + b.x, a.y + b.y); }
		public static Position2 operator -(Position2 a, Position2 b) { return new Position2(a.x - b.x, a.y - b.y); }
		public static Position2 operator *(Position2 p, int i) { return new Position2(p.x * i, p.y * i); }
		public static Position2 operator /(Position2 p, int i) { return new Position2(p.x / i, p.y / i); }
		public static Position2 operator %(Position2 p, int i) { return new Position2(p.x % i, p.y % i); }
		//
		public static Position2 zero { get { return new Position2(0, 0); } }
		public static Position2 one { get { return new Position2(1, 1); } }
		public static Position2 right { get { return new Position2(1, 0); } }
		public static Position2 left { get { return new Position2(-1, 0); } }
		public static Position2 up { get { return new Position2(0, 1); } }
		public static Position2 down { get { return new Position2(0, -1); } }
		//
		public override bool Equals(object o) { try { return (bool)(this == (Position2)o); } catch { return false; } }
		public override int GetHashCode() { return base.GetHashCode(); }
		public override string ToString() { return x + ", " + y; }
		//
		public bool Equals(Position2 other) { return x == other.x && y == other.y; }
		public bool Equals(ImmutablePosition2 other) { return x == other.x && y == other.y; }
	}
}