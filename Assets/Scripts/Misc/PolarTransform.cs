using UnityEngine;

namespace YaEm
{
	[RequireComponent(typeof(Transform)), DisallowMultipleComponent()]
	public sealed class PolarTransform : MonoBehaviour
	{
		[SerializeField] private float _angle;
		[SerializeField] private float _offset;
		[SerializeField] private bool _debug;
		private Transform _transform;

		private void Awake()
		{
			_transform = transform;
		}

		public void SetPosition(in Vector2 position)
		{
			Vector2 dir = _transform.position.GetDirection(position);
			_angle = dir.AngleRadFromVector();
			_offset = dir.magnitude;
		}

		public Vector2 GetPosition()
		{
			return _angle.VectorFromAngle() * _offset;
		}

		private void OnDrawGizmos()
		{
			if (!_debug) return;

			Vector2 origin = transform.position;
			Vector2 pos = GetPosition() + origin;
			Gizmos.DrawLine(origin, pos);
			Gizmos.DrawWireSphere(pos, 0.1f);
		}

		public float Offset
		{
			get => _offset;
			set => _offset = Mathf.Max(0, value);
		}

		public float Angle
		{
			get => _angle;
			set => _angle = value;
		}
	}
}
