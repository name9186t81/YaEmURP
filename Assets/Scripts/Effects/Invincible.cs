using YaEm.Health;

namespace YaEm.Effects
{
	public class Invincible : IEffect<IHealth>
	{
		private IHealth _health;
		private readonly float _duration;
		private float _elapsed;
		private EffectState _selfState;

		public Invincible(float duration)
		{
			_elapsed = _duration = duration;
		}

		public EffectState State => _selfState;

		public EffectType Type => EffectType.Invincible;

		public void ApplyEffect(in IHealth obj)
		{
			_health = obj;
			obj.Flags |= HealthFlags.Invincible;
			_selfState = EffectState.Running;
			_elapsed = _duration;
		}

		public bool CanApply(in IHealth obj)
		{
			return true;
		}

		public void Update(float deltaTime)
		{
			_elapsed -= deltaTime;

			if (_elapsed < 0)
			{
				_selfState = EffectState.Finished;
				_health.Flags &= ~HealthFlags.Invincible;
			}
		}

		public void Break()
		{
			_selfState = EffectState.Finished;
			_health.Flags &= ~HealthFlags.Invincible;
		}
	}
}
