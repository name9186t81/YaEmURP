using UnityEngine;

using YaEm.Ability;
using YaEm.Core;

namespace YaEm.Effects
{
	public sealed class HealOtherVFX : MonoBehaviour, IActorComponent
	{
		[SerializeField] private LineRenderer _renderer;
		[SerializeField] private ParticleSystem _particles;
		private IActor _actor;
		private IActor _target;
		private Transform _cached;
		private HealOtherAbility _ability;

		public IActor Actor { set => _actor = value; }

		public void Init(IActor actor)
		{
			if(actor is not IProvider<IAbility> prov || prov.Value is not HealOtherAbility heal)
			{
				Debug.LogError("Actor does not have heal other ability");
				return;
			}

			_cached = transform;
			_actor = actor;
			_ability = heal;
			_ability.OnTick += Tick;
		}

		private void Update()
		{
			if (_target == null) return;

			if (_target.Equals(null))
			{
				_particles.Stop();
				_renderer.positionCount = 0;
				_target = null;
				return;
			}

			Vector2 targetPos = _target.Position;
			_renderer.SetPosition(0, SelfPos);
			_renderer.SetPosition(1, targetPos);

			float angle = SelfPos.GetDirection(targetPos).AngleFromVector();
			_particles.transform.rotation = Quaternion.Euler(Vector3.forward * (angle - 90));
			_particles.startLifetime = (SelfPos.GetDirection(targetPos).sqrMagnitude / (_particles.startSpeed * _particles.startSpeed)) * 2;
		}
		private void Tick()
		{
			if(_ability.Target == null)
			{
				_particles.Stop();
				_renderer.positionCount = 0;
				_target = null;
				return;
			}

			_particles.Play();
			_target = _ability.Target;
			_renderer.positionCount = 2;
		}

		private Vector2 SelfPos => _cached.position;
	}
}