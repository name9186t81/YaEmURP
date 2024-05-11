using UnityEngine;

namespace YaEm.Effects
{
	[DisallowMultipleComponent()]
	public sealed class LightningCharge : MonoBehaviour
	{
		private class Lightning
		{
			public float Time;
			public LinearLightning Linear;

			public Lightning(float time, LinearLightning linear)
			{
				Time = time;
				Linear = linear;
			}
		}

		[SerializeField] private float _chargeTime;
		[SerializeField] private int _maxLightnings;
		[SerializeField] private AnimationCurve _lightningsCount;
		[SerializeField] private LinearLightning _lightningPrefab;
		[SerializeField] private Material _lightningMaterial;

		[SerializeField] private float _minRadius;
		[SerializeField] private float _maxRadius;
		[SerializeField] private float _lightningTime;

		private int _count;
		private float _elapsed;
		private bool _isActive;
		private Lightning[] _lightnings;

		private void Awake()
		{
			_lightnings = new Lightning[_maxLightnings];

			for(int i = 0; i < _maxLightnings ; i++)
			{
				_lightnings[i] = new Lightning(0f, Instantiate(_lightningPrefab, transform.position, Quaternion.identity, transform));
				_lightnings[i].Linear.Renderer.material = new Material(_lightningMaterial);
			}

			_isActive = true;
		}

		private void Update()
		{
			if (!_isActive) return;

			_elapsed += Time.deltaTime;
			float delta = Mathf.Clamp01(_elapsed / _chargeTime);

			_count = (int)(_lightningsCount.Evaluate(delta) * _maxLightnings);
			float radius = Mathf.Lerp(_minRadius, _maxRadius, delta) * 2f;
			float deltaTime = Time.deltaTime;

			for(int i = 0; i < _count; ++i)
			{
				var lightning = _lightnings[i];
				lightning.Time -= deltaTime;

				if (lightning.Time < 0f && _elapsed < _chargeTime)
				{
					lightning.Time = _lightningTime;
					lightning.Linear.UpdatePattern(Vector2Extensions.RandomDirection() * radius);
				}

				float localDelta = Mathf.Clamp01(lightning.Time / _lightningTime);

				lightning.Linear.Renderer.widthMultiplier = localDelta;
				lightning.Linear.Renderer.material.SetFloat("_Factor", localDelta);
			}

			if(_elapsed > _chargeTime + _lightningTime * 1.2f)
			{
				Deactivate();
			}
		}

		public void Activate()
		{
			_isActive = true;
			_count = 0;
			_elapsed = 0f;
		}

		public void Deactivate()
		{
			_isActive = false;
			
			for(int i = 0; i < _count; i++)
			{
				_lightnings[i].Linear.Renderer.enabled = false;
			}

			_count = 0;
			_elapsed = 0f;
		}
	}
}