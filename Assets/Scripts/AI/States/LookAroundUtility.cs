using YaEm;

using UnityEngine;
using Random = UnityEngine.Random;

namespace YaEm.AI.States
{
	public class LookAroundUtility : IUtility
	{
		private readonly float _minLookingTime= 1f;
		private readonly float _maxLookingTime = 5f;
		private float _time;
		private Vector2 _randomPoint;
		private AIController _controller;

		public StateType StateType => StateType.Idling;

		public void Execute()
		{
			_time -= Time.deltaTime;
			_controller.LookAtPoint(_randomPoint);

			if (_time < 0)
			{
				_time = Random.Range(_minLookingTime, _maxLookingTime);
				_randomPoint = Vector2Extensions.RandomDirection() + _controller.Position;
			}
		}

		public float GetEffectivness()
		{
			return 0.8f;
		}

		public void Init(AIController controller)
		{
			_controller = controller;
		}

		public void PreExecute()
		{
			_time = Random.Range(_minLookingTime, _maxLookingTime);
			_randomPoint = Vector2Extensions.RandomDirection() + _controller.Position;
			_controller.StopMoving();
		}

		public void Undo()
		{

		}
	}
}
