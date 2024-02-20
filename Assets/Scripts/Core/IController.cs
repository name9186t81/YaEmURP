using System;
using UnityEngine;

namespace YaEm.Core
{
	public interface IController
	{
		ControllerType Type { get; }
		Vector2 DesiredMoveDirection { get; }
		/// <summary>
		/// Angle in degrees to which controlled actor should be rotated.
		/// </summary>
		float DesiredRotation { get; }
		event Action<ControllerAction> ControllerAction;
		bool IsEffectedBySlowMotion { get; }
	}

	public enum ControllerType
	{
		AI,
		Player
	}
}