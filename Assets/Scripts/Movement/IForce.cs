using System;
using UnityEngine;

namespace YaEm.Movement
{
	public enum ForceState
	{
		Alive,
		Destroyed
	}

	public interface IForce
	{
		/// <summary>
		/// Return force, takes worldPosition as a paramater
		/// </summary>
		Func<Vector2, Vector2> ForceFunc { get; }
		ForceState State { get; set; }
	}
}