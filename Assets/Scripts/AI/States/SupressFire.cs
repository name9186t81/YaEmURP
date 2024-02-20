using YaEm.Core;
using YaEm.Health;

using UnityEngine;

namespace YaEm.AI.States
{
	public class SupressFire : IUtility
	{
		private AIController _controller;
		private Vector2 _realPosition;

		public StateType StateType => StateType.Attacking;

		public void Execute()
		{
			if (!_controller.Memory.TryGetValue(AIMemoryKey.LastTarget, out var target) || target == null) return;

			//if (_controller.Vision.CanSeeTarget((IActor)target))
			//{
			//	_controller.CurrentTarget = (IActor)target;
			//	_controller.TargetTransform = ((MonoBehaviour)target).transform;
			//	return;
			//}

			_controller.Memory.TryGetValue(AIMemoryKey.LastTargetPosition, out object posRaw);
			Vector2 pos = (Vector2)posRaw;

			if(_controller.Weapon != null && (_controller.Weapon.Flags & YaEm.Weapons.WeaponFlags.PreAim) == 0)
			_controller.InitCommand(ControllerAction.Fire);
			_controller.LookAtPoint(_realPosition);
		}

		public float GetEffectivness()
		{
			return (_controller.CurrentTarget == null && _controller.Memory.TryGetValue(AIMemoryKey.LastTarget, out _)) ? _controller.Memory.TryGetValue(AIMemoryKey.LastTargetHealth, out var health) ? Mathf.Lerp((1 - _controller.Aggresivness), 0f, 1 - ((IHealth)health).Delta()) : (1 - _controller.Aggresivness) : -1f;
		}

		public void Init(AIController controller)
		{
			_controller = controller;
		}

		public void PreExecute()
		{
			_controller.StopMoving();
			if (_controller.Memory.TryGetValue(AIMemoryKey.LastTarget, out object target) && target != null 
				&& _controller.Memory.TryGetValue(AIMemoryKey.LastTargetPosition, out object position))
			{
				Transform targetTransf = (target as MonoBehaviour).transform;
				_realPosition = targetTransf.position.GetDirectionNormalized((Vector2)position) * (target as IActor).Scale * 2 + (Vector2)position;
			}
			else
			{
				if(_controller.Memory.TryGetValue(AIMemoryKey.LastTargetPosition, out object position2))
				{
					_realPosition = (Vector2)position2;
				}
			}
		}

		public void Undo()
		{

		}
	}
}
