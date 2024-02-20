using UnityEngine;

namespace YaEm.Movement
{
	public class RigidbodyVelocityProvider : ITransformProvider
	{
		private readonly Rigidbody2D _rigidbody;
		public RigidbodyVelocityProvider(Rigidbody2D body)
		{
			_rigidbody = body;
		}
		public Vector2 Velocity { get => _rigidbody.velocity; set => _rigidbody.velocity = value; }
	}
}