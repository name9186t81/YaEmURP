using UnityEngine;

using YaEm.Ability;
using YaEm.Core;

namespace YaEm.Effects
{
	public sealed class SpriteTrailOnAbilityUse : MonoBehaviour, IActorComponent
	{
		[SerializeField] private SpriteTrail _trail;
		private IActor _actor;

		public IActor Actor { set => _actor = value; }

		public void Init(IActor actor)
		{
			_actor = actor;
			if(_actor is IProvider<IAbility> prov && prov.Value != null)
			{
				prov.Value.OnActivate += Activated;
				prov.Value.OnDeactivate += Deactivated;
			}
			else
			{
				Debug.LogWarning("Actor does not have ability");
			}
		}

		private void Deactivated()
		{
			_trail.Deactivate();
		}

		private void Activated()
		{
			_trail.Activate();
		}
	}
}