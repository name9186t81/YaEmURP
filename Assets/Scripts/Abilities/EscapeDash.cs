using UnityEngine;

using YaEm.Ability;
using YaEm.Core;
using YaEm.Health;
using YaEm.Weapons;

namespace YaEm.AI
{
	public sealed class EscapeDash : IAIAbilityInstruction
	{
		private AIController _controller;
		private DashAbility _dash;

		public StateType StateType => StateType.Attacking;

		public void Execute()
		{
			var direction = _controller.CurrentTarget.Position.GetDirectionNormalized(_controller.Position);
			Debug.DrawLine(_controller.Position, _controller.Position + direction * 10, Color.red, 0.2f);
			Debug.DrawLine(_controller.Position, _controller.Position + direction.Perpendicular() * 10, Color.green, 0.2f);
			Debug.DrawLine(_controller.Position, _controller.Position + direction.Perpendicular2() * 10, Color.yellow, 0.2f);
			if (Random.value < 0.5f)
			{
				direction = direction.Perpendicular();
			}
			else
			{
				direction = direction.Perpendicular2();
			}

			_dash.Direction = direction;
			_dash.Use();
		}

		public float GetEffectivness()
		{
			if (_controller.IsTargetNull || _controller.CurrentTarget is not IProvider<IWeapon> weapon || !weapon.Value.CanAttack) return -100f;

			return _controller.Experience + _controller.Aggresivness * (Mathf.Clamp01(0.5f - _controller.Health.Delta()) * 2) + Mathf.Lerp(_controller.Vision.EnemiesInRangeCount, 0, _controller.Braveness) * 0.5f;
		}

		public void Init(AIController controller)
		{
			_controller = controller;
		}

		public void PreExecute()
		{
		}

		public void SetAbility(IAbility ability)
		{
			_dash = ability as DashAbility;
		}

		public void Undo()
		{
		}
	}
}