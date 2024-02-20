using UnityEngine;

using YaEm.Weapons;

public class WeaponSwingAnimation : MonoBehaviour
{
	[SerializeField] private float _startAngle;
	[SerializeField] private float _endAngle;
	[SerializeField] private float _time;
	private float _delta;
	private IWeapon _weapon;
	private bool _started = false;
	private float _elapsed;

	private void Awake()
	{
		if (!TryGetComponent<IWeapon>(out _weapon))
		{
			Debug.LogError("Cannot find Weapon component");
			return;
		}

		_delta = (_endAngle - _startAngle) / _time * 2;
		_elapsed = _time;
		_weapon.OnAttack += StartAnimation;
	}

	private void StartAnimation()
	{
		_started = true;
	}

	private void Update()
	{
		if (!_started) return;

		_elapsed -= Time.deltaTime;
		if (_elapsed > _time / 2)
			transform.localRotation = Quaternion.Euler(0, 0, transform.localRotation.eulerAngles.z + _delta * Time.deltaTime);
		else
			transform.localRotation = Quaternion.Euler(0, 0, transform.localRotation.eulerAngles.z - _delta * Time.deltaTime);

		if (_elapsed < 0)
		{
			_started = false;
			_elapsed = _time;
			transform.localRotation = Quaternion.Euler(0, 0, _startAngle);
		}
	}
}
