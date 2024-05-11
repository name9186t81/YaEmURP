using UnityEngine;

using YaEm.Core;

namespace YaEm.Ability
{
	[CreateAssetMenu(fileName = "Shield Ability", menuName = "YaEm/Shield")]
	public sealed class ShieldBuilder : AbilityBuilder
	{
		[SerializeField] private int _shieldHealth;
		[SerializeField] private float _shieldRegenerationTime;

		[SerializeField] private float _damageCooldown;
		[SerializeField] private float _deathCooldown;
		[SerializeField] private float _regenerationTick;
		[SerializeField] private bool _canPassivlyRegenerate;

		public override IAbility Build(IActor owner)
		{
			var shield = new ShieldAbility(_shieldHealth, _deathCooldown, _damageCooldown, _shieldRegenerationTime, _regenerationTick, _canPassivlyRegenerate);
			shield.Init(owner);
			return shield;
		}

		private void OnValidate()
		{
			_shieldHealth = Mathf.Max(_shieldHealth, 0);
			_shieldRegenerationTime = Mathf.Max(_shieldRegenerationTime, 0);
			_damageCooldown = Mathf.Max(_damageCooldown, 0);
			_deathCooldown = Mathf.Max(_deathCooldown, 0);
			_regenerationTick = Mathf.Max(_regenerationTick, 0);
		}
	}
}