using System;
using UnityEngine;

namespace YaEm.Core
{
	public interface IActor
	{
		Vector2 Position { get; }
		/// <summary>
		/// Measured in radians.
		/// </summary>
		float Rotation { get; }
		float Scale { get; }
		IController Controller { get; }
		string Name { get; }
		event Action OnInit;

		//should just use Controller.OnAction but its used here for simplification reasons.
		event Action<ControllerAction> OnAction;
		Vector2 DesiredMoveDirection { get => Controller != null ? Controller.DesiredMoveDirection : Vector2.zero; }
		float DesiredRotation { get => Controller != null ? Controller.DesiredRotation : 0f; }
		/// <summary>
		/// First argument is old controller, second is new.
		/// </summary>
		event Action<IController, IController> OnControllerChange;
		bool TryChangeController(in IController controller);
		bool IsVisible { get; }
	}
}