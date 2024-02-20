using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YaEm.Weapons
{
    public class WeaponOverheating : MonoBehaviour
    {
        [SerializeField] private RangedWeapon _weapon;
        [SerializeField] private float _totalFallTime;
        [SerializeField, Range(0, 1f)] private float _increasePerShot;
        [SerializeField] private float _delayBeforeDecreasing;
		[SerializeField] private bool _blockWeaponUponMaxHeat;
        private float _elapsedDelay;
		private float _storedHeat;
		private float _decreaseDelta;
		private bool _blocked = false;

		public event Action OnOverheat;
		public event Action OnOverheatDropped;

		private void Awake()
		{
			_weapon.OnAttack += Attacked;
			_decreaseDelta = 1 / _totalFallTime;
		}

		private void Attacked()
		{
			_elapsedDelay = _delayBeforeDecreasing;
			_storedHeat += _increasePerShot;
			bool wasBlocked = _blocked;
			_blocked |= _blockWeaponUponMaxHeat && _storedHeat > 1;
			if(!wasBlocked && _blocked)
			{
				OnOverheat?.Invoke();
			}
		}

		private void Update()
		{
			_elapsedDelay -= Time.deltaTime;
			_weapon.CanFire = !_blocked && _storedHeat < 1f;
			if (_elapsedDelay > 0) return;

			_storedHeat -= _decreaseDelta * Time.deltaTime;
			bool wasBlocked = _blocked;
			_blocked &= _storedHeat > 0;
			if(wasBlocked && !_blocked)
			{
				OnOverheatDropped?.Invoke();
			}
			_storedHeat = Mathf.Max(0, _storedHeat);
		}

		public float CurrentOverheat => _storedHeat;
	}
}