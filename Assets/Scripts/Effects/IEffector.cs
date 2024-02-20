using System.Collections.Generic;

namespace YaEm.Effects
{
	public interface IEffector<T>
	{
		bool Contains(EffectType type);
		IReadOnlyList<IEffect<T>> Effects { get; }
		void UpdateEffector(float delta);
		bool AddEffect(IEffect<T> effect);
	}

	public interface IEffect<T>
	{
		bool CanApply(in T obj);
		EffectState State { get; }
		EffectType Type { get; }
		void ApplyEffect(in T obj);
		void Update(float deltaTime);
	}

	public enum EffectState
	{
		Running = 0,
		Finished = 1,
		Paused = 2
	}

	public enum EffectType
	{
		OnFire,
		Invincible
	}
}
