using System.Collections.Generic;
using UnityEngine;
using YaEm;

namespace HP.AI
{
	[RequireComponent(typeof(Grid))]
	public class PathFinding : MonoBehaviour, IService
	{
		[SerializeField] private Grid _grid;

		private void Awake()
		{
			_grid = GetComponent<Grid>();

			if (ServiceLocator.TryGet<PathFinding>(out var pathFinding))
			{
				pathFinding._grid = _grid;
			}
			else
			{
				ServiceLocator.Register(this);
			}
		}

		public IReadOnlyList<Vector2> GetRandomPath(Vector2 start, out Vector2 end)
		{
			const int MAXTRIES = 16;
			for (int i = 0; i < MAXTRIES; i++)
			{
				Node rn = _grid.RandomNode;
				if (!rn.Walkable) continue;

				end = rn.WorldPosition;
				return FIndPath(start, rn.WorldPosition);
			}

			end = Vector2.zero;
			return null;
		}

		public IReadOnlyList<Vector2> FIndPath(Vector2 start, Vector2 end)
		{
			Node st = _grid.NodeFromWorldPoint(start);
			Node en = _grid.NodeFromWorldPoint(end);

			const int LIMIT_CELL_PER_CALL = 35;
			int cells = 0;

			List<Node> open = new List<Node>();
			HashSet<Node> closed = new HashSet<Node>();
			open.Add(st);

			while (open.Count > 0)
			{
				Node cur = open[0];
				for (int i = 1; i < open.Count; i++)
				{
					Node ch = open[i];
					if (ch.FCost < cur.FCost || ch.FCost == cur.FCost && ch.HCost < cur.HCost)
					{
						cur = open[i];
					}
				}

				open.Remove(cur);
				closed.Add(cur);
				cells++;

				if(cells == LIMIT_CELL_PER_CALL)
				{
					return RetracePath(st, cur);
				}

				if (cur == en)
				{
					return RetracePath(st, en);
				}

				foreach (Node neighbour in cur.Neighbours)
				{
					if (!neighbour.Walkable || closed.Contains(neighbour)) continue;

					int newCost = cur.GCost + GetDistance(cur, neighbour);
					if (newCost < neighbour.GCost || !open.Contains(neighbour))
					{
						neighbour.GCost = newCost;
						neighbour.HCost = GetDistance(neighbour, en);
						neighbour.Previous = cur;

						if (!open.Contains(neighbour))
						{
							open.Add(neighbour);
						}
					}
				}
			}
			return null;
		}

		private IReadOnlyList<Vector2> RetracePath(Node start, Node end)
		{
			List<Node> path = new List<Node>();
			List<Vector2> pathV = new List<Vector2>();
			Node cur = end;

			while (cur != start)
			{
				path.Add(cur);
				pathV.Add(cur.WorldPosition);
				cur = cur.Previous;
			}

			_grid.Path = path;
			pathV.Reverse();
			return pathV;
		}

		private int GetDistance(Node a, Node b)
		{
			int rx = Mathf.Abs(a.XPos - b.XPos);
			int ry = Mathf.Abs(a.YPos - b.YPos);

			if (rx > ry)
			{
				return 14 * ry + 10 * (rx - ry);
			}
			return 14 * rx + 10 * (ry - rx);
		}
	}
}