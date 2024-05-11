using Global;

using System;

using UnityEngine;

using YaEm.AI;
using YaEm.Weapons;

namespace YaEm.Core
{
	public sealed class CustomPlayerSpawner : MonoBehaviour, ITeamProvider
	{
		private bool _isInited;
		private Unit _unit;

		public int TeamNumber => _unit.TeamNumber;

		public event Action<int, int> OnTeamNumberChange;

		public bool TryChangeTeamNumber(int newTeamNumber)
		{
			if (!_isInited) Init(newTeamNumber);
			return true;
		}

		private void Awake()
		{
			if (!_isInited) Init();
		}

		private void Init(int team = 1)
		{
			_isInited = true;
			var container = ServiceLocator.Get<PlayerChararcterContainer>();

			var body = Instantiate(container.Body.Unit, transform.position, Quaternion.identity, transform);

			var weapon = Instantiate(container.Weapon.Weapon, transform.position, Quaternion.identity, body.transform);
			if (weapon.TryGetComponent<ActorGraphics>(out var gr1) && body.TryGetComponent<ActorGraphics>(out var gr2))
			{
				gr2.Merge(gr1);
			}

			var ability = Instantiate(container.Ability.Visual, transform.position, Quaternion.identity, body.transform);
			ability.Offset *= body.Scale;
			ability.transform.position = ability.GetPosition() + (Vector2)body.transform.position;
			if (ability.TryGetComponent<ActorGraphics>(out var gr3) && body.TryGetComponent<ActorGraphics>(out var gr4))
			{
				gr4.Merge(gr3);
			}
			body.Init();
			body.SetAbility(container.Ability.Builder.Build(body));

			body.TryChangeTeamNumber(team);
			body.InitComponents();

			if(body.TryGetComponent<AIController>(out var comp))
			{
				comp.Disable();
			}
			body.Value.OnDeath += Death;
		}

		private void Death(Health.DamageArgs obj)
		{
			Destroy(gameObject);
		}
	}
}