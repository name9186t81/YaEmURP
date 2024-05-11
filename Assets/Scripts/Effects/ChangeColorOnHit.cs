using UnityEngine;

using YaEm.Core;
using YaEm.Health;

namespace YaEm.Effects
{
	public sealed class ChangeColorOnHit : MonoBehaviour, IActorComponent
	{
		[SerializeField] private SpriteRenderer _renderer;
		[SerializeField] private float _duration;
		private IActor _actor;
		private bool _active;
		private float _elapsed;

		public IActor Actor { set => _actor = value; }

		public void Init(IActor actor)
		{
			_actor = actor;

			if(_actor is IProvider<IHealth> prov)
			{
				prov.Value.OnDamage += Damaged;
				prov.Value.OnDeath += Death;
			}
		}

		private void Death(DamageArgs obj)
		{
			var health = (_actor as IProvider<IHealth>);
			health.Value.OnDeath -= Death;
			health.Value.OnDamage -= Damaged;
		}

		private void Update()
		{
			if (!_active) return;

			_elapsed += Time.deltaTime;
			if(_elapsed > _duration)
			{
				Color.RGBToHSV(_renderer.color, out float h, out _, out float v);
				Color c = Color.HSVToRGB(h, 1f, v);
				_renderer.color = c;
				_active = false;
			}
		}
		private void Damaged(DamageArgs obj)
		{
			_active = true;
			_elapsed = 0f;

			Color.RGBToHSV(_renderer.color, out float h, out _, out float v);
			Color c = Color.HSVToRGB(h, 0.01f, v);
			_renderer.color = c;
		}
	}
}