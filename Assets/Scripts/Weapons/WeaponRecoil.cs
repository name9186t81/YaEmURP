using UnityEngine;

using YaEm.Weapons;

public class WeaponRecoil : MonoBehaviour
{
	[SerializeField] private float _maxAngleVelocity;
	[SerializeField] private float _maxLinearVelocity;
	[SerializeField] private float _returnTime;
	private IWeapon _weapon;
	private float _velocity;
	private float _linearVelocity;
	private float _originalAngle;
	private float _elapsed;
	private bool _active = false;
	private float _delta;
	private float _linearDelta;
	private Vector2 _originalPosition;

	private void Awake()
	{
		if (!TryGetComponent<IWeapon>(out _weapon))
		{
			Debug.LogError("Cannot find weapon");
			return;
		}

		_weapon.OnInit += Init;
	}

	private void Init()
	{
		_maxAngleVelocity *= Mathf.Deg2Rad;
		_originalAngle = transform.eulerAngles.z;
		_originalPosition = transform.localPosition;
		_weapon.OnAttack += Recoil;
	}

	private void Update()
	{
		if (!_active) return;

		if (_elapsed > _returnTime / 2)
		{
			_velocity += _delta;
			_linearVelocity += _linearDelta;
		}
		else
		{
			_velocity -= _delta;
			_linearVelocity -= _linearDelta * 2;
			if (_elapsed < 0)
			{
				_active = false;
				_velocity = _linearVelocity = 0f;
				transform.localPosition = _originalPosition;
				transform.eulerAngles = Vector3.forward * _originalAngle;
			}
		}

		_elapsed -= Time.deltaTime;
		transform.localPosition = _originalPosition + Vector2.up * _linearVelocity * Time.deltaTime;
		transform.localEulerAngles = Vector3.forward * (_velocity + _originalAngle);
	}

	private void Recoil()
	{
		_delta = Random.Range(-_maxAngleVelocity, _maxAngleVelocity) / _returnTime;
		_linearDelta = Random.Range(0f, -_maxLinearVelocity) / _returnTime;
		_active = true;
		_elapsed = _returnTime;
	}
}
