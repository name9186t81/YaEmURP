using UnityEngine;

namespace YaEm.Effects
{
	[DisallowMultipleComponent(), RequireComponent(typeof(LineRenderer))]
	public sealed class CircleLightning : MonoBehaviour
	{
		[SerializeField] private float _raduis;
		[SerializeField] private float _minRadius;
		[SerializeField] private int _minPoints;
		[SerializeField] private int _maxPoints;
		[SerializeField] private float _minAngle;
		[SerializeField] private int _maxAngle;
		[SerializeField] private float _width;
		[SerializeField] private float _widthDeviation;
		[SerializeField, Range(0, 1f)] private float _centerTolerancy;
		[SerializeField] private bool _activeFromStart;
		[SerializeField] private bool _worldPosition;
		[SerializeField] private bool _debug;
		private LineRenderer _renderer;

		private void Awake()
		{
			_renderer = GetComponent<LineRenderer>();
			if (_activeFromStart)
				UpdatePattern();
			else
				_renderer.positionCount = 0;

			_renderer.useWorldSpace = _worldPosition;
		}

		private void OnValidate()
		{
			_widthDeviation = Mathf.Max(_widthDeviation, 0);
			_maxPoints = Mathf.Max(_minPoints, _maxPoints);
			_maxPoints = Mathf.Max(_maxPoints, 0);
		}

		public void UpdatePattern()
		{
			int points = Random.Range(_minPoints, _maxPoints);
			_renderer.positionCount = points;
			Keyframe[] keys = new Keyframe[points];
			float startAngle = Random.Range(0, 360);

			for(int i = 0; i < points; i++)
			{
				float delta = (float)i / (points - 1);
				float width = Random.Range(Mathf.Max(0, _width - _width * _widthDeviation), _width + _width * _widthDeviation);
				keys[i] = new Keyframe(delta, width);

				float deltaAngle = Random.Range(_minAngle, _maxAngle);
				float angle = startAngle + Mathf.Lerp(-deltaAngle, deltaAngle, Random.value);

				Vector2 point = (angle * Mathf.Deg2Rad).VectorFromAngle() * Mathf.Lerp(_raduis, Random.Range(_minRadius, _raduis), _centerTolerancy);
				if(_worldPosition)
					_renderer.SetPosition(i, point + (Vector2)transform.position);
				else
					_renderer.SetPosition(i, point);
			}

			AnimationCurve curve = new AnimationCurve(keys);
			_renderer.widthCurve = curve;
		}

		private void OnDrawGizmos()
		{
			if(!_debug) return;

			var pos = transform.position;

			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(pos, _minRadius);

			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(pos, _raduis);
		}
	}
}