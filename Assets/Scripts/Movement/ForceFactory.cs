using System;
using UnityEngine;

namespace YaEm.Movement
{
	public static class ForceFactory
	{
		public static ParameterizedForce GetParameterizedForce(Func<Vector2, Vector2> forceFunc) =>
			new ParameterizedForce().SetForce(forceFunc);

		public static ParameterizedForce GetParameterizedForce() =>
			new ParameterizedForce();
	}
}