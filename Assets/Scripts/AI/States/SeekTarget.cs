using YaEm;
using UnityEngine;
using YaEm.AI;
using YaEm.Health;

public class SeekTarget : IUtility
{
	private AIController _controller;

	public StateType StateType => StateType.Pursing;

	public void Execute()
	{
		if (!_controller.Memory.TryGetValue(AIMemoryKey.LastTarget, out var target) || target == null) return;

		//if (_controller.Vision.CanSeeTarget((IActor)target))
		//{
		//	_controller.CurrentTarget = (IActor)target;
		//	_controller.TargetTransform = ((MonoBehaviour)target).transform;
		//	return;
		//}

		_controller.Memory.TryGetValue(AIMemoryKey.LastTargetPosition, out object posRaw);
		Vector2 pos = (Vector2)posRaw;

		if (_controller.Position.DistanceLess(pos, _controller.Actor.Scale * 2f))
		{
			_controller.ForceClearLastTarget();
			return;
		}

		_controller.SafeWalk(pos);
		_controller.LookAtPoint(pos);
	}

	public float GetEffectivness()
	{
		return (_controller.CurrentTarget == null && _controller.Memory.TryGetValue(AIMemoryKey.LastTarget, out _)) ? _controller.Memory.TryGetValue(AIMemoryKey.LastTargetHealth, out var health) ? Mathf.Lerp(_controller.Aggresivness, _controller.Aggresivness / 2, ((IHealth)health).Delta()) : _controller.Aggresivness : -1f;
	}

	public void Init(AIController controller)
	{
		_controller = controller;
	}

	public void PreExecute()
	{
		_controller.StopMoving();
	}

	public void Undo()
	{

	}
}
