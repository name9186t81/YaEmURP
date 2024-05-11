using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using YaEm.Core;

namespace Global
{
	public class AreaSpawner : MonoBehaviour
	{
		[Header("Area settings")]
		[SerializeField] private Vector2 _size;
		[SerializeField] private GameObject[] _objects;
		[SerializeField] private float _delay;
		[SerializeField] private int _forcedTeam;
		public int MaxObjects;

		private List<GameObject> _objectsList = new List<GameObject>();
		private float _elapsed;
		private Transform _cached;

		protected virtual void Awake()
		{
			_cached = transform;
		}

		private void Update()
		{
			_elapsed += Time.deltaTime;
			if (_elapsed < _delay) return;

			_elapsed = 0f;
			CheckList();
			if (_objectsList.Count > MaxObjects) return;

			var obj = Instantiate(RandomObject, RandomPoint(), Quaternion.identity, null);
			_objectsList.Add(obj);
			if (obj.TryGetComponent<ITeamProvider>(out var prov))
			{
				if (!prov.TryChangeTeamNumber(_forcedTeam))
				{
					Debug.LogError("Cannot change team on " + obj.name);
				}
			}
		}

		private void CheckList()
		{
			_objectsList = (from o in _objectsList where o != null select o).ToList();
		}
		public Vector2 Size => _size;

		public Vector2 RandomPoint()
		{
			Vector2 halfSize = _size / 2;
			return (Vector2)_cached.position + new Vector2(Random.Range(-halfSize.x, halfSize.x), Random.Range(-halfSize.y, halfSize.y));
		}
#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			Gizmos.DrawWireCube(transform.position, _size);
		}
#endif
		private GameObject RandomObject => _objects[Random.Range(0, _objects.Length)];
	}

}