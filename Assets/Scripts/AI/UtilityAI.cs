using System;

using UnityEngine;

namespace YaEm.AI
{
	public enum StateType
	{
		Unknown,
		Pursing = 1,
		RunningAway  = 2,
		Idling = 3,
		Attacking = 4,
		Initimidating = 5
	}
	public sealed class UtilityAI
	{
		private readonly IUtility[] _utilities;
		private IUtility _currentBest;

		public IUtility CurrentUtility => _currentBest;

		public UtilityAI(AIController controller, IUtility[] utilities)
		{
			_utilities = utilities;

			if (_utilities == null || _utilities.Length == 0)
			{
				throw new ArgumentException();
			}

			for (int i = 0; i < _utilities.Length; i++)
			{
				_utilities[i].Init(controller);
			}
		}

		public void Update()
		{
			IUtility best = UpdateCurrentUtility();

			if (_currentBest != best)
			{
				if(_currentBest != null)
					_currentBest.Undo();

				_currentBest = best;
				_currentBest.PreExecute();
			}

			_currentBest.Execute();
		}

		public void Stop()
		{
			_currentBest.Undo();
			_currentBest = null;
		}

		private IUtility UpdateCurrentUtility()
		{
			IUtility best = null;
			float max = float.MinValue;

			for (int i = 0, length = _utilities.Length; i < length; i++)
			{
				float val = _utilities[i].GetEffectivness();
				//Debug.Log(_utilities[i].GetType().Name + " " + val);
				if (val > max)
				{
					max = val;
					best = _utilities[i];
				}
			}

			return best;
		}
	}
}
