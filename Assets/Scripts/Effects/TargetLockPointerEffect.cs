using YaEm;

using UnityEngine;

using YaEm.Weapons;

public class TargetLockPointerEffect : MonoBehaviour
{
	[SerializeField] private float _range;
	[SerializeField] private float _angle;
	[SerializeField] private float _playTime;
	[SerializeField] private LineRenderer _renderer1;
	[SerializeField] private LineRenderer _renderer2;
	[SerializeField] private LayerMask _mask;
	[SerializeField] private RangedWeapon _weapon;
	private float _elapsed;
	private float _halfAngle;
	private bool _isPlaying;

	private void Awake()
	{
		_halfAngle = _angle / 2;
		_weapon.OnPreFireStart += Play;
		_weapon.OnPreFireEnd += Stop;
	}

	private void Stop()
	{
		_isPlaying = false;
		_elapsed = 0f;
	}

	private void Play()
	{
		_isPlaying = true;
	}

	private void Update()
	{
		_renderer1.SetPosition(0, _weapon.GlobalShootPoint);
		_renderer2.SetPosition(0, _weapon.GlobalShootPoint);
		if (!_isPlaying && _weapon.ChargeTime < 0.1f)
		{
			_renderer1.SetPosition(1, _weapon.GlobalShootPoint);
			_renderer2.SetPosition(1, _weapon.GlobalShootPoint);
			return;
		}

		_elapsed += Time.deltaTime;
		float dirAngle = _weapon.ShootDirection.AngleFromVector();
		float lerpedAngle = Mathf.Lerp(_halfAngle, 0, _elapsed / _playTime);
		float angle1 = (dirAngle + lerpedAngle) * Mathf.Deg2Rad;
		float angle2 = (dirAngle - lerpedAngle) * Mathf.Deg2Rad;

		Vector2 dir1 = angle1.VectorFromAngle();
		Vector2 dir2 = angle2.VectorFromAngle();

		var ray = Physics2D.Raycast(_weapon.GlobalShootPoint, dir1, _range, _mask);
		if (ray)
		{
			_renderer1.SetPosition(1, ray.point);
		}
		else
		{
			_renderer1.SetPosition(1, dir1 * _range + _weapon.GlobalShootPoint);
		}

		ray = Physics2D.Raycast(_weapon.GlobalShootPoint, dir2, _range, _mask);
		if (ray)
		{
			_renderer2.SetPosition(1, ray.point);
		}
		else
		{
			_renderer2.SetPosition(1, dir2 * _range + _weapon.GlobalShootPoint);
		}
	}
}
