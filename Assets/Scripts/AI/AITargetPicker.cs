using UnityEngine;
using YaEm.Core;
using YaEm.Health;

namespace YaEm.AI
{
	public sealed class AITargetPicker : MonoBehaviour
	{
		private AIVision _vision;
		private AIController _controller;
		[SerializeField] private TargetPickMode _targetPickMode;

		public void Init(AIController controller)
		{
			_controller = controller;
			_vision = _controller.Vision;

			_vision.OnScan += OnScan;
		}

		private void OnScan()
		{
			if (_controller.CurrentTarget != null)
			{
				if (_controller.TargetTransform == null)
				{
					_controller.ForgotTarget(false);
					return;
				}
				if (!_vision.CanSeeTarget(_controller.CurrentTarget, false))
				{
					_controller.ForgotTarget(true);
				}
				return;
			}

			if (_vision.EnemiesInRangeCount == 0) return;

			var targets = _vision.EnemiesInRange;
			IActor choosen = null;

			switch (_targetPickMode)
			{
				case TargetPickMode.Closest:
					{
						float closest = float.MaxValue;
						Vector2 selfPos = _controller.Position;

						for (int i = 0, length = targets.Count; i < length; i++)
						{
							Vector2 targetPos = targets[i].Position;
							//todo: move relative distance into vector2 extensions
							float relDist = Mathf.Abs(selfPos.x - targetPos.x) + Mathf.Abs(selfPos.y - targetPos.y);

							if (relDist < closest)
							{
								closest = relDist;
								choosen = targets[i];
							}
						}
						break;
					}
				case TargetPickMode.HPWeakest:
					{
						float minDelta = 2f;

						for (int i = 0, length = targets.Count; i < length; i++)
						{
							if (targets[i] is IProvider<IHealth> prov && prov.Value != null
								&& prov.Value.Delta() < minDelta)
							{
								choosen = targets[i];
								minDelta = prov.Value.Delta();
							}
						}
						break;
					}
			}

			if(choosen != null ) _controller.UpdateTarget(choosen);
		}
	}

	public enum TargetPickMode
	{
		Closest,
		HPWeakest
	}
}
