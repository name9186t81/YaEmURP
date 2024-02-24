using System;
using System.Collections.Generic;
using UnityEngine;

using YaEm.Core;

namespace YaEm.AI {
	[DisallowMultipleComponent]
	public class AIVision : MonoBehaviour
	{
		[SerializeField] private float _range;
		[SerializeField] private float _angle = 360;
		[SerializeField] private float _scanFrequancy;
		[SerializeField] private LayerMask _scanMask;
		[SerializeField] private LayerMask _wallsMask;
		[SerializeField] private LayerMask _ignoredInVision;
		[SerializeField] private bool _debug;
		[SerializeField] private bool _canSeeThroughWalls;
		private float _delay;
		private AIController _controller;
		private IList<IActor> _cachedScan;
		private IList<IActor> _enemies;
		private IList<IActor> _alies;
		private bool _haveCollider;

		public event Action OnScan;
		public IReadOnlyList<IActor> ActorsInRange => (IReadOnlyList<IActor>)_cachedScan;
		public IReadOnlyList<IActor> EnemiesInRange => (IReadOnlyList<IActor>)_enemies;
		public IReadOnlyList<IActor> AliesInRange => (IReadOnlyList<IActor>)_alies;

		public void Init(AIController controller)
		{
			enabled = true;
			_controller = controller;
		}

		private void Update()
		{
			_delay += Time.deltaTime;
			if (_delay > _scanFrequancy)
			{
				ForceScan();
			}
		}

		public bool CanSeePoint(Vector2 start, Vector2 end, bool seeIgnoredWalls)
		{
			return seeIgnoredWalls ? !Physics2D.Linecast(start, end, _wallsMask) : !Physics2D.Linecast(start, end, _wallsMask & ~_ignoredInVision);
		}
		public bool CanSeePoint(Vector2 point, bool seeIgnoredWalls)
		{
			//return (!_haveCollider && !Physics2D.Linecast(_controller.Position, point, _wallsMask)) ||
			//		(_haveCollider && Physics2D.LinecastAll(_controller.Position, point, _wallsMask).Length == 1);
			return seeIgnoredWalls ? !Physics2D.Linecast(_controller.Position, point, _wallsMask) : !Physics2D.Linecast(_controller.Position, point, _wallsMask & ~_ignoredInVision);
		}

		public bool CanSeeTarget(IActor target, bool seeIgnoredWalls)
		{
			//return (!_haveCollider && Physics2D.LinecastAll(_controller.Position, target.Position, _wallsMask)[0].transform.TryGetComponent<IActor>(out var act) && act == target) ||
			//		(_haveCollider && Physics2D.LinecastAll(_controller.Position, target.Position, _wallsMask)[1].transform.TryGetComponent<IActor>(out var act2) && act2 == target);
			return CanSeePoint(target.Position, seeIgnoredWalls);
		}

		private static Collider2D[] _cachedHits = new Collider2D[64];

		public void ForceScan()
		{
			_delay = 0;

			int hits = Physics2D.OverlapCircleNonAlloc(_controller.Position, _range, _cachedHits, _scanMask);

			IList<IActor> listed = ClearForActors(_cachedHits);
			if (!_canSeeThroughWalls) listed = ClearForWalls(listed);
			if (_angle < 360) listed = ClearForAngle(listed);

			_cachedScan = listed;
			if (_controller.Actor is ITeamProvider prov)
			{
				_enemies = SortForTeamNumber(prov.TeamNumber, true);
				_alies = SortForTeamNumber(prov.TeamNumber, false);
			}
			OnScan?.Invoke();
		}

		public IList<IActor> SortForTeamNumber(int teamNumber, bool excluded)
		{
			IList<IActor> list = new List<IActor>();
			foreach (var el in _cachedScan)
			{
				if (!(el is ITeamProvider prov)) continue;

				if (prov.TeamNumber == teamNumber && !excluded || excluded && prov.TeamNumber != teamNumber)
				{
					list.Add(el);
				}
			}
			return list;
		}

		public IList<T> SortForT<T>() where T : class
		{
			IList<T> list = new List<T>();
			foreach (var el in _cachedScan)
			{
				if (el is T t)
				{
					list.Add(t);
				}
			}
			return list;
		}

		private IList<IActor> ClearForActors(IList<Collider2D> hits)
		{
			IList<IActor> newList = new List<IActor>();

			foreach (var col in hits)
			{
				if (col == null) break;

				if (col.transform.TryGetComponent<IActor>(out var act) && act.ToString() != "null")
				{
					newList.Add(act);
				}
			}
			return newList;
		}

		private IList<IActor> ClearForWalls(IList<IActor> hits)
		{
			IList<IActor> newList = new List<IActor>();

			foreach (var col in hits)
			{
				if (!Physics2D.Linecast(_controller.Position, col.Position, _wallsMask & ~_ignoredInVision))
				{
					newList.Add(col);
				}
			}
			return newList;
		}


		private IList<IActor> ClearForAngle(IList<IActor> hits)
		{
			IList<IActor> newList = new List<IActor>();
			float angle1 = -AngleRad / 2 + ActorDirection.AngleRadFromVector();
			float angle2 = AngleRad / 2 + ActorDirection.AngleRadFromVector();
			foreach (var col in hits)
			{
				Vector2 dir = col.Position - _controller.Position;
				float angle = dir.AngleRadFromVector();
				if (angle > angle1 && angle < angle2)
				{
					Debug.Log("Spotted");
					newList.Add(col);
				}
			}
			return newList;
		}

		private void OnValidate()
		{
			_angle = Mathf.Clamp(_angle, 0, 360);
			_range = Mathf.Max(_range, 0);
			_scanFrequancy = Mathf.Max(_scanFrequancy, 0);
		}
#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			if (!_debug) return;

			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(transform.position, _range);

			Gizmos.color = Color.green;
			float radAngle = (_angle) * Mathf.Deg2Rad;
			Vector3 dir = (radAngle / 2 + ((Vector2)transform.up).AngleRadFromVector()).VectorFromAngle();
			Vector3 dir2 = (-radAngle / 2 + ((Vector2)transform.up).AngleRadFromVector()).VectorFromAngle();
			Gizmos.DrawLine(transform.position, dir * _range + transform.position);
			Gizmos.DrawLine(transform.position, dir2 * _range + transform.position);
		}
#endif

		private float AngleRad => _angle * Mathf.Deg2Rad;
		private Vector2 ActorDirection => (_controller.DesiredMoveDirection);
		public int AlliesInRangeCount => _alies == null ? 0 : _alies.Count;
		public int EnemiesInRangeCount => _enemies == null ? 0 : _enemies.Count;
	}
}