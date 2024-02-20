using YaEm.Core;
using UnityEngine;

namespace YaEm.Weapons
{
	public abstract class ProjectileBuilder : ScriptableObject
	{
		public abstract IProjectile PeekProjectile();
		public abstract IProjectile BuildProjectile(IActor owner);
	}
}
