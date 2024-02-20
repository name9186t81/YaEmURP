using System;
using UnityEngine;
using UnityEngine.UI;

using YaEm.Weapons;
using YaEm.Core;

namespace YaEm
{
	public class MobileController : MonoBehaviour, IController
	{
		[SerializeField] private Joystick _attackJoystick;
		[SerializeField] private Joystick _moveJoystick;
		[SerializeField] private Button _slowmoButton;
		[SerializeField] private float _attackRotationDelay;
		[SerializeField] private Unit _target;
		public Unit Target { get { return _target; }
			set
			{
				if(_target is IProvider<IWeapon> prov)
				{
					prov.Value.OnAttack -= AttackEnded;
				}

				_target = value;
				if(_target is IProvider<IWeapon> prov2)
				{
					prov2.Value.OnAttack += AttackEnded;
				}
			}
		}

		private void AttackEnded()
		{
			_elapsed = _attackRotationDelay;
		}

		private bool _isSlowmoActive = false;
		private float _rotation;
		private float _elapsed;
		private float _prevLength;
		private IWeapon _weapon;

		public ControllerType Type => ControllerType.Player;
		public Vector2 DesiredMoveDirection => _moveJoystick.Direction;
		public float DesiredRotation => _rotation;
		public bool IsEffectedBySlowMotion =>false;

		public event Action<ControllerAction> ControllerAction;

		private void Awake()
		{
			_slowmoButton.onClick.AddListener(() =>
			{
				_isSlowmoActive = !_isSlowmoActive;
				if (_isSlowmoActive) ServiceLocator.Get<GlobalTimeModifier>().SetTimeModificator(0.5f);
				else ServiceLocator.Get<GlobalTimeModifier>().SetTimeModificator(1f);
			});

			if(_target!= null)
			{
				_target.OnInit += Init;
			}
		}

		private void Init()
		{
			if (_target is IProvider<IWeapon> prov2)
			{
				prov2.Value.OnAttack += AttackEnded;
			}

		}

		public void Disable()
		{
			Destroy(_slowmoButton.gameObject);
			Destroy(_moveJoystick.gameObject);
			Destroy(_attackJoystick.gameObject);
			Destroy(this);
		}

		private void Update()
		{
			float length = _attackJoystick.Direction.magnitude;
			bool flag = Target is IProvider<IWeapon> prov && (prov.Value.Flags & WeaponFlags.PreAim) == 0;
			_elapsed -= Time.deltaTime;

			if (length > 0.9f && flag)
			{
				ControllerAction?.Invoke(YaEm.Core.ControllerAction.Fire);
			}
			if (length > 0.1f)
			{
				_rotation = _attackJoystick.Direction.AngleFromVector() - 90;
				ControllerAction?.Invoke(YaEm.Core.ControllerAction.Charge);
			}
			else
			{
				if (_moveJoystick.Direction.magnitude > 0.1f && _elapsed < 0f)
				{
					_rotation = _moveJoystick.Direction.AngleFromVector() - 90;
				}

				if(_prevLength > 0.5f && !flag)
				{
					ControllerAction?.Invoke(YaEm.Core.ControllerAction.BreakCharge);
					ControllerAction?.Invoke(YaEm.Core.ControllerAction.Fire);
				}
			}
			_prevLength = length;
		}
	}
}