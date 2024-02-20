using UnityEngine;

using YaEm.Weapons;

namespace YaEm.Effects
{
	public class MuzzleFlash : MonoBehaviour
	{
		[SerializeField] private RangedWeapon _weapon;
		[SerializeField] private Transform _flash;
		[SerializeField] private float _maxScale;
		[SerializeField] private float _duration;
		[SerializeField] private AnimationCurve _growCurve;
		private float _elapsed;
		private bool _isActive;

		private void Awake()
		{
			_weapon.OnAttack += OnAttack;
		}

		private void OnAttack()
		{
			_isActive = true;
			_elapsed = 0f;
		}

		private void Update()
		{
			if (!_isActive) return;

			_elapsed += Time.deltaTime;
			float delta = _elapsed / _duration;
			if(delta > 1)
			{
				_flash.localScale = Vector3.zero;
				_isActive = false;
				return;
			}

			_flash.localScale = _growCurve.Evaluate(delta) * _maxScale * Vector3.one;
		}
	}
}