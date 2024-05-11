using UnityEngine;

using YaEm.Ability;
using YaEm.Core;

namespace YaEm.Effects
{
	public sealed class AbilityReadinessDisplayer : MonoBehaviour, IActorComponent
	{
		[SerializeField] private SpriteRenderer _renderer;
		private IActor _actor;
		private IAbility _ability;
		public IActor Actor { set => _actor = value; }

		public void Init(IActor actor)
		{
			if(actor is IProvider<IAbility> prov && prov.Value != null)
			{
				_ability = prov.Value;
			}
			else
			{
				Debug.LogWarning("Actor " + actor.Name + " does not have ability!");
				Destroy(this);
			}
			_actor = actor;
		}

		private void Update()
		{
			if (_actor == null) return;
			_renderer.color = _renderer.color.WhiteOut(1 - _ability.Readiness);
		}
	}
}