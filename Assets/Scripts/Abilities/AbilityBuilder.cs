using UnityEngine;

using YaEm.Core;

namespace YaEm.Ability
{
	public abstract class AbilityBuilder : ScriptableObject
	{
		public abstract IAbility Build(IActor owner);
	}
}