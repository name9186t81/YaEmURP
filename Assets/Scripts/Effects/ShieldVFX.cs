using UnityEngine;

using YaEm.Ability;
using YaEm.Core;

namespace YaEm.Effects
{
	public sealed class ShieldVFX : MonoBehaviour, IActorComponent
	{
		[System.Serializable]
		private struct HitInfo
		{
			public Vector2 position;
			public float strength;
		}

		[SerializeField] private ParticleSystem _breakEffect;
		[SerializeField] private ParticleSystem _partlyRestoreEffect;
		[SerializeField] private SpriteRenderer _spriteRenderer;
		[SerializeField] private Shader _shieldShader;
		[SerializeField] private float _radius = 1.5f;
		[SerializeField] private float _maxShieldTake;
		private ShieldAbility _shield;
		private IActor _actor;
		private int _index = 0;
		private float _elapsedTime;
		private HitInfo[] _array = new HitInfo[8];
		private Material _mat;
		private ComputeBuffer _hitsBuffer;

		public IActor Actor { set => _actor = value; }

		public void Init(IActor actor)
		{
			_actor = actor;
			if(actor is not IProvider<IAbility> prov || prov.Value == null || prov.Value is not ShieldAbility abil)
			{
				Debug.LogError("Actor does not have ability or ability is not shield");
				return;
			}

			_radius *= actor.Scale * 2;
			_shield = abil;
			_shield.Value.OnDamage += ShieldDamaged;
			_shield.Value.OnDeath += ShieldDied;
			transform.localScale = Vector3.one * _radius;
			_mat = _spriteRenderer.material = new Material(_shieldShader);
			_hitsBuffer = new ComputeBuffer(8, sizeof(float) * 3);
		}

		private void ShieldDied(Health.DamageArgs obj)
		{
			if (_breakEffect != null) _breakEffect.Play();
		}

		private void Update()
		{
			_elapsedTime = Time.time;
		}

		private void ShieldDamaged(Health.DamageArgs obj)
		{
			if((obj.DamageFlags & Health.DamageFlags.Heal) != 0)
			{
				RestoreShield(obj);
				return;
			}

			float deltaDamage = obj.Damage / _shield.Value.MaxHealth;
			Vector2 point = _actor.Position.GetDirectionNormalized(obj.HitPosition);
			_array[_index] = new HitInfo()
			{
				position = point.Rotate(-_actor.Rotation),
				strength = -(deltaDamage * _maxShieldTake + _elapsedTime / 4)
			};

			_index++;
			_hitsBuffer.SetData(_array);
			_mat.SetBuffer("ar", _hitsBuffer);
			if (_index == 8) _index = 0;
		}

		private void OnDestroy()
		{
			_hitsBuffer.Release();
			_hitsBuffer = null;
		}

		private void RestoreShield(Health.DamageArgs obj)
		{
			if (_partlyRestoreEffect != null) _partlyRestoreEffect.Play();
		}
	}
}