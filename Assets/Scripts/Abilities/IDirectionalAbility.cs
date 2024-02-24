using UnityEngine;

namespace YaEm.Ability
{
	public interface IDirectionalAbility : IAbility
	{
		Vector2 Direction { set; }
	}
}