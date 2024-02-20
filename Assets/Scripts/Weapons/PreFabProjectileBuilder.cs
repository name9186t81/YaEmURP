using YaEm.Core;

using System;

using UnityEngine;

namespace YaEm.Weapons
{
	[CreateAssetMenu(fileName = "Projectile Builder", menuName = "YaEm/Projectile Builder", order = 1)]
	public sealed class PreFabProjectileBuilder : ProjectileBuilder
	{
		[SerializeField] private GameObject _projectile;

		public override IProjectile BuildProjectile(IActor owner)
		{
			GameObject instance = MonoBehaviour.Instantiate(_projectile);
			if (!instance.TryGetComponent<IProjectile>(out var projectile))
			{
				throw new ArgumentException();
			}
			return projectile;
		}

		public override IProjectile PeekProjectile()
		{
			return _projectile.GetComponent<IProjectile>();
		}
	}
}