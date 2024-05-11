using System;
using System.Collections;

using UnityEngine;

namespace YaEm.GUI {
	public class CameraShake : MonoBehaviour
	{
		public enum ShakeType
		{
			Constant,
			Fading
		}

		[Serializable]
		private struct ShakeCurve
		{
			public AnimationCurve Curve;
			public ShakeType Type;
		}

		[SerializeField] private Transform _cameraTransform;
		[SerializeField] private int _maxShakes;
		[SerializeField] private float _maxStrength;
		[SerializeField] private float _maxDuration;
		[SerializeField] private float _maxRadius;
		[SerializeField] private ShakeCurve[] _curves;
		[SerializeField] private AnimationCurve _shakesCount;

		private static event Action<float, ShakeType> OnShake;

		public static void Shake(float strength, ShakeType type) => OnShake?.Invoke(strength, type);

		private void OnEnable()
		{
			OnShake += StartShaking;
		}

		private void OnDisable()
		{
			OnShake -= StartShaking;
		}

		private void StartShaking(float strength, ShakeType type)
		{
			float delta = Mathf.Clamp01(strength / _maxStrength);
			float radius = delta * _maxRadius;
			float duration = delta * _maxDuration;

			int shakes = (int)(_shakesCount.Evaluate(delta) * _maxShakes);

			AnimationCurve curve = _shakesCount;

			for (int i = 0; i < _curves.Length; i++)
			{
				if (_curves[i].Type == type)
				{
					curve = _curves[i].Curve;
					break;
				}
			}

			StartCoroutine(ShakingRoutine(shakes, radius, duration, curve));
		}

		private IEnumerator ShakingRoutine(int shakes, float radius, float duration, AnimationCurve curve)
		{
			float timePerShake = duration / shakes;

			for (int i = 0; i < shakes; ++i)
			{
				Vector2 pos = _cameraTransform.position;
				Vector2 shakePos = pos + (UnityEngine.Random.Range(0, Mathf.PI * 2)).VectorFromAngle() * radius * curve.Evaluate((float)i / _maxShakes);
				_cameraTransform.position = (Vector3)shakePos + Vector3.forward * _cameraTransform.position.z;
				yield return new WaitForSeconds(timePerShake);
			}
		}
	}
}