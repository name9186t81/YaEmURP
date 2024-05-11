using System;

using UnityEngine;

using YaEm.Core;

namespace YaEm.Effects
{
	public sealed class RapidTeamChanger : MonoBehaviour, IActorComponent
	{
		[SerializeField] private float _time;
		private float _elapsed;
		private int _prevTeam; 
		private IActor _actor;
		private ITeamProvider _team;
		public IActor Actor { set => _actor = value; }

		public void Init(IActor actor)
		{
			_actor = actor;

			if(_actor is ITeamProvider prov)
			{
				_team = prov;
			}
		}

		private void Update()
		{
			_elapsed += Time.deltaTime;
			int current = (int)((_elapsed / _time) * 6);

			if(current != _prevTeam)
			{
				_team.TryChangeTeamNumber(current);
				_prevTeam = current;
			}

			if(_elapsed > _time)
			{
				_elapsed = 0;
			}
		}
	}
}