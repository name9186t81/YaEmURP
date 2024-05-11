using UnityEngine;

using YaEm.Core;

namespace YaEm.Ability
{
	[CreateAssetMenu(menuName = "YaEm/Deflector", fileName = "Deflector")]
	public sealed class DeflectorBuilder : AbilityBuilder
	{
		[SerializeField] private float _aiCheckTime;
		[SerializeField] private float _radiusMultiplayer;
		[SerializeField] private float _cooldown;
		[SerializeField] private LayerMask _projectilesMask;
		[SerializeField] private bool _deflectDeniesCooldown;

		public override IAbility Build(IActor owner)
		{
			var deflector = new Deflector(_radiusMultiplayer, _cooldown, _aiCheckTime, _deflectDeniesCooldown, _projectilesMask);
			deflector.Init(owner);
			return deflector;
		}
	}
}