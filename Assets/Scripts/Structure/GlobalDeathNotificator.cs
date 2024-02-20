using YaEm.Health;
using YaEm.Core;

using System;

public sealed class GlobalDeathNotificator : IService
{
	public event Action<DamageArgs, IActor> OnDeath;

	public void Die(DamageArgs args, IActor actor)
	{
		OnDeath?.Invoke(args, actor);
	}
}
