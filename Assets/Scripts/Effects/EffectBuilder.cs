using YaEm.Core;
using YaEm.Effects;

public sealed class EffectBuilder<T>
{
	private float _duration;
	private float _ticksPerSecond;
	private int _damage;
	private IActor _owner;

	public EffectBuilder<T> SetDuration(float duration)
	{
		_duration = duration;
		return this;
	}

	public EffectBuilder<T> SetTicksPerSecond(float ticks)
	{
		_ticksPerSecond = ticks;
		return this;
	}

	public EffectBuilder<T> SetDamage(int damage)
	{
		_damage = damage;
		return this;
	}

	public EffectBuilder<T> SetOwner(IActor owner)
	{
		_owner = owner;
		return this;
	}

	public IEffect<T> Build(EffectType type)
	{
		switch(type)
		{
			case EffectType.OnFire:
			{
				return BuildOnFireEffect();
			}
			case EffectType.Invincible:
			{
				return BuildInvincibleEffect();
			}
		}
		throw new System.Exception("Invalid effect type");
	}

	private IEffect<T> BuildOnFireEffect()
	{
		return (IEffect<T>)new OnFireEffect(_owner, _damage, _ticksPerSecond, _duration);
	}
	private IEffect<T> BuildInvincibleEffect()
	{
		return (IEffect<T>)new Invincible(_duration);
	}
}
