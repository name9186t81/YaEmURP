using UnityEngine;

namespace YaEm.Movement
{
	public interface ITransformProvider
	{
		Vector2 Velocity { get; set; }
	}
}