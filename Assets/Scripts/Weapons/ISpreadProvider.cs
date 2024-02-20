using UnityEngine;

namespace YaEm.Weapons
{
	public interface ISpreadProvider
	{
		Vector2 GetDirection(in Vector2 originalDirection);
	}
}