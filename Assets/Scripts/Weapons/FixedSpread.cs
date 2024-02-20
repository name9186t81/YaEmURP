using UnityEngine;

namespace YaEm.Weapons
{
	public class FixedSpread : ISpreadProvider
	{
		public Vector2 GetDirection(in Vector2 originalDirection)
		{
			return originalDirection;
		}
	}
}