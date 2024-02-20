using System;
using UnityEngine;

namespace YaEm.Movement
{
	public sealed class BaseForce : IForce
    {
        private readonly Func<Vector2, Vector2> _force;
		private ForceState _state;

		public BaseForce(Func<Vector2, Vector2> force)
		{
			_force = force;
		}

		public Func<Vector2, Vector2> ForceFunc => _force;

		ForceState IForce.State { get => _state; set => _state = value; }
	}
}