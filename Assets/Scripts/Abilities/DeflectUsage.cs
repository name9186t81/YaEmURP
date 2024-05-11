using UnityEngine;

using YaEm.AI;
using YaEm.Core;
using YaEm.Weapons;

namespace YaEm.Ability
{
	public sealed class DeflectUsage : IAIAbilityInstruction
	{
		private readonly float _updateTime;
		private float _elapsed;
		private Deflector _deflector;
		private AIController _controller;
		private bool _hasProjectiles;

		public DeflectUsage(float updateTime)
		{
			_updateTime = updateTime;
		}

		public StateType StateType => StateType.Ability;

		public void Execute()
		{
		}

		public float GetEffectivness()
		{
			_elapsed -= Time.deltaTime;
			if(_elapsed < 0)
			{
				var overlap = Physics2D.OverlapCircle(_controller.Position, _deflector.Radius, _deflector.ProjectilesMask);
				_elapsed = _updateTime;
				_hasProjectiles = overlap && overlap.TryGetComponent<Projectile>(out var projectile) &&
					(projectile.ProjectileFlags & ProjectileFlags.Parriable) != 0 &&
					(_controller.Actor is ITeamProvider prov ? prov.TeamNumber != projectile.TeamNumber : projectile.TeamNumber != 0);
			}

			return _deflector.CanUse() && _hasProjectiles ?
				(_controller.Experience - Random.value) * 4f
				: -100f;
		}

		public void Init(AIController controller)
		{
			_controller = controller;
		}

		public void PreExecute()
		{
			_deflector.Use();
			_hasProjectiles = false;
		}

		public void SetAbility(IAbility ability)
		{
			if(ability is not Deflector def)
			{
				throw new System.ArgumentException($"Ability {ability} is not deflector");
			}
			_deflector = def;
		}

		public void Undo()
		{

		}
	}
}
