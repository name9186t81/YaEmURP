using System;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
	[SerializeField] private LayerMask _unWalkable;
	[SerializeField] private Vector2 _worldSize;
	[SerializeField] private float _cellSize = 1f;
	[SerializeField] private bool _debug;
	private Transform _cached;

	private float _cellDiameter;
	private Vector2Int _size;
	private Node[,] _grid;

	private void Awake()
	{
		_cached = transform;
		_cellDiameter = _cellSize * 2;
		_size = new Vector2Int(Mathf.RoundToInt(_worldSize.x / _cellDiameter), Mathf.RoundToInt(_worldSize.y / _cellDiameter));

		_grid = new Node[_size.x, _size.y];
		Vector2 startPos = (Vector2)_cached.position - _worldSize / 2;

		for (int x = 0; x < _size.x; x++)
		{
			for (int y = 0; y < _size.y; y++)
			{
				Vector2 pos = startPos + new Vector2(x * _cellDiameter + _cellSize, y * _cellDiameter + _cellSize);
				bool isWalkable = !Physics2D.OverlapCircle(pos, _cellSize, _unWalkable);

				_grid[x, y] = new Node(isWalkable, pos, x, y);
			}
		}

		foreach(Node n in _grid)
		{
			n.SetNeighbours(GetNeighbours(n));
		}
	}

	public Vector2 WorldSize => _worldSize;
	public Node RandomNode => _grid[UnityEngine.Random.Range(0, _grid.GetLength(0)), UnityEngine.Random.Range(0, _grid.GetLength(1))];

	private void OnValidate()
	{
		if (_cellSize < 0.1f)
		{
			Debug.LogError("FailSafe: prevented divison by zero");
			_cellSize = 1f;
		}
	}

	private Node[] GetNeighbours(Node node)
	{
		List<Node> neighbours = new List<Node>();

		for (int i = -1; i < 2; i++)
		{
			for (int j = -1; j < 2; j++)
			{
				if (i == 0 && j == 0) continue;

				int cx = node.XPos + i;
				int cy = node.YPos + j;

				if (cx >= 0 && cx < _size.x && cy >= 0 && cy < _size.y)
				{
					neighbours.Add(_grid[cx, cy]);
				}
			}
		}

		return neighbours.ToArray();
	}

	public Node NodeFromWorldPoint(Vector2 worldPosition)
	{
		float percentX = (worldPosition.x - transform.position.x + _worldSize.x / 2) / _worldSize.x;
		float percentY = (worldPosition.y - transform.position.y + _worldSize.y / 2) / _worldSize.y;
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.RoundToInt((_size.x - 1) * percentX);
		int y = Mathf.RoundToInt((_size.y - 1) * percentY);

		return _grid[x, y];
	}

	public List<Node> Path = new List<Node>();
	private void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(transform.position, _worldSize);
		if (!_debug) return;

		if (_grid == null) return;

		foreach (Node n in _grid)
		{
			if (Path.Contains(n))
			{
				Gizmos.color = Color.black;
				Gizmos.DrawCube(n.WorldPosition, Vector3.one * (_cellDiameter - .1f));
				continue;
			}
			Gizmos.color = n.Walkable ? Color.Lerp(Color.green, Color.red, n.WallFactor) : Color.red;
			Gizmos.DrawCube(n.WorldPosition, Vector3.one * (_cellDiameter -.1f));
		}
	}
}

public sealed class Node
{
	public readonly bool Walkable;
	public readonly Vector2 WorldPosition;
	public readonly int XPos;
	public readonly int YPos;
	public Node[] Neighbours;
	public Node Previous;

	public float WallFactor;
	public int HCost;
	public int GCost;
	public int FCost => (HCost + GCost) + (int)((HCost + GCost) * WallFactor);

	public Node(bool walkable, Vector2 worldPos, int x, int y)
	{
		Walkable = walkable;
		WorldPosition = worldPos;
		XPos = x;
		YPos = y;
	}

	public void SetNeighbours(Node[] neighbours)
	{
		int total = neighbours.Length;
		int unWalkable = 0;
		for(int i = 0; i < total; i++)
		{
			if (neighbours[i].Walkable) continue;

			unWalkable++;
		}

		Neighbours = neighbours;
		WallFactor = (float)unWalkable / total;
	}
}