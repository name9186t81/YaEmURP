using UnityEngine;

using YaEm;

namespace YaEm.Effects
{
	[RequireComponent(typeof(LineRenderer))]
	public class ShockWave : MonoBehaviour
	{
		[SerializeField] private float _maxWidth;
		[SerializeField] private float _minWidth;
		[SerializeField] private float _radius;
		[SerializeField] private float _growTime;
		[SerializeField] private Gradient _gradient;
		[SerializeField] private AnimationCurve _growCurve;

		[SerializeField] private int _pointsCount;

		[Header("Advanced settings")]
		[SerializeField] private bool _usePhysics;
		[SerializeField] private LayerMask _hitMask;
		[SerializeField] private bool _debug;

		private Vector2[] _directions;
		private bool _isPlaying;
		private float _elapsedTime;
		private LineRenderer _renderer;

		public Gradient Gradient { get => _gradient; }
		private void Start()
		{
			_directions = new Vector2[_pointsCount];

			float delta = (Mathf.PI * 2) / _pointsCount;
			for (int i = 0; i < _pointsCount; i++)
			{
				_directions[i] = (delta * i).VectorFromAngle();
			}

			_renderer = GetComponent<LineRenderer>();
			_renderer.positionCount = 0;
			_renderer.useWorldSpace = true;
		}

		public void Play()
		{
			_isPlaying = true;
			_elapsedTime = 0f;
			_renderer.positionCount = _pointsCount;
		}

		public void Stop()
		{
			_isPlaying = false;
			_renderer.positionCount = 0;
		}

		private void Update()
		{
			if (!_isPlaying) return;

			_elapsedTime += Time.deltaTime;
			float delta = _elapsedTime / _growTime;
			if (delta > 1)
			{
				Stop();
				return;
			}

			float distance = _growCurve.Evaluate(delta) * _radius;
			Vector2 prevPos = _directions[0] * distance;
			Vector2 center = transform.position;

			for (int i = 0; i < _pointsCount; ++i)
			{
				if (_usePhysics)
				{
					var ray = Physics2D.Raycast(center + _directions[i] * 0.1f, _directions[i], distance, _hitMask);
					if (ray)
					{
						_renderer.SetPosition(i, center + _directions[i] * ray.distance);
						continue;
					}

					Debug.DrawLine(center, center + _directions[i] * distance, ray ? Color.green : Color.red, 2f);
				}

				_renderer.SetPosition(i, center + _directions[i] * distance);
			}

			if (_usePhysics)
			{
				var ray = Physics2D.Raycast(center, _directions[0], distance, _hitMask);
				if (ray)
				{
					_renderer.SetPosition(_pointsCount - 1, center + _directions[0] * ray.distance);
				}
				else
				{
					_renderer.SetPosition(_pointsCount - 1, center + _directions[0] * distance);
				}
			}
			else
			{
				_renderer.SetPosition(_pointsCount - 1, center + _directions[0] * distance);
			}

			_renderer.widthMultiplier = Mathf.Lerp(_minWidth, _maxWidth, 1 - delta);
			var color = _gradient.Evaluate(delta);
			_renderer.startColor = _renderer.endColor = color;
		}

		private void OnDrawGizmos()
		{
			if (!_debug) return;

			Gizmos.DrawWireSphere(transform.position, _radius);
		}
	}
}