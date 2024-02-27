using UnityEngine;

using YaEm.Ability;
using YaEm.Core;

namespace YaEm.Effects
{
	public sealed class ParticlesOnAbilityUse : MonoBehaviour, IActorComponent
	{
		private IActor _actor;
		[SerializeField] private ParticleSystem _particles;
		public IActor Actor { set => _actor = value; }

		public void Init(IActor actor)
		{
			_actor = actor;

			if(_actor is IProvider<IAbility> prov && prov.Value != null)
			{
				prov.Value.OnActivate += Activate;
				prov.Value.OnDeactivate += Deactivate;
			}
		}

		private void Deactivate()
		{
			_particles.Stop();
		}

		private void Activate()
		{
			_particles.Play();
		}
	}
}