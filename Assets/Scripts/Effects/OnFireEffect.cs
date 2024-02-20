using YaEm.Effects;
using YaEm.Core;
using YaEm.Health;
using UnityEngine;

public class OnFireEffect : IEffect<IHealth>
{
	private readonly float _lifeTime;
	private readonly int _damagePerTick;
	private readonly float _ticksPerSecond;
	private float _elapsedTotal;
	private float _elapsedTick;
	private IHealth _health;
	private IActor _owner;
	private DamageArgs _args;

	public OnFireEffect(IActor owner, int damage, float ticksPerSecond, float lifeTime)
	{
		_owner = owner;
		_damagePerTick = damage;
		_ticksPerSecond = ticksPerSecond;
		_lifeTime = lifeTime;

		_args = new DamageArgs(owner, damage, DamageFlags.Fire);
	}

	public EffectState State => _elapsedTotal > _lifeTime ? EffectState.Finished : EffectState.Running;

	public EffectType Type => EffectType.OnFire;

	public void ApplyEffect(in IHealth obj)
	{
		_health = obj;
	}

	public bool CanApply(in IHealth obj)
	{
		return !obj.Contains(EffectType.OnFire);
	}

	public void Update(float deltaTime)
	{
		if (State == EffectState.Finished) return;

		_elapsedTick += deltaTime;
		_elapsedTotal += deltaTime;

		if(_elapsedTick > _ticksPerSecond)
		{
			_args.HitPosition = _health.Actor.Position;
			_health.TakeDamage(_args);
			_elapsedTick = 0;
		}
	}
}
