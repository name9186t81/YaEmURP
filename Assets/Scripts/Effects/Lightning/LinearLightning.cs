using System.Collections.Generic;

using UnityEngine;

namespace YaEm.Effects
{
	[DisallowMultipleComponent(), RequireComponent(typeof(LineRenderer))]
	public sealed class LinearLightning : MonoBehaviour
	{
		[Header("Points settings")]
		[SerializeField] private int _minPointsPerUnit;
		[SerializeField] private int _maxPointsPerUnit;
		[SerializeField] private float _minDistance;
		[SerializeField] private float _maxDistance;

		[Space]
		[Header("Points spread settings")]
		[SerializeField] private AnimationCurve _distanceSpread;
		[SerializeField] private float _distanceSpreadStrength;
		[SerializeField] private float _deviation;
		[SerializeField] private float _strikesDistance;
		[SerializeField] private bool _useNoise;

		[Space]
		[Header("Lightning settings")]
		[SerializeField] private float _width;
		[SerializeField] private float _widthDeviation;

		private float _minFactor;
		private float _maxFactor;
		private float _minWidth;
		private float _maxWidth;
		private LineRenderer _renderer;

		private void Awake()
		{
			_minFactor = _strikesDistance - _strikesDistance * _deviation;
			_maxFactor = _strikesDistance + _strikesDistance * _deviation;

			_minWidth = _width - _width * _widthDeviation;
			_maxWidth = _width + _width * _widthDeviation;

			_renderer = GetComponent<LineRenderer>();
			_renderer.useWorldSpace = false;
		}

		public void UpdatePattern(in Vector2 endPoint)
		{
			float totalDistance = _maxDistance = endPoint.magnitude;
			Vector2 dir = endPoint / totalDistance;
			Vector2 perp = dir.Perpendicular();

			List<Vector2> points = new List<Vector2>();
			int pointsCount = (int)(totalDistance * Random.Range(_minPointsPerUnit, _maxPointsPerUnit));
			int count = 0;
			float coveredDistance = 0f;

			while(count < pointsCount || coveredDistance < totalDistance)
			{
				//sin2x+sin(x+2)-cos4x
				float delta = (float)count / pointsCount * 7.022f;
				count++;
				coveredDistance += Random.Range(_minDistance, _maxDistance) / pointsCount;

				Vector2 point = coveredDistance * dir + perp * Mathf.Sign(Random.Range(-1f, 1f)) * (_useNoise ? Mathf.Lerp(-1f, 1, Mathf.InverseLerp(-2.082f, 2.37f, Mathf.Sin(2 * delta) + Mathf.Sin(delta + 2) - Mathf.Cos(4 * delta))) * _strikesDistance : Random.Range(_minFactor, _maxFactor));
				points.Add(point);
			}

			_renderer.positionCount = pointsCount;
			AnimationCurve curve = new AnimationCurve();
			Keyframe[] frames = new Keyframe[pointsCount];

			for(int i = 0; i < pointsCount; i++)
			{
				frames[i] = new Keyframe((float)i / (pointsCount - 1), Mathf.Lerp(_width + Random.Range(_minWidth, _maxWidth), _distanceSpread.Evaluate((float)i / (pointsCount - 1)), _distanceSpreadStrength));
				_renderer.SetPosition(i, points[i]);
			}

			curve.keys = frames;
			_renderer.widthCurve = curve;
		}

		public LineRenderer Renderer => _renderer;
	}
}