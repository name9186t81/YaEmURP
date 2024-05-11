using UnityEngine;

public sealed class Pulse : MonoBehaviour
{
	[SerializeField] private float _growTime;
	[SerializeField] private float _maxScale;
	[SerializeField] private float _minScale;
	[SerializeField] private float _chillTime;
	[SerializeField] private SpriteRenderer _renderer;
	[SerializeField] private AnimationCurve _alphaFalloff;
	private float _elapsed;
	private bool _active;

	private Transform _cached;

	private void Start()
	{
		_cached = transform;
		_active = true;
	}

	private void Update()
	{
		_elapsed += Time.deltaTime;	

		if (_active)
		{
			float delta = _elapsed / _growTime;
			if (delta > 1)
			{
				DeActivate();
			}

			_cached.localScale = Vector3.one * Mathf.Lerp(_minScale, _maxScale, delta);
			if(_renderer != null)
			{
				var color = _renderer.color;
				_renderer.color = new Color(color.r, color.g, color.b, _alphaFalloff.Evaluate(delta));
			}
		}
		else
		{
			float delta = _elapsed / _chillTime; 
			_cached.localScale = Vector3.one * Mathf.Lerp(_maxScale, _minScale, delta);
			if (delta > 1)
			{
				_active = true;
				_elapsed = 0f;
			}
		}
	}

	public void DeActivate()
	{
		_active = false;
		_elapsed = 0f;
		_cached.localScale = Vector3.zero + Vector3.forward;
	}
}
