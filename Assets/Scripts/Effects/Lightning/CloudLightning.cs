using UnityEngine;

namespace YaEm.Effects
{
	[DisallowMultipleComponent(), RequireComponent(typeof(CircleLightning))]
	public sealed class CloudLightning : MonoBehaviour
	{
		[SerializeField] private float _minAppearTime;
		[SerializeField] private float _maxAppearTime;
		[SerializeField] private float _minChillTime;
		[SerializeField] private float _maxChillTime;
		[SerializeField] private AnimationCurve _growthSpeed;
		[SerializeField] private Material _lightningMaterial;
		private CircleLightning _lightning;
		private LineRenderer _renderer;
		private int _factorIndex;
		private float _chillTime;
		private float _appearTime;
		private float _elapsed;
		private float _area;
		private bool _chilling;

		private void Awake()
		{
			_renderer = GetComponent<LineRenderer>();
			var gradient = new Gradient();
			gradient.alphaKeys = new GradientAlphaKey[2] {
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 1f),
			};

			_renderer.colorGradient = gradient;
			_lightning = GetComponent<CircleLightning>();
			_appearTime = AppearTime;

			_renderer.material = new Material(_lightningMaterial); //rest in peace batching
			_factorIndex = _renderer.material.shader.FindPropertyIndex("_Factor");
			_area = _growthSpeed.Area();
		}

		private void Update()
		{
			if (_chilling)
			{
				_elapsed += Time.deltaTime;
				if (_elapsed > _chillTime)
				{
					_elapsed = 0f;
					_chilling = false;
					_appearTime = AppearTime;
					_renderer.widthMultiplier = 1f;
					_lightning.UpdatePattern();
				}

				float delta2 = _elapsed / _chillTime;
				_renderer.widthMultiplier = 1 - delta2;
				return;
			}

			_elapsed += Time.deltaTime;
			float delta = _elapsed / _appearTime;
			_renderer.material.SetFloat("_Factor", delta * _growthSpeed.Evaluate(delta) / _area);
			if (delta > 1f)
			{
				_chilling = true;
				_chillTime = ChillTime;
				_elapsed = 0f;
			}
		}
		private void OnValidate()
		{
			_minAppearTime = Mathf.Min(_minAppearTime, _maxAppearTime);
			_minChillTime = Mathf.Min(_minChillTime, _maxChillTime);

			_maxAppearTime = Mathf.Max(0, _maxAppearTime);
			_maxChillTime = Mathf.Max(0, _maxChillTime);
		}

		private float ChillTime => Random.Range(_minChillTime, _maxChillTime);
		private float AppearTime => Random.Range(_minAppearTime, _maxAppearTime);
	}
}