using UnityEngine;

using YaEm.AI;
using YaEm.Core;

namespace YaEm.Ability
{
	[CreateAssetMenu(fileName = "Dash ability", menuName = "YaEm/Dash")]
	public sealed class DashBuilder : AbilityBuilder
	{
		[SerializeField] private float _dashLength;
		[SerializeField] private float _dashDuration;
		[SerializeField] private float _dashCooldown;
		[SerializeField, Range(0, 1f)] private float _escapeTolerancy = 1f;
		[SerializeField, Range(0, 1f)] private float _attackTolerancy = 1f;
		[SerializeField, Range(0, 1f)] private float _moveTolerancy = 1f;
		[SerializeField] private AnimationCurve _speedOverTime;

		private void OnValidate()
		{
			_dashLength = Mathf.Max(0f, _dashLength);
			_dashDuration = Mathf.Max(0f, _dashDuration);
		}

		public override IAbility Build(IActor owner)
		{
			DashAbility ability = new DashAbility(_speedOverTime, _dashLength, _dashDuration, _dashCooldown, 
				new DashSubState(new IAIAbilityInstruction[]
				{
					new EscapeDash()
				}));
			ability.Init(owner);
			return ability;
		}
	}
}