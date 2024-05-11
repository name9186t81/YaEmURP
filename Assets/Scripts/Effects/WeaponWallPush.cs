using UnityEngine;

using YaEm.Weapons;

namespace YaEm.Effects
{
	public class WeaponWallPush : MonoBehaviour
	{
		[SerializeField] private RangedWeapon _weapon;
		[SerializeField] private Transform _transform;
		[SerializeField] private float _length;
		[SerializeField] private Vector2 _offset;
		[SerializeField] private LayerMask _mask;
		[SerializeField] private bool _debug;
		private float _startAngle;
		private float _startOffset;
		private bool _isDirty;

		private void Awake()
		{
			_weapon.OnInit += Init;
		}

		private void Init()
		{
			_weapon.OnInit -= Init;
			_startOffset = _transform.localPosition.magnitude;
			_startAngle = ((Vector2)_transform.localPosition).AngleRadFromVector();
		}

		private void Update()
		{
			if(!_weapon.Actor.IsVisible) return;

			var ray = Physics2D.Raycast(RayPos, _transform.up, _length, _mask);
			if (ray)
			{
				float dist = (_length - ray.distance);
				_transform.position = RayPos - (Vector2)transform.up * dist;
				_isDirty = true;
			}
			else if (_isDirty)
			{
				_transform.localPosition = _startOffset * (_startAngle).VectorFromAngle();
				_isDirty = false;
			}
		}

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			if (!_debug || _transform == null) return;

			Vector2 start = _transform.position;
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(start + _offset, (Vector2)_transform.up * _length + start + _offset);
		}
#endif
		private Vector2 RayPos => _weapon.Actor.Position + (_startAngle + _weapon.Actor.Rotation).VectorFromAngle() * _startOffset;
	}
}