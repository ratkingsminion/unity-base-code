using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base {

	public static class Pathfinding {
		public class Waypoint<T> where T : IPosition { // TODO make poolable
			public T pos;
			public Vector3 worldPos;
			public bool solid;
			public List<Waypoint<T>> neighbours = new List<Waypoint<T>>();
			public Creature creature;
			public Creature nextCreature;
			//public int type;
			public Waypoint(T pos, bool solid) { this.pos = pos; worldPos = pos.ToVector(); this.solid = solid; }
		}

		public class Node<T> where T : IPosition { // TODO make poolable
			public Node<T> parent = null;
			public Waypoint<T> block;
			public int F = 0;
			public int G = 0;
			public int H = 0;
			public Node(Waypoint<T> block) { this.block = block; }
			public Node(Waypoint<T> block, Node<T> parent, int F, int G, int H) { this.block = block; this.parent = parent; this.F = F; this.G = G; this.H = H; }
		};

		// Finds A Path Between Two Blocks, Returns The Path In An Array
		public static bool Find<T>(Waypoint<T> A, Waypoint<T> B, out Waypoint<T>[] path, int maxLength, bool ignoreCreatures) where T : IPosition {
			if (A == null || B == null) {
				path = null;
				return false;
			}

			path = null;
			List<Node<T>> open = new List<Node<T>>(maxLength);
			List<Node<T>> closed = new List<Node<T>>(maxLength);
			Node<T> current = null;
			open.Add(new Node<T>(A));

			// pathfinding!
			for (;;) {

				// no nodes any more, but target not found? -> end
				if (open.Count == 0)
					return false;

				current = open[0]; // list is sorted!

				// remove node from open list and move it into closed list
				open.Remove(current);
				closed.Add(current);

				// target found? -> end the search!
				if (current.block == B) {
					int pathLength = 0;
					Node<T> counter = current;

					do // go backwards, for counting only
					{

						counter = counter.parent;
						++pathLength;
					} while (counter != null);

					path = new Waypoint<T>[pathLength];

					// go forward, for building path
					for (int i = 0; i < pathLength; ++i) {
						path[pathLength - i - 1] = current.block;
						current = current.parent;
					}

					return true;
				}

				int G = current.G + 1; // Position.GetSqrDistance(current.block.absPosition, neighbour.absPosition);
				if (G > maxLength) continue; // return false;

				// iterate through connections
				int nm = current.block.neighbours.Count;
				for (int i = 0; i < nm; ++i) {
					Waypoint<T> neighBlock = current.block.neighbours[i];
					if (neighBlock == null || (!ignoreCreatures && neighBlock != B && neighBlock.creature != null))
						continue;

					// node is already in closed list?
					if (closed.Find(n => n.block == neighBlock) != null)
						continue;

					// node is already in open list?
					Node<T> oldNode = open.Find(n => n.block == neighBlock);
					if (oldNode != null) {
						if (G < oldNode.G) {
							oldNode.G = G;
							oldNode.F = G + oldNode.H;
							oldNode.parent = current;
							open.Sort((Node<T> a, Node<T> b) => a.F - b.F);
						}
					}
					else {
						int H = neighBlock.pos.GetManhattanDistanceTo(B.pos);
						// int H = Mathf.RoundToInt(Mathf.Abs(neighBlock.worldPos.x - B.worldPos.x) + Mathf.Abs(neighBlock.worldPos.y - B.worldPos.y) + Mathf.Abs(neighBlock.worldPos.z - B.worldPos.z));
						int F = G + H;
						Node<T> newNode = new Node<T>(neighBlock, current, F, G, H);
						open.Add(newNode);
						open.Sort((Node<T> a, Node<T> b) => a.F - b.F);
					}
				}
			}
		}
	}

}