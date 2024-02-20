using YaEm.Health;

namespace YaEm.Effects
{
	public class Invincible : IEffect<IHealth>
	{
		private IHealth _health;
		private float _duration;
		private EffectState _selfState;

		public Invincible(float duration)
		{
			_duration = duration;
		}

		public EffectState State => _selfState;

		public EffectType Type => EffectType.Invincible;

		public void ApplyEffect(in IHealth obj)
		{
			_health = obj;
			obj.Flags |= HealthFlags.Invincible;
			_selfState = EffectState.Running;
		}

		public bool CanApply(in IHealth obj)
		{
			return true;
		}

		public void Update(float deltaTime)
		{
			_duration -= deltaTime;

			if (_duration < 0)
			{
				_selfState = EffectState.Finished;
				//_health.Flags &= _health.Flags & !HealthFlags.Invincible;
			}
		}
	}
}
